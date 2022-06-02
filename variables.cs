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

$hTankHealth = 2000;

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