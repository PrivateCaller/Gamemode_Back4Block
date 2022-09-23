datablock fxDTSBrickData (BrickZombieTankBot_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
	uiName = "Zombie Tank Hole";
	iconName = "Add-Ons/Gamemode_Left4Block/add-ins/bot_l4b/icons/icon_Tank";
	
	holeBot = "ZombieTankHoleBot";
};

datablock PlayerData(ZombieTankHoleBot : CommonZombieHoleBot)
{
	uiName = "Tank Infected";
	shapeFile = "add-ons/gamemode_left4block/add-ins/bot_l4b/models/zTank.dts";
	maxDamage = $Pref::Server::L4B2Bots::TankHealth;//Health
	mass = 500;

	runforce = 100 * 75;
	drag = 0.1;

    maxForwardSpeed = 6;
    maxBackwardSpeed = 4;
    maxSideSpeed = 5;

 	maxForwardCrouchSpeed = 3;
    maxBackwardCrouchSpeed = 2;
    maxSideCrouchSpeed = 1;

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

	jumpForce = 8.3 * 90;

	hName = "Tank";//cannot contain spaces
	hTickRate = 5000;
	jumpSound = "HorseJumpSound";
	resistMelee = true;
	
	hSearchRadius = 512;//in brick units
	hStrafe = 0;//Randomly strafe while following player
	hSearchFOV = 1;//if enabled disables normal hSearch
	hMaxShootRange = 120;//The range in which the bot will shoot the player

	hAttackDamage = $Pref::Server::L4B2Bots::SpecialsDamage*2.5;
	hMeleeCI = "Tank";

	hMaxShootRange = 512;
	hSuperStacker = 0;
	hIdleAnimation = 0;//Plays random animations/emotes, sit, click, love/hate/etc
	hSpasticLook = 1;//Makes them look around their environment a bit more.

	rechargeRate = 0.5;
	maxenergy = 100;
	showEnergyBar = true;
	
	hIsInfected = 2;
	hZombieL4BType = "Special";
	hPinCI = "<bitmapk:gamemode_left4block/add-ins/bot_l4b/models/icons/ci_tank2>";
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
		if(%obj.getClassName() $= "AIPlayer")
		%obj.setcrouching(1);
	}

	Parent::onImpact(%this, %obj, %col, %vec, %force);
}

function ZombieTankHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(getRandom(1,100) <= $Pref::Server::L4B2Bots::TankChance && getWord(%obj.getvelocity(),2) == 0 && vectorDist(%obj.getPosition(),%targ.getPosition()) >= 35)
	{
		%obj.setaimobject(%targ);
		%obj.mountImage(BoulderImage,0);
		%obj.schedule(2500,setImageTrigger,0,1);
	}

	if(!%obj.startMusic)
	{
		if(isObject(%minigame = getMiniGameFromObject(%obj))) %minigame.DirectorMusic("musicdata_L4D_tank",true,1,%client);
		%obj.startMusic = 1;
	}
}

function ZombieTankHoleBot::onNewDataBlock(%this,%obj)
{	
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);
	%obj.hDefaultL4BAppearance(%obj);

	if(getRandom(1,8) == 1)
	{
		%scale = getRandom(14,15)*0.1;
		%obj.setscale(%scale SPC %scale SPC %scale);
	}
	else %obj.setscale("1.25 1.25 1.25");
}


function ZombieTankHoleBot::onAdd(%this,%obj)
{	
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
}

function ZombieTankHoleBot::onBotLoop(%this,%obj)
{
	%obj.hLimitedLifetime();

	if(!%obj.isBurning)
	{
		if(!%obj.hFollowing)
		%obj.playaudio(0,"tank_idle" @ getrandom(1,7) @ "_sound");
		else %obj.playaudio(0,"tank_yell" @ getrandom(1,6) @ "_sound");
	}
}	

function ZombieTankHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact) %damage = %damage/1.5;
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function ZombieTankHoleBot::onDamage(%this,%obj,%Am,%Type )
{
    Parent::onDamage(%this,%obj,%Am,%Type);

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
	
	if(isObject(%minigame = getMiniGamefromObject(%obj)))
	%minigame.RoundEnd();

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

function ZombieTankHoleBot::Appearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{
	%obj.hidenode("BallisticHelmet");
	%obj.hidenode("BallisticVest");
 	%obj.hidenode("Headskin2");

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

	if(isObject(getMiniGamefromObject(%obj)) && getMiniGamefromObject(%obj).SoldierTank)
	{
		%armorcolor = getRandomBotPantsColor();

		%obj.PantsColor = %armorcolor;
		%obj.TorsoColor = %armorcolor;
		%obj.HelmetColor = %armorcolor;
		%obj.VestColor = %armorcolor;

		%LegColorR = getRandom(0,1);
		if(%LegColorR)
		%LegColorR = %armorcolor;
		else %LegColorR = %skincolor;

		%LegColorL = getRandom(0,1);
		if(%LegColorL)
		%LegColorL = %armorcolor;
		else %LegColorL = %skincolor;

		%obj.ShoeRColor = %LegColorR;
		%obj.ShoeLColor = %LegColorL;

		%armRcolor = getRandom(0,1);
		if(%armRcolor)
		%armRcolor = %obj.ArmRColor;
		else %armRcolor = %armorcolor;

		%armLcolor = getRandom(0,1);
		if(%armLcolor)
		%armLcolor = %skincolor;
		else %armLcolor = %armorcolor;

		%obj.ArmRColor = %skincolor;
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