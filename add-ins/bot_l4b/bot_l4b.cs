%pattern = "add-ons/gamemode_left4block/add-ins/bot_l4b/sound/*.wav";//Too lazy to write datablock files for the sounds, just took this from the Disease Gamemode
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/gamemode_left4block/add-ins/bot_l4b/sound/", "");
	%soundName = strreplace(%soundName, "/", "");
	%soundName = strreplace(%soundName, ".wav", "");
	%soundName = strreplace(%soundName, "quiet", "");
	%soundName = strreplace(%soundName, "normal", "");
	%soundName = strreplace(%soundName, "loud", "");
	%soundName = strreplace(%soundName, "snd-tank", "");//Has to be written with snd-zombiename or it might replace the name of the zombie sound files themselves
	%soundName = strreplace(%soundName, "snd-witch", "");
	%soundName = strreplace(%soundName, "snd-hunter", "");
	%soundName = strreplace(%soundName, "snd-boomer", "");
	%soundName = strreplace(%soundName, "snd-charger", "");
	%soundName = strreplace(%soundName, "snd-smoker", "");
	%soundName = strreplace(%soundName, "snd-jockey", "");
	%soundName = strreplace(%soundName, "snd-spitter", "");
	%soundName = strreplace(%soundName, "snd-common", "");
	%soundName = strreplace(%soundName, "snd-survivor", "");

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

