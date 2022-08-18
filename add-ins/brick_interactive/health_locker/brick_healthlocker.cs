datablock ItemData(RHealthLockerItem)
{
	category = "Item";  // Mission editor category
	//className = "Item"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./HealthLocker15.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "";
	iconName = "";
	doColorShift = false;

	 // Dynamic properties defined by the scripts
	image = "";
	canDrop = true;
};
datablock fxDTSBrickData (BrickLockerData)
{
	brickFile = "./LockerBrick.blb";
	category = "Special";
	subCategory = "Interactive";
	uiName = "Health Locker";
	iconName = "add-ons/gamemode_left4block/add-ins/brick_interactive/health_locker/Icon_HealthLocker";
	indestructable = 1;
};

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
		canPickup = false;
		AmmoSupplies = $Pref::L4BHealthLocker::SupplyAmount;
		AmmoSupplyMax = $Pref::L4BHealthLocker::SupplyAmount;
	};

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
	function Armor::onCollision(%this,%obj,%col,%a,%b,%c,%d)
	{
		if(%col.getdatablock().getname() $= "RHealthLockerItem")
		{
			if(%obj.getdamagelevel() < 5 || %obj.getstate() $= "Dead" || %obj.hIsInfected)
			return;	

			%brick = %col.spawnbrick;

			if(%col.lasttouch+$Pref::L4BHealthLocker::AcquireDelay < getsimtime() && !%col.isshutting)
			{
				%col.lasttouch = getsimtime();

				if($Pref::L4BHealthLocker::Supplies)//Functions if supplycount equals supply amount
				{
					if(isObject(getMiniGamefromObject(%obj,%col)))
					{
						if(%col.AmmoSupplies > 0)
						{
							GiveHealth(%obj,%col);

							%col.AmmoSupplies -= mFloatLength(%obj.getdamagepercent()*2,2);
							%col.AmmoSupplies = mClampF(%col.AmmoSupplies, 0, $Pref::L4BHealthLocker::SupplyAmount);

							AmmoShapeName(%brick,"Supplies:" SPC mFloatLength(%col.AmmoSupplies,2));
						}
						else
						AmmoShapeName(%brick,"Supplies:" SPC mFloatLength(%col.AmmoSupplies,2));
					}
					else GiveHealth(%obj,%col);
        		}
				else
				{
					GiveHealth(%obj,%col);
					AmmoShapeName(%brick,"");
				}

			}		
			return;
		}
		else return Parent::onCollision(%this,%obj,%col,%a,%b,%c,%d);
	}

};
activatePackage(HealthLockerColFunctions);