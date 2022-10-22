if(forceRequiredAddOn("Tool_NewDuplicator") == $Error::AddOn_NotFound || !isFunction(NetObject, setNetFlag)) return;
if(!isObject(AreaZoneGroup)) new ScriptGroup(AreaZoneGroup);

package L4B_AreaZones
{
    function MiniGameSO::Reset(%minigame,%client)
	{		
		Parent::Reset(%minigame,%client);

		%currTime = getSimTime();
		if(%obj.lastResetTime + 5000 > %currTime) return;
		%minigame.lastResetTime = %currTime;
	
		if(AreaZoneGroup.getCount())
		for(%i = 0; %i < AreaZoneGroup.getCount(); %i++)
		if(isObject(%zone = AreaZoneGroup.getObject(%i)))
		{
			%zone.trigger.firstentry = false;
			%zone.trigger.presencecount = 0;

			for(%g = 0; %g < %zone.simset.getCount(); %g++)  if(isObject(%setbricks = %zone.simset.getObject(%g))) %setbricks.setitem(none);
		}
	} 		

	function Armor::onDisabled(%this, %obj)
	{
		if(isObject(%obj.currentZone)) %obj.currentZone.getDataBlock().onLeaveTrigger(%obj.currentZone,%obj);

		Parent::onDisabled(%this, %obj);
	}

	function serverDirectSaveFileLoad (%filename, %colorMethod, %dirName, %ownership, %silent)
	{
		Parent::serverDirectSaveFileLoad (%filename, %colorMethod, %dirName, %ownership, %silent);
		serverazload(strreplace(fileName(%fileName),".bls",""));
	}

	function serverLoadSaveFile_End()
	{
		parent::serverLoadSaveFile_End();

		if(!$LoadingBricks_BrickGroup.getCount()) return;
		
		for(%i=0;%i<$LoadingBricks_BrickGroup.getCount();%i++)
		{
			%brick = $LoadingBricks_BrickGroup.getObject(%i);
			%zone.trigger.firstentry = false;
								
			if(%brick.getdataBlock().IsZoneBrick)
			{
				if(AreaZoneGroup.getCount())
				for(%azg = 0; %azg < AreaZoneGroup.getCount(); %azg++)
				if(isObject(%zone = AreaZoneGroup.getObject(%azg)) && strstr(strlwr(%brick.getName()),strlwr(%zone.zonename)) != -1) %zone.simset.add(%brick);				
			}
		}
	}

	function GameConnection::onClientLeaveGame(%this)
	{
		if(isObject(%this.AreaEditZone)) %this.AreaEditZone.stopEdit();
		Parent::onClientLeaveGame(%this);
	}

	function serverCmdShiftBrick(%client, %x, %y, %z)
	{
		if(!isObject(%client.AreaEditZone)) return parent::serverCmdShiftBrick(%client, %x, %y, %z);

		//Move the corner
		switch(getAngleIDFromPlayer(%client.getControlObject()))
		{
			case 0: %newX =  %x; %newY =  %y;
			case 1: %newX = -%y; %newY =  %x;
			case 2: %newX = -%x; %newY = -%y;
			case 3: %newX =  %y; %newY = -%x;
		}

		%newX = mFloor(%newX) / 2;
		%newY = mFloor(%newY) / 2;
		%z = mFloor(%z) / 5;

		%client.AreaEditZone.editBox.shiftCorner(%newX SPC %newY SPC %z, 100000);
	}

	//Super Shift Brick
	function serverCmdSuperShiftBrick(%client, %x, %y, %z)
	{
		if(!isObject(%client.AreaEditZone)) return parent::serverCmdSuperShiftBrick(%client, %x, %y, %z);
		serverCmdShiftBrick(%client, %x * 8, %y * 8, %z * 20);
	}

	//Rotate Brick
	function serverCmdRotateBrick(%client, %direction)
	{
		if(!isObject(%client.AreaEditZone)) return parent::serverCmdRotateBrick(%client, %direction);
		%client.AreaEditZone.editBox.switchCorner();
	}
};
activatePackage(L4B_AreaZones);