// Tank Datablocks
datablock TSShapeConstructor(RotZTankDts)
{
	baseshape  = "./models/ztank.dts";
	sequence0  = "./models/ztankroot.dsq root";
	sequence1  = "./models/ztankrun.dsq run";
	sequence2  = "./models/ztankrun.dsq walk";
	sequence3  = "./models/ztankback.dsq back";
	sequence4  = "./models/ztankrunside.dsq side";
	sequence5  = "./models/ztankcrouch.dsq crouch";
	sequence6  = "./models/ztankcrouchrun.dsq crouchrun";
	sequence7  = "./models/ztankcrouchback.dsq crouchback";
	sequence8 = "./models/ztankheadside.dsq headside";
	sequence9 = "./models/ztankjump.dsq jump";
	sequence10 = "./models/ztankjump.dsq standjump";
	sequence11 = "./models/ztankfall.dsq fall";
	sequence12 = "./models/ztankland.dsq land";
	sequence13 = "./models/ztankattack.dsq armattack";
	sequence14 = "./models/ztankactivate.dsq activate";
    sequence15 = "./models/ztankactivate2.dsq activate2";
	sequence16 = "./models/ztankarmcharge.dsq spearready";  
	sequence17 = "./models/ztankarmthrow.dsq spearthrow";  
	sequence18 = "./models/ztankdie.dsq death1";
	sequence19  = "./models/zTankLook.dsq look";
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

datablock shapeBaseImageData( ConstructionConeSpeakerImage )
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

	shapeFile = "./models/GIBLET1.dts";
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

	shapeFile = "./models/GIBLET2.dts";
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

	shapeFile = "./models/NEWGIB5.dts";
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

	shapeFile = "./models/NEWGIB3.dts";
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
	shapeFile = "./models/NEWGIB2.dts";
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

	shapeFile = "./models/NEWGIB4.dts";
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

exec("./bots/common.cs");
exec("./bots/uncommon_soldier.cs");
exec("./bots/uncommon_fallen.cs");
exec("./bots/uncommon_mud.cs");
exec("./bots/uncommon_construction.cs");
exec("./bots/uncommon_clown.cs");
exec("./bots/uncommon_jimmy_gibbs.cs");
exec("./bots/uncommon_ceda.cs");
exec("./bots/uncommon_toxic.cs");
exec("./bots/uncommon_pirate.cs");
exec("./bots/uncommon_police.cs");
exec("./bots/boomer.cs");
exec("./bots/charger.cs");
exec("./bots/hunter.cs");
exec("./bots/jockey.cs");
exec("./bots/smoker.cs");
exec("./bots/spitter.cs");
exec("./bots/tank.cs");
exec("./bots/witch.cs");

function Player::hChangeBotToInfectedAppearance(%obj)
{
	%this = %obj.getdataBlock();
	%obj.resetHoleLoop();

	if(!%this.hNeedsWeapons)
	%obj.setWeapon(-1);

	%obj.hNeutralAttackChance = %this.hNeutralAttackChance;
	%obj.hSearch = %this.hSearch;
	%obj.hSearchRadius = %this.hSearchRadius;
	%obj.hSight = %this.hSight;
	%obj.hSearchFov = %this.hSearchFov;
	%obj.hSuperStacker = %this.hSuperStacker;
	%obj.hAttackDamage = $L4B_NormalDamage;
	%obj.hMelee = %this.hMelee;

	for(%a = 0; %a <= $aCL; %a++)
	{
		%cur = $avatarColorLoop[%a];
		%newskincolor = getWord(%obj.headColor,0)/1.5 SPC getWord(%obj.headColor,1)/1.15 SPC getWord(%obj.headColor,2)/1.5 SPC 1;

		eval("%objC = %obj." @ %cur @ ";");
		if(%objC $= %obj.headColor)
		eval("%obj." @ %cur @ " = %newskincolor;");

		if($Pref::Server::L4B2Bots::CustomStyle < 2)
		%obj.faceName =  "asciiTerror";
		else %obj.faceName =  $hZombieFace[getRandom(1,$hZombieFaceAmount)];
	}

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function AIPlayer::hLimitedLifetime(%obj)
{
	if(!$Pref::Server::L4B2Bots::LimitedLifetime || !isObject(getMinigameFromObject(%obj)) || %obj.hFollowing)
	{
		%obj.hLimitLife = 0;
		return;
	}

	%obj.hLimitLife++;

	if(%obj.hLimitLife >= 25)
	%obj.kill();
}

function Player::onL4BDatablockAttributes(%obj)
{
	%this = %obj.getdataBlock();
	%obj.setDamageLevel(0);
	%obj.schedule(10,setenergylevel,0);

	%obj.hIsInfected = %this.hIsInfected;
	%obj.hZombieL4BType = %this.hZombieL4BType;
	%obj.hType = "Zombie";
	%obj.isStrangling = 0;
	%obj.hEating = 0;

	if(%obj.getClassName() $= "Player")
	{
		%obj.client.isInInfectedTeam = 1;
		commandToClient(%obj.client, 'SetVignette', true, "0.25 0.15 0 1" );

		if(%obj.getdataBlock().getName() $= "CommonZombieHoleBot")
		schedule(10,0,commandToClient,%obj.client, 'centerPrint', "<just:right><font:impact:30>\c6You are \c0" @ %obj.getdataBlock().hName @ "\c6! <br><font:impact:20>\c6Left click to attack or <br>\c6Plant brick key to change zombie types", 5);
		else schedule(10,0,commandToClient,%obj.client, 'centerPrint', "<just:right><font:impact:30>\c6You are a \c0" @ %obj.getdataBlock().hName @ "\c6! <br><font:impact:20>\c6" @ %obj.getdataBlock().SpecialCPMessage @ "<br>\c6Plant brick key to change zombie types", 5);
	}
	
	if(%this.hCustomNodeAppearance)
	%this.hCustomNodeAppearance(%obj);

	if(%obj.hZombieL4BType == 5)
	%obj.playaudio(3,strlwr(%obj.name) @ "_spawn" @ getRandom(1,2) @ "_sound");
}

$hZombieDecalDefault[%n = 1] = "AAA-None";
$hZombieDecalDefault[%n++] = "Mod-Army";
$hZombieDecalDefault[%n++] = "Mod-Police";
$hZombieDecalDefault[%n++] = "Mod-Suit";
$hZombieDecalDefault[%n++] = "Meme-Mongler";
$hZombieDecalDefault[%n++] = "Mod-Daredevil";
$hZombieDecalDefault[%n++] = "Mod-Pilot";
$hZombieDecalDefault[%n++] = "Mod-Prisoner";
$hZombieDecalDefault[%n++] = "Meme-Mongler";
$hZombieDecalDefault[%n++] = "Medieval-YARLY";
$hZombieDecalDefault[%n++] = "Medieval-ORLY";
$hZombieDecalDefault[%n++] = "Medieval-Eagle";
$hZombieDecalDefault[%n++] = "Medieval-Lion";
$hZombieDecalDefault[%n++] = "Medieval-Tunic";
$hZombieDecalDefault[%n++] = "Hoodie";
$hZombieDecalDefault[%n++] = "DrKleiner";
$hZombieDecalDefault[%n++] = "Chef";
$hZombieDecalDefault[%n++] = "worm-sweater";
$hZombieDecalDefault[%n++] = "worm_engineer";
$hZombieDecalDefault[%n++] = "Archer";
$hZombieDecalDefaultAmount = %n;

$hZombieHat[%c++] = 4;
$hZombieHat[%c++] = 6;
$hZombieHat[%c++] = 7;
$hZombieHat[%c++] = 0;
$hZombieHat[%c++] = 1;
$hZombieHatAmount = %c;

$hZombiePack[%d++] = 0;
$hZombiePack[%d++] = 2;
$hZombiePack[%d++] = 3;
$hZombiePack[%d++] = 4;
$hZombiePack[%d++] = 5;
$hZombiePackAmount = %d;

function Player::hDefaultL4BAppearance(%obj)
{	
	%this = %obj.getDataBlock();
	
	switch($Pref::Server::L4B2Bots::CustomStyle)//Random common appearance
	{
		case 0: %randmultiplier = getRandom(200,1000)*0.001;
				%randskin = $hZombieSkin[1];
				%skincolor = getWord(%randskin,0)*%randmultiplier SPC getWord(%randskin,1)*%randmultiplier SPC getWord(%randskin,2)*%randmultiplier SPC 1;

				%decal = $hZombieDecalDefault[getRandom(1,$hZombieDecalDefaultAmount)];
				%face = "asciiTerror";

		case 1:	%randmultiplier = getRandom(400,2000)*0.001;
				%randskin = $hZombieSkin[4];
				%skincolor = getWord(%randskin,0)*%randmultiplier SPC getWord(%randskin,1)*%randmultiplier SPC getWord(%randskin,2)*%randmultiplier SPC 1;

				%decal = $hZombieDecalDefault[getRandom(1,$hZombieDecalDefaultAmount)];
				%face = "asciiTerror";

		case 2: %randmultiplier = getRandom(400,2000)*0.001;
				%randskin = $hZombieSkin[4];
				%skincolor = getWord(%randskin,0)*%randmultiplier SPC getWord(%randskin,1)*%randmultiplier SPC getWord(%randskin,2)*%randmultiplier SPC 1;

				%decal = $hZombieDecal[getRandom(1,$hZombieDecalAmount)];		
				%face = $hZombieFace[getRandom(1,$hZombieFaceAmount)];

		case 3: %randmultiplier = getRandom(400,2000)*0.001;
				%randskin = $hZombieSkin[getRandom(2,$hZombieSkinAmount)];
				%skincolor = getWord(%randskin,0)*%randmultiplier SPC getWord(%randskin,1)*%randmultiplier SPC getWord(%randskin,2)*%randmultiplier SPC 1;

				%decal = $hZombieDecal[getRandom(1,$hZombieDecalAmount)];
				%face = $hZombieFace[getRandom(1,$hZombieFaceAmount)];
	}

	%chest = getRandom(0,1);
	%hat = $hZombieHat[getRandom(1,$hZombieHatAmount)];
	%pack = $hZombiePack[getRandom(1,$hZombiePackAmount)];

	if(%obj.hZombieL4BType == 1)
	{
		if(getRandom(1,4) == 1)
		%obj.hZombieL4BType = getrandom(2,3);
	}
	
	switch(%obj.hZombieL4BType)
	{
		case 1: %obj.getDataBlock().L4BCommonAppearance(%obj,%skinColor,%face,%decal,%hat,%pack,%chest);
		case 2: %obj.getDataBlock().L4BCommonFastAppearance(%obj,%skinColor,%face,%chest);
		case 3: %obj.getDataBlock().L4BCommonAppearance(%obj,%skinColor,%face,%decal,%hat,%pack,%chest);
		case 4: %obj.getDataBlock().L4BUncommonAppearance(%obj,%skinColor,%face,%decal,%hat,%pack,%chest);			
		case 5: %obj.getDataBlock().L4BSpecialAppearance(%obj,%skinColor,%face,%decal,%hat,%pack,%chest);
	}

	if(%obj.getDataBlock().hCustomNodeAppearance)
	%obj.getDataBlock().hCustomNodeAppearance(%obj);
}

function L4B_SpazzZombie(%obj,%count)
{	
	if(!isObject(%obj) || %obj.getstate() $= "Dead" || %count >= 15)
	return;

	if(!%obj.hLoopActive && %obj.lastheadshake+getrandom(250,750) < getsimtime())
	{
		%obj.playthread(0,undo);
		%obj.lastheadshake = getsimtime();
	}
	%obj.activateStuff();

	cancel(%obj.L4B_SpazzZombie);
	%time = getRandom(100,200);
	%obj.L4B_SpazzZombie = schedule(%time,0,L4B_SpazzZombie,%obj,%count+1);
}

function L4B_ZombieDropLoot(%obj,%lootitem,%chance)
{
	if(!isObject(%lootitem))
	return;
	else if(getRandom(1,100) <= %chance)
	{
		%loot = new item()
		{
			dataBlock = %lootitem;
			position = %obj.getHackPosition();
			dropped = 1;
			canPickup = 1;
			client = %cl;
			minigame = getMinigameFromObject(%obj);
		};
		missionCleanup.add(%loot);

		%loot.applyimpulse(%loot.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),getRandom(4/4,4)),getRandom(4*-1,4) @ " 0 " @ getRandom(6/3,6)));
		%loot.fadeSched = %loot.schedule(8000,fadeOut);
		%loot.delSched = %loot.schedule(8200,delete);
	}
}

