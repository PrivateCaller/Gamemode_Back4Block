AddDamageType("Crowbar",'<bitmap:Add-Ons/Gamemode_Left4Block/modules/add-ins/weapon_l4b/icons/ci_crowbar> %1','%2 <bitmap:Add-Ons/Gamemode_Left4Block/modules/add-ins/weapon_l4b/icons/ci_crowbar> %1',0.2,1);
AddDamageType("BoulderDirect",'<bitmap:Add-ons/Gamemode_Left4Block/modules/add-ins/weapon_l4b/icons/CI_Boulder> %1',       '%2 <bitmap:Add-ons/Gamemode_Left4Block/modules/add-ins/weapon_l4b/icons/CI_Boulder> %1',1,1);
AddDamageType("BoulderRadius",'<bitmap:Add-ons/Gamemode_Left4Block/modules/add-ins/weapon_l4b/icons/CI_Boulder> %1', '%2 <bitmap:Add-ons/Gamemode_Left4Block/modules/add-ins/weapon_l4b/icons/CI_Boulder> %1',1,0);
AddDamageType("Molotov",'<bitmap:Add-Ons/Gamemode_Left4Block/modules/add-ins/weapon_l4b/icons/ci_molotov> %1','%2 <bitmap:Add-Ons/Gamemode_Left4Block/modules/add-ins/weapon_l4b/icons/ci_molotov> %1',1,1);
AddDamageType("Flamer",'<bitmap:Add-Ons/Gamemode_Left4Block/modules/add-ins/weapon_l4b/icons/ci_fire> %1','%2 <bitmap:Add-Ons/Gamemode_Left4Block/modules/add-ins/weapon_l4b/icons/ci_fire> %1',1,1);

%pattern = "add-ons/gamemode_left4block/modules/add-ins/weapon_l4b/sound/*.wav";
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

datablock ParticleData(flamerFleshBurningParticle)
{
	dragCoefficient      = 1;
	gravityCoefficient   = -3;
	inheritedVelFactor   = 0.7;
	constantAcceleration = 0.0;
	lifetimeMS           = 425;
	lifetimeVarianceMS   = 355;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 16.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.7 0.6 0.3 0.5";
	colors[1]     = "0.9 0.4 0.1 0.8";
	colors[2]     = "0.9 0.2 0.1 0.2";
	colors[3]     = "0.9 0.1 0.1 0";
	sizes[0]      = 0.75;
	sizes[1]      = 0.97;
	sizes[2]      = 1.45;
	sizes[3]      = 1.85;
   times[0] = 0.0;
   times[1] = 0.3;
   times[2] = 0.6;
   times[3] = 1.0;

	useInvAlpha = false;
};
datablock ParticleEmitterData(flamerFleshBurningEmitter)
{
	ejectionPeriodMS = 4;
	periodVarianceMS = 0;
	ejectionVelocity = 1;
	velocityVariance = 0.4;
	ejectionOffset   = 0.4;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	orientOnVelocity = true;
	particles = flamerFleshBurningParticle;
};

datablock ParticleData(flamerRubbishBurningParticle)
{
	dragCoefficient      = 1;
	gravityCoefficient   = -2;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 525;
	lifetimeVarianceMS   = 455;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 16.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.7 0.6 0.3 0.5";
	colors[1]     = "0.9 0.4 0.1 0.8";
	colors[2]     = "0.9 0.2 0.1 0.2";
	colors[3]     = "0.9 0.1 0.1 0";
	sizes[0]      = 0.67;
	sizes[1]      = 1.1;
	sizes[2]      = 1.2;
	sizes[3]      = 1.5;
	times[0] = 0.0;
	times[1] = 0.3;
	times[2] = 0.6;
	times[3] = 1.0;

	useInvAlpha = false;
};
datablock ParticleEmitterData(flamerRubbishBurningEmitter)
{
   ejectionPeriodMS = 12;
   periodVarianceMS = 0;
   ejectionVelocity = 5;
   velocityVariance = 1;
   ejectionOffset   = 0;
   thetaMin         = 0;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   orientOnVelocity = true;
   particles = flamerRubbishBurningParticle;
};

datablock projectileData(flamerRubbishProjectile)
{
	projectileShapeName 	= "base/data/shapes/empty.dts";
	particleEmitter     	= flamerRubbishBurningEmitter;
	isBallistic 			= 1;
	gravityMod 				= 1;
	lifeTime 				= 12000;
	explodeOnDeath 			= 1;
	explosion 				= "";
	muzzleVelocity = 5;
};

datablock ShapeBaseImageData(flamerFleshBurningImage)
{
	emap = false;
	mountPoint = $backSlot;
	eyeOffset = "0 0 -0.4";
	doColorShift = true;
	shapeFile = "base/data/shapes/empty.dts";
	offset = "0 0 -0.8";
	rotation = "1 0 0 90";
	
	stateName[0]					= "FireA";
	stateEmitter[0]					= flamerFleshBurningEmitter;
	stateEmitterTime[0]				= 100;
	stateTransitionOnTimeout[0]     = "FireB";
	stateTimeoutValue[0]            = 100;
	
	stateName[1]					= "FireB";
	stateEmitter[1]					= flamerFleshBurningEmitter;
	stateEmitterTime[1]				= 100;
	stateTransitionOnTimeout[1]     = "FireA";
	stateTimeoutValue[1]            = 100;
};

datablock ParticleData(molotovExplosionSmokeParticle)
{
	dragCoefficient      = 5;
	gravityCoefficient   = -0.1;
	inheritedVelFactor   = 0;
	constantAcceleration = 0.0;
	lifetimeMS           = 4700;
	lifetimeVarianceMS   = 900;
	textureName          = "base/data/particles/cloud";
	spinSpeed			= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]			= "0.1 0.1 0.1 0.2";
	colors[1]			= "0.2 0.2 0.2 0.1";
	colors[2]			= "0.4 0.4 0.4 0";
	sizes[0]			= 2;
	sizes[1]			= 5;
	sizes[2]			= 7;
	times[0]			= 0;
	times[1]			= 0.1;
	times[2]			= 1;
	useInvAlpha 		= true;
};

