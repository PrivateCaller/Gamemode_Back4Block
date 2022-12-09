exec("./datablocks.cs");
exec("./script_areazones.cs");
exec("./script_director.cs");
exec("./script_clientlogger.cs");
exec("./script_rblood.cs");
exec("./script_footsteps.cs");
exec("./script_holstergun.cs");

function L4BHatModel::onAdd(%this, %obj) 
{
	if(!isObject(%wearer = %obj.wearer))
	{ 
		%obj.delete();
		return;
	}

	%obj.setDamageLevel(%this.maxDamage);
	%obj.hideNode("ALL");
	%obj.unhideNode(%obj.currentHat);
	%obj.setNodeColor(%obj.currentHat,%obj.color);	
	%obj.setTransform(%wearer.getTransform());
	%obj.position = vectorAdd(%wearer.getMuzzlePoint(2),"0 0 0.35");
	%objhalfvelocity = getWord(%wearer.getVelocity(),0)/2 SPC getWord(%wearer.getVelocity(),1)/2 SPC getWord(%wearer.getVelocity(),2)/2;
	%obj.setvelocity(vectorAdd(%objhalfvelocity,getRandom(-8,8) SPC getRandom(-8,8) SPC getRandom(5,10)));	
}

function emptyPlayer::onAdd(%this, %obj) 
{
	%obj.setDamageLevel(%this.maxDamage);

	if(isObject(%source = %obj.source) && %obj.slotToMountBot)
	{
		%source.mountObject(%obj,%obj.slotToMountBot);
		
		if(%obj.imageToMount !$= "") %obj.mountImage(%obj.imageToMount,0);
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

			for (%i = 0; %i < clientgroup.getCount(); %i++) 			
			if(isObject(%client clientgroup.getObject(%i)) && isObject(%client.player))
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