function L4B_ZombieLunge(%obj,%targ,%power)
{
	if(!isObject(%obj) || !isObject(%targ) || !L4B_IsOnGround(%obj) || %obj.getState() $= "Dead")
	return;

	if(isObject(%obj.light))
	%obj.light.delete();

	%dis = VectorSub(%targ.getposition(),%obj.getposition());
	%normVec = VectorNormalize(vectoradd(%dis,"0 0" SPC 0.15*vectordist(%targ.getposition(),%obj.getposition())));
	%obj.playthread(0,jump);

	%eye = vectorscale(%normVec,2);
	%mp = %power;
	%final = vectorscale(%eye,%mp);
	%obj.setvelocity(%final);
}

function Player::bigZombieMelee(%obj)
{	
	if(%obj.bigMeleeShed+750 > getSimTime())
	return;

	%obj.bigMeleeShed = getSimTime();

	%this = %obj.getdataBlock();
	%oscale = getWord(%obj.getScale(),2);
	%mask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	initContainerRadiusSearch(%obj.getEyePoint(),10,%mask);
	while(%hit = containerSearchNext())
	{
		if(%hit == %obj)
		continue;

		%line = vectorNormalize( vectorSub( %obj.getposition(), %hit.getposition()));
		%dot = vectorDot( %obj.getEyeVector(), %line );
		%obscure = containerRayCast(%obj.getEyePoint(),vectorAdd(%hit.getPosition(),"0 0 1.9"),$TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::FxBrickObjectType, %obj);

		if(isObject(%obscure) || ContainerSearchCurrRadiusDist() > 2 || %dot > -0.5)
		continue;

		if(%hit.getType() & $TypeMasks::PlayerObjectType && miniGameCanDamage(%obj,%hit) && checkHoleBotTeams(%obj,%hit))
		{
			if(%hit.getstate() $= "Dead")
			continue;

			%obj.playaudio(3,%this.hBigMeleeSound);
			%obj.playthread(1,"activate2");
			
			if(%this.getName() $= "ZombieTankHoleBot" || vectorDot(%obj.getVelocity(), %obj.getForwardVector()) < 20)
			%hit.applyimpulse(%hit.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),2000),"0 0 500"));
			else
			{
				%normVec = VectorNormalize(vectorAdd(%obj.getForwardVector(),"0" SPC "0" SPC "0.15"));
				%eye = vectorscale(%normVec,30);
				%hit.setvelocity(%eye/2);
			}
			%hit.damage(%obj.hFakeProjectile, %hit.getposition(), $Pref::Server::L4B2Bots::SpecialsDamage*%oScale, %obj.hDamageType);

			%p = new Projectile()
			{
				dataBlock = "BigZombieHitProjectile";
				initialPosition = %hit.getPosition();
				sourceObject = %obj;
				client = %obj.client;
			};
			MissionCleanup.add(%p);
			%p.explode();

			%c = new Projectile()
			{
				dataBlock = "pushBroomProjectile";
				initialPosition = %hit.getPosition();
				sourceObject = %obj;
				client = %obj.client;
			};
			MissionCleanup.add(%c);
			%c.explode();
			%c.setScale("2 2 2");
		}
		if(%hit.getType() & $TypeMasks::VehicleObjectType)
		{
			%obj.playaudio(3,%this.hBigMeleeSound);
			%obj.playthread(2,"activate2");

			%muzzlepoint = vectorSub(%obj.getHackPosition(),"0 0 0.5");
			%muzzlevector = vectorScale(%obj.getEyeVector(),2.5);
			%muzzlepoint = VectorAdd (%muzzlepoint, %muzzlevector);
			%hit.setTransform (%muzzlepoint @ " " @ rotFromTransform(%hit.getTransform()));
			%impulse = VectorNormalize(VectorAdd (%obj.getEyeVector(), "0 0 0.2"));
			%force = %hit.getDataBlock ().mass * 25;
			%scaleRatio = getWord (%obj.getScale (), 2) / getWord (%hit.getScale (), 2);
			%force *= %scaleRatio;
			%impulse = VectorScale (%impulse, %force);
			%hit.schedule(50,applyImpulse,%hit.getPosition(),%impulse);
			%hit.damage(%obj.hFakeProjectile, %hit.getposition(), 50*getWord(%obj.getScale(),0), $DamageType::Tank);

			%c = new Projectile()
			{
				dataBlock = "pushBroomProjectile";
				initialPosition = %hit.getPosition();
				sourceObject = %obj;
				client = %obj.client;
			};
			MissionCleanup.add(%c);
			%c.explode();
			%c.setScale("2 2 2");
		}
	}
}

