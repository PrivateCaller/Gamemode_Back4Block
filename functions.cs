// ============================================================
// 1. General
// ============================================================

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

function L4B_SaveVictim(%obj,%target)
{	
	
	if(isObject(getMinigameFromObject(%obj,%target)) && %target.getState() !$= "Dead" && !%obj.getState() !$= "Dead")
	if(%target.isBeingStrangled && !%obj.isBeingStrangled && !%obj.hIsInfected)
	{
		%target.isBeingStrangled = 0;
		%target.hEater.SmokerTongueTarget = 0;
		%obj.playthread(3,activate2);
		%target.playthread(3,plant);

		if(%target.isHoleBot)
		%target.resetHoleLoop();

		if(%target.getClassName() $= "Player" && %obj.getClassName() $= "Player" && $Pref::Server::L4B2Bots::VictimSavedMessages)
		{
			chatMessageTeam(%target.client,'fakedeathmessage',"<color:00FF00>" @ %obj.client.name SPC "<bitmapk:add-ons/package_left4block/icons/CI_VictimSaved>" SPC %target.client.name);
			%target.client.centerprint("<color:FFFFFF>You were saved by " @ %obj.client.name,5);
			%obj.client.centerprint("<color:FFFFFF>You saved " @ %target.client.name,5);

			if(isObject(%minigame = %obj.client.minigame))
			%minigame.L4B_PlaySound("victim_saved_sound");
		}

		if(%target.hEater.getDataBlock().getName() !$= "ZombieChargerHoleBot")
		L4B_SpecialsPinCheck(%target.hEater,%target);
		else return;
	}
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
	%obj.hAttackDamage = $Pref::Server::L4B2Bots::NormalsDamage;
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
// 5. Player
// ============================================================
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

// ============================================================
// 8. Event
// ============================================================
function fxDTSBrick::RandomizeZombieSpecial(%obj)
{
	%type[ %ntype++ ] = "ZombieChargerHoleBot";
	%type[ %ntype++ ] = "ZombieBoomerHoleBot";
	%type[ %ntype++ ] = "ZombieSpitterHoleBot";
	%type[ %ntype++ ] = "ZombieHunterHoleBot";
	%type[ %ntype++ ] = "ZombieSmokerHoleBot";
	%type[ %ntype++ ] = "ZombieJockeyHoleBot";
	%type = %type[ getRandom( 1, %ntype ) ];
	%obj.hBotType = %type;	
}
registerOutputEvent(fxDTSBrick, "RandomizeZombieSpecial");

function fxDTSBrick::RandomizeZombieUncommon(%obj)
{
	%type[ %ntype++ ] = "ZombieConstructionHoleBot";
	%type[ %ntype++ ] = "ZombieFallenHoleBot";
	%type[ %ntype++ ] = "ZombieCedaHoleBot";
	%type[ %ntype++ ] = "ZombieSoldierHoleBot";
	%type[ %ntype++ ] = "MudZombieHoleBot";
	%type[ %ntype++ ] = "ZombieClownHoleBot";
	%type[ %ntype++ ] = "ZombieJimmyHoleBot";

	//if($AddOn__Bot_Zombie_L4B2_EXT2 $= "1")
	//{
	//	%type[ %ntype++ ] = "ToxicZombieHoleBot";
	//	%type[ %ntype++ ] = "ZombieNaziHoleBot";
	//	%type[ %ntype++ ] = "ZombieSpaceHoleBot";
	//	%type[ %ntype++ ] = "ZombiePirateHoleBot";
	//	%type[ %ntype++ ] = "BurningZombieHoleBot";
	//	%type[ %ntype++ ] = "HeadcrabZombieHoleBot";
	//}
//
	//if($AddOn__Bot_SkeletonRev $= "1")
	//%type[ %ntype++ ] = "RandomSkeletonHoleBot";

	%type = %type[ getRandom( 1, %ntype ) ];
	%obj.hBotType = %type;

}
registerOutputEvent(fxDTSBrick, "RandomizeZombieUncommon");

function fxDTSBrick::enableZombieLoot( %obj, %on )
{
	if(isObject(%obj.hBot))
	%obj = %obj.hbot;

	if(!isObject(%obj))
	return;
	
	%obj.hZombieLoot = %on;
}
registerOutputEvent( fxDTSBrick, "enableZombieLoot", "bool" );

function fxDTSBrickData::onZombieTouch(%data,%obj,%player)
{
	$InputTarget_["Self"] = %obj;

	if(isObject(%player.client))
	{
	$InputTarget_["Player"] = %player;
	$InputTarget_["Client"] = %player.client;
	}

	if ( $Server::LAN )
	{
		$InputTarget_["MiniGame"] = getMiniGameFromObject (%player.client);
	}
	else if ( getMiniGameFromObject(%obj) == getMiniGameFromObject(%player.client) )
	{
		$InputTarget_["MiniGame"] = getMiniGameFromObject (%obj);
	}
	else
	{
		$InputTarget_["MiniGame"] = 0;
	}

	if ( !isObject(%player.client) )
	{
		$InputTarget_["Bot"] = %player;
	}
}
registerInputEvent ("fxDTSBrick", "onZombieTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB 
	"Client GameConnection" TAB "MiniGame MiniGame");

function fxDTSBrickData::onTankTouch(%data,%obj,%player)
{
	fxDTSBrickData::onZombieTouch(%data,%obj,%player);
}
registerInputEvent ("fxDTSBrick", "onTankTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB 
	"Client GameConnection" TAB "MiniGame MiniGame");


function centerprintcounter(%obj,%amount)
{
	%client = %obj.client;
	%per = %amount/4*100;
	%maxcounters = 20;
	%char = "|";for(%a =0; %a<%per/10; %a++){%fchar = %char @ %fchar;}
	centerprint(%client,"<just:center><color:00fa00>Get Up! <color:FFFFFF>: <just:left><color:FFFF00>" @ %fchar,1);
}

function L4B_ReviveDowned(%obj)
{
	if(%obj.getDatablock().getName() !$= "DownPlayerSurvivorArmor")
	{
		%eyeVec = %obj.getEyeVector();
		%startPos = %obj.getEyePoint();
		%endPos = VectorAdd(%startPos,vectorscale(%eyeVec,3));

		%mask = $TypeMasks::PlayerObjectType;
		%target = ContainerRayCast(%startPos, %endPos, %mask,%obj);
		if(%target)
		{
			if(%target.isdowned)
			{	
				if(%obj.savetimer < 4)
				{
					%obj.savetimer += 1;
					%target.isBeingSaved = %obj;
					if(%obj.issavior != 1)
					{
						%obj.issavior = 1;
						%obj.isSaving = %target;
						%obj.playthread(1,"armreadyright");
					}
					centerprintcounter(%obj,%obj.savetimer);
					centerprintcounter(%target,%obj.savetimer);
					%obj.savesched = schedule(1000,0,L4B_ReviveDowned,%obj);
				}

				if(%obj.savetimer == 4)
				{
					centerprintcounter(%obj,%obj.savetimer);
					%obj.savetimer = 0;
					%target.isdowned = 0;
					%obj.isSaving = 0;
					%target.lastdamage = getsimtime();
					%target.sethealth(25);

					%target.SetDataBlock("SurvivorPlayerLow");

					if(isObject(%target.billboard) && $L4B_hasSelectiveGhosting)
					Billboard_DeallocFromPlayer($L4B::Billboard_SO, %target);

					%target.playthread(0,root);
					%obj.playthread(1,root);
					
					cancel(%target.energydeath1);
					cancel(%target.energydeath2);
					cancel(%target.energydeath3);

					if(%target.getClassName() $= "Player")
					{
						%target.client.centerprint("<color:00fa00>You were saved by " @ %obj.client.name,5);
						chatMessageTeam(%target.client,'fakedeathmessage',"<color:00fa00>" @ %obj.client.name SPC "<bitmapk:add-ons/package_left4block/icons/CI_VictimSaved>" SPC %target.client.name);
						%target.client.play2d("victim_revived_sound");
						%obj.client.play2d("victim_revived_sound");
					}

					return;
				}
			}
		}
	}
}

datablock ParticleData(SecondaryMeleeFlashParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = -0.5;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 80;
	lifetimeVarianceMS   = 15;
	textureName          = "base/data/particles/star1";
	spinSpeed		= 10.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.6 0.6 0.1 0.9";
	colors[1]     = "0.6 0.6 0.6 0.0";
	sizes[0]      = 2.0;
	sizes[1]      = 3.0;

	useInvAlpha = false;
};
datablock ParticleData(SecondaryMeleeFlashSmallParticle : SecondaryMeleeFlashParticle)
{
	sizes[0]      = 1.0;
	sizes[1]      = 2.0;
};
datablock ParticleEmitterData(SecondaryMeleeFlashEmitter)
{
	ejectionPeriodMS = 3;
	periodVarianceMS = 0;
	ejectionVelocity = 1.0;
	velocityVariance = 1.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 90;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = SecondaryMeleeFlashParticle;
};

datablock ParticleEmitterData(SecondaryMeleeFlashSmallEmitter : SecondaryMeleeFlashEmitter)
{
	particles = SecondaryMeleeFlashSmallParticle;
};
datablock ExplosionData(SecondaryMeleeExplosion)
{
	soundProfile = "";
	lifeTimeMS = 150;

	particleEmitter = SecondaryMeleeFlashEmitter;
	particleDensity = 5;
	particleRadius = 0.2;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "5 5 5";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 1;
	camShakeRadius = 10.0;

	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0 0 0";
	lightEndColor = "0 0 0";
};
datablock ExplosionData(SecondaryMeleeSmallExplosion : SecondaryMeleeExplosion)
{
	camShakeFreq = "3 3 3";
	camShakeAmp = "0.6 0.6 0.6";
	particleEmitter = SecondaryMeleeFlashSmallEmitter;
};
datablock ProjectileData(SecondaryMeleeProjectile)
{
	lifeTime = 1;
	explodeOnDeath = true;
	explosion = SecondaryMeleeExplosion;
};
datablock ProjectileData(SecondaryMeleeSmallProjectile : SecondaryMeleeProjectile)
{
	explosion = SecondaryMeleeSmallExplosion;
};
AddDamageType("SecondaryMelee",'<bitmap:add-ons/package_left4block/icons/ci_punch> %1','%2 <bitmap:add-ons/package_left4block/icons/ci_punch> %1',0,1);

function serverCmdSecondaryMelee(%cl)
{
	%pl = %cl.player;
	if(!isObject(%pl) || %pl.meleeItem || %pl.hIsInfected || %pl.getDatablock().isdowned)
	return;

	if($Pref::SecondaryMelee::MeleeMode == 2 || ($Pref::SecondaryMelee::MeleeMode == 1 && isObject(%cl.minigame)))
	if(!%pl.disabledMelee)
	%pl.meleeTrigger();		
}
	
function player::meleeTrigger(%pl)
{
	if(%pl.getstate() $= "Dead" || %pl.meleeItem || %pl.hIsInfected || %pl.getDatablock().getName() $= "DownPlayerSurvivorArmor" || %pl.isBeingStrangled)
	if(%pl.disabledMelee)
	return;

	if(!isObject(%pl.getObjectMount()) && getSimTime()-%pl.lastMelee > 400-%pl.meleeCooldownRush && (%pl.meleeTick < $Pref::SecondaryMelee::MeleeCharges || $Pref::SecondaryMelee::MeleeCharges == -1))
	{
		%pl.lastMelee = getSimTime();
		%pl.meleeTick++;
		%hit = %pl.doMelee();
		%cd = (%hit == 0 ? 500 : 0);
		%pl.meleeCooldownRush = (%hit == 1 ? 50 : 0);
		if($Pref::SecondaryMelee::MeleeCharges != -1 && !isEventPending(%pl.meleeCdSched))
		%pl.meleeCdSched = %pl.schedule($Pref::SecondaryMelee::MeleeCooldownRate+%cd,meleeCooldown);
	}
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
	
function Player::doMelee(%obj)
{
	%obj.playThread(3,"activate2");
	serverPlay3D("melee_swing" @ getRandom(1,2) @ "_sound",%obj.getPosition());

	%pos = %obj.getEyePoint();
	%masks = $TypeMasks::StaticObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	initContainerRadiusSearch(%pos,5,%masks);
	while(%hit = containerSearchNext())
	{
      	if(%hit == %obj)
      	continue;
	
      	%len = 5 * getWord(%obj.getScale (), 2);
      	%vec = %obj.getEyeVector();
      	%beam = vectorScale(%vec,%len); //lengthened vector (for calculating the raycast's endpoint)
      	%end = vectorAdd(%pos,%beam); //calculated endpoint for raycast
      	%ray = containerRayCast(%pos,%end,%masks,%obj); //fire raycast
		
      	%line = vectorNormalize(vectorSub(%pos,%hit.getposition()));
		%dot = vectorDot(%vec,%line);

     	if(vectorDist(%pos,%hit.getposition()) > 3.75 || %dot > -0.4)
		continue;

		if(isObject(%ray))
        if(%ray.getType() & $TypeMasks::StaticObjectType || %ray.getType() & $TypeMasks::FxBrickObjectType || %ray.getType() & $TypeMasks::VehicleObjectType)
     	{
			serverPlay3D(%MeleeType @ "HitEnv_Sound",posFromRaycast(%ray));
			%p = new projectile()
			{
				datablock = "SecondaryMeleeSmallProjectile";
				initialPosition = posFromRaycast(%ray);
			};
			return;
     	}

		%obscure = containerRayCast(%obj.getEyePoint(),vectorAdd(%hit.getPosition(),"0 0 1.9"),$TypeMasks::VehicleObjectType | $TypeMasks::StaticObjectType | $TypeMasks::FxBrickObjectType, %obj);
		if(isObject(%obscure))
		continue;

		if(%hit.getType() & $TypeMasks::PlayerObjectType)
		{
			if(minigameCanDamage(%obj,%hit))
			{
				if(getSimTime()-%hit.punchedTime[%obj] > 2500)
				%hit.punched[%obj] = 0;

				%hit.punched[%obj]++;
				%hit.punchedTime[%obj] = getSimTime();
				if(%hit.punched[%obj] != 0 && %hit.punched[%obj]%3 == 0 && getSimTime()-%obj.lastSlap > 1500)
				{
					%slap = true;
					%obj.lastSlap = getSimTime();
				}

				if(%slap)//Do the good stuff
				{
					%stunchance = 75;
					%sound = "melee_slap_sound";
					%projectile = "SecondaryMeleeProjectile";
					%dmg = %hit.getdatablock().maxDamage/10;
					%hitknockback = 800;
					%hitz = 200;

					%obj.playThread(3,jump);
				}
				else
				{
					%stunchance = 20;
					%sound = "Zombie_hit" @ getrandom(1,8) @ "_sound";
					%projectile = "SecondaryMeleeSmallProjectile";
					%dmg = %hit.getdatablock().maxDamage/12;
					%hitknockback = 400;
					%hitz = 150;
				}
				if(%hit.getdatablock().getname() $= "ZombieTankHoleBot")
				%dmg = %hit.getdatablock().maxDamage/30; 

				if(getRandom(1,100) <= %stunchance && %hit.hZombieL4BType & %hit.hZombieL4BType < 4 )
				{
					if(%hit.isHoleBot)
					%hit.stopHoleLoop();
					
					%hit.emote(winStarProjectile, 1);
					L4B_SpazzZombieInitialize(%hit,1);
					%hit.mountImage(stunImage,2);
					schedule(1000,0,serverCmdSit,%hit);
				}


				serverPlay3D(%sound,%hit.getHackPosition());

				%hscale = %hit.getScale();
				%p = new projectile()
				{
					datablock = %projectile;
					initialPosition = %hit.getHackPosition();
				};

				%sVec = %hit.getForwardVector();
				%aimVec = %obj.getForwardVector();
				%reflect = (vectorDot(%sVec, %aimVec) < 0);

				if(%hit.getMountedImage(0) == RiotShieldimage.getID())
				{
					serverPlay3d("riotshield_hit_sound",%hit.getHackPosition());

					if(!%reflect)
					{
						%hit.applyimpulse(%hit.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),%hitknockback*$Pref::SecondaryMelee::MeleeForce),"0" SPC "0" SPC %hitz*$Pref::SecondaryMelee::MeleeForce));
						if(minigameCanDamage(%obj,%hit))
						%hit.damage(%obj,%hit.getPosition(),%dmg,$DamageType::SecondaryMelee);
					}
					else
					{
						%hitknockback = 200;
						%hitz = 75;
						%hit.applyimpulse(%hit.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),%hitknockback*$Pref::SecondaryMelee::MeleeForce),"0" SPC "0" SPC %hitz*$Pref::SecondaryMelee::MeleeForce));
						serverPlay3d("riotshield_block_sound",%obj.getPosition());
					}
				}
				else
				{
					%hit.applyimpulse(%hit.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),%hitknockback*$Pref::SecondaryMelee::MeleeForce),"0" SPC "0" SPC %hitz*$Pref::SecondaryMelee::MeleeForce));
					if(minigameCanDamage(%obj,%hit))
					%hit.damage(%obj,%hit.getPosition(),%dmg,$DamageType::SecondaryMelee);
				}
			}
		}
	}
}
	
