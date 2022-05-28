datablock fxDTSBrickData (BrickZombieTankBot_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
	uiName = "Zombie Tank Hole";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_Tank";
	
	holeBot = "ZombieTankHoleBot";
};

datablock PlayerData(ZombieTankHoleBot : CommonZombieHoleBot)
{
	uiName = "Tank Infected";
	shapeFile = "Add-ons/Package_Left4Block/models/tank/zTank.dts";
	maxDamage = $Pref::Server::L4B2Bots::TankHP;//Health
	mass = 1000;

	runforce = 48 * 220;
	drag = 0.2;

    maxForwardSpeed = 6;
    maxBackwardSpeed = 4;
    maxSideSpeed = 5;

 	maxForwardCrouchSpeed = 5;
    maxBackwardCrouchSpeed = 3;
    maxSideCrouchSpeed = 4;

    minimpactspeed = 15;
    speeddamageScale = 10;

	cameramaxdist = 5;
    cameraVerticalOffset = 1;
    cameraHorizontalOffset = 0.6;
    cameratilt = 0.1;
    maxfreelookangle = 2;

	attackpower = 30;
	BrickDestroyMaxVolume = 1000;
	BrickMaxJumpHeight = 20;

	paintable = true;

	boundingBox = "9.5 4 12";
	crouchboundingBox = "8 3 6";

	jumpForce = 100 * 100; //8.3 * 90;

	hName = "Tank";//cannot contain spaces
	hTickRate = 5000;
	jumpSound = "HorseJumpSound";
	
	hSearchRadius = 512;//in brick units
	hStrafe = 0;//Randomly strafe while following player
	hSearchFOV = 1;//if enabled disables normal hSearch
	hMaxShootRange = 120;//The range in which the bot will shoot the player

	hAttackDamage = $Pref::Server::L4B2Bots::TankMeleeDamage;
	hMeleeCI = "Tank";

	hMaxShootRange = 512;
	hSuperStacker = 0;
	hIdleAnimation = 0;//Plays random animations/emotes, sit, click, love/hate/etc
	hSpasticLook = 1;//Makes them look around their environment a bit more.

	rechargeRate = 0.5;
	maxenergy = 100;
	showEnergyBar = true;
	
	ShapeNameDistance = 100;
	hIsInfected = 2;
	hZombieL4BType = 5;
	hCustomNodeAppearance = 1;
	hPinCI = "<bitmapk:Add-Ons/Package_Left4Block/icons/ci_tank2>";
	hBigMeleeSound = "tank_punch_sound";
	SpecialCPMessage = "Right click to throw boulders";
};

datablock ExplosionData(BigZombieHitExplosion)
{
   shakeCamera = true;
   camShakeFreq = "10.0 10.0 100.0";
   camShakeAmp = "1 1 2";
   camShakeDuration = 0.75;
   camShakeRadius = 1.0;
};
datablock ProjectileData(BigZombieHitProjectile)
{
   explosion = BigZombieHitExplosion;
};

function ZombieTankHoleBot::onImpact(%this, %obj, %col, %vec, %force)
{
	%oScale = 2*getWord(%obj.getScale(),0);
	%obj.spawnExplosion(pushBroomProjectile,%oScale SPC %oScale SPC %oScale);

	if(%force >= 25)
	{
		%obj.spawnExplosion(TankLandProjectile,%oScale SPC %oScale SPC %oScale);

		if(%obj.getClassName() $= "AIPlayer")
		%obj.setcrouching(1);
	}

	Parent::onImpact(%this, %obj, %col, %vec, %force);
}

function ZombieTankHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(getRandom(1,100) <= $Pref::Server::L4B2Bots::TankLunge)
	schedule(2000,0,L4B_ZombieLunge,%obj,%targ,10);

	if(getRandom(1,100) <= $Pref::Server::L4B2Bots::TankBoulders && L4B_IsOnGround(%obj) && vectorDist(%obj.getPosition(),%targ.getPosition()) >= 35)
	{
		%obj.setaimobject(%targ);
		%obj.mountImage(BoulderImage,0);
		%obj.schedule(2500,setImageTrigger,0,1);
	}

}

