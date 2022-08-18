%pattern = "add-ons/gamemode_left4block/add-ins/script_secondary_melee/sound/*.wav";//Too lazy to write datablock files for the sounds, just took this from the Disease Gamemode
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/gamemode_left4block/add-ins/script_secondary_melee/sound/", "");
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
	camShakeRadius = 10.0;

	camShakeFreq = "3 3 3";
	camShakeAmp = "0.6 0.6 0.6";
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
AddDamageType("SecondaryMelee",'<bitmap:add-ons/Gamemode_Left4Block/add-ins/script_secondary_melee/icons/ci_punch> %1','%2 <bitmap:add-ons/Gamemode_Left4Block/add-ins/script_secondary_melee/icons/ci_punch> %1',0,1);

function Player::doMelee(%obj)
{
	%obj.playThread(3,"activate2");
	serverPlay3D("melee_swing" @ getRandom(1,2) @ "_sound",%obj.getPosition());

	%pos = %obj.getEyePoint();
	%masks = $TypeMasks::StaticObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	initContainerRadiusSearch(%pos,5,%masks);
	while(%hit = containerSearchNext())
	{
      	if(%hit == %obj)
      	continue;
	
      	%len = 5 * getWord(%obj.getScale (), 2);
      	%vec = %obj.getEyeVector();
      	%beam = vectorScale(%vec,%len); //lengthened vector (for calculating the raycast's endpoint)
      	%end = vectorAdd(%pos,%beam); //calculated endpoint for raycast
      	%ray = containerRayCast(%pos,%end,%masks,%obj); //fire raycast
		
      	%line = vectorNormalize(vectorSub(%pos,%hit.getposition()));
		%dot = vectorDot(%vec,%line);

     	if(vectorDist(%pos,%hit.getposition()) > 5 || %dot > -0.4)
		continue;

		if(isObject(%ray))
        if(%ray.getType() & $TypeMasks::StaticObjectType || %ray.getType() & $TypeMasks::FxBrickObjectType || %ray.getType() & $TypeMasks::VehicleObjectType)
     	{
			serverPlay3D(%MeleeType @ "HitEnv_Sound",posFromRaycast(%ray));

			%p = new projectile()
			{
				datablock = "SecondaryMeleeProjectile";
				initialPosition = posFromRaycast(%ray);
			};
			%p.explode();
			return;
     	}

		%obscure = containerRayCast(%obj.getEyePoint(),vectorAdd(%hit.getPosition(),"0 0 1.9"),$TypeMasks::VehicleObjectType | $TypeMasks::StaticObjectType | $TypeMasks::FxBrickObjectType, %obj);
		if(isObject(%obscure))
		continue;

		if(%hit.getType() & $TypeMasks::PlayerObjectType)
		{
			if(L4B_CheckifinMinigame(%obj,%hit))
			{
				%p = new projectile()
				{
					datablock = "SecondaryMeleeProjectile";
					initialPosition = posFromRaycast(%ray);
				};
				%p.explode();

				serverPlay3D("melee_hit" @ getrandom(1,8) @ "_sound",posFromRaycast(%ray));
				%dmg = %hit.getdatablock().maxDamage/4;
				%hitknockback = 1200;
				%hitz = 500;

				if(%hit.getdatablock().getName() $= "ZombieTankHoleBot")
				%hit.damage(%obj,posFromRaycast(%ray),15,$DamageType::SecondaryMelee);
				else %hit.damage(%obj,posFromRaycast(%ray),%dmg,$DamageType::SecondaryMelee);
			}

				if(%hit.hZombieL4BType & %hit.hZombieL4BType < 4 && %hit.lastdamage+1250 < getsimtime())
				{
					%hit.playaudio(0,"zombie_shoved" @ getrandom(1,10) @ "_sound");
					%hit.lastdamage = getsimtime();
				}

				%hit.applyimpulse(posFromRaycast(%ray),vectoradd(vectorscale(%obj.getforwardvector(),%hitknockback),"0" SPC "0" SPC %hitz));
		}
	}
}