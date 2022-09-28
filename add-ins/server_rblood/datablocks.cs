%pattern = "add-ons/Gamemode_Left4Block/add-ins/server_rblood/sounds/*.wav";
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strreplace(filename(strlwr(%file)), ".wav", "");
   	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosest3d; filename = \"" @ %file @ "\"; };");
	%file = findNextFile(%pattern);
}

datablock ParticleData(largeRBloodParticle)
{
   dragCoefficient = 3;
   gravityCoefficient = 0.0;
   inheritedVelFactor = 1;
   constantAcceleration = 0;
   lifetimeMS         = 600;
   lifetimeVarianceMS = 250;
   textureName = "base/data/particles/cloud";
   spinSpeed     = 0;
   spinRandomMin = -20;
   spinRandomMax = 20;
   colors[0] = "0.6 0 0 1";
   colors[1] = "0.5 0 0 1";
   colors[2] = "0.4 0 0 0";
   sizes[0] = 1.5;
   sizes[1] = 0.3;
   sizes[2] = 0.04;
   times[1] = 0.5;
   times[2] = 1;
   useInvAlpha = true;
};

datablock ParticleEmitterData(largeRBloodEmitter)
{
   ejectionPeriodMS = 50;
   periodVarianceMS = 0;
   ejectionVelocity = 4;
   velocityVariance = 0;
   ejectionOffset   = 0;
   thetaMin = 0;
   thetaMax = 180;
   phiReferenceVel = 0;
   phiVariance     = 360;
   overrideAdvance = false;
   lifetimeMS = 3500;
   particles = "largeRBloodParticle";

   uiName = "";
};

datablock ParticleData(smallBloodParticle)
{
   dragCoefficient = 3;
   gravityCoefficient = 0.5;
   inheritedVelFactor = 0.3;
   constantAcceleration = 0;
   lifetimeMS         = 100;
   lifetimeVarianceMS = 75;
   textureName = "base/data/particles/dot";
   spinSpeed     = 0;
   spinRandomMin = -20;
   spinRandomMax = 20;
   colors[0] = "0.6 0 0 1";
   colors[1] = "0.5 0 0 0.3 ";
   colors[2] = "0.4 0 0 0";
   sizes[0] = 0.06;
   sizes[1] = 0.2;
   sizes[2] = 0.04;
   times[1] = 0.5;
   times[2] = 1;
   useInvAlpha = true;
};

datablock ParticleEmitterData(smallBloodEmitter)
{
   ejectionPeriodMS = 50;
   periodVarianceMS = 0;
   ejectionVelocity = 1;
   velocityVariance = 0;
   ejectionOffset   = 0;
   thetaMin = 0;
   thetaMax = 180;
   phiReferenceVel = 0;
   phiVariance     = 360;
   overrideAdvance = false;
   lifetimeMS = 3500;
   particles = "smallBloodParticle";

   uiName = "";
};

datablock ParticleData(bloodBurstSprinkleSmallParticle)
{
	dragCoefficient			= 1.0;
	windCoefficient			= 0.0;
	gravityCoefficient		= 3.0;
	inheritedVelFactor		= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS				= 600;
	lifetimeVarianceMS		= 200;
	spinSpeed				= 10.0;
	spinRandomMin			= -50.0;
	spinRandomMax			= 50.0;
	useInvAlpha				= true;
	animateTexture			= false;
	
	textureName		= "base/data/particles/dot";
	
	colors[0]	= "0.9 0.0 0.0 0.0";
	colors[1]	= "0.8 0.0 0.0 0.5";
	colors[2]	= "0.7 0.0 0.0 0.3";

	sizes[0]	= 0.0;
	sizes[1]	= 0.1;
	sizes[2]	= 0.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 1.0;
};
datablock ParticleEmitterData(bloodBurstSprinkleSmallEmitter)
{
   ejectionPeriodMS = 10;
   periodVarianceMS = 0;
   lifetimeMS       = 150;
   ejectionVelocity = 10;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "bloodBurstSprinkleSmallParticle";
};

// small blood explosion
datablock ParticleData(bloodBurstSmallParticle)
{
	dragCoefficient			= 1.0;
	windCoefficient			= 0.0;
	gravityCoefficient		= 1.0;
	inheritedVelFactor		= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS				= 500;
	lifetimeVarianceMS		= 200;
	spinSpeed				= 10.0;
	spinRandomMin			= -50.0;
	spinRandomMax			= 50.0;
	useInvAlpha				= true;
	animateTexture			= false;
	
	textureName		= "base/data/particles/cloud";
	
	colors[0]	= "0.5 0.0 0.0 0.0";
	colors[1]	= "0.5 0.0 0.0 0.5";
	colors[2]	= "0.5 0.0 0.0 0.3";

	sizes[0]	= 0.2;
	sizes[1]	= 1.0;
	sizes[2]	= 0.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 1.0;
};
datablock ParticleEmitterData(bloodBurstSmallEmitter)
{
   ejectionPeriodMS = 10;
   periodVarianceMS = 0;
   lifetimeMS       = 150;
   ejectionVelocity = 2;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 85;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "bloodBurstSmallParticle";
};