function player::meleeCooldown(%pl)
{
	cancel(%pl.meleeCdSched);
	%pl.meleeTick--;
	if(!%pl.meleeTick)
		return;
	%pl.meleeCdSched = %pl.schedule(1000,meleeCooldown);
}

function Player::ToxifyHealth ( %Player, %amt )
{
	if ( %Player.getDamagePercent() >= 1.0 )
	{
		return;
	}

	if ( %amt > 0.0 && %Player.isToxic == 0)
	{
		%Player.setDamageLevel (%Player.getDamageLevel() - %amt);
		if(!%Player.Toxified)
		{
		   schedule(500,0,ToxicityE,%Player);
		   %Player.Toxified = 1;
		}
	}
	else if(%Player.isToxic == 0)
	{
		%Player.Damage (%Player.hFakeProjectile, %Player.getPosition(), %amt * -1, $DamageType::Default);
				if(!%Player.Toxified)
		{
		schedule(500,0,ToxicityE,%Player);
		%Player.Toxified = 1;
		}
	}
}
registerOutputEvent ("Player", "ToxifyHealth", "int -1000 1000 25");
registerOutputEvent ("Bot", "ToxifyHealth", "int -1000 1000 25");

function ToxicityE(%Player)
{
      if(isObject(%Player) && %Player.getState() !$= "Dead" && %Player.Toxified)//For bots/Players that aren't part of the team the obj is in   
      {
         if(%Player.ToxicityCount <= "16")
         {
            %Player.ToxicityCount++;
            %Player.ToxicESchedule = schedule(500,0,ToxicityE,%Player);
            %Player.mountImage(SpitAcidStatusPlayerImage, 3);

			%Player.damage(%Player, %Player.getposition(), 2, $DamageType::Toxic);

            if(%Player.hCanDistract)
            %Player.hRunAwayFromPlayer(%Player);

            else if(!isObject(%Player))
            return;
         }
         else
         {
            %Player.ToxicityCount = 0;
            if(isObject(%Player.getMountedImage(3)) && %Player.getMountedImage(3).getName() $= "SpitAcidStatusPlayerImage")
            %Player.unMountImage(3);

            %Player.Toxified = 0;
         }
      }
}

