datablock ParticleData(mud_TrailParticle)
{
	dragCoefficient		 = 3.0;
	windCoefficient		 = 0.0;
	gravityCoefficient	 = 0.0;
	inheritedVelFactor	 = 0.0;
	constantAcceleration = 0.0;
	lifetimeMS			 = 500;
	lifetimeVarianceMS	 = 0;
	spinSpeed			 = 0;
	spinRandomMin		 = 0;
	spinRandomMax		 = 0;
	
	useInvAlpha = true;
	animateTexture = false;
	textureName	= "base/data/particles/dot";

	colors[0]	= "0.5 0.375 0 0.3";
	colors[1]	= "0.5 0.375 0 0.2";
	colors[2]	= "0 0 0 0";
	sizes[0]	= 0.3;
	sizes[1]	= 0.0;
	sizes[2]	= 0.0;
	times[0]	= 0.0;
	times[1]	= 0.4;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(mud_TrailEmitter)
{
	ejectionPeriodMS = 5;
	periodVarianceMS = 0;
	ejectionVelocity = 0;
	velocityVariance = 0;
	ejectionOffset	 = 0;
	thetaMin		 = 0.0;
	thetaMax		 = 0.0;
	
	particles = mud_TrailParticle;
	useEmitterColors = false;
	uiName = "";
};

datablock ParticleData(mud_ExplosionParticle)
{
	dragCoefficient		 = 5;
	windCoefficient		 = 0;
	gravityCoefficient	 = 0.5;
	inheritedVelFactor	 = 0;
	constantAcceleration = 0;
	lifetimeMS			 = 300;
	lifetimeVarianceMS	 = 100;
	spinSpeed			 = 0;
	spinRandomMin		 = 0;
	spinRandomMax		 = 0;
	
	useInvAlpha = true;
	animateTexture = false;
	textureName		= "base/data/particles/cloud";

	colors[0]	= "0.5 0.375 0 1";
	colors[1]	= "0.5 0.375 0 0";
	sizes[0]	= 1;
	sizes[1]	= 2;
	times[0]	= 0;
	times[1]	= 1;
};

datablock ParticleEmitterData(mud_ExplosionEmitter)
{
	ejectionPeriodMS = 7;
	periodVarianceMS = 0;
	lifeTimeMS	   	 = 50;
	ejectionVelocity = 6;
	velocityVariance = 1.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 90;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	
	particles = "mud_ExplosionParticle";
	useEmitterColors = true;
	uiName = "mud Puff";
	emitterNode = TenthEmitterNode;
};

datablock ParticleData(mud_ExplosionParticle2)
{
	dragCoefficient		 = 2;
	windCoefficient		 = 0;
	gravityCoefficient	 = 2;
	inheritedVelFactor	 = 0;
	constantAcceleration = 0;
	lifetimeMS			 = 500;
	lifetimeVarianceMS	 = 100;
	spinSpeed			 = 10.0;
	spinRandomMin		 = -50.0;
	spinRandomMax		 = 50.0;
	
	useInvAlpha	= true;
	animateTexture = false;
	textureName = "base/data/particles/chunk";

	colors[0]	= "0.5 0.375 0 1";
	colors[1]	= "0.5 0.375 0 0";
	sizes[0]	= 0.3;
	sizes[1]	= 0;
	times[0]	= 0;
	times[1]	= 1;
};

datablock ParticleEmitterData(mud_ExplosionEmitter2)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	lifetimeMS       = 7;
	ejectionVelocity = 15;
	velocityVariance = 5;
	ejectionOffset   = 0;
	thetaMin         = 0;
	thetaMax         = 90;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "mud_ExplosionParticle2";
	useEmitterColors = true;
	uiName = "mud Chunk";
	emitterNode = HalfEmitterNode;
};

