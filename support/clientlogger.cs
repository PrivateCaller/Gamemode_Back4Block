$L4B_clientLog = new ScriptGroup(L4B_clientLog) {};

function L4B_createClientSnapshot(%playerclient)
{
	if(!isObject(%playerclient))
	{
		return;
	}
	echo("Identified new client:" SPC %playerclient.name);
	%clientObject = new ScriptObject()
	{
		name = %playerclient.name;
		blid = %playerclient.getBLID();

        headColor = %playerclient.headColor;

        accent = %playerclient.accent;
        hat = %playerclient.hat;
        chest = %playerclient.chest;
        decalName = %playerclient.decalName;
        faceName = %playerclient.faceName;
        pack = %playerclient.pack;
        secondPack = %playerclient.secondPack;	
        larm = %playerclient.larm;	
        lhand = %playerclient.lhand;	
        rarm = %playerclient.rarm;
        rhand = %playerclient.rhand;
        hip = %playerclient.hip;	
        lleg = %playerclient.lleg;	
        rleg = %playerclient.rleg;

        accentColor = %playerclient.accentColor;
        hatColor = %playerclient.hatColor;
        packColor = %playerclient.packColor;
        secondPackColor = %playerclient.secondPackColor;

        chestColor = %playerclient.chestColor;
        larmColor = %playerclient.larmColor;
        rarmColor = %playerclient.rarmColor;
        rhandColor = %playerclient.rhandColor;
        lhandColor = %playerclient.lhandColor;
        hipColor = %playerclient.hipColor;
        llegColor = %playerclient.llegColor;
        rlegColor = %playerclient.rlegColor;
	};
	$L4B_clientLog.add(%clientObject);
	return %clientObject;
}

function L4B_storeClientSnapshots()
{
    echo("Storing" SPC $L4B_clientLog.getCount() SPC "clients...");
    %root_object = JettisonObject();
    for(%i = 0; %i < $L4B_clientLog.getCount(); %i++)
    {
        %snapshot = $L4B_clientLog.getObject(%i);
        %sub_object = %root_object.set(%snapshot.blid, "object", JettisonObject());
        %sub_object.set("name", "string", %snapshot.name);
        %sub_object.set("blid", "string", %snapshot.blid); //Redundent, but I'll use it anyway.
        %sub_object.set("headColor", "string", %snapshot.headColor);
        %sub_object.set("accent", "string", %snapshot.accent);
        %sub_object.set("hat", "string", %snapshot.hat);
        %sub_object.set("chest", "string", %snapshot.chest);
        %sub_object.set("decalName", "string", %snapshot.decalName);
        %sub_object.set("faceName", "string", %snapshot.faceName);
        %sub_object.set("pack", "string", %snapshot.pack);
        %sub_object.set("secondPack", "string", %snapshot.secondPack);
        %sub_object.set("larm", "string", %snapshot.larm);
        %sub_object.set("lhand", "string", %snapshot.lhand);
        %sub_object.set("rarm", "string", %snapshot.rarm);
        %sub_object.set("rhand", "string", %snapshot.rhand);
        %sub_object.set("hip", "string", %snapshot.hip);
        %sub_object.set("lleg", "string", %snapshot.lleg);
        %sub_object.set("rleg", "string", %snapshot.rleg);
        %sub_object.set("accentColor", "string", %snapshot.accentColor);
        %sub_object.set("hatColor", "string", %snapshot.hatColor);
        %sub_object.set("packColor", "string", %snapshot.packColor);
        %sub_object.set("secondPackColor", "string", %snapshot.secondPackColor);
        %sub_object.set("chestColor", "string", %snapshot.chestColor);
        %sub_object.set("larmColor", "string", %snapshot.larmColor);
        %sub_object.set("rarmColor", "string", %snapshot.rarmColor);
        %sub_object.set("lhandColor", "string", %snapshot.lhandColor);
        %sub_object.set("rhandColor", "string", %snapshot.rhandColor);
        %sub_object.set("hipColor", "string", %snapshot.hipColor);
        %sub_object.set("llegColor", "string", %snapshot.llegColor);
        %sub_object.set("rlegColor", "string", %snapshot.rlegColor);
    }
    jettisonWriteFile("config/server/L4B2_Bots/loggedplayers.json", "object", %root_object);
}

