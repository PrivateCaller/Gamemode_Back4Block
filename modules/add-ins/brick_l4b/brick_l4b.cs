exec("./ammo_crate/brick_ammocrate.cs");
exec("./support_bricktext.cs"); 
exec("./health_locker/brick_healthlocker.cs");

if(!isObject(AmmoLockerOpenSound))
{
	datablock AudioProfile(AmmoLockerOpenSound)
	{
		filename = "./lockeropen.wav";
		description = AudioClosest3d;
		preload = true;
	};
}

if(!isObject(AmmoLockerCloseSound))
{
	datablock AudioProfile(AmmoLockerCloseSound)
	{
		filename = "./lockerclose.wav";
		description = AudioClosest3d;
		preload = true;
	};
}

if(!isObject(AmmoCrateSound))
{
	datablock AudioProfile(AmmoCrateSound)
	{
		filename = "./ammocratesound.wav";
		description = AudioClosest3d;
		preload = true;
	};
}

package L4B_InteractiveBricks
{
	function fxDTSBrickData::onColorChange (%data, %brick)
	{
		if(isObject(%brick.healthlocker)) %brick.healthlocker.setnodecolor("ALL",getwords(getColorIdTable(%brick.colorid),0,2) SPC "1");
		if(isObject(%brick.ammocrate)) %brick.ammocrate.setnodecolor("ALL",getwords(getColorIdTable(%brick.colorid),0,2) SPC "1");

		Parent::onColorChange (%data, %brick);
	}

    function MiniGameSO::Reset(%minigame,%client)
	{
		Parent::Reset(%minigame,%client);

		if(isObject(LockerCrateSet))
		{
			for (%i = 0; %i < LockerCrateSet.getCount(); %i++) {
				
				if(isObject(%obj = LockerCrateSet.getObject(%i)))
				%obj.AmmoSupplies = %obj.AmmoSupplyMax;
				AmmoShapeName(%obj,"");
			}
		}
	}

};
activatepackage(L4B_InteractiveBricks);