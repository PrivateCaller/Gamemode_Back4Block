%pattern = "Add-Ons/Package_Left4Block/sound/*.wav";//Too lazy to write datablock files for the sounds, just took this from the Disease Gamemode
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/package_left4block/sound/", "");
	%soundName = strreplace(%soundName, "/", "");
	%soundName = strreplace(%soundName, ".wav", "");
	%soundName = strreplace(%soundName, "quiet", "");
	%soundName = strreplace(%soundName, "normal", "");
	%soundName = strreplace(%soundName, "loud", "");
	%soundName = strreplace(%soundName, "snd-tank", "");//Has to be written with snd-zombiename or it might replace the name of the zombie sound files themselves
	%soundName = strreplace(%soundName, "snd-witch", "");
	%soundName = strreplace(%soundName, "snd-hunter", "");
	%soundName = strreplace(%soundName, "snd-boomer", "");
	%soundName = strreplace(%soundName, "snd-charger", "");
	%soundName = strreplace(%soundName, "snd-smoker", "");
	%soundName = strreplace(%soundName, "snd-jockey", "");
	%soundName = strreplace(%soundName, "snd-spitter", "");
	%soundName = strreplace(%soundName, "snd-common", "");
	%soundName = strreplace(%soundName, "snd-survivor", "");

	//Check the names of the folders to determine what type of soundscape it will be, and check if it's a loopable sound or not
	if(strstr(%file,"normal") != -1)//Normal soundscape
	if(strstr(%file,"loop") != -1)
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioCloseLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClose3d; filename = \"" @ %file @ "\"; };");

	if(strstr(%file,"quiet") != -1)//Quiet soundscape
	if(strstr(%file,"loop") != -1)
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosestLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosest3d; filename = \"" @ %file @ "\"; };");

	if(strstr(%file,"loud") != -1)//Loudest Soundscape
	if(strstr(%file,"loop") != -1)
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioDefaultLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioDefault3d; filename = \"" @ %file @ "\"; };");

	%file = findNextFile(%pattern);
}
function configLoadL4BTXT(%file,%svartype)//Set up custom appearances
{
	%read = new FileObject();
	if(!isFile("config/server/L4B2_Bots/" @ %file @ ".txt"))
	{
		%read.openForRead("add-ons/package_left4block/defaultconfig/" @ %file @ ".txt");

		%write = new FileObject();
		%write.openForWrite("config/server/L4B2_Bots/" @ %file @ ".txt");
	
		while(!%read.isEOF())
		{
			%line = %read.readLine();
			%write.writeLine(%line);
		}

		%write.close();
		%write.delete();
	}

	%read.openForRead("config/server/L4B2_Bots/" @ %file @ ".txt");

	while(!%read.isEOF())
	{
		%i++;
		%line = %read.readLine(); 
		eval("$" @ %svartype @"[%i] = \"" @ %line @ "\";");
		eval("$" @ %svartype @"Amount = %i;");
	}
	
	%read.close();
	%read.delete();
}
configLoadL4BTXT("zombiemalenames",hZombieNameMale);
configLoadL4BTXT("zombiefemalenames",hZombieNameFemale);
configLoadL4BTXT("zombiefaces",hZombieFace);
configLoadL4BTXT("zombiedecals",hZombieDecal);
configLoadL4BTXT("zombieskin",hZombieSkin);

$hZombieDecalDefault[%n = 1] = "AAA-None";
$hZombieDecalDefault[%n++] = "Mod-Army";
$hZombieDecalDefault[%n++] = "Mod-Police";
$hZombieDecalDefault[%n++] = "Mod-Suit";
$hZombieDecalDefault[%n++] = "Meme-Mongler";
$hZombieDecalDefault[%n++] = "Mod-Daredevil";
$hZombieDecalDefault[%n++] = "Mod-Pilot";
$hZombieDecalDefault[%n++] = "Mod-Prisoner";
$hZombieDecalDefault[%n++] = "Meme-Mongler";
$hZombieDecalDefault[%n++] = "Medieval-YARLY";
$hZombieDecalDefault[%n++] = "Medieval-ORLY";
$hZombieDecalDefault[%n++] = "Medieval-Eagle";
$hZombieDecalDefault[%n++] = "Medieval-Lion";
$hZombieDecalDefault[%n++] = "Medieval-Tunic";
$hZombieDecalDefault[%n++] = "Hoodie";
$hZombieDecalDefault[%n++] = "DrKleiner";
$hZombieDecalDefault[%n++] = "Chef";
$hZombieDecalDefault[%n++] = "worm-sweater";
$hZombieDecalDefault[%n++] = "worm_engineer";
$hZombieDecalDefault[%n++] = "Archer";
$hZombieDecalDefaultAmount = %n;