registerOutputEvent("Bot","doMRandomTele","string 20 100");
registerOutputEvent("Player","doMRandomTele","string 20 100");

datablock fxDTSBrickData (brickAreaZoneSpawnerData:brick4x4fData)
{
	category = "Special";
	subCategory = "Left 4 BLock";
	uiName = "Area Zone Spawner";
	IsZoneBrick = true;
	ZoneBrickType = "spawner";
	alwaysShowWireFrame = false;
};

datablock fxDTSBrickData (brickAreaZoneItemSpawnData:brick1x1fData)
{
	category = "Special";
	subCategory = "Left 4 BLock";
	uiName = "Area Zone Item Spawn";
	IsZoneBrick = true;
	ZoneBrickType = "item";
	alwaysShowWireFrame = false;
};

function brickAreaZoneSpawnerData::onPlant(%data,%obj)
{
	Parent::onPlant(%data, %obj);

	%obj.setrendering(0);
	%obj.setcolliding(0);
	%obj.setraycasting(0);

	if(isObject(%zone = %obj.client.currAreaZone))
	{
		%simset = %zone.simset;
		%simset.add(%obj);
		%obj.setNTObjectName(%zone.ZoneName @ "_AZ_Spawn_Horde");

		if(isObject(%obj.client)) %obj.client.centerprint("\c2Placed spawn to zone <br>" @ "\c2" @ %zone.ZoneName,3);
	}
	else if(isObject(%obj.client)) %obj.client.centerprint("\c2No zone detected! Please set one first before placing a area zone.",3);
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

	if(isObject(%zone = %obj.client.currAreaZone))
	{
		%simset = %zone.simset;
		%simset.add(%obj);
		%obj.setNTObjectName(%zone.ZoneName @ "_AZ_Item_Gen");

		if(isObject(%obj.client)) %obj.client.centerprint("\c2Placed item spawn to zone <br>" @ "\c2" @ %zone.ZoneName,3);
	}
	else if(isObject(%obj.client)) %obj.client.centerprint("\c2No zone detected! Please set one first before placing an item spawn.",3);
}

function brickAreaZoneItemSpawnData::onloadPlant(%data, %obj)
{
	brickAreaZoneItemSpawnData::onPlant(%this, %obj);
}

function brickAreaZoneItemSpawnData::onDeath(%this,%obj)
{
	Parent::onDeath(%this, %obj);
}

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
		%brick.processInputEvent("onBotTeleSpawn",%brick.getgroup().client);
	}
	else
	{
		%obj.kill();
		return;
	}
}

function MiniGameSO::sortItemSpawns(%minigame,%AreaZone,%client)
{
	for(%g = 0; %g < %AreaZone.getCount(); %g++) 
	{				
		if(isObject(%brick = %AreaZone.getObject(%g)) && strstr(strlwr(%brick.getname()), "az_item_gen") != -1) %list[%l++] = %brick;		
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
				if(%minigame.survivorStatHealthAverage < %minigame.survivorStatHealthMax/4) %list[%count].setitem($L4B_Medical[getRandom(1,$L4B_MedicalAmount)]);
				else if(getRandom(1,8) == 1) %list[%count].setitem($L4B_Misc[getRandom(1,$L4B_MiscAmount)]);
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
				if(strstr(strlwr(%brick.getname()), "_grenade") != -1) %brick.setitem($L4B_Grenade[getRandom(1,$L4B_GrenadeAmount)]);
				if(strstr(strlwr(%brick.getname()), "_melee") != -1) %brick.setitem($L4B_Melee[getRandom(1,$L4B_MeleeAmount)]);
				if(strstr(strlwr(%brick.getname()), "_misc") != -1) %brick.setitem($L4B_Misc[getRandom(1,$L4B_MiscAmount)]);
				if(strstr(strlwr(%brick.getname()), "_medical") != -1) %brick.setitem($L4B_Medical[getRandom(1,$L4B_MedicalAmount)]);
				if(strstr(strlwr(%brick.getname()), "_pistolt1") != -1) %brick.setitem($L4B_PistolT1[getRandom(1,$L4B_PistolT1Amount)]);				
				if(strstr(strlwr(%brick.getname()), "_pistolt2") != -1) %brick.setitem($L4B_PistolT2[getRandom(1,$L4B_PistolT2Amount)]);
				if(strstr(strlwr(%brick.getname()), "_smgt1") != -1) %brick.setitem($L4B_SMGT1[getRandom(1,$L4B_SMGT1Amount)]);
				if(strstr(strlwr(%brick.getname()), "_shotgunt1") != -1) %brick.setitem($L4B_ShotgunT1[getRandom(1,$L4B_ShotgunT1Amount)]);
				if(strstr(strlwr(%brick.getname()), "_shotgunt2") != -1) %brick.setitem($L4B_ShotgunT2[getRandom(1,$L4B_ShotgunT2Amount)]);
				if(strstr(strlwr(%brick.getname()), "_riflet2") != -1) %brick.setitem($L4B_RifleT2[getRandom(1,$L4B_RifleT2Amount)]);				
			}
		}
	}
}

