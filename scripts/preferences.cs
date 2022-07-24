if(isFunction(registerPreferenceAddon))//Function for BLG preferences
{
	registerPreferenceAddon("Gamemode_Left4Block", "Left 4 Block", "bios");

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
		category       = "Melee";
		title          = "Enable survivor melee";

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
		category       = "Melee";
		title          = "Meelee force";

		type           = "num";
		params         = "0 10 0";

		variable       = "$Pref::SecondaryMelee::MeleeForce";

		defaultValue   = "2";

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
		category       = "Melee";
		title          = "Melee charges";

		type           = "num";
		params         = "-1 10 0";

		variable       = "$Pref::SecondaryMelee::MeleeCharges";

		defaultValue   = "-1";

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
		category       = "Melee";
		title          = "Melee charges cooldown Rate";

		type           = "num";
		params         = "0 500 0";

		variable       = "$Pref::SecondaryMelee::MeleeCooldownRate";

		defaultValue   = "250";

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
		className      = "Gamemode_Left4Block_Difficulty";
		addon          = "Gamemode_Left4Block";
		category       = "Bots Main";
		title          = "Difficulty";
		type           = "dropdown";
		params         = "Easy 0 Normal 1 Advanced 2 Expert 3";
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
		title          = "Specials warn light";
		type           = "dropdown";
		params         = "Disabled 0 Red 1 Green 2 Blue 3 White 4";
		variable       = "$Pref::Server::L4B2Bots::SpecialsWarnLight";
		defaultValue   = "Disabled 0";
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
}
else
{
	$Pref::L4BDirector::EnableOnMG = 0;
	$Pref::L4BDirector::EnableCues = 1;
    $Pref::SurvivorPlayer::BrickScanning = 1;
    $Pref::SurvivorPlayer::EnableDowning = 1;
	$Pref::SecondaryMelee::MeleeMode = 1;
	$Pref::SecondaryMelee::MeleeForce = 1;
	$Pref::SecondaryMelee::MeleeCharges = 3;
	$Pref::SecondaryMelee::MeleeCooldownRate = 1000;
	
	$Pref::Server::L4B2Bots::CustomStyle = 0;
	$Pref::Server::L4B2Bots::Difficulty = 1;
	$Pref::Server::L4B2Bots::SpecialsCue = 1;
	$Pref::Server::L4B2Bots::LimitedLifetime = 1;
	$Pref::Server::L4B2Bots::MinigameMessages = 1;
	$Pref::Server::L4B2Bots::ZombieRandomScale = 1;
	$Pref::Server::L4B2Bots::SpecialsWarnLight = 0;
	$Pref::Server::L4B2Bots::UncommonWarningLight = 0;
	$Pref::Server::L4B2Bots::VictimSavedMessages = 1;
	$Pref::Server::L4B2Bots::ZombieBurnerCollision = 1;
}

function L4B_DifficultyAdjustment()
{
	switch($Pref::Server::L4B2Bots::Difficulty)
	{
		case 0: $L4B_NormalDamage = 2;
				$L4B_SpecialsDamage = 4;
				$L4B_TankChance = 15;
				$L4B_TankRounds = 0;
				$L4B_TankHealth = 2000;
				$L4B_SurvivorImmunity = 1;
				$L4B_MaxSpecial = 4;
				$L4B_MaxHorde = 40;
				$L4B_MaxTank = 1;

		case 1: $L4B_NormalDamage = 6;
				$L4B_SpecialsDamage = 12;
				$L4B_TankChance = 30;
				$L4B_TankRounds = 1;
				$L4B_TankHealth = 2000;
				$L4B_SurvivorImmunity = 1;
				$L4B_MaxSpecial = 4;
				$L4B_MaxHorde = 75;
				$L4B_MaxTank = 2;

		case 2: $L4B_NormalDamage = 8;
				$L4B_SpecialsDamage = 16;
				$L4B_TankChance = 50;
				$L4B_TankRounds = 1;
				$L4B_TankHealth = 4000;
				$L4B_SurvivorImmunity = 0;
				$L4B_MaxSpecial = 6;
				$L4B_MaxHorde = 75;
				$L4B_MaxTank = 2;

		case 3: $L4B_NormalDamage = 16;
				$L4B_SpecialsDamage = 32;
				$L4B_TankChance = 75;
				$L4B_TankRounds = 2;
				$L4B_TankHealth = 5000;
				$L4B_SurvivorImmunity = 0;
				$L4B_MaxSpecial = 8;
				$L4B_MaxHorde = 100;
				$L4B_MaxTank = 3;

		default:
	}

	eval("ZombieTankHoleBot.maxDamage =" @ $L4B_TankHealth @ ";");
}


function Gamemode_Left4Block_Difficulty::onUpdate(%this, %val) 
{
	L4B_DifficultyAdjustment();
}