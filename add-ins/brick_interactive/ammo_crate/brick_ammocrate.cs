datablock ItemData(RAmmoCrateItem)
{
	category = "Item";  // Mission editor category
	//className = "Item"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./RotCoAmmoCrate2.dts";
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
	isAmmoCrate = 1;

	 // Dynamic properties defined by the scripts
	image = "";
	canDrop = true;
};
datablock fxDTSBrickData (BrickAmmoCrateData)
{
	brickFile = "./AmmoCrateBrick.blb";
	category = "Special";
	subCategory = "Interactive";
	uiName = "Ammo Crate";
	iconName = "add-ons/gamemode_left4block/add-ins/brick_interactive/ammo_crate/icon_ammocrate";
	indestructable = 1;
	isAmmoCrate = 1;
};

function BrickAmmoCrateData::onPlant(%this, %obj)
{
	%obj.setName("_Ammocrate");
	
	%ammocrate = new Item()
	{
		datablock = "RAmmoCrateItem";
		static = 1;
		spawnbrick = %obj;
		canPickup = false;
		AmmoSupplies = $Pref::L4BAmmoCrate::SupplyAmount;
		AmmoSupplyMax = $Pref::L4BAmmoCrate::SupplyAmount;
	};

	%obj.setrendering(0);
	%ammocrate.settransform(vectoradd(%obj.gettransform(),"0 0 -0.4") SPC getwords(%obj.gettransform(),3,6));
	%obj.ammocrate = %ammocrate;
}

function RAmmoCrateItem::onAdd(%this, %obj)
{
	Parent::onAdd(%this, %obj);

	if(!isObject(LockerCrateSet))
	{
		new simset(LockerCrateSet);
		missionCleanup.add(LockerCrateSet);
		LockerCrateSet.add(%obj);
	}
	else if(!LockerCrateSet.isMember(%obj))
	LockerCrateSet.add(%obj);

	%obj.schedule(5000,setnodecolor,"ALL",getwords(getColorIdTable(%obj.spawnBrick.colorid),0,2) SPC "1");
}

function BrickAmmoCrateData::onloadPlant(%this, %obj)
{
	BrickAmmoCrateData::onPlant(%this, %obj);
}

function BrickAmmoCrateData::onRemove(%data, %brick)
{
	if(isObject(%brick.ammocrate))
	%brick.ammocrate.delete();

	Parent::OnRemove(%data,%brick);
}

function BrickAmmoCrateData::onDeath(%data, %brick)
{
   BrickAmmoCrateData::onRemove(%data, %brick);
}

function AmmoCrateAnim(%obj,%oc)
{
	if(!isObject(%obj))
	return;
	
	if(%oc)
	{
		%obj.playaudio(2,AmmoLockerOpenSound);
		%obj.playthread(1,open);
		%obj.isopen = 1;
	}
	else
	{
		%obj.playthread(1,close);
		cancel(%obj.slowd);
		%obj.isopen = 0;
		%obj.isshutting = 1;
		%obj.slowd = schedule(500,0,AmmoEvalcrashes,%obj);
		%obj.playaudio(2,AmmoLockerCloseSound);
	}
}

function toggleCloseOpen(%col)
{
	if(!%col.isopen)
	AmmoCrateAnim(%col,1);

	cancel(%col.closedoor);
	%col.closedoor = schedule(2000,0,AmmoCrateAnim,%col,0);
}

function AmmoEvalCrashes(%obj)
{
	%obj.isshutting= 0;
	if(isObject(%obj.spawnBrick.textShape))
	%obj.spawnbrick.textshape.delete();
}