function serverCmdazhelp(%client)
{
	messageClient(%client, '', "\c7--------------------------------------------------------------------------------");
	messageClient(%client, '', "<font:Arial:25>\c6Current Area Zone:" SPC "\c4" @ %client.currAreaZone.zonename);
	messageClient(%client, '', "<font:Arial:25>\c6Current Area Zone Number:" SPC "\c4" @ %client.currAreaZone.zonenumber);
	if(isObject(%client.currAreaZone.simset)) messageClient(%client, '', "<font:Arial:25>\c6Current Area Zone Object Count:" SPC "\c4" @ %client.currAreaZone.simset.getCount());
	messageClient(%client, '', "\c7--------------------------------------------------------------------------------");
	messageClient(%client, '', "<tab:280>\c3/azcreate\c6 [\c3name\c6]\t\c6 Create a new named Area Zone.");
	messageClient(%client, '', "<tab:280>\c3/azedit\t\c6 Toggle editing the Area Zone's size or apply changes.");
	messageClient(%client, '', "<tab:280>\c3/azset\c6 [\c3name\c6]\t\c6 Set a new active area zone to edit, leave blank to clear.");
	messageClient(%client, '', "<tab:280>\c3/azsetnum\c6 [\c3name\c6]\t\c6 Sets a zone number, leave blank to clear.");
	messageClient(%client, '', "<tab:280>\c3/azclear\t\c6 Delete all Area Zones.");
	messageClient(%client, '', "<tab:280>\c3/azshow\t\c6 Toggle showing Area Zones in the world using dup selection boxes.");
	messageClient(%client, '', "<tab:280>\c3/azlist\t\c6 List all Area Zones in the world.");
	messageClient(%client, '', "<tab:280>\c3/azsave\c6 [\c3name\c6]\t\c6 Save all Area Zones to a file.");
	messageClient(%client, '', "<tab:280>\c3/azload\c6 [\c3name\c6]\t\c6 Replace all Area Zones with saved ones.");
	messageClient(%client, '', "\c7--------------------------------------------------------------------------------");
}

$EZ::AdminFailMsg = "\c6Modifying Area Zones is admin only.";

function servercmdazshow(%client)
{
	if(!%client.isAdmin){messageClient(%client, '', $EZ::AdminFailMsg); return;}
	
	if(!%client.showAreaZones)
	{
		%client.showAreaZones = true;
		showAreaZones(1);
		messageClient(%client, '', "\c6Area Zones are now visible.");
	}
	else
	{
		%client.showAreaZones = false;
		messageClient(%client, '', "\c6Area Zones are now hidden.");
		showAreaZones(0);
	}
}

