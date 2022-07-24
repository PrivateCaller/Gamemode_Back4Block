package L4B2Bots_RadiusTeleporting
{	
	function serverLoadSaveFile_End()
	{
		parent::serverLoadSaveFile_End();

	    %count = $LoadingBricks_BrickGroup.getCount();
        if(%count < 1) 
		return;
        
		for(%i=0;%i<%count;%i++)
        {
			%brick = $LoadingBricks_BrickGroup.getObject(%i);

			if(%brick.getdataBlock().IsTeleBrick)
			{
				if(strstr(%brick.getName(),"SetCheck") != -1)
				{
					%bricknamefix1 = strreplace(%brick.getName(), "_TeleventSetCheck", "");
					%bricknamefix2 = strreplace(%bricknamefix1, "_", "");
					%name = %bricknamefix2 @ "_TeleventSet";

					%brick.teleset = new SimSet(%name);
					missionCleanup.add(%brick.teleset);
					%brick.teleset.ParBrick = %brick;
					%mainbrick = %brick;
					checkTeleAndSubCheckBricks(%mainbrick,%name);
				}
			}
		}
	}

	function checkTeleAndSubCheckBricks(%mainbrick,%name)
	{
		%count = $LoadingBricks_BrickGroup.getCount();
		for(%i=0;%i<%count;%i++)
        {
			%brick = $LoadingBricks_BrickGroup.getObject(%i);
			if(%brick.getdataBlock().IsTeleBrick && strstr(%brick.getname(),%name) != -1)
			{
				if(strstr(%brick.getName(),"SetBrick") != -1)
				%mainbrick.teleset.add(%brick);

				if(strstr(%brick.getName(),"SetSubCheck") != -1)
				%brick.teleset = %mainbrick.teleset;
			}
		}
	}
};
activatePackage(L4B2Bots_RadiusTeleporting);

function serverCmdCLEARMT(%client)
{
	if(%client.isAdmin || %client.isSuperAdmin)
	{	
		%client.currTeleSet = 0;
		%client.centerprint("\c6Cleared Teleset data",3);
	}
}

function serverCmdMODEMT(%client)
{
	if(%client.isAdmin || %client.isSuperAdmin)
	{	
		if(!%client.TeleSetMainMode)
		{
			%client.TeleSetMainMode = true;
			%client.centerprint("\c6Telebrick Checker mode set to main",3);
		}
		else
		{
			%client.TeleSetMainMode = false;
			%client.centerprint("\c6Telebrick Checker mode set to sub",3);
		}
	}
}

datablock fxDTSBrickData (brickTeleBrickCheckData:brick2x2fData)
{
	category = "Special";
	subCategory = "Left 4 BLock";
	uiName = "Telebrick Checker";
	isTeleBrick = true;
	alwaysShowWireFrame = false;
};

datablock fxDTSBrickData (brickTeleBrickData:brick4x4fData)
{
	category = "Special";
	subCategory = "Left 4 BLock";
	uiName = "Telebrick";
	isTeleBrick = true;
	alwaysShowWireFrame = false;
};

function brickTeleBrickData::onPlant(%data,%obj)
{
    Parent::onPlant(%data, %obj);

	%obj.setrendering(0);
	%obj.setcolliding(0);
	%obj.setraycasting(0);

    if(isObject(%obj.client.currTeleSet))
	{
		%obj.client.currTeleSet.add(%obj);
		%obj.setNTObjectName(%obj.client.currTeleSet.ParBrick @ "_TeleventSetBrick");

		if(isObject(%obj.client))
		%obj.client.centerprint("\c2Placed tele to checker <br>" @ "\c2" @ %obj.client.currTeleSet.ParBrick,3);
	}
	else if(isObject(%obj.client))
	%obj.client.centerprint("\c2No checker detected! Please set one first before placing a telebrick.",3);
}

function brickTeleBrickData::onloadPlant(%data, %obj)
{
	brickTeleBrickData::onPlant(%this, %obj);
}

function brickTeleBrickData::onDeath(%this,%obj)
{
	if(isObject(%obj.teleset))
	{
		%setname = strreplace(%obj.teleset.getname(), "_TeleventSet", "");
		%obj.client.centerprint("\c2Tele removed from teleset" SPC "\c2" @ %setname,3);
	}
	
	Parent::onDeath(%this, %obj);
}