function Player::PlayerZombieMeleeAttack(%obj,%col)
{
	if(%obj.getState() $= "Dead")
	return;

	if(%col.getType() & $TypeMasks::VehicleObjectType || %col.getType() & $TypeMasks::PlayerObjectType && %obj.lasthit+250 < getsimtime())
	{
		%obj.lasthit = getsimtime();

		if(%obj.getdataBlock().getName() !$= "ZombieChargerHoleBot")
		{
			%damage = 5*getWord(%obj.getScale(),0);
			%damage = %damage*1.5;
			%damagefinal = getRandom(%damage/2,%damage);

			%col.damage(%obj.hFakeProjectile, %col.getposition(), %damagefinal, %obj.hDamageType);
			%meleeimpulse = mClamp(%damagefinal, 1, 7.5);
			%obj.playaudio(0,"melee_hit" @ getrandom(1,8) @ "_sound");

			%p = new Projectile()
			{
				dataBlock = "ZombieHitProjectile";
				initialPosition = %col.getPosition();
				sourceObject = %obj;
				client = %obj.client;
			};
			MissionCleanup.add(%p);
			%p.explode();

			%col.applyimpulse(%col.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),getrandom(100,100*%meleeimpulse)),"0" SPC "0" SPC getrandom(100,100*%meleeimpulse)));
			%col.playthread(3,"plant");
			%obj.playthread(1,"activate2");
			%obj.playthread(2,"jump");

			if(%col.getType() & $TypeMasks::PlayerObjectType && %col.getState() !$= "Dead" && %col.getDamageLevel() >= %col.getDataBlock().maxDamage/1.333 && !%col.hIsInfected && !%col.hIsImmune)
			holeZombieInfect(%obj,%col);
		}
		else %obj.bigZombieMelee();
	}
}

