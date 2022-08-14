// ============================================================
// 1. General
// ============================================================

function configLoadL4BTXT(%file,%svartype)//Set up custom variables
{
	%read = new FileObject();
	if(!isFile("config/server/Left4Block/" @ %file @ ".txt"))
	{
		%read.openForRead("add-ons/gamemode_left4block/script/variables/" @ %file @ ".txt");

		%write = new FileObject();
		%write.openForWrite("config/server/Left4Block/" @ %file @ ".txt");
	
		while(!%read.isEOF())
		{
			%line = %read.readLine();
			%write.writeLine(%line);
		}

		%write.close();
		%write.delete();
	}

	%read.openForRead("config/server/Left4Block/" @ %file @ ".txt");

	while(!%read.isEOF())
	{
		%i++;
		%line = %read.readLine(); 
		eval("$" @ %svartype @"[%i] = \"" @ %line @ "\";");
		eval("$" @ %svartype @"Amount = %i;");
	}
	
	%read.close();
	%read.delete();
}
configLoadL4BTXT("zombiefaces",hZombieFace);
configLoadL4BTXT("zombiedecals",hZombieDecal);
configLoadL4BTXT("zombieskin",hZombieSkin);
configLoadL4BTXT("zombiespecial",hZombieSpecialType);
configLoadL4BTXT("zombieuncommon",hZombieUncommonType);

function configLoadL4BItemTXT()//Set up custom variables
{
	%read = new FileObject();
	if(!isFile("config/server/Left4Block/items.txt"))
	{
		%read.openForRead("add-ons/gamemode_left4block/script/variables/items.txt");

		%write = new FileObject();
		%write.openForWrite("config/server/Left4Block/items.txt");
	
		while(!%read.isEOF())
		{
			%line = %read.readLine();
			%write.writeLine(%line);
		}

		%write.close();
		%write.delete();
	}

	%read.openForRead("config/server/Left4Block/items.txt");

	while(!%read.isEOF())
	{
		%i++;
		%line = %read.readLine(); 

		%itemremoveword = strreplace(%line, getWord(%line,0) @ " ", "");
		%previousline[%i] = getWord(%line,0);

		if(%previousline[%i] $= %previousline[mClamp(%i-1, 1, %i)])
		{
			%j++;

			eval("$" @ getWord(%line,0) @"[%j] = \"" @ %itemremoveword @ "\";");
			eval("$" @ getWord(%line,0) @"Amount = %j;");
		}
		else 
		{
			eval("$" @ getWord(%line,0) @"[1] = \"" @ %itemremoveword @ "\";");
			%j = 1;
		}

		for (%d = 0; %d < DatablockGroup.getCount(); %d++) 
		{
			%datablock = DatablockGroup.getObject(%d);

			if(%datablock.getClassName() $= "ItemData")
			if(strstr(%line, %datablock.uiName) != -1)
			{
				%item = %datablock;
				eval("$" @ getWord(%line,0) @"[%j] = \"" @ %item.getName() @ "\";");
			}
		}
	}

	
	%read.close();
	%read.delete();
}
configLoadL4BItemTXT();

registerInputEvent ("fxDTSBrick", "onZombieTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerInputEvent ("fxDTSBrick", "onSurvivorTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB "Client GameConnection" TAB "MiniGame MiniGame");

function fxDTSBrickData::onTankTouch(%data,%obj,%player)
{
	fxDTSBrickData::onZombieTouch(%data,%obj,%player);
}
registerInputEvent ("fxDTSBrick", "onTankTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerInputEvent ("fxDTSBrick", "onDoorClose", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerInputEvent ("fxDTSBrick", "onDoorOpen", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame");

function fxDTSBrick::RandomizeZombieSpecial(%obj)
{
	%obj.hBotType = $hZombieSpecialType[getRandom(1,$hZombieSpecialTypeAmount)];	
}
registerOutputEvent(fxDTSBrick, "RandomizeZombieSpecial");

function fxDTSBrick::RandomizeZombieUncommon(%obj)
{
	%obj.hBotType = $hZombieUncommonType[getRandom(1,$hZombieUncommonTypeAmount)];	
}
registerOutputEvent(fxDTSBrick, "RandomizeZombieUncommon");