// large blood explosion (dismembering)
datablock ParticleData(bloodBurstLargeParticle)
{
	dragCoefficient			= 1.0;
	windCoefficient			= 0.0;
	gravityCoefficient		= 1.0;
	inheritedVelFactor		= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS				= 400;
	lifetimeVarianceMS		= 200;
	spinSpeed				= 10.0;
	spinRandomMin			= -50.0;
	spinRandomMax			= 50.0;
	useInvAlpha				= true;
	animateTexture			= false;
	
	textureName		= "base/data/particles/cloud";
	
	colors[0]	= "0.5 0.0 0.0 0.0";
	colors[1]	= "0.5 0.0 0.0 0.5";
	colors[2]	= "0.5 0.0 0.0 0.3";

	sizes[0]	= 0.2;
	sizes[1]	= 2.0;
	sizes[2]	= 0.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 1.0;
};
datablock ParticleEmitterData(bloodBurstLargeEmitter)
{
   ejectionPeriodMS = 10;
   periodVarianceMS = 0;
   lifetimeMS       = 150;
   ejectionVelocity = 2;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 85;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "bloodBurstLargeParticle";
};


// Bloodbot Image
datablock ShapeBaseImageData(RBloodLargeImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0 0 0";
	rotation = eulerToMatrix("0 0 0");
	scale = "1 1 1";
	doColorShift = true;
	colorShiftColor = "0.500 0.000 0.000 1.000";
	
	stateName[0] = "Ready";
	stateName[1] = "FireA";
	stateName[2] = "Done";
   stateScript[2] = "onDone";
	
	stateTimeoutValue[0] = 0.01;
	stateTimeoutValue[1] = 30;
	stateTimeoutValue[2] = 0;
	
	stateTransitionOnTimeout[0] = "FireA";
	stateTransitionOnTimeout[1]	= "Done";
	
	stateEmitter[1]      = largeRBloodEmitter;
	stateEmitterTime[1]  = 30;
};

function RBloodLargeImage::onDone(%this,%obj) { %obj.delete(); }

datablock DebrisData(rBloodFootDebris)
{
   emitters = smallBloodEmitter;
   
	shapeFile = "./models/shoe.dts";
	lifetime = 5.0;
	minSpinSpeed = -500.0;
	maxSpinSpeed = 500.0;
	elasticity = 0.2;
	friction = 0.8;
	numBounces = 2;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 1;
};
datablock DebrisData(rBloodHeadDebris)
{
   emitters = largeRBloodEmitter;
   
	shapeFile = "./models/head.dts";
	lifetime = 5.0;
	minSpinSpeed = -500.0;
	maxSpinSpeed = 500.0;
	elasticity = 0.2;
	friction = 0.8;
	numBounces = 2;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 1;
};
datablock DebrisData(rBloodHandDebris)
{
   emitters = smallBloodEmitter;
   
	shapeFile = "./models/hand.dts";
	lifetime = 5.0;
	minSpinSpeed = -500.0;
	maxSpinSpeed = 500.0;
	elasticity = 0.2;
	friction = 0.8;
	numBounces = 2;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 1;
};
datablock DebrisData(bloodChunkDebris)
{
   emitters = smallBloodEmitter;

	shapeFile = "./models/chunk.dts";
	lifetime = 5.0;
	minSpinSpeed = -2000.0;
	maxSpinSpeed = 2000.0;
	elasticity = 0.2;
	friction = 0.8;
	numBounces = 2;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 2;
};

