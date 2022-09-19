luaexec("./weapon_melee.lua");
%pattern = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/sound/*.wav";//Too lazy to write datablock files for the sounds, just took this from the Disease Gamemode
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/sound/", "");
	%soundName = strreplace(%soundName, "/", "");
	%soundName = strreplace(%soundName, ".wav", "");
	%soundName = strreplace(%soundName, "quiet", "");
	%soundName = strreplace(%soundName, "normal", "");
	%soundName = strreplace(%soundName, "loud", "");

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
	lifetimeMS		= 1800;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;

	textureName		= "base/data/particles/dot";
	colors[0]	= "2 2 2 0.005";
	colors[1]	= "2 2 2 0.0";
	sizes[0]	= 0.75;
	sizes[1]	= 0.5;
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
	offset = "0 0.05 0.25";
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
	stateTimeoutValue[6]            = 0.25;
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

datablock ItemData(macheteItem : crowbarItem)
{
	shapeFile = "./models/model_machete.dts";
	uiName = "Machete";
	iconName = "./icons/icon_machete";
	colorShiftColor = "0.5 0.5 0.5 1";
	image = macheteImage;
};

datablock ShapeBaseImageData(macheteImage : crowbarImage)
{
	shapeFile = "./models/model_machete.dts";
	offset = "0 0.05 0.25";
	item = macheteItem;
	doColorShift = macheteItem.doColorShift;
	colorShiftColor = macheteItem.colorShiftColor;
	meleeDamageDivisor = 1.25;
	meleeHitEnvSound = "machete";
	meleeHitPlSound = "machete";
	stateTimeoutValue[6]            = 0.16;
};

function macheteImage::onReady(%this, %obj, %slot)
{
	crowbarImage::onReady(%this, %obj, %slot);
}

function macheteImage::onFire(%this, %obj, %slot)
{
	crowbarImage::onFire(%this, %obj, %slot);
}

function macheteImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

datablock ItemData(baseballbatItem : crowbarItem)
{
	shapeFile = "./models/model_baseballbat.dts";
	uiName = "baseballbat";
	iconName = "./icons/icon_baseballbat";
	colorShiftColor = "0.5 0.5 0.5 1";
	image = baseballbatImage;
};

datablock ShapeBaseImageData(baseballbatImage : crowbarImage)
{
	shapeFile = "./models/model_baseballbat.dts";
	offset = "0 0.05 0.25";
	item = baseballbatItem;
	doColorShift = baseballbatItem.doColorShift;
	colorShiftColor = baseballbatItem.colorShiftColor;
	meleeDamageDivisor = 2.5;
	meleeHitEnvSound = "baseballbat";
	meleeHitPlSound = "baseballbat";
	stateTimeoutValue[6]            = 0.16;
};

function baseballbatImage::onReady(%this, %obj, %slot)
{
	crowbarImage::onReady(%this, %obj, %slot);
}

function baseballbatImage::onFire(%this, %obj, %slot)
{
	crowbarImage::onFire(%this, %obj, %slot);
}

function baseballbatImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

datablock ItemData(fryingpanItem : crowbarItem)
{
	shapeFile = "./models/model_fryingpan.dts";
	uiName = "fryingpan";
	iconName = "./icons/icon_fryingpan";
	colorShiftColor = "0.5 0.5 0.5 1";
	image = fryingpanImage;
};

datablock ShapeBaseImageData(fryingpanImage : crowbarImage)
{
	shapeFile = "./models/model_fryingpan.dts";
	offset = "0 0.05 0.25";
	item = fryingpanItem;
	doColorShift = fryingpanItem.doColorShift;
	colorShiftColor = fryingpanItem.colorShiftColor;
	meleeDamageDivisor = 3;
	meleeHitEnvSound = "fryingpan";
	meleeHitPlSound = "fryingpan";
	stateTimeoutValue[6]            = 0.16;
};

function fryingpanImage::onReady(%this, %obj, %slot)
{
	crowbarImage::onReady(%this, %obj, %slot);
}

function fryingpanImage::onFire(%this, %obj, %slot)
{
	crowbarImage::onFire(%this, %obj, %slot);
}

function fryingpanImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

datablock ItemData(hatchetItem : crowbarItem)
{
	shapeFile = "./models/model_hatchet.dts";
	uiName = "hatchet";
	iconName = "./icons/icon_hatchet";
	colorShiftColor = "0.5 0.5 0.5 1";
	image = hatchetImage;
};

datablock ShapeBaseImageData(hatchetImage : crowbarImage)
{
	shapeFile = "./models/model_hatchet.dts";
	offset = "0 0.05 0.25";
	item = hatchetItem;
	doColorShift = hatchetItem.doColorShift;
	colorShiftColor = hatchetItem.colorShiftColor;
	meleeDamageDivisor = 1.25;
	meleeHitEnvSound = "crowbar";
	meleeHitPlSound = "machete";
	stateTimeoutValue[6]            = 0.16;
};

function hatchetImage::onReady(%this, %obj, %slot)
{
	crowbarImage::onReady(%this, %obj, %slot);
}

function hatchetImage::onFire(%this, %obj, %slot)
{
	crowbarImage::onFire(%this, %obj, %slot);
}

function hatchetImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

datablock ItemData(shovelItem : crowbarItem)
{
	shapeFile = "./models/model_shovel.dts";
	uiName = "shovel";
	iconName = "./icons/icon_shovel";
	colorShiftColor = "0.5 0.5 0.5 1";
	image = shovelImage;
};

datablock ShapeBaseImageData(shovelImage : crowbarImage)
{
	shapeFile = "./models/model_shovel.dts";
	offset = "0 0.05 0.25";
	item = shovelItem;
	doColorShift = shovelItem.doColorShift;
	colorShiftColor = shovelItem.colorShiftColor;
	meleeDamageDivisor = 1.25;
	meleeHitEnvSound = "crowbar";
	meleeHitPlSound = "crowbar";
	stateTimeoutValue[6]            = 0.16;
};

function shovelImage::onReady(%this, %obj, %slot)
{
	crowbarImage::onReady(%this, %obj, %slot);
}

function shovelImage::onFire(%this, %obj, %slot)
{
	crowbarImage::onFire(%this, %obj, %slot);
}

function shovelImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

datablock ItemData(macheteItem : crowbarItem)
{
	shapeFile = "./models/model_machete.dts";
	uiName = "Machete";
	iconName = "./icons/icon_machete";
	colorShiftColor = "0.5 0.5 0.5 1";
	image = macheteImage;
};

datablock ShapeBaseImageData(batonImage : crowbarImage)
{
	shapeFile = "./models/model_baton.dts";
	offset = "0 0.05 0.25";
	item = batonItem;
	doColorShift = batonItem.doColorShift;
	colorShiftColor = batonItem.colorShiftColor;
	meleeDamageDivisor = 1.25;
	meleeHitEnvSound = "baseballbat";
	meleeHitPlSound = "baseballbat";
	stateTimeoutValue[6]            = 0.16;
};

function batonImage::onReady(%this, %obj, %slot)
{
	crowbarImage::onReady(%this, %obj, %slot);
}

function batonImage::onFire(%this, %obj, %slot)
{
	crowbarImage::onFire(%this, %obj, %slot);
}

function batonImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}