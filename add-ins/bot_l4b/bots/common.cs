function BrickCommonZombie_HoleSpawnData::onPlant(%this, %obj)
{
	if(!isObject(directorBricks))
    {
        new SimSet(directorBricks);
        directorBricks.add(%obj);
        MissionCleanup.add(directorBricks);
    }
    else if(isObject(directorBricks))
    directorBricks.add(%obj);

	Parent::onPlant(%this,%obj);
}

function BrickCommonZombie_HoleSpawnData::onloadPlant(%this, %obj)
{
	BrickCommonZombie_HoleSpawnData::onPlant(%this,%obj);
}

datablock PlayerData(CommonZombieHoleBot : PlayerMeleeAnims)
{
	canJet = false;
	jumpForce = 9.5*100;
	minImpactSpeed = 32;
	airControl = 0.1;
	speedDamageScale = 2.5;

    maxForwardSpeed = 7;
    maxBackwardSpeed = 6;
    maxSideSpeed = 5;

 	maxForwardCrouchSpeed = 7;
    maxBackwardCrouchSpeed = 5;
    maxSideCrouchSpeed = 6;

	cameramaxdist = 4;
    cameraVerticalOffset = 1;
    cameraHorizontalOffset = 0.6;
    cameratilt = 0.1;
    maxfreelookangle = 2;

	rideable = false;
	canRide = false;

	uiName = "Infected";
	maxTools = 0;
	maxWeapons = 0;
	maxdamage = 50;//Health
	
	useCustomPainEffects = true;
	jumpSound = "JumpSound";
	PainSound		= "";
	DeathSound		= "";

	ShapeNameDistance = 15;
	hIsInfected = 1;
	hZombieL4BType = 1;
	hCustomNodeAppearance = 0;
	hPinCI = "";
	hBigMeleeSound = "";
	
	//Hole Attributes
	isHoleBot = 1;

	//Spawning option
	hSpawnTooClose = 0;//Doesn't spawn when player is too close and can see it
	hSpawnTCRange = 0;//above range, set in brick units
	hSpawnClose = 0;//Only spawn when close to a player, can be used with above function as long as hSCRange is higher than hSpawnTCRange
	hSpawnCRange = 0;//above range, set in brick units

	hType = "Zombie"; //Enemy,Friendly, Neutral
	hNeutralAttackChance = 100;
	//can have unique types, nazis will attack zombies but nazis will not attack other bots labeled nazi
	hName = "Infected";//cannot contain spaces
	hTickRate = 8000;

	//Wander Options
	hWander = 1;//Enables random walking
	hSmoothWander = 1;
	hReturnToSpawn = 0;//Returns to spawn when too far
	hSpawnDist = 32;//Defines the distance bot can travel away from spawnbrick

	//Searching options
	hSearch = 1;//Search for Players
	hSearchRadius = 512;//in brick units
	hSight = 1;//Require bot to see player before pursuing
	hStrafe = 0;//Randomly strafe while following player
	hSearchFOV = 1;//if enabled disables normal hSearch
	hFOVRadius = 10;//max 10

	hAlertOtherBots = 1;//Alerts other bots when he sees a player, or gets attacked

	//Attack Options
	hMelee = 2;//Melee
	hAttackDamage = $Pref::Server::L4B2Bots::NormalDamage;
	hShoot = 1;
	hWep = "";
	hShootTimes = 4;
	hMaxShootRange = 60;
	hAvoidCloseRange = 0;
	hTooCloseRange = 0;

	
	//Misc options
	hAvoidObstacles = 1;
	hSuperStacker = 0;
	hSpazJump = 0;

	hAFKOmeter = 1;
	hIdle = 1;
	hIdleAnimation = 1;
	hIdleLookAtOthers = 1;
	hIdleSpam = 0;
	hSpasticLook = 1;
	hEmote = 1;
};

datablock ExplosionData(ZombieHitExplosion)
{
   shakeCamera = true;
   camShakeFreq = "3.0 8.0 4.0";
   camShakeAmp = "0.15 1.25 0.15";
   camShakeDuration = 0.6;
   camShakeRadius = 1;
};
datablock ProjectileData(ZombieHitProjectile)
{
   explosion = ZombieHitExplosion;
};

function CommonZombieHoleBot::onAdd(%this,%obj)
{			
	Parent::onAdd(%this,%obj);
	%obj.onL4BDatablockAttributes();
	%obj.hDefaultL4BAppearance();
}

function CommonZombieHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	%obj.onL4BDatablockAttributes();
	%obj.setscale("1 1 1");
}

