//
// The main object that stores all of the server's billboards.
//

$L4B::Billboard_SO = AVBillboards_Create(OverheadBillboardMount, $Pref::Server::MaxPlayers);

//
// The lights(billboards) the gamemode will use.
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
	flareColor = "1 0 0 1";

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
	flareColor = "1 0 0 1";

	AnimOffsets = true;
	startOffset = "0 0 1.25";
	endOffset = "0 0 1.25";
};

//
// Some of my own custom functions for Monoblaster's billboard script.
//

function Billboard_ExtendGroupLimit(%group, %mountDB, %count)
{
    for(%i = 0; %i < %count; %i++)
	{
		%billboard = Billboard_Create("LoadingBillboard",%mountDB,true);
		%group.add(%billboard);
	}
}

function Billboard_MountToPlayer(%target, %group, %lightDB)
{
	%billboard = %group.Make(%lightDB, "0 0 -1000", %target.getID());
    %target.MountObject(%billboard, 8);
	%target.billboard = %billboard;
}

function Billboard_DeallocFromPlayer(%group, %target)
{
    %group.Clear(%target.getID());
}

//
// A package for automatic management of billboards.
//

package Package_Left4Block_Billboards
{
    function GameConnection::onClientEnterGame(%client)
    {
        parent::onClientEnterGame(%client);
        //Load billboards onto the client when they are given a camera. This only needs to be done once.
        $L4B::Billboard_SO.Load(%client, "0 0 -1000");
    }
    function Armor::onRemove(%this, %obj)
    {
        //Deallocates the billboard attached to a player when they are deleted.
        if(isObject(%obj.billboard))
        {
            %obj.billboard.unmount();
            $L4B::Billboard_SO.Clear(%obj.getID());
        }
        parent::onRemove(%this, %obj);
    }
};
activatePackage(Package_Left4Block_Billboards);

//
// Finally, the function for spawning billboards on survivors that need help.
//

function Billboard_NeedySurvivor(%target, %mode)
{
	if($L4B::Billboard_SO.active >= $L4B::Billboard_SO.getCount())
	{
        Billboard_ExtendGroupLimit($L4B::Billboard_SO, OverheadBillboardMount, 1);
    }
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
    Billboard_MountToPlayer(%target, $L4B::Billboard_SO, %lightDB);
}

// INSTRUCTIONS:
// To mount a billboard to a player, do: Billboard_NeedySurvivor(`player_object`, `"Strangled" or "Incapped"`);
// To deallocate a billboard from a player, do: Billboard_DeallocFromPlayer(`player_object`);
// Billboards are automatically deallocated from a player when they die, despawn, or leave the game.