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