function CommonZombieHoleBot::onDamage(%this,%obj)
{
	Parent::OnDamage(%this,%obj);

	if(%obj.getWaterCoverage() == 1)
	{
		%obj.emote(oxygenBubbleImage, 1);
		serverPlay3D("drown_bubbles_sound",%obj.getPosition());
	}
	
	if(%obj.getstate() $= "Dead")
	return;

	if(%obj.lastdamage+1250 < getsimtime())
	{
		%obj.lastdamage = getsimtime();

		if(%obj.raisearms && !%obj.hasRiotshield)
		{
			%obj.raisearms = 0;
			%obj.playthread(1,"root");
			%obj.playthread(2,"plant");
		}

		if(%obj.getWaterCoverage() != 1)
		if(%obj.isBurning)
		{
			switch(%obj.chest)	
			{
				case 0: %obj.playaudio(0,"zombiemale_ignite" @ getrandom(1,5) @ "_sound");
				case 1: %obj.playaudio(0,"zombiefemale_ignite1" @ getrandom(1,5) @ "_sound");
			}
			
			if(!%obj.isBurningToDeath)
			{
				%obj.MaxSpazzClick = getrandom(16,32);
				L4B_SpazzZombie(%obj,0);
				%obj.isBurningToDeath = 1;
			}
		}
		else switch(%obj.chest)	
		{
			case 0: %obj.playaudio(0,"zombiemale_pain" @ getrandom(1,8) @ "_sound");
			case 1: %obj.playaudio(0,"zombiefemale_pain" @ getrandom(1,8) @ "_sound");
		}
	}
}

function CommonZombieHoleBot::onDisabled(%this,%obj)
{
	if(%obj.getstate() !$= "Dead")
	return;

	Parent::OnDisabled(%this,%obj);
	
	if(isObject(%obj.client))
	commandToClient( %obj.client, 'SetVignette', $EnvGuiServer::VignetteMultiply, $EnvGuiServer::VignetteColor );

	if(%obj.getWaterCoverage() == 1)
	serverPlay3D("die_underwater_bubbles_sound",%obj.getPosition());
	else if(%obj.headless)
	%obj.playaudio(0,"zombie_headless" @ getrandom(1,4) @ "_sound");
	else switch(%obj.chest)
	{
		case 0: %obj.playaudio(0,"zombiemale_death" @ getrandom(1,10) @ "_sound");
		case 1: %obj.playaudio(0,"zombiefemale_death" @ getrandom(1,10) @ "_sound");
	}

	if(isObject(%weapon = %obj.getMountedImage(0)) && %weapon.item)
	{
		L4B_ZombieDropLoot(%obj,getMountedImage(0),100);
		%obj.unMountImage(0);
	}
}

function CommonZombieHoleBot::onBotLoop(%this,%obj)
{
	if(%obj.getWaterCoverage() == 1)
	%obj.Damage(%obj, %obj.getPosition(), %obj.getdatablock().maxDamage/1.25, $DamageType::Suicide);
	
	if(isObject(%minigame = getMinigameFromObject(%obj)))
	{
		%prevdamage = $Pref::Server::L4B2Bots::NormalDamage;

		if(%minigame.UrgentRound)
		{
			%obj.hAttackDamage = %prevdamage*3;
			%obj.setnodeColor("gloweyes","1 0 0 1");
		}
		else
		{
			%obj.hAttackDamage = %prevdamage;
			%obj.setnodeColor("gloweyes","1 1 0 1");
		}
	}

	%obj.hLimitedLifetime();

	if(!isObject(%obj.distraction))
	{
		%obj.hSearch = 1;
		%obj.distraction = 0;
	}

	if(!%obj.hFollowing)
	{
		%obj.raisearms = 0;
		%obj.playthread(1,"root");

		switch(%obj.hZombieL4BType)
		{
			case 1:	%obj.setMaxForwardSpeed(7);
					%obj.setmaxUnderwaterForwardSpeed(7);		

			case 3: %obj.setMaxForwardSpeed(5);
					%obj.setmaxUnderwaterForwardSpeed(5);
		}
	}
}

function CommonZombieHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	return;
	
	if(%obj.lastsaw+getRandom(2000,6000) < getsimtime() && %obj.getWaterCoverage() != 1)
	{
		switch(%obj.chest)
		{
			case 0: %obj.playaudio(0,"zombiemale_attack" @ getrandom(1,10) @ "_sound");
			case 1: %obj.playaudio(0,"zombiefemale_attack" @ getrandom(1,12) @ "_sound");
		}

		switch(%obj.hZombieL4BType)
		{
			case 1: %obj.setMaxForwardSpeed(15);
					%obj.setmaxUnderwaterForwardSpeed(15);

			case 3: %obj.setMaxForwardSpeed(4);
					%obj.setmaxUnderwaterForwardSpeed(2);
		}

		%obj.lastsaw = getsimtime();
		%obj.playthread(2,plant);

		if(getRandom(1,100) <= 25)
		{
			%obj.MaxSpazzClick = getrandom(16,32);
			L4B_SpazzZombie(%obj,0);
		}

		if(!%obj.raisearms)
		{	
			%obj.playthread(1,"armReadyboth");
			%obj.raisearms = 1;
			%obj.setMaxForwardSpeed(10);
		}	
	}
}