$hZombieHat[%c++] = 4;
$hZombieHat[%c++] = 6;
$hZombieHat[%c++] = 7;
$hZombieHat[%c++] = 0;
$hZombieHat[%c++] = 1;
$hZombieHatAmount = %c;

$hZombiePack[%d++] = 0;
$hZombiePack[%d++] = 2;
$hZombiePack[%d++] = 3;
$hZombiePack[%d++] = 4;
$hZombiePack[%d++] = 5;
$hZombiePackAmount = %d;

$hZombieSpecialType[%e++] = "CommonZombieHoleBot";
$hZombieSpecialType[%e++] = "ZombieHunterHoleBot";
$hZombieSpecialType[%e++] = "ZombieBoomerHoleBot";
$hZombieSpecialType[%e++] = "ZombieChargerHoleBot";
$hZombieSpecialType[%e++] = "ZombieSpitterHoleBot";
$hZombieSpecialType[%e++] = "ZombieSmokerHoleBot";
$hZombieSpecialType[%e++] = "ZombieJockeyHoleBot";
$hZombieSpecialType[%e++] = "ZombieTankHoleBot";
$hZombieSpecialTypeAmount = %e;

if(isObject(RiotShieldImage))
$IsRiotShieldEnabled = 1;
else $IsRiotShieldEnabled = 0;

exec("./datablocks.cs");
exec("./functions.cs");
exec("./packages.cs");

exec("./support/billboards/billboards.cs");
if(isFunction(NetObject, setNetFlag))
{
	exec("./support/billboards_wrapper.cs");
	$L4B_hasSelectiveGhosting = true;
}
else
{
	$L4B_hasSelectiveGhosting = false;
	error("ERROR: The Selective Ghosting DLL is required for Package_Left4Block's billboards to work.");
	schedule(1000, 0, messageAll, 'MsgError', "\c0ERROR: The Selective Ghosting DLL is required for Package_Left4Block's billboards to work.");
}

exec("./weapon_rocks.cs");
exec("./weapon_melee.cs");
exec("./weapon_throwableexplosives.cs");
exec("./item_healing.cs");
exec("./weapon_distractions.cs");

exec("./bots/zombies/common.cs");
exec("./bots/zombies/uncommon_soldier.cs");
exec("./bots/zombies/uncommon_fallen.cs");
exec("./bots/zombies/uncommon_mud.cs");
exec("./bots/zombies/uncommon_construction.cs");
exec("./bots/zombies/uncommon_clown.cs");
exec("./bots/zombies/uncommon_jimmy_gibbs.cs");
exec("./bots/zombies/uncommon_ceda.cs");
exec("./bots/zombies/uncommon_toxic.cs");
exec("./bots/zombies/boomer.cs");
exec("./bots/zombies/charger.cs");
exec("./bots/zombies/hunter.cs");
exec("./bots/zombies/jockey.cs");
exec("./bots/zombies/smoker.cs");
exec("./bots/zombies/spitter.cs");
exec("./bots/zombies/tank.cs");
exec("./bots/zombies/witch.cs");
exec("./bots/survivors/bot_survivor.cs");

exec("./support/afk_system.cs");

