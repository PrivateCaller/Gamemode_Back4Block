forceRequiredAddOn("Bot_Hole");
forceRequiredAddOn("Support_Lua");
exec("./modules/support/support.cs");
exec("./modules/add-ins/player_l4b/player_l4b.cs");
exec("./modules/add-ins/script_l4b/script_l4b.cs");
exec("./modules/add-ins/item_l4b/item_l4b.cs");
exec("./modules/add-ins/weapon_l4b/weapon_l4b.cs");
exec("./modules/add-ins/brick_l4b/brick_l4b.cs");

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

function GunImages_GenerateSideImages()
{
	for(%i = 0; %i < DatablockGroup.getCount(); %i++)	
	if(isObject(%item = DatablockGroup.getObject(%i)))
	{
		if(%item.L4Bitemslot $= "Secondary")
		{
			if(%item.image.meleeDamageDivisor) %mount = 2;
			else %mount = 1;

			%eval = %eval @ "datablock ShapeBaseImageData(" @ %item.image.getName() @ "GunImagesSIDE)";
			%eval = %eval @ "{";
				%eval = %eval @ "shapeFile = \"" @ %item.image.shapeFile @ "\";";
				%eval = %eval @ "doColorShift = " @ (%item.image.doColorShift && 1) @ ";";
				%eval = %eval @ "colorShiftColor = \"" @ %item.image.colorShiftColor @ "\";";
				%eval = %eval @ "offset = \"0 0 0\";";
				%eval = %eval @ "mountPoint = " @ %mount @ ";";
			%eval = %eval @ "};";
			eval(%eval);
		}

		if(%item.L4Bitemslot $= "Grenade")
		{			
			%eval = %eval @ "datablock ShapeBaseImageData(" @ %item.image.getName() @ "GunImagesSIDE)";
			%eval = %eval @ "{";
				%eval = %eval @ "shapeFile = \"" @ %item.image.shapeFile @ "\";";
				%eval = %eval @ "doColorShift = " @ (%item.image.doColorShift && 1) @ ";";
				%eval = %eval @ "colorShiftColor = \"" @ %item.image.colorShiftColor @ "\";";
				%eval = %eval @ "offset = \"0 0 0\";";
				%eval = %eval @ "mountPoint = 3;";
			%eval = %eval @ "};";
			eval(%eval);
		}
	}	

}
GunImages_GenerateSideImages();