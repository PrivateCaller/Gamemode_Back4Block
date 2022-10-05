package L4B_AreaZones
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
				if(strstr(strlwr(%brick.getName()),"_az_main") != -1)
				{
					%nameposafterid = strstr(strlwr(%brick.getName()),"_az_main_ct");
					%removenoid = getSubStr(%brick.getName(), %nameposafterid, strlen(%brick.getName()));
					%name = strreplace(%brick.getName(), %removenoid, "");
					%removeunderspace = strreplace(%name, "_", "");

					%brick.AreaZone = new SimSet(%removeunderspace @ "_AZ");
					%brick.AreaZone.ParBrick = %brick;

					%ctpos = strstr(strlwr(%brick.getName()),"_ct")+3;
					%removenoctnum = getSubStr(%brick.getName(), 0, %ctpos);
					%ctnum = strreplace(%brick.getName(), %removenoctnum, "");
					%brick.AreaZone.CtNum = %ctnum;

					if(!isObject(GlobalAreaZone))
					{
						new SimSet(GlobalAreaZone);
						missionCleanup.add(GlobalAreaZone);
						GlobalAreaZone.add(%brick.AreaZone);
					}
					else GlobalAreaZone.add(%brick.AreaZone);
					missionCleanup.add(%brick.AreaZone);

					checkSetSubBricks(%brick,%removeunderspace);
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
			if(strstr(%brick.getname(),%name) != -1)
			{
				if(strstr(strlwr(%brick.getName()),"az_spawn") != -1)
				%mainbrick.AreaZone.add(%brick);

				if(strstr(strlwr(%brick.getName()),"az_item") != -1)
				%mainbrick.AreaZone.add(%brick);

				if(strstr(strlwr(%brick.getName()),"az_submain") != -1)
				%brick.AreaZone = %mainbrick.AreaZone;
			}
		}
	}
};
activatePackage(L4B_AreaZones);

function serverCmdAZ(%client)
{
	if(%client.isAdmin || %client.isSuperAdmin)
	{	
		messageClient(%client, '', "<font:impact:25>\c6Area Zone Mode:" SPC "\c4" @ %client.AreaZoneMainMode);
		messageClient(%client, '', "<font:impact:25>\c6Area Zone:" SPC "\c4" @ %client.currAreaZone);

		if(isObject(%client.currAreaZone))
		messageClient(%client, '', "<font:impact:25>\c6Area Zone Count:" SPC "\c4" @ %client.currAreaZone.getCount());
		messageClient(%client, '', "<font:impact:25>\c6Area Zone Par Brick:" SPC "\c4" @ %client.currAreaZone.ParBrick);
		messageClient(%client, '', "<font:impact:25>\c6Area Zone Item Brick Type:" SPC "\c4" @ %client.AZItemType);
		messageClient(%client, '', "<font:impact:25>\c6Area Zone Spawn Brick Type:" SPC "\c4" @ %client.AZSpawnType);
		messageClient(%client, '', "<font:impact:25>\c6Area Zone Number:" SPC "\c4" @ %client.AZCount);
	}
}


function serverCmdCLEARAZ(%client)
{
	if(%client.isAdmin || %client.isSuperAdmin)
	{	
		%client.currAreaZone = 0;
		%client.AZCount = 0;
		%client.centerprint("\c6Cleared area zone data",3);
	}
}

function serverCmdAZM(%client)
{
	if(%client.isAdmin || %client.isSuperAdmin)
	{
		if(!%client.AreaZoneMainMode)
		{
			%client.AreaZoneMainMode = true;
			%client.centerprint("\c6Area Zone Checker mode set to main",3);
		}
		else
		{
			%client.AreaZoneMainMode = false;
			%client.centerprint("\c6Area Zone Checker mode set to sub",3);
		}
	}
}

