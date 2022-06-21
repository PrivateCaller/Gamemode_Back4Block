// ============================================================
// 1. General
// ============================================================

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

function Player::MeleePlayerBloodify(%obj,%percent)
{
	if(getRandom(1,100) <= %percent)
	{
		%obj.unhidenode(LShoe_blood);
		%obj.bloody["lshoe"] = true;
	}
	if(getRandom(1,100) <= %percent)
	{
		%obj.unhidenode(Rshoe_blood);
		%obj.bloody["rshoe"] = true;
	}
	if(getRandom(1,100) <= %percent)
	{
		%obj.unhidenode(Lhand_blood);
		%obj.bloody["lhand"] = true;
	}
	if(getRandom(1,100) <= %percent)
	{
		%obj.unhidenode(Rhand_blood);
		%obj.bloody["rhand"] = true;
	}
	if(getRandom(1,100) <= %percent)
	{
		if(%obj.chest $= 0)
		{
			%obj.unhidenode(chest_blood_front);
			%obj.unhidenode(chest_blood_back);
		}
		else
		{
			%obj.unhidenode(femchest_blood_front);
			%obj.unhidenode(femchest_blood_back);
		}
		%obj.bloody["chest_front"] = true;
		%obj.bloody["chest_back"] = true;
		%obj.bloody["chest_lside"] = true;
		%obj.bloody["chest_rside"] = true;
	}
}

function Player::setShapeNameHealth(%obj)
{
	if(!isObject(%obj))
	return;

	if(%obj.getState() $= "Dead")
	{
		%obj.setShapeName("", 8564862);
		return;
	}

	if(%obj.getClassName() $= "Player")
	%name = %obj.name SPC %obj.client.name;
	else %name = %obj.name;
	
	%obj.setShapeName(%name SPC "|" SPC %obj.getDataBlock().MaxDamage-mFloatLength(%obj.getDamageLevel(), 0), 8564862);
	%obj.setShapeNameColor("1 0 0");
	%obj.setShapeNameDistance(%obj.getdataBlock().ShapeNameDistance);
}

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
	if(%obj.hLimitLife >= "20")
	%obj.kill();
}

function Player::onL4BDatablockAttributes(%obj)
{
	%this = %obj.getdataBlock();
	%obj.setDamageLevel(0);

	if(!%this.hNeedsWeapons)
	{
		%obj.unMountImage(0);
		%obj.unMountImage(1);
	}
	cancel(%obj.energydeath1);
	cancel(%obj.energydeath2);
	cancel(%obj.energydeath3);

	%obj.schedule(10,setenergylevel,0);

	//hZombieL4BType is another variable used for some specific functions
	%obj.hIsInfected = %this.hIsInfected;//1 = common, 2 = fast, 3 = slow 4 = uncommon, 5 = special
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
	L4B_SpecialsSpawnMusic(%obj);

	if(%obj.hZombieL4BType && %obj.hZombieL4BType < 5)
	%obj.name = "Infected" SPC %obj.name;

	%obj.setShapeNameHealth();
}

function Player::hDefaultL4BAppearance(%obj) //This is how the appearances are determined on the datablock's onAdd function
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
		if(getRandom(1,100) <= 25)
		%obj.hZombieL4BType = getrandom(2,3);
	}

	%obj.MeleePlayerBloodify(25);

	if(%obj.hZombieL4BType && %obj.hZombieL4BType < 5)
	switch(%obj.chest)
	{
		case 0: %obj.name = %this.name SPC $hZombieNameMale[getRandom(0,$hZombieNameMaleAmount)];
		case 1: %obj.name = %this.name SPC $hZombieNameFemale[getRandom(0,$hZombieNameFemaleAmount)];
	}

	switch(%obj.hZombieL4BType)
	{
		case 1: %obj.getDataBlock().L4BCommonAppearance(%obj,%skinColor,%face,%decal,%hat,%pack,%chest);
				L4B_CommonZombieAttributes(%obj);

		case 2: %obj.getDataBlock().L4BCommonFastAppearance(%obj,%skinColor,%face,%chest);
				L4B_CommonZombieAttributes(%obj);

		case 3: %obj.getDataBlock().L4BCommonAppearance(%obj,%skinColor,%face,%decal,%hat,%pack,%chest);
				L4B_CommonZombieAttributes(%obj);

		case 4: %obj.getDataBlock().L4BUncommonAppearance(%obj,%skinColor,%face,%decal,%hat,%pack,%chest);
				L4B_CommonZombieAttributes(%obj);
				
		case 5: %obj.getDataBlock().L4BSpecialAppearance(%obj,%skinColor,%face,%decal,%hat,%pack,%chest);
	}

	if(%obj.getDataBlock().hCustomNodeAppearance)
	%obj.getDataBlock().hCustomNodeAppearance(%obj);

	%obj.setShapeNameHealth();
}

