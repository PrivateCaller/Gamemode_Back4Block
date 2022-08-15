if(loadRequiredAddOn("Bot_Hole") == $Error::None)
{	
	exec("./scripts/script_newplayerdatablock.cs");
	exec("./scripts/datablocks.cs");
	exec("./scripts/preferences.cs");
	exec("./scripts/functions.cs");
	exec("./support/packages.cs");
	
	if(LoadRequiredAddOn("Weapon_FlashGrenade") == $Error::None)
	exec("./support/support_flashbang.cs");
	
	if(LoadRequiredAddOn("Weapon_SWeps_EXT") == $Error::None)
	exec("./support/support_sweps_ext_flames.cs");
	
	if(LoadRequiredAddOn("Weapon_SWeps_FlareGun") == $Error::None && LoadRequiredAddOn("Weapon_SWeps") == $Error::None)
	exec("./support/support_sweps_flaregun.cs");

	exec("./scripts/script_director.cs");
	exec("./scripts/script_radiusteleporting.cs");
	exec("./add-ins/player_survivor/script_survivor.cs");
	exec("./add-ins/bot_l4b/bot_l4b.cs");
	exec("./add-ins/item_healing/item_healing.cs");
	exec("./add-ins/script_secondary_melee/script_melee.cs");
	exec("./add-ins/weapon_dav_melee/weapon_melee.cs");
	exec("./add-ins/weapon_distractions/weapon_distractions.cs");
	exec("./add-ins/weapon_throwable_explosives/weapon_throwable_explosives.cs");
	exec("./add-ins/weapon_rocks/weapon_rocks.cs");

	configLoadL4BTXT("zombiefaces",hZombieFace);
	configLoadL4BTXT("zombiedecals",hZombieDecal);
	configLoadL4BTXT("zombieskin",hZombieSkin);
	configLoadL4BTXT("zombiespecial",hZombieSpecialType);
	configLoadL4BTXT("zombieuncommon",hZombieUncommonType);
	configLoadL4BItemTXT();

	exec("./support/jettison.cs");
	exec("./support/clientlogger.cs");
	// if(isFunction(NetObject, setNetFlag))
	// {
	// 	exec("./support/billboards/billboards.cs");
	// 	exec("./support/billboards_wrapper.cs");
	// 	$L4B_hasSelectiveGhosting = true;
	// }
	// else
	// {
	// 	$L4B_hasSelectiveGhosting = false;
	// 	error("ERROR: The Selective Ghosting DLL is required for Gamemode_Left4Block's billboards to work.");
	// 	schedule(1000, 0, messageAll, 'MsgError', "\c0ERROR: The Selective Ghosting DLL is required for Package_Left4Block's billboards to work.");
	// }
	// if(LoadRequiredAddOn("Support_BotHolePlus") == $Error::None)
	exec("./support/support_holebothooks.cs");
	exec("./add-ins/bot_l4b/bots/survivor.cs");
	exec("./support/afk_system.cs");
}