function L4B_IsOnGround(%obj)
{
	%eyeVec = "0 0 -1";
	%startPos = %obj.getposition();
	%endPos = VectorAdd(%startPos,vectorscale(%eyeVec,1));
	%mask = $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType;
	%target = ContainerRayCast(%startPos, %endPos, %mask,%obj);

	if(%target)
	return true;
	else return false;
}

function L4B_IsOnWall(%obj)
{
	%eyeVec = vectorsub(%obj.getforwardvector(),vectorscale(%obj.getforwardvector(),2));
	%startPos = %obj.getposition();
	%endPos = VectorAdd(%startPos,vectorscale(%eyeVec,1));
	%mask = $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType;
	%target = ContainerRayCast(%startPos, %endPos, %mask,%obj);

	if(%target)
	return true;
	else return false;
}

//Ripped from Rotondo's holebot "hFOVCheck" function, then minimized.
function L4B_isInFOV(%viewer, %target)
{	
	return vectorDot(%viewer.getEyeVector(), vectorNormalize(vectorSub(%target.getPosition(), %viewer.getPosition()))) >= 0.7;
}

function L4B_isPlayerObstructed(%viewer, %target)
{
    //Check if there's anything blocking line-of-sight between the viewer and the target, then return the result.
    return ContainerRayCast(%viewer.getEyePoint(), %target.getHackPosition(), $TypeMasks::FxBrickObjectType | $TypeMasks::DebrisObjectType | $TypeMasks::InteriorObjectType, %viewer);
}

function L4B_DespaceString(%string)
{
	return strReplace(%string, " ", "!&!");
}

function L4B_RespaceString(%string)
{
	return strReplace(%string, "!&!", " ");
}

function fxDTSBrick::zfakeKillBrick(%obj)
{
	if(strstr(strlwr(%obj.getName()),"breakbrick") != -1)
	{
		%obj.fakeKillBrick("0 0 1", "5");
		%obj.schedule(5100,disappear,-1);

		if($oldTimescale $= "")
		$oldTimescale = getTimescale();
		setTimescale(getRandom(8,16)*0.1);
		%obj.playSound(BrickBreakSound.getID());
		setTimescale($oldTimescale);
	}
}
registerOutputEvent ("fxDTSBrick", "zfakeKillBrick");

// ============================================================
// 2. Zombies
// ============================================================

