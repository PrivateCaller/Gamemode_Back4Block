//Rope shape
datablock StaticShapeData(HataCylinder2Shape)
{
	shapeFile = "./models/tongue.dts";
};

if(!isObject(swordExplosionParticle)) //I can't imagine how creating a datablock exactly the same as the default one could mess something up, but this should save the modified ones.
{
	datablock ParticleData(swordExplosionParticle) //A bash explosion doesn't need to be fancy.
	{
		dragCoefficient			= 2;
		gravityCoefficient		= 1.0;
		inheritedVelFactor		= 0.2;
		constantAcceleration	= 0.0;
		
		spinRandomMin			= -90;
		spinRandomMax			= 90;
		
		lifetimeMS			 	= 500;
		lifetimeVarianceMS		= 300;
		
		textureName				= "base/data/particles/chunk";
		
		colors[0]				= "0.7 0.7 0.9 0.9";
		colors[1]				= "0.9 0.9 0.9 0.0";
		sizes[0]				= 0.5;
		sizes[1]				= 0.25;
	};
	
	datablock ParticleEmitterData(swordExplosionEmitter)
	{
		ejectionPeriodMS	= 7;
		periodVarianceMS	= 0;
		ejectionVelocity	= 8;
		velocityVariance	= 1.0;
		ejectionOffset		= 0.0;
		thetaMin			= 0;
		thetaMax			= 60;
		phiReferenceVel		= 0;
		phiVariance			= 360;
		
		overrideAdvance		= false;
		
		particles			= "swordExplosionParticle";

		uiName				= "Sword Hit";
	};
}

datablock ItemData(RiotShieldItem)
{
	category		= "Weapon";
	className		= "Weapon";

	shapeFile		= "./models/riotshield/Riotshield.dts";
	rotate			= false;
	mass			= 1;
	density			= 0.2;
	elasticity		= 0.2;
	friction		= 0.6;
	emap			= true;

	uiName			= "Riot Shield";
	iconName		= "./icons/icon_riotshield";
	doColorShift	= false;

	image			= RiotShieldImage;
	canDrop			= true;
};

datablock ShapeBaseImageData(RiotShieldImage)
{
	shapeFile		= "./models/riotshield/Riotshield.dts";
	emap							= true;

	mountPoint						= 0;
	offset							= "0 0 0";
	eyeOffset						= 0;
	rotation						= eulerToMatrix( "0 0 0" );
	scale							= "3 3 3";
	correctMuzzleVector				= true;

	className						= "WeaponImage";
	item							= " ";
	ammo							= " ";
	projectile						= "";
	projectileType					= "Projectile";

	melee							= false;
	armReady						= true;

	doColorShift					= false;
	colorShiftColor					= "0.196 0.196 0.196 0.5";

	stateName[0]					= "Activate";
	stateSound[0]					= weaponSwitchSound;
	stateTransitionOnTimeout[0]		= "Ready";
	stateTimeoutValue[0]			= 0.5;

	stateName[1]					= "Ready";
	stateTransitionOnTriggerDown[1]	= "Attack";

	stateName[2]					= "Attack";
	stateTimeoutValue[2]			= 0.75;
	stateScript[2]					= "onAttack";
	stateTransitionOnTimeout[2]		= "Ready";
};

function RiotShieldImage::onAttack(%this, %obj, %slot)
{	
	%obj.playthread(1,"armReady");
	%obj.doMelee();
}

datablock shapeBaseImageData( ConstructionConeSpeakerImage )
{
	shapefile = "./models/construction/conespeaker.dts";

	mountPoint = 1;
	offset = "0 0.18 0.25";
	doColorShift = false;
	className = "WeaponImage";
	armReady = false;
};

//NOTE TO SELF: TSShapeConstructor has to be done BEFORE player datablock.
datablock TSShapeConstructor(mMeleeDts) 
{
	baseShape = "base/data/shapes/player/mmelee.dts";
	sequence0 = "base/data/shapes/player/default.dsq";
	sequence1 = "base/data/shapes/player/melee.dsq";
	sequence2 = "base/data/shapes/player/actions.dsq";
};

datablock PlayerData(PlayerMeleeAnims : PlayerStandardArmor)
{
	shapeFile = "base/data/shapes/player/mMelee.dts";
	uniformCompatible = true;
	canJet = 0;
	uiName = "";
};

datablock PlayerData(SurvivorPlayer : PlayerMeleeAnims)
{
	canPhysRoll = true;

	canJet = false;
	jumpforce = 100*8.5;
	jumpDelay = 25;
	minimpactspeed = 20;
	speedDamageScale = 3;

	airControl = 0.1;

	cameramaxdist = 2.5;
    cameraVerticalOffset = 1;
    cameraHorizontalOffset = 0.8;
    cameratilt = 0;
    maxfreelookangle = 2;

    maxForwardSpeed = 8;
    maxBackwardSpeed = 4;
    maxSideSpeed = 5;

 	maxForwardCrouchSpeed = 4;
    maxBackwardCrouchSpeed = 2;
    maxSideCrouchSpeed = 3;

	uiName = "Survivor Player";
	usesL4DItems = 1;
	isSurvivor = 1;
	hType = "Survivors";
	maxtools = 5;
	maxWeapons = 5;

	jumpSound 		= JumpSound;
	PainSound		= "";
	DeathSound		= "";
	useCustomPainEffects = true;
	rechargeRate = 0.025;
	maxenergy = 100;
	showEnergyBar = true;
};
datablock PlayerData(SurvivorPlayerMed : SurvivorPlayer)
{
	maxForwardSpeed = 6;
   	maxBackwardSpeed = 2;
   	maxSideSpeed = 3;
 	maxForwardCrouchSpeed = 3;
    maxBackwardCrouchSpeed = 1;
    maxSideCrouchSpeed = 2;

	uiName = "";
};
datablock PlayerData(SurvivorPlayerLow : SurvivorPlayer)
{
	canPhysRoll = false;
	maxForwardSpeed = 3;
   	maxBackwardSpeed = 1;
   	maxSideSpeed = 1;

	maxForwardCrouchSpeed = 2;
   	maxBackwardCrouchSpeed = 1;
   	maxSideCrouchSpeed = 1;

	uiName = "";
};

