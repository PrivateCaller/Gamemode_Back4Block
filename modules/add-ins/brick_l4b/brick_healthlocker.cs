function RHealthLockerItem::onAdd(%this, %obj)
{
	Parent::onAdd(%this, %obj);
	%obj.schedule(5000,setnodecolor,"ALL",getwords(getColorIdTable(%obj.spawnBrick.colorid),0,2) SPC "1");

	if(!isObject(LockerCrateSet))
	{
		new simset(LockerCrateSet);
		missionCleanup.add(LockerCrateSet);
		LockerCrateSet.add(%obj);
	}
	else if(!LockerCrateSet.isMember(%obj))
	LockerCrateSet.add(%obj);
}

function BrickLockerData::onPlant(%this, %obj)
{
  	%obj.setName("_Healthlocker");
	
	%healthlocker = new Item()
	{
		datablock = "RHealthLockerItem";
		static = 1;
		spawnbrick = %obj;		
	};

	%healthlocker.canPickup = false;
	%healthlocker.settransform(vectoradd(%obj.gettransform(),"0 0 -1.42") SPC getwords(%obj.gettransform(),3,6));
  	%obj.healthlocker = %healthlocker;
	%obj.setrendering(0);
}

function BrickLockerData::onloadPlant(%this, %obj)
{
	BrickLockerData::OnPlant(%this, %obj);
}

function BrickLockerData::onRemove(%this, %obj)
{
	if(isObject(%obj.healthlocker))
	{
		%obj.healthlocker.delete();
		cancel(%obj.healthlocker.HealthlockerAnim);
		cancel(%obj.healthlocker.slowd);
	}
	parent::OnRemove(%this,%obj);
}

function BrickLockerData::onDeath(%this, %obj)
{
   if(isObject(%obj.healthlocker))
    %obj.healthlocker.delete();

    parent::OnDeath(%this,%obj);
}

function GiveHealth(%obj,%col)
{
	%obj.addhealth(%obj.getDatablock().maxDamage/5);
	%obj.playthread(3, plant);
	%obj.emote(HealImage, 3);
	serverPlay3D(PrintFireSound,%obj.getPosition());
	toggleCloseOpen(%col);
}

package HealthLockerColFunctions
{
	function Armor::onCollision(%this,%obj,%col)
	{
		if(isObject(%col))
		if(%col.getdatablock().getname() $= "RHealthLockerItem")
		{
			if(%obj.getdamagelevel() < 5 || %obj.getstate() $= "Dead" || %obj.hIsInfected) return;	

			%brick = %col.spawnbrick;
			if(%col.lasttouch+250 < getsimtime() && !%col.isshutting)
			{
				%col.lasttouch = getsimtime();
				GiveHealth(%obj,%col);
				AmmoShapeName(%brick,"");
			}		
			return;
		}
		else return Parent::onCollision(%this,%obj,%col,%a,%b,%c,%d);
	}

};
activatePackage(HealthLockerColFunctions);