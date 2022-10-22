function BrickAmmoCrateData::onPlant(%this, %obj)
{
	%obj.setName("_Ammocrate");
	
	%ammocrate = new Item()
	{
		datablock = "RAmmoCrateItem";
		spawnbrick = %obj;
		color = getColorIdTable(%obj.colorid);
		canPickup = false;
		position = %obj.getPosition();
	};
	%obj.setrendering(0);
	%obj.ammocrate = %ammocrate;
	%ammocrate.settransform(vectoradd(%obj.gettransform(),"0 0 -0.4") SPC getwords(%obj.gettransform(),3,6));
}

function RAmmoCrateItem::onAdd(%this, %obj)
{
	Parent::onAdd(%this, %obj);
	%obj.schedule(100,setnodecolor,"ALL",%obj.color);
}

function BrickAmmoCrateData::onloadPlant(%this, %obj) { BrickAmmoCrateData::onPlant(%this, %obj); }

function BrickAmmoCrateData::onRemove(%data, %brick)
{
	if(isObject(%brick.ammocrate)) %brick.ammocrate.delete();

	Parent::OnRemove(%data,%brick);
}

function BrickAmmoCrateData::onDeath(%data, %brick)
{
   BrickAmmoCrateData::onRemove(%data, %brick);
}

function AmmoCrateAnim(%obj,%oc)
{
	if(!isObject(%obj)) return;
	
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
		if(isObject(%col) && %col.getdatablock().getname() $= "RAmmoCrateItem")
		{
			if(%obj.getstate() $= "Dead" || %obj.hIsInfected) return;	

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

			if(%col.lasttouch+50 < getsimtime() && !%col.isshutting)
			{
				%col.lasttouch = getsimtime();

				if(%ammoneeded < %max)
				{
					GiveAmmo(%obj,%col);
					AmmoShapeName(%brick,"");
					%col.unhideNode(ammobox);
				}
				return;
			}
	   	}
		else return Parent::onCollision(%this,%obj,%col);
	}
};
activatepackage(CrateColFunctions);