datablock PlayerData(DownPlayerSurvivorArmor : SurvivorPlayerLow)
{	
   	runForce = 100 * 60;
   	maxForwardSpeed = 0;
   	maxBackwardSpeed = 0;
   	maxSideSpeed = 0;

   	maxForwardCrouchSpeed = 0;
   	maxBackwardCrouchSpeed = 0;
   	maxSideCrouchSpeed = 0;

   	jumpForce = 0; //8.3 * 90;

	firstpersononly = 1;

	uiName = "";
	maxenergy = 100;
	maxDamage = 100;
	showEnergyBar = true;

	jumpSound 		= "";
	PainSound		= "";
	DeathSound		= "";
	useCustomPainEffects = true;
	isDowned = 1;

   	runSurfaceAngle  = 5;
   	jumpSurfaceAngle = 5;
   	isSurvivor = 1;
   	rechargerate = 0;
};




if(!isObject(HealParticle))
{
	datablock ParticleData(HealParticle)
	{
		dragCoefficient      = 5;
		gravityCoefficient   = -0.2;
		inheritedVelFactor   = 0;
		constantAcceleration = 0;
		lifetimeMS           = 1000;
		lifetimeVarianceMS   = 500;
		useInvAlpha          = false;
		textureName          = "./icons/heal";
		colors[0]     = "1 1 1 1";
		colors[1]     = "1 1 1 0.5";
		colors[2]     = "0 0 0 0";
		sizes[0]      = 0.4;
		sizes[1]      = 0.6;
		sizes[2]      = 0.4;
		times[0]      = 0;
		times[1]      = 0.2;
		times[2]      = 1;
	};
}

if(!isObject(HealEmitter))
{
	datablock ParticleEmitterData(HealEmitter)
	{
		ejectionPeriodMS = 35;
		periodVarianceMS = 0;
		ejectionVelocity = 0.5;
		ejectionOffset   = 1;
		velocityVariance = 0.49;
		thetaMin         = 0;
		thetaMax         = 120;
		phiReferenceVel  = 0;
		phiVariance      = 360;
		overrideAdvance = false;
		particles = "HealParticle";

		uiName = "";
	};
}

if(!isObject(HealImage))
{
	datablock ShapeBaseImageData(HealImage)
	{
		shapeFile = "base/data/shapes/empty.dts";
		emap = false;

		mountPoint = $HeadSlot;

		stateName[0]			= "Ready";
		stateTransitionOnTimeout[0]	= "FireA";
		stateTimeoutValue[0]		= 0.01;

		stateName[1]			= "FireA";
		stateTransitionOnTimeout[1]	= "Done";
		stateWaitForTimeout[1]		= True;
		stateTimeoutValue[1]		= 0.350;
		stateEmitter[1]			= HealEmitter;
		stateEmitterTime[1]		= 0.350;

		stateName[2]			= "Done";
		stateScript[2]			= "onDone";
	};
}

function HealImage::onDone(%data, %player, %slot)
{
	%player.unMountImage(%slot);
}

// Tank Datablocks
datablock TSShapeConstructor(RotZTankDts)
{
	baseshape  = "./models/tank/ztank.dts";
	sequence0  = "./models/tank/ztankroot.dsq root";
	sequence1  = "./models/tank/ztankrun.dsq run";
	sequence2  = "./models/tank/ztankrun.dsq walk";
	sequence3  = "./models/tank/ztankback.dsq back";
	sequence4  = "./models/tank/ztankrunside.dsq side";
	sequence5  = "./models/tank/ztankcrouch.dsq crouch";
	sequence6  = "./models/tank/ztankcrouchrun.dsq crouchrun";
	sequence7  = "./models/tank/ztankcrouchback.dsq crouchback";
	sequence8 = "./models/tank/ztankheadside.dsq headside";
	sequence9 = "./models/tank/ztankjump.dsq jump";
	sequence10 = "./models/tank/ztankjump.dsq standjump";
	sequence11 = "./models/tank/ztankfall.dsq fall";
	sequence12 = "./models/tank/ztankland.dsq land";
	sequence13 = "./models/tank/ztankattack.dsq armattack";
	sequence14 = "./models/tank/ztankactivate.dsq activate";
    sequence15 = "./models/tank/ztankactivate2.dsq activate2";
	sequence16 = "./models/tank/ztankarmcharge.dsq spearready";  
	sequence17 = "./models/tank/ztankarmthrow.dsq spearthrow";  
	sequence18 = "./models/tank/ztankdie.dsq death1";
	sequence19  = "./models/tank/zTankLook.dsq look";
	//sequence16 = "./models/tank/ztankroot.dsq leftrecoil";
	//sequence17 = "./models/tank/ztankroot.dsq armreadyleft";
	//sequence18 = "./models/tank/ztankroot.dsq armreadyright";
	//sequence19 = "./models/tank/ztankroot.dsq armreadyboth";
	//sequence22 = "./models/tank/ztankroot.dsq talk";	
	//sequence8  = "./models/tank/ztankroot.dsq crouchside";
	//sequence11 = "./models/tank/ztankroot.dsq headup";
	//sequence24 = "./models/tank/ztankroot.dsq shiftup";
	//sequence25 = "./models/tank/ztankroot.dsq shiftdown";
	//sequence26 = "./models/tank/ztankroot.dsq shiftaway";
	//sequence27 = "./models/tank/ztankroot.dsq shiftto";
	//sequence28 = "./models/tank/ztankroot.dsq shiftleft";
	//sequence29 = "./models/tank/ztankroot.dsq shiftright";
	//sequence30 = "./models/tank/ztankroot.dsq rotcw";
	//sequence31 = "./models/tank/ztankroot.dsq rotccw";
	//sequence32 = "./models/tank/ztankroot.dsq undo";
	//sequence33 = "./models/tank/ztankroot.dsq plant";
	//sequence34 = "./models/tank/ztankroot.dsq sit";
	//sequence35 = "./models/tank/ztankroot.dsq wrench";
};

