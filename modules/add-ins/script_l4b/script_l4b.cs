exec("./datablocks.cs");
exec("./script_areazones.cs");
exec("./script_director.cs");
exec("./script_clientlogger.cs");
exec("./script_rblood.cs");
exec("./script_footsteps.cs");
exec("./script_holstergun.cs");

function emptyPlayer::onAdd(%this, %obj) 
{
	%obj.setDamageLevel(%this.maxDamage);

	if(isObject(%source = %obj.source) && %obj.slotToMountBot)
	{
		%source.mountObject(%obj,%obj.slotToMountBot);
		
		if(%obj.imageToMount !$= "") 
		{
			if(%obj.imageColor !$= "") %obj.mountImage(%obj.imageToMount,0,1,%obj.imageColor);
			else %obj.mountImage(%obj.imageToMount,0);
		}
		if(%obj.lightToMount !$= "")
		{
			%billboard = new fxLight ("")
			{
				dataBlock = %obj.lightToMount;
				source = %source;
			};
			
			%obj.lightToMount = %billboard;
			MissionCleanup.add(%billboard);
			%billboard.setTransform(%obj.getTransform());
			%billboard.attachToObject(%obj);

			for(%i = 0; %i < clientgroup.getCount(); %i++) 			
			if(isObject(%client = clientgroup.getObject(%i)) && isObject(%client.player))
			%billboard.ScopeToClient(%client);
		}
	}
	else
	{
		%obj.delete();
		return;
	}
}

function emptyPlayer::onRemove(%this, %obj)
{
	if(isObject(%obj.lightToMount)) %obj.lightToMount.delete();
}
function emptyPlayer::doDismount(%this, %obj, %forced) 
{
	return;
}
function emptyPlayer::onDisabled(%this, %obj) 
{
	return;
}