// Explosions
datablock ExplosionData(bloodBurstFinalExplosion)
{
   //explosionShape = "";
   lifeTimeMS = 1000;

   soundProfile = "";
   
   emitter[0] = bloodBurstSprinkleSmallEmitter;
   emitter[1] = "";

   particleEmitter = bloodBurstSmallEmitter;
   particleDensity = 5;
   particleRadius = 0.2;

   debris = "";
   debrisNum = 5;
   debrisNumVariance = 0;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 5;
   debrisVelocityVariance = 3;

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = false;
   camShakeFreq = "7.0 8.0 7.0";
   camShakeAmp = "10.0 10.0 10.0";
   camShakeDuration = 0.75;
   camShakeRadius = 15.0;

   // Dynamic light
   lightStartRadius = 0;
   lightEndRadius = 0;
   lightStartColor = "0.45 0.3 0.1";
   lightEndColor = "0 0 0";

   //impulse
   impulseRadius = 0;
   impulseForce = 0;
   impulseVertical = 0;

   //radius damage
   radiusDamage        = 0;
   damageRadius        = 0;

   //burn the players?
   playerBurnTime = 0;
};
datablock ExplosionData(bloodDismemberExplosion : bloodBurstFinalExplosion)
{   
   emitter[0] = bloodBurstSprinkleSmallEmitter;
   emitter[1] = "";

   particleEmitter = bloodBurstLargeEmitter;
   particleDensity = 20;
   particleRadius = 1.0;

   debris = bloodChunkDebris;
   debrisNum = 3;
   debrisNumVariance = 1;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 10;
   debrisVelocityVariance = 3;
};
datablock ExplosionData(bloodFootDebrisExplosion : bloodBurstFinalExplosion)
{   
   emitter[0] = "";
   emitter[1] = "";

   particleEmitter = "";
   particleDensity = 20;
   particleRadius = 1.0;

   debris = rBloodFootDebris;
   debrisNum = 2;
   debrisNumVariance = 0;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 10;
   debrisVelocityVariance = 5;
};
datablock ExplosionData(bloodHeadDebrisExplosion : bloodBurstFinalExplosion)
{   
   emitter[0] = "";
   emitter[1] = "";

   particleEmitter = "";
   particleDensity = 20;
   particleRadius = 1.0;

   debris = rBloodHeadDebris;
   debrisNum = 1;
   debrisNumVariance = 0;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 10;
   debrisVelocityVariance = 5;
};
datablock ExplosionData(bloodHandDebrisExplosion : bloodBurstFinalExplosion)
{   
   emitter[0] = "";
   emitter[1] = "";

   particleEmitter = "";
   particleDensity = 20;
   particleRadius = 1.0;

   debris = rBloodHandDebris;
   debrisNum = 2;
   debrisNumVariance = 0;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 10;
   debrisVelocityVariance = 5;
};

// Projectiles
datablock ProjectileData(bloodBurstFinalExplosionProjectile)
{
   directDamage        = 0;
   radiusDamage        = 0;
   damageRadius        = 0;
   explosion           = bloodBurstFinalExplosion;

   directDamageType  = $DamageType::jeepExplosion;
   radiusDamageType  = $DamageType::jeepExplosion;

   explodeOnDeath		= 1;

   armingDelay         = 0;
   lifetime            = 10;
};
datablock ProjectileData(bloodDismemberProjectile : bloodBurstFinalExplosionProjectile)
{
   explosion           = bloodDismemberExplosion;
};
datablock ProjectileData(footGibProjectile : bloodBurstFinalExplosionProjectile)
{
   explosion           = bloodFootDebrisExplosion;
};
datablock ProjectileData(headGibProjectile : bloodBurstFinalExplosionProjectile)
{
   explosion           = bloodHeadDebrisExplosion;
};
datablock ProjectileData(handGibProjectile : bloodBurstFinalExplosionProjectile)
{
   explosion           = bloodHandDebrisExplosion;
};

$RBloodGib0 = footGibProjectile;
$RBloodGib1 = headGibProjectile;
$RBloodGib2 = handGibProjectile;


datablock staticShapeData(BloodDecal1) {
	shapeFile = "./models/decals/blood1.dts";

	doColorShift = true;
	colorShiftColor = "0.7 0 0 1";
};

datablock staticShapeData(BloodDecal2) {
	shapeFile = "./models/decals/blood2.dts";

	doColorShift = true;
	colorShiftColor = "0.7 0 0 1";
};

datablock ParticleData(bloodParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.2;
	gravityCoefficient	= 0.2;
	inheritedVelFactor	= 1;
	constantAcceleration	= 0.0;
	lifetimeMS		= 500;
	lifetimeVarianceMS	= 10;
	spinSpeed		= 40.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "./models/decals/blood2.png";
	//animTexName		= " ";

	// Interpolation variables
	colors[0]	= "0.7 0 0 1";
	colors[1]	= "0.7 0 0 0";
	sizes[0]	= 0.4;
	sizes[1]	= 2;
	//times[0]	= 0.5;
	//times[1]	= 0.5;
};