function Player::hChangeBotToInfectedAppearance(%obj)
{
	%this = %obj.getdataBlock();
	%obj.resetHoleLoop();

	if(!%this.hNeedsWeapons)
	%obj.setWeapon(-1);

	%obj.hNeutralAttackChance = %this.hNeutralAttackChance;
	%obj.hSearch = %this.hSearch;
	%obj.hSearchRadius = %this.hSearchRadius;
	%obj.hSight = %this.hSight;
	%obj.hSearchFov = %this.hSearchFov;
	%obj.hSuperStacker = %this.hSuperStacker;
	%obj.hAttackDamage = $L4B_NormalDamage;
	%obj.hMelee = %this.hMelee;

	for(%a = 0; %a <= $aCL; %a++)
	{
		%cur = $avatarColorLoop[%a];
		%newskincolor = getWord(%obj.headColor,0)/1.5 SPC getWord(%obj.headColor,1)/1.15 SPC getWord(%obj.headColor,2)/1.5 SPC 1;

		eval("%objC = %obj." @ %cur @ ";");
		if(%objC $= %obj.headColor)
		eval("%obj." @ %cur @ " = %newskincolor;");

		if($Pref::Server::L4B2Bots::CustomStyle < 2)
		%obj.faceName =  "asciiTerror";
		else %obj.faceName =  $hZombieFace[getRandom(1,$hZombieFaceAmount)];
	}

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function AIPlayer::hLimitedLifetime(%obj)
{
	if(!$Pref::Server::L4B2Bots::LimitedLifetime || !isObject(getMinigameFromObject(%obj)) || %obj.hFollowing)
	{
		%obj.hLimitLife = 0;
		return;
	}

	%obj.hLimitLife++;

	if(%obj.hLimitLife >= 25)
	%obj.kill();
}

function Player::onL4BDatablockAttributes(%obj)
{
	%this = %obj.getdataBlock();
	%obj.setDamageLevel(0);
	%obj.schedule(10,setenergylevel,0);

	%obj.hIsInfected = %this.hIsInfected;
	%obj.hZombieL4BType = %this.hZombieL4BType;
	%obj.hType = "Zombie";
	%obj.isStrangling = 0;
	%obj.hEating = 0;

	if(%obj.getClassName() $= "Player")
	{
		%obj.client.isInInfectedTeam = 1;
		commandToClient(%obj.client, 'SetVignette', true, "0.25 0.15 0 1" );

		if(%obj.getdataBlock().getName() $= "CommonZombieHoleBot")
		schedule(10,0,commandToClient,%obj.client, 'centerPrint', "<just:right><font:impact:30>\c6You are \c0" @ %obj.getdataBlock().hName @ "\c6! <br><font:impact:20>\c6Left click to attack or <br>\c6Plant brick key to change zombie types", 5);
		else schedule(10,0,commandToClient,%obj.client, 'centerPrint', "<just:right><font:impact:30>\c6You are a \c0" @ %obj.getdataBlock().hName @ "\c6! <br><font:impact:20>\c6" @ %obj.getdataBlock().SpecialCPMessage @ "<br>\c6Plant brick key to change zombie types", 5);
	}
	
	if(%this.hCustomNodeAppearance)
	%this.hCustomNodeAppearance(%obj);

	if(%obj.hZombieL4BType == 5)
	%obj.playaudio(3,strlwr(%obj.name) @ "_spawn" @ getRandom(1,2) @ "_sound");
}

$hZombieDecalDefault[%n = 1] = "AAA-None";
$hZombieDecalDefault[%n++] = "Mod-Army";
$hZombieDecalDefault[%n++] = "Mod-Police";
$hZombieDecalDefault[%n++] = "Mod-Suit";
$hZombieDecalDefault[%n++] = "Meme-Mongler";
$hZombieDecalDefault[%n++] = "Mod-Daredevil";
$hZombieDecalDefault[%n++] = "Mod-Pilot";
$hZombieDecalDefault[%n++] = "Mod-Prisoner";
$hZombieDecalDefault[%n++] = "Meme-Mongler";
$hZombieDecalDefault[%n++] = "Medieval-YARLY";
$hZombieDecalDefault[%n++] = "Medieval-ORLY";
$hZombieDecalDefault[%n++] = "Medieval-Eagle";
$hZombieDecalDefault[%n++] = "Medieval-Lion";
$hZombieDecalDefault[%n++] = "Medieval-Tunic";
$hZombieDecalDefault[%n++] = "Hoodie";
$hZombieDecalDefault[%n++] = "DrKleiner";
$hZombieDecalDefault[%n++] = "Chef";
$hZombieDecalDefault[%n++] = "worm-sweater";
$hZombieDecalDefault[%n++] = "worm_engineer";
$hZombieDecalDefault[%n++] = "Archer";
$hZombieDecalDefaultAmount = %n;

$hZombieHat[%c++] = 4;
$hZombieHat[%c++] = 6;
$hZombieHat[%c++] = 7;
$hZombieHat[%c++] = 0;
$hZombieHat[%c++] = 1;
$hZombieHatAmount = %c;

$hZombiePack[%d++] = 0;
$hZombiePack[%d++] = 2;
$hZombiePack[%d++] = 3;
$hZombiePack[%d++] = 4;
$hZombiePack[%d++] = 5;
$hZombiePackAmount = %d;

function Player::hDefaultL4BAppearance(%obj)
{	
	%this = %obj.getDataBlock();
	
	switch($Pref::Server::L4B2Bots::CustomStyle)//Random common appearance
	{
		case 0: %randmultiplier = getRandom(200,1000)*0.001;
				%randskin = $hZombieSkin[1];
				%skincolor = getWord(%randskin,0)*%randmultiplier SPC getWord(%randskin,1)*%randmultiplier SPC getWord(%randskin,2)*%randmultiplier SPC 1;

				%decal = $hZombieDecalDefault[getRandom(1,$hZombieDecalDefaultAmount)];
				%face = "asciiTerror";

		case 1:	%randmultiplier = getRandom(400,2000)*0.001;
				%randskin = $hZombieSkin[4];
				%skincolor = getWord(%randskin,0)*%randmultiplier SPC getWord(%randskin,1)*%randmultiplier SPC getWord(%randskin,2)*%randmultiplier SPC 1;

				%decal = $hZombieDecalDefault[getRandom(1,$hZombieDecalDefaultAmount)];
				%face = "asciiTerror";

		case 2: %randmultiplier = getRandom(400,2000)*0.001;
				%randskin = $hZombieSkin[4];
				%skincolor = getWord(%randskin,0)*%randmultiplier SPC getWord(%randskin,1)*%randmultiplier SPC getWord(%randskin,2)*%randmultiplier SPC 1;

				%decal = $hZombieDecal[getRandom(1,$hZombieDecalAmount)];		
				%face = $hZombieFace[getRandom(1,$hZombieFaceAmount)];

		case 3: %randmultiplier = getRandom(400,2000)*0.001;
				%randskin = $hZombieSkin[getRandom(2,$hZombieSkinAmount)];
				%skincolor = getWord(%randskin,0)*%randmultiplier SPC getWord(%randskin,1)*%randmultiplier SPC getWord(%randskin,2)*%randmultiplier SPC 1;

				%decal = $hZombieDecal[getRandom(1,$hZombieDecalAmount)];
				%face = $hZombieFace[getRandom(1,$hZombieFaceAmount)];
	}

	%chest = getRandom(0,1);
	%hat = $hZombieHat[getRandom(1,$hZombieHatAmount)];
	%pack = $hZombiePack[getRandom(1,$hZombiePackAmount)];

	if(%obj.hZombieL4BType == 1)
	{
		if(getRandom(1,4) == 1)
		%obj.hZombieL4BType = getrandom(2,3);
	}
	
	switch(%obj.hZombieL4BType)
	{
		case 1: %obj.getDataBlock().L4BCommonAppearance(%obj,%skinColor,%face,%decal,%hat,%pack,%chest);
		case 2: %obj.getDataBlock().L4BCommonFastAppearance(%obj,%skinColor,%face,%chest);
		case 3: %obj.getDataBlock().L4BCommonAppearance(%obj,%skinColor,%face,%decal,%hat,%pack,%chest);
		case 4: %obj.getDataBlock().L4BUncommonAppearance(%obj,%skinColor,%face,%decal,%hat,%pack,%chest);			
		case 5: %obj.getDataBlock().L4BSpecialAppearance(%obj,%skinColor,%face,%decal,%hat,%pack,%chest);
	}

	if(%obj.getDataBlock().hCustomNodeAppearance)
	%obj.getDataBlock().hCustomNodeAppearance(%obj);
}

function L4B_SpazzZombie(%obj,%count)
{	
	if(!isObject(%obj) || %obj.getstate() $= "Dead" || %count >= 15)
	return;

	if(!%obj.hLoopActive && %obj.lastheadshake+getrandom(250,750) < getsimtime())
	{
		%obj.playthread(0,undo);
		%obj.lastheadshake = getsimtime();
	}
	%obj.activateStuff();

	cancel(%obj.L4B_SpazzZombie);
	%time = getRandom(100,200);
	%obj.L4B_SpazzZombie = schedule(%time,0,L4B_SpazzZombie,%obj,%count+1);
}

function L4B_ZombieDropLoot(%obj,%lootitem,%chance)
{
	if(!isObject(%lootitem))
	return;
	else if(getRandom(1,100) <= %chance)
	{
		%loot = new item()
		{
			dataBlock = %lootitem;
			position = %obj.getHackPosition();
			dropped = 1;
			canPickup = 1;
			client = %cl;
			minigame = getMinigameFromObject(%obj);
		};
		missionCleanup.add(%loot);

		%loot.applyimpulse(%loot.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),getRandom(4/4,4)),getRandom(4*-1,4) @ " 0 " @ getRandom(6/3,6)));
		%loot.fadeSched = %loot.schedule(8000,fadeOut);
		%loot.delSched = %loot.schedule(8200,delete);
	}
}

