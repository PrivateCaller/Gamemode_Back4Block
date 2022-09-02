datablock fxDTSBrickData (BrickCommonZombie_HoleSpawnData)
{
	brickFile = "Add-ons/Bot_Hole/4xSpawn.blb";
	category = "Special";
	subCategory = "Holes - L4B";
	uiName = "Common Zombie Hole";
	iconName = "Add-Ons/Gamemode_Left4Block/add-ins/bot_l4b/icons/icon_zombie";

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

    maxForwardSpeed = 9;
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
	maxdamage = 100;//Health

	rechargeRate = 1.15;
	maxenergy = 100;
	showEnergyBar = true;
	
	useCustomPainEffects = true;
	jumpSound = "JumpSound";
	PainSound		= "";
	DeathSound		= "";

	hIsInfected = 1;
	hZombieL4BType = "Normal";
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
	hMelee = 1;
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
	if(%obj.getstate() $= "Dead") return;

	if(%obj.lastdamage+1250 < getsimtime())
	{
		%obj.lastdamage = getsimtime();
		%obj.playthread(2,"plant");

		if(%obj.raisearms)
		{
			%obj.raisearms = 0;
			%obj.playthread(1,"root");
		}

		if(%obj.getWaterCoverage() == 1)
		{
			%obj.emote(oxygenBubbleImage, 1);
			serverPlay3D("drown_bubbles_sound",%obj.getPosition());
		}
		else if(%obj.isBurning)
		{
			switch(%obj.chest)	
			{
				case 0: %obj.playaudio(0,"zombiemale_ignite" @ getrandom(1,5) @ "_sound");
				case 1: %obj.playaudio(0,"zombiefemale_ignite1" @ getrandom(1,5) @ "_sound");
			}

			%obj.MaxSpazzClick = getrandom(16,32);
			L4B_SpazzZombie(%obj,0);
			
		}
		else switch(%obj.chest)	
		{
			case 0: %obj.playaudio(0,"zombiemale_pain" @ getrandom(1,8) @ "_sound");
			case 1: %obj.playaudio(0,"zombiefemale_pain" @ getrandom(1,8) @ "_sound");
		}
	}

	Parent::OnDamage(%this,%obj);
}

function CommonZombieHoleBot::onDisabled(%this,%obj)
{
	if(%obj.getstate() !$= "Dead") return;

	Parent::OnDisabled(%this,%obj);
	
	if(isObject(%obj.client)) commandToClient(%obj.client,'SetVignette',$EnvGuiServer::VignetteMultiply,$EnvGuiServer::VignetteColor);

	if(%obj.getWaterCoverage() == 1) serverPlay3D("die_underwater_bubbles_sound",%obj.getPosition());

	else if(%obj.headless) %obj.playaudio(0,"zombie_headless" @ getrandom(1,4) @ "_sound");
	
	else switch(%obj.chest)
	{
		case 0: %obj.playaudio(0,"zombiemale_death" @ getrandom(1,10) @ "_sound");
		case 1: %obj.playaudio(0,"zombiefemale_death" @ getrandom(1,10) @ "_sound");
	}
}

function CommonZombieHoleBot::onBotLoop(%this,%obj)
{
	if(%obj.getWaterCoverage() == 1) %obj.Damage(%obj,%obj.getPosition(),%obj.getdatablock().maxDamage/1.25,$DamageType::Suicide);

	if(%obj.hState !$= "Following")
	{		
		if(!isObject(%obj.distraction)) %obj.hSearch = 1;

		%obj.raisearms = 0;
		%obj.playthread(1,"root");
		%obj.setMaxForwardSpeed(7);
		%obj.setmaxUnderwaterForwardSpeed(7);
		%obj.hLimitedLifetime();
	}
}

function CommonZombieHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(!isObject(%obj) || %obj.getState() $= "Dead") return;

	%obj.playthread(2,plant);
	%obj.setMaxForwardSpeed(11);
	%obj.setmaxUnderwaterForwardSpeed(11);
	
	switch(%obj.chest)
	{
		case 0: %obj.playaudio(0,"zombiemale_attack" @ getrandom(1,10) @ "_sound");
		case 1: %obj.playaudio(0,"zombiefemale_attack" @ getrandom(1,12) @ "_sound");
	}

	if(isObject(%targ) && vectordist(%obj.getposition(),%targ.getposition()) < 15)
	{
		if(!%obj.raisearms)
		{	
			%obj.playthread(1,"armReadyboth");
			%obj.raisearms = 1;
		}

		if(getRandom(1,4) == 1) L4B_SpazzZombie(%obj,0);
	}
	else if(%obj.raisearms)
	{	
		%obj.playthread(1,"root");
		%obj.raisearms = 0;
	}	
}

function CommonZombieHoleBot::onCollision(%this, %obj, %col, %fade, %pos, %norm)
{
	Parent::onCollision(%this, %obj, %col, %fade, %pos, %norm);
}

function CommonZombieHoleBot::onBotMelee(%this,%obj,%col)
{		
	%meleeimpulse = mClamp(%obj.hLastMeleeDamage, 1, 10);
	%obj.playaudio(1,"melee_hit" @ getrandom(1,8) @ "_sound");
	%col.applyimpulse(%col.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),getrandom(100,100*%meleeimpulse)),"0" SPC "0" SPC getrandom(100,100*%meleeimpulse)));
	
	if(%col.getType() & $TypeMasks::PlayerObjectType)
	{
		if(%obj.hIsInfected && !%col.hIsImmune && !%col.hIsInfected && %col.getEnergyLevel() < 1)
		{
			%col.setenergylevel(%col.getEnergyLevel()-%meleeimpulse*2);
			holeZombieInfect(%obj,%col);
		}

		%col.spawnExplosion("ZombieHitProjectile",%meleeimpulse/4);
		%col.playthread(3,"plant");
	}
}

function CommonZombieHoleBot::onBotCollision( %this, %obj, %col, %normal, %speed )
{	
	if(%obj.getState() $= "Dead") return;
	//do something, I have no idea yet
}

function CommonZombieHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{
	Parent::onTrigger (%this, %obj, %triggerNum, %val);

	if(%obj.getClassName() $= "Player" && %obj.getstate() !$= "Dead")
	{
		switch(%triggerNum)
		{
			case 0: if(%val && %obj.getEnergyLevel() > 25)
					{
						if(!%obj.IsZombieArmsUp)
						{
							%obj.playthread(2,armReadyboth);
							%obj.IsZombieArmsUp = 1;
						}

						if(isObject(%touchedobj = %obj.lastactivated) && checkHoleBotTeams(%obj,%touchedobj)) %obj.hMeleeAttack(%touchedobj);

						cancel(%obj.ZombieLowerArmsSchedule);
						%obj.ZombieLowerArmsSchedule = %obj.schedule(500,ZombieLowerArms);
					}
			default:
		}
	}
}

function Player::ZombieLowerArms(%player)
{
	if(isObject(%player))
	{
		%player.playthread(2,root);
		%player.IsZombieArmsUp = 0;
	}
}

function CommonZombieHoleBot::Appearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
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

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}