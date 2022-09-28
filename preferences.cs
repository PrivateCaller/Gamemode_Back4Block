function configLoadL4BTXT(%file,%svartype)//Set up custom variables
{
	%read = new FileObject();
	if(!isFile("config/server/Left4Block/" @ %file @ ".txt"))
	{
		%read.openForRead("add-ons/gamemode_left4block/config/" @ %file @ ".txt");

		%write = new FileObject();
		%write.openForWrite("config/server/Left4Block/" @ %file @ ".txt");
	
		while(!%read.isEOF())
		{
			%line = %read.readLine();
			%write.writeLine(%line);
		}

		%write.close();
		%write.delete();
	}

	%read.openForRead("config/server/Left4Block/" @ %file @ ".txt");

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

function configLoadL4BItemScavenge()//Set up items
{
	%read = new FileObject();
	if(!isFile("config/server/Left4Block/itemscavenge.txt"))
	{
		%read.openForRead("add-ons/gamemode_left4block/config/itemscavenge.txt");

		%write = new FileObject();
		%write.openForWrite("config/server/Left4Block/itemscavenge.txt");
	
		while(!%read.isEOF())
		{
			%line = %read.readLine();
			%write.writeLine(%line);
		}

		%write.close();
		%write.delete();
	}

	%read.openForRead("config/server/Left4Block/itemscavenge.txt");

	while(!%read.isEOF())
	{
		%i++;
		%line = %read.readLine(); 

		%itemremoveword = strreplace(%line, getWord(%line,0) @ " ", "");
		%previousline[%i] = getWord(%line,0);

		if(%previousline[%i] $= %previousline[mClamp(%i-1, 1, %i)])
		{
			%j++;
			eval("$" @ getWord(%line,0) @"[%j] = \"" @ %itemremoveword @ "\";");
			eval("$" @ getWord(%line,0) @"Amount = %j;");
		}
		else 
		{
			eval("$" @ getWord(%line,0) @"[1] = \"" @ %itemremoveword @ "\";");
			%j = 1;
		}

		for (%d = 0; %d < DatablockGroup.getCount(); %d++) 
		{
			%datablock = DatablockGroup.getObject(%d);

			if(%datablock.getClassName() $= "ItemData")
			if(%datablock.uiName $= %itemremoveword)
			{	
				%item = %datablock;
				eval("$" @ getWord(%line,0) @"[%j] = \"" @ %item.getName() @ "\";");
			}
		}
	}
	%read.close();
	%read.delete();
}

function configLoadL4BItemSlots()
{
	%read = new FileObject();
	if(!isFile("config/server/Left4Block/itemslots.txt"))
	{
		%read.openForRead("Add-Ons/Gamemode_Left4Block/config/itemslots.txt");
		%write = new FileObject();
		%write.openForWrite("config/server/Left4Block/itemslots.txt");
		
		while(!%read.isEOF())
		{
			%line = %read.readLine();
			%write.writeLine(%line);
		}
		
		%read.close();
		%write.close();
		%write.delete();
	}
	
	%read.openForRead("config/server/Left4Block/itemslots.txt");
	
	while(!%read.isEOF())
	{
		%i++;
		%line = %read.readLine(); 

		%firstword = getWord(%line,0);
		%itemremoveword = strreplace(%line, %firstword @ " ", "");

		for (%d = 0; %d < DatablockGroup.getCount(); %d++) 
		{
			%datablock = DatablockGroup.getObject(%d);
			if(%datablock.getClassName() $= "ItemData" && %datablock.uiName $= %itemremoveword) %datablock.L4Bitemslot = %firstword;
		}
	}
	%read.close();
	%read.delete();
}

configLoadL4BItemScavenge();
configLoadL4BItemSlots(); 

$DecalSystem::DecalLimit = 200;
$DecalSystem::DecalTimeout = 300000;

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
		category       = "General";
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
		category       = "Interactive Bricks";
		title          = "Ammo crate delay (ms)";

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
		category       = "Interactive Bricks";
		title          = "Health locker delay (ms)";

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
	
	new ScriptObject(Preference)
	{
		className      = "Gamemode_Left4Block_Blood";
		addon          = "Gamemode_Left4Block";
		category       = "Blood";
		title          = "Enable blood";

		type           = "bool";
		params         = "";

		variable       = "$Pref::Server::L4BBlood::Enable";

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
		className      = "Gamemode_Left4Block_Blood";
		addon          = "Gamemode_Left4Block";
		category       = "Blood";
		title          = "Blood Decals";

		type           = "bool";
		params         = "";

		variable       = "$Pref::Server::L4BBlood::BloodDecals";

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
		className      = "Gamemode_Left4Block_Blood";
		addon          = "Gamemode_Left4Block";
		category       = "Blood";
		title          = "Blood Decals Drip";

		type           = "bool";
		params         = "";

		variable       = "$Pref::Server::L4BBlood::BloodDecalsDrip";

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
		className      = "Gamemode_Left4Block_Blood";
		addon          = "Gamemode_Left4Block";
		category       = "Blood";
		title          = "Blood Decals Limit";
		type           = "num";
		params         = "0 250 0";
		variable       = "$Pref::Server::L4BBlood::BloodDecalsLimit";
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
		className      = "Gamemode_Left4Block_Blood";
		addon          = "Gamemode_Left4Block";
		category       = "Blood";
		title          = "Blood Decals Timeout (ms)";
		type           = "num";
		params         = "0 30000 0";
		variable       = "$Pref::Server::L4BBlood::BloodDecalsTimeout";
		defaultValue   = "15000";
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
	$Pref::Server::L4BBlood::Enable = true;
	$Pref::Server::L4BBlood::BloodDecals = true;
	$Pref::Server::L4BBlood::BloodDecalsDrip = true;
	$Pref::Server::L4BAmmocrate::BloodDecalsLimit = 250;
	$Pref::Server::L4BAmmocrate::BloodDecalsTimeout = 15000;
	$Pref::Server::L4BAmmocrate::AcquireDelay = 500;
	$Pref::L4BHealthLocker::AcquireDelay = 500;
	$Pref::L4BDirector::EnableOnMG = false;	
	$Pref::Server::L4B2Bots::Difficulty = 1;
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
				$Pref::Server::L4B2Bots::TankHealth = 2500;
				$Pref::Server::L4B2Bots::SurvivorImmunity = 1;
				$Pref::Server::L4B2Bots::MaxSpecial = 4;
				$Pref::Server::L4B2Bots::MaxHorde = 60;
				$Pref::Server::L4B2Bots::MaxTank = 1;

		case 1: $Pref::Server::L4B2Bots::NormalDamage = 10;
				$Pref::Server::L4B2Bots::SpecialsDamage = 25;
				$Pref::Server::L4B2Bots::TankRounds = 2;
				$Pref::Server::L4B2Bots::TankRoundChance = 60;
				$Pref::Server::L4B2Bots::TankHealth = 5000;
				$Pref::Server::L4B2Bots::SurvivorImmunity = 0;
				$Pref::Server::L4B2Bots::MaxSpecial = 8;
				$Pref::Server::L4B2Bots::MaxHorde = 35;
				$Pref::Server::L4B2Bots::MaxTank = 2;

		default:
	}

	eval("ZombieTankHoleBot.maxDamage =" @ $Pref::Server::L4B2Bots::TankHealth @ ";");
}

function Gamemode_Left4Block_Difficulty::onUpdate(%this, %val) { L4B_DifficultyAdjustment(); }