// ============================================================
// 12. Client Logger functions.
// ============================================================

function L4B_createClientSnapshot(%playerclient)
{
	echo("Storing new client:" SPC %playerclient.name);
	%clientObject = new ScriptObject()
	{
		name = %playerclient.name;
		blid = %playerclient.getBLID();

		accent =  %playerclient.accent;
		hat = %playerclient.hat;
		chest =  %playerclient.chest;
		decalName = %playerclient.decalName;
		pack =  %playerclient.pack;
		secondPack =  %playerclient.secondPack;	
		larm =  %playerclient.larm;	
		lhand =  %playerclient.lhand;	
		rarm =  %playerclient.rarm;
		rhand = %playerclient.rhand;
		hip =  %playerclient.hip;	
		lleg =  %playerclient.lleg;	
		rleg =  %playerclient.rleg;

		accentColor = %playerclient.accentColor;
		hatColor = %playerclient.hatColor;
		packColor =  %playerclient.packColor;
		secondPackColor =  %playerclient.secondPackColor;

		skinColor = getWord(%playerclient.headColor,0)/2.75 SPC getWord(%playerclient.headColor,1)/1.5 SPC getWord(%playerclient.headColor,2)/2.75 SPC 1;
	};
	if(%playerclient.chestColor $= %playerclient.headColor)
	{
		%clientObject.chestColor = %clientObject.skinColor;
	}
	else
	{
		%clientObject.chestColor = %playerclient.chestColor;
	}
	if(%playerclient.larmColor $= %playerclient.headColor)
	{
		%clientObject.larmColor = %clientObject.skinColor;
	}
	else
	{
		%clientObject.larmColor = %playerclient.larmColor;
	}
	if(%playerclient.rarmColor $= %playerclient.headColor)
	{
		%clientObject.rarmColor = %clientObject.skinColor;
	}
	else
	{
		%clientObject.rarmColor = %playerclient.rarmColor;
	}
	if(%playerclient.rhandColor $= %playerclient.headColor)
	{
		%clientObject.rhandColor = %clientObject.skinColor;
	}
	else
	{
		%clientObject.rhandColor = %playerclient.rhandColor;
	}
	if(%playerclient.lhandColor $= %playerclient.headColor)
	{
		%clientObject.lhandColor = %clientObject.skinColor;
	}
	else
	{
		%clientObject.lhandColor = %playerclient.larmColor;
	}
	if(%playerclient.hipColor $= %playerclient.headColor)
	{
		%clientObject.hipColor = %clientObject.skinColor;
	}
	else
	{
		%clientObject.hipColor = %playerclient.hipColor;
	}
	if(%playerclient.llegColor $= %playerclient.headColor)
	{
		%clientObject.llegColor = %clientObject.skinColor;
	}
	else
	{
		%clientObject.llegColor = %playerclient.llegColor;
	}
	if(%playerclient.rlegColor $= %playerclient.headColor)
	{
		%clientObject.rlegColor = %clientObject.skinColor;
	}
	else
	{
		%clientObject.rlegColor = %playerclient.rlegColor;
	}

	$L4B_clientLog.add(%clientObject);
	return %clientObject;
}