//Smoker Datablocks
datablock ParticleData(SmokerSpores)
{
	dragCoefficient      = 4;
	gravityCoefficient   = 0.2;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS = 1000;
	lifetimeVarianceMS = 800;
	textureName = "base/data/particles/cloud";
	
	spinSpeed = 10.0;
	spinRandomMin = -50.0;
	spinRandomMax = 50.0;
	
	colors[0] = 33/255 SPC 33/255 SPC 33/255 SPC 0.5;
	colors[1] = 33/255 SPC 33/255 SPC 33/255 SPC 0.25;
	colors[2] = 33/255 SPC 33/255 SPC 33/255 SPC 0;
	sizes[0] = 12.0;
	sizes[1] = 8.0;
	sizes[2] = 4.0;
	
	useInvAlpha = true;
};

datablock ParticleEmitterData(SmokerSporeEmitter)
{
	lifeTimeMS = 8000;
	
   ejectionPeriodMS = 10;
   periodVarianceMS = 5;
   ejectionVelocity = 15;
   velocityVariance = 12;
   ejectionOffset   = 0.0;
   thetaMin         = 89;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
	particles = SmokerSpores;
	
	emitterNode = HalfEmitterNode;
};

datablock ProjectileData(SmokerSporeProjectile)
{
   projectileShapeName				= "base/data/shapes/empty.dts";
   directDamage        = 0;
   directDamageType = $DamageType::RocketDirect;
   radiusDamageType = $DamageType::RocketRadius;
   //impactImpulse	   = 1000;
   //verticalImpulse	   = 1000;
   explosion           = "";
   particleEmitter     = "SmokerSporeEmitter";

   brickExplosionRadius = 0;
   brickExplosionImpact = false;
   brickExplosionForce  = 0;             
   brickExplosionMaxVolume = 0;
   brickExplosionMaxVolumeFloating = 0;

   sound = "smoker_explode_sound";

   muzzleVelocity      = 0;
   velInheritFactor    = 0;
   explodeOnDeath = false;

   armingDelay         = 0; //4 second fuse 
   lifetime            = 8000;
   fadeDelay           = 7500;
   bounceElasticity    = 0;
   bounceFriction      = 0;
   isBallistic         = true;
   gravityMod = 0.02;

   hasLight    = false;
   lightRadius = 1.0;
   lightColor  = "0 0 0";

   uiName = "";
};

datablock ParticleData(SmokerPulseParticle)
{
	dragCoefficient = 10;
	gravityCoefficient = -0.15;
	inheritedVelFactor = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS = 1000;
	lifetimeVarianceMS = 400;
	textureName = "base/data/particles/cloud";
	
	spinSpeed = 50.0;
	spinRandomMin = -500.0;
	spinRandomMax = 500.0;
	
	colors[0] = 33/255 SPC 33/2555 SPC 33/255 SPC 0.2;
	colors[1] = 33/255 SPC 33/255 SPC 33/255 SPC 0.1;
	colors[2] = 33/255 SPC 33/255 SPC 33/255 SPC 0.0;
	sizes[0] = 2.0;
	sizes[1] = 0.5;
	sizes[2] = 0.2;
	
	useInvAlpha = true;
};

datablock ParticleEmitterData(SmokerPulseEmitter)
{	
	ejectionPeriodMS = 15;
	periodVarianceMS = 10;
	ejectionVelocity = 10;
	velocityVariance = 5;
	ejectionOffset = 0.0;
	thetaMin = 0;
	thetaMax = 180;
	phiReferenceVel = 0;
	phiVariance = 360;
	overrideAdvance = false;
	
	emitterNode = HalfEmitterNode;

   particles = "SmokerPulseParticle";
   uiName = "";
};

datablock ShapeBaseImageData(SmokeStatusPlayerImage)
{
   shapeFile = "base/data/shapes/empty.dts";
   emap = true;

   mountPoint = 2;
   offset = "0 -0.25 -0.75";
   eyeOffset = 0;
   rotation = "0 0 0";

   correctMuzzleVector = true;

   className = "WeaponImage";

   item = "";
   ammo = " ";
   projectile = "";
   projectileType = Projectile;

   melee = false;
   armReady = false;

   doColorShift = false;

	stateName[0]                   = "Smoke";
	stateTimeoutValue[0]           = 0.3;
	stateEmitter[0]                = SmokerPulseEmitter;
	stateEmitterTime[0]            = 1;
	stateTransitionOnTimeout[0]    = "Smoke2";

	stateName[1]                   = "Smoke2";
	stateEmitter[1]                = SmokerPulseEmitter;
	stateEmitterTime[1]            = 0.6;
	stateTimeoutValue[1]           = 0.1;
	stateTransitionOnTimeout[1]    = "Smoke";
};

datablock ShapeBaseImageData(ZombieSmokerConstrictImage)
{
	shapeFile = "./models/constricted.dts";
	emap = true;
	mountPoint = $BackSlot;
	offset = "0 0 0";
	eyeOffset = "0 0 0";
	rotation = eulerToMatrix("0 0 0");
	firstPerson = false;
		
	className = "WeaponImage";
	item = "";
		
	armReady = false;
   doColorShift = true;
   colorShiftColor = "1 0 0 1";
};

//Spitter Datablocks
datablock ParticleData(SpitAcidBallHitParticle)
{
	dragCoefficient      = 4;
	gravityCoefficient   = 0.5;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	spinRandomMin 	     = -90;
	spinRandomMax 		= 90;
	lifetimeMS           = 2000;
	lifetimeVarianceMS   = 800;
	textureName          = "base/data/particles/dot";
	colors[0]     = "0.530 0.825 0.591 0.7";
	colors[1]     = "0.697 0.770 0.380 0.0";
	sizes[0]      = 0.20;
	sizes[1]      = 0.18;
	times[0]	  = 0.0;
	times[1]	  = 1.0;

};