datablock ParticleEmitterData(molotovExplosionSmokeEmitter)
{
	ejectionPeriodMS	= 3;
	periodVarianceMS	= 0;
	ejectionVelocity	= 4;
	velocityVariance	= 0;
	ejectionOffset  	= 0.1;
	thetaMin			= 0;
	thetaMax			= 90;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= molotovExplosionSmokeParticle;
};
datablock explosionData(molotovExplosion)
{
	lifetimeMS 				= 150;
	
	particleEmitter 		= molotovExplosionSmokeEmitter;
	particleDensity 		= 12;
	particleRadius 			= 0.5;
	soundProfile 			= "";

	
	emitter[0]				= "";
	
	debris 					= "";
	debrisNum 				= 0;
	debrisNumVariance 		= 0;

	radiusDamage = 0;
	damageRadius = 0;
	
	impulseForce = 0;
	impulseRadius = 0;
	
	faceViewer = 1;
	
	explosionScale = "1 1 1";
	
	shakeCamera = true;
	camShakeRadius = 20;
	camShakeAmp = "1 2 1";
	camShakeDuration = 3;
	camShakeFallOff = 10;
	camShakeFreq = "2 4 2";
	cameraShakeFalloff = 1;
};
datablock ParticleData(molotovSparkParticle)
{
	dragCoefficient      = 1;
	gravityCoefficient   = -1;
	inheritedVelFactor   = 0.4;
	constantAcceleration = 0.0;
	lifetimeMS           = 425;
	lifetimeVarianceMS   = 255;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 16.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.7 0.6 0.3 0.8";
	colors[1]     = "0.9 0.4 0.1 0.5";
	colors[2]     = "0.9 0.2 0.1 0.2";
	colors[3]     = "0.9 0.1 0.1 0";
	sizes[0]      = 0.18;
	sizes[1]      = 0.22;
	sizes[2]      = 0.29;
	sizes[3]      = 0.17;
   times[0] = 0.0;
   times[1] = 0.3;
   times[2] = 0.6;
   times[3] = 1.0;

	useInvAlpha = false;
};
datablock ParticleEmitterData(molotovSparkEmitter)
{
   ejectionPeriodMS = 8;
   periodVarianceMS = 0;
   ejectionVelocity = 0.1;
   velocityVariance = 0;
   ejectionOffset   = 0.05;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   orientOnVelocity = true;
   particles = molotovSparkParticle;
};
datablock ProjectileData(molotovProjectile : gunProjectile)
{
	projectileShapeName 	= "./models/molotov_proj.dts";
	directDamage        	= 5;
	explodeOnDeath = 1;
	explosion 				= molotovExplosion;
	directDamageType    	= $DamageType::Flamer;
	radiusDamageType    	= $DamageType::Flamer;
	particleEmitter     	= molotovSparkEmitter;
	uiName 					= "";
	muzzleVelocity 			= 35;
	verticalImpulse 		= 20;
	impactImpulse			= 20;
	lifeTime				= 20000;
	sound 					= "";
	sProjectile 			= 0;
	noBulletWhiz = 1;
	gravityMod 				= 1;
	isBallistic 			= 1;
	
	sound = molotovLoopSound;
	
	impactImpulse = 0;
	verticalImpulse = 0;
};
datablock ItemData(molotovItem)
{
	category = "Weapon";
	className = "Weapon";
	
	weaponClass = "slot3";

	shapeFile = "./models/molotov.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Molotov";
	iconName = "./icons/icon_molotov";
	fakeIcon = c4Icon;
	doColorShift = true;
	colorShiftColor = "0.4 0.2 0 1";
	ammo = 1;
	canPutInSuitcase = 0;
	isLargeWeapon = 0;

	image = molotovImage;
	canDrop = true;
};
datablock ShapeBaseImageData(molotovImage)
{
	className = "WeaponImage";
	shapeFile = "./models/molotov.dts";
	emap = true;
	mountPoint = 0;
	offset = "-0.01 0.035 0";
	eyeOffset = "0 0 0";
	rotation = "0 0 1 20";
	eyeRotation = "0 0 0 0";
	item = molotovItem;
	ammo = " ";
	
	armReady = true;
	
	projectile = molotovProjectile;
	projectileType = Projectile;
	
	
	doColorShift = true;
	colorShiftColor = molotovItem.colorShiftColor;

	stateName[0]			= "Activate";
	stateTimeoutValue[0]		= 0.3;
	stateTransitionOnTimeout[0]	= "FlickA";
	stateSequence[0]		= "ready";
	stateSound[0]			= weaponSwitchSound;
	
	stateName[1]			= "FlickA";
	stateTimeoutValue[1]		= 0.1;
	stateTransitionOnTimeout[1]	= "FlickB";
	stateSequence[1]		= "ready";
	stateScript[1]			= "onFlick";
	stateEmitter[1]                 = molotovSparkEmitter;
	stateEmitterNode[1]             = "mountPoint";
	stateEmitterTime[1]             = "2";
	stateSound[1]			= "molotov_light_sound";
	
	stateName[7]			= "FlickB";
	stateTimeoutValue[7]		= 0.1;
	stateTransitionOnTimeout[7]	= "Ready";
	stateSequence[7]		= "ready";
	stateEmitter[7]                 = molotovSparkEmitter;
	stateEmitterNode[7]             = "mountPoint";
	stateEmitterTime[7]             = "2";
	stateSound[7]			= "molotov_loop_sound";

	stateName[2]			= "Ready";
	stateTransitionOnTriggerDown[2]	= "Charge";
	stateTransitionOnTimeout[2]	= "Ready";
	stateTimeoutValue[2]		= 0.15;
	stateWaitForTimeout[2]		= 0;
	stateAllowImageChange[2]	= true;
	stateEmitter[2]                 = molotovSparkEmitter;
	stateEmitterNode[2]             = "mountPoint";
	stateEmitterTime[2]             = "2";
	stateSound[2]			= "molotov_loop_sound";
	
	stateName[3]                    = "Charge";
	stateTransitionOnTimeout[3]	= "Armed";
	stateTimeoutValue[3]            = 0.25;
	stateWaitForTimeout[3]		= false;
	stateTransitionOnTriggerUp[3]	= "AbortCharge";
	stateScript[3]                  = "onCharge";
	stateAllowImageChange[3]        = false;
	stateEmitter[3]                 = molotovSparkEmitter;
	stateEmitterNode[3]             = "mountPoint";
	stateEmitterTime[3]             = "0.6";
	stateSound[3]			= "molotov_loop_sound";
	
	stateName[4]			= "AbortCharge";
	stateTransitionOnTimeout[4]	= "Ready";
	stateTimeoutValue[4]		= 0.1;
	stateWaitForTimeout[4]		= true;
	stateScript[4]			= "onAbortCharge";
	stateAllowImageChange[4]	= false;
	stateEmitter[4]                 = molotovSparkEmitter;
	stateEmitterNode[4]             = "mountPoint";
	stateEmitterTime[4]             = "0.4";
	stateSound[4]			= "molotov_loop_sound";

	stateName[5]			= "Armed";
	stateWaitFotTimeout[5]		= 0;
	stateTransitionOnTimeout[5]	= "Armed";
	stateTimeoutValue[5]		= 0.1;
	stateTransitionOnTriggerUp[5]	= "Fire";
	stateAllowImageChange[5]	= false;
	stateEmitter[5]                 = molotovSparkEmitter;
	stateEmitterNode[5]             = "mountPoint";
	stateEmitterTime[5]             = "0.2";
	stateSound[5]			= "molotov_loop_sound";

	stateName[6]			= "Fire";
	stateTransitionOnTimeout[6]	= "FlickA";
	stateTimeoutValue[6]		= 1.5;
	stateFire[6]			= true;
	stateSequence[6]		= "ready";
	stateScript[6]			= "onFire";
	stateWaitForTimeout[6]		= true;
	stateAllowImageChange[6]	= false;
	stateSound[6]			= "";
};