function serverCmdAZI(%client,%type)
{
	if(%client.isAdmin || %client.isSuperAdmin)
	{	
		if(%type $= "")
		{
			%client.AZItemType = "Gen";
			%client.centerprint("\c6Area Zone Item mode set to" SPC "\c6Gen",3);
		}
		else 
		{
			%client.AZItemType = %type;
			%client.centerprint("\c6Area Zone Item mode set to" SPC "\c6" @ %type,3);
		}
	}
}

function serverCmdAZS(%client,%type)
{
	if(%client.isAdmin || %client.isSuperAdmin)
	{	
		if(%type $= "")
		{
			%client.AZSpawnType = "Horde";
			%client.centerprint("\c6Area Zone Spawn mode set to" SPC "\c6Horde",3);
		}
		else 
		{
			%client.AZSpawnType = %type;
			%client.centerprint("\c6Area Zone Spawn mode set to" SPC "\c6" @ %type,3);
		}
	}
}

datablock fxDTSBrickData (brickAreaZoneCheckData:brick2x2fData)
{
	category = "Special";
	subCategory = "Left 4 BLock";
	uiName = "Area Zone Checker";
	IsZoneBrick = true;
	alwaysShowWireFrame = false;
};

datablock fxDTSBrickData (brickAreaZoneSpawnerData:brick4x4fData)
{
	category = "Special";
	subCategory = "Left 4 BLock";
	uiName = "Area Zone Spawner";
	IsZoneBrick = false;
	alwaysShowWireFrame = false;
};

datablock fxDTSBrickData (brickAreaZoneItemSpawnData:brick1x1fData)
{
	category = "Special";
	subCategory = "Left 4 BLock";
	uiName = "Area Zone Item Spawn";
	IsZoneBrick = false;
	alwaysShowWireFrame = false;
};

function brickAreaZoneSpawnerData::onPlant(%data,%obj)
{
    Parent::onPlant(%data, %obj);

	%obj.setrendering(0);
	%obj.setcolliding(0);
	%obj.setraycasting(0);

    if(isObject(%obj.client.currAreaZone))
	{
		%obj.client.currAreaZone.add(%obj);

		%parentbrick = %obj.client.currAreaZone.ParBrick;
		%nameposafterid = strstr(strlwr(%parentbrick.getName()),"_az_main_ct");
		%removenoid = getSubStr(%parentbrick.getName(), %nameposafterid, strlen(%parentbrick.getName()));
		%name = strreplace(%parentbrick.getName(), %removenoid, "");
		%removeunderspace = strreplace(%name, "_", "");

		if(%obj.client.AZSpawnType !$= "")
		%obj.setNTObjectName(%name @ "_AZ_Spawn_" @ %obj.client.AZSpawnType);
		else %obj.setNTObjectName(%name @ "_AZ_Spawn_Horde");

		if(isObject(%obj.client))
		%obj.client.centerprint("\c2Placed spawn to zone <br>" @ "\c2" @ %removeunderspace,3);
	}
	else if(isObject(%obj.client))
	%obj.client.centerprint("\c2No zone detected! Please set one first before placing a area zone.",3);
}

function brickAreaZoneSpawnerData::onloadPlant(%data, %obj)
{
	brickAreaZoneSpawnerData::onPlant(%this, %obj);
}

function brickAreaZoneSpawnerData::onDeath(%this,%obj)
{	
	Parent::onDeath(%this, %obj);
}

