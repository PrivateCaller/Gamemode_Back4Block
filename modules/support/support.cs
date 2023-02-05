exec("./support_extraresources.cs");
exec("./support_multipleslots.cs");
exec("./support_leadprojectiles.cs");
exec("./support_l4bdebug.cs");
if(LoadRequiredAddOn("Weapon_FlashGrenade") == $Error::None) exec("./support_flashbang.cs");
if(LoadRequiredAddOn("Weapon_AEBase_BreachEnter") == $Error::None) exec("./support_aebase_breachenter.cs");
if(LoadRequiredAddOn("Support_BotHolePlus") == $Error::None) exec("./modules/add-ins/support_packages/support_afk_system.cs");