datablock ParticleEmitterData(SpitAcidBallHitEmitter)
{
   ejectionPeriodMS = 7;
   periodVarianceMS = 1;
   ejectionVelocity = 6;
   velocityVariance = 2.0;
   ejectionOffset   = 0.2;
   thetaMin         = 0;
   thetaMax         = 60;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "SpitAcidBallHitParticle";

   uiName = "";
};

datablock ParticleData(SpitAcidBallParticle)
{
	dragCoefficient      = 5;
	gravityCoefficient   = 0.05;
	inheritedVelFactor   = 0.0;
	constantAcceleration = 0.0;
	spinRandomMin 	     = 0;
	spinRandomMax 		= 0;
	lifetimeMS           = 300;
	lifetimeVarianceMS   = 200;
	textureName          = "base/data/particles/dot";
	colors[0]     = "0.6 0.790 0.4 0.1";
//	colors[1]     = "0.435 0.676 0.472 0.2";
	colors[1]     = "0.5 0.770 0.300 0.0";
	sizes[0]      = 0.8;
	sizes[1]      = 0.0;
//	sizes[2]      = 0.0;
	times[0]	  = 0.0;
	times[1]	  = 1;
//	times[2]	  = 1.0;

	useInvAlpha		= false;

};

datablock ParticleData(SpitAcidBallTrailParticle)
{
	dragCoefficient      = 5;
	gravityCoefficient   = 0.05;
	inheritedVelFactor   = 0.0;
	constantAcceleration = 0.0;
	spinRandomMin 	     = 0;
	spinRandomMax 		= 0;
	lifetimeMS           = 700;
	lifetimeVarianceMS   = 200;
	textureName          = "base/data/particles/dot";
	colors[0]     = "0.6 0.790 0.6 0.1";
	colors[1]     = "0.435 0.676 0.472 0.2";
	colors[2]     = "0.697 0.770 0.300 0.0";
	sizes[0]      = 0.5;
	sizes[1]      = 0.15;
	sizes[2]      = 0.0;
	times[0]	  = 0.0;
	times[1]	  = 0.2;
	times[2]	  = 1.0;

	useInvAlpha		= false;

};

datablock ParticleEmitterData(SpitAcidBallTrailEmitter)
{
   ejectionPeriodMS = 3;
   periodVarianceMS = 1;
   ejectionVelocity = 0;
   velocityVariance = 0;
   ejectionOffset   = 0.1;
   thetaMin         = 0;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 180;
   overrideAdvance = false;
   particles = "SpitAcidBallTrailParticle SpitAcidBallParticle";

   uiName = "";
};

datablock ParticleData(SpitAcidStatusParticle)
{
	dragCoefficient      = 0.2;
	gravityCoefficient   = 0.5;
	inheritedVelFactor   = 0.6;
	constantAcceleration = 0.0;
	spinRandomMin 	     = -90;
	spinRandomMax 		= 90;
	lifetimeMS           = 1300;
	lifetimeVarianceMS   = 400;
	textureName          = "base/data/particles/dot";
	colors[0]     = "0.6 0.7 0.3 0.3";
	colors[1]     = "0.75 0.8 0.4 0.4";
	colors[2]     = "0.3 0.79 0.2 0.0";
//	colors[3]     = "0.4 0.6 0.4 0.0";
	sizes[0]      = 0.01;
	sizes[1]      = 0.25;
	sizes[2]      = 0.05;
//	sizes[3]      = 0.0;
	times[0]	  = 0.0;
	times[1]	  = 0.3;
	times[2]	  = 1;
//	times[3]	  = 1.0;

};

datablock ParticleEmitterData(SpitAcidStatusEmitter)
{
   ejectionPeriodMS = 30;	//7
   periodVarianceMS = 2;
   ejectionVelocity = 0;
   velocityVariance = 0.0;
   ejectionOffset   = 1.0;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "SpitAcidStatusParticle";

   uiName = "";
};

datablock ParticleData(SpitAcidPulseParticle)
{
	dragCoefficient      = 0.8;
	gravityCoefficient   = -1.0;
	inheritedVelFactor   = 1;
	constantAcceleration = 0.0;
	spinRandomMin 	     = -90;
	spinRandomMax 		= 90;
	lifetimeMS           = 1000;
	lifetimeVarianceMS   = 300;
	textureName          = "base/data/particles/cloud";
	colors[0]     = "0.8 0.9 0.6 0.2";
	colors[1]     = "0.7 0.7 0.7 0.0";
	sizes[0]      = 2.0;
	sizes[1]      = 0.01;
	times[0]	  = 0.0;
	times[1]	  = 1.0;

};

datablock ParticleEmitterData(SpitAcidPulseEmitter)
{
   ejectionPeriodMS = 70;	//7
   periodVarianceMS = 5;
   ejectionVelocity = 0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.6;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "SpitAcidPulseParticle";

   uiName = "";
};

datablock ShapeBaseImageData(SpitAcidStatusPlayerImage)
{
   shapeFile = "base/data/shapes/empty.dts";
   emap = true;

   mountPoint = 2;
   offset = "0 -0.25 -0.75";
   eyeOffset = 0;
   rotation = "0 0 0";

   correctMuzzleVector = true;

   className = "WeaponImage";

   item = "";
   ammo = " ";
   projectile = "";
   projectileType = Projectile;
   
   isPoison = 1;

   melee = false;
   armReady = false;

   doColorShift = false;
   colorShiftColor = "1 1 1 1";

	//lightType = "ConstantLight";
	//lightColor = "0 1 0";
	//lightRadius = 4;

	stateName[0]                   = "Wait";
	stateTimeoutValue[0]           = 0.3;
	stateEmitter[0]                = SpitAcidStatusEmitter;
	stateEmitterTime[0]            = 1;
	stateTransitionOnTimeout[0]    = "Poison";
	//stateSound[0]                  = SpitAcidPulseSound;

	stateName[1]                   = "Poison";
	//stateScript[1]                 = "Damage";
	stateEmitter[1]                = SpitAcidPulseEmitter;
	stateEmitterTime[1]            = 0.6;
	stateTimeoutValue[1]           = 0.1;
	stateTransitionOnTimeout[1]    = "Wait";
	//stateSound[1]                  = FireShotSound;	//No sound
};