function L4B_loadClientSnapshots()
{
	//The "jettisonReadFile" call here sets the "$JSON::Value" variable you see below.
	//Regardless of the changes that could happen, that needs to be called.
	if(!isFile("config/server/L4B2_Bots/loggedplayers.json") || jettisonReadFile("config/server/L4B2_Bots/loggedplayers.json"))
	{
		return true;
	}
    %root_object = $JSON::Value;
    echo("Loading" SPC %root_object.keyCount SPC "clients...");
    for(%i = 0; %i < %root_object.keyCount; %i++)
    {
        %sub_object = %root_object.value[%root_object.keyName[%i]];
        for(%i = 0; %i < $L4B_clientLog.getCount(); %i++)
        {
            if($L4B_clientLog.getObject(%i).blid $= %sub_object.value["blid"])
            {
                //That client is already in memory, no need to load them in.
                continue;
            }
        }

        %clientObject = new ScriptObject()
        {
            name = %sub_object.value["name"];
            blid = %sub_object.value["blid"];

            headColor = %sub_object.value["headColor"];

            accent = %sub_object.value["accent"];
            hat = %sub_object.value["hat"];
            chest = %sub_object.value["chest"];
            decalName = %sub_object.value["decalName"];
            faceName = %sub_object.value["faceName"];
            pack = %sub_object.value["pack"];
            secondPack = %sub_object.value["secondPack"];
            larm = %sub_object.value["larm"];	
            lhand = %sub_object.value["lhand"];
            rarm = %sub_object.value["rarm"];
            rhand = %sub_object.value["rhand"];
            hip = %sub_object.value["hip"];
            lleg = %sub_object.value["lleg"];
            rleg = %sub_object.value["name"];

            accentColor = %sub_object.value["accentColor"];
            hatColor = %sub_object.value["hatColor"];
            packColor = %sub_object.value["packColor"];
            secondPackColor = %sub_object.value["secondPackColor"];

            chestColor = %sub_object.value["chestColor"];
            larmColor = %sub_object.value["larmColor"];
            rarmColor = %sub_object.value["rarmColor"];
            rhandColor = %sub_object.value["rhandColor"];
            lhandColor = %sub_object.value["lhandColor"];
            hipColor = %sub_object.value["hipColor"];
            llegColor = %sub_object.value["llegColor"];
            rlegColor = %sub_object.value["rlegColor"];
        };
        $L4B_clientLog.add(%clientObject);
        return %clientObject;
    }
	//No longer needed, save some RAM.
	if($JSON::Type $= "object") 
	{
		$JSON::Value.delete();
	}
}

