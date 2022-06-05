if(loadRequiredAddOn("Bot_Hole") == $Error::None)
{
	exec("./support/billboards/billboards.cs");
	if(isFunction(NetObject, setNetFlag))
	{
		exec("./support/billboards_wrapper.cs");
		$L4B_hasSelectiveGhosting = true;
	}
	else
	{
		$L4B_hasSelectiveGhosting = false;
		error("ERROR: The Selective Ghosting DLL is required for Package_Left4Block's billboards to work.");
		schedule(1000, 0, messageAll, 'MsgError', "\c0ERROR: The Selective Ghosting DLL is required for Package_Left4Block's billboards to work.");
	}
	exec("./support/afk_system.cs");
	$L4B_clientLog = new ScriptGroup() {};
	MissionCleanup.add($L4B_clientLog);
	function L4B_loadClientSnapshots()
	{
		echo("Loading saved clients...");
		if(!isFile("config/server/L4B2_Bots/loggedplayers.txt"))
		{
			echo("None found.");
			return;
		}
		%file = new fileObject();
		%file.openForRead("config/server/L4B2_Bots/loggedplayers.txt");
		while(!%file.isEOF())
		{
			%client_info = %file.readLine();
			%client_object = new ScriptObject() 
			{
				//It is important that these are "getRecord" and not "getWord".
				name = getRecord(%client_info, 0);
				blid = getRecord(%client_info, 1);
	
				accent = getRecord(%client_info, 2);
				hat = getRecord(%client_info, 3);
				chest = getRecord(%client_info, 4);
				decalName = getRecord(%client_info, 5);
				pack =  getRecord(%client_info, 6);
				secondPack = getRecord(%client_info, 7);
				larm = getRecord(%client_info, 8);
				lhand = getRecord(%client_info, 9);
				rarm = getRecord(%client_info, 10);
				rhand = getRecord(%client_info, 11);
				hip = getRecord(%client_info, 12);
				lleg = getRecord(%client_info, 13);
				rleg = getRecord(%client_info, 14);
	
				accentColor = getRecord(%client_info, 15);
				hatColor = getRecord(%client_info, 16);
				packColor = getRecord(%client_info, 17);
				secondPackColor = getRecord(%client_info, 18);
	
				skinColor = getRecord(%client_info, 19);
			};
			$L4B_clientLog.add(%client_object);
		}
		%file.close();
		%file.delete();
		echo($L4B_clientLog.getCount() SPC "clients found.");
	}
	L4B_loadClientSnapshots();
	exec("./bots/survivors/bot_survivor.cs");
	
	exec("./scripts/script_newplayerdatablock.cs");
	exec("./datablocks.cs");
	exec("./preferences.cs");
	exec("./variables.cs");

	exec("./support/packages.cs");
	
	if(LoadRequiredAddOn("Weapon_FlashGrenade") == $Error::None)
	exec("./support/support_flashbang.cs");
	
	if(LoadRequiredAddOn("Weapon_SWeps_EXT") == $Error::None)
	exec("./support/support_sweps_ext_flames.cs");
	
	if(LoadRequiredAddOn("Weapon_SWeps_FlareGun") == $Error::None && LoadRequiredAddOn("Weapon_SWeps") == $Error::None)
	exec("./support/support_sweps_flaregun.cs");

	exec("./functions.cs");
	exec("./scripts/script_survivor.cs");
	exec("./scripts/script_scavenge.cs");
	exec("./scripts/script_director.cs");
	exec("./scripts/script_radiusteleporting.cs");
	exec("./scripts/script_getitemfrominventory.cs");
	exec("./scripts/script_melee.cs");
	exec("./scripts/script_events.cs");
	exec("./scripts/items/weapon_rocks.cs");
	exec("./scripts/items/weapon_melee.cs");
	exec("./scripts/items/item_healing.cs");
	exec("./scripts/items/weapon_throwableexplosives.cs");
	exec("./scripts/items/weapon_distractions.cs");
	
	exec("./bots/zombies/common.cs");
	exec("./bots/zombies/uncommon_soldier.cs");
	exec("./bots/zombies/uncommon_fallen.cs");
	exec("./bots/zombies/uncommon_mud.cs");
	exec("./bots/zombies/uncommon_construction.cs");
	exec("./bots/zombies/uncommon_clown.cs");
	exec("./bots/zombies/uncommon_jimmy_gibbs.cs");
	exec("./bots/zombies/uncommon_ceda.cs");
	exec("./bots/zombies/uncommon_toxic.cs");
	exec("./bots/zombies/uncommon_pirate.cs");
	exec("./bots/zombies/boomer.cs");
	exec("./bots/zombies/charger.cs");
	exec("./bots/zombies/hunter.cs");
	exec("./bots/zombies/jockey.cs");
	exec("./bots/zombies/smoker.cs");
	exec("./bots/zombies/spitter.cs");
	exec("./bots/zombies/tank.cs");
	exec("./bots/zombies/witch.cs");
}