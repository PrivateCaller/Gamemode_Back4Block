package L4B_Debug
{
	function ServerCmdPlantBrick (%client)
	{
		Parent::ServerCmdPlantBrick (%client);
		if(isObject(%player = %client.player) && %client.isSuperAdmin) %player.l4bdebug();
	}
};

activatePackage(L4B_Debug);

function Player::l4bdebug(%obj)
{
    if(%obj.debugDelay < getSimTime())
    {
        %obj.debugDelay = getSimTime()+150;


    }
}