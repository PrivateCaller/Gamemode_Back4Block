%pattern = "add-ons/gamemode_left4block/add-ins/weapon_distractions/sound/*.wav";//Too lazy to write datablock files for the sounds, just took this from the Disease Gamemode
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/gamemode_left4block/add-ins/weapon_distractions/sound/", "");
	%soundName = strreplace(%soundName, "/", "");
	%soundName = strreplace(%soundName, ".wav", "");
	%soundName = strreplace(%soundName, "quiet", "");
	%soundName = strreplace(%soundName, "normal", "");
	%soundName = strreplace(%soundName, "loud", "");

	//Check the names of the folders to determine what type of soundscape it will be, and check if it's a loopable sound or not
	if(strstr(%file,"normal") != -1)//Normal soundscape
	if(strstr(%file,"loop") != -1)
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioCloseLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClose3d; filename = \"" @ %file @ "\"; };");

	if(strstr(%file,"quiet") != -1)//Quiet soundscape
	if(strstr(%file,"loop") != -1)
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosestLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosest3d; filename = \"" @ %file @ "\"; };");

	if(strstr(%file,"loud") != -1)//Loudest Soundscape
	if(strstr(%file,"loop") != -1)
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioDefaultLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioDefault3d; filename = \"" @ %file @ "\"; };");

	%file = findNextFile(%pattern);
}

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
	projectileShapeName = "./models/sPipeBombProjectile.dts";
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

	shapeFile = "./models/sPipeBombItem.dts";
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
   shapeFile = "./models/sPipeBombWeapon.dts";
   emap = true;
   mountPoint = 0;
   offset = "0.1 -0.1 0.7";
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

function sPipeBombImage::onFlick(%this, %obj, %slot)
{
	%obj.playthread(2, shiftRight);
}

function sPipeBombImage::onRemove(%this, %obj, %slot)
{
	%obj.playthread(2, shiftRight);
}

function sPipeBombImage::onCharge(%this, %obj, %slot)
{
	%obj.playthread(2, spearReady);
	%obj.lastPipeSlot = %obj.currTool;
}

function sPipeBombImage::onAbortCharge(%this, %obj, %slot)
{
	%obj.playthread(2, root);
}

function sPipeBombImage::onFire(%this, %obj, %slot)
{
	Parent::onFire(%this, %obj, %slot);
	
	%currSlot = %obj.lastPipeSlot;
	%obj.tool[%currSlot] = 0;
	%obj.weaponCount--;
	messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
	serverCmdUnUseTool(%obj.client);
   	%obj.playthread(2, spearThrow);
}

function sPipeBombProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
{
   if(%col.getClassName() !$= "Player" || %col.getClassName() !$= "AIPlayer")
	serverPlay3D("pipebomb_bounce_sound",%obj.getTransform());
}

function Projectile::PipeBombDistract(%obj, %flashcount)
{ 
	%pos = %obj.getPosition();
	%radius = 100;
	%searchMasks = $TypeMasks::PlayerObjectType;
	InitContainerRadiusSearch(%pos, %radius, %searchMasks);

	while ((%targetid = containerSearchNext()) != 0 )
	{
		if(!%targetid.getState() !$= "Dead" && %targetid.getClassName() $= "AIPlayer" && %targetid.hZombieL4BType && %targetid.hZombieL4BType < 5 && !%targetid.isBurning)
		{
			if(%flashcount < 15)
			{
				if(!%targetid.Distraction)
				{
					%targetid.Distraction = %obj.getID();
					%targetid.hSearch = 0;
				}

				else if(%targetid.Distraction $= %obj.getID())
				{
					%targetid.setmoveobject(%obj);
					%targetid.setaimobject(%obj);
					%time1 = getRandom(1000,4000);
					%targetid.getDataBlock().schedule(%time1,onBotFollow,%targetid,%obj);
				}

			}
			else
			{
				%targetid.hSearch = 1;
				%targetid.Distraction = 0;
			}
		}
	}

	if(%flashcount < 4)
	{
		%sound = Phone_Tone_1_Sound;
		%time = 750;
	}
	else if(%flashcount < 8)
	{
		%sound = Phone_Tone_3_Sound;
		%time = 500;
	}
	else if(%flashcount < 12)
	{
		%sound = Phone_Tone_4_Sound;
		%time = 375;
	}
	else if(%flashcount < 16)
	{
		%sound = Phone_Tone_4_Sound;
		%time = 250;
	}
	else if(%flashcount > 16)
	{
		%obj.explode();
		return;
	}

	%obj.schedule(%time,PipeBombDistract,%flashcount+1);
	serverPlay3d(%sound,%pos);

	%p = new Projectile()
	{
		dataBlock = sPipeBombLightProjectile;
		initialPosition = %pos;
		initialVelocity = "0 0 1";
		sourceObject = %obj.sourceObject;
		client = %obj.client;
		sourceSlot = 0;
		originPoint = %obj.originPoint;
	};

	if(isObject(%p))
	{
		MissionCleanup.add(%p);
		%p.setScale(%obj.getScale());
	}
}

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
	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	 // Dynamic properties defined by the scripts
	image = BileBombImage;
	canDrop = true;
};