function L4B_storeLoggedClients()
{
	if(isFile("config/server/L4B2_Bots/loggedplayers.txt"))
	{
		%already_stored_clients = "";
		//First, check clients already stored in the file.
		%file = new fileObject();
		%file.openForRead("config/server/L4B2_Bots/loggedplayers.txt");
		while(!%file.isEOF())
		{
			%line = %file.readLine();
			for(%i = 0; %i < $L4B_clientLog.getCount(); %i++)
			{
				%client_blid = $L4B_clientLog.getObject(%i).blid;
				if(getField(%line, 1) $= %client_blid)
				{
					%already_stored_clients = %already_stored_clients @ %client_blid @ " "; 
				}
			}
		}
		%file.close();
		%file.delete();
		echo("Already stored clients:" SPC %already_stored_clients);
		//Next, append the file with the unstored clients.
		%file = new fileObject();
		%file.openForAppend("config/server/L4B2_Bots/loggedplayers.txt");
		%file.writeLine("" NL ""); //Skip down from the last line.
		for(%i = 0; %i < $L4B_clientLog.getCount(); %i++)
		{
			%client = $L4B_clientLog.getObject(%i);
			%is_stored = false;
			for(%i = 0; %i < getFieldCount(%already_stored_clients); %i++)
			{
				if(%client.blid $= getField(%already_stored_clients, %i))
				{
					%is_stored = true;
					break;
				}
			}
			if(%is_stored)
			{
				continue;
			}
			//Name (spaces replaces with !&! delimiter,) BLID, then a bunch of avatar information.
			%file.writeLine(%client.name TAB %client.blid TAB %client.accent TAB %client.hat TAB %client.chest TAB %client.decalName TAB %client.pack TAB %client.secondPack TAB %client.larm TAB %client.lhand TAB %client.rarm TAB %client.rhand TAB %client.hip TAB %client.lleg TAB %client.rleg TAB %client.accentColor TAB %client.hatColor TAB %client.packColor TAB %client.secondPackColor TAB %client.skinColor);
		}
		%file.close();
		%file.delete();
	}
	else
	{
		%file = new fileObject();
		%file.openForWrite("config/server/L4B2_Bots/loggedplayers.txt");
		for(%i = 0; %i < $L4B_clientLog.getCount(); %i++)
		{
			%client = $L4B_clientLog.getObject(%i);
			//Name, BLID, then a bunch of avatar information.
			%file.writeLine(%client.name TAB %client.blid TAB %client.accent TAB %client.hat TAB %client.chest TAB %client.decalName TAB %client.pack TAB %client.secondPack TAB %client.larm TAB %client.lhand TAB %client.rarm TAB %client.rhand TAB %client.hip TAB %client.lleg TAB %client.rleg TAB %client.accentColor TAB %client.hatColor TAB %client.packColor TAB %client.secondPackColor TAB %client.skinColor);
		}
		%file.close();
		%file.delete();
	}
}

