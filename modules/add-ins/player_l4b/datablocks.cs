%pattern = "add-ons/gamemode_left4block/modules/add-ins/player_l4b/sound/*.wav";
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strreplace(filename(strlwr(%file)), ".wav", "");

	//Check the names of the folders to determine what type of soundscape it will be, and check if it's a loopable sound or not
	if(strstr(%file,"normal") != -1)//Normal soundscape
	if(strstr(%file,"loop") != -1) eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioCloseLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClose3d; filename = \"" @ %file @ "\"; };");

	if(strstr(%file,"quiet") != -1)//Quiet soundscape
	if(strstr(%file,"loop") != -1) eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosestLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosest3d; filename = \"" @ %file @ "\"; };");

	if(strstr(%file,"loud") != -1)//Loudest Soundscape
	if(strstr(%file,"loop") != -1) eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioDefaultLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioDefault3d; filename = \"" @ %file @ "\"; };");

	%file = findNextFile(%pattern);
}

addExtraResource("add-ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_down.png");
addExtraResource("add-ons/gamemode_left4block/modules/add-ins/player_l4b/icons/ci_victimsaved.png");
addExtraResource("add-ons/gamemode_left4block/modules/add-ins/player_l4b/icons/ci_boomer2.png");
addExtraResource("add-ons/gamemode_left4block/modules/add-ins/player_l4b/icons/ci_charger2.png");
addExtraResource("add-ons/gamemode_left4block/modules/add-ins/player_l4b/icons/ci_hunter2.png");
addExtraResource("add-ons/gamemode_left4block/modules/add-ins/player_l4b/icons/ci_infected.png");
addExtraResource("add-ons/gamemode_left4block/modules/add-ins/player_l4b/icons/ci_jockey2.png");
addExtraResource("add-ons/gamemode_left4block/modules/add-ins/player_l4b/icons/ci_skull2.png");
addExtraResource("add-ons/gamemode_left4block/modules/add-ins/player_l4b/icons/ci_smoker2.png");
addExtraResource("add-ons/gamemode_left4block/modules/add-ins/player_l4b/icons/ci_witchclose.png");
addExtraResource("add-ons/gamemode_left4block/modules/add-ins/player_l4b/icons/ci_spitter2.png");
AddDamageType("SecondaryMelee",'<bitmap:add-ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_punch> %1','%2 <bitmap:add-ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_punch> %1',0,1);

if(isFile("add-ons/gamemode_left4block/modules/add-ins/player_l4b/models/decal.ifl"))
{
	%write = new FileObject();
	%write.openForWrite("add-ons/gamemode_left4block/modules/add-ins/player_l4b/models/decal.ifl");	
	%write.writeLine("base/data/shapes/players/decals/AAA-none.png");
	
	%decalpath = "add-ons/gamemode_left4block/modules/add-ins/player_l4b/models/decals/*.png";
	for(%decalfile = findFirstFile(%decalpath); %decalfile !$= ""; %decalfile = findNextFile(%decalpath))
	{
		eval("addExtraResource(\""@ %decalfile @ "\");");
		%write.writeLine(%decalfile);
		%decalName = strreplace(filename(strlwr(%decalfile)), ".png", "");

		if(strstr(strlwr(%decalfile), "tailor") == -1)
		{
			eval("$hZombieDecal[" @ %i++ @ "] = \"" @ %decalName @ "\";");
			eval("$hZombieDecalAmount = %i;");
		}
	}

	addExtraResource("add-ons/gamemode_left4block/modules/add-ins/player_l4b/models/decal.ifl");
	%write.close();
	%write.delete();
}

addExtraResource("add-ons/gamemode_left4block/modules/add-ins/player_l4b/models/face.ifl");