datablock ExplosionData(SpitAcidBallExplosion)
{
   explosionShape = "base/data/shapes/empty.dts";
   lifeTimeMS = 500;

   soundProfile = spit_hit_sound;

   particleEmitter = SpitAcidBallHitEmitter;
   particleDensity = 25;
   particleRadius = 0.2;

   faceViewer     = true;
   explosionScale = "1 1 1";

	damageRadius = 10;	//4
	radiusDamage = 1;

	impulseRadius = 0.1;
	impulseForce = 1000;

};

	datablock ProjectileData(SpitterSpitProjectile)
{
   	directDamage        = 8;
   	directDamageType  = $DamageType::SpitAcidBall;
   	radiusDamageType  = $DamageType::SpitAcidBall;
   	explosion           = "SpitAcidBallExplosion";
	particleEmitter = "SpitAcidBallTrailEmitter";

	impactImpulse	   = 0;
	verticalImpulse	   = 0;

  	 muzzleVelocity      = 30;	//50
  	 velInheritFactor    = 0.5;

 	  armingDelay         = 0;
 	  lifetime            = 5000;	//1200
 	  fadeDelay           = 1000;
	   bounceElasticity    = 0;
	   bounceFriction      = 0;
	   isBallistic         = true;
	   gravityMod = 1;

 	  hasLight    = false;
  	 lightRadius = 3.0;
  	 lightColor  = "0 0 0.5";

   	uiName = "";
};

	datablock ProjectileData(SpitterSpewedProjectile)
{
   	directDamage        = 5;
   	directDamageType  = $DamageType::SpitAcidBall;
   	radiusDamageType  = $DamageType::SpitAcidBall;
   	explosion           = "SpitAcidBallExplosion";
	particleEmitter = "SpitAcidBallTrailEmitter";

	impactImpulse	   = 0;
	verticalImpulse	   = 0;

  	 muzzleVelocity      = 30;	//50
  	 velInheritFactor    = 0.5;

 	  armingDelay         = 0;
 	  lifetime            = 5000;	//1200
 	  fadeDelay           = 1000;
	   bounceElasticity    = 0.2;
	   bounceFriction      = 0.5;
	   isBallistic         = true;
	   gravityMod = 1;

 	  hasLight    = false;

   	uiName = "";
};

//Boomer Datablocks
datablock ParticleData(BileStatusParticle)
{
	dragCoefficient      = 0.2;
	gravityCoefficient   = 1;
	inheritedVelFactor   = 0.6;
	constantAcceleration = 0.0;
	spinRandomMin 	     = -90;
	spinRandomMax 		= 90;
	lifetimeMS           = 350;
	lifetimeVarianceMS   = 150;
	textureName          = "base/data/particles/dot";
	colors[0]     = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
	colors[1]     = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
	colors[2]     = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
//	colors[3]     = "0.4 0.6 0.4 0.0";
	sizes[0]      = 0.08;
	sizes[1]      = 0.1;
	sizes[2]      = 0.16;
//	sizes[3]      = 0.0;
	times[0]	  = 0.0;
	times[1]	  = 0.3;
	times[2]	  = 1;
//	times[3]	  = 1.0;
	useInvAlpha = true;

};

datablock ParticleEmitterData(BileStatusEmitter)
{
   ejectionPeriodMS = 6;	//7
   periodVarianceMS = 5;
   ejectionVelocity = 0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.9;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "BileStatusParticle";

   uiName = "";
};

datablock ParticleData(BilePulseParticle)
{
	dragCoefficient      = 0.2;
	gravityCoefficient   = 0.5;
	inheritedVelFactor   = 0.6;
	constantAcceleration = 0.0;
	spinRandomMin 	     = -90;
	spinRandomMax 		= 90;
	lifetimeMS           = 500;
	lifetimeVarianceMS   = 250;
	textureName          = "base/data/particles/cloud";
	colors[0]     = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
	colors[1]     = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
	colors[2]     = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
//	colors[3]     = "0.4 0.6 0.4 0.0";
	sizes[0]      = 1.04;
	sizes[1]      = 1.08;
	sizes[2]      = 1.06;
//	sizes[3]      = 0.0;
	times[0]	  = 0.0;
	times[1]	  = 0.3;
	times[2]	  = 1;
//	times[3]	  = 1.0;
useInvAlpha = true;

};

datablock ParticleEmitterData(BilePulseEmitter)
{
   ejectionPeriodMS = 15;	//7
   periodVarianceMS = 10;
   ejectionVelocity = 0.25;
   velocityVariance = 0.0;
   ejectionOffset   = 1;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "BilePulseParticle";

   uiName = "";
};

datablock ShapeBaseImageData(BileStatusPlayerImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = true;

	mountPoint = 2;
	offset = "0 0 -0.3";
	eyeOffset = 0;
	rotation = "0 0 0";

	correctMuzzleVector = true;

	className = "WeaponImage";

	item = "";
	ammo = " ";
	projectile = "";
	projectileType = Projectile;

	melee = false;
	armReady = false;

	doColorShift = false;
	colorShiftColor = "1 1 1 1";

	stateName[0]                   = "Wait";
	stateTimeoutValue[0]           = 0.4;
	stateEmitter[0]                = BileStatusEmitter;
	stateEmitterTime[0]            = 1;
	stateTransitionOnTimeout[0]    = "Poison";
	
	stateName[1]                   = "Poison";
	stateEmitter[1]                = BilePulseEmitter;
	stateEmitterTime[1]            = 0.4;
	stateTimeoutValue[1]           = 0.1;
	stateTransitionOnTimeout[1]    = "Wait";
};


datablock ParticleData(BoomerBoomParticle)
{
   dragCoefficient      = 0.1;
   gravityCoefficient   = 1;
   inheritedVelFactor   = 0.9;
   constantAcceleration = 0.0;
   spinRandomMin       = -90;
   spinRandomMax     = 90;
   lifetimeMS           = 600;
   lifetimeVarianceMS   = 500;
   textureName          = "base/data/particles/cloud";
   colors[0]     = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
   colors[1]     = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
   colors[2]     = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
// colors[3]     = "0.4 0.6 0.4 0.0";
   sizes[0]      = 2.5;
   sizes[1]      = 2.25;
   sizes[2]      = 2;
// sizes[3]      = 0.0;
   times[0]   = 0.0;
   times[1]   = 0.3;
   times[2]   = 1;
// times[3]   = 1.0;

	useInvAlpha = true;

};