datablock PlayerData(emptyPlayer : playerStandardArmor)
{
	shapeFile = "base/data/shapes/empty.dts";
	boundingBox = "0.01 0.01 0.01";
	crouchboundingBox = "0.01 0.01 0.01";
	isEmptyPlayer = true;
	deathSound = "";
	painSound = "";
	useCustomPainEffects = true;
	mountSound = "";
	jumpSound = "";
	uiName = "";
};

datablock ParticleData(flamerBurningParticle)
{
	dragCoefficient      = 1;
	gravityCoefficient   = -1;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 525;
	lifetimeVarianceMS   = 455;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 16.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.7 0.6 0.3 0.5";
	colors[1]     = "0.9 0.4 0.1 0.8";
	colors[2]     = "0.9 0.2 0.1 0.2";
	colors[3]     = "0.9 0.1 0.1 0";
	sizes[0]      = 0.45;
	sizes[1]      = 0.67;
	sizes[2]      = 1.05;
	sizes[3]      = 1.85;
   times[0] = 0.0;
   times[1] = 0.3;
   times[2] = 0.6;
   times[3] = 1.0;

	useInvAlpha = false;
};
datablock ParticleEmitterData(flamerBurningAEmitter)
{
   ejectionPeriodMS = 12;
   periodVarianceMS = 0;
   ejectionVelocity = 5;
   velocityVariance = 1;
   ejectionOffset   = 0.1;
   thetaMin         = 0;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   orientOnVelocity = true;
   particles = flamerBurningParticle;
};
datablock ParticleEmitterData(flamerBurningBEmitter)
{
   ejectionPeriodMS = 12;
   periodVarianceMS = 0;
   ejectionVelocity = 0;
   velocityVariance = 0;
   ejectionOffset   = 1;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   orientOnVelocity = true;
   particles = flamerBurningParticle;
};
datablock ParticleData(flamerSparkParticle)
{
	dragCoefficient      = 4;
	gravityCoefficient   = -0.4;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 325;
	lifetimeVarianceMS   = 255;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 16.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.7 0.6 0.3 0.8";
	colors[1]     = "0.9 0.4 0.1 0.8";
	colors[2]     = "0.9 0.2 0.1 0.2";
	colors[3]     = "0.9 0.1 0.1 0";
	sizes[0]      = 0.15;
	sizes[1]      = 0.17;
	sizes[2]      = 0.26;
	sizes[3]      = 0.14;
   times[0] = 0.0;
   times[1] = 0.3;
   times[2] = 0.6;
   times[3] = 1.0;

	useInvAlpha = false;
};
datablock ParticleEmitterData(flamerSparkEmitter)
{
   ejectionPeriodMS = 8;
   periodVarianceMS = 0;
   ejectionVelocity = 9;
   velocityVariance = 0.2;
   ejectionOffset   = 0;
   thetaMin         = 0;
   thetaMax         = 20;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   orientOnVelocity = true;
   particles = flamerSparkParticle;
};
datablock DebrisData(flamerSparkDebris)
{
	shapeFile = "base/data/shapes/empty.dts";
	lifetime = 2;
	minSpinSpeed = -400.0;
	maxSpinSpeed = 200.0;
	elasticity = 0.5;
	friction = 0;
	numBounces = 1;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 6;
	emitters[0] = flamerSparkEmitter;
};
datablock explosionData(flamerSparkExplosion)
{
	lifetimeMS = 50;
	debris 					= flamerSparkDebris;
	debrisNum 				= 4;
	debrisNumVariance 		= 3;
	debrisPhiMin 			= 0;
	debrisPhiMax 			= 360;
	debrisThetaMin 			= 140;
	debrisThetaMax 			= 180;
	debrisVelocity 			= 6;
	debrisVelocityVariance 	= 2;
	explosionScale = "0.1 0.1 0.1";
	faceViewer = 1;
	offset = 0.3;
};
datablock explosionData(flamerExplosion)
{
	soundProfile = "";
	lifetimeMS = 4000;
	emitter[0] = flamerBurningAEmitter;
	emitter[1] = flamerBurningBEmitter;
	subExplosion[0] = flamerSparkExplosion;
};
datablock projectileData(flamerFakeProjectile)
{
	explosion = flamerExplosion;
	lifetimeMS = 10000;
};


datablock ParticleData(meleeTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 1.5;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 1500;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;

	textureName		= "base/data/particles/dot";
	colors[0]	= "2 2 2 0.0025";
	colors[1]	= "2 2 2 0.0";
	sizes[0]	= 0.4;
	sizes[1]	= 0.1;
	times[0]	= 0.5;
	times[1]	= 0.1;
};

