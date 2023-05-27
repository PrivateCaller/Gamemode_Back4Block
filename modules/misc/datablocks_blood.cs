datablock ParticleData(largeRBloodParticle)
{
   dragCoefficient = 3;
   gravityCoefficient = 2.5;
   inheritedVelFactor = 1;
   constantAcceleration = 0;
   lifetimeMS         = 600;
   lifetimeVarianceMS = 250;
   textureName = "base/data/particles/dot";
   spinSpeed     = 0;
   spinRandomMin = -20;
   spinRandomMax = 20;
   colors[0] = "0.3 0 0 1";
   colors[1] = "0.2 0 0 0.75";
   colors[2] = "0.1 0 0 0.5";
   colors[3] = "0.05 0 0 0.25";
   sizes[0] = 0.5;
   sizes[1] = 0.4;
   sizes[2] = 0.3;
   sizes[3] = 0.2;
   times[0] = 0.25;
   times[1] = 0.5;
   times[2] = 0.75;
   times[3] = 1;
   useInvAlpha = true;
};

datablock ParticleEmitterData(largeRBloodEmitter)
{
   ejectionPeriodMS = 25;
   periodVarianceMS = 0;
   ejectionVelocity = 4;
   velocityVariance = 0;
   ejectionOffset   = 0;
   thetaMin = 0;
   thetaMax = 180;
   phiReferenceVel = 0;
   phiVariance     = 360;
   overrideAdvance = false;
   lifetimeMS = 4500;
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
   colors[0] = "0.4 0 0 1";
   colors[1] = "0.3 0 0 0.3 ";
   colors[2] = "0.2 0 0 0";
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
	
	colors[0]	= "0.5 0.0 0.0 0.0";
	colors[1]	= "0.4 0.0 0.0 0.5";
	colors[2]	= "0.3 0.0 0.0 0.3";

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
	
	textureName		= "base/data/particles/dot";
	
	colors[0]	= "0.5 0.0 0.0 0.0";
	colors[1]	= "0.4 0.0 0.0 0.5";
	colors[2]	= "0.3 0.0 0.0 0.3";

	sizes[0]	= 0.5;
	sizes[1]	= 0.25;
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
	
	textureName		= "base/data/particles/dot";
	
	colors[0]	= "0.5 0.0 0.0 0.0";
	colors[1]	= "0.4 0.0 0.0 0.5";
	colors[2]	= "0.3 0.0 0.0 0.3";

	sizes[0]	= 0.25;
	sizes[1]	= 1.0;
	sizes[2]	= 0.25;

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
	stateEmitter[0]      = "largeRBloodEmitter";
	stateEmitterTime[0]  = 4;
	stateTimeoutValue[0] = 4;
	stateTransitionOnTimeout[0] = "Done";
	stateName[1] = "Done";
	stateScript[1]			= "onDone";	
};

datablock DebrisData(rBloodFootDebris)
{
   	emitters = "bloodBurstSmallEmitter";
	shapeFile = "./models/gibs/shoe.dts";
	lifetime = 5.0;
	spinSpeed = 500.0;
	minSpinSpeed = -500.0;
	maxSpinSpeed = 500.0;
	elasticity = 0.2;
	friction = 0.8;
	numBounces = 2;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = false;
	gravModifier = 2.5;
};
datablock DebrisData(bloodChunkDebris : rBloodFootDebris)
{
	shapeFile = "./models/gibs/chunk.dts";
};
datablock DebrisData(rBloodHeadDebris : rBloodFootDebris)
{
   	emitters = "bloodBurstSmallEmitter";
	shapeFile = "./models/gibs/head.dts";
};
datablock DebrisData(rBloodHandDebris : rBloodFootDebris)
{   
	shapeFile = "./models/gibs/hand.dts";
};
datablock DebrisData(rBloodStomachDebris : rBloodFootDebris)
{   
	shapeFile = "./models/gibs/stomach.dts";
};
datablock DebrisData(rBloodLungRDebris : rBloodFootDebris)
{   
	shapeFile = "./models/gibs/lungsr.dts";
};
datablock DebrisData(rBloodLungLDebris : rBloodFootDebris)
{   
	shapeFile = "./models/gibs/lungsl.dts";
};
datablock DebrisData(rBloodLiverDebris : rBloodFootDebris)
{   
	shapeFile = "./models/gibs/liver.dts";
};
datablock DebrisData(rBloodIntestinesDebris : rBloodFootDebris)
{   
	shapeFile = "./models/gibs/intestines.dts";
};
datablock DebrisData(rBloodHeartDebris : rBloodFootDebris)
{   
	shapeFile = "./models/gibs/heart.dts";
};
datablock DebrisData(rBloodEsophagusDebris : rBloodFootDebris)
{   
	shapeFile = "./models/gibs/esophagus.dts";
};
datablock DebrisData(rBloodRibsDebris : rBloodFootDebris)
{   
	shapeFile = "./models/gibs/ribs.dts";
};
datablock DebrisData(rBloodBrain1Debris : rBloodFootDebris)
{   
	shapeFile = "./models/gibs/brain1.dts";
};
datablock DebrisData(rBloodBrain2Debris : rBloodFootDebris)
{   
	shapeFile = "./models/gibs/brain2.dts";
};
datablock DebrisData(rBloodBrain3Debris : rBloodFootDebris)
{   
	shapeFile = "./models/gibs/brain3.dts";
};
datablock DebrisData(rBloodBrain4Debris : rBloodFootDebris)
{   
	shapeFile = "./models/gibs/brain4.dts";
};