function Player::SpecialPinAttack(%obj,%col,%force)
{	
	if(!isObject(%col) || !isObject(%obj))
	return;

	if(%col.getType() & $TypeMasks::PlayerObjectType && checkHoleBotTeams(%obj,%col))
	{	
		%shape = %col.getDataBlock().shapeFile;
		if(miniGameCanDamage(%obj,%col) && %obj.getState() !$= "Dead" && %col.getState() !$= "Dead" && !%obj.isStrangling && !%col.isBeingStrangled && %obj.laststun+5000 < getsimtime() && %shape $= "base/data/shapes/player/m.dts" || %shape $= "base/data/shapes/player/mmelee.dts")
		{
			%obj.laststun = getsimtime();
			%col.isBeingStrangled = 1;
			%obj.isStrangling = 1;
			%obj.hEating = %col;
			%col.hEater = %obj;

			if(%obj.getClassName() $= "AIPlayer")
			{
				%obj.schedule(100,hClearMovement);
				%obj.stopHoleLoop();
				%obj.hIgnore = %col;
			}

			switch$(%col.getclassname())
			{
				case "Player":	if($Pref::Server::L4B2Bots::MinigameMessages)
								{
									chatMessageTeam(%col.client,'fakedeathmessage',"<color:FFFF00>" @ %obj.getDatablock().hName SPC %obj.getdataBlock().hPinCI SPC %col.client.name);
									%col.client.minigame.L4B_PlaySound("victim_needshelp_sound");
								}

								%col.client.camera.setOrbitMode(%col, %col.getTransform(), 0, 5, 0, 1);
								%col.client.setControlObject(%col.client.camera);
								ServerCmdUnUseTool (%target.client);

				case "AIPlayer": %col.stopHoleLoop();
			}

			switch$(%obj.getdataBlock().getName()) 
			{
				case "ZombieChargerHoleBot": %obj.mountObject(%col,0);
											 %obj.playthread(1,"root");
											 %obj.hSharkEatDelay = schedule(2000,0,L4B_holeChargerKill,%obj,%col);
											 %forcedam = %force/2;
											 %col.damage(%obj.hFakeProjectile, %col.getposition(),%forcedam, %obj.hDamageType);

											%p = new Projectile()
											{
												dataBlock = "BigZombieHitProjectile";
												initialPosition = %col.getPosition();
												sourceObject = %obj;
												client = %obj.client;
											};
											MissionCleanup.add(%p);
											%p.explode();

				case "ZombieHunterHoleBot": %obj.playthread(0,root);
											%col.playthread(0,death1);

											%phackloc = %col.getHackPosition();
											%obj.schedule(10,setvelocity,"0 0 0");
											%obj.schedule(5,setTransform,%phackloc SPC %phackloc);

											%obj.HunterHurt = schedule(100,0,L4B_holeHunterKill,%obj,%col);

											%forcedam = %force/2;
											%col.damage(%obj.hFakeProjectile, %col.getposition(),%forcedam, %obj.hDamageType);

				case "ZombieJockeyHoleBot":	%col.mountObject(%obj,2);
											%obj.setControlObject(%col);

											%obj.playthread(0,sit);
											%obj.playthread(1,armreadyboth);
											%obj.playaudio(0,"jockey_attack_loop" @ getrandom(1,2) @ "_sound");

											%obj.JockeyHurt = schedule(1000,0,L4B_holeJockeyKill,%obj,%col);

				case "ZombieSmokerHoleBot":  %obj.playaudio(1,"smoker_launch_tongue_sound");
											 %col.playaudio(2,"smoker_tongue_hit_sound");
											 %obj.playthread(2,"plant");
											 %obj.playthread(3,"shiftup");
											 %col.mountImage(ZombieSmokerConstrictImage, 2);
			}
		}
	}
}

