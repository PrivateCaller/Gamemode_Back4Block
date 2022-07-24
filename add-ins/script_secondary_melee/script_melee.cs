%pattern = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/sound/*.wav";//Too lazy to write datablock files for the sounds, just took this from the Disease Gamemode
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/sound/", "");
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
	sizes[0]      = 2.0;
	sizes[1]      = 3.0;

	useInvAlpha = false;
};
datablock ParticleData(SecondaryMeleeFlashSmallParticle : SecondaryMeleeFlashParticle)
{
	sizes[0]      = 1.0;
	sizes[1]      = 2.0;
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

datablock ParticleEmitterData(SecondaryMeleeFlashSmallEmitter : SecondaryMeleeFlashEmitter)
{
	particles = SecondaryMeleeFlashSmallParticle;
};
datablock ExplosionData(SecondaryMeleeExplosion)
{
	soundProfile = "";
	lifeTimeMS = 150;

	particleEmitter = SecondaryMeleeFlashEmitter;
	particleDensity = 5;
	particleRadius = 0.2;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "5 5 5";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 1;
	camShakeRadius = 10.0;

	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0 0 0";
	lightEndColor = "0 0 0";
};
datablock ExplosionData(SecondaryMeleeSmallExplosion : SecondaryMeleeExplosion)
{
	camShakeFreq = "3 3 3";
	camShakeAmp = "0.6 0.6 0.6";
	particleEmitter = SecondaryMeleeFlashSmallEmitter;
};
datablock ProjectileData(SecondaryMeleeProjectile)
{
	lifeTime = 1;
	explodeOnDeath = true;
	explosion = SecondaryMeleeExplosion;
};
datablock ProjectileData(SecondaryMeleeSmallProjectile : SecondaryMeleeProjectile)
{
	explosion = SecondaryMeleeSmallExplosion;
};
AddDamageType("SecondaryMelee",'<bitmap:add-ons/Gamemode_Left4Block/add-ins/script_secondary_melee/icons/ci_punch> %1','%2 <bitmap:add-ons/Gamemode_Left4Block/add-ins/script_secondary_melee/icons/ci_punch> %1',0,1);

function serverCmdSecondaryMelee(%cl)
{
	%pl = %cl.player;
	if(!isObject(%pl) || %pl.meleeItem || %pl.hIsInfected || %pl.getDatablock().isdowned)
	return;

	if($Pref::SecondaryMelee::MeleeMode == 2 || ($Pref::SecondaryMelee::MeleeMode == 1 && isObject(%cl.minigame)))
	if(!%pl.disabledMelee)
	%pl.meleeTrigger();		
}
	
function player::meleeTrigger(%pl)
{
	if(%pl.getstate() $= "Dead" || %pl.meleeItem || %pl.hIsInfected || %pl.getDatablock().getName() $= "DownPlayerSurvivorArmor" || %pl.isBeingStrangled)
	if(%pl.disabledMelee)
	return;

	if(!isObject(%pl.getObjectMount()) && getSimTime()-%pl.lastMelee > 400-%pl.meleeCooldownRush && (%pl.meleeTick < $Pref::SecondaryMelee::MeleeCharges || $Pref::SecondaryMelee::MeleeCharges == -1))
	{
		%pl.lastMelee = getSimTime();
		%pl.meleeTick++;
		%hit = %pl.doMelee();
		%cd = (%hit == 0 ? 500 : 0);
		%pl.meleeCooldownRush = (%hit == 1 ? 50 : 0);
		if($Pref::SecondaryMelee::MeleeCharges != -1 && !isEventPending(%pl.meleeCdSched))
		%pl.meleeCdSched = %pl.schedule($Pref::SecondaryMelee::MeleeCooldownRate+%cd,meleeCooldown);
	}
}

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

     	if(vectorDist(%pos,%hit.getposition()) > 3.75 || %dot > -0.4)
		continue;

		if(isObject(%ray))
        if(%ray.getType() & $TypeMasks::StaticObjectType || %ray.getType() & $TypeMasks::FxBrickObjectType || %ray.getType() & $TypeMasks::VehicleObjectType)
     	{
			serverPlay3D(%MeleeType @ "HitEnv_Sound",posFromRaycast(%ray));
			%p = new projectile()
			{
				datablock = "SecondaryMeleeSmallProjectile";
				initialPosition = posFromRaycast(%ray);
			};
			return;
     	}

		%obscure = containerRayCast(%obj.getEyePoint(),vectorAdd(%hit.getPosition(),"0 0 1.9"),$TypeMasks::VehicleObjectType | $TypeMasks::StaticObjectType | $TypeMasks::FxBrickObjectType, %obj);
		if(isObject(%obscure))
		continue;

		if(%hit.getType() & $TypeMasks::PlayerObjectType)
		{
			if(L4B_CheckifinMinigame(%obj,%hit))
			{
				if(getSimTime()-%hit.punchedTime[%obj] > 2500)
				%hit.punched[%obj] = 0;

				%hit.punched[%obj]++;
				%hit.punchedTime[%obj] = getSimTime();
				if(%hit.punched[%obj] != 0 && %hit.punched[%obj]%3 == 0 && getSimTime()-%obj.lastSlap > 1500)
				{
					%slap = true;
					%obj.lastSlap = getSimTime();
				}

				if(%slap)//Do the good stuff
				{
					%stunchance = 75;
					%sound = "melee_slap_sound";
					%projectile = "SecondaryMeleeProjectile";
					%dmg = %hit.getdatablock().maxDamage/10;
					%hitknockback = 800;
					%hitz = 200;

					%obj.playThread(3,jump);
				}
				else
				{
					%stunchance = 20;
					%sound = "melee_hit" @ getrandom(1,8) @ "_sound";
					%projectile = "SecondaryMeleeSmallProjectile";
					%dmg = %hit.getdatablock().maxDamage/12;
					%hitknockback = 400;
					%hitz = 150;
				}
				if(%hit.getdatablock().getname() $= "ZombieTankHoleBot")
				%dmg = %hit.getdatablock().maxDamage/30; 

				if(getRandom(1,100) <= %stunchance && %hit.hZombieL4BType & %hit.hZombieL4BType < 4 )
				{
					if(%hit.isHoleBot)
					%hit.stopHoleLoop();
					
					%hit.emote(winStarProjectile, 1);
					L4B_SpazzZombieInitialize(%hit,1);
					%hit.mountImage(stunImage,2);
					schedule(1000,0,serverCmdSit,%hit);
				}


				serverPlay3D(%sound,%hit.getHackPosition());

				%hscale = %hit.getScale();
				%p = new projectile()
				{
					datablock = %projectile;
					initialPosition = %hit.getHackPosition();
				};

				%sVec = %hit.getForwardVector();
				%aimVec = %obj.getForwardVector();
				%reflect = (vectorDot(%sVec, %aimVec) < 0);

				if(%hit.getMountedImage(0) == RiotShieldimage.getID())
				{
					serverPlay3d("riotshield_hit_sound",%hit.getHackPosition());

					if(!%reflect)
					{
						%hit.applyimpulse(%hit.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),%hitknockback*$Pref::SecondaryMelee::MeleeForce),"0" SPC "0" SPC %hitz*$Pref::SecondaryMelee::MeleeForce));
						%hit.damage(%obj,%hit.getPosition(),%dmg,$DamageType::SecondaryMelee);
					}
					else
					{
						%hitknockback = 200;
						%hitz = 75;
						%hit.applyimpulse(%hit.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),%hitknockback*$Pref::SecondaryMelee::MeleeForce),"0" SPC "0" SPC %hitz*$Pref::SecondaryMelee::MeleeForce));
						serverPlay3d("riotshield_block_sound",%obj.getPosition());
					}
				}
				else
				{
					%hit.applyimpulse(%hit.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),%hitknockback*$Pref::SecondaryMelee::MeleeForce),"0" SPC "0" SPC %hitz*$Pref::SecondaryMelee::MeleeForce));
					%hit.damage(%obj,%hit.getPosition(),%dmg,$DamageType::SecondaryMelee);
				}
			}
		}
	}
}
	
function player::meleeCooldown(%pl)
{
	cancel(%pl.meleeCdSched);
	%pl.meleeTick--;
	if(!%pl.meleeTick)
		return;
	%pl.meleeCdSched = %pl.schedule(1000,meleeCooldown);
}