function CommonZombieHoleBot::onCollision(%this, %obj, %col, %fade, %pos, %norm)
{
	Parent::oncollision(%this, %obj, %col, %fade, %pos, %norm);
}

function CommonZombieHoleBot::onBotMelee(%this,%obj,%col)
{		
	%meleeimpulse = mClamp(%obj.hLastMeleeDamage, 1, 10);
	
	if(%obj.hIsInfected $= "1" && !%col.hIsImmune && !%col.hIsInfected)
	{
		%col.setenergylevel(%col.getEnergyLevel()-%meleeimpulse*2);
		
		if(%col.getEnergyLevel() < 1)
		holeZombieInfect(%obj,%col);
	}

	if(%col.getType() & $TypeMasks::PlayerObjectType)
	%col.spawnExplosion("ZombieHitProjectile",%meleeimpulse/4);


	%col.applyimpulse(%col.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),getrandom(100,100*%meleeimpulse)),"0" SPC "0" SPC getrandom(100,100*%meleeimpulse)));
	%col.playthread(3,"plant");
	%obj.playaudio(1,"melee_hit" @ getrandom(1,8) @ "_sound");
}

function Player::ZombieLowerArms(%player)
{
	%player.playthread(2,root);
	%player.IsZombieArmsUp = 0;
}

function CommonZombieHoleBot::onBotCollision( %this, %obj, %col, %normal, %speed )
{	
	if(%obj.getState() $= "Dead")
	return;
	
	if(%col.getClassName() $= "Player" || %col.getClassName() $= "AIPlayer")
	if(%col.getState() !$= "Dead" && %obj.lasthit+getRandom(250,750) < getsimtime())
	{
		if(!checkholebotTeams(%obj,%col))//get out of my way
		{
			%col.applyimpulse(%col.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),getrandom(500,700)),"0" SPC "0" SPC getrandom(400,500)));
			%obj.playaudio(1,"melee_shove_sound");
			%obj.playthread(3,"activate2");
			%obj.lasthit = getsimtime();
		}
	}
}

function CommonZombieHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{
	Parent::onTrigger (%this, %obj, %triggerNum, %val);

	if(%obj.getClassName() $= "Player" && %obj.getstate() !$= "Dead")
	{
		switch(%triggerNum)
		{
			case 0: if(%val)
					{
						if(!%obj.IsZombieArmsUp)
						{
							%obj.playthread(2,armReadyboth);
							%obj.IsZombieArmsUp = 1;
						}

						cancel(%obj.ZombieLowerArmsSchedule);
						%obj.ZombieLowerArmsSchedule = %obj.schedule(500,ZombieLowerArms);

						%eye = %obj.getEyePoint(); //eye point
						%scale = getWord (%obj.getScale (), 2);
						%len = $Game::BrickActivateRange * %scale; //raycast length
						%masks = $TypeMasks::FxBrickObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType;
						%vec = %obj.getEyeVector(); //eye vector

						%beam = vectorScale(%vec,%len); //lengthened vector (for calculating the raycast's endpoint)
						%end = vectorAdd(%eye,%beam); //calculated endpoint for raycast
						%ray = containerRayCast(%eye,%end,%masks,%obj); //fire raycast
						%ray = isObject(firstWord(%ray)) ? %ray : 0; //if raycast hit - keep ray, else set it to zero

						if(!%ray) //if Beam Check fcn returned "0" (found nothing)
						return Parent::onTrigger (%this, %obj, %triggerNum, %val); //stop here

						%target = firstWord(%ray);

						if(%target.getType() & $TypeMasks::PlayerObjectType)
						if(isObject(getMinigamefromObject(%obj,%target)) && miniGameCanDamage(%obj,%target) && checkHoleBotTeams(%obj,%target))
						%obj.PlayerZombieMeleeAttack(%target);
					}
			default:
		}
	}
}

