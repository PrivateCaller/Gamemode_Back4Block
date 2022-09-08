function configLoadL4BTXT(%file,%svartype)//Set up custom variables
{
	%read = new FileObject();
	if(!isFile("config/server/Left4Block/" @ %file @ ".txt"))
	{
		%read.openForRead("add-ons/gamemode_left4block/config/" @ %file @ ".txt");

		%write = new FileObject();
		%write.openForWrite("config/server/Left4Block/" @ %file @ ".txt");
	
		while(!%read.isEOF())
		{
			%line = %read.readLine();
			%write.writeLine(%line);
		}

		%write.close();
		%write.delete();
	}

	%read.openForRead("config/server/Left4Block/" @ %file @ ".txt");

	while(!%read.isEOF())
	{
		%i++;
		%line = %read.readLine(); 
		eval("$" @ %svartype @"[%i] = \"" @ %line @ "\";");
		eval("$" @ %svartype @"Amount = %i;");
	}
	
	%read.close();
	%read.delete();
}

function configLoadL4BItemScavenge()//Set up items
{
	%read = new FileObject();
	if(!isFile("config/server/Left4Block/itemscavenge.txt"))
	{
		%read.openForRead("add-ons/gamemode_left4block/config/itemscavenge.txt");

		%write = new FileObject();
		%write.openForWrite("config/server/Left4Block/itemscavenge.txt");
	
		while(!%read.isEOF())
		{
			%line = %read.readLine();
			%write.writeLine(%line);
		}

		%write.close();
		%write.delete();
	}

	%read.openForRead("config/server/Left4Block/itemscavenge.txt");

	while(!%read.isEOF())
	{
		%i++;
		%line = %read.readLine(); 

		%itemremoveword = strreplace(%line, getWord(%line,0) @ " ", "");
		%previousline[%i] = getWord(%line,0);

		if(%previousline[%i] $= %previousline[mClamp(%i-1, 1, %i)])
		{
			%j++;
			eval("$" @ getWord(%line,0) @"[%j] = \"" @ %itemremoveword @ "\";");
			eval("$" @ getWord(%line,0) @"Amount = %j;");
		}
		else 
		{
			eval("$" @ getWord(%line,0) @"[1] = \"" @ %itemremoveword @ "\";");
			%j = 1;
		}

		for (%d = 0; %d < DatablockGroup.getCount(); %d++) 
		{
			%datablock = DatablockGroup.getObject(%d);

			if(%datablock.getClassName() $= "ItemData")
			if(%datablock.uiName $= %itemremoveword)
			{	
				%item = %datablock;
				eval("$" @ getWord(%line,0) @"[%j] = \"" @ %item.getName() @ "\";");
			}
		}
	}
	%read.close();
	%read.delete();
}

function configLoadL4BItemSlots()
{
	%read = new FileObject();
	if(!isFile("config/server/Left4Block/itemslots.txt"))
	{
		%read.openForRead("Add-Ons/Gamemode_Left4Block/config/itemslots.txt");
		%write = new FileObject();
		%write.openForWrite("config/server/Left4Block/itemslots.txt");
		
		while(!%read.isEOF())
		{
			%line = %read.readLine();
			%write.writeLine(%line);
		}
		
		%read.close();
		%write.close();
		%write.delete();
	}
	
	%read.openForRead("config/server/Left4Block/itemslots.txt");
	
	while(!%read.isEOF())
	{
		%i++;
		%line = %read.readLine(); 

		%firstword = getWord(%line,0);
		%itemremoveword = strreplace(%line, %firstword @ " ", "");

		for (%d = 0; %d < DatablockGroup.getCount(); %d++) 
		{
			%datablock = DatablockGroup.getObject(%d);

			if(%datablock.getClassName() $= "ItemData" && %datablock.uiName $= %itemremoveword)
			%datablock.L4Bitemslot = %firstword;
		}
	}
	%read.close();
	%read.delete();
}