function servercmdazcreate(%client, %n1, %n2, %n3, %n4, %n5)
{
	if(!%client.isAdmin){messageClient(%client, '', $EZ::AdminFailMsg); return;}
	if(!$ShowAreaZones) showAreaZones(1);

	%name = trim(%n1 SPC %n2 SPC %n3 SPC %n4 SPC %n5);
	if(!strLen(%name))
	{
		messageClient(%client, '', "\c0You have to specify a name for the new Zone!");
		return;
	}

	if(isObject(findAreaZoneByName(%name)))
	{
		messageClient(%client, '', "\c0A Zone already exists with that name!");
		return;
	}

	if(!isObject(%client.player))
	{
		messageClient(%client, '', "\c0You need to spawn before creating Zones!");
		return;
	}

	%pos = %client.player.getPosition();
	%pos = (getWord(%pos, 0) | 0) SPC (getWord(%pos, 1) | 0) SPC (getWord(%pos, 2) | 0);

	if(!%client.currAreaZoneNumber) %client.currAreaZoneNumber = 1;
	%Zone = AreaZone(%name);
	%Zone.setSize(vectorAdd(%pos, "-5 -5 0"), vectorAdd(%pos, "5 5 10"));
	%zone.zonenumber = %client.currAreaZoneNumber++;
	%client.currAreaZone = %Zone;
	messageClient(%client, '', "\c6A new Zone has been created somewhere around your player.");
	%client.player.setTransform(%client.player.getTransform());
}

function deleteAllAreaZones() { while(AreaZoneGroup.getCount()) AreaZoneGroup.getObject(0).delete(); }

function servercmdazclear(%client)
{
	if(!%client.isAdmin){messageClient(%client, '', $EZ::AdminFailMsg); return;}

	if(!AreaZoneGroup.getCount())
	{
		messageClient(%client, '', "\c0There are no Zones to delete!");
		return;
	}

	%client.currAreaZone = 0;
	deleteAllAreaZones();
	messageClient(%client, '', "\c6All Area Zones have been deleted.");
}

function servercmdazlist(%client)
{
	if(!%client.isAdmin){messageClient(%client, '', $EZ::AdminFailMsg); return;}

	if(!AreaZoneGroup.getCount())
	{
		messageClient(%client, '', "\c0There are no Zones to list!");
		return;
	}
	
	messageClient(%client, '', "\c6The following Zones exist in the world:");

	for(%i = 0; %i < AreaZoneGroup.getCount(); %i++)
	{
		%Zone = AreaZoneGroup.getObject(%i);
		messageClient(%client, '', "\c6" @ %Zone.ZoneName SPC "\c6Number " @ %zone.zonenumber);
	}
}

function servercmdazset(%client, %n1, %n2, %n3, %n4, %n5)
{
	if(!%client.isAdmin){messageClient(%client, '', $EZ::AdminFailMsg); return;}

	%name = trim(%n1 SPC %n2 SPC %n3 SPC %n4 SPC %n5);
	if(isObject(%zone = findAreaZoneByName(%name)))
	{
		if(isObject(%client.currAreaZone))
		{
			%client.currAreaZone.stopEdit();
			%client.isEditingAreaZone = false;
		}

		%client.currAreaZone = %zone;
		messageClient(%client, '', "\c6Set new active zone \c6" @ %zone.zonename);
	}
	else
	{
		messageClient(%client, '', "\c0There is no Zone with that name!");
		if(isObject(%client.currAreaZone))
		{			
			messageClient(%client, '', "\c0Cleared currently active zone\c0" SPC %client.currAreaZone.zonename SPC "\c0(does not delete)");

			%client.isEditingAreaZone = false;
			%client.currAreaZone.stopEdit();
			%client.currAreaZone = 0;
		}			
		return;
	}
}

function servercmdazsetnum(%client, %name, %n1)
{
	if(!%client.isAdmin){messageClient(%client, '', $EZ::AdminFailMsg); return;}

	if(isObject(%zone = findAreaZoneByName(%name)))
	{
		if(%n1)
		{
			%zone.zonenumber = %n1;
			messageClient(%client, '', "\c0Set zone number to \c0" @ %n1);
		}
		else 
		{
			%zone.zonenumber = "";
			messageClient(%client, '', "\c0Cleared zone number");
		}
	}
	else messageClient(%client, '', "\c0There is no Zone with that name!");
}

