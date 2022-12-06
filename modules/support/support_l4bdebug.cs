$admingod = false;
package L4B_Debug
{
	function ServerCmdPlantBrick (%client)
	{
		Parent::ServerCmdPlantBrick (%client);
		if(isObject(%player = %client.player) && %client.isSuperAdmin) %player.l4bdebug();
	}

    function Armor::Damage(%data, %obj, %sourceObject, %position, %damage, %damageType)
    {
        if($admingod && isObject(%obj.client) && %obj.client.isSuperAdmin) return;
		Parent::Damage(%data, %obj, %sourceObject, %position, %damage, %damageType);
    }	
};

activatePackage(L4B_Debug);

function Player::l4bdebug(%obj)
{
//	initContainerRadiusSearch(%obj.getHackPosition(), 25, $TypeMasks::PlayerObjectType);
//	while(%target = containerSearchNext()) 
//	{
//		if(%target == %obj) continue;
//
//		%obj.playthread(3,"activate2");
//		if(%target.hType $= "Zombie" && %target.getState() !$= "Dead") %target.damage(%obj, %target.getHackposition(), 500, $damageType::default);		
//	}
////if(isObject(%minigame = getMiniGameFromObject(%obj))) %minigame.spawnzombies(special,1);

	//if(%obj.client.isSuperAdmin && isObject(%brickgroup = %obj.client.brickgroup))
	//{		
	//	for (%i = 0; %i < %brickgroup.getCount(); %i++) 
	//	if(isObject(%brick = %brickgroup.getObject(%i)) && %brick.getdataBlock().isDoor)
	//	{
	//		
	//		%brick.delete();
	//	}
	//	
	//}

	//for (%i = 0; %i < $L4B_clientLog.getCount(); %i++) 
	//{
	//	talk($L4B_clientLog.getObject(%i).name);		
	//}
}