function L4B_pushClientSnapshot(%obj, %sourceClient)
{
    if(%sourceClient $= "")
    {
        if(!isObject($L4B_clientLog) || $L4B_clientLog.getCount() < 1)
        {
            return false;
        }
        %sourceClient = $L4B_clientLog.getObject(getRandom(0, $L4B_clientLog.getCount() - 1));
    }

    %obj.llegColor = %sourceClient.llegColor;
	%obj.secondPackColor = %sourceClient.secondPackColor;
	%obj.lhand = %sourceClient.lhand;
	%obj.hip = %sourceClient.hip;
	%obj.faceName = %sourceClient.faceName;
	%obj.rarmColor = %sourceClient.rarmColor;
	%obj.hatColor = %sourceClient.hatColor;
	%obj.hipColor = %sourceClient.hipColor;
	%obj.chest = %sourceClient.chest;
	%obj.rarm = %sourceClient.rarm;
	%obj.packColor = %sourceClient.packColor;
	%obj.pack = %sourceClient.pack;
	%obj.decalName = %sourceClient.decalName;
	%obj.larmColor = %sourceClient.larmColor;
	%obj.secondPack = %sourceClient.secondPack;
	%obj.larm = %sourceClient.larm;
	%obj.chestColor = %sourceClient.chestColor;
	%obj.accentColor = %sourceClient.accentColor;
	%obj.rhandColor = %sourceClient.rhandColor;
	%obj.rleg = %sourceClient.rleg;
	%obj.rlegColor = %sourceClient.rlegColor;
	%obj.accent = %sourceClient.accent;
	%obj.headColor = %sourceClient.headColor;
	%obj.rhand = %sourceClient.rhand;
	%obj.lleg = %sourceClient.lleg;
	%obj.lhandColor = %sourceClient.lhandColor;
	%obj.hat = %sourceClient.hat;

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function L4B_pushZombifiedSnapshot(%obj)
{
    if(%sourceClient $= "")
    {
        if(!isObject($L4B_clientLog) || $L4B_clientLog.getCount() < 1)
        {
            return false;
        }
        %sourceClient = $L4B_clientLog.getObject(getRandom(0, $L4B_clientLog.getCount() - 1));
    }

    %skinColor = getWord(%sourceClient.headColor,0)/2.75 SPC getWord(%sourceClient.headColor,1)/1.5 SPC getWord(%sourceClient.headColor,2)/2.75 SPC 1;
    if(%sourceClient.chestColor $= %sourceClient.headColor)
	{
		%obj.chestColor = %skinColor;
	}
	else
	{
		%obj.chestColor = %sourceClient.chestColor;
	}
	if(%sourceClient.larmColor $= %sourceClient.headColor)
	{
		%obj.larmColor = %skinColor;
	}
	else
	{
		%obj.larmColor = %sourceClient.larmColor;
	}
	if(%playerclient.rarmColor $= %playerclient.headColor)
	{
		%obj.rarmColor = %skinColor;
	}
	else
	{
		%obj.rarmColor = %playerclient.rarmColor;
	}
	if(%sourceClient.rhandColor $= %sourceClient.headColor)
	{
		%obj.rhandColor = %skinColor;
	}
	else
	{
		%obj.rhandColor = %sourceClient.rhandColor;
	}
	if(%sourceClient.lhandColor $= %sourceClient.headColor)
	{
		%obj.lhandColor = %skinColor;
	}
	else
	{
		%obj.lhandColor = %sourceClient.larmColor;
	}
	if(%sourceClient.hipColor $= %sourceClient.headColor)
	{
		%obj.hipColor = %skinColor;
	}
	else
	{
		%obj.hipColor = %sourceClient.hipColor;
	}
	if(%sourceClient.llegColor $= %sourceClient.headColor)
	{
		%obj.llegColor = %skinColor;
	}
	else
	{
		%obj.llegColor = %sourceClient.llegColor;
	}
	if(%sourceClient.rlegColor $= %sourceClient.headColor)
	{
		%obj.rlegColor = %skinColor;
	}
	else
	{
		%obj.rlegColor = %sourceClient.rlegColor;
	}

	%obj.accentColor = %sourceClient.accentColor;
	%obj.accent = %sourceClient.accent;
	%obj.hatColor = %sourceClient.hatColor;
	%obj.hat = %sourceClient.hat;
	%obj.headColor = %skinColor;
	%obj.faceName = "asciiTerror";
	%obj.chest =  %sourceClient.chest;
	%obj.decalName = %sourceClient.decalName;
	%obj.pack = %sourceClient.pack;
	%obj.packColor = %sourceClient.packColor;
	%obj.secondPack = %sourceClient.secondPack;
	%obj.secondPackColor = %sourceClient.secondPackColor;
	%obj.larm = %sourceClient.larm;
	%obj.lhand = %sourceClient.lhand;
	%obj.rarm = %sourceClient.rarm;
	%obj.rhand = %sourceClient.rhand;
	%obj.hip = %sourceClient.hip;
	%obj.lleg = %sourceClient.lleg;
	%obj.rleg = %sourceClient.rleg;
	%obj.vestColor = getRandomBotRGBColor();

	%obj.name = "Infected" SPC %sourceClient.name;
	%obj.setShapeNameHealth();

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

package L4B_ClientLogger
{
	function GameConnection::onClientEnterGame(%this)
	{
		parent::onClientEnterGame(%this);
		for(%i = 0; %i < $L4B_clientLog.getCount(); %i++)
		{
			if($L4B_clientLog.getObject(%i).blid $= %this.getBLID())
			{
				return;
			}
		}
		L4B_createClientSnapshot(%this);
		L4B_storeClientSnapshots();
	}
};
activatePackage(L4B_ClientLogger);

L4B_loadClientSnapshots();