function L4B_ZombieLunge(%obj,%targ,%power)
{
	if(!isObject(%obj) || !isObject(%targ) || !L4B_IsOnGround(%obj) || %obj.getState() $= "Dead")
	return;

	if(isObject(%obj.light))
	%obj.light.delete();

	%dis = VectorSub(%targ.getposition(),%obj.getposition());
	%normVec = VectorNormalize(vectoradd(%dis,"0 0" SPC 0.15*vectordist(%targ.getposition(),%obj.getposition())));
	%obj.playthread(0,jump);

	%eye = vectorscale(%normVec,2);
	%mp = %power;
	%final = vectorscale(%eye,%mp);
	%obj.setvelocity(%final);
}

function Player::bigZombieMelee(%obj)
{
	%this = %obj.getdataBlock();
	%oscale = getWord(%obj.getScale(),2);
	%mask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	initContainerRadiusSearch(%obj.getEyePoint(),10,%mask);
	while(%hit = containerSearchNext())
	{
		if(%hit == %obj)
		continue;

		%obscure = containerRayCast(%obj.getEyePoint(),vectorAdd(%hit.getPosition(),"0 0 1.9"),$TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::FxBrickObjectType, %obj);
		if(isObject(%obscure))
		continue;

		%line = vectorNormalize( vectorSub( %obj.getposition(), %hit.getposition()));
		%dot = vectorDot( %obj.getEyeVector(), %line );

		if(ContainerSearchCurrRadiusDist() < 2 && %dot < -0.5)
		{
			if(%hit.getType() & $TypeMasks::PlayerObjectType && checkHoleBotTeams(%obj,%hit))
			{
				if(%hit.getstate() $= "Dead")
				continue;

				%obj.playaudio(3,%this.hBigMeleeSound);
				%obj.playthread(1,"activate2");
				%hit.applyimpulse(%hit.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),2000),"0 0 500"));
				%hit.damage(%obj.hFakeProjectile, %hit.getposition(), $Pref::Server::L4B2Bots::SpecialsDamage*%oScale, %obj.hDamageType);

				%p = new Projectile()
				{
					dataBlock = "BigZombieHitProjectile";
					initialPosition = %hit.getPosition();
					sourceObject = %obj;
					client = %obj.client;
				};
				MissionCleanup.add(%p);
				%p.explode();

				%c = new Projectile()
				{
					dataBlock = "pushBroomProjectile";
					initialPosition = %hit.getPosition();
					sourceObject = %obj;
					client = %obj.client;
				};
				MissionCleanup.add(%c);
				%c.explode();
				%c.setScale("2 2 2");
			}
			if(%hit.getType() & $TypeMasks::VehicleObjectType)
			{
				%obj.playaudio(3,%this.hBigMeleeSound);
				%obj.playthread(2,"activate2");

				%muzzlepoint = vectorSub(%obj.getHackPosition(),"0 0 0.5");
				%muzzlevector = vectorScale(%obj.getEyeVector(),2.5);
				%muzzlepoint = VectorAdd (%muzzlepoint, %muzzlevector);
				%hit.setTransform (%muzzlepoint @ " " @ rotFromTransform(%hit.getTransform()));
				%impulse = VectorNormalize(VectorAdd (%obj.getEyeVector(), "0 0 0.2"));
				%force = %hit.getDataBlock ().mass * 25;
				%scaleRatio = getWord (%obj.getScale (), 2) / getWord (%hit.getScale (), 2);
				%force *= %scaleRatio;
				%impulse = VectorScale (%impulse, %force);
				%hit.schedule(50,applyImpulse,%hit.getPosition(),%impulse);
				%hit.damage(%obj.hFakeProjectile, %hit.getposition(), 50*getWord(%obj.getScale(),0), $DamageType::Tank);

				%c = new Projectile()
				{
					dataBlock = "pushBroomProjectile";
					initialPosition = %hit.getPosition();
					sourceObject = %obj;
					client = %obj.client;
				};
				MissionCleanup.add(%c);
				%c.explode();
				%c.setScale("2 2 2");
			}
		}
	}
}

