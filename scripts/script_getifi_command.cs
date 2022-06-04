function serverCmdGetIFI(%client)
{
	if(%client.isAdmin || %client.isSuperAdmin)
	{	
		if(isObject(%client.player) && %client.player.getState() !$= "Dead")
		{
			for (%n = 0; %n < %client.player.getDatablock().maxTools; %n++)
			{
				if(isObject(%client.player.tool[%n]))
				messageClient(%client,'ItemName',"\c2" @ %client.player.tool[%n].getName());
			}
		}
		else messageClient(%client,'NoItemName',"\c6Respawn and equip items.");
	}
	else messageClient(%client,'NoItemName',"\c6Must be an admin to use this command.");
}
function serverCmdGIFI(%client)
{
	serverCmdGetIFI(%client);
}