function brickAreaZoneItemSpawnData::onPlant(%data,%obj)
{
    Parent::onPlant(%data, %obj);

	%obj.setrendering(0);
	%obj.setcolliding(0);
	%obj.setraycasting(0);

    if(isObject(%obj.client.currAreaZone))
	{
		%obj.client.currAreaZone.add(%obj);

		%parentbrick = %obj.client.currAreaZone.ParBrick;
		%nameposafterid = strstr(strlwr(%parentbrick.getName()),"_az_main_ct");
		%removenoid = getSubStr(%parentbrick.getName(), %nameposafterid, strlen(%parentbrick.getName()));
		%name = strreplace(%parentbrick.getName(), %removenoid, "");
		%removeunderspace = strreplace(%name, "_", "");

		if(%obj.client.AZItemType !$= "")
		%obj.setNTObjectName(%name @ "_AZ_Item_" @ %obj.client.AZItemType);
		else %obj.setNTObjectName(%name @ "_AZ_Item_Gen");

		if(isObject(%obj.client))
		%obj.client.centerprint("\c2Placed item spawn to zone <br>" @ "\c2" @ %removeunderspace,3);
	}
	else if(isObject(%obj.client))
	%obj.client.centerprint("\c2No zone detected! Please set one first before placing an item spawn.",3);
}

function brickAreaZoneItemSpawnData::onloadPlant(%data, %obj)
{
	brickAreaZoneItemSpawnData::onPlant(%this, %obj);
}

function brickAreaZoneItemSpawnData::onDeath(%this,%obj)
{
	Parent::onDeath(%this, %obj);
}

function brickAreaZoneCheckData::onPlant(%data,%obj)
{
	Parent::onPlant(%data, %obj);

	%obj.setrendering(0);
	%obj.setcolliding(0);

	if(%obj.client.AreaZoneMainMode)
	{
    	%obj.AreaZone = new SimSet(%obj @ "_AZ");
		%obj.AreaZone.ParBrick = %obj;
		%obj.AreaZone.CtNum = %obj.client.AZCount++;
    	%obj.client.currAreaZone = %obj.AreaZone;
		%obj.setNTObjectName(%obj @ "_AZ_Main_CT" @ %obj.client.AZCount);
		%obj.client.centerprint("\c2Placed new area checker <br>" @ "\c2" @ %obj,3);

		if(!isObject(GlobalAreaZone))
		{
			new SimSet(GlobalAreaZone);
			missionCleanup.add(GlobalAreaZone);
			GlobalAreaZone.add(%obj.AreaZone);
		}
		else if(!GlobalAreaZone.isMember(%obj.AreaZone))
		GlobalAreaZone.add(%obj.AreaZone);
	}
	else if(!%obj.AreaZoneMainMode)
	{
		if(isObject(%obj.client.currAreaZone))
		{
			%obj.AreaZone = %obj.client.currAreaZone;
			%parentbrick = %obj.client.currAreaZone.ParBrick;

			%nameposafterid = strstr(strlwr(%parentbrick.getName()),"_az_main_ct");
			%removenoid = getSubStr(%parentbrick.getName(), %nameposafterid, strlen(%parentbrick.getName()));
			%name = strreplace(%parentbrick.getName(), %removenoid, "");
			%removeunderspace = strreplace(%name, "_", "");
			
			%obj.setNTObjectName(%name @ "_AZ_SubMain");

			if(isObject(%obj.client))
			%obj.client.centerprint("\c2Placed sub to checker <br>" @ "\c2" @ %removeunderspace,3);
		}
		else if(isObject(%obj.client))
		%obj.client.centerprint("\c2No checker detected! Set mode to Main before placing a sub checker.",3);
	}
}

function brickAreaZoneCheckData::onloadPlant(%data, %obj)
{
	brickAreaZoneCheckData::onPlant(%this, %obj);
}

function brickAreaZoneCheckData::onRemove(%this,%obj)
{
	if(strstr(strlwr(%obj.getName()),"_main") != -1 && isObject(%obj.AreaZone))
	%obj.AreaZone.delete();

	Parent::onRemove(%this, %obj);
}

function brickAreaZoneCheckData::onDeath(%this,%obj)
{
	if(strstr(strlwr(%obj.getName()),"_main") != -1 && isObject(%obj.AreaZone))
	{
		%obj.AreaZone.delete();
		%obj.client.centerprint("\c2Area Zone" SPC "\c2" @ %obj SPC "\c2removed",3);
	}

	Parent::onDeath(%this, %obj);
}