datablock ParticleEmitterData(meleeTrailEmitter)
{
   ejectionPeriodMS = 2;
   periodVarianceMS = 0;
   ejectionVelocity = 0; //0.25;
   velocityVariance = 0; //0.10;
   ejectionOffset = 0;
   thetaMin         = 0.0;
   thetaMax         = 90.0;  
   particles = meleeTrailParticle;
   useEmitterColors = true;
   uiName = "";
};

datablock ItemData(crowbarItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system
	shapeFile = "./models/model_crowbar.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	uiName = "Crowbar";
	iconName = "./icons/icon_crowbar";
	doColorShift = true;
	colorShiftColor = "0.5 0.5 0.5 1";
	image = crowbarImage;
	canDrop = true;
};

datablock ShapeBaseImageData(crowbarImage)
{
	shapeFile = "./models/model_crowbar.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	correctMuzzleVector = false;
	eyeOffset = "0 0 0";
	className = "WeaponImage";
	item = crowbarItem;
	ammo = " ";
	projectile = "";
	projectileType = Projectile;
	melee = true;
	hasLunge = true;
	doRetraction = false;
	armReady = false;
	doColorShift = crowbarItem.doColorShift;
	colorShiftColor = crowbarItem.colorShiftColor;

	meleeDamageDivisor = 2;
	meleeDamageType = $DamageType::Crowbar;
	meleeHitEnvSound = "crowbar";
	meleeHitPlSound = "crowbar";

	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0;
	stateTransitionOnTimeout[0]      = "Ready";
	stateSound[0]                    = WeaponSwitchsound;

	stateName[1]                     = "Ready";
	stateScript[1]                  = "onReady";
	stateTransitionOnTriggerDown[1]  = "PreFire";
	stateAllowImageChange[1]         = true;

	stateName[2]					= "PreFire";
	stateScript[2]                  = "onPreFire";
	stateAllowImageChange[2]        = false;
	stateTimeoutValue[2]            = 0.05;
	stateTransitionOnTimeout[2]     = "Fire";

	stateName[3]                    = "Fire";
	stateTransitionOnTimeout[3]     = "CheckFire";
	stateFire[3]                    = false;
	stateScript[3]                  = "onFire";
	stateTimeoutValue[3]            = 0.1;
	stateEmitter[3]					= "meleeTrailEmitter";
	stateEmitterNode[3]             = "muzzlePoint";
	stateEmitterTime[3]             = "0.225";

	stateName[4]			= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "StopFire";

	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Break";
	stateTimeoutValue[5]            = 0.1925;
	stateScript[5]                  = "onStopFire";
	stateEmitter[5]					= "";
	stateEmitterNode[5]             = "muzzlePoint";
	stateEmitterTime[5]             = "0.1";

	stateName[6]                    = "Break";
	stateTransitionOnTimeout[6]     = "Ready";
	stateTimeoutValue[6]            = 0.3;
};

datablock DebrisData(boulder1debris)
{
   emitters = "";

	shapeFile = "./models/boulderpiece1.dts";
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

	gravModifier = 1;
};
datablock ExplosionData(boulder1debrisExplosion)
{
   particleEmitter = "";

   debris = boulder1debris;
   debrisNum = 2;
   debrisNumVariance = 6;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 3;
   debrisVelocityVariance = 1;
};

datablock DebrisData(boulder2debris)
{
   emitters = "";

	shapeFile = "./models/boulderpiece2.dts";
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

	gravModifier = 1;
};
datablock ExplosionData(boulder2debrisExplosion)
{
   particleEmitter = "";

   debris = boulder2debris;
   debrisNum = 2;
   debrisNumVariance = 6;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 3;
   debrisVelocityVariance = 1;
};

datablock DebrisData(boulder3debris)
{
   emitters = "";

	shapeFile = "./models/boulderpiece3.dts";
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

	gravModifier = 1;
};
datablock ExplosionData(boulder3debrisExplosion)
{
   particleEmitter = "";

   debris = boulder3debris;
   debrisNum = 2;
   debrisNumVariance = 6;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 3;
   debrisVelocityVariance = 1;
};

datablock DebrisData(boulder4debris)
{
   emitters = "";

	shapeFile = "./models/boulderpiece4.dts";
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

	gravModifier = 1;
};
datablock ExplosionData(boulder4debrisExplosion)
{
   particleEmitter = "";

   debris = boulder4debris;
   debrisNum = 2;
   debrisNumVariance = 6;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 3;
   debrisVelocityVariance = 1;
};

datablock ExplosionData(BoulderExplosion : spearExplosion)
{
   soundProfile = "boulder_hit_sound";

   damageRadius = 3;
   radiusDamage = 100;

   impulseRadius = 5;
   impulseForce = 1000;

   //subExplosion[0] = boulder1debrisExplosion;
   //subExplosion[1] = boulder2debrisExplosion;
   //subExplosion[2] = boulder3debrisExplosion;
   //subExplosion[3] = boulder4debrisExplosion;

   faceViewer     = true;
   explosionScale = "5 5 5";

   shakeCamera = true;
   camShakeFreq = "2 2 2";
   camShakeAmp = "1.25 1.25 1.25";
   camShakeDuration = 4;
   camShakeRadius = 1.25;
};

datablock ExplosionData(BoulderExplosion1 : spearExplosion)
{
   soundProfile = "";
   //impulse
   impulseRadius = 0;
   impulseForce = 0;

   //radius damage
   radiusDamage        = 0;
   damageRadius        = 0;
};