datablock ExplosionData(ZombieHitExplosion)
{
	shakeCamera = true;
	camShakeDuration = 1;
	camShakeRadius = 1;
	camShakeFreq = "3 3 3";
	camShakeAmp = "0.6 0.6 0.6";
};
datablock ProjectileData(ZombieHitProjectile)
{
   explosion = ZombieHitExplosion;
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

if(!isObject(swordExplosionParticle)) //I mean... Just in case right?
{
	datablock ParticleData(swordExplosionParticle)
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
	sizes[0]      = 1.0;
	sizes[1]      = 2.0;

	useInvAlpha = false;
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

datablock ExplosionData(SecondaryMeleeExplosion)
{
	soundProfile = "";
	lifeTimeMS = 150;

	particleDensity = 5;
	particleRadius = 0.2;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeDuration = 1;
	camShakeRadius = 2.5;

	camShakeFreq = "1.5 1.5 1.5";
	camShakeAmp = "0.75 0.75 0.75";
	particleEmitter = SecondaryMeleeFlashEmitter;

	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0 0 0";
	lightEndColor = "0 0 0";
};
datablock ProjectileData(SecondaryMeleeProjectile)
{
	explosion = SecondaryMeleeExplosion;
};

datablock ParticleData(oxygenBubbleParticle : painMidParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= -2.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 800;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= false;
	animateTexture		= false;

	textureName		= "base/data/particles/bubble";
   
	colors[0]	= "0.2 0.6 1 0.4";
	colors[1]	= "0.2 0.6 1 0.8";
	colors[2]	= "0.2 0.6 1 0.8";
	sizes[0]	= 0.2;
	sizes[1]	= 0.4;
	sizes[2]	= 0.0;
	times[0]	= 0.0;
	times[1]	= 0.8;
   times[2]	= 1.0;
};

datablock ParticleEmitterData(oxygenBubbleEmitter : painMidEmitter)
{
   ejectionPeriodMS = 5;
   periodVarianceMS = 0;
   ejectionVelocity = 6;
   velocityVariance = 2;
   ejectionOffset   = 0.2;
   thetaMin         = 0;
   thetaMax         = 105;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;

   particles = oxygenBubbleParticle;

   uiName = "Oxygen Bubbles";
};

datablock ShapeBaseImageData(oxygenBubbleImage : painMidImage)
{
	stateTimeoutValue[1] = 0.05;
	stateEmitter[1] = oxygenBubbleEmitter;
	stateEmitterTime[1]	= 0.05;
};

function oxygenBubbleImage::onDone(%this,%obj,%slot) { %obj.unMountImage(%slot); }

datablock TSShapeConstructor(NewMDts) 
{
	baseShape = "./models/newm.dts";	
	sequence0 = "./models/default.dsq";
	sequence1 = "./models/default_lookarmed.dsq";	
	sequence2 = "./models/survivor.dsq";		
};

datablock PlayerData(SurvivorPlayer : PlayerStandardArmor)
{
	shapeFile = NewMDts.baseshape;
	canJet = false;
	runForce = 100 * 45;
	jumpforce = 100*9.25;
	jumpDelay = 25;
	minimpactspeed = 15;
	speedDamageScale = 0.25;
	mass = 105;
	airControl = 0.05;

	cameramaxdist = 2.25;
    cameraVerticalOffset = 1;
    cameraHorizontalOffset = 0.75;
    cameratilt = 0;
    maxfreelookangle = 2;

    maxForwardSpeed = 9;
	maxSideSpeed = 8;
	maxBackwardSpeed = 7;

 	maxForwardCrouchSpeed = 5;
	maxSideCrouchSpeed = 4;
	maxBackwardCrouchSpeed = 3;
    
	groundImpactMinSpeed = 5;
	groundImpactShakeFreq = "4.0 4.0 4.0";
	groundImpactShakeAmp = "1.0 1.0 1.0";
	groundImpactShakeDuration = 0.8;
	groundImpactShakeFalloff = 15;

	upMaxSpeed = 150;
	upResistSpeed = 10;
	upResistFactor = 0.25;	
	horizMaxSpeed = 150;
	horizResistSpeed = 10;
	horizResistFactor = 0.25;

	uiName = "Survivor Player";
	usesL4DItems = true;
	isSurvivor = true;
	hType = "Survivors";
	enableRBlood = true;
	usesL4Bappearance = true;
	renderFirstPerson = true;
	maxtools = 5;
	maxWeapons = 5;

	boundingBox = "4.5 4 10.6";
	crouchboundingBox = "4.5 4 9";
	
	jumpSound 		= JumpSound;
	PainSound		= "";
	DeathSound		= "";
	useCustomPainEffects = true;
	rechargeRate = 0.025;
	maxenergy = 100;
	showEnergyBar = false;
};
datablock PlayerData(SurvivorPlayerDowned : SurvivorPlayer)
{	
   	runForce = SurvivorPlayer.runForce;
   	maxForwardSpeed = 0;
   	maxBackwardSpeed = 0;
   	maxSideSpeed = 0;
   	maxForwardCrouchSpeed = 0;
   	maxBackwardCrouchSpeed = 0;
   	maxSideCrouchSpeed = 0;
   	jumpForce = 0;
	rechargerate = 0;
	isDowned = true;
	uiName = "";
};

//Smoker Datablocks
datablock StaticShapeData(SmokerTongueShape)
{
	shapeFile = "./models/tongue.dts";
	isSmokerTongue = 1;
};

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
   explosion           = "";
   particleEmitter     = "SmokerSporeEmitter";

   brickExplosionImpact = false;

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
   gravityMod = 0;
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

datablock shapeBaseImageData(ConstructionConeSpeakerImage)
{
	shapefile = "./models/conespeaker.dts";

	mountPoint = 1;
	offset = "0 0.18 0.25";
	doColorShift = false;
	className = "WeaponImage";
	armReady = false;
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
	colors[1]     = "0.5 0.770 0.300 0.0";
	sizes[0]      = 0.8;
	sizes[1]      = 0.0;
	times[0]	  = 0.0;
	times[1]	  = 1;

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
	sizes[0]      = 0.01;
	sizes[1]      = 0.25;
	sizes[2]      = 0.05;
	times[0]	  = 0.0;
	times[1]	  = 0.3;
	times[2]	  = 1;
};

datablock ParticleEmitterData(SpitAcidStatusEmitter)
{
   ejectionPeriodMS = 30;
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

   	melee = false;
   	armReady = false;

   	doColorShift = false;

	stateName[0]                   = "Wait";
	stateTimeoutValue[0]           = 0.3;
	stateEmitter[0]                = SpitAcidStatusEmitter;
	stateEmitterTime[0]            = 1;
	stateTransitionOnTimeout[0]    = "Poison";

	stateName[1]                   = "Poison";
	stateEmitter[1]                = SpitAcidPulseEmitter;
	stateEmitterTime[1]            = 0.6;
	stateTimeoutValue[1]           = 0.1;
	stateTransitionOnTimeout[1]    = "Wait";
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
   	directDamage        = 1;
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
   	directDamage        = 1;
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

	stateName[0]                   = "Status";
	stateTimeoutValue[0]           = 0.5;
	stateEmitter[0]                = "BileStatusEmitter";
	stateEmitterTime[0]            = 1;
	stateTransitionOnTimeout[0]    = "Pulse";
	
	stateName[1]                   = "Pulse";
	stateEmitter[1]                = "BilePulseEmitter";
	stateEmitterTime[1]            = 1;
	stateTimeoutValue[1]           = 0.5;
	stateTransitionOnTimeout[1]    = "Status";
	stateScript[1]					= "onPulse";
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
   colors[0]     = "0.3 0.15 0.05 0.3";
   colors[1]     = "0.3 0.1 0.05 0.15";
   colors[2]     = "0.3 0.05 0.05 0";
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
	soundProfile		= "boomer_explode_sound";

   explosionShape = "";
   explosion           = "BoomerExplosion";

   particleEmitter = BoomerBoomEmitter;
   particleDensity = 300;
   particleRadius = 0.25;

   lifeTimeMS = 500;

   subExplosion[0] = RBloodOrganExplosion;
   subExplosion[1] = RBloodBrainExplosion;
   //subExplosion[2] = bloodDebris3Explosion;
   //subExplosion[3] = bloodDebris4Explosion;
   //subExplosion[4] = bloodDebris5Explosion;   

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
   	explosionScale = "2.5 2.5 2.5";
	damageRadius = 0;
	radiusDamage = 0;
	impulseRadius = 0;
	impulseForce = 0;
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
   	uiName = "";
};

datablock fxDTSBrickData (BrickCommonZombie_HoleSpawnData)
{
	brickFile = "Add-ons/Bot_Hole/4xSpawn.blb";
	category = "Special";
	subCategory = "Holes - L4B";
	uiName = "Common Zombie Hole";
	iconName = "Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/icon_zombie";

	bricktype = 2;
	cancover = 0;
	orientationfix = 1;
	indestructable = 1;

	isBotHole = 1;
	holeBot = "CommonZombieHoleBot";
};

datablock TSShapeConstructor(ZombieMDts) 
{
	baseShape = "./models/oldm.dts";
	sequence0 = "./models/default_old.dsq";
	sequence1 = "./models/zombie.dsq";
};

datablock PlayerData(CommonZombieHoleBot : SurvivorPlayer)
{
	shapeFile = ZombieMDts.baseShape;
	boundingBox = VectorScale ("1.25 1.25 2.65", 4);
	crouchBoundingBox = VectorScale ("1.25 1.25 1.00", 4);	
	renderFirstPerson = false;
	thirdpersononly = true;
	jumpForce = 9.5*100;
	minImpactSpeed = 20;
	airControl = 0.1;
	speedDamageScale = 0.5;
	isSurvivor = false;

    maxForwardSpeed = 11;
    maxSideSpeed = 10;
	maxBackwardSpeed = 9;

 	maxForwardCrouchSpeed = 7;
	maxSideCrouchSpeed = 6;
    maxBackwardCrouchSpeed = 5;

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
	jumpSound = "";
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
	hMoveSlowdown = false;

	//Searching options
	hSearch = 1;//Search for Players
	hSearchRadius = 512;//in brick units
	hSight = 1;//Require bot to see player before pursuing
	hStrafe = 1;//Randomly strafe while following player
	hSearchFOV = 1;//if enabled disables normal hSearch
	hFOVRadius = 10;//max 10
	hAlertOtherBots = 1;//Alerts other bots when he sees a player, or gets attacked

	//Attack Options
	hMelee = 1;
	hAttackDamage = $Pref::L4B::Zombies::NormalDamage;
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

datablock fxDTSBrickData (BrickZombieCharger_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
	uiName = "Zombie Charger Hole";
	iconName = "Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/icon_charger";

	holeBot = "ZombieChargerHoleBot";
};

datablock TSShapeConstructor(ChargerMDts) 
{
	baseShape = "./models/zombie_charger.dts";
	sequence0 = "./models/default_old.dsq";
	sequence1 = "./models/zombie.dsq";
	sequence2 = "./models/charger_zombie.dsq";
};

datablock PlayerData(ZombieChargerHoleBot : CommonZombieHoleBot)
{
	shapeFile = "./models/zombie_charger.dts";
	uiName = "Charger Infected";
	minImpactSpeed = 25;
	airControl = 0.1;
	speedDamageScale = 0.2;
	maxdamage = 100;//Health
	hName = "Charger";//cannot contain spaces
	hTickRate = 4000;
	hMeleeCI = "Charger";
	hAttackDamage = $Pref::L4B::Zombies::SpecialsDamage;

	cameramaxdist = 4;
    cameraVerticalOffset = 1;
    cameraHorizontalOffset = 0.6;
    cameratilt = 0.1;
    maxfreelookangle = 2;

    maxForwardSpeed = 8;
    maxBackwardSpeed = 7;
    maxSideSpeed = 6;

 	maxForwardCrouchSpeed = 6;
    maxBackwardCrouchSpeed = 5;
    maxSideCrouchSpeed = 4;

	hIsInfected = 2;
	hZombieL4BType = "Special";
	resistMelee = true;
	hPinCI = "<bitmapk:Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_charger2>";
	SpecialCPMessage = "Right click to charge <br>\c6Charge to pin non-infected";
	hBigMeleeSound = "charger_punch1_sound";

	rechargeRate = 0.75;
	maxenergy = 100;
	showEnergyBar = true;
};

// Tank Datablocks
datablock TSShapeConstructor(RotZTankDts)
{
	baseshape  = "./models/zombie_tank.dts";
	sequence0  = "./models/tank.dsq root";
};

datablock fxDTSBrickData (BrickZombieTankBot_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
	uiName = "Zombie Tank Hole";
	iconName = "Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/icon_Tank";
	
	holeBot = "ZombieTankHoleBot";
};

datablock PlayerData(ZombieTankHoleBot : CommonZombieHoleBot)
{
	uiName = "Tank Infected";
	shapeFile = RotZTankDts.baseShape;
	maxDamage = 5000;//Health
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
	hTickRate = 4000;
	jumpSound = "";
	resistMelee = true;
	
	hSearchRadius = 512;//in brick units
	hStrafe = 1;//Randomly strafe while following player
	hSearchFOV = 1;//if enabled disables normal hSearch
	hMaxShootRange = 120;//The range in which the bot will shoot the player

	hAttackDamage = $Pref::L4B::Zombies::SpecialsDamage*2.5;
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
	hPinCI = "<bitmapk:gamemode_left4block/modules/add-ins/player_l4b/models/icons/ci_tank2>";
	hBigMeleeSound = "tank_punch_sound";
	SpecialCPMessage = "Right click to throw boulders";
};

datablock fxDTSBrickData (BrickHunter_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
	uiName = "Hunter Zombie Hole";
	iconName = "Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/icon_hunter";

	holeBot = "ZombieHunterHoleBot";
};

datablock TSShapeConstructor(ClawsMDts) 
{
	baseShape = "./models/zombie_claws.dts";
	sequence0 = "./models/default_old.dsq";
	sequence1 = "./models/zombie.dsq";
};

datablock PlayerData(ZombieHunterHoleBot : CommonZombieHoleBot)
{
	shapeFile = "./models/zombie_claws.dts";
	uiName = "Hunter Infected";
	speedDamageScale = 0;
	jumpForce = 90*8;

	cameramaxdist = 4;
    cameraVerticalOffset = 1;
    cameraHorizontalOffset = 0.6;
    cameratilt = 0.1;
    maxfreelookangle = 2;

    maxForwardSpeed = 8;
    maxBackwardSpeed = 7;
    maxSideSpeed = 6;

 	maxForwardCrouchSpeed = 6;
    maxBackwardCrouchSpeed = 5;
    maxSideCrouchSpeed = 4;

	hIsInfected = 2;
	hZombieL4BType = "Special";
	hPinCI = "<bitmapk:Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_hunter2>";
	SpecialCPMessage = "Hold shift, then press space to leap <br>\c6Pounce to pin non-infected";
	hBigMeleeSound = "";

	maxdamage = 100;//Health
	hTickRate = 4000;

	hName = "Hunter";//cannot contain spaces
	hStrafe = 1;//Randomly strafe while following player
	hAttackDamage = $Pref::L4B::Zombies::SpecialsDamage/2;

	rechargeRate = 1.75;
	maxenergy = 100;
	showEnergyBar = true;
};

datablock fxDTSBrickData (BrickZombieJockey_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
	uiName = "Zombie Jockey Hole";
	iconName = "Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/icon_Jockey";

	holeBot = "ZombieJockeyHoleBot";
};

datablock PlayerData(ZombieJockeyHoleBot : CommonZombieHoleBot)
{
	uiName = "Jockey Infected";
	jumpForce = 90*8;
	minImpactSpeed = 10;
	airControl = 0.1;
	speedDamageScale = 0.01;

    maxForwardSpeed = 10;
    maxBackwardSpeed = 8;
    maxSideSpeed = 9;

 	maxForwardCrouchSpeed = 7;
    maxBackwardCrouchSpeed = 5;
    maxSideCrouchSpeed = 6;

	cameramaxdist = 4;
    cameraVerticalOffset = 1;
    cameraHorizontalOffset = 0.6;
    cameratilt = 0.1;
    maxfreelookangle = 2;

	maxdamage = 100;//Health
	jumpForce = 100 * 10; //8.3 * 90;
	hTickRate = 4000;

	hName = "Jockey";//cannot contain spaces
	hAttackDamage = $Pref::L4B::Zombies::SpecialsDamage;
	
	hIsInfected = 2;
	hZombieL4BType = "Special";
	hPinCI = "<bitmapk:Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_jockey2>";
	SpecialCPMessage = "Right click to leap <br>\c6Jump on non-infected to control them";
	hBigMeleeSound = "";

	rechargeRate = 1.5;
	maxenergy = 100;
	showEnergyBar = true;
};

datablock fxDTSBrickData (BrickZombieSpitter_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
	uiName = "Zombie Spitter Hole";
	iconName = "Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/icon_Spitter";

	holeBot = "ZombieSpitterHoleBot";
};

datablock PlayerData(ZombieSpitterHoleBot : CommonZombieHoleBot)
{
	uiName = "Spitter Infected";
	minImpactSpeed = 16;
	speedDamageScale = 2;

	maxdamage = 100;//Health

    maxForwardSpeed = 8;
    maxBackwardSpeed = 7;
    maxSideSpeed = 6;

 	maxForwardCrouchSpeed = 6;
    maxBackwardCrouchSpeed = 5;
    maxSideCrouchSpeed = 4;

	cameramaxdist = 4;
    cameraVerticalOffset = 1;
    cameraHorizontalOffset = 0.6;
    cameratilt = 0.1;
    maxfreelookangle = 2;

	hName = "Spitter";//cannot contain spaces
	hTickRate = 4000;
	hShoot = 1;
	hMaxShootRange = 100;//The range in which the bot will shoot the player
	hTooCloseRange = 50;//in brick units
	hAttackDamage = $Pref::L4B::Zombies::SpecialsDamage;

	hIsInfected = 1;
	hZombieL4BType = "Special";
	hPinCI = "<bitmapk:Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_spitter2>";
	SpecialCPMessage = "Right click to spit";
	hBigMeleeSound = "";
	hNeedsWeapons = 1;

	rechargeRate = 0.75;
	maxenergy = 100;
	showEnergyBar = true;
};

datablock fxDTSBrickData (BrickZombieSmoker_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
	uiName = "Zombie Smoker Hole";
	iconName = "Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/icon_smoker";

	holeBot = "ZombieSmokerHoleBot";
};

datablock TSShapeConstructor(SmokerMDts) 
{
	baseShape = "./models/zombie_smoker.dts";
	sequence0 = "./models/default_old.dsq";
	sequence1 = "./models/zombie.dsq";
};

datablock PlayerData(ZombieSmokerHoleBot : CommonZombieHoleBot)
{
	shapeFile = "./models/zombie_smoker.dts";
	uiName = "Smoker Infected";
	minImpactSpeed = 24;
	speedDamageScale = 2;

	maxdamage = 100;//Health

	hName = "Smoker";//cannot contain spaces
	hAttackDamage = $Pref::L4B::Zombies::SpecialsDamage/2.5;
	hTickRate = 4000;

    maxForwardSpeed = 8;
    maxBackwardSpeed = 7;
    maxSideSpeed = 6;

 	maxForwardCrouchSpeed = 6;
    maxBackwardCrouchSpeed = 5;
    maxSideCrouchSpeed = 4;

	cameramaxdist = 4;
    cameraVerticalOffset = 0.9;
    cameraHorizontalOffset = 0.6;
    cameratilt = 0.1;
    maxfreelookangle = 2;

	hIsInfected = 2;
	hZombieL4BType = "Special";
	hPinCI = "<bitmapk:Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_smoker2>";
	SpecialCPMessage = "Right click to shoot your tongue <br>\c6Pin non-infected by hitting them with your tongue";
	hBigMeleeSound = "";
	hNeedsWeapons = 1;

	rechargeRate = 0.75;
	maxenergy = 100;
	showEnergyBar = true;
};

datablock fxDTSBrickData (BrickZombieBoomer_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
	uiName = "Zombie Boomer Hole";
	iconName = "Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/icon_boomer";
	holeBot = "ZombieBoomerHoleBot";
};

datablock TSShapeConstructor(BoomerMDts) 
{
	baseShape = "./models/zombie_boomer.dts";
	sequence0 = "./models/default_old.dsq";
	sequence1 = "./models/zombie.dsq";
	sequence2 = "./models/boomer_zombie.dsq";
};

datablock PlayerData(ZombieBoomerHoleBot : CommonZombieHoleBot)
{
	shapeFile = BoomerMDts.baseShape;
	uiName = "Boomer Infected";
	jumpForce = 9*175;
	minImpactSpeed = 20;
	airControl = 0.01;
	speedDamageScale = 10;
	mass = 250;

	cameramaxdist = 2;
    cameraVerticalOffset = 1.1;
    cameraHorizontalOffset = 0.6;
    cameratilt = 0.1;
    maxfreelookangle = 2;

    maxForwardSpeed = 8;
    maxBackwardSpeed = 6;
    maxSideSpeed = 7;

 	maxForwardCrouchSpeed = 6;
    maxBackwardCrouchSpeed = 4;
    maxSideCrouchSpeed = 5;
	
	hIsInfected = 1;
	hZombieL4BType = "Special";
	hPinCI = "<bitmapk:Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_boomer2>";
	SpecialCPMessage = "Right click to vomit";
	hBigMeleeSound = "";
	hNeedsWeapons = 1;
	maxdamage = 100;
	hTickRate = 4000;
	hShoot = 1;
	hMaxShootRange = 2.5;//The range in which the bot will shoot the player
	hMoveSlowdown = 1;
	hName = "Boomer";//cannot contain spaces
	hAttackDamage = $Pref::L4B::Zombies::SpecialsDamage/3;

	rechargeRate = 0.5;
	maxenergy = 100;
	showEnergyBar = true;
};

datablock TSShapeConstructor(UncommonMDts) 
{
	baseShape = "./models/zombie_uncommon.dts";
	sequence0 = "./models/default_old.dsq";
	sequence1 = "./models/zombie.dsq";
};

datablock PlayerData(ZombieSoldierHoleBot : CommonZombieHoleBot)
{
	shapeFile = "./models/zombie_uncommon.dts";
	uiName = "";
	hName = "Infected" SPC "Soldier";//cannot contain spaces//except it can lmao

	hIsInfected = 1;
	hZombieL4BType = "Normal";
	hPinCI = "";
	hBigMeleeSound = "";
	maxdamage = 250;//Health

	hShootTimes = 4;
	hMaxShootRange = 60;
};

datablock PlayerData(ZombieConstructionHoleBot : CommonZombieHoleBot)
{
	shapeFile = "./models/zombie_uncommon.dts";
	uiName = "";
	hName =  "Infected" SPC "Construction";//cannot contain spaces
	hIsInfected = 1;
	hZombieL4BType = "Normal";
	hPinCI = "";
	hBigMeleeSound = "";
};

datablock fxDTSBrickData (BrickZombieWitch_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
    uiName = "Zombie Witch Hole";
    iconName = "Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/icon_witch";
    
    holeBot = "ZombieWitchHoleBot";
};

datablock PlayerData(ZombieWitchHoleBot : CommonZombieHoleBot)
{
    shapeFile = "./models/zombie_claws.dts";
    uiName = "";
    maxdamage = 2500;//Health
    hTickRate = 4000;

    hName = "Witch";//cannot contain spaces
    hMeleeCI = "Witched";
    hZombieL4BType = "Special";
	hSearch = false;
    runforce = 100 * 75;
    hAttackDamage = $Pref::L4B::Zombies::SpecialsDamage*5;//15;//Melee Damage
    resistMelee = true;
};

datablock DebrisData(skeleheadDebris)
{
   shapeFile = "./models/skeleton_head.dts";
   lifetime = 6.0;
   minSpinSpeed = -400.0;
   maxSpinSpeed = 200.0;
   elasticity = 0.5;
   friction = 0.2;
   numBounces = 3;
   staticOnMaxBounce = true;
   snapOnMaxBounce = false;
   fade = true;
   gravModifier = 2;
};
datablock DebrisData(handDebris : skeleheadDebris)
{
   shapeFile = "./models/skeleton_hand.dts";
   lifetime = 6.0;
   minSpinSpeed = -400.0;
   maxSpinSpeed = 200.0;
   elasticity = 0.5;
   friction = 0.2;
   numBounces = 3;
   staticOnMaxBounce = true;
   snapOnMaxBounce = false;
   fade = true;
   gravModifier = 2;
};
datablock DebrisData(handLeftDebris : skeleheadDebris)
{
   shapeFile = "./models/skeleton_handLeft.dts";
   lifetime = 6.0;
   minSpinSpeed = -400.0;
   maxSpinSpeed = 200.0;
   elasticity = 0.5;
   friction = 0.2;
   numBounces = 3;
   staticOnMaxBounce = true;
   snapOnMaxBounce = false;
   fade = true;
   gravModifier = 2;
};

datablock TSShapeConstructor(mSkeletonMDts)
{
	baseShape = "./models/skeleton.dts";
	sequence0 = "./models/default_old.dsq";
	sequence1 = "./models/zombie.dsq";
	sequence2 = "./models/survivor.dsq";
};

datablock PlayerData(SkeletonHoleBot : CommonZombieHoleBot)
{
	shapeFile = "./models/skeleton.dts";
	DeathSound	= "skele_death_sound";
	enableRBlood = false;
	uiName = "";

	hShoot = 1;
	hShootTimes = 16;//Number of times the bot will shoot between each tick
	hMaxShootRange = 256;//The range in which the bot will shoot the player
	hAvoidCloseRange = 1;//
	hTooCloseRange = 7;//in brick units	
};

datablock fxDTSBrickData (BrickSkeletonBot_HoleSpawnData)
{
	brickFile = "Add-ons/Bot_Hole/4xSpawn.blb";
	category = "Special";
	subCategory = "Holes";
	uiName = "Skeleton Hole";
	iconName = "Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/icon_skeleton";

	bricktype = 2;
	cancover = 0;
	orientationfix = 1;
	indestructable = 1;

	isBotHole = 1;
	holeBot = "SkeletonHoleBot";
};

%imageskinpath = "add-ons/gamemode_left4block/modules/add-ins/player_l4b/models/hats/*.png";
for(%imageskinfile = findFirstFile(%imageskinpath); %imageskinfile !$= ""; %imageskinfile = findNextFile(%imageskinpath))
eval("addExtraResource(\""@ %imageskinfile @ "\");");


datablock shapeBaseImageData(scouthatimage)
{
	shapefile = "./models/hats/scouthat.dts";
	mountPoint = 6;
	offset = "0 0 0";
	eyeOffset = "0 0 10";
	doColorShift = false;
	className = "WeaponImage";
	armReady = false;
};

datablock shapeBaseImageData(knithatimage : scouthatimage)
{
	shapefile = "./models/hats/knitHat.dts";
	offset = "0 0 0";
};

datablock shapeBaseImageData(helmetimage : scouthatimage)
{
	shapefile = "./models/hats/helmet.dts";
	offset = "0 0 0";
};

datablock shapeBaseImageData(fedorahatimage : scouthatimage)
{
	shapefile = "./models/hats/fedorahat.dts";
	offset = "0 0 0.05";
};

datablock shapeBaseImageData(fancyhatimage : scouthatimage)
{
	shapefile = "./models/hats/fancyhat.dts";
	offset = "0 0 0";
};

datablock shapeBaseImageData(hoodieimage : scouthatimage)
{
	shapefile = "./models/hats/hoodie.dts";
	offset = "0 0 0";
};

datablock shapeBaseImageData(detectivehatimage : scouthatimage)
{
	shapefile = "./models/hats/detectivehat.dts";
	offset = "0 0 0";
};

datablock shapeBaseImageData(cophatimage : scouthatimage)
{
	shapefile = "./models/hats/cophat.dts";
	offset = "0 0 0";
};

datablock shapeBaseImageData(constructionhelmetimage : scouthatimage)
{
	shapefile = "./models/hats/constructionhelmet.dts";
	offset = "0 0 0";
};

datablock shapeBaseImageData(caphatimage : scouthatimage)
{
	shapefile = "./models/hats/caphat.dts";
	offset = "0 0 0";
};

datablock shapeBaseImageData(armorhelmetimage : scouthatimage)
{
	shapefile = "./models/hats/armorhelmet.dts";
	offset = "0 0 0";
};