function L4B_SpazzZombieInitialize(%obj,%count)
{
	if(!isObject(%obj) || %obj.hLoopActive || %obj.isBurning || %obj.getstate() $= "Dead" || %obj.SpazzOff >= 5)
	{
		if(isObject(%obj))
		{
			if(%obj.getclassname() $= "AIPlayer")
			%obj.startHoleLoop();

			%obj.unMountImage(1);
			%obj.unMountImage(2);
			%obj.SpazzOff = 0;
		}
		return;
	}

	%obj.MaxSpazzClick = getrandom(8,16);

	if(%obj.getClassName() $= "Player")
	%time = 1000;
	else %time = getRandom(5000,8000);
	cancel(%obj.L4B_SpazzZombieInitialize);
	%obj.L4B_SpazzZombieInitialize = schedule(%time,0,L4B_SpazzZombieInitialize,%obj,%count+1);
	%obj.L4B_SpazzZombie = schedule(%time,0,L4B_SpazzZombie,%obj,0);

	%obj.raisearms = 0;
	%obj.playthread(1,"root");
	%obj.emote(winStarProjectile, 1);
	%obj.mountImage(stunImage,2);
	%obj.SpazzOff++;
	
	if(%obj.getclassname() $= "AIPlayer")
	{
		%obj.stopHoleLoop();
		%obj.hClearMovement();
		serverCmdSit(%obj);
	}
}