//spear trail
datablock ParticleData(boulderTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 600;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/cloud";
	//animTexName		= " ";

	// Interpolation variables
	colors[0]	= "0.5 0.5 0.5 0.1";
	colors[1]	= "0.25 0.25 0.25 0.05";
	colors[2]	= "0.05 0.05 0.05 0";
	sizes[0]	= 3;
	sizes[1]	= 1;
	sizes[2]	= 0.1;
	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(boulderTrailEmitter)
{
   ejectionPeriodMS = 5;
   periodVarianceMS = 0;

   ejectionVelocity = 0.25; //0.25;
   velocityVariance = 0.10; //0.10;

   ejectionOffset = 0;

   thetaMin         = 0.0;
   thetaMax         = 90.0;  

   particles = boulderTrailParticle;

   useEmitterColors = true;
   uiName = "";
};

datablock ItemData(PropaneTankItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/propanetankbox.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Propane Tank";
	iconName = "./icons/Icon_Propane";
	doColorShift = true;
	colorShiftColor = "0.471 0.471 0.471 1.000";

	image = PropaneTankImage;
	canDrop = true;
	throwableExplosive = 1;
};

datablock ShapeBaseImageData(PropaneTankImage)
{
   	shapeFile = "./models/propanetank.dts";
   	emap = true;
   	vehicle = PropaneTankCol;

   	mountPoint = 0;
   	offset = "-0.53 0.05 -0.6";
   	rotation = "0 0 1 180";

   	correctMuzzleVector = true;
   	className = "WeaponImage";

   	item = PropaneTankItem;
   	ammo = " ";
   	projectile = "";
   	projectileType = "";

   	melee = false;
   	armReady = 2;

	doColorShift = true;
	colorShiftColor = "0.471 0.471 0.471 1.000";

	stateName[0]			= "Activate";
	stateTimeoutValue[0]		= 0.1;
	stateTransitionOnTimeout[0]	= "Ready";
	stateSequence[0]		= "ready";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]			= "Ready";
	stateTransitionOnTriggerDown[1]	= "Fire";
	stateAllowImageChange[1]	= true;

	stateName[5]			= "Fire";
	stateTransitionOnTimeout[5]	= "Ready";
	stateTimeoutValue[5]		= 0.5;
	stateFire[5]			= true;
	stateSequence[5]		= "fire";
	stateScript[5]			= "onFire";
	stateWaitForTimeout[5]		= true;
	stateAllowImageChange[5]	= false;
};

datablock WheeledVehicleData(PropaneTankCol)
{
	category = "Vehicles";
	displayName = "";
	shapeFile = "./models/PropaneTankCol.dts";
	emap = true;
	minMountDist = 0;
   
   	numMountPoints = 0;

	image = PropaneTankImage;
	maxDamage = 1;
	destroyedLevel = 1;
	energyPerDamagePoint = 1;
	speedDamageScale = 1.04;
	collDamageThresholdVel = 20.0;
	collDamageMultiplier   = 0.02;
    doColorShift = true;
    colorShiftColor = "0.471 0.471 0.471 1.000";
	massCenter = "0 0 0";

	maxSteeringAngle = 0.9785;
	integration = 4;

	cameraRoll = false;
	cameraMaxDist = 13;
	cameraOffset = 7.5;
	cameraLag = 0.0;   
	cameraDecay = 0.75;
	cameraTilt = 0.4;
   	collisionTol = 0.1; 
   	contactTol = 0.1;

	useEyePoint = false;	


	numWheels = 0;

	mass = 800;
	density = 5.0;
	drag = 1.6;
	bodyFriction = 0.6;
	bodyRestitution = 0.6;
	minImpactSpeed = 10;
	softImpactSpeed = 10;
	hardImpactSpeed = 15;
	groundImpactMinSpeed    = 10.0;

	// Engine
	engineTorque = 12000;
	engineBrake = 2000; 
	brakeTorque = 50000;
	maxWheelSpeed = 0;

	rollForce		= 900;
	yawForce		= 600;
	pitchForce		= 1000;
	rotationalDrag		= 0.2;

	// Energy
	maxEnergy = 5;
	jetForce = 3000;
	minJetEnergy = 30;
	jetEnergyDrain = 2;

	splash = PropaneTankSplash;
	splashVelocity = 4.0;
	splashAngle = 67.0;
	splashFreqMod = 300.0;
	splashVelEpsilon = 0.60;
	bubbleEmitTime = 1.4;

	mediumSplashSoundVelocity = 10.0;   
	hardSplashSoundVelocity = 20.0;   
	exitSplashSoundVelocity = 5.0;

	softImpactSound = slowImpactSound;
	hardImpactSound = fastImpactSound;

	justcollided = 0;

   	uiName = "";
	rideable = true;
	lookUpLimit = 0.65;
	lookDownLimit = 0.45;

	paintable = true;
   
	damageEmitterOffset[0] = "0.0 0.0 0.0 ";
	damageLevelTolerance[0] = 0.99;

	damageEmitterOffset[1] = "0.0 0.0 0.0 ";
	damageLevelTolerance[1] = 1.0;

   	numDmgEmitterAreas = 1;

  	initialExplosionProjectile = "";
	finalExplosionProjectile = "";
   	finalExplosionOffset = 0;

   	burnTime = 100;

   	minRunOverSpeed    = 15;
   	runOverDamageScale = 5;
   	runOverPushScale   = 2;
};

datablock ExplosionData(PropaneTankFinalExplosion : vehicleFinalExplosion)
{
   //explosionShape = "";
   lifeTimeMS = 150;

   debris = "";
   debrisNum = 1;
   debrisNumVariance = 0;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 20;
   debrisVelocity = 18;
   debrisVelocityVariance = 3;

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = true;
   camShakeFreq = "7.0 8.0 7.0";
   camShakeAmp = "10.0 10.0 10.0";
   camShakeDuration = 0.75;
   camShakeRadius = 15.0;

   // Dynamic light
   lightStartRadius = 0;
   lightEndRadius = 20;
   lightStartColor = "0.45 0.3 0.1";
   lightEndColor = "0 0 0";

   //impulse
   impulseRadius = 10;
   impulseForce = 1000;
   impulseVertical = 2000;

   //radius damage
   radiusDamage        = 1000;
   damageRadius        = 10.0;

   //burn the players?
   playerBurnTime = 4000;

};

datablock ProjectileData(PropaneTankFinalExplosionProjectile : vehicleFinalExplosionProjectile)
{
   directDamage        = 0;
   radiusDamage        = 0;
   damageRadius        = 0;
   explosion           = PropaneTankFinalExplosion;

   directDamageType  = $DamageType::PropaneTankExplosion;
   radiusDamageType  = $DamageType::PropaneTankExplosion;

    brickExplosionRadius = 8;
   brickExplosionImpact = false;          //destroy a brick if we hit it directly?
   brickExplosionForce  = 30;             
   brickExplosionMaxVolume = 30;          //max volume of bricks that we can destroy
   brickExplosionMaxVolumeFloating = 60;  //max volume of bricks that we can destroy if they aren't connected to the ground (should always be >= brickExplosionMaxVolume)

   explodeOnDeath		= 1;

   armingDelay         = 0;
   lifetime            = 10;
};