function removeAreaFromZone(%brick)
{
    if(isObject(%brick.AreaZone))
    {
        for(%i = 0; %i < %brick.AreaZone.getcount(); %i++)
        {
            %areazone = %brick.AreaZone.getObject(%i);

            if(isObject(MainAreaZone) && MainAreaZone.isMember(%areazone))
            MainAreaZone.remove(%areazone);
        }
    }
}

function MiniGameSO::sortItemSpawns(%minigame,%AreaZone,%client)
{
	for(%g = 0; %g < %AreaZone.getCount(); %g++) 
	{				
		if(isObject(%brick = %AreaZone.getObject(%g)))
		{
			if(strstr(strlwr(%brick.getname()), "az_item") != -1)
			{
				if(strstr(strlwr(%brick.getname()), "_gen") != -1)
				%list[%l++] = %brick;
			}
		}
	}

	%eigth = mClamp(mFloatLength(%l/8, 0), 1, %half);
	%quarter = mFloatLength(%l/4, 0);
	%half = mFloatLength(%l/2, 0);

    while(%count <= %l)//General randomness
	{
		%count++;
		if(isObject(%list[%count]))
		{
			if(%count <= %eigth)
			{
			if(%minigame.survivorStatHealthAverage < %minigame.survivorStatHealthMax/4)
				%list[%count].setitem($L4B_Medical[getRandom(1,$L4B_MedicalAmount)]);
				else if(getRandom(1,8) == 1)
				%list[%count].setitem($L4B_Misc[getRandom(1,$L4B_MiscAmount)]);
				else %list[%count].setitem(none);
			}
			else if(%count <= %quarter)
			{
			if(%minigame.survivorStatStressAverage > %minigame.survivorStressMax/2)
			{
				%randomquartert2 = getRandom(1,3);
				switch(%randomquartert2)
				{
					case 1: %list[%count].setitem($L4B_Melee[getRandom(1,$L4B_MeleeAmount)]);
					case 2: %list[%count].setitem($L4B_RifleT2[getRandom(1,$L4B_RifleT2Amount)]);
					case 3: %list[%count].setitem($L4B_ShotgunT1[getRandom(1,$L4B_ShotgunT2Amount)]);
				}
			}
			else if(%minigame.survivorStatHealthAverage < %minigame.survivorStatHealthMax/3)
			%list[%count].setitem($L4B_Medical[getRandom(1,$L4B_MedicalAmount)]);
			else
			{
				if(getRandom(1,6) == 1)
				{
					%randomquartert1 = getRandom(1,4);
					switch(%randomquartert1)
					{
						case 1: %list[%count].setitem($L4B_Melee[getRandom(1,$L4B_MeleeAmount)]);
						case 2: %list[%count].setitem($L4B_Grenade[getRandom(1,$L4B_GrenadeAmount)]);
						case 3: %list[%count].setitem($L4B_Medical[getRandom(1,$L4B_MedicalAmount)]);
						case 4: %list[%count].setitem($L4B_PistolT2[getRandom(1,$L4B_PistolT2Amount)]);
					}
				}
				else %list[%count].setitem(none);
			}
			}
			else if(%count <= %half)
			{		
			if(getRandom(1,2) == 1)
			{
				%randomhalf = getRandom(1,4);
				switch(%randomhalf)
				{
					case 1: %list[%count].setitem($L4B_PistolT1[getRandom(1,$L4B_PistolT1Amount)]);
					case 2: %list[%count].setitem($L4B_SMGT1[getRandom(1,$L4B_SMGT1Amount)]);
					case 3: %list[%count].setitem($L4B_ShotgunT1[getRandom(1,$L4B_ShotgunT1Amount)]);
					case 4: %list[%count].setitem($L4B_Medical[getRandom(1,$L4B_MedicalAmount)]);
				}
			}
			else %list[%count].setitem(none);
			}
			else if(%count <= %l)
			{
			if(getRandom(1,4) == 1)
			%list[%count].setitem($L4B_Melee[getRandom(1,$L4B_MeleeAmount)]);
			else %list[%count].setitem(none);
			}
		}
	}

	for(%g = 0; %g < %AreaZone.getCount(); %g++) 
	{				
		if(isObject(%brick = %AreaZone.getObject(%g)))
		{
			if(strstr(strlwr(%brick.getname()), "az_item") != -1)
			{
				if(strstr(strlwr(%brick.getname()), "_grenade") != -1)
				%brick.setitem($L4B_Grenade[getRandom(1,$L4B_GrenadeAmount)]);

				if(strstr(strlwr(%brick.getname()), "_melee") != -1)
				%brick.setitem($L4B_Melee[getRandom(1,$L4B_MeleeAmount)]);

				if(strstr(strlwr(%brick.getname()), "_misc") != -1)
				%brick.setitem($L4B_Misc[getRandom(1,$L4B_MiscAmount)]);

				if(strstr(strlwr(%brick.getname()), "_medical") != -1)
				%brick.setitem($L4B_Medical[getRandom(1,$L4B_MedicalAmount)]);

				if(strstr(strlwr(%brick.getname()), "_pistolt1") != -1)
				%brick.setitem($L4B_PistolT1[getRandom(1,$L4B_PistolT1Amount)]);				
				
				if(strstr(strlwr(%brick.getname()), "_pistolt2") != -1)
				%brick.setitem($L4B_PistolT2[getRandom(1,$L4B_PistolT2Amount)]);

				if(strstr(strlwr(%brick.getname()), "_smgt1") != -1)
				%brick.setitem($L4B_SMGT1[getRandom(1,$L4B_SMGT1Amount)]);

				if(strstr(strlwr(%brick.getname()), "_shotgunt1") != -1)
				%brick.setitem($L4B_ShotgunT1[getRandom(1,$L4B_ShotgunT1Amount)]);

				if(strstr(strlwr(%brick.getname()), "_shotgunt2") != -1)
				%brick.setitem($L4B_ShotgunT2[getRandom(1,$L4B_ShotgunT2Amount)]);

				if(strstr(strlwr(%brick.getname()), "_riflet2") != -1)
				%brick.setitem($L4B_RifleT2[getRandom(1,$L4B_RifleT2Amount)]);
				
			}
		}
	}
}