datablock ParticleEmitterData(BoomerBoomEmitter)
{
   ejectionPeriodMS = 5;
   periodVarianceMS = 1;
   ejectionVelocity = 15;
   velocityVariance = 10;
   ejectionOffset   = 1;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "BoomerBoomParticle";

   uiName = "";
};

datablock ExplosionData(BoomerExplosion)
{
	soundProfile		= boomer_explode_sound;

   explosionShape = "";
   explosion           = "BoomerExplosion";

   particleEmitter = BoomerBoomEmitter;
   particleDensity = 300;
   particleRadius = 0.25;

   lifeTimeMS = 500;

   // Dynamic light
   lightStartRadius = 10;
   lightEndRadius = 25;
   lightStartColor = "0.2 1 0.4 1";
   lightEndColor = "0 0 0 1";

   damageRadius = 8;
   radiusDamage = 20;

   impulseRadius = 8;
   impulseForce = 2500;

   faceViewer     = true;
   explosionScale = "2 2 2";

   shakeCamera = true;
   camShakeFreq = "10.0 11.0 10.0";
   camShakeAmp = "8.0 10.0 8.0";
   camShakeDuration = 1.5;
   camShakeRadius = 20.0;
};

datablock ProjectileData(BoomerProjectile)
{
	uiname							= "";
	lifetime						= 10;
	fadeDelay						= 10;
	explodeondeath						= true;

   directDamage        = 0;
   directDamageType = $DamageType::Boomer;
   radiusDamageType = $DamageType::Boomer;

   impactImpulse	   = 1;
   verticalImpulse	   = 1;

	explosion	= BoomerExplosion;
};

//particle fx
//////////////////////////////////////////
datablock ParticleData(bloodStreakParticle)
{
	dragCoefficient      = 1;
	gravityCoefficient   = 0;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 400;
	lifetimeVarianceMS   = 100;
	textureName          = "base/data/particles/cloud";
	spinSpeed			= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]			= "0.3 0.1 0 0.08";
	colors[1]			= "0.3 0.1 0 0.2";
	colors[2]			= "0.3 0.1 0 0.01";
	colors[3]			= "0.3 0.1 0 0";
	sizes[0]			= 0.85;
	sizes[1]			= 0.95;
	sizes[2]			= 0.65;
	sizes[3]			= 0.0;

	useInvAlpha = true;
};

datablock ParticleEmitterData(bloodStreakEmitter)
{
	lifeTimeMS			= 3500;

	ejectionPeriodMS	= 12;
	periodVarianceMS	= 0;
	ejectionVelocity	= 0;
	velocityVariance	= 0.0;
	ejectionOffset		= 0.1;
	thetaMin			= 0;
	thetaMax			= 180;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= "bloodStreakParticle";
};

datablock DebrisData(gib1Debris)
{
   emitters = "bloodStreakEmitter";

	shapeFile = "./models/Boomer/GIBLET1.dts";
	lifetime = 10;
	spinSpeed			= 2000.0;
	minSpinSpeed = -100.0;
	maxSpinSpeed = 100.0;
	elasticity = 0.5;
	friction = 0.4;
	numBounces = 5;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 1.5;
};

datablock DebrisData(gib2Debris)
{
   emitters = "bloodStreakEmitter";

	shapeFile = "./models/Boomer/GIBLET2.dts";
	lifetime = 10;
	spinSpeed			= 2000.0;
	minSpinSpeed = -600.0;
	maxSpinSpeed = 600.0;
	elasticity = 0.5;
	friction = 0.3;
	numBounces = 5;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 1.5;
};

datablock DebrisData(gib3Debris)
{
   emitters = "bloodStreakEmitter";

	shapeFile = "./models/Boomer/NEWGIB5.dts";
	lifetime = 10;
	spinSpeed			= 2000.0;
	minSpinSpeed = -600.0;
	maxSpinSpeed = 600.0;
	elasticity = 0.5;
	friction = 0.3;
	numBounces = 5;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 1.5;
};

datablock DebrisData(gib4Debris)
{
   emitters = "bloodStreakEmitter";

	shapeFile = "./models/Boomer/NEWGIB3.dts";
	lifetime = 10;
	spinSpeed			= 2000.0;
	minSpinSpeed = -600.0;
	maxSpinSpeed = 600.0;
	elasticity = 0.5;
	friction = 0.3;
	numBounces = 5;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 1.5;
};

datablock DebrisData(gib5Debris)
{
	shapeFile = "./models/Boomer/NEWGIB2.dts";
	lifetime = 10;
	spinSpeed			= 2000.0;
	minSpinSpeed = -600.0;
	maxSpinSpeed = 600.0;
	elasticity = 0.5;
	friction = 0.3;
	numBounces = 5;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 1.5;
};

datablock DebrisData(gib6Debris)
{
   emitters = "bloodStreakEmitter";

	shapeFile = "./models/Boomer/NEWGIB4.dts";
	lifetime = 10;
	spinSpeed			= 2000.0;
	minSpinSpeed = -600.0;
	maxSpinSpeed = 600.0;
	elasticity = 0.5;
	friction = 0.3;
	numBounces = 5;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 1.5;
};

datablock ParticleData(bloodExplosionParticle)
{
	dragCoefficient      = 1;
	gravityCoefficient   = 0.4;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 5000;
	lifetimeVarianceMS   = 100;
	textureName          = "base/data/particles/cloud";
	spinSpeed			= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]				= "0.5 0.3 0 0.5";
	colors[1]				= "0.5 0.3 0 0";
	sizes[0]			= 5.25;
	sizes[1]			= 4.25;

	useInvAlpha = true;
};