datablock ItemData(GasCanItem : PropaneTankItem)
{
	shapeFile = "./models/GasCan.dts";
	uiName = "Gas Can";
	iconName = "./icons/Icon_GasCan";
	colorShiftColor = "0.4 0.071 0.071 1.000";
	image = GasCanImage;
};

datablock ShapeBaseImageData(GasCanImage : PropaneTankImage)
{
	shapeFile = "./models/GasCan.dts";
	offset = "-0.53 0.05 -0.7";
	rotation = "0 0 1 90";
	vehicle = GasCanCol;
	item = GasCanItem;
	colorShiftColor = GasCanItem.colorShiftColor;
};

datablock WheeledVehicleData(GasCanCol : PropaneTankCol)
{
	shapeFile = "./models/GasCanCol.dts";
	image = GasCanImage;
};

datablock ItemData(BileOGasItem : PropaneTankItem)
{
	shapeFile = "./models/Jug.dts";
	uiName = "Gasbile Bottle";
	iconName = "./icons/Icon_BileOGas";
	doColorShift = false;
	colorShiftColor = "0.4 0.071 0.071 1.000";
	image = BileOGasImage;
};

datablock ShapeBaseImageData(BileOGasImage : PropaneTankImage)
{
	shapeFile = "./models/Jug.dts";
	offset = "-0.25 0 0";
	rotation = "0 0 1 90";
	vehicle = BileOGasCol;
	item = BileOGasItem;
	doColorShift = false;
	colorShiftColor = BileOGasItem.colorShiftColor;
};

datablock WheeledVehicleData(BileOGasCol : PropaneTankCol)
{
	shapeFile = "./models/JugCol.dts";
	image = BileOGasImage;
	DistractionRadius = 100;
};

datablock WheeledVehicleData(OxygenTankCol : PropaneTankCol)
{
	shapeFile = "./models/OxygenTankCol.dts";
	maxDamage = 50;
	image = OxygenTankImage;
	DistractionRadius = 50;
};

datablock ItemData(OxygenTankItem : PropaneTankItem)
{
	shapeFile = "./models/OxygenTank.dts";
	uiName = "Oxygen Tank";
	iconName = "./icons/Icon_OxygenTank";
	colorShiftColor = "0.4 0.071 0.071 1.000";
	image = OxygenTankImage;
};

datablock ShapeBaseImageData(OxygenTankImage : PropaneTankItem)
{
	shapeFile = "./models/OxygenTank.dts";
	offset = "-0.53 0.05 -0.7";
	rotation = "0 0 1 90";
	vehicle = OxygenTankCol;
	item = OxygenTankItem;
	colorShiftColor = OxygenTankItem.colorShiftColor;
};

datablock ParticleData(sPipeBombTrailParticle) {
   dragCoefficient = "20";
   windCoefficient = "1";
   gravityCoefficient = "-1";
   inheritedVelFactor = "1";
   constantAcceleration = "0";
   lifetimeMS = "100";
   lifetimeVarianceMS = "25";
   spinSpeed = "0";
   spinRandomMin = "-150";
   spinRandomMax = "150";
   useInvAlpha = "0";
   animateTexture = "0";
   framesPerSec = "1";
   textureName = "base/data/particles/dot";
   animTexName[0] = "base/data/particles/dot";
   colors[0] = "1 1 0 0.9";
   colors[1] = "1 0 0 0.5";
   colors[2] = "1 1 1 0";
   colors[3] = "1.000000 1.000000 1.000000 1.000000";
   sizes[0] = "0.05";
   sizes[1] = "0.05";
   sizes[2] = "0.05";
   sizes[3] = "1";
   times[0] = "0";
   times[1] = "0.7";
   times[2] = "1";
   times[3] = "2";
};

datablock ParticleEmitterData(sPipeBombTrailEmitter) {
   className = "ParticleEmitterData";
   ejectionPeriodMS = "3";
   periodVarianceMS = "0";
   ejectionVelocity = "5";
   velocityVariance = "2";
   ejectionOffset = "0.0";
   thetaMin = "0";
   thetaMax = "180";
   phiReferenceVel = "0";
   phiVariance = "360";
   overrideAdvance = "0";
   orientParticles = "0";
   orientOnVelocity = "1";
   particles = "sPipeBombTrailParticle";
   lifetimeMS = -1;
   lifetimeVarianceMS = "0";
   useEmitterSizes = "0";
   useEmitterColors = "0";
   uiName = "Pipe Bomb Trail";
   doFalloff = "1";
   doDetail = "1";
};

datablock ParticleData(sPipeBombFlashParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = -0.5;
	inheritedVelFactor   = 1;
	constantAcceleration = 0.0;
	lifetimeMS           = 100;
	lifetimeVarianceMS   = 0;
	textureName          = "base/lighting/corona";
	spinSpeed		= 0.0;
	spinRandomMin		= 0.0;
	spinRandomMax		= 0.0;
	colors[0]     = "1 0.1 0.1 1";
	colors[1]     = "1 0.1 0.1 0";
	colors[2]     = "1 0.1 0.1 0.0";
	sizes[0]      = 0;
	sizes[1]      = 10;
	sizes[2]      = 0;
	times[0]      = 0;
	times[1]      = 1;
        times[2]      = 1;

	useInvAlpha = false;
};

datablock ParticleEmitterData(sPipeBombFlashEmitter)
{
   ejectionPeriodMS = 100;
   periodVarianceMS = 0;
   ejectionVelocity = 1.0;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "sPipeBombFlashParticle";
   
   emitterNode = GenericEmitterNode;        //used when placed on a brick
   pointEmitterNode = TenthEmitterNode; //used when placed on a 1x1 brick
   
   useEmitterColors = 1;

   uiName = "Pipe Bomb Flash";
};