function ZombieTankHoleBot::onNewDataBlock(%this,%obj)
{	
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);
	%obj.hDefaultL4BAppearance(%obj);
	%obj.setscale("1.5 1.5 1.5");
}


function ZombieTankHoleBot::onAdd(%this,%obj)
{	
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
}

function ZombieTankHoleBot::onBotLoop(%this,%obj)
{
	%obj.hAttackDamage = $Pref::Server::L4B2Bots::TankMeleeDamage;
	%obj.hLimitedLifetime();

	if(!%obj.isBurning)
	{
		if(%obj.lastcalm+5000 < getsimtime())
		{
			if(!%obj.hFollowing)
			%obj.playaudio(0,"tank_idle" @ getrandom(1,7) @ "_sound");
			else %obj.playaudio(0,"tank_yell" @ getrandom(1,6) @ "_sound");

			%obj.lastcalm = getsimtime();
			%obj.tankDefaultSpeed = 4;
			%obj.setMaxForwardSpeed(%obj.tankDefaultSpeed);
		}
	}
	else
	{
		%obj.tankDefaultSpeed = 6;
		%obj.setMaxForwardSpeed(%obj.tankDefaultSpeed);
	}
}	

function ZombieTankHoleBot::onDamage(%this,%obj,%Am,%Type )
{
    Parent::onDamage(%this,%obj,%Am,%Type);

	%obj.setShapeNameHealth();

	if(%obj.getstate() !$= "Dead" && %obj.lastdamage+750 < getsimtime())//Check if the chest is the female variant and add a 1 second cooldown
	{
		%obj.playaudio(0,"tank_pain" @ getrandom(1,5) @ "_sound");
		%obj.lastdamage = getsimtime();
	}	
}

function ZombieTankHoleBot::onDisabled(%this,%obj)
{
	%obj.playaudio(0,"tank_death" @ getrandom(1,4) @ "_sound");
	Parent::OnDisabled(%this,%obj);


	if(isObject(%rock = %obj.getMountedImage(0)) && %rock.getName() $= "BoulderImage")
	{
		%obj.unMountImage(0);
		%rnd = getRandom();
		%dist = getRandom()*15;
		%x = mCos(%rnd*$PI*3)*%dist;
		%y = mSin(%rnd*$PI*3)*%dist;
		%p = new projectile()
		{
			datablock = BoulderProjectile;
			initialPosition = %obj.getHackPosition();
			initialVelocity = %x SPC %y SPC (getRandom()*4);
			client = %obj.sourceObject.client;
			sourceObject = %obj.sourceObject;
			damageType = $DamageType::BoulderDirect;
			scale = "4 4 4";
		};
		MissionCleanup.add(%p);
	}
}

function ZombieTankHoleBot::onCollision(%this, %obj, %col, %fade, %pos, %norm)
{
	Parent::oncollision(%this, %obj, %col, %fade, %pos, %norm);
}

function ZombieTankHoleBot::onBotMelee(%this,%obj,%col)
{
	%oscale = getWord(%obj.getScale(),2);
	if(%oScale >= 1.25 && %obj.lastpunch+500 < getsimtime())
    {
		%obj.lastpunch = getsimtime();
		%obj.bigZombieMelee();
	}
}

function ZombieTankHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{	
	if(%obj.getClassName() $= "Player" && %obj.getstate() !$= "Dead")
	{
		if(%val)
		switch(%triggerNum)
		{
			case 0: if(!isObject(%obj.getMountedImage(0)))
					{
						%obj.playthread(2,"activate2");
						%obj.playthread(0,"jump");
						%obj.bigZombieMelee();
					}
			case 4: if(%obj.GetEnergyLevel() >= %this.maxenergy)
					%obj.mountImage(BoulderImage,0);
				
			default:
		}
	}
	Parent::onTrigger (%this, %obj, %triggerNum, %val);
}