datablock ExplosionData(rBloodStomachDebrisExplosion)
{
   debris = rBloodStomachDebris;
   debrisNum = 1;
   debrisNumVariance = 0;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 10;
   debrisVelocityVariance = 8;
};
datablock ExplosionData(rBloodLungRDebrisExplosion : rBloodStomachDebrisExplosion)
{
   debris = rBloodLungRDebris;
};
datablock ExplosionData(rBloodLungLDebrisExplosion : rBloodStomachDebrisExplosion)
{
   debris = rBloodLungLDebris;
   subExplosion[0] = rBloodLungRDebrisExplosion;
};
datablock ExplosionData(rBloodIntestinesDebrisExplosion : rBloodStomachDebrisExplosion)
{
	debrisNum = 2;
	debris = rBloodIntestinesDebris;
};
datablock ExplosionData(rBloodHeartDebrisExplosion : rBloodStomachDebrisExplosion)
{
   debris = rBloodHeartDebris;
};
datablock ExplosionData(rBloodEsophagusDebrisExplosion : rBloodStomachDebrisExplosion)
{
   debris = rBloodEsophagusDebris;
};
datablock ExplosionData(rBloodLiverDebrisExplosion : rBloodStomachDebrisExplosion)
{
   	debris = rBloodLiverDebris;
	subExplosion[0] = rBloodIntestinesDebrisExplosion;
   	subExplosion[1] = rBloodHeartDebrisExplosion;
   	subExplosion[2] = rBloodEsophagusDebrisExplosion;
};

datablock ExplosionData(rBloodBrain1DebrisExplosion : rBloodStomachDebrisExplosion)
{
   debris = rBloodBrain1Debris;
};
datablock ExplosionData(rBloodBrain2DebrisExplosion : rBloodStomachDebrisExplosion)
{
   debris = rBloodBrain2Debris;
};
datablock ExplosionData(rBloodBrain3DebrisExplosion : rBloodStomachDebrisExplosion)
{
   debris = rBloodBrain3Debris;
};
datablock ExplosionData(rBloodBrain4DebrisExplosion : rBloodStomachDebrisExplosion)
{
   debris = rBloodBrain4Debris;
};
datablock ExplosionData(rBloodRibsDebrisExplosion : rBloodStomachDebrisExplosion)
{
   debris = rBloodRibsDebris;
};

datablock ExplosionData(RBloodOrganExplosion)
{
	soundProfile	= "";
	explosionShape = "";
   	lifeTimeMS = 150;
   	debris = "bloodChunkDebris";
   	debrisNum = 4;
   	debrisNumVariance = 2;
   	debrisPhiMin = 0;
   	debrisPhiMax = 360;
   	debrisThetaMin = 0;
   	debrisThetaMax = 180;
   	debrisVelocity = 10;
   	debrisVelocityVariance = 8;
   	particleEmitter = bloodBurstSmallEmitter;
   	particleDensity = 10;
   	particleRadius = 0.2;
   	emitter[0] = bloodBurstSprinkleSmallEmitter;
   	subExplosion[0] = rBloodStomachDebrisExplosion;
   	subExplosion[1] = rBloodLungRDebrisExplosion;
   	subExplosion[2] = rBloodLiverDebrisExplosion;
	subExplosion[3] = rBloodRibsDebrisExplosion;

   	faceViewer     = true;
   	explosionScale = "1 1 1";
   	shakeCamera = false;
};