datablock ExplosionData(sPipeBombExplosion)
{
   explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
   soundProfile = "pipebomb_explode_sound";

   lifeTimeMS = 200;

   particleEmitter = gravityRocketExplosionEmitter;
   particleDensity = 10;
   particleRadius = 0.2;

   emitter[0] = gravityRocketExplosionRingEmitter;
   emitter[1] = gravityRocketExplosionChunkEmitter;

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = true;
   camShakeFreq = "10.0 11.0 10.0";
   camShakeAmp = "3.0 10.0 3.0";
   camShakeDuration = 0.5;
   camShakeRadius = 20.0;

   // Dynamic light
   lightStartRadius = 5;
   lightEndRadius = 20;
   lightStartColor = "1 1 0 1";
   lightEndColor = "1 0 0 0";

   damageRadius = 10;
   radiusDamage = 600;

   impulseRadius = 15;
   impulseForce = 4000;

   playerBurnTime = 5000;
};

datablock ProjectileData(sPipeBombProjectile)
{
	isDistraction = 1;
	distractionFunction = PipeBombDistract;
	distractionDelay = 1000;
	projectileShapeName = "./models/pipebombprojectile.dts";
	directDamage        = 0;
	explosion           = sPipeBombExplosion;
	particleEmitter     = sPipeBombTrailEmitter;   
	explodeOnPlayerImpact = 0;
	explodeOnDeath = 1;
	brickExplosionRadius = 4;
	brickExplosionImpact = false;
	brickExplosionForce  = 35;             
	brickExplosionMaxVolume = 60;
	brickExplosionMaxVolumeFloating = 100;
	sound = "pipebomb_loop_sound";
	muzzleVelocity      = 30;
	velInheritFactor    = 0;
	explodeOnDeath = true;
	armingDelay         = 10000; //4 second fuse 
	lifetime            = 10000;
	fadeDelay           = 3500;
	bounceElasticity    = 0.4;
	bounceFriction      = 0.5;
	isBallistic         = true;
	gravityMod = 1.0;
	hasLight    = false;
	lightRadius = 1.0;
	lightColor  = "0 0 0";

	uiName = "";
};

datablock ExplosionData(sPipeBombLightExplosion)
{
   explosionShape = "";
   soundProfile = ""; //Phone_Tone_4_Sound;
   particleEmitter = sPipeBombFlashEmitter;
   particleDensity = 1;
   particleRadius = 0; 
   lifeTimeMS = 150;
   lightStartRadius = 0;
   lightEndRadius = 3;
   lightStartColor = "1 0 0 1";
   lightEndColor = "1 0 0 0";
};

datablock ProjectileData(sPipeBombLightProjectile)
{
   explosion           = sPipeBombLightExplosion;
   armingDelay         = 0;
   lifetime            = 10;
   explodeOnDeath		= true;
   uiName = "Pipe Bomb Flash";
};

datablock ItemData(sPipeBombItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/pipebombitem.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Pipe Bomb";
	iconName = "./icons/icon_PipeBomb";
	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	image = sPipeBombImage;
	canDrop = true;
};

datablock ShapeBaseImageData(sPipeBombImage)
{
   shapeFile = "./models/pipebombitem.dts";
   emap = true;
   mountPoint = 0;
   offset = "0 0 0";
   correctMuzzleVector = true;
   className = "WeaponImage";
   item = sPipeBombItem;
   ammo = " ";
   projectile = sPipeBombProjectile;
   projectileType = Projectile;
   melee = false;
   armReady = true;
   doColorShift = false;

	stateName[0]			= "Activate";
	stateTimeoutValue[0]		= 0.3;
	stateTransitionOnTimeout[0]	= "FlickA";
	stateSequence[0]		= "ready";
	stateSound[0]			= weaponSwitchSound;
	
	stateName[1]			= "FlickA";
	stateTimeoutValue[1]		= 0.25;
	stateTransitionOnTimeout[1]	= "FlickB";
	stateSequence[1]		= "ready";
	stateScript[1]			= "onFlick";
	stateEmitter[1]                 = sPipeBombTrailEmitter;
	stateEmitterNode[1]             = "mountPoint";
	stateEmitterTime[1]             = "2";
	stateSound[1]			= "pipebomb_start_sound";
	
	stateName[7]			= "FlickB";
	stateTimeoutValue[7]		= 0.3;
	stateTransitionOnTimeout[7]	= "Ready";
	stateSequence[7]		= "ready";
	stateEmitter[7]                 = sPipeBombTrailEmitter;
	stateEmitterNode[7]             = "mountPoint";
	stateEmitterTime[7]             = "2";
	stateSound[7]			= "pipebomb_loop_sound";

	stateName[2]			= "Ready";
	stateTransitionOnTriggerDown[2]	= "Charge";
	stateTransitionOnTimeout[2]	= "Ready";
	stateTimeoutValue[2]		= 0.15;
	stateWaitForTimeout[2]		= 0;
	stateAllowImageChange[2]	= true;
	stateEmitter[2]                 = sPipeBombTrailEmitter;
	stateEmitterNode[2]             = "mountPoint";
	stateEmitterTime[2]             = "2";
	stateSound[2]			= "pipebomb_loop_sound";
	
	stateName[3]                    = "Charge";
	stateTransitionOnTimeout[3]	= "Armed";
	stateTimeoutValue[3]            = 0.5;
	stateWaitForTimeout[3]		= false;
	stateTransitionOnTriggerUp[3]	= "AbortCharge";
	stateScript[3]                  = "onCharge";
	stateAllowImageChange[3]        = false;
	stateEmitter[3]                 = sPipeBombTrailEmitter;
	stateEmitterNode[3]             = "mountPoint";
	stateEmitterTime[3]             = "0.3";
	stateSound[3]			= "pipebomb_loop_sound";
	
	stateName[4]			= "AbortCharge";
	stateTransitionOnTimeout[4]	= "Ready";
	stateTimeoutValue[4]		= 0.3;
	stateWaitForTimeout[4]		= true;
	stateScript[4]			= "onAbortCharge";
	stateAllowImageChange[4]	= false;
	stateEmitter[4]                 = sPipeBombTrailEmitter;
	stateEmitterNode[4]             = "mountPoint";
	stateEmitterTime[4]             = "0.4";
	stateSound[4]			= "pipebomb_loop_sound";

	stateName[5]			= "Armed";
	stateWaitFotTimeout[5]		= 0;
	stateTransitionOnTimeout[5]	= "Armed";
	stateTimeoutValue[5]		= 0.1;
	stateTransitionOnTriggerUp[5]	= "Fire";
	stateAllowImageChange[5]	= false;
	stateEmitter[5]                 = sPipeBombTrailEmitter;
	stateEmitterNode[5]             = "mountPoint";
	stateEmitterTime[5]             = "0.2";
	stateSound[5]			= "pipebomb_loop_sound";

	stateName[6]			= "Fire";
	stateTransitionOnTimeout[6]	= "FlickA";
	stateTimeoutValue[6]		= 1.5;
	stateFire[6]			= true;
	stateSequence[6]		= "ready";
	stateScript[6]			= "onFire";
	stateWaitForTimeout[6]		= true;
	stateAllowImageChange[6]	= false;
	stateSound[6]			= "";
};

