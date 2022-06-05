package L4B_SWepFlamesBurnsZombs
{
	function player::flamer_burnStart(%pl,%tick)
	{
		if(!%pl.getDatablock().noBurning)
		Parent::flamer_burnStart(%pl,%tick);
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
				%pl.playAudio(3,fleshFireLoopSound);
				%pl.isPlayingBurningSound = 1;
			}
			
			%dmg = mClamp(%pl.getdataBlock().maxDamage/25,10,%pl.getdataBlock().maxDamage);
			if(%pl.isCrouched())
			%dmg *= 0.47619;
			if(!%pl.noFireBurning)
			{
				%pl.damage(%pl.lastFireAttacker,%pl.getPosition(),%dmg,%pl.lastBurnDmgType);
				if(%pl.getclassname() $= "AIPlayer" && %pl.hZombieL4BType && %pl.hZombieL4BType < 5)
				{
					%pl.hRunAwayFromPlayer(%pl);
					%pl.stopHoleLoop();
				}
				%pl.playThread(2,plant);
			}
		
			%pl.burnSched = %pl.schedule(500,flamer_burn,%tick);
		}
		else
		Parent::flamer_burn(%pl,%tick);
	}

	function molotov_explode(%pos,%obj,%cl)
	{
		Parent::molotov_explode(%pos,%obj,%cl);

		//for (%n = 0; %n < 2; %n++)
		//schedule(3500 * %n, 0, createFireCircle, %pos,30,40,%cl,%obj,$DamageType::Molotov);
	}

	function flamerProjectile::damage(%this,%obj,%col,%fade,%pos,%normal)
	{
		if(!%col.getDatablock().noBurning)
		Parent::damage(%this,%obj,%col,%fade,%pos,%normal);
	}

	function molotovProjectile::damage(%this,%obj,%col,%fade,%pos,%normal)
	{
		if(!%col.getDatablock().noBurning)
		Parent::damage(%this,%obj,%col,%fade,%pos,%normal);	
	}
};
activatePackage(L4B_SWepFlamesBurnsZombs);