datablock ParticleEmitterData(bloodEmitter)
{
	ejectionPeriodMS = 3;
	periodVarianceMS = 0;

	ejectionVelocity = 0; //0.25;
	velocityVariance = 0; //0.10;

	ejectionOffset = 0;

	thetaMin         = 0.0;
	thetaMax         = 90.0;  

	particles = bloodParticle;

	useEmitterColors = true;
	uiName = "";
};

datablock ExplosionData(bloodExplosion)
{
	//explosionShape = "";
	//soundProfile = bulletHitSound;
	lifeTimeMS = 300;

	particleEmitter = bloodEmitter;
	particleDensity = 5;
	particleRadius = 0.2;
	//emitter[0] = bloodEmitter;

	faceViewer     = true;
	explosionScale = "1 1 1";
};

datablock ProjectileData(bloodExplosionProjectile1)
{
	directDamage        = 0;
	impactImpulse	     = 0;
	verticalImpulse	  = 0;
	explosion           = bloodExplosion;
	particleEmitter     = bloodEmitter;

	muzzleVelocity      = 50;
	velInheritFactor    = 1;

	armingDelay         = 0;
	lifetime            = 2000;
	fadeDelay           = 1000;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.20;
	isBallistic         = true;
	gravityMod = 0.1;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
};



datablock ParticleData(bloodParticle2)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.1;
	gravityCoefficient	= 0.3;
	inheritedVelFactor	= 1;
	constantAcceleration	= 0.0;
	lifetimeMS		= 300;
	lifetimeVarianceMS	= 10;
	spinSpeed		= 20.0;
	spinRandomMin		= -10.0;
	spinRandomMax		= 10.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "./models/decals/blood3.png";
	//animTexName		= " ";

	// Interpolation variables
	colors[0]	= "0.7 0 0 1";
	colors[1]	= "0.7 0 0 0";
	sizes[0]	= 1;
	sizes[1]	= 0;
	//times[0]	= 0.5;
	//times[1]	= 0.5;
};

datablock ParticleEmitterData(bloodEmitter2)
{
	ejectionPeriodMS = 5;
	periodVarianceMS = 0;

	ejectionVelocity = 0; //0.25;
	velocityVariance = 0; //0.10;

	ejectionOffset = 0;

	thetaMin         = 0.0;
	thetaMax         = 90.0;  

	particles = bloodParticle2;

	useEmitterColors = true;
	uiName = "";
};

datablock ExplosionData(bloodExplosion2)
{
	//explosionShape = "";
	//soundProfile = bulletHitSound;
	lifeTimeMS = 300;

	particleEmitter = bloodEmitter2;
	particleDensity = 5;
	particleRadius = 0.2;
	//emitter[0] = bloodEmitter;

	faceViewer     = true;
	explosionScale = "1 1 1";
};

datablock ProjectileData(bloodExplosionProjectile2)
{
	directDamage        = 0;
	impactImpulse	     = 0;
	verticalImpulse	  = 0;
	explosion           = bloodExplosion2;
	particleEmitter     = bloodEmitter2;

	muzzleVelocity      = 50;
	velInheritFactor    = 1;

	armingDelay         = 0;
	lifetime            = 2000;
	fadeDelay           = 1000;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.20;
	isBallistic         = true;
	gravityMod = 0.1;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
};

datablock ParticleData(bloodDripParticle)
{
	dragCoefficient		= 1.0;
	windCoefficient		= 0.1;
	gravityCoefficient	= 0.5;
	inheritedVelFactor	= 1;
	constantAcceleration	= 0.0;
	lifetimeMS		= 200;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 20.0;
	spinRandomMin		= -10.0;
	spinRandomMax		= 10.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/dot.png";
	//animTexName		= " ";

	// Interpolation variables
	colors[0]	= "0.7 0 0 1";
	colors[1]	= "0.7 0 0 0.8";
	sizes[0]	= 0.1;
	sizes[1]	= 0;
	//times[0]	= 0.5;
	//times[1]	= 0.5;
};

datablock ParticleEmitterData(bloodDripEmitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;

	ejectionVelocity = 0; //0.25;
	velocityVariance = 0; //0.10;

	ejectionOffset = 0;

	thetaMin         = 0.0;
	thetaMax         = 90.0;  

	particles = bloodDripParticle;

	useEmitterColors = true;
	uiName = "";
};

datablock ProjectileData(bloodDripProjectile)
{
	directDamage        = 0;
	impactImpulse	     = 0;
	verticalImpulse	  = 0;
	explosion           = bloodExplosion2;
	particleEmitter     = bloodDripEmitter;

	muzzleVelocity      = 60;
	velInheritFactor    = 1;

	armingDelay         = 3000;
	lifetime            = 3000;
	fadeDelay           = 2000;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.20;
	isBallistic         = true;
	gravityMod = 1;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
};