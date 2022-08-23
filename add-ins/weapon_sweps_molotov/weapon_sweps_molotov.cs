AddDamageType("Molotov",'<bitmap:Add-Ons/Weapon_SWeps_Ext/model/icon/ci_molotov> %1','%2 <bitmap:Add-Ons/Weapon_SWeps_Ext/model/icon/ci_molotov> %1',1,1);
AddDamageType("Flamer",'<bitmap:Add-Ons/Weapon_SWeps_Ext/model/icon/ci_fire> %1','%2 <bitmap:Add-Ons/Weapon_SWeps_Ext/model/icon/ci_fire> %1',1,1);

exec("./molotov.cs");

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
};

function createFireCircle(%pos,%rad,%amt,%cl,%obj,%damageType)
{
	for(%i=0;%i<%amt;%i++)
	{
		%rnd = getRandom();
		%dist = getRandom()*%rad;
		%x = mCos(%rnd*$PI*2)*%dist;
		%y = mSin(%rnd*$PI*2)*%dist;
		%p = new projectile()
		{
			datablock = flamerRubbishProjectile;
			initialPosition = %pos;
			initialVelocity = %x SPC %y SPC (getRandom()*4);
			client = %cl;
			sourceObject = %obj;
			damageType = %damageType;
		};
	}
}
function player::flamer_burnStart(%pl,%tick)
{
	//if(%pl.getDamagePercent() >= 1)
	//	return;
	if(!%pl.isBurning)
	{
		%pl.setTempColor("0.1 0.1 0.1 1");
		%pl.flamer_burn(%tick);
		%pl.flamerBurnTickAdd = 0;
	}
	else
	{
		%pl.flamerBurnTickAdd = %tick;
	}
}
function player::flamer_burn(%pl,%tick)
{
	cancel(%pl.flamerClearBurnSched);
	%pl.isBurning = 1;
	if(%pl.flamerBurnTickAdd > 0 && %pl.flamerBurnTickAdd > %tick)
	{
		%tick = mFloor(%pl.flamerBurnTickAdd);
		%pl.flamerBurnTickAdd = 0;
	}
	cancel(%pl.burnSched);
	if(!isObject(%pl.getMountedImage(3)))
	{
		%pl.mountImage(flamerFleshBurningImage,3);
	}
	if(!%pl.isPlayingBurningSound)
	{
		%pl.playAudio(3,fleshFireLoopSound);
		%pl.isPlayingBurningSound = 1;
	}
	
	%dmg = mClamp(%tick,3,6)*1.75;
	if(%pl.isCrouched())
		%dmg *= 0.47619;
	if(%pl.getDamagePercent() < 1)
	{
		%pl.damage(%pl.lastFireAttacker,%pl.getPosition(),%dmg,%pl.lastBurnDmgType);
		%pl.playThread(2,plant);
	}
	
	if(%tick <= 0)
	{
		%pl.unMountImage(3);
		%pl.isBurning = 0;
		%pl.playAudio(3,napalmFireEndSound);
		%pl.isPlayingBurningSound = 0;
		if(%pl.getDamagePercent() < 1)
		{
			%pl.flamerClearBurnSched = %pl.schedule(1500,flamer_clearBurn);
		}
		return;
	}
	%pl.burnSched = %pl.schedule(500,flamer_burn,%tick--);
}
function player::flamer_clearBurn(%pl)
{
	cancel(%pl.flamerClearBurnSched);
	%p = new projectile()
	{
		initialPosition = vectorAdd(%pl.getPosition(),"0 0 1");
		datablock = PlayerSootProjectile;
		scale = "0.7 0.7 0.7";
	};
	%p.explode();
	%pl.clearTempColor();
}


function cleanFlameSet()
{
	%tailClear = 1;
	for(%i=$flameSetCnt-1;%i>=0;%i--)
	{
		%set = $flameSet[%i];
		if(%set $= "")
		{
			if(%tailClear == 1)
			{
				$flameSetCnt--;
			}
			continue;
		}
		if(%set.getCount() == 0)
		{
			if(isObject(%set.soundObj))
				%set.soundObj.delete();
			%set.delete();
			$flameSet[%i] = "";
			if(%tailClear == 1)
			{
				$flameSetCnt--;
			}
		}
		else
		{
			%tailClear = 0;
		}
	}
}
function getFlameSet(%exp)
{
	%pos = %exp.position;
	for(%i=0;%i<$flameSetCnt;%i++)
	{
		if($flameSet[%i] $= "")
			continue;
		%med = $flameSet[%i].median;
		if(vectorDist(%pos,%med) < 6)
			return $flameSet[%i];
	}
	$flameSet[%i = $flameSetCnt] = new simSet();
	$flameSetCnt++;
	return $flameSet[%i];
}
function flameSetAdd(%set,%exp)
{
	%set.add(%exp);
	flameSetGenerateMedian(%set);
	flameSetPlayAudio(%set);
	%exp.set = %set;
}
function flameSetGenerateMedian(%set)
{
	%cnt = %set.getCount();
	%xAv = 0;
	%yAv = 0;
	%zAv = 0;
	for(%i=0;%i<%cnt;%i++)
	{
		%pos = %set.getObject(%i).position;
		%x = getWord(%pos,0);
		%y = getWord(%pos,1);
		%z = getWord(%pos,2);
		%xAv += %x;
		%yAv += %y;
		%zAv += %z;
	}
	%av = (%xAv/%cnt) SPC (%yAv/%cnt) SPC (%zAv/%cnt);
	%set.median = %av;
}
function flameSetPlayAudio(%set)
{
	if(isObject(%set.soundObj))
	{
		%set.soundObj.setTransform(%set.median);
	}
	else
	{
		%soundObj = %set.soundObj = new aiPlayer()
		{
			datablock = emptyPlayer;
			position = %set.median;
			audioBot = 1;
		};
		%soundObj.setDamageLevel(100);
		%soundObj.playAudio(0,napalmFireLoop @ getRandom(1,2) @ Sound);
	}
}
function flameSetDestroy(%set)
{
	if(isObject(%set.soundObj))
		%set.soundObj.delete();
	if(isObject(%set.debugObj))
		%set.debugObj.delete();
	serverPlay3D(napalmFireEndSound,%set.median);
	cleanFlameSet();
}
function flameSetFlamePop(%set)
{
	if(%set.getCount() == 0)
	{
		flameSetDestroy(%set);
		return 0;
	}
	else
	{
		flameSetGenerateMedian(%set);
		flameSetPlayAudio(%set);
		return 1;
	}
}

package swol_sweps_extpack
{
	function player::emote(%pl,%im,%override)
	{
		if(%pl.getDatablock().isEmptyPlayer)
			return;

		if(%pl.isBurning)
		{
			if(isObject(%im))
			{
				if(%im.getClassName() !$= "ProjectileData")
				{
					return;
				}
			}
		}
		parent::emote(%pl,%im,%override);
	}

	function armor::onDisabled(%db,%pl,%bool)
	{
		if(%db.isEmptyPlayer)
			return;
		return parent::onDisabled(%db,%pl,%bool);
	}
	function armor::onMount(%db,%pl,%obj,%node)
	{
		if(%db.isEmptyPlayer)
			return;
		return parent::onMount(%db,%pl,%obj,%node);
	}
};
activatePackage(swol_sweps_extpack);