datablock ExplosionData(mud_Explosion)
{
	lifeTimeMS = 150;

	soundProfile = "mud_splat_sound";

	emitter[0] = mud_ExplosionEmitter;
	emitter[1] = mud_ExplosionEmitter2;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "3 3 6";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 0.5;
	camShakeRadius = 2;

	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0 0 0";
	lightEndColor = "0 0 0";

	impulseRadius = 4;
	impulseForce = 400;

	radiusDamage = 2;
	damageRadius = 3;
};

AddDamageType("mudDirect",   '<bitmap:add-ons/Package_Left4Block/icons/CI_snowball> %1',       '%2 <bitmap:add-ons/Package_Left4Block/icons/CI_snowball> %1',1,1);
AddDamageType("mudRadius",   '<bitmap:add-ons/Package_Left4Block/icons/CI_snowballRadius> %1', '%2 <bitmap:add-ons/Package_Left4Block/icons/CI_snowballRadius> %1',1,0);

datablock ProjectileData(mud_Projectile)
{
	projectileShapeName = "./models/mudball.dts";
	directDamage = 1;
	directDamageType = $DamageType::mudDirect;
	radiusDamageType = $DamageType::mudRadius;
	impactImpulse = 130;
	verticalImpulse = 130;
	explosion = mud_Explosion;
	particleEmitter = mud_TrailEmitter;

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce = 0;
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;
   
	muzzleVelocity = 50;
	velInheritFactor = 1;

	armingDelay = 0;
	lifetime = 8000;
	fadeDelay = 19500;
	bounceElasticity = 0;
	bounceFriction = 0;
	isBallistic = true;
	gravityMod = 1.0;
	
	hasLight = false;
	lightRadius = 1.0;
	lightColor = "0 0 0";
	
	uiName = "";
};



datablock ShapeBaseImageData(mud_Image)
{
	shapeFile  = "./models/mudball.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";

	correctMuzzleVector = true;

	className = "WeaponImage";
	
	item = "";
	ammo = " ";
	projectile = mud_Projectile;
	projectileType = Projectile;

	melee = false;
	armReady = true;

	stateName[0]				= "Activate";
	stateTimeoutValue[0]		= 0.1;
	stateTransitionOnTimeout[0]	= "Ready";
	stateSequence[0]			= "ready";
	stateSound[0]				= weaponSwitchSound;

	stateName[1]					= "Ready";
	stateTransitionOnTriggerDown[1]	= "Charge";
	stateAllowImageChange[1]		= true;
	
	stateName[2]                  = "Charge";
	stateTransitionOnTimeout[2]	  = "Armed";
	stateTimeoutValue[2]          = 0.6;
	stateWaitForTimeout[2]		  = false;
	stateTransitionOnTriggerUp[2] = "AbortCharge";
	stateScript[2]                = "onCharge";
	stateAllowImageChange[2]      = false;
	
	stateName[3]				= "AbortCharge";
	stateTransitionOnTimeout[3]	= "Ready";
	stateTimeoutValue[3]		= 0.2;
	stateWaitForTimeout[3]		= true;
	stateScript[3]				= "onAbortCharge";
	stateAllowImageChange[3]	= false;

	stateName[4]				  = "Armed";
	stateTransitionOnTriggerUp[4] = "Fire";
	stateAllowImageChange[4]	  = false;

	stateName[5]				= "Fire";
	stateTransitionOnTimeout[5]	= "Ready";
	stateTimeoutValue[5]		= 0.2;
	stateFire[5]				= true;
	stateSequence[5]			= "fire";
	stateScript[5]				= "onFire";
	stateWaitForTimeout[5]		= true;
	stateAllowImageChange[5]	= false;
};

function mud_Image::onCharge(%this, %obj, %slot)
{
	%obj.playthread(2, spearReady);
}

function mud_Image::onAbortCharge(%this, %obj, %slot)
{
	%obj.playthread(2, root);
}

function mud_Image::onFire(%this, %obj, %slot)
{
	%obj.playthread(2, spearThrow);
	Parent::onFire(%this, %obj, %slot);
}