if(isFunction(registerPreferenceAddon))//Function for BLG preferences
{
	registerPreferenceAddon("Package_left4block", "L4B Package", "bios");

	new ScriptObject(Preference)
	{
		className      = "Package_left4block";

		addon          = "Package_left4block";
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
		className      = "Package_left4block";

		addon          = "Package_left4block";
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
		className      = "L4B Director";

		addon          = "Package_Left4Block";
		category       = "Director (Minigame Only)";
		title          = "Director message prefix";

		type           = "string";
		params         = "100 0";

		variable       = "$Pref::L4BDirector::Director_Message_Prefix";

		defaultValue   = "<color:FFFF00>";

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
		className      = "L4B Director";

		addon          = "Package_Left4Block";
		category       = "Director (Minigame Only)";
		title          = "Director interval (seconds)";

		type           = "num";
		params         = "0 120 0";

		variable       = "$Pref::L4BDirector::Director_Interval";

		defaultValue   = "30";

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
		className      = "Package_left4block";

		addon          = "Package_left4block";
		category       = "Director (Minigame Only)";
		title          = "Director minigame messages";

		type           = "bool";
		params         = "";

		variable       = "$Pref::L4BDirector::AllowMGMessages";

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
		className      = "Package_left4block";

		addon          = "Package_left4block";
		category       = "Player";
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
		className      = "Package_left4block";

		addon          = "Package_left4block";
		category       = "Player";
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
		className      = "Package_left4block";

		addon          = "Package_left4block";
		category       = "Player";
		title          = "Survivor infection immunity";

		type           = "bool";
		params         = "";

		variable       = "$Pref::SurvivorPlayer::SurvivorImmunity";

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
		className      = "Package_left4block";

		addon          = "Package_left4block";
		category       = "Melee";
		title          = "Enable Survivor Melee";

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
		className      = "Package_left4block";

		addon          = "Package_left4block";
		category       = "Melee";
		title          = "Melee punch damage";

		type           = "num";
		params         = "0 1000 0";

		variable       = "$Pref::SecondaryMelee::MeleeDamage";

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
		className      = "Package_left4block";

		addon          = "Package_left4block";
		category       = "Melee";
		title          = "Melee slap damage";

		type           = "num";
		params         = "0 1000 0";

		variable       = "$Pref::SecondaryMelee::SlapDamage";

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
		className      = "Package_left4block";

		addon          = "Package_left4block";
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
		className      = "Package_left4block";

		addon          = "Package_left4block";
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
		className      = "Package_left4block";

		addon          = "Package_left4block";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bots Main";
		title          = "Tank & charger tumble chance";
		type           = "num";
		params         = "0 100 0";
		variable       = "$Pref::Server::L4B2Bots::TankChargerTumbleChance";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bots Main";
		title          = "Zombified player bot appearance chance";
		type           = "num";
		params         = "0 100 0";
		variable       = "$Pref::Server::L4B2Bots::ZombifiedPlayerBotAppearance";
		defaultValue   = "5";
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
		addon          = "Package_left4block";
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
		className      = "L4B2Bots_NormalDamage";
		addon          = "Package_left4block";
		category       = "Bots Normal";
		title          = "Normal zombies damage amount";
		type           = "num";
		params         = "0 100 0";
		variable       = "$Pref::Server::L4B2Bots::NormalsDamage";
		defaultValue   = "5";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bots Normal";
		title          = "Hold down victims";
		type           = "bool";
		params         = "";
		variable       = "$Pref::Server::L4B2Bots::HoldVictims";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bots Normal";
		title          = "Enable spawning with rocks";
		type           = "bool";
		params         = "";
		variable       = "$Pref::Server::L4B2Bots::RockSpawning";
		defaultValue   = "0";
		updateCallback = "";
		loadCallback   = "";
		hostOnly       = false;
		secret         = false;
		loadNow        = false;
		noSave         = false;
		requireRestart = false;
	};

	if(LoadRequiredAddOn("Item_Skis") == $Error::None)
	{
		new ScriptObject(Preference)
		{
			className      = "Package_left4block";
			addon          = "Package_left4block";
			category       = "Bots Normal";
			title          = "Clumsify common/uncommon zombies";
			type           = "bool";
			params         = "";
			variable       = "$Pref::Server::L4B2Bots::ClumsyZombies";
			defaultValue   = "0";
			updateCallback = "";
			loadCallback   = "";
			hostOnly       = false;
			secret         = false;
			loadNow        = false;
			noSave         = false;
			requireRestart = false;
		};
	}

	new ScriptObject(Preference)
	{
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bot Smoker";
		title          = "Tongue max reach distance";
		type           = "num";
		params         = "0 100 0";
		variable       = "$Pref::Server::L4B2Bots::SmokerTongueDist";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bot Smoker";
		title          = "Tongue pull force";
		type           = "num";
		params         = "0 5 0";
		variable       = "$Pref::Server::L4B2Bots::SmokerTonguePull";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bot Specials";
		title          = "Specials zombies pinned damage amount";
		type           = "num";
		params         = "0 100 0";
		variable       = "$Pref::Server::L4B2Bots::SpecialsPinDamage";
		defaultValue   = "12";
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
		className      = "L4B2Bots_SpecialDamage";
		addon          = "Package_left4block";
		category       = "Bot Specials";
		title          = "Specials zombies damage amount";
		type           = "num";
		params         = "0 100 0";
		variable       = "$Pref::Server::L4B2Bots::SpecialsDamage";
		defaultValue   = "6";
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
		className      = "L4B2Bots_TankHP";
		addon          = "Package_left4block";
		category       = "Bot Tank";
		title          = "Tank health";
		type           = "num";
		params         = "0 10000 0";
		variable       = "$Pref::Server::L4B2Bots::TankHP";
		defaultValue   = "3000";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bot Tank";
		title          = "Tank melee damage";
		type           = "num";
		params         = "0 100 0";
		variable       = "$Pref::Server::L4B2Bots::TankMeleeDamage";
		defaultValue   = "24";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bot Tank";
		title          = "Tank boulders chance";
		type           = "num";
		params         = "0 100 0";
		variable       = "$Pref::Server::L4B2Bots::TankBoulders";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bot Tank";
		title          = "Tank lunge chance";
		type           = "num";
		params         = "0 100 0";
		variable       = "$Pref::Server::L4B2Bots::TankLunge";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bot Loot";
		title          = "Normal Zombie Loot Chance";
		type           = "num";
		params         = "0 50 0";
		variable       = "$Pref::Server::L4B2Bots::ZombieLootChance";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bot Loot";
		title          = "Normal zombie loot item 1 (use /gifi or /getifi)";
		type           = "string";
		params         = "50 1";
		variable       = "$Pref::Server::L4B2Bots::ZombieLootItem1";
		defaultValue   = "None";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bot Loot";
		title          = "Normal zombie loot item 2";
		type           = "string";
		params         = "50 1";
		variable       = "$Pref::Server::L4B2Bots::ZombieLootItem2";
		defaultValue   = "None";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bot Loot";
		title          = "Normal zombie loot item 3";
		type           = "string";
		params         = "50 1";
		variable       = "$Pref::Server::L4B2Bots::ZombieLootItem3";
		defaultValue   = "None";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bot Loot";
		title          = "Normal zombie loot item 4";
		type           = "string";
		params         = "50 1";
		variable       = "$Pref::Server::L4B2Bots::ZombieLootItem4";
		defaultValue   = "None";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bot Loot";
		title          = "Normal zombie loot item 5";
		type           = "string";
		params         = "50 1";
		variable       = "$Pref::Server::L4B2Bots::ZombieLootItem5";
		defaultValue   = "None";
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
		className      = "Package_left4block";
		addon          = "Package_left4block";
		category       = "Bot Loot";
		title          = "Fallen zombie loot item bonus";
		type           = "string";
		params         = "50 1";
		variable       = "$Pref::Server::L4B2Bots::ZombieLootItemFallen";
		defaultValue   = "None";
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
    $Pref::L4BDirector::Director_Message_Prefix = "<color:FFFF00>";
    $Pref::L4BDirector::Director_Interval = 30;
	$Pref::L4BDirector::AllowMGMessages = 1;
	$Pref::L4BDirector::EnableCues = 1;
    $Pref::SurvivorPlayer::BrickScanning = 1;
    $Pref::SurvivorPlayer::EnableDowning = 1;
	$Pref::SurvivorPlayer::SurvivorImmunity = 1;
	$Pref::SecondaryMelee::MeleeMode = 1;
	$Pref::SecondaryMelee::MeleeDamage = 5;
	$Pref::SecondaryMelee::SlapDamage = 15;
	$Pref::SecondaryMelee::MeleeForce = 1;
	$Pref::SecondaryMelee::MeleeCharges = 3;
	$Pref::SecondaryMelee::MeleeCooldownRate = 1000;
	
	$Pref::Server::L4B2Bots::CustomStyle = 0;
	$Pref::Server::L4B2Bots::SmokerTonguePull = 1;
	$Pref::Server::L4B2Bots::SpecialsCue = 1;
	$Pref::Server::L4B2Bots::SmokerTongueDist = 50;
	$Pref::Server::L4B2Bots::SpecialsPinDamage = 12;
	$Pref::Server::L4B2Bots::SpecialsDamage = 6;
	$Pref::Server::L4B2Bots::LimitedLifetime = 1;
	$Pref::Server::L4B2Bots::MinigameMessages = 1;
	$Pref::Server::L4B2Bots::TankChargerTumbleChance = 25;
	$Pref::Server::L4B2Bots::ZombieRandomScale = 1;
	$Pref::Server::L4B2Bots::HoldVictims = 0;
	$Pref::Server::L4B2Bots::SpecialsWarnLight = 0;
	$Pref::Server::L4B2Bots::ClumsyZombies = 0;
	$Pref::Server::L4B2Bots::TankHP = 3000;
	$Pref::Server::L4B2Bots::TankBoulders = 50;
	$Pref::Server::L4B2Bots::NormalsDamage = 5;
	$Pref::Server::L4B2Bots::TankMeleeDamage = 24;
	$Pref::Server::L4B2Bots::TankLunge = 25;
	$Pref::Server::L4B2Bots::UncommonWarningLight = 0;
	$Pref::Server::L4B2Bots::ZombieLootChance = 15;
	$Pref::Server::L4B2Bots::ZombieLootItem1 = "None";
	$Pref::Server::L4B2Bots::ZombieLootItem2 = "None";
	$Pref::Server::L4B2Bots::ZombieLootItem3 = "None";
	$Pref::Server::L4B2Bots::ZombieLootItem4 = "None";
	$Pref::Server::L4B2Bots::ZombieLootItem5 = "None";
	$Pref::Server::L4B2Bots::ZombieLootItemFallen = "None";
	$Pref::Server::L4B2Bots::VictimSavedMessages = 1;
	$Pref::Server::L4B2Bots::ZombieBurnerCollision = 1;
	$Pref::Server::L4B2Bots::ZombifiedPlayerBotAppearance = 5;
}

function L4B2Bots_TankHP::onUpdate(%this, %val) 
{
	eval("ZombieTankHoleBot.maxDamage =" @ $Pref::Server::L4B2Bots::TankHP @ ";");
}