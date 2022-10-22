forceRequiredAddOn("Bot_Hole");
forceRequiredAddOn("Support_Lua");

exec("./modules/scripts/preferences.cs");
exec("./modules/scripts/support/support.cs");
exec("./modules/add-ins/player_l4b/player_l4b.cs");
exec("./modules/scripts/support/support_flavormessages.cs");
exec("./modules/add-ins/script_l4b/script_l4b.cs");
exec("./modules/add-ins/item_l4b/item_l4b.cs");
exec("./modules/add-ins/weapon_l4b/weapon_l4b.cs");
exec("./modules/add-ins/brick_l4b/brick_l4b.cs");

%time = strreplace(getDateTime(), "/", " ");
$L4B_CurrentMonth = getWord(%time,0);

$hZombieSkin[%hzskin++] = "0.16 0.25 0.21 1";
$hZombieSkinAmount = %hzskin;

$hZombieSpecialType[%hzs++] = "ZombieChargerHoleBot";
$hZombieSpecialType[%hzs++] = "ZombieBoomerHoleBot";
$hZombieSpecialType[%hzs++] = "ZombieSpitterHoleBot";
$hZombieSpecialType[%hzs++] = "ZombieHunterHoleBot";
$hZombieSpecialType[%hzs++] = "ZombieSmokerHoleBot";
$hZombieSpecialType[%hzs++] = "ZombieJockeyHoleBot";
$hZombieSpecialTypeAmount = %hzs;

$hZombieUncommonType[%hzu++] = "ZombieConstructionHoleBot";
$hZombieUncommonType[%hzu++] = "ZombieFallenHoleBot";
$hZombieUncommonType[%hzu++] = "ZombieCedaHoleBot";
$hZombieUncommonType[%hzu++] = "ZombieSoldierHoleBot";
$hZombieUncommonType[%hzu++] = "ZombiePoliceHoleBot";
$hZombieUncommonTypeAmount = %hzu;

configLoadL4BItemScavenge();
configLoadL4BItemSlots(); 