function GiveAmmo(%obj,%col)
{
	%image = %obj.getmountedimage(0);
	%item = %image.item;
	%ammoType = %item.AEType;
	%ammo = %item.AEAmmo;
	
	%max = %item.AEType.aeMax;
	%ammoused = %ammo-%obj.aeAmmo[%obj.currTool, %image.AEUseAmmo, 0];

	toggleCloseOpen(%col);
	%obj.playthread(3, leftrecoil);
	serverPlay3D(ammocratesound,%obj.getPosition());
	serverPlay3D(pickupAmmo @ getRandom(1,2) @ Sound,%col.getPosition());

	%obj.AEReserve[%ammoType] += %ammo*3;
  	%obj.AEReserve[%ammoType] = mClamp(%obj.AEReserve[%ammoType], 0, %max + %ammoused);
	%obj.baadDisplayAmmo(%image);
	%obj.AENotifyAmmo(1);
}

function AmmoShapeName(%brick,%shapename)
{
	if(isObject(%brick.textShape))
	{
		%brick.textShape.setShapeName(%shapename);
		%brick.textShape.setShapeNameColor("1 1 1 1");        
		%brick.textShape.setShapeNameDistance(25);
	}
	else
	{
		%brick.textShape = new StaticShape()
		{
			datablock = BrickTextEmptyShape;
			position = vectorAdd(%brick.getPosition(),"0 0" SPC %brick.getDatablock().brickSizeZ/9 + "0.166");
			scale = "0.1 0.1 0.1";
		};

		if(isObject(%brick.textShape))
		{
			%brick.textShape.setShapeName(%shapename);
			%brick.textShape.setShapeNameColor("1 1 1 1");        
			%brick.textShape.setShapeNameDistance(25);
		}
	}
}

package CrateColFunctions
{
	function Armor::onCollision(%this,%obj,%col)
	{	
		if(isObject(%col))
		if(%col.getdatablock().getname() $= "RAmmoCrateItem")
		{
			if(%obj.getstate() $= "Dead" || %obj.hIsInfected)
			return;	

			%image = %obj.getmountedimage(0);
			%item = %image.item;
			%ammoType = %item.AEType;
			%ammo = %item.AEAmmo;
			%max = %item.AEType.aeMax;

			%ammoused = %ammo-%obj.aeAmmo[%obj.currTool, %image.AEUseAmmo, 0];
			%ammoneeded = %obj.AEReserve[%ammoType]-%ammoused;
			%ammocompensate = mFloatLength(%max/%ammoneeded * 0.5, 0);

			%brick = %col.spawnbrick;
			%obj.lasttouch = getsimtime();

			if(%col.lasttouch+$Pref::L4BAmmoCrate::AcquireDelay < getsimtime() && !%col.isshutting)
			{
				%col.lasttouch = getsimtime();

				if(%ammoneeded < %max)
				{
					if($Pref::L4BAmmoCrate::Supplies) //If supplies is enabled
					{
						if(isObject(getMiniGamefromObject(%obj,%col)))
						{
							if(%col.AmmoSupplies > 0)//Functions if supplycount is greater than or equal to supply amount (Empty)
							{
								GiveAmmo(%obj,%col);

								%col.AmmoSupplies -= %ammocompensate;
								%col.AmmoSupplies = mClampF(%col.AmmoSupplies, 0, $Pref::L4BAmmoCrate::SupplyAmount);

								AmmoShapeName(%brick,"Supplies:" SPC mFloatLength(%col.AmmoSupplies,0));
								%col.unhideNode(ammobox);
							}
							else 
							{
								AmmoShapeName(%brick,"Supplies:" SPC mFloatLength(%col.AmmoSupplies,0));
								%col.hideNode(ammobox);
							}
						}
						else 
						{
							GiveAmmo(%obj,%col);
							%col.unhideNode(ammobox);
						}
					}
					else 
					{
						GiveAmmo(%obj,%col);
						AmmoShapeName(%brick,"");
						%col.unhideNode(ammobox);
					}
				}
				return;
			}
	   	}
		else return Parent::onCollision(%this,%obj,%col);
	}
};
activatepackage(CrateColFunctions);