datablock ParticleEmitterData(bloodExplosionEmitter)
{
	ejectionPeriodMS	= 1;
	periodVarianceMS	= 0;
	ejectionVelocity	= 4;
	velocityVariance	= 1.0;
	ejectionOffset  	= 0.0;
	thetaMin			= 89;
	thetaMax			= 90;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= "bloodExplosionParticle";
};

datablock ParticleData(bloodChunksParticle)
{
	dragCoefficient			= 0;
	gravityCoefficient		= 3;
	inheritedVelFactor		= 0.2;
	constantAcceleration	= 0.0;
	lifetimeMS				= 7500;
	lifetimeVarianceMS		= 300;
	textureName				= "base/data/particles/chunk";
	spinSpeed				= 190.0;
	spinRandomMin			= -290.0;
	spinRandomMax			= 290.0;
	colors[0]				= "0.3 0 0.0 1";
	colors[1]				= "0.3 0 0.0 0";
	sizes[0]				= 0.7;
	sizes[1]				= 0.6;

	useInvAlpha				= true;
};

datablock ParticleEmitterData(bloodChunksEmitter)
{
	lifeTimeMS			= 100;
	ejectionPeriodMS	= 1;
	periodVarianceMS	= 0;
	ejectionVelocity	= 17;
	velocityVariance	= 16.0;
	ejectionOffset		= 1.0;
	thetaMin			= 0;
	thetaMax			= 180;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= "bloodChunksParticle";
};

datablock ParticleData(bloodSprayParticle)
{
	dragCoefficient			= 2;
	gravityCoefficient		= 2;
	inheritedVelFactor		= 0.2;
	constantAcceleration	= 0.0;
	lifetimeMS				= 5840;
	lifetimeVarianceMS		= 200;
	textureName				= "base/data/particles/dot";
	spinSpeed				= 0.0;
	spinRandomMin			= 0.0;
	spinRandomMax			= 0.0;
	colors[0]				= "0.5 0 0.0 1";
	colors[1]				= "0.5 0 0.0 0.5";
	colors[2]				= "0.5 0 0.0 0";
	sizes[0]				= 0.2;
	sizes[1]				= 0.2;
	sizes[2]				= 0.2;
	useInvAlpha				= true;
};

datablock ParticleEmitterData(bloodSprayEmitter)
{
	lifeTimeMS			= 1000;
	ejectionPeriodMS	= 1;
	periodVarianceMS	= 0;
	ejectionVelocity	= 18;
	velocityVariance	= 7.0;
	ejectionOffset		= 1.0;
	thetaMin			= 0;
	thetaMax			= 180;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= "bloodSprayParticle";
};

//explosion
//////////////////////////////////////////

datablock ExplosionData(bloodDebris1Explosion)
{
   particleEmitter = bloodChunksEmitter;
   particleDensity = 30;
   particleRadius = 0.2;

   debris = gib2Debris;
   debrisNum = 8;
   debrisNumVariance = 6;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 8;
   debrisVelocityVariance = 6;
};

datablock ExplosionData(bloodDebris2Explosion)
{
   debris = gib3Debris;
   debrisNum = 6;
   debrisNumVariance = 6;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 8;
   debrisVelocityVariance = 6;
};
datablock ExplosionData(bloodDebris3Explosion)
{
   debris = gib4Debris;
   debrisNum = 9;
   debrisNumVariance = 7;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 8;
   debrisVelocityVariance = 6;
};

datablock ExplosionData(bloodDebris4Explosion)
{
   debris = gib5Debris;
   debrisNum = 1;
   debrisNumVariance = 0;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 12;
   debrisVelocityVariance = 8;
};

datablock ExplosionData(bloodDebris5Explosion)
{
   debris = gib6Debris;
   debrisNum = 16;
   debrisNumVariance = 9;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 8;
   debrisVelocityVariance = 6;
};

datablock ExplosionData(goremodExplosion)
{
	soundProfile		= boomer_kablooey_sound;

   explosionShape = "";

   lifeTimeMS = 150;

   debris = gib1Debris;
   debrisNum = 16;
   debrisNumVariance = 16;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 8;
   debrisVelocityVariance = 6;

   particleEmitter = bloodExplosionEmitter;
   particleDensity = 10;
   particleRadius = 0.2;

   emitter[0] = bloodSprayEmitter;

   subExplosion[0] = bloodDebris1Explosion;
   subExplosion[1] = bloodDebris2Explosion;
   subExplosion[2] = bloodDebris3Explosion;
   subExplosion[3] = bloodDebris4Explosion;
   subExplosion[4] = bloodDebris5Explosion;

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = true;
   camShakeFreq = "10.0 11.0 10.0";
   camShakeAmp = "3.0 10.0 3.0";
   camShakeDuration = 0.1;
   camShakeRadius = 20.0;
};

datablock ProjectileData(goreModProjectile)
{
	uiname							= "";

	lifetime						= 10;
	fadeDelay						= 10;
	explodeondeath						= true;
	explosion						= goremodExplosion;

};

datablock ParticleData(stunParticle)
{
	dragCoefficient      = 13;
	gravityCoefficient   = 0.2;
	inheritedVelFactor   = 1.0;
	constantAcceleration = 0.0;
	lifetimeMS           = 400;
	lifetimeVarianceMS   = 0;
	textureName          = "base/data/particles/star1";
	spinSpeed		   = 0.0;
	spinRandomMin		= 0.0;
	spinRandomMax		= 0.0;
	colors[0]     = "1 1 0.2 0.9";
	colors[1]     = "1 1 0.4 0.5";
	colors[2]     = "1 1 0.5 0";

	sizes[0]      = 0.5;
	sizes[1]      = 0.2;
	sizes[2]      = 0.1;

	times[0] = 0.0;
	times[1] = 0.5;
	times[2] = 1.0;

	useInvAlpha = false;
};
datablock ParticleEmitterData(stunEmitter)
{
	ejectionPeriodMS = 12;
	periodVarianceMS = 1;
	ejectionVelocity = 5.25;
	velocityVariance = 0.0;
	ejectionOffset   = 0.25;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = stunParticle;
};
datablock ShapeBaseImageData(stunImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = false;

	mountPoint = $HeadSlot;
	offset = "0 0 0.4";
	eyeOffset = "0 0 999";

	stateName[0]				= "Ready";
	stateTimeoutValue[0]		= 0.01;
	stateTransitionOnTimeout[0]	= "FireA";

	stateName[1]				= "FireA";
	stateEmitter[1]				= stunEmitter;
	stateEmitterTime[1]			= 1.2;
	stateTimeoutValue[1]		= 1.2;
	stateTransitionOnTimeout[1]	= "Done";
	stateWaitForTimeout[1]		= true;

	stateName[2]				= "Done";
	stateTimeoutValue[2]		= 0.01;
	stateTransitionOnTimeout[2]	= "FireA";
};

