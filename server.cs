if(ForceRequiredAddOn("Bot_Hole") == $Error::AddOn_NotFound) return error("Bot_Hole is required for Gamemode_Left4Block to work");
if(ForceRequiredAddOn("Projectile_Radio_Wave") == $Error::AddOn_NotFound) return error("Projectile_Radio_Wave is required for Gamemode_Left4Block to work");
if(ForceRequiredAddOn("Support_Lua") == $Error::AddOn_NotFound || !isFunction(luacall)) return error("Support_Lua and the Lua dll are required for Gamemode_Left4Block to work");

exec("./modules/scripts/module_scripts.cs");
exec("./modules/misc/module_misc.cs");

if(isFunction(registerPreferenceAddon))//Function for BLG preferences
{
	registerPreferenceAddon("Gamemode_Left4Block", "Left 4 Block", "bios");				

	new ScriptObject(Preference)
	{
		className      = "Gamemode_Left4Block";
		addon          = "Gamemode_Left4Block";
		category       = "Blood";
		title          = "Blood damage threshold (0 to disable)";

		type           = "num";
		params         = "0 1000 0";

		variable       = "$Pref::L4B::Blood::BloodDamageThreshold";

		defaultValue   = "15";

		updateCallback = "";
		loadCallback   = "";

		hostOnly       = false;
		secret         = false;

		loadNow        = false;
		noSave         = false;
		requireRestart = false;
	};

	new ScriptObject(Preference)
	{
		className      = "Gamemode_Left4Block";
		addon          = "Gamemode_Left4Block";
		category       = "Blood";
		title          = "Blood dismember threshold (0 to disable)";

		type           = "num";
		params         = "0 1000 0";

		variable       = "$Pref::L4B::Blood::BloodDismemberThreshold";

		defaultValue   = "50";

		updateCallback = "";
		loadCallback   = "";

		hostOnly       = false;
		secret         = false;

		loadNow        = false;
		noSave         = false;
		requireRestart = false;
	};	

	new ScriptObject(Preference)
	{
		className      = "Gamemode_Left4Block";
		addon          = "Gamemode_Left4Block";
		category       = "Blood";
		title          = "Blood Decals Limit (0 to disable)";
		type           = "num";
		params         = "0 1000 0";
		variable       = "$Pref::L4B::Blood::BloodDecalsLimit";
		defaultValue   = "100";
		updateCallback = "";
		loadCallback   = "";
		hostOnly       = false;
		secret         = false;
		loadNow        = false;
		noSave         = false;
		requireRestart = false;
	};

	new ScriptObject(Preference)
	{
		className      = "Gamemode_Left4Block";
		addon          = "Gamemode_Left4Block";
		category       = "Blood";
		title          = "Blood Decals Timeout (ms)";
		type           = "num";
		params         = "0 30000 0";
		variable       = "$Pref::L4B::Blood::BloodDecalsTimeout";
		defaultValue   = "5000";
		updateCallback = "";
		loadCallback   = "";
		hostOnly       = false;
		secret         = false;
		loadNow        = false;
		noSave         = false;
		requireRestart = false;
	};
}
else
{
	$Pref::L4B::Blood::BloodDamageThreshold = 15;
	$Pref::L4B::Blood::BloodDismemberThreshold = 50;
	$Pref::L4B::Blood::BloodDecalsLimit = 100;
	$Pref::L4B::Blood::BloodDecalsTimeout = 5000;
	$Pref::L4B::MapRotation::Enabled = true;
	$Pref::L4B::MapRotation::RequiredVotes = 0;
	$Pref::L4B::MapRotation::RequiredNext = 2;
	$Pref::L4B::MapRotation::RequiredReload = 2;
	$Pref::L4B::MapRotation::VoteMin = 5;
	$Pref::L4B::MapRotation::MinReset = 5;
	$Pref::L4B:MapRotation::CoolDown = 10;
}

$Pref::L4B::Zombies::NormalDamage = 5;
$Pref::L4B::Zombies::SpecialsDamage = $Pref::L4B::Zombies::NormalDamage*3;
$L4B_CurrentMonth = getWord(strreplace(getDateTime(), "/", " "),0);
$RBloodLimbString0 = "headskin helmet bicorn visor shades gloweyes ballistichelmet constructionhelmet constructionhelmetbuds gloweyeL gloweyeR copHat caphat detective fancyhat fedora detectiveHat knitHat scoutHat fedoraHat shades gasmask";
$RBloodLimbDismemberString0 = "headpart1 headpart3 headpart4 headpart5 headpart6 headskullpart1 headskullpart3 headskullpart4 headskullpart5 headskullpart6";
$RBloodLimbString1 = "femchest chest clothstrap armor bucket cape pack quiver tank";
$RBloodLimbDismemberString1 = "chestpart1 chestpart2 chestpart3 chestpart4 chestpart5 chestpart6 chestpart7 chestpart8";
$RBloodLimbDismemberStringF1 = "femchestpart1 femchestpart2 femchestpart3 femchestpart4 femchestpart5 femchestpart6 femchestpart7 femchestpart8";
$RBloodLimbString2 = "rarm";
$RBloodLimbString3 = "larm";
$RBloodLimbString4 = "rhand";
$RBloodLimbString5 = "lhand";
$RBloodLimbString6 = "pants";
$RBloodLimbString7 = "rshoe";
$RBloodLimbString8 = "lshoe";
$L4BHat[$L4BHatAmount++] = "helmet";
$L4BHat[$L4BHatAmount++] = "capHat";
$L4BHat[$L4BHatAmount++] = "detectiveHat";
$L4BHat[$L4BHatAmount++] = "scoutHat";
$L4BHat[$L4BHatAmount++] = "fancyHat";
$L4BHat[$L4BHatAmount++] = "copHat";
$L4BHat[$L4BHatAmount++] = "knitHat";
$L4BHat[$L4BHatAmount++] = "fedoraHat";
$hZombieSkin[$hZombieSkinAmount++] = "0.16 0.25 0.21 1";
$hZombieSpecialType[$hZombieSpecialTypeAmount++] = "ZombieChargerHoleBot";
$hZombieSpecialType[$hZombieSpecialTypeAmount++] = "ZombieBoomerHoleBot";
$hZombieSpecialType[$hZombieSpecialTypeAmount++] = "ZombieSpitterHoleBot";
$hZombieSpecialType[$hZombieSpecialTypeAmount++] = "ZombieHunterHoleBot";
$hZombieSpecialType[$hZombieSpecialTypeAmount++] = "ZombieSmokerHoleBot";
$hZombieSpecialType[$hZombieSpecialTypeAmount++] = "ZombieJockeyHoleBot";
$hZombieUncommonType[$hZombieUncommonTypeAmount++] = "ZombieConstructionHoleBot";
$hZombieUncommonType[$hZombieUncommonTypeAmount++] = "ZombieFallenHoleBot";
$hZombieUncommonType[$hZombieUncommonTypeAmount++] = "ZombieCedaHoleBot";
$hZombieUncommonType[$hZombieUncommonTypeAmount++] = "ZombieSoldierHoleBot";
$hZombieUncommonType[$hZombieUncommonTypeAmount++] = "ZombiePoliceHoleBot";
configLoadL4BItemScavenge();
configLoadL4BItemSlots();
GunImages_GenerateSideImages();