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
		variable       = "$Pref::L4B::Zombies::Difficulty";
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

		variable       = "$Pref::L4B::Director::EnableOnMG";

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
		className      = "Gamemode_Left4Block_Blood";
		addon          = "Gamemode_Left4Block";
		category       = "Blood";
		title          = "Enable blood";

		type           = "bool";
		params         = "";

		variable       = "$Pref::L4B::Blood::Enable";

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

		variable       = "$Pref::L4B::Blood::BloodDecals";

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
		params         = "0 300 0";
		variable       = "$Pref::L4B::Blood::BloodDecalsLimit";
		defaultValue   = "150";
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
		variable       = "$Pref::L4B::Blood::BloodDecalsTimeout";
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
	$Pref::L4B::Blood::Enable = true;
	$Pref::L4B::Blood::BloodDecals = true;
	$Pref::L4B::Blood::BloodDecalsLimit = 300;
	$Pref::L4B::Blood::BloodDecalsTimeout = 15000;
	$Pref::L4B:MapRotation::Enabled = true;
	$Pref::L4B:MapRotation::RequiredVotes = 0;
	$Pref::L4B:MapRotation::RequiredNext = 2;
	$Pref::L4B:MapRotation::RequiredReload = 2;
	$Pref::L4B:MapRotation::VoteMin = 5;
	$Pref::L4B:MapRotation::MinReset = 5;
	$Pref::L4B:MapRotation::CoolDown = 10;	
	$Pref::L4B::L4B::Director::EnableOnMG = false;	
	$Pref::L4B::Zombies::Difficulty = 1;
}

function L4B_DifficultyAdjustment()
{
	switch($Pref::L4B::Zombies::Difficulty)
	{
		case 0: $Pref::L4B::Zombies::NormalDamage = 5;
				$Pref::L4B::Zombies::SpecialsDamage = $Pref::L4B::Zombies::NormalDamage*3;
				$Pref::L4B::Zombies::TankRounds = 1;
				$Pref::L4B::Zombies::TankRoundChance = 50;
				$Pref::L4B::Zombies::TankHealth = 5000;
				$Pref::L4B::Zombies::SurvivorImmunity = 1;
				$Pref::L4B::Zombies::MaxSpecial = 4;
				$Pref::L4B::Zombies::MaxHorde = 50;
				$Pref::L4B::Zombies::MaxTank = 1;

		case 1: $Pref::L4B::Zombies::NormalDamage = 10;
				$Pref::L4B::Zombies::SpecialsDamage = $Pref::L4B::Zombies::NormalDamage*4;
				$Pref::L4B::Zombies::TankRounds = 2;
				$Pref::L4B::Zombies::TankRoundChance = 75;
				$Pref::L4B::Zombies::TankHealth = 10000;
				$Pref::L4B::Zombies::SurvivorImmunity = 0;
				$Pref::L4B::Zombies::MaxSpecial = 8;
				$Pref::L4B::Zombies::MaxHorde = 75;
				$Pref::L4B::Zombies::MaxTank = 2;

		default:
	}
	eval("ZombieTankHoleBot.maxDamage =" @ $Pref::L4B::Zombies::TankHealth @ ";");
}
L4B_DifficultyAdjustment();


function Gamemode_Left4Block_Difficulty::onUpdate(%this, %val) { L4B_DifficultyAdjustment(); }