function Player::PlayerZombieMeleeAttack(%obj,%col)
{
	if(%obj.getState() $= "Dead")
	return;

	if(%col.getType() & $TypeMasks::VehicleObjectType || %col.getType() & $TypeMasks::PlayerObjectType && %obj.lasthit+250 < getsimtime())
	{
		%obj.lasthit = getsimtime();

		if(%obj.getdataBlock().getName() !$= "ZombieChargerHoleBot")
		{
			%damage = 5*getWord(%obj.getScale(),0);
			%damage = %damage*1.5;
			%damagefinal = getRandom(%damage/2,%damage);

			%col.damage(%obj.hFakeProjectile, %col.getposition(), %damagefinal, %obj.hDamageType);
			%meleeimpulse = mClamp(%damagefinal, 1, 7.5);

			if(%obj.getDataBlock().getName() !$= "ZombieHunterHoleBot")
			%obj.playaudio(1,"zombie_hit" @ getrandom(1,8) @ "_sound");
			else %obj.playaudio(2,"hunter_hit" @ getrandom(1,3) @ "_sound");

			%p = new Projectile()
			{
				dataBlock = "ZombieHitProjectile";
				initialPosition = %col.getPosition();
				sourceObject = %obj;
				client = %obj.client;
			};
			MissionCleanup.add(%p);
			%p.explode();

			%col.applyimpulse(%col.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),getrandom(100,100*%meleeimpulse)),"0" SPC "0" SPC getrandom(100,100*%meleeimpulse)));
			%col.playthread(3,"plant");
			%obj.playthread(1,"activate2");
			%obj.playthread(2,"jump");

			if(%col.getType() & $TypeMasks::PlayerObjectType && %col.getState() !$= "Dead" && %col.getDamageLevel() >= %col.getDataBlock().maxDamage/1.333 && !%col.hIsInfected && !%col.hIsImmune)
			holeZombieInfect(%obj,%col);
		}
		else %obj.bigZombieMelee();
	}
}