function L4B_SpazzZombie(%obj,%count)
{
	if(!isObject(%obj) || %obj.isBurning || %obj.getstate() $= "Dead" || %count >= %obj.MaxSpazzClick)
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

function L4B_CommonZombieAttributes(%obj)
{
	if($Pref::Server::L4B2Bots::ZombieRandomScale && %obj.hZombieL4BType && %obj.hZombieL4BType < 5)
	{
		%randscale = getRandom(85,115)*0.01 SPC getRandom(85,115)*0.01 SPC getRandom(85,115)*0.01;
		%obj.setscale(%randscale);
	}

	if(getRandom(0,100) <= 5)
	%obj.MountImage(MxRockImage,0);
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

function AIPlayer::hZombieBotToPlayerApearance(%obj,%playerclient)
{
	%obj.name = %obj.getDataBlock().name SPC %playerclient.name;

	%skin = %playerclient.headColor;
	%zskin = getWord(%skin,0)/2.75 SPC getWord(%skin,1)/1.5 SPC getWord(%skin,2)/2.75 SPC 1;
	%obj.headColor = %zskin;

	%obj.accent =  %playerclient.accent;
	%obj.hat = %playerclient.hat;
	%obj.chest =  %playerclient.chest;
	%obj.decalName = %playerclient.decalName;
	%obj.pack =  %playerclient.pack;
	%obj.secondPack =  %playerclient.secondPack;	
	%obj.larm =  %playerclient.larm;	
	%obj.lhand =  %playerclient.lhand;	
	%obj.rarm =  %playerclient.rarm;
	%obj.rhand = %playerclient.rhand;
	%obj.hip =  %playerclient.hip;	
	%obj.lleg =  %playerclient.lleg;	
	%obj.rleg =  %playerclient.rleg;

	%obj.accentColor = %playerclient.accentColor;
	%obj.hatColor = %playerclient.hatColor;
	%obj.packColor =  %playerclient.packColor;
	%obj.secondPackColor =  %playerclient.secondPackColor;

	%obj.chestColor = %playerclient.chestColor;
	%obj.larmColor = %playerclient.larmColor;
	%obj.rarmColor = %playerclient.rarmColor;
	%obj.rhandColor = %playerclient.rhandColor;
	%obj.lhandColor = %playerclient.lhandColor;
	%obj.hipColor = %playerclient.hipColor;
	%obj.llegColor = %playerclient.llegColor;
	%obj.rlegColor = %playerclient.rlegColor;

	if(%playerclient.chestColor $= %skin)
	%obj.chestColor = %zskin;

	if(%playerclient.larmColor $= %skin)
	%obj.larmColor = %zskin;

	if(%playerclient.rarmColor $= %skin)
	%obj.rarmColor = %zskin;

	if(%playerclient.rhandColor $= %skin)
	%obj.rhandColor = %zskin;

	if(%playerclient.lhandColor $= %skin)
	%obj.lhandColor = %zskin;

	if(%playerclient.hipColor $= %skin)
	%obj.hipColor = %zskin;

	if(%playerclient.llegColor $= %skin)
	%obj.llegColor = %zskin;

	if(%playerclient.rlegColor $= %skin)
	%obj.rlegColor = %zskin;	
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

function Player::PlayerZombieMeleeAttack(%obj,%col)
{
	if(%obj.getState() $= "Dead")
	return;

	if(isEventPending(%obj.L4B_SpazzZombieInitialize))
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
// 4. Projectile
// ============================================================
function Projectile::PipeBombDistract(%obj, %flashcount)
{ 
	%pos = %obj.getPosition();
	%radius = 100;
	%searchMasks = $TypeMasks::PlayerObjectType;
	InitContainerRadiusSearch(%pos, %radius, %searchMasks);

	while ((%targetid = containerSearchNext()) != 0 )
	{
		if(!%targetid.getState() !$= "Dead" && %targetid.getClassName() $= "AIPlayer" && %targetid.hZombieL4BType && %targetid.hZombieL4BType < 5 && !%targetid.isBurning)
		{
			if(%flashcount < 15)
			{
				if(!%targetid.Distraction)
				{
					%targetid.Distraction = %obj.getID();
					%targetid.hSearch = 0;
				}

				else if(%targetid.Distraction $= %obj.getID())
				{
					%targetid.setmoveobject(%obj);
					%targetid.setaimobject(%obj);
					%time1 = getRandom(1000,4000);
					%targetid.getDataBlock().schedule(%time1,onBotFollow,%targetid,%obj);
				}

			}
			else
			{
				%targetid.hSearch = 1;
				%targetid.Distraction = 0;
			}
		}
	}

	if(%flashcount < 4)
	{
		%sound = Phone_Tone_1_Sound;
		%time = 750;
	}
	else if(%flashcount < 8)
	{
		%sound = Phone_Tone_3_Sound;
		%time = 500;
	}
	else if(%flashcount < 12)
	{
		%sound = Phone_Tone_4_Sound;
		%time = 375;
	}
	else if(%flashcount < 16)
	{
		%sound = Phone_Tone_4_Sound;
		%time = 250;
	}
	else if(%flashcount > 16)
	{
		%obj.explode();
		return;
	}

	%obj.schedule(%time,PipeBombDistract,%flashcount+1);
	serverPlay3d(%sound,%pos);

	%p = new Projectile()
	{
		dataBlock = sPipeBombLightProjectile;
		initialPosition = %pos;
		initialVelocity = "0 0 1";
		sourceObject = %obj.sourceObject;
		client = %obj.client;
		sourceSlot = 0;
		originPoint = %obj.originPoint;
	};

	if(isObject(%p))
	{
		MissionCleanup.add(%p);
		%p.setScale(%obj.getScale());
	}
}

function Projectile::BileBombDistract(%obj, %count)
{
	if(isObject(%obj) || %count < %obj.getDatablock().distractionLifetime)
	%obj.ContinueSearch = %obj.schedule(1000,BileBombDistract,%count+1);

	%pos = %obj.getPosition();
	%radius = %obj.getDatablock().DistractionRadius;

	%searchMasks = $TypeMasks::PlayerObjectType;
	InitContainerRadiusSearch(%pos, %radius, %searchMasks);

	while((%targetid = containerSearchNext()) != 0 )
	{
		if(!%targetid.getState() !$= "Dead" && %targetid.getClassName() $= "AIPlayer" && %targetid.hZombieL4BType && %targetid.hZombieL4BType < 5 && !%targetid.isBurning)
		{
			if(%count < %obj.getDatablock().distractionLifetime)
			{
				if(!%targetid.Distraction)
				{
					%targetid.Distraction = %obj.getID();
					%targetid.hSearch = 0;
				}
				else if(%targetid.Distraction == %obj.getID())
				{
					%targetid.setaimobject(%obj);
					%targetid.setmoveobject(%obj);
					%time1 = getRandom(1000,4000);
					%targetid.getDataBlock().schedule(%time1,onBotFollow,%targetid,%obj);
				}

				if(%obj.getdataBlock().getID() == BileBombFakeProjectile.getID() && ContainerSearchCurrRadiusDist() <= 4)
				{
					if(%targetid.hType $= "zombie")
					{
						%targetid.hType = "biled" @ getRandom(1,9999);
						%targetid.mountImage(BileStatusPlayerImage, 2);
						%targetid.BoomerBiled = 1;
					}
				}
			}
			else
			{
				%targetid.hSearch = 1;
				%targetid.Distraction = 0;
			}
		}
	}
}

function Projectile::SmokerExplosionCoughEffect(%obj)
{
	if(!isObject(%obj))
	return;

	%pos = %obj.getPosition();
	%radius = 5;
	%searchMasks = $TypeMasks::PlayerObjectType;
	InitContainerRadiusSearch(%pos, %radius, %searchMasks);
	while((%target = containerSearchNext()) != 0 )
	{
		if(%target.hZombieL4BType && %target.getstate() !$= "Dead")
		{
			%randomtime = getRandom(400,800);
			%target.schedule(%randomtime,playaudio,2,"norm_cough" @ getrandom(1,3) @ "_sound");
			%target.schedule(%randomtime,playthread,3,"plant");
		}
	}
	%obj.schedule(750,L4B_SmokerCoughEffect,%obj);
}

// ============================================================
// 6. Specials
// ============================================================
function L4B_SpecialsSpawnMusic(%obj)
{
	if($Pref::Server::L4B2Bots::SpecialsCue && isObject(getMinigameFromObject(%obj)))
	{
		%special = strlwr(%obj.getDataBlock().name);
		%obj.playaudio(3,%special @ "_spawn" @ getRandom(1,2) @ "_sound");
	}
}

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

											 //face away from the Smoker
											// %col.schedule(750, "setTransform", %col.getPosition() SPC %obj.getForwardVector());
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

function L4B_SpecialsWarningLight(%obj)
{
	if(!$Pref::Server::L4B2Bots::SpecialsWarnLight)
	return;
	else if(isObject(%obj) && %obj.getState() !$= "Dead")//If you suddenly remove the bot, there'll be console spam
	{
		if(!isObject(%obj.light))
		{		
			%pref = $Pref::Server::L4B2Bots::SpecialsWarnLight;
			switch(%pref)
			{
				case 1: %light = "RedLight";
				case 2: %light = "GreenLight";
				case 3: %light = "BlueLight";
				case 4: %light = "PlayerLight";
			}

			%obj.light = new fxlight()
			{
				dataBlock = %light;
				enable = true;
				iconsize = 1;
				player = %obj;
			};
			%obj.light.attachToObject(%obj);
		}
	}
}