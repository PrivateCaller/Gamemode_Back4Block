//Just integrating Swollow's old scavenge mod, might make some changes to it
package swol_scavenge
{
	function serverCmdReloadScavenge(%client)
	{
		if(!%client.isSuperAdmin)
		return;
		if(getSimTime()-%client.lastScavengeCommand < 5000)
		return;
		
		setModPaths(getModPaths());
		if(%client.lastScavengeCommand $= "")
		messageClient(%client,'',"\c6Please note: you must restart for new events to show up");
		messageAll('',"\c3" @ %client.getPlayerName() @ " \c0reloaded scavenge files");
		%client.lastScavengeCommand = getSimTime();
		swol_updateScavenge();
	}

	function swol_iniScavenge()
	{
		setModPaths(getModPaths());
		new scriptObject(_Swol_ScavengeMainClass)
		{
			scavengeGroups = 0;
			scavengeDelCnt = 0;
		};
		swol_loadAllScavengeFiles();
	}

	function swol_updateScavenge()
	{
		new scriptObject(_Swol_ScavengeMainClass)
		{
			scavengeGroups = 0;
			scavengeDelCnt = 0;
		};
		swol_updateAllScavengeFiles();
	}

	function swol_addScavengeEvents(%name,%cl)
	{
		registerOutputEvent(fxDTSBrick, "setScavenge" @ %name);
		eval("function fxDTSBrick::" @ "setScavenge" @ %name @ "(%this) { %this.setScavengeRand(\"" @ %name @ "\"); }");
	}
	
	function fxDTSBrick::setScavengeRand(%this,%name)
	{
		%d = swol_chooseRandomItem(%name);
		
		if(%d $= "none" || !isObject(swol_parseScavengeUi(%d)))
		%this.setItem(swol_parseScavengeUi("None"));
		else %this.setItem(swol_parseScavengeUi(%d));
	}
	function swol_updateAllScavengeFiles()
	{
		%pattern = "config/server/L4B2_Bots/scavenge/*.txt";
		%file = findFirstFile(%pattern);
		while(%file !$= "")
		{
			%nfix = strreplace(%file, "config/server/L4B2_Bots/scavenge/", "");
			%n = strreplace(%nfix, ".txt", "");
			swol_loadScavangeFile(%file,%n);
			%file = findNextFile(%pattern);
		}
	}
	function swol_loadAllScavengeFiles()
	{
		%pattern = "config/server/L4B2_Bots/scavenge/*.txt";

		%file = findFirstFile(%pattern);
		while(%file !$= "")
		{
			%nfix = strreplace(%file, "config/server/L4B2_Bots/scavenge/", "");
			%nfix2 = strreplace(%nfix, ".txt", "");

			swol_loadScavangeFile(%file,%nfix2);
			swol_addScavengeEvents(%nfix2);
			%file = findNextFile(%pattern);
		}
	}
	function swol_iniScavengeClass(%name)
	{
		%n = "Swol_Scavenge" @ %name;
		new scriptObject(%n)
		{
			count = 0;
		};
		_Swol_ScavengeMainClass.group[%name] = 1;
		_Swol_ScavengeMainClass.delTmp[_Swol_ScavengeMainClass.scavengeDelCnt] = %name;
		_Swol_ScavengeMainClass.scavengeGroups++;
		_Swol_ScavengeMainClass.scavengeDelCnt++;
	}
	function swol_addToScavengeClass(%name,%item)
	{
		%n = "Swol_Scavenge" @ %name;
		%count = %n.count;
		%n.item[%count] = %item;
		%n.count++;
	}
	function swol_clearScavengeClass(%name)
	{
		%n = "Swol_Scavenge" @ %name;
		%n.delete();
		_Swol_ScavengeMainClass.scavengeGroups--;
		_Swol_ScavengeMainClass.removeClass(%name);
	}
	function swol_getItemScavengeClass(%name,%id)
	{
		%name = "Swol_Scavenge" @ %name;
		if(%id>%name.count)
		return;
		
		return %name.item[%id];
	}
	function swol_getItemCount(%name)
	{
		%name = "Swol_Scavenge" @ %name;
		return %name.count;
	}
	function swol_chooseRandomItem(%name)
	{
		%rand = getRandom(0,swol_getItemCount(%name)-1);
		return swol_getItemScavengeClass(%name,%rand);
	}
	function swol_parseScavengeUi(%text)
	{
		%d = $uiNameTable_Items[%text];
		if(isObject(%d))
		return %d;
		return -1;
	}
	function swol_loadScavangeFile(%file,%name)
	{
		if(!isFile(%file))
		return error("SCAVENGE MOD ERROR: file does not exist");
		
		if(_Swol_ScavengeMainClass.classExist(%name))
		swol_clearScavengeClass(%name);

		swol_iniScavengeClass(%name);
		%f = new fileObject();
		%f.openForRead(%file);
		%count = 0;
		%rarityMode = 0;
		while(!%f.isEOF())
		{
			%l = %f.readLine();
			if(!%count)
			{
				if(getWord(%l,0) $= ">MODE")
				{
					if(getWord(%l,1) $= "rarity")
					%rarityMode = 1;

					continue;
				}
			}
			if(%rarityMode)
			{
				%adds = getWord(%l,0);
				if(!%adds) %adds = 1;
				for(%i=0;%i<%adds;%i++)
				swol_addToScavengeClass(%name,getWords(%l,1,getWordCount(%l)-1));
			}
			else
			{
				swol_addToScavengeClass(%name,%l);
			}
			%count++;
		}
		%f.close();
		%f.delete();
		echo("Scavenge Mod Loaded " @ %file @ " (" @ %name @ ")");
	}
	function _Swol_ScavengeMainClass::removeClass(%this,%name)
	{
		%this.group[%name] = "";
	}
	function _Swol_ScavengeMainClass::classExist(%this,%name)
	{
		if(%this.group[%name] !$= "")
		return true;
		return false;
	}
	function disconnect(%a)
	{
		for(%i=0;%i<_Swol_ScavengeMainClass.scavengeDelCnt;%i++)
		{
			%n = "Swol_Scavenge" @ _Swol_ScavengeMainClass.delTmp[%i];
			if(isObject(%n))
			%n.delete();
		}
		_Swol_ScavengeMainClass.delete();
		return parent::disconnect(%a);
	}
};
activatePackage(swol_scavenge);

if(!isObject(_Swol_ScavengeMainClass))
swol_iniScavenge();