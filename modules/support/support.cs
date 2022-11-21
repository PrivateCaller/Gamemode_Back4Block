exec("./support_extraresources.cs");
if(LoadRequiredAddOn("Weapon_FlashGrenade") == $Error::None) exec("./support_flashbang.cs");
if(LoadRequiredAddOn("Support_BotHolePlus") == $Error::None) exec("./modules/add-ins/support_packages/support_afk_system.cs");
exec("./support_clientlogger.cs");
exec("./support_leadprojectiles.cs");
exec("./support_multipleslots.cs");
exec("./support_l4bdebug.cs");