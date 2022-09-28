if(loadRequiredAddOn("Bot_Hole") == $Error::None && loadRequiredAddOn("Support_Lua") == $Error::None)
{	
	exec("./preferences.cs");	
	
	//Packages
	if(LoadRequiredAddOn("Weapon_FlashGrenade") == $Error::None) exec("./add-ins/support_packages/support_flashbang.cs");
	exec("./add-ins/support_packages/support_extraresources.cs");
	exec("./add-ins/support_packages/support_mainpackage.cs");
	exec("./add-ins/support_packages/support_multipleslots.cs");
	exec("./add-ins/support_packages/support_leadprojectiles.cs");
	exec("./add-ins/support_packages/support_jettison.cs");
	exec("./add-ins/support_packages/support_clientlogger.cs");

	//if(isFunction(NetObject, setNetFlag))
	//{
	//	exec("./add-ins/support_billboards/billboards/billboardv2.cs");
	//	exec("./add-ins/support_billboards/billboards_wrapper.cs");
	//	$L4B_hasSelectiveGhosting = true;
	//}

	exec("./add-ins/player_l4b/player_l4b.cs");
	exec("./add-ins/script_secondary_melee/script_melee.cs");
	exec("./add-ins/server_rblood/server_rblood.cs");
	exec("./add-ins/script_director/director.cs");
	exec("./add-ins/script_director/areazones.cs");
	exec("./add-ins/item_healing/item_healing.cs");
	exec("./add-ins/weapon_dav_melee/weapon_melee.cs");
	exec("./add-ins/weapon_distractions/weapon_distractions.cs");
	exec("./add-ins/weapon_sweps_molotov/weapon_sweps_molotov.cs");
	exec("./add-ins/weapon_sweps_molotov/support_sweps_ext_flames.cs");
	exec("./add-ins/weapon_throwable_explosives/weapon_throwable_explosives.cs");
	exec("./add-ins/weapon_rocks/weapon_rocks.cs");
	exec("./add-ins/brick_interactive/brick_interactive.cs");
}