exec("./datablocks.cs"); 
exec("./brick_ammocrate.cs");
exec("./brick_healthlocker.cs");
exec("./brick_elevator.cs");

function InteractiveBrickAnim(%obj,%oc)
{
	if(!isObject(%obj)) return;
	
	if(%oc)
	{
		if(!%obj.isopen)
		{
			%obj.playaudio(2,"lockeropen_sound");
			%obj.playthread(1,"open");
			%obj.isopen = 1;
		}
	}
	else
	{
		%obj.playthread(1,"close");
		cancel(%obj.slowd);
		%obj.isopen = 0;
		%obj.isshutting = 1;
		%obj.slowd = schedule(500,0,%obj.isshutting = 0,%obj);
		%obj.playaudio(2,"lockerclose_sound");
	}
}

package L4B_InteractiveBricks
{
	function fxDTSBrickData::onColorChange (%data, %brick)
	{
		if(isObject(%brick.interactiveshape)) %brick.interactiveshape.setnodecolor("ALL",getwords(getColorIdTable(%brick.colorid),0,2) SPC "1");

		Parent::onColorChange (%data, %brick);
	}

	function Armor::onCollision(%this,%obj,%col)
	{
		Parent::onCollision(%this,%obj,%col,%a,%b,%c,%d);

		if(isObject(%col) && %col.getdatablock().isInteractiveShape) %col.getdatablock().CheckConditions(%col,%obj);
	}	
};
activatepackage(L4B_InteractiveBricks);