function L4B_SpecialsPinCheck(%obj,%col)
{
	if(!miniGameCanDamage(%obj,%col) || !isObject(%obj) || %obj.getstate() $= "Dead" || !isObject(%col) || (isObject(%col) & %col.getState() $= "Dead" && %col.isBeingStrangled || %col.hIsInfected))
	{
		if(isObject(%obj))
		{
			%obj.isStrangling = 0;
	
			if(isObject(%obj.light))
			%obj.light.delete();			
			%obj.stopAudio(0);

			%obj.hIgnore = 0;
			%obj.hEating = 0;
	
			if(%obj.getdataBlock().getName() $= "ZombieJockeyHoleBot")
			{
				%obj.dismount();
				%obj.setControlObject(%obj);
			}

			if(%obj.getState() !$= "Dead")
			{
				%obj.setDamageLevel(0);
				%obj.playThread(1,root);
				%obj.emote(HealImage,3);

				if(%obj.getClassName() $= "AIPlayer")
				{
					if(isObject(%col))
					%obj.hRunAwayFromPlayer(%col);
					else %obj.hRunAwayFromPlayer(%obj);
					%obj.schedule(4000,resetHoleLoop);
				}
			}
		}

		if(isObject(%col))
		{
			%col.isBeingStrangled = 0;

			if(isObject(%col.getMountedImage(2)) && %col.getMountedImage(2).getID() == ZombieSmokerConstrictImage.getID())
			%col.unMountImage(2);

			if($L4B_hasSelectiveGhosting)
			Billboard_DeallocFromPlayer(%col, "Strangled");

			if(%col.getstate() !$= "Dead")
			{
				if(isObject(%col.client))
				%col.client.setControlObject(%col);
				else %col.setControlObject(%col);
				%col.playthread(0,root);

				if(%col.getClassName() $= "AIPlayer")
				%col.resetHoleLoop();
			}
		}
		return false;
	}
	else 
	{
		Billboard_NeedySurvivor(%col, "Strangled");
		return true;
	}
}

function fxDTSBrickData::onTankTouch(%data,%obj,%player)
{
	fxDTSBrickData::onZombieTouch(%data,%obj,%player);
}
registerInputEvent ("fxDTSBrick", "onTankTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerInputEvent ("fxDTSBrick", "onDoorClose", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerInputEvent ("fxDTSBrick", "onDoorOpen", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame");

function fxDTSBrick::RandomizeZombieSpecial(%obj)
{
	%obj.hBotType = $hZombieSpecialType[getRandom(1,$hZombieSpecialTypeAmount)];	
}
registerOutputEvent(fxDTSBrick, "RandomizeZombieSpecial");

function fxDTSBrick::RandomizeZombieUncommon(%obj)
{
	%obj.hBotType = $hZombieUncommonType[getRandom(1,$hZombieUncommonTypeAmount)];	
}
registerOutputEvent(fxDTSBrick, "RandomizeZombieUncommon");