function servercmdazedit(%client, %type)
{
	if(!%client.isAdmin){messageClient(%client, '', $EZ::AdminFailMsg); return;}
	

	if(isObject(%client.currAreaZone))
	{
		if(!$ShowAreaZones) showAreaZones(1);

		if(!%client.isEditingAreaZone)
		{
			%client.currAreaZone.startEdit(%client);
			messageClient(%client, '', "\c6Started editing the Zone. Use ghost brick controls, like with a new duplicator selection box.");
			%client.isEditingAreaZone = true;
		}
		else
		{
			%client.currAreaZone.stopEdit();
			messageClient(%client, '', "\c6Stopped editing the Zone.");
			%client.isEditingAreaZone = false;			
		}
	}
	else
	{
		messageClient(%client, '', "\c6No currently active zone selected!");
		messageClient(%client, '', "\c6Make a new zone or set a new active zone! Type /azhelp for commands");
	}
}

function servercmdazsave(%client, %f0, %f1, %f2, %f3, %f4, %f5, %f6, %f7)
{
	if(!%client.isAdmin){messageClient(%client, '', $EZ::AdminFailMsg); return;}
	if(!$ShowAreaZones) showAreaZones(1);

	%fileName = trim(%f0 SPC %f1 SPC %f2 SPC %f3 SPC %f4 SPC %f5 SPC %f6 SPC %f7);
	if(!AreaZoneGroup.getCount() || !strLen(%fileName))
	{
		if(!AreaZoneGroup.getCount()) messageClient(%client, '', "\c0There are no Zones to save!");
		if(!strLen(%fileName)) messageClient(%client, '', "\c0You have to specify a name for the save file!");
		return;
	}

	%allowed = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ._-()";
	%filePath = "config/server/Left4Block/MapAreaZones/" @ %fileName @ ".txt";

	for(%i = 0; %i < strLen(%fileName); %i++)
	if(strStr(%allowed, getSubStr(%fileName, %i, 1)) == -1)
	{
		%forbidden = true;
		break;
	}
	
	if(%forbidden || !strLen(%fileName) || strLen(%fileName) > 50)
	{
		messageClient(%client, '', "\c0The file name contains invalid characters, try again!");
		return;
	}

	if(isFile(%filePath)) fileDelete(%filePath);
	exportAreaZones(%filePath);
	messageClient(%client, '', "\c6All Area Zones have been saved.");
}

function exportAreaZones(%filename)
{
	%file = new FileObject();
	if(!%file.openForWrite(%filename)) return false;

	for(%i = 0; %i < AreaZoneGroup.getCount(); %i++)
	{
		%Zone = AreaZoneGroup.getObject(%i);
		%file.writeLine(%Zone.ZoneName TAB %zone.zonenumber TAB %Zone.point1 TAB %Zone.point2);
	}

	%file.delete();
	return true;
}

function serverCmdazload(%client, %f0, %f1, %f2, %f3, %f4, %f5, %f6, %f7)
{
	if(!%client.isAdmin){messageClient(%client, '', $EZ::AdminFailMsg); return;}
	if(!$ShowAreaZones) showAreaZones(1);

	%fileName = trim(%f0 SPC %f1 SPC %f2 SPC %f3 SPC %f4 SPC %f5 SPC %f6 SPC %f7);
	if(!strLen(%fileName))
	{
		messageClient(%client, '', "\c0You have to specify a name for the save file to load!");
		return;
	}

	%allowed = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ._-()";
	%filePath = "config/server/Left4Block/MapAreaZones/" @ %fileName @ ".txt";

	for(%i = 0; %i < strLen(%fileName); %i++)
	{
		if(strStr(%allowed, getSubStr(%fileName, %i, 1)) == -1)
		{
			%forbidden = true;
			break;
		}
	}

	if(!isFile(%filePath) || %forbidden || !strLen(%fileName) || strLen(%fileName) > 50)
	{
		if(%forbidden || !strLen(%fileName) || strLen(%fileName) > 50) messageClient(%client, '', "\c0The file name contains invalid characters, try again!");
		if(!isFile(%filePath)) messageClient(%client, '', "\c0There is no save file with that name!");
		return;
	}

	loadAreaZones(%filePath);
	messageClient(%client, '', "\c6All Area Zones have been loaded.");
}

