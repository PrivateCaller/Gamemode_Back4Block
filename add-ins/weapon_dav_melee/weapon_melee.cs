luaexec("./weapon_melee.lua");
%pattern = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/sound/*.wav";
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strreplace(filename(strlwr(%file)), ".wav", "");
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

datablock ParticleData(meleeTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 1.5;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 1500;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;

	textureName		= "base/data/particles/dot";
	colors[0]	= "2 2 2 0.0025";
	colors[1]	= "2 2 2 0.0";
	sizes[0]	= 0.4;
	sizes[1]	= 0.1;
	times[0]	= 0.5;
	times[1]	= 0.1;
};

datablock ParticleEmitterData(meleeTrailEmitter)
{
   ejectionPeriodMS = 2;
   periodVarianceMS = 0;
   ejectionVelocity = 0; //0.25;
   velocityVariance = 0; //0.10;
   ejectionOffset = 0;
   thetaMin         = 0.0;
   thetaMax         = 90.0;  
   particles = meleeTrailParticle;
   useEmitterColors = true;
   uiName = "";
};

AddDamageType("Crowbar",'<bitmap:Add-Ons/Gamemode_Left4Block/add-ins/weapon_dav_melee/icons/ci_crowbar> %1','%2 <bitmap:Add-Ons/Gamemode_Left4Block/add-ins/weapon_dav_melee/icons/ci_crowbar> %1',0.2,1);

datablock ItemData(crowbarItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system
	shapeFile = "./models/model_crowbar.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	uiName = "Crowbar";
	iconName = "./icons/icon_crowbar";
	doColorShift = true;
	colorShiftColor = "0.5 0.5 0.5 1";
	image = crowbarImage;
	canDrop = true;
};

datablock ShapeBaseImageData(crowbarImage)
{
	shapeFile = "./models/model_crowbar.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	correctMuzzleVector = false;
	eyeOffset = "0 0 0";
	className = "WeaponImage";
	item = crowbarItem;
	ammo = " ";
	projectile = "";
	projectileType = Projectile;
	melee = true;
	hasLunge = true;
	doRetraction = false;
	armReady = false;
	doColorShift = crowbarItem.doColorShift;
	colorShiftColor = crowbarItem.colorShiftColor;

	meleeDamageDivisor = 2;
	meleeHitEnvSound = "crowbar";
	meleeHitPlSound = "crowbar";

	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0;
	stateTransitionOnTimeout[0]      = "Ready";
	stateSound[0]                    = WeaponSwitchsound;

	stateName[1]                     = "Ready";
	stateScript[1]                  = "onReady";
	stateTransitionOnTriggerDown[1]  = "PreFire";
	stateAllowImageChange[1]         = true;

	stateName[2]					= "PreFire";
	stateScript[2]                  = "onPreFire";
	stateAllowImageChange[2]        = false;
	stateTimeoutValue[2]            = 0.05;
	stateTransitionOnTimeout[2]     = "Fire";

	stateName[3]                    = "Fire";
	stateTransitionOnTimeout[3]     = "CheckFire";
	stateFire[3]                    = false;
	stateScript[3]                  = "onFire";
	stateTimeoutValue[3]            = 0.1;
	stateEmitter[3]					= "meleeTrailEmitter";
	stateEmitterNode[3]             = "muzzlePoint";
	stateEmitterTime[3]             = "0.225";

	stateName[4]			= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "StopFire";

	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Break";
	stateTimeoutValue[5]            = 0.1925;
	stateScript[5]                  = "onStopFire";
	stateEmitter[5]					= "";
	stateEmitterNode[5]             = "muzzlePoint";
	stateEmitterTime[5]             = "0.1";

	stateName[6]                    = "Break";
	stateTransitionOnTimeout[6]     = "Ready";
	stateTimeoutValue[6]            = 0.3;
};

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

function L4B_CreateMeleeItem(%doOverwrite,%name,%colorShiftColor,%damageDivisor,%HitEnvSound,%HitPlSound,%delay)
{
    %mainDirectory = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee";
	%itemData = "datablock ItemData(" @ %name @ "Item : crowbarItem){shapeFile = \"" @ %mainDirectory @ "/models/model_" @ %name @ ".dts\"; uiName = \"" @ %name @ "\"; iconName = \""  @ %mainDirectory @  "/icons/icon_" @ %name @ "\"; colorShiftColor = \"" @ %colorShiftColor @ "\"; image = \"" @ %name @ "Image\";};";
    %imageData = "datablock ShapeBaseImageData(" @ %name @ "Image : crowbarImage){shapeFile = \""  @ %mainDirectory @  "/models/model_" @ %name @ ".dts\"; item = \"" @ %name @ "Item\"; doColorShift = " @ %name @ "Item.doColorShift; colorShiftColor = " @ %name @ "Item.colorShiftColor; meleeDamageDivisor = " @ %damageDivisor @ "; meleeHitEnvSound = \"" @ %HitEnvSound @ "\"; meleeHitPlSound = \"" @ %HitPlSound @ "\"; stateTimeoutValue[6] = " @ %delay @ ";};";
    %onReady = "function" SPC %name @ "Image::onReady(%this, %obj, %slot) {crowbarImage::onReady(%this, %obj, %slot);}";
	%onPreFire = "function" SPC %name @ "Image::onPreFire(%this, %obj, %slot) {crowbarImage::onPreFire(%this, %obj, %slot);}";
    %onFire = "function" SPC %name @ "Image::onFire(%this, %obj, %slot) {crowbarImage::onFire(%this, %obj, %slot);}";    

    %file = new fileObject();
    
	if(%doOverwrite)
	{ 
		%file.openForWrite("add-ons/gamemode_left4block/add-ins/weapon_dav_melee/extraweapons.cs");
		%file.writeLine("//Don't edit anything, the function in weapon_melee.cs edits the stuff here");
	}
	else %file.openForAppend("add-ons/gamemode_left4block/add-ins/weapon_dav_melee/extraweapons.cs");

	%file.writeLine("");
	%file.writeLine("AddDamageType(" @ %name @ ",'<bitmap:" @ %mainDirectory @ "/icons/ci_" @ %name @ "> %1','%2 <bitmap:" @ %mainDirectory @ "/icons/ci_" @ %name @ "> %1',0.2,1);");
    %file.writeLine(%itemData);
	%file.writeLine(%imageData);
    %file.writeLine(%onReady);
	%file.writeLine(%onPreFire);
    %file.writeLine(%onFire);
    
    %file.close();
    %file.delete();
}

L4B_CreateMeleeItem(true,"Machete","0.5 0.5 0.5 1",1.275,"Machete","Machete",0.275);
L4B_CreateMeleeItem(false,"Katana","0.75 0.75 0.75 1",1.275,"Machete","Machete",0.3);
L4B_CreateMeleeItem(false,"cKnife","0.75 0.75 0.75 1",1.125,"Machete","cKnife",0.125);
L4B_CreateMeleeItem(false,"Bat","0.675 0.45 0.275 1",4,"Bat","Bat",0.325);
L4B_CreateMeleeItem(false,"Spikebat","0.675 0.45 0.275 1",1.25,"Bat","Spikebat",0.3);
L4B_CreateMeleeItem(false,"Baton","0.125 0.125 0.125 1",1.25,"Bat","Bat",0.275);
L4B_CreateMeleeItem(false,"Hatchet","0.75 0.75 0.75 1",1.375,"Crowbar","Machete",0.35);
L4B_CreateMeleeItem(false,"Axe","0.75 0.75 0.5 1",1.15,"Crowbar","Machete",0.5);
L4B_CreateMeleeItem(false,"Shovel","0.5 0.5 0.5 1",3,"Crowbar","Crowbar",0.45);
L4B_CreateMeleeItem(false,"Sledgehammer","0.8 0.8 0.8 1",1.25,"Crowbar","Sledgehammer",0.5);
L4B_CreateMeleeItem(false,"Pan","0.375 0.375 0.375 1",4,"Pan","Pan",0.225);
exec("./extraweapons.cs");