function configLoadL4BItemNoSlots()
{
	%read = new FileObject();
	if(!isFile("config/server/Left4Block/itemnoslots.txt"))
	{
		%read.openForRead("Add-Ons/Gamemode_Left4Block/config/itemnoslots.txt");
		%write = new FileObject();
		%write.openForWrite("config/server/Left4Block/itemnoslots.txt");
		
		while(!%read.isEOF())
		{
			%line = %read.readLine();
			%write.writeLine(%line);
		}
		
		%read.close();
		%write.close();
		%write.delete();
	}
	
	%read.openForRead("config/server/Left4Block/itemnoslots.txt");
	
	while(!%read.isEOF())
	{
		%i++;
		%line = %read.readLine(); 

		for (%d = 0; %d < DatablockGroup.getCount(); %d++) 
		{
			%datablock = DatablockGroup.getObject(%d);

			if(%datablock.getClassName() $= "ItemData" && %datablock.uiName $= %line)
			%dataBlock.L4Bitemnoslot = 1;
		}
	}
	%read.close();
	%read.delete();
}

if(loadRequiredAddOn("Bot_Hole") == $Error::None && loadRequiredAddOn("Support_Lua") == $Error::None)
{	
	exec("./preferences.cs");	
	exec("./add-ins/support_packages/extraResources.cs");
	exec("./add-ins/player_survivor/script_newplayerdatablock.cs");
	exec("./add-ins/support_packages/support_mainpackage.cs");
	exec("./add-ins/support_packages/support_multipleslots.cs");
	exec("./add-ins/support_packages/support_jettison.cs");
	exec("./add-ins/support_packages/support_clientlogger.cs");
	exec("./add-ins/player_survivor/script_survivor.cs");

	if(isFunction(NetObject, setNetFlag))
	{
		exec("./add-ins/support_packages/support_billboards/billboards/billboards.cs");
		exec("./add-ins/support_packages/support_billboards/billboards_wrapper.cs");
		$L4B_hasSelectiveGhosting = true;
	}
	else
	{
		$L4B_hasSelectiveGhosting = false;
		error("ERROR: The Selective Ghosting DLL is required for Gamemode_Left4Block's billboards to work.");
		schedule(1000, 0, messageAll, 'MsgError', "\c0ERROR: The Selective Ghosting DLL is required for Gamemode_Left4Block's billboards to work.");
	}

	if(LoadRequiredAddOn("Support_BotHolePlus") == $Error::None)
	{
		exec("./add-ins/bot_l4b/bots/survivor.cs");
		exec("./add-ins/support_packages/support_afk_system.cs");
	}
	
	if(LoadRequiredAddOn("Weapon_FlashGrenade") == $Error::None)
	exec("./add-ins/support_packages/support_flashbang.cs");
	
	if(LoadRequiredAddOn("Weapon_SWeps_FlareGun") == $Error::None && LoadRequiredAddOn("Weapon_SWeps") == $Error::None)
	exec("./add-ins/support_packages/support_sweps_flaregun.cs");

	exec("./add-ins/script_director/director.cs");
	exec("./add-ins/script_director/areazones.cs");
	exec("./add-ins/bot_l4b/bot_l4b.cs");
	exec("./add-ins/item_healing/item_healing.cs");
	exec("./add-ins/script_secondary_melee/script_melee.cs");
	exec("./add-ins/weapon_dav_melee/weapon_melee.cs");
	exec("./add-ins/weapon_distractions/weapon_distractions.cs");
	exec("./add-ins/weapon_sweps_molotov/weapon_sweps_molotov.cs");
	exec("./add-ins/weapon_sweps_molotov/support_sweps_ext_flames.cs");
	exec("./add-ins/weapon_throwable_explosives/weapon_throwable_explosives.cs");
	exec("./add-ins/weapon_rocks/weapon_rocks.cs");
	exec("./add-ins/brick_interactive/brick_interactive.cs");

	configLoadL4BTXT("zombiefaces",hZombieFace);
	configLoadL4BTXT("zombiedecals",hZombieDecal);
	configLoadL4BTXT("zombieskin",hZombieSkin);
	configLoadL4BTXT("zombiespecial",hZombieSpecialType);
	configLoadL4BTXT("zombieuncommon",hZombieUncommonType);
	configLoadL4BItemScavenge();
	configLoadL4BItemSlots();
	configLoadL4BItemNoSlots();
}