function brickTeleBrickCheckData::onPlant(%data,%obj)
{
	Parent::onPlant(%data, %obj);

	%obj.setrendering(0);
	%obj.setcolliding(0);

	if(%obj.client.TeleSetMainMode)
	{
    	%obj.teleset = new SimSet(%obj @ "_TeleventSet");
		%obj.teleset.ParBrick = %obj;
    	%obj.client.currTeleSet = %obj.teleset;
		%obj.setNTObjectName(%obj @ "_TeleventSetCheck");
		%obj.client.centerprint("\c2Placed new checker <br>" @ "\c2" @ %obj,3);
	}
	else if(!%obj.TeleSetMainMode)
	{
		if(isObject(%obj.client.currTeleSet))
		{
			%obj.teleset = %obj.client.currTeleSet;
			%obj.setNTObjectName(%obj.client.currTeleSet.ParBrick @ "_TeleventSetSubCheck");

			if(isObject(%obj.client))
			%obj.client.centerprint("\c2Placed sub to checker <br>" @ "\c2" @ %obj.client.currTeleSet.ParBrick,3);
		}
		else if(isObject(%obj.client))
		%obj.client.centerprint("\c2No checker detected! Set mode to Main before placing a sub checker.",3);
	}
}

function brickTeleBrickCheckData::onloadPlant(%data, %obj)
{
	brickTeleBrickCheckData::onPlant(%this, %obj);
}

function brickTeleBrickCheckData::onDeath(%this,%obj)
{
	if(strstr(%obj.getName(),"TeleventSetCheck") != -1 && isObject(%obj.teleset))
	{
		%obj.teleset.delete();
		%obj.client.centerprint("\c2Teleset" SPC "\c2" @ %obj SPC "\c2removed",3);
	}
	else if(isObject(%obj.teleset))
	{
		%setname = strreplace(%obj.teleset.getname(), "_TeleventSet", "");
		%obj.client.centerprint("\c2Sub check removed from teleset" SPC "\c2" @ %setname,3);
	}

	Parent::onDeath(%this, %obj);
}

function DisableTeleporting(%brick)
{
    if(isObject(%brick.teleset))
    {
        for(%i = 0; %i < %brick.teleset.getcount(); %i++)
        {
            %telebrick = %brick.teleset.getObject(%i);

            if(isObject(MainTeleSet) && MainTeleSet.isMember(%telebrick))
            MainTeleSet.remove(%telebrick);
        }
    }
}

registerInputEvent("fxDTSBrick","onSurvivorBrickScan","Self fxDTSBrick");

function Player::BrickScanCheck(%obj)
{
	if(!$Pref::SurvivorPlayer::BrickScanning || !isObject(%obj) || %obj.getState() $= "Dead")
	return;

	InitContainerRadiusSearch(%obj.getPosition(), 15, $TypeMasks::FxBrickObjectType);
	while(%brick = containerSearchNext())
	{
		$InputTarget_["Self"] = %brick;
		%brick.processInputEvent("onSurvivorBrickScan",%brick.getgroup().client);

		if(%brick.getdataBlock().IsTeleBrick)
    	{
			if(isObject(%brick.teleset))
			{
    	    	for(%i = 0; %i < %brick.teleset.getcount(); %i++)
    	    	{
					%telebrick = %brick.teleset.getObject(%i);

    	    		if(!isObject(MainTeleSet))
    	    		new SimSet(MainTeleSet);
					else if(!MainTeleSet.isMember(%telebrick))
					missionCleanup.add(MainTeleSet);
					MainTeleSet.add(%telebrick);
		        }
    	    	cancel(%brick.teleset.DisableTeleporting);
    	    	%brick.teleset.DisableTeleporting = schedule(3000,0,DisableTeleporting,%brick);
			}
    	}
	}

	cancel(%obj.BrickScanCheck);
	%obj.BrickScanCheck = %obj.schedule(2000,BrickScanCheck);
}
registerOutputEvent("Bot","doMRandomTele");
registerOutputEvent("Player","doMRandomTele");
registerInputEvent("fxDTSBrick","onMRandomTele","Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "Bot Bot" TAB "MiniGame MiniGame");

function Player::doMRandomTele(%obj)
{	
	if(isObject(%main = MainTeleSet) && %main.getCount() > 0)
	{	
		%brick = %main.getObject(getRandom(0,%main.getcount()-1));

		%obj.settransform(vectorAdd(getwords(%brick.gettransform(),0,2),"0 0 0.25"));
		%obj.setvelocity(%obj.getvelocity());

		$InputTarget_["Self"] = %brick;
		switch$(%obj.getclassname())
		{
			case "Player":	$InputTarget_["Player"] = %obj;
							$InputTarget_["Client"] = %obj;

			case "AIPlayer": $InputTarget_["Bot"] = %obj;
		}
		$InputTarget_["MiniGame"] = getMiniGameFromObject(%obj);
		%brick.processInputEvent("onMRandomTele",%brick.getgroup().client);
	}
	else
	{
		%obj.kill();
		return;
	}
}