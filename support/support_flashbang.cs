package L4B_FlashGrenadeSupport
{
	function flashGrenadeProjectile::onExplode(%this,%obj)
	{
	   parent::onExplode(%this, %obj);
	   initContainerRadiusSearch(%obj.getPosition(),30,$TypeMasks::PlayerObjectType);
	   while((%target = ContainerSearchNext()) != 0)
	   {
	      if(!isObstruction(%obj.getPosition(),%target) && isObject(getMinigameFromObject(%obj,%target)))
	      {
				%angle = calculateAngle(%target,%obj.getPosition());
				if(%angle < 100 && %angle > -100 || %angle > -360 && %angle < -260 || %angle < 360 && %angle > 260)
				{
					if(%angle < 0)
					%angle = mAbs(%angle);

					if(%angle > 180)
					%angle = %angle/6;

					if(%angle < 10)
					%start = 2;
					else
					%start = mFloatLength(1/%angle*25,4);

					if(%start < 0.2)
					%start = 0.2;
					%target.setWhiteout(%start);
					%sched = %start*1000;

					setTime(%target.client,0.2);
					%target.timeSched = schedule(%sched,0,"setTime",%target.client,1);
				}
			}

			if(%target.isHoleBot && %target.hZombieL4BType && %target.hZombieL4BType < 5)
			{
				%target.stopHoleLoop();
				L4B_SpazzZombieInitialize(%target,0);
			}
		
		}
	}
};

activatePackage(L4B_FlashGrenadeSupport);
eval("flashgrenadeProjectile.lifetime = 3500;");
eval("flashgrenadeProjectile.fadeDelay = 4000;");
eval("flashgrenadeProjectile.armingDelay = 3500;");
eval("flashgrenadeProjectile.bounceElasticity = 0.25;");
eval("flashgrenadeProjectile.isDistraction = 1;");
eval("flashgrenadeProjectile.distractionLifetime = 3;");
eval("flashgrenadeProjectile.distractionDelay = 0;");
eval("flashgrenadeProjectile.DistractionFunction = BileBombDistract;");
eval("flashgrenadeProjectile.DistractionRadius = 50;");