datablock ShapeBaseImageData(BileBombImage)
{
   // Basic Item properties
   shapeFile = "./models/BileBombItem.dts";
   emap = true;

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   offset = "-0.05 0.05 0";
   //eyeOffset = "0.1 0.2 -0.55";

   // When firing from a point offset from the eye, muzzle correction
   // will adjust the muzzle vector to point to the eye LOS point.
   // Since this weapon doesn't actually fire from the muzzle point,
   // we need to turn this off.  
   correctMuzzleVector = true;

   // Add the WeaponImage namespace as a parent, WeaponImage namespace
   // provides some hooks into the inventory system.
   className = "WeaponImage";

   // Projectile && Ammo.
   item = BileBombItem;
   ammo = " ";
   projectile = BileBombProjectile;
   projectileType = Projectile;

   //melee particles shoot from eye node for consistancy
   melee = false;
   //raise your arm up or not
   armReady = true;

   //casing = " ";
   doColorShift = true;
   colorShiftColor = "1 1 1 1";

   // Images have a state system which controls how the animations
   // are run, which sounds are played, script callbacks, etc. This
   // state system is downloaded to the client so that clients can
   // predict state changes and animate accordingly.  The following
   // system supports basic ready->fire->reload transitions as
   // well as a no-ammo->dryfire idle state.

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

function BileBombImage::onCharge(%this, %obj, %slot)
{
	%obj.playthread(2, spearReady);
	%obj.lastBileSlot = %obj.currTool;
}

function BileBombImage::onAbortCharge(%this, %obj, %slot)
{
	%obj.playthread(2, root);
}

function BileBombImage::onFire(%this, %obj, %slot)
{
	%obj.playthread(2, spearThrow);
	Parent::onFire(%this, %obj, %slot);
	%obj.hasBileBomb = 0;
	
	%currSlot = %obj.lastBileSlot;
	%obj.tool[%currSlot] = 0;
	%obj.weaponCount--;
	%obj.unMountImage(0);
	messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
	serverCmdUnUseTool(%obj.client);
}

function Projectile::BileBombDistract(%obj, %count)
{
	if(isObject(%obj) || %count < %obj.getDatablock().distractionLifetime)
	%obj.ContinueSearch = %obj.schedule(1000,BileBombDistract,%count+1);

	%pos = %obj.getPosition();
	%radius = %obj.getDatablock().DistractionRadius;

	%searchMasks = $TypeMasks::PlayerObjectType;
	InitContainerRadiusSearch(%pos, %radius, %searchMasks);

	while((%targetid = containerSearchNext()) != 0 )
	{
		if(!%targetid.getState() !$= "Dead" && %targetid.getClassName() $= "AIPlayer" && %targetid.hZombieL4BType && %targetid.hZombieL4BType < 5 && !%targetid.isBurning)
		{
			if(%count < %obj.getDatablock().distractionLifetime)
			{
				if(!%targetid.Distraction)
				{
					%targetid.Distraction = %obj.getID();
					%targetid.hSearch = 0;
				}
				else if(%targetid.Distraction == %obj.getID())
				{
					%targetid.setaimobject(%obj);
					%targetid.setmoveobject(%obj);
					%time1 = getRandom(1000,4000);
					%targetid.getDataBlock().schedule(%time1,onBotFollow,%targetid,%obj);
				}

				if(%obj.getdataBlock().getID() == BileBombFakeProjectile.getID() && ContainerSearchCurrRadiusDist() <= 4)
				{
					if(%targetid.hType $= "zombie")
					{
						%targetid.hType = "biled" @ getRandom(1,9999);
						%targetid.mountImage(BileStatusPlayerImage, 2);
						%targetid.BoomerBiled = 1;
					}
				}
			}
			else
			{
				%targetid.hSearch = 1;
				%targetid.Distraction = 0;
			}
		}
	}
}

function BileBombProjectile::onCollision(%this, %obj)
{
	%pos = %obj.getPosition();
	serverPlay3d("bilejar_explode_sound",%pos);

   %p = new Projectile()
   {
      dataBlock = BileBombFakeProjectile;
      initialPosition = %pos;
      initialVelocity = "0 0 1";
      sourceObject = %obj.sourceObject;
      client = %obj.client;
      sourceSlot = 0;
      originPoint = %obj.originPoint;
   };
   if(isObject(%p))
   {
      MissionCleanup.add(%p);
      %p.setScale(%obj.getScale());
   } 
}