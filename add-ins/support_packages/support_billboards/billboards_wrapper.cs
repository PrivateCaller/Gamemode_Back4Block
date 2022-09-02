//
// The lights (billboards) the gamemode will use.
//

datablock fxLightData(strangledBillboard)
{
	LightOn = false;

	flareOn = true;
	flarebitmap = "./billboards/strangled.png";
	ConstantSize = 1.35;
    ConstantSizeOn = true;
    FadeTime = inf;

	LinkFlare = false;
	blendMode = 1;
	flareColor = "1 1 1 1";

	AnimOffsets = true;
	startOffset = "0 0 1.25";
	endOffset = "0 0 1.25";
};

datablock fxLightData(incappedBillboard)
{
	LightOn = false;

	flareOn = true;
	flarebitmap = "./billboards/incapped.png";
	ConstantSize = 1.35;
    ConstantSizeOn = true;
    FadeTime = inf;

	LinkFlare = false;
	blendMode = 0;
	flareColor = "1 1 1 1";

	AnimOffsets = true;
	startOffset = "0 0 1.25";
	endOffset = "0 0 1.25";
};

//
// Some of my own custom functions for Monoblaster's billboard script.
//

function Billboard_MountToPlayer(%target, %mode, %lightDB)
{
    // for(%i = 0; %i < ClientGroup.getCount(); %i++)
    // {
    //     %client = ClientGroup.getObject(%i);
    //     %group = %client.avBillboardGroup;
    //     if(isObject(%group))
    //     {
    //         %billboard = %group.Make(%lightDB, VectorAdd(%target.getEyePoint(), "0 0 5"), %target.client.bl_id @ "_" @ %mode);
    //         %target.MountObject(%billboard, 8);
    //     }
    // }
}

function Billboard_refreshAll()
{
    // for(%i = 0; %i < ClientGroup.getCount() - 1; %i++)
    // {
    //     %client = ClientGroup.getObject($i);
    //     if(!isObject(%client))
    //     {
    //         return; //Hate.
    //     }
    //     if(isObject(%client.avBillboardGroup))
    //     {
    //         %client.avBillboardGroup.delete();
    //     }
    //     %client.avBillboardGroup = AVBillboards_Create(OverheadBillboardMount, $Pref::Server::MaxPlayers * 2);
    //     %client.avBillboardGroup.load(%client,"0 0 1000");
    // }
}

function Billboard_DeallocFromPlayer(%target, %mode)
{
    // for(%i = 0; %i < ClientGroup.getCount(); %i++)
    // {
    //     %client = ClientGroup.getObject(%i);
        
    //     if(isObject(%group = %client.avBillboardGroup))
    //     for(%k = 0; %k < %group.getCount(); %k++)
    //     {
    //         if(%group.getObject(%k).tag $= %target.client.bl_id @ "_" @ %mode)
    //         {
    //             %group.Clear(%target.client.bl_id @ "_" @ %mode);
    //             return;
    //         }
    //     }
    // }
}

//
// A package for automatic management of billboards.
//

package Gamemode_Left4Block_Billboards
{
    function GameConnection::onClientEnterGame(%client)
    {
		%r = Parent::onClientEnterGame(%client);
		%client.avBillboardGroup = %group = AVBillboards_Create(OverheadBillboardMount, $Pref::Server::MaxPlayers * 2);
		%group.load(%client,"0 0 1000");
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
    function Armor::onRemove(%this, %obj)
    {
        //Deallocates the billboard attached to a player when they are deleted.
        Billboard_DeallocFromPlayer(%obj);
        parent::onRemove(%this, %obj);
    }
};
//activatePackage(Gamemode_Left4Block_Billboards);

//
// Finally, the function for spawning billboards on survivors that need help.
//

function Billboard_NeedySurvivor(%target, %mode)
{
	// if($L4B::Billboard_SO.active >= $L4B::Billboard_SO.getCount())
	// {
    //     Billboard_ExtendGroupLimit($L4B::Billboard_SO, OverheadBillboardMount, 1);
    // }
    // if(%mode $= "Strangled")
    // {
    //     %lightDB = strangledBillboard;
    // }
    // else if(%mode $= "Incapped")
    // {
    //     %lightDB = incappedBillboard;
    // }
    // else
    // {
    //     error("Billboard_NeedySurvivor :" SPC %mode SPC "is not a valid mode.");
    //     return; 
    // }
    // Billboard_MountToPlayer(%target, %mode, %lightDB);
}

// INSTRUCTIONS:
// To mount a billboard to a player, do: Billboard_NeedySurvivor(`player_object`, `"Strangled" or "Incapped"`);
// To deallocate a billboard from a player, do: Billboard_DeallocFromPlayer(`player_object`);
// Billboards are automatically deallocated from a player when they die, despawn, or leave the game.