datablock ParticleData(Disease3Spores)
{
	dragCoefficient      = 4;
	gravityCoefficient   = 0.01;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS = 15000;
	lifetimeVarianceMS = 500;
	textureName = "base/data/particles/cloud";
	
	spinSpeed = 25.0;
	spinRandomMin = -500.0;
	spinRandomMax = 500.0;
	
	colors[0] = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
	colors[1] = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
	colors[2] = 33/255 SPC 158/255 SPC 11/255 SPC 0.3;
	sizes[0] = 5.0;
	sizes[1] = 4.0;
	sizes[2] = 3.0;
	
	useInvAlpha = true;
};

datablock ParticleEmitterData(Disease3SporeEmitter)
{
	lifeTimeMS = 50;
	
   ejectionPeriodMS = 3;
   periodVarianceMS = 0;
   ejectionVelocity = 1;
   velocityVariance = 1.0;
   ejectionOffset   = 2.0;
   thetaMin         = 89;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
	particles = Disease3Spores;
	
	emitterNode = HalfEmitterNode;
};

datablock ExplosionData(Disease3SporeExplosion)
{
	lifeTimeMS = 15000;

	soundProfile = "";
	
	emitter[0] = Disease3SporeEmitter;
	particleEmitter = Disease3SporeEmitter;
   particleDensity = 100;
   particleRadius = 1;
	
	faceViewer = true;
	explosionScale = "1 1 1";
	
	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0.0 0.0 0.0";
	lightEndColor = "0 0 0";
};

//Bile
datablock ProjectileData(BileBombProjectile)
{
		projectileShapeName				= "./models/BileBombProjectile.dts";
		directDamage					= 0;
		
		brickExplosionImpact			= false;

		
		impactImpulse					= 0;
		verticalImpulse					= 0;
		explosion						= "Disease3SporeExplosion";
		particleEmitter					= "";
		
		muzzleVelocity					= 25;
		velInheritFactor				= 0;
		
		armingDelay						= 0;
		lifetime						= 20000;
		fadeDelay						= 20000;
		bounceElasticity				= 0.4;
		bounceFriction					= 0.3;
		isBallistic						= true;
		gravityMod						= 1.0;
		
		hasLight						= false;
		lightRadius						= 3.0;
		lightColor						= "0 0 0.5";
		
		uiName							= "";
};

datablock ProjectileData(BileBombFakeProjectile)
{
	isDistraction = 1;
	distractionLifetime = 15;
	distractionFunction = BileBombDistract;
	distractionDelay = 0;
	DistractionRadius = 100;

	projectileShapeName				= "base/data/shapes/empty.dts";
	directDamage        = 0;
	explosion           = "";
	particleEmitter     = "";

	explodeOnDeath = 1;
	brickExplosionImpact = false;

	sound = "";

	muzzleVelocity      = 30;
	velInheritFactor    = 0;
	explodeOnDeath = false;

	armingDelay         = 15000; //4 second fuse 
	lifetime            = 15000;
	fadeDelay           = 13000;
	bounceElasticity    = 0.4;
	bounceFriction      = 0.5;
	isBallistic         = false;
	gravityMod = 0;

	hasLight    = false;


	uiName = "";
};

datablock ItemData(BileBombItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./models/BileBombItem.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Bile Bomb";
	iconName = "./icons/icon_BileBomb";
	doColorShift = false;

	 // Dynamic properties defined by the scripts
	image = BileBombImage;
	canDrop = true;
};

datablock ShapeBaseImageData(BileBombImage)
{
   // Basic Item properties
   shapeFile = "./models/BileBombItem.dts";
   emap = true;
   mountPoint = 0;
   offset = "-0.05 0.1 0";
   correctMuzzleVector = true;
   className = "WeaponImage";

   item = BileBombItem;
   ammo = " ";
   projectile = BileBombProjectile;
   projectileType = Projectile;

   melee = false;
   armReady = true;
   doColorShift = false;

	// States
	stateName[0]					= "Activate";
	stateSequence[0]				= "ready";
	stateSound[0]					= weaponSwitchSound;
	stateTimeoutValue[0]			= 0.1;
	stateTransitionOnTimeout[0]		= "Ready";

	stateName[1]					= "Ready";
	stateAllowImageChange[1]		= true;
	stateTransitionOnTriggerDown[1]	= "Charge";

	stateName[2]					= "Charge";
	stateTimeoutValue[2]			= 0.3;
	stateTransitionOnTimeout[2]		= "Charged";
	stateTransitionOnTriggerUp[2]	= "AbortCharge";
	stateScript[2]					= "onCharge";
	stateWaitForTimeout[2]			= true;

	stateName[3]					= "Charged";
	stateAllowImageChange[3]		= false;
	stateTransitionOnTriggerUp[3]	= "Fire";

	stateName[4]					= "Fire";
	stateAllowImageChange[4]		= false;
	stateFire[4]					= true;
	stateScript[4]					= "onFire";
	stateTimeoutValue[4]			= 0.5;
	stateTransitionOnTimeout[4]		= "Done";
	stateWaitForTimeout[4]			= true;
	
	stateName[5]					= "Done";
	stateScript[5]					= "onDone";
	
	stateName[6]					= "AbortCharge";
	stateScript[6]					= "onChargeAbort";
	stateTimeoutValue[6]			= 0;
	stateTransitionOnTimeout[6]		= "Ready";
};