// ============================================================
// 3. Minigame
// ============================================================
function MinigameSO::L4B_PlaySound(%minigame,%sound,%client)
{
    for(%i=0;%i<%minigame.numMembers;%i++)
    {
        %cl=%minigame.member[%i];

        if(isObject(%cl) && %cl.getClassName() $= "GameConnection")
        %cl.play2d(%sound.getID());
    }
}

function L4B_CheckifinMinigame(%target1,%target2)
{
	if(isObject(getMinigameFromObject(%target1,%target2)) && miniGameCanDamage(%target1,%target2))
	return true;
}

// ============================================================
// 6. Specials
// ============================================================

function Player::SpecialPinAttack(%obj,%col,%force)
{	
	if(!isObject(%col) || !isObject(%obj))
	return;

	if(!isObject(%col.billboard) && $L4B_hasSelectiveGhosting)
	{
		Billboard_MountToPlayer(%col, $L4B::Billboard_SO, strangledBillboard);
	}

	if(%col.getType() & $TypeMasks::PlayerObjectType && checkHoleBotTeams(%obj,%col))
	{	
		%shape = %col.getDataBlock().shapeFile;
		if(L4B_CheckifinMinigame(%obj,%col) && %obj.getState() !$= "Dead" && %col.getState() !$= "Dead" && !%obj.isStrangling && !%col.isBeingStrangled && %obj.laststun+5000 < getsimtime() && %shape $= "base/data/shapes/player/m.dts" || %shape $= "base/data/shapes/player/mmelee.dts")
		{
			%obj.laststun = getsimtime();
			%col.isBeingStrangled = 1;
			%obj.isStrangling = 1;
			%obj.hEating = %col;
			%col.hEater = %obj;

			if(%obj.getClassName() $= "AIPlayer")
			{
				%obj.schedule(100,hClearMovement);
				%obj.stopHoleLoop();
				%obj.hIgnore = %col;
			}

			switch$(%col.getclassname())
			{
				case "Player":	if($Pref::Server::L4B2Bots::MinigameMessages)
								{
									chatMessageTeam(%col.client,'fakedeathmessage',"<color:FFFF00>" @ %obj.getDatablock().hName SPC %obj.getdataBlock().hPinCI SPC %col.client.name);
									%col.client.minigame.L4B_PlaySound("victim_needshelp_sound");
								}
								//NeedHelp_Cutscene(%col.client, strangledBillboard);

								%col.client.camera.setOrbitMode(%col, %col.getTransform(), 0, 5, 0, 1);
								%col.client.setControlObject(%col.client.camera);
								ServerCmdUnUseTool (%target.client);

				case "AIPlayer": %col.stopHoleLoop();
			}

			switch$(%obj.getdataBlock().getName()) 
			{
				case "ZombieChargerHoleBot": %obj.mountObject(%col,0);
											 %obj.playthread(1,"root");
											 %obj.hSharkEatDelay = schedule(2000,0,L4B_holeChargerKill,%obj,%col);
											 %forcedam = %force/2;
											 %col.damage(%obj.hFakeProjectile, %col.getposition(),%forcedam, %obj.hDamageType);

											%p = new Projectile()
											{
												dataBlock = "BigZombieHitProjectile";
												initialPosition = %col.getPosition();
												sourceObject = %obj;
												client = %obj.client;
											};
											MissionCleanup.add(%p);
											%p.explode();

				case "ZombieHunterHoleBot": %obj.playthread(0,root);
											%col.playthread(0,death1);

											%phackloc = %col.getHackPosition();
											%obj.schedule(10,setvelocity,"0 0 0");
											%obj.schedule(5,setTransform,%phackloc SPC %phackloc);

											%obj.HunterHurt = schedule(100,0,L4B_holeHunterKill,%obj,%col);

											%forcedam = %force/2;
											%col.damage(%obj.hFakeProjectile, %col.getposition(),%forcedam, %obj.hDamageType);

				case "ZombieJockeyHoleBot":	%col.mountObject(%obj,2);
											%obj.setControlObject(%col);

											%obj.playthread(0,sit);
											%obj.playthread(1,armreadyboth);
											%obj.playaudio(0,"jockey_attack_loop" @ getrandom(1,2) @ "_sound");

											%obj.JockeyHurt = schedule(1000,0,L4B_holeJockeyKill,%obj,%col);

				case "ZombieSmokerHoleBot":  %obj.playaudio(1,"smoker_launch_tongue_sound");
											 %col.playaudio(2,"smoker_tongue_hit_sound");
											 %obj.playthread(2,"plant");
											 %obj.playthread(3,"shiftup");
											 %col.mountImage(ZombieSmokerConstrictImage, 2);
											 %obj.getdataBlock().SmokerTongueLoop(%obj,%col);
			}
		}
	}
}

