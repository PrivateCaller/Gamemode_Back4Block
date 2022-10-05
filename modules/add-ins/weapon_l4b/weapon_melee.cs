luaexec("./weapon_melee.lua");

function crowbarImage::onReady(%this, %obj, %slot)
{
	if(%obj.getstate() $= "Dead") return;
	%obj.playthread(1, "root");
}

function crowbarImage::onFire(%this, %obj, %slot)
{
	LuaCall(Melee_SwingCheck,%obj,%this,%slot);
}

function crowbarImage::onPreFire(%this, %obj, %slot)
{
	if(%obj.getstate() $= "Dead") return;
	serverPlay3D("melee_swing_sound",%obj.gethackposition());
	%obj.playthread(1, "meleeRaise");
	%obj.playthread(2, "meleeSwing" @ getRandom(1,3));
}

function L4B_CreateMeleeItem(%doOverwrite,%name,%colorShiftColor,%damageDivisor,%rBlood,%HitEnvSound,%HitPlSound,%delay)
{
    %mainDirectory = "add-ons/gamemode_left4block/modules/add-ins/weapon_l4b";
	%itemData = "datablock ItemData(" @ %name @ "Item : crowbarItem){shapeFile = \"" @ %mainDirectory @ "/models/model_" @ %name @ ".dts\"; uiName = \"" @ %name @ "\"; iconName = \""  @ %mainDirectory @  "/icons/icon_" @ %name @ "\"; colorShiftColor = \"" @ %colorShiftColor @ "\"; image = \"" @ %name @ "Image\";};";
    %imageData = "datablock ShapeBaseImageData(" @ %name @ "Image : crowbarImage){shapeFile = \""  @ %mainDirectory @  "/models/model_" @ %name @ ".dts\"; item = \"" @ %name @ "Item\"; doColorShift = " @ %name @ "Item.doColorShift; colorShiftColor = " @ %name @ "Item.colorShiftColor; meleeDamageDivisor = " @ %damageDivisor @ "; damageType = $DamageType::" @ %name @ "; meleeHitEnvSound = \"" @ %HitEnvSound @ "\"; meleeHitPlSound = \"" @ %HitPlSound @ "\"; stateTimeoutValue[6] = " @ %delay @ ";};";
    %onReady = "function" SPC %name @ "Image::onReady(%this, %obj, %slot) {crowbarImage::onReady(%this, %obj, %slot);}";
	%onPreFire = "function" SPC %name @ "Image::onPreFire(%this, %obj, %slot) {crowbarImage::onPreFire(%this, %obj, %slot);}";
    %onFire = "function" SPC %name @ "Image::onFire(%this, %obj, %slot) {crowbarImage::onFire(%this, %obj, %slot);}";    

    %file = new fileObject();
    
	if(%doOverwrite)
	{ 
		%file.openForWrite("add-ons/gamemode_left4block/modules/add-ins/weapon_l4b/weapon_melee_extra.cs");
		%file.writeLine("//Don't edit anything, the function in weapon_melee.cs edits the stuff here");
	}
	else %file.openForAppend("add-ons/gamemode_left4block/modules/add-ins/weapon_l4b/weapon_melee_extra.cs");

	%file.writeLine("");
	%file.writeLine("AddDamageType(" @ %name @ ",'<bitmap:" @ %mainDirectory @ "/icons/ci_" @ %name @ "> %1','%2 <bitmap:" @ %mainDirectory @ "/icons/ci_" @ %name @ "> %1',0.2,1);");
	%file.writeLine("$DamageType::" @ %name @ ".rBlood = \"" @ %rBlood @ "\";");
    %file.writeLine(%itemData);
	%file.writeLine(%imageData);
    %file.writeLine(%onReady);
	%file.writeLine(%onPreFire);
    %file.writeLine(%onFire);
    
    %file.close();
    %file.delete();
}

L4B_CreateMeleeItem(true,"Machete","0.5 0.5 0.5 1",1,true,"Machete","Machete",0.275);
L4B_CreateMeleeItem(false,"cKnife","0.75 0.75 0.75 1",1,true,"Machete","cKnife",0.125);
L4B_CreateMeleeItem(false,"Hatchet","0.75 0.75 0.75 1",1,true,"Crowbar","Machete",0.35);
L4B_CreateMeleeItem(false,"Axe","0.75 0.75 0.5 1",1,true,"Crowbar","Machete",0.4);

L4B_CreateMeleeItem(false,"Spikebat","0.675 0.45 0.275 1",2,true,"Bat","Spikebat",0.3);
L4B_CreateMeleeItem(false,"Katana","0.75 0.75 0.75 1",2,true,"Machete","Machete",0.3);

L4B_CreateMeleeItem(false,"Bat","0.675 0.45 0.275 1",3,false,"Bat","Bat",0.325);
L4B_CreateMeleeItem(false,"Baton","0.125 0.125 0.125 1",2,false,"Bat","Bat",0.275);
L4B_CreateMeleeItem(false,"Shovel","0.5 0.5 0.5 1",3,false,"Crowbar","Crowbar",0.45);
L4B_CreateMeleeItem(false,"Sledgehammer","0.8 0.8 0.8 1",2,false,"Crowbar","Sledgehammer",0.4);
L4B_CreateMeleeItem(false,"Pan","0.375 0.375 0.375 1",3,false,"Pan","Pan",0.225);
exec("./weapon_melee_extra.cs");