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

	exec("./support/jettison.cs");
	exec("./support/clientlogger.cs");

	if(LoadRequiredAddOn("Support_BotHolePlus") == $Error::None)
	//exec("Add-Ons/Support_BotHolePlus/server.cs");
	exec("./support/afk_system.cs");
	exec("./bots/survivors/bot_survivor.cs");
	
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