function serverazload(%fileName)
{
	if(!strLen(%fileName)) return;

	%allowed = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ._-()";
	%filePath = "config/server/Left4Block/MapAreaZones/" @ %fileName @ ".txt";

	for(%i = 0; %i < strLen(%fileName); %i++)
	{
		if(strStr(%allowed, getSubStr(%fileName, %i, 1)) == -1)
		{
			%forbidden = true;
			break;
		}
	}

	if(!isFile(%filePath) || %forbidden || !strLen(%fileName) || strLen(%fileName) > 50) return;	
	loadAreaZones(%filePath);
}

function loadAreaZones(%filename)
{
	deleteAllAreaZones();
	%file = new FileObject();
	if(!%file.openForRead(%filename)) return false;
	
	while(!%file.isEOF())
	{
		%line = %file.readLine();
		%name = getField(%line,0);
		%zonenumber = getField(%line,1);
		%point1 = getField(%line,2);
		%point2 = getField(%line,3);
		%Zone = AreaZone(%name);
		%Zone.zonenumber = %zonenumber;
		%Zone.setSize(%point1, %point2);
		%Zone.updateShapeName();
	}

	%file.delete();
	return true;
}

function findAreaZoneByName(%name)
{
	for(%i = 0; %i < AreaZoneGroup.getCount(); %i++)
	{
		%Zone = AreaZoneGroup.getObject(%i);
		if(%Zone.ZoneName $= %name) return %Zone.getId();
	}
}

datablock TriggerData(AreaZoneTrigger)
{
	tickPeriodMS = 250;
};

function AreaZoneTrigger::onEnterTrigger(%this, %trigger, %obj)
{
	if(%obj.getType() & $TypeMasks::PlayerObjectType && %obj.getState() !$= "Dead" && %obj.getdataBlock().isSurvivor && isObject(%minigame = getMiniGameFromObject(%obj)))
	{	
		%obj.currentZone = %trigger;
		%obj.currentZoneNumber = %trigger.zone.zonenumber;
		%trigger.presencecount++;
		%simset = %trigger.simset;

		if(%simset.getcount())
		for(%i = 0; %i < %simset.getcount(); %i++)
		{					
		    if(isObject(%setbricks = %simset.getObject(%i)))
		    if(!isObject(MainAreaZone)) 
		    {
		        new SimSet(MainAreaZone);
		        missionCleanup.add(MainAreaZone);
		    }
		    else if(!MainAreaZone.isMember(%setbricks)) MainAreaZone.add(%setbricks);										
		}
		
		if(!%trigger.firstentry)
		{
			%trigger.firstentry = true;

		    //$InputTarget_["Self"] = %brick.areazone.parbrick;
		    //switch$(%obj.getclassname())
		    //{
		    //    case "Player":	$InputTarget_["Player"] = %obj;
		    //                    $InputTarget_["Client"] = %obj.client;
		    //    case "AIPlayer": $InputTarget_["Bot"] = %obj;
		    //}
		    //$InputTarget_["MiniGame"] = getMiniGameFromObject(%obj);
		    //%brick.areazone.parbrick.processInputEvent("onAZFirstEntry",%obj.client);		    
		
		    for(%g = 0; %g < %simset.getCount(); %g++) 
			if(isObject(%itembricks = %simset.getObject(%g)) && strstr(strlwr(%itembricks.getname()),"_item") != -1)
			{		            
				%minigame.schedule(25,sortItemSpawns,%simset);
				break;
			}

		    for(%h = 0; %h < %simset.getCount(); %h++) 
			if(isObject(%spawnbricks = %simset.getObject(%h)) && strstr(strlwr(%spawnbricks.getname()),"_wander") != -1)
			{		            
				%minigame.schedule(25,spawnZombies,"Wander",getRandom(15,25),%simset);
				break;
			}
		}
	}
}

