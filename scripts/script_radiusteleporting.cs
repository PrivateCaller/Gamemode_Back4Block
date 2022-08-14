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

			if(%brick.getdataBlock().IsZoneBrick)
			{
				if(strstr(strlwr(%brick.getName()),"setcheck") != -1)
				{
					%bricknamefix1 = strreplace(strlwr(%brick.getName()), "_televentsetcheck", "");
					%bricknamefix2 = strreplace(%bricknamefix1, "_", "");
					%name = %bricknamefix2 @ "_TeleventSet";

					%brick.teleset = new SimSet(%name);
					missionCleanup.add(%brick.teleset);

					if(!isObject(GlobalTeleSet))
					{
						new SimSet(GlobalTeleSet);
						missionCleanup.add(GlobalTeleSet);
						GlobalTeleSet.add(%brick.teleset);
					}
					else GlobalTeleSet.add(%brick.teleset);

					%brick.teleset.ParBrick = %brick;
					%mainbrick = %brick;
					checkSetSubBricks(%mainbrick,%name);
				}
			}
		}
	}

	function checkSetSubBricks(%mainbrick,%name)
	{
		%count = $LoadingBricks_BrickGroup.getCount();
		for(%i=0;%i<%count;%i++)
        {
			%brick = $LoadingBricks_BrickGroup.getObject(%i);
			if(%brick.getdataBlock().IsZoneBrick && strstr(%brick.getname(),%name) != -1)
			{
				if(strstr(strlwr(%brick.getName()),"setbrick") != -1)
				%mainbrick.teleset.add(%brick);

				if(strstr(strlwr(%brick.getName()),"setitemspawn") != -1)
				%mainbrick.teleset.add(%brick);

				if(strstr(strlwr(%brick.getName()),"setsubcheck") != -1)
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
	IsZoneBrick = true;
	alwaysShowWireFrame = false;
};

datablock fxDTSBrickData (brickTeleBrickData:brick4x4fData)
{
	category = "Special";
	subCategory = "Left 4 BLock";
	uiName = "Telebrick";
	IsZoneBrick = true;
	alwaysShowWireFrame = false;
};

datablock fxDTSBrickData (brickTeleItemSpawnData:brick1x1fData)
{
	category = "Special";
	subCategory = "Left 4 BLock";
	uiName = "Tele Item Spawn";
	IsZoneBrick = true;
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

function brickTeleItemSpawnData::onPlant(%data,%obj)
{
    Parent::onPlant(%data, %obj);

	%obj.setrendering(0);
	%obj.setcolliding(0);
	%obj.setraycasting(0);

    if(isObject(%obj.client.currTeleSet))
	{
		%obj.client.currTeleSet.add(%obj);
		%obj.setNTObjectName(%obj.client.currTeleSet.ParBrick @ "_TeleventSetItemSpawn");

		if(isObject(%obj.client))
		%obj.client.centerprint("\c2Placed item spawn to zone <br>" @ "\c2" @ %obj.client.currTeleSet.ParBrick,3);
	}
	else if(isObject(%obj.client))
	%obj.client.centerprint("\c2No zone detected! Please set one first before placing an item spawn.",3);
}

function brickTeleItemSpawnData::onloadPlant(%data, %obj)
{
	brickTeleItemSpawnData::onPlant(%this, %obj);
}

function brickTeleItemSpawnData::onDeath(%this,%obj)
{
	if(isObject(%obj.teleset))
	{
		%setname = strreplace(%obj.teleset.getname(), "_TeleventSet", "");
		%obj.client.centerprint("\c2Item spawn removed from teleset" SPC "\c2" @ %setname,3);
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

		if(!isObject(GlobalTeleSet))
		{
			new SimSet(GlobalTeleSet);
			missionCleanup.add(GlobalTeleSet);
			GlobalTeleSet.add(%obj.teleset);
		}
		else if(!GlobalTeleSet.isMember(%obj.teleset))
		GlobalTeleSet.add(%obj.teleset);
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

function brickTeleBrickCheckData::onRemove(%this,%obj)
{
	if(isObject(%obj.teleset) && strstr(%obj.getName(),"TeleventSetCheck"))
	%obj.teleset.delete();

	Parent::onRemove(%this, %obj);
}

function brickTeleBrickCheckData::onDeath(%this,%obj)
{
	%obj.teleset.delete();

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

function removeTeleFromZone(%brick)
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

function MiniGameSO::sortItemSpawns(%minigame,%teleset,%client)
{
	for(%g = 0; %g < %teleset.getCount(); %g++) 
	{				
		if(isObject(%brick = %teleset.getObject(%g)))
		if(strstr(strlwr(%brick.getname()), "setitemspawn") != -1)
		{
			%list[%l++] = %brick;
			%lmax = %l;
		}
	}

	//This is an algorithim that is used to calculate what items should spawn, should there be more medpacks? more items? Let's see how much
	while(%count <= %lmax)
	{
		%count++;
		%eigth = mClamp(mFloatLength(%lmax/8, 0), 1, %half);
		%quarter = mFloatLength(%lmax/4, 0);
		%half = mFloatLength(%lmax/2, 0);

		if(%count <= %eigth)
		{
			if(%minigame.survivorStatHealthAverage < %minigame.survivorStatHealthMax/4)
			%list[%count].setItem($L4B_Medical[getRandom(1,$L4B_MedicalAmount)]);
			else %list[%count].setItem($L4B_Tier1[getRandom(1,$L4B_Tier1Amount)]);
		}
		else if(%count <= %quarter)
		{
			if(%minigame.survivorStatStressAverage < %minigame.survivorStressMax/2)
			%list[%count].setItem($L4B_Tier2[getRandom(1,$L4B_Tier2Amount)]);
			else if(%minigame.survivorStatHealthAverage < %minigame.survivorStatHealthMax/3)
			%list[%count].setItem($L4B_Medical[getRandom(1,$L4B_MedicalAmount)]);
			else
			{
				%random = getRandom(1,3);
				switch(%random)
				{
					case 1: %list[%count].setItem($L4B_Tier2[getRandom(1,$L4B_Tier2Amount)]);
					case 2: %list[%count].setItem($L4B_Melee[getRandom(1,$L4B_MeleeAmount)]);
					case 3: %list[%count].setItem($L4B_Grenade[getRandom(1,$L4B_GrenadeAmount)]);
				}
			}
		}
		else if(%count <= %half)
		{
			
			%random = getRandom(1,3);
			switch(%random)
			{
				case 1: %list[%count].setItem($L4B_Tier2[getRandom(1,$L4B_Tier2Amount)]);
				case 2: %list[%count].setItem($L4B_Medical[getRandom(1,$L4B_MedicalAmount)]);
				case 3: %list[%count].setItem($L4B_Grenade[getRandom(1,$L4B_GrenadeAmount)]);
			}
		}
		else if(%count <= %lmax)
		{
			if(getRandom(1,3) == 1)
			%list[%count].setItem($L4B_Tier1[getRandom(1,$L4B_Tier1Amount)]);
			else %list[%count].setItem($L4B_Melee[getRandom(1,$L4B_MeleeAmount)]);
		}

	}
}

registerInputEvent("fxDTSBrick","onSurvivorBrickScan","Self fxDTSBrick");
function Player::BrickScanCheck(%obj)
{
	if(!$Pref::SurvivorPlayer::BrickScanning || !isObject(%obj) || %obj.getState() $= "Dead" || !isObject(%minigame = getMiniGameFromObject(%obj)))
	return;

	InitContainerRadiusSearch(%obj.getPosition(), 10, $TypeMasks::FxBrickObjectType);
	while(%brick = containerSearchNext())
	{	
		$InputTarget_["Self"] = %brick;
		%brick.processInputEvent("onSurvivorBrickScan",%brick.getgroup().client);

		if(%brick.getdataBlock().IsZoneBrick)
    	{
			if(VectorDist(getWord(%obj.getPosition(),2), getWord(%brick.getPosition(),2)) > 5)
			continue;

			if(isObject(%brick.teleset))
			{
				if(!%brick.teleset.firstentry)
				{
					%brick.teleset.firstentry = 1;

					for(%g = 0; %g < %brick.teleset.getCount(); %g++) 
    				{				
    				    if(strstr(strlwr(%brick.teleset.getObject(%g).getName()),"wander") != -1)
    				    %spawn++;

						if(isObject(%setbricks = %brick.teleset.getObject(%g)) && strstr(strlwr(%setbricks.getname()), "itemspawn") != -1 && isObject(%minigame) && !%execonce)
						{
							%minigame.sortItemSpawns(%brick.teleset);
							%execonce = 1;
						}
    				}
					
					if(isObject(%minigame) && %spawn)
					%minigame.spawnWanderers(16,%brick.teleset,%client);
				}

    	    	for(%i = 0; %i < %brick.teleset.getcount(); %i++)
    	    	{					
					%telebrick = %brick.teleset.getObject(%i);
    	    		if(!isObject(MainTeleSet))
    	    		new SimSet(MainTeleSet);
					else if(!MainTeleSet.isMember(%telebrick))
					missionCleanup.add(MainTeleSet);
					MainTeleSet.add(%telebrick);
		        }
    	    	cancel(%brick.teleset.removeTeleFromZone);
    	    	%brick.teleset.removeTeleFromZone = schedule(3000,0,removeTeleFromZone,%brick);
			}
    	}
	}

	cancel(%obj.BrickScanCheck);
	%obj.BrickScanCheck = %obj.schedule(2000,BrickScanCheck);
}
registerOutputEvent("Bot","doMRandomTele","string 20 100");
registerOutputEvent("Player","doMRandomTele","string 20 100");
registerInputEvent("fxDTSBrick","onMRandomTele","Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "Bot Bot" TAB "MiniGame MiniGame");

function MinigameSO::spawnWanderers(%minigame,%amount,%teleset,%client)
{	
	if(isObject(directorBricks) && directorBricks.getCount() > 0)
    {
        for (%i = 0; %i < directorBricks.getCount(); %i++) 
        {
            %brick = directorBricks.getObject(%i);

            if(%brick.getdatablock().isZombieBrick && strstr(strlwr(%brick.getname()), "wander") != -1)
            {
                %brick.BotHoleAmountMax = %amount;
                %brick.spawnWandererBots(%teleset);
                break;
            }
        }
    }
}

function fxDTSBrick::spawnWandererBots(%obj,%teleset,%count)
{    
    if(%count >= %obj.BotHoleAmountMax)
    {
        %obj.BotHoleAmountMax = 0;
        return;
    }

    %obj.respawnBot();
	%obj.hBot.teleset = %teleset;
    %obj.hBot = 0;
    %obj.BotHoleAmount++;

    cancel(%obj.spawnBots);
    %obj.spawnBots = %obj.schedule(250,spawnWandererBots,%teleset,%count++);
}

function Player::doMRandomTele(%obj,%type)
{		
	if(isObject(%main = MainTeleSet) && %main.getCount() > 0)
	{	
		if(%type $= "")
		%brick = %main.getObject(getRandom(0,%main.getcount()-1));
		else
		{
			for (%i = 0; %i < %main.getCount(); %i++) 
			{
				if(strstr(strlwr(%main.getObject(%i).getName()),strlwr(%type)) != -1)
				{
					%bricklist[%n++] = %main.getObject(%i);
					%brick = %bricklist[getRandom(1,%n)];					
				}
			}
			if(!%n)
			%brick = %main.getObject(getRandom(0,%main.getcount()-1));
		}

		if(%obj.teleset)
		{
			for(%i = 0; %i < %obj.teleset.getCount(); %i++) 
			{				
				if(strstr(strlwr(%obj.teleset.getObject(%i).getName()),"wander") != -1)
				{
					%wanderlist[%m++] = %obj.teleset.getObject(%i);
					%brick = %wanderlist[getRandom(1,%m)];
				}
				else if(!%m)
				%brick = %main.getObject(getRandom(0,%main.getcount()-1));
			}
		}

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