function ZombieTankHoleBot::L4BSpecialAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{
	%obj.hidenode("BallisticHelmet");
	%obj.hidenode("BallisticVest");

	switch($Pref::Server::L4B2Bots::CustomStyle)
	{
		case 0: %obj.hidenode(Headskin2);
		case 1: %obj.hidenode(Headskin2);
		case 2: %obj.hidenode(Headskin1);		
		case 3: %obj.hidenode(Headskin1);
	}

	%pantsrandmultiplier = getrandom(2,8)*0.25;
	%pantsColor = 0 SPC 0.141*%pantsrandmultiplier SPC 0.333*%pantsrandmultiplier SPC 1;

	%LegColorR = getRandom(0,1);
	if(%LegColorR)
	%LegColorR = %pantsColor;
	else %LegColorR = %skinColor;

	%LegColorL = getRandom(0,1);
	if(%LegColorL)
	%LegColorL = %pantsColor;
	else %LegColorL = %skinColor;
	
	%obj.headColor = %skinColor;
	%obj.headColor = %skinColor;
	%obj.hipColor = %pantsColor;
	%obj.rlegColor = %LegColorR;
	%obj.llegColor = %LegColorL;
	%obj.chestColor = %skincolor;
	%obj.rarmColor = %skincolor;
	%obj.larmColor = %skincolor;
	%obj.rhandColor = %skincolor;
	%obj.lhandColor = %skincolor;

	//Head
	%obj.setnodecolor("HeadSkin1",%obj.headColor);
	%obj.setnodecolor("HeadSkin2",%obj.headColor);

	//Lower Body
	%obj.setnodecolor("Pants",%obj.hipColor);
	%obj.setnodecolor("ShoeR",%obj.rlegColor);
	%obj.setnodecolor("ShoeL",%obj.llegColor);

	//Upper Body
	%obj.setnodecolor("Torso",%obj.chestColor);
	%obj.setnodecolor("armR",%obj.larmColor);
	%obj.setnodecolor("armL",%obj.rarmColor);
	%obj.setnodecolor("handR",%obj.rhandColor);
	%obj.setnodecolor("handL",%obj.lhandColor);

	if(getRandom(1,1000) <= 10)
	{
		%armorcolor = getRandomBotPantsColor();

		%obj.PantsColor = %armorcolor;
		%obj.TorsoColor = %armorcolor;
		%obj.HelmetColor = %armorcolor;
		%obj.VestColor = %armorcolor;

		%LegColorR = getRandom(0,1);
		if(%LegColorR)
		%LegColorR = %armorcolor;
		else %LegColorR = %obj.HeadSkin1Color;

		%LegColorL = getRandom(0,1);
		if(%LegColorL)
		%LegColorL = %armorcolor;
		else %LegColorL = %obj.HeadSkin1Color;

		%obj.ShoeRColor = %LegColorR;
		%obj.ShoeLColor = %LegColorL;

		%armRcolor = getRandom(0,1);
		if(%armRcolor)
		%armRcolor = %obj.ArmRColor;
		else %armRcolor = %armorcolor;

		%armLcolor = getRandom(0,1);
		if(%armLcolor)
		%armLcolor = %obj.ArmLColor;
		else %armLcolor = %armorcolor;

		%obj.ArmRColor = %armRcolor;
		%obj.ArmLColor = %armLcolor;

		//Vest
		%obj.setnodecolor("BallisticHelmet",%obj.HelmetColor);
		%obj.setnodecolor("BallisticVest",%obj.VestColor);
		%obj.unhidenode("BallisticHelmet");
		%obj.unhidenode("BallisticVest");

		//Lower Body
		%obj.setnodecolor("Pants",%obj.PantsColor);
		%obj.setnodecolor("ShoeR",%obj.ShoeRColor);
		%obj.setnodecolor("ShoeL",%obj.ShoeLColor);

		//Upper Body
		%obj.setnodecolor("Torso",%obj.TorsoColor);
		%obj.setnodecolor("armR",%obj.ArmRColor);
		%obj.setnodecolor("armL",%obj.ArmLColor);
	}
}

function ZombieTankHoleBot::hCustomNodeAppearance(%this,%obj)
{
	if($Pref::Server::L4B2Bots::CustomStyle < 2)
	{
		%obj.unhidenode("gloweyes");
		%obj.setnodeColor("gloweyes","1 1 0 1");
	}
	else %obj.hidenode("gloweyes");
}