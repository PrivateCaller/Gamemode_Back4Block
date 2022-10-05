if(loadRequiredAddOn("Bot_Hole") == $Error::None && loadRequiredAddOn("Support_Lua") == $Error::None)
{	
	exec("./modules/scripts/support/support.cs");
	exec("./modules/add-ins/player_l4b/player_l4b.cs");
	exec("./modules/add-ins/script_l4b/script_l4b.cs");
	exec("./modules/add-ins/item_l4b/item_l4b.cs");
	exec("./modules/add-ins/weapon_l4b/weapon_l4b.cs");
	exec("./modules/add-ins/brick_l4b/brick_l4b.cs");
	exec("./modules/scripts/preferences.cs");
}