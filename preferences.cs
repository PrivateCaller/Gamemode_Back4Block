if(isFunction(registerPreferenceAddon))//Function for BLG preferences
{
	registerPreferenceAddon("Gamemode_Left4Block", "Left 4 Block", "bios");

	new ScriptObject(Preference)
	{
		className      = "Gamemode_Left4Block_Difficulty";
		addon          = "Gamemode_Left4Block";
		category       = "General";
		title          = "Difficulty";
		type           = "dropdown";
		params         = "Normal 0 Advanced 1";
		variable       = "$Pref::Server::L4B2Bots::Difficulty";
		defaultValue   = "Normal 1";
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
		category       = "Director (Minigame Only)";
		title          = "Director enable on minigame start/reset";

		type           = "bool";
		params         = "";

		variable       = "$Pref::L4BDirector::EnableOnMG";

		defaultValue   = "0";

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
		category       = "Bot Specials";
		title          = "Specials pinned messages";
		type           = "bool";
		params         = "";
		variable       = "$Pref::Server::L4B2Bots::MinigameMessages";
		defaultValue   = "1";
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
		category       = "Bot Specials";
		title          = "Specials spawn cue";
		type           = "bool";
		params         = "";
		variable       = "$Pref::Server::L4B2Bots::SpecialsCue";
		defaultValue   = "1";
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
		category       = "Ammo Crate";
		title          = "Limited supplies (Minigame only)";

		type           = "bool";
		params         = "";

		variable       = "$Pref::Server::L4BAmmocrate::AmmoSupplies";

		defaultValue   = "1";

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
		category       = "Ammo Crate";
		title          = "Supply amount";

		type           = "num";
		params         = "0 100 0";

		variable       = "$Pref::Server::L4BAmmocrate::AmmoSupplyAmount";

		defaultValue   = "25";

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
		category       = "Ammo Crate";
		title          = "Acquire delay (ms)";

		type           = "num";
		params         = "0 750 0";

		variable       = "$Pref::Server::L4BAmmocrate::AmmoAcquireDelay";

		defaultValue   = "500";

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
		category       = "Health Locker";
		title          = "Limited supplies (Minigame only)";

		type           = "bool";
		params         = "";

		variable       = "$Pref::L4BHealthLocker::Supplies";

		defaultValue   = "1";

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
		category       = "Health Locker";
		title          = "Supply amount";

		type           = "num";
		params         = "0 100 0";

		variable       = "$Pref::L4BHealthLocker::SupplyAmount";

		defaultValue   = "20";

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
		category       = "Health Locker";
		title          = "Acquire delay (ms)";

		type           = "num";
		params         = "0 750 0";

		variable       = "$Pref::L4BHealthLocker::AcquireDelay";

		defaultValue   = "500";

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
	$Pref::Server::L4BAmmocrate::Supplies = 1;
	$Pref::Server::L4BAmmocrate::SupplyAmount = 25;
	$Pref::Server::L4BAmmocrate::AcquireDelay = 500;
	$Pref::L4BHealthLocker::Supplies = 1;
	$Pref::L4BHealthLocker::SupplyAmount = 20;
	$Pref::L4BHealthLocker::AcquireDelay = 500;

	$Pref::L4BDirector::EnableOnMG = 0;
	
	$Pref::Server::L4B2Bots::Difficulty = 1;
	$Pref::Server::L4B2Bots::MinigameMessages = 1;

	$Pref::Server::L4B2Bots::NormalDamage = 5;
	$Pref::Server::L4B2Bots::SpecialsDamage = 15;
	$Pref::Server::L4B2Bots::TankRounds = 1;
	$Pref::Server::L4B2Bots::TankRoundChance = 25;
	$Pref::Server::L4B2Bots::TankHealth = 5000;
	$Pref::Server::L4B2Bots::SurvivorImmunity = 1;
	$Pref::Server::L4B2Bots::MaxSpecial = 4;
	$Pref::Server::L4B2Bots::MaxHorde = 60;
	$Pref::Server::L4B2Bots::MaxTank = 1;
}

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

function L4B_DifficultyAdjustment()
{
	switch($Pref::Server::L4B2Bots::Difficulty)
	{
		case 0: $Pref::Server::L4B2Bots::NormalDamage = 5;
				$Pref::Server::L4B2Bots::SpecialsDamage = 15;
				$Pref::Server::L4B2Bots::TankRounds = 1;
				$Pref::Server::L4B2Bots::TankRoundChance = 25;
				$Pref::Server::L4B2Bots::TankHealth = 5000;
				$Pref::Server::L4B2Bots::SurvivorImmunity = 1;
				$Pref::Server::L4B2Bots::MaxSpecial = 4;
				$Pref::Server::L4B2Bots::MaxHorde = 60;
				$Pref::Server::L4B2Bots::MaxTank = 1;

		case 1: $Pref::Server::L4B2Bots::NormalDamage = 10;
				$Pref::Server::L4B2Bots::SpecialsDamage = 25;
				$Pref::Server::L4B2Bots::TankRounds = 2;
				$Pref::Server::L4B2Bots::TankRoundChance = 60;
				$Pref::Server::L4B2Bots::TankHealth = 7500;
				$Pref::Server::L4B2Bots::SurvivorImmunity = 0;
				$Pref::Server::L4B2Bots::MaxSpecial = 8;
				$Pref::Server::L4B2Bots::MaxHorde = 35;
				$Pref::Server::L4B2Bots::MaxTank = 2;

		default:
	}

	eval("ZombieTankHoleBot.maxDamage =" @ $Pref::Server::L4B2Bots::TankHealth @ ";");
}

function Gamemode_Left4Block_Difficulty::onUpdate(%this, %val) 
{
	L4B_DifficultyAdjustment();
}