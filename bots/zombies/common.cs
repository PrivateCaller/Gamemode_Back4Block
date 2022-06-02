datablock fxDTSBrickData (BrickCommonZombie_HoleSpawnData)
{
	brickFile = "Add-ons/Bot_Hole/4xSpawn.blb";
	category = "Special";
	subCategory = "Holes - L4B";
	uiName = "Common Zombie Hole";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_zombie";

	bricktype = 2;
	cancover = 0;
	orientationfix = 1;
	indestructable = 1;

	isBotHole = 1;
	holeBot = "CommonZombieHoleBot";
};

datablock PlayerData(CommonZombieHoleBot : PlayerMeleeAnims)
{
	canJet = false;
	jumpForce = 9.5*100;
	minImpactSpeed = 32;
	airControl = 0.1;
	speedDamageScale = 2.5;

    maxForwardSpeed = 11;
    maxBackwardSpeed = 7;
    maxSideSpeed = 8;

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
	maxdamage = 35;//Health
	
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
	hAttackDamage = $Pref::Server::L4B2Bots::NormalsDamage;
	hShoot = 1;
	hWep = "";
	hShootTimes = 4;
	hMaxShootRange = 60;
	hAvoidCloseRange = 0;
	hTooCloseRange = 0;

	
	//Misc options
	hAvoidObstacles = 1;
	hSuperStacker = 1;
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

	//%obj.schedule(500,doMRandomTele);

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
	
	%obj.setShapeNameHealth();
	
	if(%obj.getstate() $= "Dead")
	return;

	if(%obj.lastdamage+1000 < getsimtime())
	{
		%obj.lastdamage = getsimtime();

		if(%obj.raisearms && !%obj.hasRiotshield)
		{
			%obj.raisearms = 0;
			%obj.playthread(1,"root");
			%obj.playthread(2,"plant");
		}

		switch(%obj.chest)	
		{
			case 0: %obj.playaudio(0,"zombiemale_pain" @ getrandom(1,3) @ "_sound");
			case 1: %obj.playaudio(0,"zombiefemale_pain" @ getrandom(1,3) @ "_sound");
		}
	}
}

function CommonZombieHoleBot::onDisabled(%this,%obj)
{
	if(%obj.getstate() !$= "Dead")
	return;
	
	if(isObject(%obj.client))
	commandToClient( %obj.client, 'SetVignette', $EnvGuiServer::VignetteMultiply, $EnvGuiServer::VignetteColor );

	switch(%obj.chest)	
	{
		case 0: %obj.playaudio(0,"zombiemale_death" @ getrandom(1,3) @ "_sound");
		case 1: %obj.playaudio(0,"zombiefemale_death" @ getrandom(1,3) @ "_sound");
	}

	if(isObject(%weapon = %obj.getMountedImage(0)))
	{
		L4B_ZombieDropLoot(%obj,%weapon.item,100);
		%obj.unMountImage(0);
		L4B_ZombieLootInitialize(%this,%obj);
	}

	Parent::OnDisabled(%this,%obj);
}

function CommonZombieHoleBot::onBotLoop(%this,%obj)
{
	%obj.hAttackDamage = $Pref::Server::L4B2Bots::NormalsDamage;
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
			case 1:	%obj.setMaxForwardSpeed(%obj.getDatablock().maxForwardSpeed);
					%obj.setmaxUnderwaterForwardSpeed(%obj.getDatablock().maxForwardSpeed);		

			case 3: %obj.setMaxForwardSpeed(2);
					%obj.setmaxUnderwaterForwardSpeed(2);
		}
	}
}

function CommonZombieHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	return;
	
	if(%obj.lastsaw+getRandom(2000,6000) < getsimtime())
	{
		switch(%obj.chest)
		{
			case 0: %obj.playaudio(0,"zombiemale_attack" @ getrandom(1,4) @ "_sound");
			case 1: %obj.playaudio(0,"zombiefemale_attack" @ getrandom(1,4) @ "_sound");
		}

		switch(%obj.hZombieL4BType)
		{
			case 1: %obj.setMaxForwardSpeed(20);
					%obj.setmaxUnderwaterForwardSpeed(20);

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

		if($Pref::Server::L4B2Bots::ClumsyZombies && getRandom(1,100) <= 5)
		{
			%obj.stopHoleLoop();
			L4B_SpazzZombieInitialize(%obj,0);
			schedule(1000,0,serverCmdSit,%obj);
		}
	}
}

function CommonZombieHoleBot::onCollision(%this, %obj, %col, %fade, %pos, %norm)
{
	if(!$Pref::Server::L4B2Bots::HoldVictims || %obj.getstate() $= "dead" || !%obj.hLoopActive)
	return Parent::oncollision(%this, %obj, %col, %fade, %pos, %norm);

	if(strlwr(strstr(%obj.hType,"biled") != -1))
	{
		%obj.hIgnore = %col;
		%obj.hFollowing = 0;
		return Parent::oncollision(%this, %obj, %col, %fade, %pos, %norm);
	}
	
	if(%obj.hLastPull+250 < getsimtime() && %col.getType() & $TypeMasks::PlayerObjectType)
	{	
		if(L4B_CheckifinMinigame(%obj, %col) && checkHoleBotTeams(%obj,%col) && !%col.isBeingStrangled)
		{
			%col.setvelocity("0 0 -.25");
			%obj.playthread(3,"shiftdown");
		}
		%obj.hLastPull = getsimtime();
	}
	Parent::oncollision(%this, %obj, %col, %fade, %pos, %norm);
}

function CommonZombieHoleBot::onBotMelee(%this,%obj,%col)
{		
	%p = new Projectile()
	{
		dataBlock = "ZombieHitProjectile";
		initialPosition = %col.getPosition();
		sourceObject = %obj;
		client = %obj.client;
	};
	MissionCleanup.add(%p);
	%p.explode();

	%meleeimpulse = mClamp(%obj.hLastMeleeDamage, 1, 10);
	%col.applyimpulse(%col.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),getrandom(100,100*%meleeimpulse)),"0" SPC "0" SPC getrandom(100,100*%meleeimpulse)));
	%col.playthread(3,"plant");
	%obj.playaudio(1,"zombie_hit" @ getrandom(1,8) @ "_sound");
}

function Player::ZombieLowerArms(%player)
{
	%player.playthread(2,root);
	%player.IsZombieArmsUp = 0;
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

	if(%obj.getClassName() $= "AIPlayer" && getRandom(1,1000) < 5)//Chance to become zombie version of player
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

	if(%obj.getClassName() $= "AIPlayer" && getRandom(1,1000) < $Pref::Server::L4B2Bots::ZombifiedPlayerBotAppearance*10)//Chance to become zombie version of player
	{
		if(isObject(%playerclient = ClientGroup.getObject(getRandom(ClientGroup.getCount()-1))))
		%obj.hZombieBotToPlayerApearance(%playerclient);
	}

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}