function Player::BrickScanCheck(%obj)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead" || !isObject(%minigame = getMiniGameFromObject(%obj)))
	return;
	
	if(getword(%obj.getvelocity(),2) < -15)
	{
		%obj.playthread(2,"side");
		L4B_SpazzZombie(%obj,0);
		if(!%obj.isFalling)
		{
			%obj.playaudio(0,"survivor_pain_high1_sound");
			%obj.isFalling = 1;
		}
	}
	else if(%obj.isFalling)
	{
		L4B_SpazzZombie(%obj,15);
		%obj.playthread(2,"root");
		%obj.isFalling = 0;
	}

	%obj.AreaZoneNum = 0;
	%survivorsfound = 1;
	%obj.survivorAllyCount = %survivorsfound;

	InitContainerRadiusSearch(%obj.getPosition(), 25, $TypeMasks::FxBrickObjectType | $TypeMasks::PlayerObjectType);
	while(%scan = containerSearchNext())
	{
		if(%scan == %obj || VectorDist(getWord(%obj.getPosition(),2), getWord(%scan.getPosition(),2)) > 5)
		continue;
		
		if(%scan.getType() & $TypeMasks::PlayerObjectType && %scan.getdataBlock().isSurvivor)
		{
			%survivorsfound++;
			%obj.survivorAllyCount = %survivorsfound;
		}

		if(ContainerSearchCurrRadiusDist() <= 10 && %scan.getType() & $TypeMasks::FxBrickObjectType && %scan.getdataBlock().IsZoneBrick)
    	{
			%brick = %scan;
			
			if(isObject(%brick.AreaZone))
			{
    	    	%obj.AreaZoneNum = %brick.AreaZone.CtNum;
				%obj.currAreaZone = %brick.AreaZone;
				
				for(%i = 0; %i < %brick.AreaZone.getcount(); %i++)
    	    	{					
					%areazone = %brick.AreaZone.getObject(%i);
    	    		if(!isObject(MainAreaZone)) 
					{
						new SimSet(MainAreaZone);
						missionCleanup.add(MainAreaZone);
					}
					else if(!MainAreaZone.isMember(%areazone)) MainAreaZone.add(%areazone);										
		        }		
				
				if(!%brick.AreaZone.firstentry)
				{
					%brick.AreaZone.firstentry = 1;

					if(isObject(%brick.areazone.ParBrick))
					{						
						$InputTarget_["Self"] = %brick.areazone.parbrick;
						switch$(%obj.getclassname())
						{
							case "Player":	$InputTarget_["Player"] = %obj;
											$InputTarget_["Client"] = %obj.client;
							case "AIPlayer": $InputTarget_["Bot"] = %obj;
						}
						$InputTarget_["MiniGame"] = getMiniGameFromObject(%obj);
						%brick.areazone.parbrick.processInputEvent("onAZFirstEntry",%obj.client);
					}

					for(%g = 0; %g < %brick.AreaZone.getCount(); %g++) 
    				{				
    				    if(strstr(strlwr(%brick.AreaZone.getObject(%g).getName()),"_wander") != -1)
						%spawn++;

						if(isObject(%setbricks = %brick.AreaZone.getObject(%g)) && strstr(strlwr(%setbricks.getname()), "item") != -1 && isObject(%minigame) && !%execonce)
						{
							%minigame.sortItemSpawns(%brick.AreaZone);
							break;
						}
    				}

					for(%g = 0; %g < %brick.AreaZone.getCount(); %g++) 
    				{				
    				    if(strstr(strlwr(%brick.AreaZone.getObject(%g).getName()),"_wander") != -1)
						{
							if(isObject(%minigame = getMiniGameFromObject(%obj)))
							%minigame.spawnZombies("Wander",getRandom(20,25),%obj.currAreaZone);
							break;
						}
    				}
				}

    	    	cancel(%brick.AreaZone.removeAreaFromZone);
    	    	%brick.AreaZone.removeAreaFromZone = schedule(2500,0,removeAreaFromZone,%brick);
			}
    	}
	}

	cancel(%obj.BrickScanCheck);
	%obj.BrickScanCheck = %obj.schedule(1500,BrickScanCheck);
}

registerInputEvent("fxDTSBrick","onAZFirstEntry","Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "Bot Bot" TAB "MiniGame MiniGame");
registerOutputEvent("Bot","doMRandomTele","string 20 100");
registerOutputEvent("Player","doMRandomTele","string 20 100");

function Player::doMRandomTele(%obj,%type)
{		
	if(isObject(%main = MainAreaZone) && %main.getCount() > 0)
	{	
		if(%type $= "") %brick = %main.getObject(getRandom(0,%main.getcount()-1));
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
			if(!%n) %brick = %main.getObject(getRandom(0,%main.getcount()-1));
		}

		if(%obj.AreaZone)
		{
			for(%i = 0; %i < %obj.AreaZone.getCount(); %i++) 
			{				
				if(strstr(strlwr(%obj.AreaZone.getObject(%i).getName()),"wander") != -1)
				{
					%wanderlist[%m++] = %obj.AreaZone.getObject(%i);
					%brick = %wanderlist[getRandom(1,%m)];
				}
				else if(!%m) %brick = %main.getObject(getRandom(0,%main.getcount()-1));
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
		%brick.processInputEvent("onDirectorBotTeleSpawn",%brick.getgroup().client);
	}
	else
	{
		%obj.kill();
		return;
	}
}