///////////////////////////
//Boomer Vomit Datablocks//
///////////////////////////
datablock ParticleData(BoomerVomitBallHitParticle)
{
	dragCoefficient      = 4;
	gravityCoefficient   = 0.5;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	spinRandomMin 	     = -90;
	spinRandomMax 		= 90;
	lifetimeMS           = 2000;
	lifetimeVarianceMS   = 800;
	textureName          = "base/data/particles/cloud";
	colors[0]     = 33/255 SPC 158/255 SPC 11/255 SPC 0.7;
	colors[1]     = 33/255 SPC 158/255 SPC 11/255 SPC 0;
	sizes[0]      = 0.5;
	sizes[1]      = 0.1;
	times[0]	  = 0.0;
	times[1]	  = 1.0;

	useInvAlpha		= true;

};

datablock ParticleEmitterData(BoomerVomitBallHitEmitter)
{
   ejectionPeriodMS = 7;
   periodVarianceMS = 1;
   ejectionVelocity = 6;
   velocityVariance = 2.0;
   ejectionOffset   = 0.2;
   thetaMin         = 0;
   thetaMax         = 60;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "BoomerVomitBallHitParticle";

   uiName = "";
};

datablock ParticleData(BoomerVomitBallParticle)
{
	dragCoefficient      = 5;
	gravityCoefficient   = 0.05;
	inheritedVelFactor   = 0.0;
	constantAcceleration = 0.0;
	spinRandomMin 	     = 0;
	spinRandomMax 		= 0;
	lifetimeMS           = 300;
	lifetimeVarianceMS   = 200;
	textureName          = "base/data/particles/dot";
	colors[0]     = 33/255 SPC 255/255 SPC 11/255 SPC 0.3;
	colors[1]     = 33/255 SPC 200/255 SPC 11/255 SPC 0.3;
	colors[2]     = 33/255 SPC 158/255 SPC 11/255 SPC 0;
	sizes[0]      = 0.8;
	sizes[1]      = 0.0;
//	sizes[2]      = 0.0;
	times[0]	  = 0.0;
	times[1]	  = 1;
//	times[2]	  = 1.0;

	useInvAlpha		= true;

};

datablock ParticleData(BoomerVomitBallTrailParticle)
{
	dragCoefficient      = 5;
	gravityCoefficient   = 0.05;
	inheritedVelFactor   = 0.0;
	constantAcceleration = 0.0;
	spinRandomMin 	     = 0;
	spinRandomMax 		= 0;
	lifetimeMS           = 700;
	lifetimeVarianceMS   = 200;
	textureName          = "base/data/particles/cloud";
	colors[0]     = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
	colors[1]     = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
	colors[2]     = 33/255 SPC 158/255 SPC 11/255 SPC 0;
	sizes[0]      = 0.9;
	sizes[1]      = 0.4;
	sizes[2]      = 0.0;
	times[0]	  = 0.0;
	times[1]	  = 0.2;
	times[2]	  = 1.0;

	useInvAlpha		= true;

};

datablock ParticleEmitterData(BoomerVomitBallTrailEmitter)
{
   ejectionPeriodMS = 3;
   periodVarianceMS = 1;
   ejectionVelocity = 0;
   velocityVariance = 0;
   ejectionOffset   = 0.1;
   thetaMin         = 0;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 180;
   overrideAdvance = false;
   particles = "BoomerVomitBallTrailParticle BoomerVomitBallParticle";

   uiName = "";
};


datablock ExplosionData(BoomerVomitBallExplosion)
{
   explosionShape = "base/data/shapes/empty.dts";
   lifeTimeMS = 500;

   soundProfile = spit_hit_sound;

   particleEmitter = BoomerVomitBallHitEmitter;
   particleDensity = 25;
   particleRadius = 0.2;

   faceViewer     = true;
   explosionScale = "1 1 1";

	damageRadius = 10;	//4
	radiusDamage = 1;

	impulseRadius = 0.1;
	impulseForce = 1000;

};

	datablock ProjectileData(BoomerVomitProjectile)
{
   	directDamage        = 0;
	radiusDamage		= 0;
   	explosion           = "BoomerVomitBallExplosion";
	particleEmitter = "BoomerVomitBallTrailEmitter";

	impactImpulse	   = 0;
	verticalImpulse	   = 0;

	muzzleVelocity      = 15;	//50
	velInheritFactor    = 0.5;

	armingDelay         = 0;
	lifetime            = 5000;	//1200
	fadeDelay           = 1000;
	bounceElasticity    = 0;
	bounceFriction      = 0;
	isBallistic         = true;
	gravityMod = 1;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";

   	uiName = "";
};

	datablock ProjectileData(BoomerVomitSpewedProjectile)
{
   	directDamage        = 0;
	radiusDamage		= 0;
   	explosion           = "BoomerVomitBallExplosion";
	particleEmitter = "BoomerVomitBallTrailEmitter";

	impactImpulse	   = 0;
	verticalImpulse	   = 0;

	muzzleVelocity      = 15;	//50
	velInheritFactor    = 0.5;

	armingDelay         = 0;
	lifetime            = 5000;	//1200
	fadeDelay           = 1000;
	bounceElasticity    = 0.2;
	bounceFriction      = 0.5;
	isBallistic         = true;
	gravityMod = 1;

	hasLight    = false;

   	uiName = "";
};