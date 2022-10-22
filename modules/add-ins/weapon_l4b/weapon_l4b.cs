package L4B_Projectiles
{	
	function Projectile::onAdd(%obj)
	{
		if(%obj.getdataBlock().isDistraction) %obj.schedule(%obj.getDataBlock().distractionDelay,%obj.getDataBlock().distractionFunction,0);

		Parent::onAdd(%obj,%datablock);
	}

	function ProjectileData::onCollision (%this, %obj, %col, %fade, %pos, %normal, %velocity)
	{
		if(%this.directDamage && %col.getType() & $TypeMasks::PlayerObjectType)
		{
			if(!%obj.sourceObject.hIsInfected && %col.isBeingStrangled && %col.hEater.getDataBlock().getName() $= "ZombieSmokerHoleBot")
			{
				%col.hEater.isBeingStrangled = false;
				%obj.isStrangling = false;
				L4B_SpecialsPinCheck(%col.hEater,%col);
				%col.hEater.damage(%obj, %pos, %this.directDamage/2, %this.directDamageType);
			}
		}		

		Parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
	}
};
activatePackage(L4B_Projectiles);

exec("./datablocks.cs");
exec("./weapon_melee.cs");
exec("./weapon_boulder.cs");
exec("./weapon_distractions.cs");
exec("./weapon_molotov.cs");
exec("./weapon_throwable_explosives.cs");