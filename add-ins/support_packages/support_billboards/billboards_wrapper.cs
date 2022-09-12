//
// The lights (billboards) the gamemode will use.
//

datablock fxLightData(strangledBillboard : DefaultAVBillboard)
{
	flarebitmap = "./billboards/strangled.png";
	ConstantSize = 1.35;
};

datablock fxLightData(incappedBillboard : DefaultAVBillboard)
{
	flarebitmap = "./billboards/incapped.png";
	ConstantSize = 1.35;
};

//
// Some of my own custom functions for Monoblaster's billboard script.
//

function Billboard_MountToPlayer(%target, %mode, %lightDB)
{
    for(%i = 0; %i < ClientGroup.getCount(); %i++)
    {
        %client = ClientGroup.getObject(%i);
        %group = %client.avBillboardGroup;
        if(isObject(%group))
        {
            BillboardMount_AddAVBillboard(%target.billboardMount, %group, %lightDB, %target.client.bl_id @ "_" @ %mode);
        }
    }
}

function Billboard_DeallocFromPlayer(%target, %mode)
{
    for(%i = 0; %i < ClientGroup.getCount(); %i++)
    {
        %client = ClientGroup.getObject(%i);
        %group = %client.avBillboardGroup;
        if(isObject(%group))
        {
            for(%k = 0; %k < %group.getCount(); %k++)
            {
                if(%group.getObject(%k).tag $= %target.client.bl_id @ "_" @ %mode)
                {
                    %group.Clear(%target.client.bl_id @ "_" @ %mode);
                    return;
                }
            }
        }
    }
}

//
// A package for automatic management of billboards.
//

package Gamemode_Left4Block_Billboards
{
    function GameConnection::onClientEnterGame(%client)
    {
		%r = Parent::onClientEnterGame(%client);
		%client.avBillboardGroup = %group = AVBillboardGroup_Make();
		%group.Load(%client, $Pref::Server::MaxPlayers * 2);
		%client.loadingbillboards = true;
		return %r;
	}
    function GameConnection::onClientLeaveGame(%client)
	{
		if(isObject(%client.avBillboardGroup))
		{
			%client.avBillboardGroup.delete();
		}
		return Parent::onClientLeaveGame(%client);
	}
    function Armor::onAdd(%this, %obj)
    {
        parent::onAdd(%this, %obj);
        %mount = OverheadBillboardMount.Make();
        %obj.billboardMount = %mount;
        %obj.MountObject(%mount, 8);
    }
    function Armor::onRemove(%this, %obj)
    {
        //Deallocates the billboard attached to a player when they are deleted.
        parent::onRemove(%this, %obj);
        BillboardMount_ClearBillboards(%obj.billboardMount);
    }
};
activatePackage(Gamemode_Left4Block_Billboards);

//
// Finally, the function for spawning billboards on survivors that need help.
//

function Billboard_NeedySurvivor(%target, %mode)
{
    if(%mode $= "Strangled")
    {
        %lightDB = strangledBillboard;
    }
    else if(%mode $= "Incapped")
    {
        %lightDB = incappedBillboard;
    }
    else
    {
        error("Billboard_NeedySurvivor :" SPC %mode SPC "is not a valid mode.");
        return; 
    }
    Billboard_MountToPlayer(%target, %mode, %lightDB);
}

// INSTRUCTIONS:
// To mount a billboard to a player, do: Billboard_NeedySurvivor(`player_object`, `"Strangled" or "Incapped"`);
// To deallocate a billboard from a player, do: Billboard_DeallocFromPlayer(`player_object`);
// Billboards are automatically deallocated from a player when they die, despawn, or leave the game.