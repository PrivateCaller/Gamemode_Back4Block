package L4B_FlareGunSupport
{
	function flareGunProjectile::onCollision(%db,%proj,%hit,%fade,%pos,%normal)
	{	
		if(%hit.hZombieL4BType)
		%hit.flamer_burnStart(4);

		%pos = %proj.getPosition();
		%radius = 1000;
	    %searchMasks = $TypeMasks::PlayerObjectType;
	    InitContainerRadiusSearch(%pos, %radius, %searchMasks);
	    while((%targetid = containerSearchNext()) != 0 )
	    {
			if(%targetid.getClassName() $= "AIPlayer" && %targetid.hZombieL4BType !$= 5 && !%targetid.isBurning)
			{
				%targetid.Distraction = 0;
				%targetid.hSearch = 1;
	    	}
	    }
	    cancel(%proj.ContinueSearch);
	
		parent::onCollision(%db,%proj,%hit,%fade,%pos,%normal);
	}

};

activatePackage(L4B_FlareGunSupport);
eval("flareGunProjectile.isDistraction = 1;");
eval("flareGunProjectile.distractionLifetime = 10;");
eval("flareGunProjectile.distractionDelay = 500;");
eval("flareGunProjectile.DistractionFunction = BileBombDistract;");
eval("flareGunProjectile.DistractionRadius = 1000;");