function L4B_pushZombifiedStoredAppearance(%obj, %face)
{
	if(!isObject($L4B_clientLog) || $L4B_clientLog.getCount() < 1)
	{
		return false;
	}

	%sourceClient = $L4B_clientLog.getObject(getRandom(0, $L4B_clientLog.getCount() - 1));
	%obj.accentColor = %sourceClient.accentColor;
	%obj.accent = %sourceClient.accent;
	%obj.hatColor = %sourceClient.hatColor;
	%obj.hat = %sourceClient.hat;
	%obj.headColor = %sourceClient.skinColor;
	%obj.faceName = %face;
	%obj.chest =  %sourceClient.chest;
	%obj.decalName = %sourceClient.decalName;
	%obj.chestColor = %sourceClient.chestColor;
	%obj.pack = %sourceClient.pack;
	%obj.packColor = %sourceClient.packColor;
	%obj.secondPack = %sourceClient.secondPack;
	%obj.secondPackColor = %sourceClient.secondPackColor;
	%obj.larm = %sourceClient.larm;
	%obj.larmColor = %sourceClient.larmColor;
	%obj.lhand = %sourceClient.lhand;
	%obj.lhandColor = %sourceClient.lhandColor;
	%obj.rarm = %sourceClient.rarm;
	%obj.rarmColor = %sourceClient.rarmColor;
	%obj.rhandColor = %sourceClient.rhandColor;
	%obj.rhand = %sourceClient.rhand;
	%obj.hip = %sourceClient.hip;
	%obj.hipColor = %sourceClient.hipColor;
	%obj.lleg = %sourceClient.lleg;
	%obj.llegColor = %sourceClient.llegColor;
	%obj.rleg = %sourceClient.rleg;
	%obj.rlegColor = %sourceClient.rlegColor;
	%obj.vestColor = getRandomBotRGBColor();

	%obj.name = "Infected" SPC %sourceClient.name;
	%obj.setShapeNameHealth();

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}