datablock ExplosionData(RBloodBrainExplosion)
{
	soundProfile		= "";
	explosionShape = "";
   	lifeTimeMS = 150;
   	debris = "bloodChunkDebris";
   	debrisNum = 4;
   	debrisNumVariance = 3;
   	debrisPhiMin = 0;
   	debrisPhiMax = 360;
   	debrisThetaMin = 0;
   	debrisThetaMax = 180;
   	debrisVelocity = 10;
   	debrisVelocityVariance = 8;
   	particleEmitter = bloodBurstSmallEmitter;
   	particleDensity = 10;
   	particleRadius = 0.2;
   	emitter[0] = bloodBurstSprinkleSmallEmitter;
   	subExplosion[0] = rBloodBrain1DebrisExplosion;
   	subExplosion[1] = rBloodBrain2DebrisExplosion;
   	subExplosion[2] = rBloodBrain3DebrisExplosion;
   	subExplosion[3] = rBloodBrain4DebrisExplosion;

   	faceViewer     = true;
   	explosionScale = "1 1 1";
   	shakeCamera = false;
};

datablock ProjectileData(RBloodBrainProjectile)
{
	uiname							= "";
	lifetime						= 10;
	fadeDelay						= 10;
	explodeondeath						= true;
	explosion						= RBloodBrainExplosion;
};

datablock ProjectileData(RBloodOrganProjectile)
{
	uiname							= "";
	lifetime						= 10;
	fadeDelay						= 10;
	explodeondeath						= true;
	explosion						= RBloodOrganExplosion;
};

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
   debrisVelocity = 10;
   debrisVelocityVariance = 8;

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
   debrisVelocityVariance = 8;
};
datablock ExplosionData(bloodFootDebrisExplosion : bloodBurstFinalExplosion)
{   
   emitter[0] = "";
   emitter[1] = "";

   particleEmitter = "";
   particleDensity = 20;
   particleRadius = 1.0;

   debris = "rBloodFootDebris";
   debrisNum = 1;
   debrisNumVariance = 0;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 10;
   debrisVelocityVariance = 8;
};
datablock ExplosionData(bloodHeadDebrisExplosion : bloodFootDebrisExplosion)
{   
   debris = "rBloodHeadDebris";
};
datablock ExplosionData(bloodHandDebrisExplosion : bloodFootDebrisExplosion)
{   
   debris = "rBloodHandDebris";
};

datablock ProjectileData(bloodFootDebrisProjectile)
{
	uiname			= "";
	lifetime		= 10;
	fadeDelay		= 10;
	explodeondeath	= true;
	explosion		= "bloodFootDebrisExplosion";
};
datablock ProjectileData(bloodHeadDebrisProjectile : bloodFootDebrisProjectile)
{
	explosion	= "bloodHeadDebrisExplosion";
};
datablock ProjectileData(bloodHandDebrisProjectile : bloodFootDebrisProjectile)
{
	explosion	= "bloodHandDebrisExplosion";
};
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

datablock staticShapeData(BloodDecal1) 
{
	shapeFile = "./models/gibs/blood1trans.dts";
};

datablock staticShapeData(BloodDecal2 : BloodDecal1) 
{
	shapeFile = "./models/gibs/blood2trans.dts";
};

datablock ParticleData(bloodParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.2;
	gravityCoefficient	= 0.5;
	inheritedVelFactor	= 1;
	constantAcceleration	= 0.0;
	lifetimeMS		= 500;
	lifetimeVarianceMS	= 10;
	spinSpeed		= 40.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;

	textureName		= "./models/blood2.png";
	colors[0]	= "0.4 0 0 1";
	colors[1]	= "0.2 0 0 0";
	sizes[0]	= 0.4;
	sizes[1]	= 2;
	times[0]	= 0.5;
	times[1]	= 1;
};
datablock ParticleData(bloodParticle2 : bloodParticle)
{
	textureName		= "./models/blood3.png";
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
datablock ParticleEmitterData(bloodEmitter2 : bloodEmitter)
{
	particles = bloodParticle2;
};
datablock ExplosionData(bloodExplosion)
{
	lifeTimeMS = 300;
	particleEmitter = bloodEmitter;
	particleDensity = 5;
	particleRadius = 0.2;
	faceViewer     = true;
	explosionScale = "1 1 1";
};
datablock ExplosionData(bloodExplosion2 : bloodExplosion)
{
	particleEmitter = bloodEmitter2;
	subExplosion[0] = bloodExplosion;
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

	textureName		= "base/data/particles/dot.png";

	colors[0]	= "0.4 0 0 1";
	colors[1]	= "0.2 0 0 0.8";
	sizes[0]	= 0.1;
	sizes[1]	= 0;
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
datablock ProjectileData(bloodExplosionProjectile1)
{
	directDamage        = 0;
	impactImpulse	     = 0;
	verticalImpulse	  = 0;
	explosion           = bloodExplosion;
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