function AreaZoneTrigger::onLeaveTrigger(%this, %trigger, %obj)
{
	if(%obj.getType() & $TypeMasks::PlayerObjectType && %obj.getdataBlock().isSurvivor)
	{
		%trigger.presencecount--;
		%obj.currentZone = 0;
	}

	if(%trigger.presencecount <= 0)
	{
		%trigger.presencecount = 0;
		%simset = %trigger.simset;
		for(%i = 0; %i < %simset.getcount(); %i++) if(isObject(%setbricks = %simset.getObject(%i)) && isObject(MainAreaZone) && MainAreaZone.isMember(%setbricks)) MainAreaZone.remove(%setbricks);            	
	}
}

function AreaZone(%name)
{
	AreaZoneGroup.add(%this = new ScriptObject(AreaZone));
	
	%this.zonename = %name;	
	%this.simset = new SimSet(%name @ "_simset");
	%this.trigger = new Trigger()
	{
		datablock = AreaZoneTrigger;
		polyhedron = "-0.5 -0.5 -0.5 1 0 0 0 1 0 0 0 1";
	};
	%this.trigger.zone = %this;
	%this.trigger.simset = %this.simset;

	%this.editBox = ND_SelectionBox();
	%this.editBox.setDisabledMode();
	%this.updateShapeName();

	return %this;
}

function AreaZone::onRemove(%this)
{
	%this.trigger.delete();
	%this.simset.delete();
	if(isObject(%this.editBox)) %this.editBox.delete();
}

function AreaZone::setSize(%this, %point1, %point2)
{
	if(getWordCount(%point1) == 6)
	{
		%point2 = getWords(%point1, 3, 5);
		%point1 = getWords(%point1, 0, 2);
	}
	%this.point1 = %point1;
	%this.point2 = %point2;
	if(isObject(%this.editBox)) %this.editBox.setSize(%point1, %point2);
	%size = vectorSub(%point2, %point1);
	%center = vectorAdd(%point1, vectorScale(%size, 0.5));
	%this.trigger.setTransform(%center @ " 1 0 0 0");
	%this.trigger.setScale(%size);
}

function AreaZone::startEdit(%this, %client)
{
	if(%this.editClient)
	{
		echo("ERROR: Area Zone already has an editing client");
		return false;
	}

	if(!$ShowAreaZones) showAreaZones(true);
	%this.editClient = %client;
	%client.AreaEditZone = %this;
	%this.editBox.setSizeAligned(%this.point1, %this.point2, %client.getControlObject());
	%this.editBox.setNormalMode();
	%this.updateShapeName();
}

function AreaZone::stopEdit(%this)
{
	if(!%this.editClient)
	{
		echo("ERROR: Area Zone does not have an editing client");
		return false;
	}

	%this.editClient.AreaEditZone = "";
	%this.editClient = "";
	%this.editBox.setDisabledMode();
	%this.setSize(%this.editBox.getWorldBox());
	%this.updateShapeName();
}

function AreaZone::updateShapeName(%this)
{
	%this.editBox.shapeName.setShapeNameColor(%this.editBox.borderColor);
	if(isObject(%this.editClient)) %editor = %this.editClient.name @ " editing ";
	%this.editBox.shapeName.setShapeName(%editor @ "Area Zone \"" @ %this.ZoneName @ "\"");
}

function showAreaZones(%bool)
{
	$ShowAreaZones = %bool;
	%count = AreaZoneGroup.getCount();

	for(%i = 0; %i < %count; %i++)
	{
		%Zone = AreaZoneGroup.getObject(%i);

		if(%bool && !isObject(%Zone.editBox))
		{
			%Zone.editBox = ND_SelectionBox("Area Zone \"" @ %Zone.ZoneName @ "\"");
			%Zone.editBox.setDisabledMode();
			%Zone.editBox.setSize(%Zone.point1, %Zone.point2);
			%Zone.updateShapeName();
		}
		else if(!%bool && isObject(%Zone.editBox))
		{
			if(isObject(%Zone.editClient)) %Zone.stopEdit();			
			%Zone.editBox.delete();
		}
	}
}
