function flamerRubbishProjectile::onCollision(%db,%proj,%hit,%fade,%pos,%normal)
{
	flamerProjectileExplode(%proj);
}
function flamerProjectileExplode(%proj)
{
	%mask = $TypeMasks::fxBrickObjectType | $TypeMasks::StaticObjectType | $TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType;
	if(!isObject($flamerFireSet)) $flamerFireSet = new simSet();
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
			if(%pass) %ready = 1;
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
		if(getWord(%ray,0)) %pos = getWords(%ray,1,3);
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
	if(!isEventPending($flamerFireGlobalLoop)) flamerExplosionLoop();
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
	serverPlay3D("molotov_throw_sound",%obj.getPosition());

	%obj.playthread(2, spearThrow);
}

function molotovProjectile::onExplode(%db,%proj,%expos,%b)
{
	parent::onExplode(%db,%proj,%expos,%b);
	molotov_explode(%expos,%proj,%proj.client);
}
function molotovProjectile::damage(%this,%obj,%col,%fade,%pos,%normal)
{
	if(!%col.getDatablock().noBurning) Parent::damage(%this,%obj,%col,%fade,%pos,%normal);	
}
function molotov_explode(%pos,%obj,%cl)
{
	createFireCircle(%pos,10,30,%cl,%obj,$DamageType::Molotov);
	serverPlay3D("molotov_explode_sound",%pos);
}

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
	if(%pl.getDatablock().noBurning) return;
	
	if(!%pl.isBurning)
	{
		%pl.setTempColor("0.1 0.1 0.1 1");
		%pl.flamer_burn(%tick);
		%pl.flamerBurnTickAdd = 0;
	}
	else %pl.flamerBurnTickAdd = %tick;
}
function player::flamer_burn(%pl,%tick)
{
	if(%pl.getDataBlock().hType $= "Zombie")
	{
		cancel(%pl.flamerClearBurnSched);
		%pl.isBurning = 1; 

		cancel(%pl.burnSched);
		if(!isObject(%pl.getMountedImage(3)))
		%pl.mountImage(flamerFleshBurningImage,3);
		
		if(!%pl.isPlayingBurningSound)
		{
			%pl.playAudio(3,"fire_fleshLoop_sound");
			%pl.isPlayingBurningSound = 1;
		}
		
		%dmg = mClamp(%pl.getdataBlock().maxDamage/25,10,%pl.getdataBlock().maxDamage);
		if(%pl.isCrouched())
		%dmg *= 0.47619;
		if(!%pl.noFireBurning)
		{
			%pl.damage(%pl.lastFireAttacker,%pl.getPosition(),%dmg,%pl.lastBurnDmgType);
			if(%pl.getclassname() $= "AIPlayer" && %pl.hZombieL4BType $= "Normal")
			{
				%pl.hRunAwayFromPlayer(%pl);
				%pl.stopHoleLoop();
					
				%pl.MaxSpazzClick = getrandom(16,32);
				L4B_SpazzZombie(%pl,0);
			}
			%pl.playThread(2,plant);
		}
	
		%pl.burnSched = %pl.schedule(500,flamer_burn,%tick);
	}
	else
	{
		cancel(%pl.flamerClearBurnSched);
		%pl.isBurning = 1;
		if(%pl.flamerBurnTickAdd > 0 && %pl.flamerBurnTickAdd > %tick)
		{
			%tick = mFloor(%pl.flamerBurnTickAdd);
			%pl.flamerBurnTickAdd = 0;
		}
		cancel(%pl.burnSched);
		if(!isObject(%pl.getMountedImage(3))) %pl.mountImage(flamerFleshBurningImage,3);

		if(!%pl.isPlayingBurningSound)
		{
			%pl.playAudio(3,"fire_fleshLoop_sound");
			%pl.isPlayingBurningSound = 1;
		}

		%dmg = mClamp(%tick,3,6)*1.75;
		if(%pl.isCrouched()) %dmg *= 0.47619;
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

			if(%pl.getDamagePercent() < 1) %pl.flamerClearBurnSched = %pl.schedule(1500,flamer_clearBurn);
			return;
		}
		%pl.burnSched = %pl.schedule(500,flamer_burn,%tick--);
	}
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
			if(%tailClear == 1) $flameSetCnt--;
			continue;
		}
		if(%set.getCount() == 0)
		{
			if(isObject(%set.soundObj)) %set.soundObj.delete();
			%set.delete();
			$flameSet[%i] = "";
			if(%tailClear == 1) $flameSetCnt--;
			
		}
		else %tailClear = 0;
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
	return;
	if(isObject(%set.soundObj)) %set.soundObj.setTransform(%set.median);
	else
	{
		%soundObj = %set.soundObj = new aiPlayer()
		{
			datablock = emptyPlayer;
			position = %set.median;
			source = %set;
		};
		%soundObj.setDamageLevel(100);
		%soundObj.playAudio(0,"fire_loop" @ getRandom(1,2) @ "_sound");
	}
}
function flameSetDestroy(%set)
{
	if(isObject(%set.soundObj)) %set.soundObj.delete();
	if(isObject(%set.debugObj)) %set.debugObj.delete();
	serverPlay3D("fire_end_sound",%set.median);
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
		if(%pl.getDatablock().isEmptyPlayer) return;
		if(%pl.isBurning) if(isObject(%im)) if(%im.getClassName() !$= "ProjectileData") return;			
	
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