function CommonZombieHoleBot::L4BCommonAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{	
	%shirtColor = getRandomBotRGBColor();
	%accentColor = getRandomBotRGBColor();
	%pantsColor = getRandomBotPantsColor();
	%shoeColor = getRandomBotPantsColor();
	%hatColor = getRandomBotRGBColor();
	%packColor = getRandomBotRGBColor();
	%handColor = %skinColor;

	%larmColor = getRandom(0,1);
	if(%larmColor)
	%larmColor = %shirtColor;
	else %larmColor = %skinColor;
	%rarmColor = getRandom(0,1);
	if(%rarmColor)
	%rarmColor = %shirtColor;
	else %rarmColor = %skinColor;
	%rLegColor = getRandom(0,1);
	if(%rLegColor)
	%rLegColor = %shoeColor;
	else %rLegColor = %skinColor;
	%lLegColor = getRandom(0,1);
	if(%lLegColor)
	%lLegColor = %shoeColor;
	else %lLegColor = %skinColor;

	%pack2 = 0;
	%accent = 0;
	%obj.accentColor = %accentColor;
	%obj.accent =  %accent;
	%obj.hatColor = %hatColor;
	%obj.hat = %hat;
	%obj.headColor = %skinColor;
	%obj.faceName = %face;
	%obj.chest =  %chest;
	%obj.decalName = %decal;
	%obj.chestColor = %shirtColor;
	%obj.pack =  %pack;
	%obj.packColor =  %packColor;
	%obj.secondPack =  %pack2;
	%obj.secondPackColor =  %packColor;
	%obj.larm =  "0";
	%obj.larmColor = %larmColor;
	%obj.lhand =  0;
	%obj.lhandColor = %handColor;
	%obj.rarm =  "0";
	%obj.rarmColor = %rarmColor;
	%obj.rhandColor = %handColor;
	%obj.rhand = 0;
	%obj.hip =  "0";
	%obj.hipColor = %pantsColor;
	%obj.lleg =  0;
	%obj.llegColor = %lLegColor;
	%obj.rleg =  0;
	%obj.rlegColor = %rLegColor;

	if(%obj.getClassName() $= "AIPlayer" && getRandom(1,32) == 1)//Chance to become zombie version of player
	{
		if(isObject(%playerclient = ClientGroup.getObject(getRandom(ClientGroup.getCount()-1))))
		%obj.hZombieBotToPlayerApearance(%playerclient);
	}

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function CommonZombieHoleBot::L4BCommonFastAppearance(%this,%obj,%skinColor,%face,%chest)
{	
	%handColor = %skinColor;
	%hatColor = getRandomBotRGBColor();
	%packColor = getRandomBotRGBColor();
	%shirtColor = %skinColor;
	%pantsColor = getRandomBotPantsColor();
	%shoeColor = getRandomBotPantsColor();

	%larmColor = getRandom(0,1);
	if(%larmColor)
	%larmColor = %shirtColor;
	else %larmColor = %skinColor;
	%rarmColor = getRandom(0,1);
	if(%rarmColor)
	%rarmColor = %shirtColor;
	else %rarmColor = %skinColor;
	%rLegColor = getRandom(0,1);
	if(%rLegColor)
	%rLegColor = %shoeColor;
	else %rLegColor = %skinColor;
	%lLegColor = getRandom(0,1);
	if(%lLegColor)
	%lLegColor = %shoeColor;
	else %lLegColor = %skinColor;

	%decal = "HCZombie";
	%obj.accentColor = %accentColor;
	%obj.accent =  0;
	%obj.hatColor = %hatColor;
	%obj.hat = 0;
	%obj.headColor = %skinColor;
	%obj.faceName = %face;
	%obj.chest =  %chest;
	%obj.decalName = %decal;
	%obj.chestColor = %shirtColor;
	%obj.pack =  0;
	%obj.packColor =  %packColor;
	%obj.secondPack =  0;
	%obj.secondPackColor =  %packColor;
	%obj.larm =  "0";
	%obj.larmColor = %larmColor;
	%obj.lhand =  0;
	%obj.lhandColor = %handColor;
	%obj.rarm =  "0";
	%obj.rarmColor = %rarmColor;
	%obj.rhandColor = %handColor;
	%obj.rhand = 0;
	%obj.hip =  "0";
	%obj.hipColor = %pantsColor;
	%obj.lleg =  0;
	%obj.llegColor = %lLegColor;
	%obj.rleg =  0;
	%obj.rlegColor = %rLegColor;

	if(%obj.getClassName() $= "AIPlayer" && getRandom(1,16) == 1)//Chance to become zombie version of player
	{
		if(isObject(%playerclient = ClientGroup.getObject(getRandom(ClientGroup.getCount()-1))))
		%obj.hZombieBotToPlayerApearance(%playerclient);
	}

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
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