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
		if(!%targetid.getState() !$= "Dead" && %targetid.getClassName() $= "AIPlayer" && %targetid.hZombieL4BType $= "Normal" && !%targetid.isBurning)
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
		if(!%targetid.getState() !$= "Dead" && %targetid.getClassName() $= "AIPlayer" && %targetid.hZombieL4BType $= "Normal" && !%targetid.isBurning)
		{
			if(%count < %obj.getDatablock().distractionLifetime)
			{
				if(!%targetid.Distraction)
				{
					%targetid.Distraction = %obj.getID();
					%targetid.hSearch = 0;
					%targetid.hFollowing = 0;
				}
				else if(%targetid.Distraction == %obj.getID())
				{
					%targetid.setaimobject(%obj);
					%targetid.setmoveobject(%obj);
					%time1 = getRandom(1000,4000);
					%targetid.getDataBlock().schedule(%time1,onBotFollow,%targetid,%obj);
				}
			}
			else
			{
				%targetid.hSearch = 1;
				%targetid.Distraction = 0;
			}

			if(%obj.getdataBlock().getID() == BileBombFakeProjectile.getID() && ContainerSearchCurrRadiusDist() <= 4 && %targetid.hType $= "zombie")
			{
				%targetid.hType = "biled" @ getRandom(1,9999);
				%targetid.mountImage(BileStatusPlayerImage, 2);
				%targetid.BoomerBiled = 1;
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