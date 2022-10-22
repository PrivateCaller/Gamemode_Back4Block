exec("./datablocks.cs"); 
exec("./support_bricktext.cs"); 
exec("./ammo_crate/brick_ammocrate.cs");
exec("./health_locker/brick_healthlocker.cs");

package L4B_InteractiveBricks
{
	function fxDTSBrickData::onColorChange (%data, %brick)
	{
		if(isObject(%brick.healthlocker)) %brick.healthlocker.setnodecolor("ALL",getwords(getColorIdTable(%brick.colorid),0,2) SPC "1");
		if(isObject(%brick.ammocrate)) %brick.ammocrate.setnodecolor("ALL",getwords(getColorIdTable(%brick.colorid),0,2) SPC "1");

		Parent::onColorChange (%data, %brick);
	}
};
activatepackage(L4B_InteractiveBricks);