function L4B_SpecialsPinCheck(%obj,%col)
{
	if(!L4B_CheckifinMinigame(%obj,%col) || !isObject(%obj) || !isObject(%col) || %col.getState() $= "Dead" || !%col.isBeingStrangled || %col.hIsInfected ||%obj.getstate() $= "Dead")
	{
		if(isObject(%obj))
		{
			%obj.isStrangling = 0;
	
			if(isObject(%obj.light))
			%obj.light.delete();			
			%obj.stopAudio(0);

			%obj.hIgnore = 0;
			%obj.hEating = 0;
	
			if(%obj.getdataBlock().getName() $= "ZombieJockeyHoleBot")
			{
				%obj.dismount();
				%obj.setControlObject(%obj);
			}

			if(%obj.getState() !$= "Dead")
			{
				%obj.setDamageLevel(0);
				%obj.playThread(1,root);
				%obj.emote(HealImage,3);

				if(%obj.getClassName() $= "AIPlayer")
				{
					if(isObject(%col))
					%obj.hRunAwayFromPlayer(%col);
					else %obj.hRunAwayFromPlayer(%obj);
					%obj.schedule(4000,resetHoleLoop);
				}
			}
		}

		if(isObject(%col))
		{
			%col.isBeingStrangled = 0;

			if(isObject(%col.getMountedImage(2)) && %col.getMountedImage(2).getID() == ZombieSmokerConstrictImage.getID())
			%col.unMountImage(2);

			if(isObject(%col.billboard) && $L4B_hasSelectiveGhosting)
			Billboard_DeallocFromPlayer($L4B::Billboard_SO, %col);

			if(%col.getstate() !$= "Dead")
			{
				if(isObject(%col.client))
				%col.client.setControlObject(%col);
				else %col.setControlObject(%col);
				%col.playthread(0,root);

				if(%col.getClassName() $= "AIPlayer")
				%col.resetHoleLoop();
			}
		}
		return 0;
	}
	return 1;
}