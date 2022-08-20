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
		category       = "Director (Minigame Only)";
		title          = "Director sounds/music";

		type           = "bool";
		params         = "";

		variable       = "$Pref::L4BDirector::EnableCues";

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
		category       = "Survivor";
		title          = "Enable survivor brick scanning";

		type           = "bool";
		params         = "";

		variable       = "$Pref::SurvivorPlayer::BrickScanning";

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
		category       = "Survivor";
		title          = "Enable survivor downing";

		type           = "bool";
		params         = "";

		variable       = "$Pref::SurvivorPlayer::EnableDowning";

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
		category       = "Survivor";
		title          = "Enable melee";

		type           = "bool";
		params         = "";

		variable       = "$Pref::SecondaryMelee::MeleeMode";

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
		category       = "Bots Main";
		title          = "Style";
		type           = "dropdown";
		params         = "Default 0 Default_Green 1 Custom_Green 2 ZAPT 3";
		variable       = "$Pref::Server::L4B2Bots::CustomStyle";
		defaultValue   = "Default 0";
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
		category       = "Bots Main";
		title          = "Limited lifetime";
		type           = "bool";
		params         = "";
		variable       = "$Pref::Server::L4B2Bots::LimitedLifetime";
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
		category       = "Bots Main";
		title          = "Victim saved messages";
		type           = "bool";
		params         = "";
		variable       = "$Pref::Server::L4B2Bots::VictimSavedMessages";
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
		className      = "L4B2Bots_NormalDamage";
		addon          = "Gamemode_Left4Block";
		category       = "Bots Main";
		title          = "Incap/strangle cutscene length";
		type           = "num";
		params         = "0 1000 0";
		variable       = "$Pref::Server::L4B2Bots::NeedHelpCutsceneLength";
		defaultValue   = "200";
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
		category       = "Bots Normal";
		title          = "Normal zombies random scale";
		type           = "bool";
		params         = "";
		variable       = "$Pref::Server::L4B2Bots::ZombieRandomScale";
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
		category       = "Bots Normal";
		title          = "Burning zombies burn on collision";
		type           = "bool";
		params         = "";
		variable       = "$Pref::Server::L4B2Bots::ZombieBurnerCollision";
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
		category       = "Bots Normal";
		title          = "Uncommon zombies warning light (distractors, lethal weapons, toxic)";
		type           = "bool";
		params         = "";
		variable       = "$Pref::Server::L4B2Bots::UncommonWarningLight";
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
	$Pref::L4BDirector::EnableCues = 1;
    $Pref::SurvivorPlayer::BrickScanning = 1;
    $Pref::SurvivorPlayer::EnableDowning = 1;
	$Pref::SecondaryMelee::MeleeMode = 1;
	
	$Pref::Server::L4B2Bots::CustomStyle = 0;
	$Pref::Server::L4B2Bots::Difficulty = 1;
	$Pref::Server::L4B2Bots::SpecialsCue = 1;
	$Pref::Server::L4B2Bots::LimitedLifetime = 1;
	$Pref::Server::L4B2Bots::MinigameMessages = 1;
	$Pref::Server::L4B2Bots::ZombieRandomScale = 1;
	$Pref::Server::L4B2Bots::UncommonWarningLight = 0;
	$Pref::Server::L4B2Bots::VictimSavedMessages = 1;
	$Pref::Server::L4B2Bots::ZombieBurnerCollision = 1;

	$Pref::Server::L4B2Bots::NormalDamage = 2;
	$Pref::Server::L4B2Bots::SpecialsDamage = 4;
	$Pref::Server::L4B2Bots::TankChance = 15;
	$Pref::Server::L4B2Bots::TankRounds = 0;
	$Pref::Server::L4B2Bots::TankHealth = 2000;
	$Pref::Server::L4B2Bots::SurvivorImmunity = 1;
	$Pref::Server::L4B2Bots::MaxSpecial = 4;
	$Pref::Server::L4B2Bots::MaxHorde = 40;
	$Pref::Server::L4B2Bots::MaxTank = 1;
}

function L4B_DifficultyAdjustment()
{
	switch($Pref::Server::L4B2Bots::Difficulty)
	{
		case 0: $Pref::Server::L4B2Bots::NormalDamage = 10;
				$Pref::Server::L4B2Bots::SpecialsDamage = 15;
				$Pref::Server::L4B2Bots::TankRounds = 1;
				$Pref::Server::L4B2Bots::TankRoundChance = 4;
				$Pref::Server::L4B2Bots::TankHealth = 5000;
				$Pref::Server::L4B2Bots::SurvivorImmunity = 1;
				$Pref::Server::L4B2Bots::MaxSpecial = 4;
				$Pref::Server::L4B2Bots::MaxHorde = 60;
				$Pref::Server::L4B2Bots::MaxTank = 1;

		case 1: $Pref::Server::L4B2Bots::NormalDamage = 15;
				$Pref::Server::L4B2Bots::SpecialsDamage = 25;
				$Pref::Server::L4B2Bots::TankRounds = 2;
				$Pref::Server::L4B2Bots::TankRoundChance = 12;
				$Pref::Server::L4B2Bots::TankHealth = 7500;
				$Pref::Server::L4B2Bots::SurvivorImmunity = 0;
				$Pref::Server::L4B2Bots::MaxSpecial = 8;
				$Pref::Server::L4B2Bots::MaxHorde = 75;
				$Pref::Server::L4B2Bots::MaxTank = 2;

		default:
	}

	eval("ZombieTankHoleBot.maxDamage =" @ $Pref::Server::L4B2Bots::TankHealth @ ";");
}

function Gamemode_Left4Block_Difficulty::onUpdate(%this, %val) 
{
	L4B_DifficultyAdjustment();
}