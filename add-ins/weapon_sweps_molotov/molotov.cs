datablock AudioProfile(napalmFireLoop1Sound)
{
	filename	= "./sound/fire_loop1.wav";
	description = AudioDefaultLooping3d;
	preload = true;
};
datablock AudioProfile(napalmFireLoop2Sound)
{
	filename	= "./sound/fire_loop2.wav";
	description = AudioDefaultLooping3d;
	preload = true;
};
datablock AudioProfile(napalmFireEndSound)
{
	filename	= "./sound/fire_end.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(fleshFireLoopSound)
{
	filename	= "./sound/fire_fleshLoop.wav";
	description = AudioDefaultLooping3d;
	preload = true;
};

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
	lifeTime 				= 4023;
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

function flamerRubbishProjectile::onCollision(%db,%proj,%hit,%fade,%pos,%normal)
{
	flamerProjectileExplode(%proj);
}
function flamerProjectileExplode(%proj)
{
	%mask = $TypeMasks::fxBrickObjectType | $TypeMasks::StaticObjectType | $TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType;
	if(!isObject($flamerFireSet))
	$flamerFireSet = new simSet();
	%set = $flamerFireSet;
	%cnt = %set.getCount();
	%pos = %proj.getPosition();
	%attacker = %proj.client;
	%ignore = %proj.ignoreVehicles;
	%attackerPl = %proj.sourceObject;
	if(%cnt != 0)
	{
		%safety = 0;
		%ready = 0;
		while(!%ready && %safety < 12)
		{
			%pass = 1;
			for(%i=%cnt-1;%i>=0;%i--)
			{
				%obj = %set.getObject(%i);
				%fpos = %obj.position;
				%dist = vectorDist(%pos,%fpos);
				if(%dist < 1)
				{
					%pass = 0;
					%tmp = vectorAdd(%pos,(getRandom()-0.5)*2 SPC (getRandom()-0.5)*3 SPC 0);
					%ray = containerRayCast(%tmp,vectorAdd(%tmp,"0 0 -1.5"),%mask);
					if(getWord(%ray,0))
						%pos = %tmp;
					break;
				}
			}
			if(%pass)
				%ready = 1;
			%safety++;
		}
	}
	%damageType = %proj.damageType;
	if(%damageType $= "")
		%damageType = $DamageType::Flamer;
	%proj.delete();
	if(%pass)
	{
		%ray = containerRayCast(%pos,vectorAdd(%pos,"0 0 -1.5"),%mask);
		if(getWord(%ray,0))
			%pos = getWords(%ray,1,3);
	}
	%p = new projectile()
	{
		datablock = flamerFakeProjectile;
		initialPosition = %pos;
		initialVelocity = "0 0 -1";
	};
	%p.explode();
	%exp = new simSet()
	{
		tick = 0;
		sourceObject = %attackerPl;
		beGentleWith = %attackerPl;
		client = %attacker;
		position = %pos;
		ignoreVehicles = %ignore;
		damageType = %damageType;
	};
	$flamerFireSet.add(%exp);
	if(!isEventPending($flamerFireGlobalLoop))
		flamerExplosionLoop();
	flameSetAdd(getFlameSet(%exp),%exp);
}
function flamerExplosionLoop()
{
	cancel($flamerFireGlobalLoop);
	%set = $flamerFireSet;
	%cnt = %set.getCount();
	
	if(%cnt == 0)
		return;
	%sparks = 0;
	for(%i=%cnt-1;%i>=0;%i--)
	{
		%exp = %set.getObject(%i);
		%pos = %exp.position;
		%tick = %exp.tick;
		
		if(!getRandom(0,3) && %sparks > 0)
		{
			%p = new projectile()
			{
				datablock = flamerSparkProjectile;
				initialPosition = vectorAdd(%pos,"0 0 0.1");
				initialVelocity = "0 0 10";
			};
			%p.explode();
			%sparks--;
		}
		
		%dist = 2.5;
		%max = 10;
		%dmg = 3;
		%mask = $TypeMasks::PlayerObjectType;// | $TypeMasks::CorpseObjectType;
		initContainerRadiusSearch(%pos,mSqrt(%dist*%dist+%dist*%dist)+0.1,%mask);
		while(%hit = containerSearchNext())
		{
			if(%hit.getDatablock().noBurning)
				continue;
			%hPos = %hit.getPosition();
			%hDist = vectorDist(%pos,%hPos);
			if(%exp.beGentleWith == %hit)
			{
				if(%hDist > %dist/2)
					continue;
			}
			else if(%hDist > %dist)
				continue;
			
			%fact = ((%dist-%hDist)/%dist);
			if(%exp.beGentleWith == %hit)
				%fact *= 0.5;
			
			if(getSimTime()-%hit.lastFireDmg > 100)
			{
				cancel(%hit.burnSchedRem);
				%hit.lastFireDmg = getSimTime();
				%dmgd = %dmg*%fact;
				if(minigameCanDamage(%exp.client,%hit) == 1)
				{
					%hit.lastFireAttacker = %exp.client;
					%hit.lastBurnDmgType = %exp.damageType;
					%hit.flamer_burnStart(mCeil(%dmgd)*3);
				}
			}
		}
			
		%exp.tick++;
		if(%exp.tick > 60)
		{
			%clumpSet = %exp.set;
			%exp.delete();
			flameSetFlamePop(%clumpSet);
			%i--;
			continue;
		}
	}
	$flamerFireGlobalLoop = schedule(50,0,flamerExplosionLoop);
}




datablock AudioProfile(molotovThrowSound)
{
	filename    = "./sound/molotov_throw.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(molotovLightSound)
{
	filename    = "./sound/molotov_light.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(molotovExplosionSound)
{
	filename    = "./sound/molotov_explode.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(molotovLoopSound)
{
	filename    = "./sound/molotov_loop.wav";
	description = AudioCloseLooping3D;
	preload = true;
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
	projectileShapeName 	= "./model/molotov_proj.dts";
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

	shapeFile = "./model/molotov.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Molotov";
	iconName = "./model/icon/icon_molotov";
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
	shapeFile = "./model/molotov.dts";
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
	stateTimeoutValue[1]		= 0.25;
	stateTransitionOnTimeout[1]	= "FlickB";
	stateSequence[1]		= "ready";
	stateScript[1]			= "onFlick";
	stateEmitter[1]                 = molotovSparkEmitter;
	stateEmitterNode[1]             = "mountPoint";
	stateEmitterTime[1]             = "2";
	stateSound[1]			= molotovLightSound;
	
	stateName[7]			= "FlickB";
	stateTimeoutValue[7]		= 0.3;
	stateTransitionOnTimeout[7]	= "Ready";
	stateSequence[7]		= "ready";
	stateEmitter[7]                 = molotovSparkEmitter;
	stateEmitterNode[7]             = "mountPoint";
	stateEmitterTime[7]             = "2";
	stateSound[7]			= molotovLoopSound;

	stateName[2]			= "Ready";
	stateTransitionOnTriggerDown[2]	= "Charge";
	stateTransitionOnTimeout[2]	= "Ready";
	stateTimeoutValue[2]		= 0.15;
	stateWaitForTimeout[2]		= 0;
	stateAllowImageChange[2]	= true;
	stateEmitter[2]                 = molotovSparkEmitter;
	stateEmitterNode[2]             = "mountPoint";
	stateEmitterTime[2]             = "2";
	stateSound[2]			= molotovLoopSound;
	
	stateName[3]                    = "Charge";
	stateTransitionOnTimeout[3]	= "Armed";
	stateTimeoutValue[3]            = 0.5;
	stateWaitForTimeout[3]		= false;
	stateTransitionOnTriggerUp[3]	= "AbortCharge";
	stateScript[3]                  = "onCharge";
	stateAllowImageChange[3]        = false;
	stateEmitter[3]                 = molotovSparkEmitter;
	stateEmitterNode[3]             = "mountPoint";
	stateEmitterTime[3]             = "0.6";
	stateSound[3]			= molotovLoopSound;
	
	stateName[4]			= "AbortCharge";
	stateTransitionOnTimeout[4]	= "Ready";
	stateTimeoutValue[4]		= 0.3;
	stateWaitForTimeout[4]		= true;
	stateScript[4]			= "onAbortCharge";
	stateAllowImageChange[4]	= false;
	stateEmitter[4]                 = molotovSparkEmitter;
	stateEmitterNode[4]             = "mountPoint";
	stateEmitterTime[4]             = "0.4";
	stateSound[4]			= molotovLoopSound;

	stateName[5]			= "Armed";
	stateWaitFotTimeout[5]		= 0;
	stateTransitionOnTimeout[5]	= "Armed";
	stateTimeoutValue[5]		= 0.1;
	stateTransitionOnTriggerUp[5]	= "Fire";
	stateAllowImageChange[5]	= false;
	stateEmitter[5]                 = molotovSparkEmitter;
	stateEmitterNode[5]             = "mountPoint";
	stateEmitterTime[5]             = "0.2";
	stateSound[5]			= molotovLoopSound;

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

function molotovImage::onFlick(%this, %obj, %slot)
{
	%obj.playthread(2, shiftRight);
}

function molotovImage::onRemove(%this, %obj, %slot)
{
	%obj.playthread(2, shiftRight);
}

function molotovImage::onCharge(%this, %obj, %slot)
{
	%obj.playthread(2, spearReady);
	%obj.lastPipeSlot = %obj.currTool;
}

function molotovImage::onAbortCharge(%this, %obj, %slot)
{
	%obj.playthread(2, root);
}

function molotovImage::onFire(%this, %obj, %slot)
{
	Parent::onFire(%this, %obj, %slot);
	
	%currSlot = %obj.lastPipeSlot;
	%obj.tool[%currSlot] = 0;
	%obj.weaponCount--;
	messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
	serverCmdUnUseTool(%obj.client);
	serverPlay3D(molotovThrowSound,%obj.getPosition());

	%obj.playthread(2, spearThrow);
}

function molotovProjectile::onExplode(%db,%proj,%expos,%b)
{
	parent::onExplode(%db,%proj,%expos,%b);
	molotov_explode(%expos,%proj,%proj.client);
}
function molotov_explode(%pos,%obj,%cl)
{
	createFireCircle(%pos,10,30,%cl,%obj,$DamageType::Molotov);
	serverPlay3D(molotovExplosionSound,%pos);
}