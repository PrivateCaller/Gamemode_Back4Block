$admingod = true;
package L4B_Debug
{
	function ServerCmdPlantBrick (%client)
	{
		Parent::ServerCmdPlantBrick (%client);
		//if(isObject(%player = %client.player) && %client.isSuperAdmin) %player.l4bdebug();
	}

    function Armor::Damage(%data, %obj, %sourceObject, %position, %damage, %damageType)
    {
        //if($admingod && isObject(%obj.client) && %obj.client.isSuperAdmin) return;
		Parent::Damage(%data, %obj, %sourceObject, %position, %damage, %damageType);
    }	
};

activatePackage(L4B_Debug);

function Player::l4bdebug(%obj)
{
	initContainerRadiusSearch(%obj.getHackPosition(), 25, $TypeMasks::PlayerObjectType);
	while(%target = containerSearchNext()) 
	{
		if(%target == %obj) continue;

		%obj.playthread(3,"activate2");
		if(%target.hType $= "Zombie" && %target.getState() !$= "Dead") %target.damage(%obj, %target.getHackposition(), 500, $damageType::default);		
	}
}