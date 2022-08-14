if(LoadRequiredAddOn("Weapon_SWeps_EXT") == $Error::None)
$Item_Explosives_isSWepsExtOn = 1;
else $Item_Explosives_isSWepsExtOn = 0;

%pattern = "add-ons/gamemode_left4block/add-ins/weapon_throwable_explosives/sound/*.wav";//Too lazy to write datablock files for the sounds, just took this from the Disease Gamemode
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/gamemode_left4block/add-ins/weapon_throwable_explosives/sound/", "");
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

datablock ItemData(PropaneTankItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/propanetankbox.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Propane Tank";
	iconName = "./icons/Icon_Propane";
	doColorShift = true;
	colorShiftColor = "0.471 0.471 0.471 1.000";

	image = PropaneTankImage;
	canDrop = true;
};

function PropaneTankItem::onPickup(%this, %obj, %user, %amount)
{  
	Parent::onPickup(%this, %obj, %user, %amount);
	
	for(%i=0;%i<%user.getDatablock().maxTools;%i++)
	{
		%toolDB = %user.tool[%i];
		if(%toolDB $= %this.getID())
		{
			servercmdUseTool(%user.client,%i);
			break;
		}
	}
}

datablock ShapeBaseImageData(PropaneTankImage)
{
   	shapeFile = "./models/propanetank.dts";
   	emap = true;
   	vehicle = PropaneTankCol;

   	mountPoint = 0;
   	offset = "-0.53 0.05 -0.6";
   	rotation = "0 0 1 180";

   	correctMuzzleVector = true;
   	className = "WeaponImage";

   	item = PropaneTankItem;
   	ammo = " ";
   	projectile = "";
   	projectileType = "";

   	melee = false;
   	armReady = true;

	doColorShift = true;
	colorShiftColor = "0.471 0.471 0.471 1.000";

	stateName[0]			= "Activate";
	stateTimeoutValue[0]		= 0.1;
	stateTransitionOnTimeout[0]	= "Ready";
	stateSequence[0]		= "ready";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]			= "Ready";
	stateTransitionOnTriggerDown[1]	= "Fire";
	stateAllowImageChange[1]	= true;

	stateName[5]			= "Fire";
	stateTransitionOnTimeout[5]	= "Ready";
	stateTimeoutValue[5]		= 0.5;
	stateFire[5]			= true;
	stateSequence[5]		= "fire";
	stateScript[5]			= "onFire";
	stateWaitForTimeout[5]		= true;
	stateAllowImageChange[5]	= false;
};

function PropaneTankImage::onFire(%this, %obj, %slot)
{
	if(!isObject(getMinigamefromObject(%obj)))
	return;
	
	%currSlot = %obj.currTool;
	%obj.tool[%currSlot] = 0;
	%obj.weaponCount--;
	messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
	serverCmdUnUseTool(%obj.client);
	
	%obj.playThread(2, shiftdown);

	%obj.sourcerotation = %obj.gettransform();
	%muzzlepoint = %obj.getHackPosition();
	%muzzlevector = vectorScale(%obj.getEyeVector(),3);
	%muzzlepoint = VectorAdd (%muzzlepoint, %muzzlevector);
	%playerRot = rotFromTransform (%obj.getTransform());

	%item = new WheeledVehicle(ExplosiveItemVehicle) 
	{  
		rotation = getwords(%obj.sourcerotation,3,6);
		datablock  = %this.vehicle;
		sourceObject = %obj.client.player;
		minigame = getMinigameFromObject(%obj);
		brickGroup = %obj.client.brickGroup;
	};

	%item.schedule(60000,delete);
	%item.startfade(5000,55000,1);
	%item.setTransform (%muzzlepoint @ " " @ %playerRot);
	%item.applyimpulse(%item.gettransform(),vectoradd(vectorscale(%obj.getEyeVector(),10000),"0" SPC "0" SPC 5000));

	%obj.playThread(3,jump);
	%obj.playThread(2,activate2);
}

function PropaneTankCol::onAdd(%this,%obj)
{
	%obj.setNodeColor("ALL",%this.image.item.colorShiftColor);
	
	Parent::onAdd(%this,%obj);
}
function PropaneTankCol::onCollision(%this, %obj, %col, %fade, %pos, %norm)
{
	Parent::onCollision(%this, %obj, %col, %fade, %pos, %norm);
}

function PropaneTankCol::onDamage(%this,%obj)
{
	%c = new projectile()
	{
		datablock = PropaneTankFinalExplosionProjectile;
		initialPosition = %obj.getPosition();
		client = %obj.creator.client;
		sourceObject = %obj.sourceObject;
		damageType = $DamageType::Boomer;
	};

	Parent::onDamage(%this,%obj);
}

function PropaneTankImage::onMount(%this,%obj,%slot)
{
	Parent::onMount(%this,%obj,%slot);	
	%obj.playThread(1, armreadyboth);
}

function PropaneTankImage::onUnMount(%this,%obj,%slot)
{
	Parent::onUnMount(%this,%obj,%slot);	
	%obj.playThread(2, root);
	for(%i=0;%i<%obj.getDatablock().maxTools;%i++)
	{
		%toolDB = %obj.tool[%i];
		if(%toolDB $= %this.item.getID())
		{
			servercmdDropTool(%obj.client,%i);
			break;
		}
	}
}

datablock WheeledVehicleData(PropaneTankCol)
{
	category = "Vehicles";
	displayName = "";
	shapeFile = "./models/PropaneTankCol.dts";
	emap = true;
	minMountDist = 0;
   
   	numMountPoints = 0;

	image = PropaneTankImage;
	maxDamage = 1;
	destroyedLevel = 1;
	energyPerDamagePoint = 1;
	speedDamageScale = 1.04;
	collDamageThresholdVel = 20.0;
	collDamageMultiplier   = 0.02;
    doColorShift = true;
    colorShiftColor = "0.471 0.471 0.471 1.000";
	massCenter = "0 0 0";

	maxSteeringAngle = 0.9785;
	integration = 4;

	cameraRoll = false;
	cameraMaxDist = 13;
	cameraOffset = 7.5;
	cameraLag = 0.0;   
	cameraDecay = 0.75;
	cameraTilt = 0.4;
   	collisionTol = 0.1; 
   	contactTol = 0.1;

	useEyePoint = false;	


	numWheels = 0;

	mass = 800;
	density = 5.0;
	drag = 1.6;
	bodyFriction = 0.6;
	bodyRestitution = 0.6;
	minImpactSpeed = 10;
	softImpactSpeed = 10;
	hardImpactSpeed = 15;
	groundImpactMinSpeed    = 10.0;

	// Engine
	engineTorque = 12000;
	engineBrake = 2000; 
	brakeTorque = 50000;
	maxWheelSpeed = 0;

	rollForce		= 900;
	yawForce		= 600;
	pitchForce		= 1000;
	rotationalDrag		= 0.2;

	// Energy
	maxEnergy = 5;
	jetForce = 3000;
	minJetEnergy = 30;
	jetEnergyDrain = 2;

	splash = PropaneTankSplash;
	splashVelocity = 4.0;
	splashAngle = 67.0;
	splashFreqMod = 300.0;
	splashVelEpsilon = 0.60;
	bubbleEmitTime = 1.4;

	mediumSplashSoundVelocity = 10.0;   
	hardSplashSoundVelocity = 20.0;   
	exitSplashSoundVelocity = 5.0;

	softImpactSound = slowImpactSound;
	hardImpactSound = fastImpactSound;

	justcollided = 0;

   	uiName = "";
	rideable = true;
	lookUpLimit = 0.65;
	lookDownLimit = 0.45;

	paintable = true;
   
	damageEmitterOffset[0] = "0.0 0.0 0.0 ";
	damageLevelTolerance[0] = 0.99;

	damageEmitterOffset[1] = "0.0 0.0 0.0 ";
	damageLevelTolerance[1] = 1.0;

   	numDmgEmitterAreas = 1;

  	initialExplosionProjectile = "";
	finalExplosionProjectile = "";
   	finalExplosionOffset = 0;

   	burnTime = 100;

   	minRunOverSpeed    = 15;
   	runOverDamageScale = 5;
   	runOverPushScale   = 2;
};

datablock ExplosionData(PropaneTankFinalExplosion : vehicleFinalExplosion)
{
   //explosionShape = "";
   lifeTimeMS = 150;

   debris = "";
   debrisNum = 1;
   debrisNumVariance = 0;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 20;
   debrisVelocity = 18;
   debrisVelocityVariance = 3;

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = true;
   camShakeFreq = "7.0 8.0 7.0";
   camShakeAmp = "10.0 10.0 10.0";
   camShakeDuration = 0.75;
   camShakeRadius = 15.0;

   // Dynamic light
   lightStartRadius = 0;
   lightEndRadius = 20;
   lightStartColor = "0.45 0.3 0.1";
   lightEndColor = "0 0 0";

   //impulse
   impulseRadius = 10;
   impulseForce = 1000;
   impulseVertical = 2000;

   //radius damage
   radiusDamage        = 1000;
   damageRadius        = 10.0;

   //burn the players?
   playerBurnTime = 4000;

};

datablock ProjectileData(PropaneTankFinalExplosionProjectile : vehicleFinalExplosionProjectile)
{
   directDamage        = 0;
   radiusDamage        = 0;
   damageRadius        = 0;
   explosion           = PropaneTankFinalExplosion;

   directDamageType  = $DamageType::PropaneTankExplosion;
   radiusDamageType  = $DamageType::PropaneTankExplosion;

    brickExplosionRadius = 8;
   brickExplosionImpact = false;          //destroy a brick if we hit it directly?
   brickExplosionForce  = 30;             
   brickExplosionMaxVolume = 30;          //max volume of bricks that we can destroy
   brickExplosionMaxVolumeFloating = 60;  //max volume of bricks that we can destroy if they aren't connected to the ground (should always be >= brickExplosionMaxVolume)

   explodeOnDeath		= 1;

   armingDelay         = 0;
   lifetime            = 10;
};



datablock ItemData(GasCanItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./models/GasCan.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Gas Can";
	iconName = "./icons/Icon_GasCan";
	doColorShift = true;
	colorShiftColor = "0.4 0.071 0.071 1.000";

	 // Dynamic properties defined by the scripts
	image = GasCanImage;
	canDrop = true;
};
function GasCanItem::onPickup(%this, %obj, %player)
{  
	PropaneTankItem::onPickup(%this, %obj, %player);
}

datablock ShapeBaseImageData(GasCanImage)
{
	shapeFile = "./models/GasCan.dts";
	emap = true;

	mountPoint = 0;
	offset = "-0.53 0.05 -0.7";
	rotation = "0 0 1 90";
	vehicle = GasCanCol;

	correctMuzzleVector = true;
	className = "WeaponImage";

	// Projectile && Ammo.
	item = GasCanItem;
	ammo = " ";
	projectile = gunProjectile;
	projectileType = Projectile;

	melee = false;
	armReady = true;

	doColorShift = true;
	colorShiftColor = GasCanItem.colorShiftColor;

	stateName[0]			= "Activate";
	stateTimeoutValue[0]		= 0.1;
	stateTransitionOnTimeout[0]	= "Ready";
	stateSequence[0]		= "ready";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]			= "Ready";
	stateTransitionOnTriggerDown[1]	= "Fire";
	stateAllowImageChange[1]	= true;

	stateName[5]			= "Fire";
	stateTransitionOnTimeout[5]	= "Ready";
	stateTimeoutValue[5]		= 0.5;
	stateFire[5]			= true;
	stateSequence[5]		= "fire";
	stateScript[5]			= "onFire";
	stateWaitForTimeout[5]		= true;
	stateAllowImageChange[5]	= false;
};

function GasCanImage::onFire(%this, %obj, %slot)
{
	PropaneTankImage::onFire(%this, %obj, %slot);
}
datablock WheeledVehicleData(GasCanCol : PropaneTankCol)
{
	shapeFile = "./models/GasCanCol.dts";
	image = GasCanImage;
};
function GasCanCol::onAdd(%this,%obj)
{
	PropaneTankCol::onAdd(%this,%obj);
}
function GasCanCol::oncollision(%this, %obj, %col, %fade, %pos, %norm)
{
	propaneTankCol::oncollision(%this, %obj, %col, %fade, %pos, %norm);
}
function GasCanCol::onDamage(%this,%obj)
{
	if($Item_Explosives_isSWepsExtOn)
	createFireCircle(%obj.getPosition(),30,70,%obj.sourceobject,%obj,$DamageType::Molotov);
}
function GasCanImage::onMount(%this,%obj,%slot)
{
	PropaneTankImage::onMount(%this,%obj,%slot);
}

function GasCanImage::onUnMount(%this,%obj,%slot)
{
	PropaneTankImage::onUnMount(%this,%obj,%slot);
}



datablock ItemData(BileOGasItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./models/Jug.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Gasbile Bottle";
	iconName = "./icons/Icon_BileOGas";
	doColorShift = false;
	colorShiftColor = "0.4 0.071 0.071 1.000";

	 // Dynamic properties defined by the scripts
	image = BileOGasImage;
	canDrop = true;
};
function BileOGasItem::onPickup(%this, %obj, %player)
{  
	PropaneTankItem::onPickup(%this, %obj, %player);
}

datablock ShapeBaseImageData(BileOGasImage)
{
	shapeFile = "./models/Jug.dts";
	emap = true;

	mountPoint = 0;
	offset = "-0.25 0 0";
	rotation = "0 0 1 90";
	vehicle = BileOGasCol;

	correctMuzzleVector = true;
	className = "WeaponImage";

	// Projectile && Ammo.
	item = BileOGasItem;
	ammo = " ";
	projectile = gunProjectile;
	projectileType = Projectile;

	melee = false;
	armReady = true;

	doColorShift = false;
	colorShiftColor = BileOGasItem.colorShiftColor;

	stateName[0]			= "Activate";
	stateTimeoutValue[0]		= 0.1;
	stateTransitionOnTimeout[0]	= "Ready";
	stateSequence[0]		= "ready";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]			= "Ready";
	stateTransitionOnTriggerDown[1]	= "Fire";
	stateAllowImageChange[1]	= true;

	stateName[5]			= "Fire";
	stateTransitionOnTimeout[5]	= "Ready";
	stateTimeoutValue[5]		= 0.5;
	stateFire[5]			= true;
	stateSequence[5]		= "fire";
	stateScript[5]			= "onFire";
	stateWaitForTimeout[5]		= true;
	stateAllowImageChange[5]	= false;
};

function BileOGasImage::onFire(%this, %obj, %slot)
{
	PropaneTankImage::onFire(%this, %obj, %slot);
}
datablock WheeledVehicleData(BileOGasCol : PropaneTankCol)
{
	shapeFile = "./models/JugCol.dts";
	image = BileOGasImage;
	DistractionRadius = 100;
};

function BileOGasCol::onAdd(%this,%obj)
{
	Parent::onAdd(%this,%obj);
	
	%obj.ContinueSearch = %obj.getDatablock().schedule(500,Distract,%obj);
	%obj.setScale("1 1 1");
}

function BileOGasCol::onDamage(%this,%obj)
{
	if($Item_Explosives_isSWepsExtOn)
	createFireCircle(%obj.getPosition(),10,10,%obj.sourceObject,%obj,$DamageType::Molotov);

	%c = new projectile()
	{
		datablock = BoomerProjectile;
		initialPosition = %obj.getPosition();
		client = %obj.sourceObject.client;
		sourceObject = %obj.sourceObject;
		damageType = $DamageType::Boomer;
	};
}

function BileOGasCol::oncollision(%this, %obj, %col, %fade, %pos, %norm)
{
	propaneTankCol::oncollision(%this, %obj, %col, %fade, %pos, %norm);
}

function BileOGasImage::onMount(%this,%obj,%slot)
{
	Parent::onMount(%this,%obj,%slot);
}

function BileOGasCol::Distract(%this, %obj)
{	
	if(!isObject(%obj))
	return;

	cancel(%obj.ContinueSearch);
	%obj.ContinueSearch = %obj.getDatablock().schedule(1000,Distract,%obj);

	%pos = %obj.getPosition();
	%radius = %this.DistractionRadius;

	%searchMasks = $TypeMasks::PlayerObjectType;
	InitContainerRadiusSearch(%pos, %radius, %searchMasks);

	while((%targetid = containerSearchNext()) != 0 )
	{
		if(!%targetid.getState() !$= "Dead" && %targetid.getClassName() $= "AIPlayer" && %targetid.hZombieL4BType && %targetid.hZombieL4BType < 5 && !%targetid.isBurning)
		{
			if(!%targetid.Distraction)
			{
				%targetid.Distraction = %obj.getID();
				%targetid.hSearch = 0;
			}
			else if(%targetid.Distraction == %obj.getID())
			{
				%targetid.setaimobject(%obj);
				%targetid.setmoveobject(%obj);
				%time1 = getRandom(1000,4000);
				%targetid.getDataBlock().schedule(%time1,onBotFollow,%targetid,%obj);
			}
		}
	}
}

function BileOGasImage::onUnMount(%this,%obj,%slot)
{
	PropaneTankImage::onUnMount(%this,%obj,%slot);
}

datablock ItemData(OxygenTankItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./models/OxygenTank.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Oxygen Tank";
	iconName = "./icons/Icon_OxygenTank";
	doColorShift = true;
	colorShiftColor = "0.4 0.071 0.071 1.000";

	 // Dynamic properties defined by the scripts
	image = OxygenTankImage;
	canDrop = true;
};

function OxygenTankItem::onPickup(%this, %obj, %player)
{  
	PropaneTankItem::onPickup(%this, %obj, %player);
}

datablock ShapeBaseImageData(OxygenTankImage)
{
	shapeFile = "./models/OxygenTank.dts";
	emap = true;

	mountPoint = 0;
	offset = "-0.53 0.05 -0.7";
	rotation = "0 0 1 90";
	vehicle = OxygenTankCol;

	correctMuzzleVector = true;
	className = "WeaponImage";

	// Projectile && Ammo.
	item = OxygenTankItem;
	ammo = " ";
	projectile = gunProjectile;
	projectileType = Projectile;

	melee = false;
	armReady = true;

	doColorShift = true;
	colorShiftColor = OxygenTankItem.colorShiftColor;

	stateName[0]			= "Activate";
	stateTimeoutValue[0]		= 0.1;
	stateTransitionOnTimeout[0]	= "Ready";
	stateSequence[0]		= "ready";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]			= "Ready";
	stateTransitionOnTriggerDown[1]	= "Fire";
	stateAllowImageChange[1]	= true;

	stateName[5]			= "Fire";
	stateTransitionOnTimeout[5]	= "Ready";
	stateTimeoutValue[5]		= 0.5;
	stateFire[5]			= true;
	stateSequence[5]		= "fire";
	stateScript[5]			= "onFire";
	stateWaitForTimeout[5]		= true;
	stateAllowImageChange[5]	= false;
};

function OxygenTankImage::onFire(%this, %obj, %slot)
{
	PropaneTankImage::onFire(%this, %obj, %slot);
}

datablock WheeledVehicleData(OxygenTankCol : PropaneTankCol)
{
	shapeFile = "./models/OxygenTankCol.dts";
	maxDamage = 50;
	image = OxygenTankImage;
	DistractionRadius = 50;
};

function OxygenTankCol::onAdd(%this,%obj)
{
	PropaneTankCol::onAdd(%this,%obj);
}

function OxygenTankCol::oncollision(%this, %obj, %col, %fade, %pos, %norm)
{
	PropaneTankCol::oncollision(%this, %obj, %col, %fade, %pos, %norm);
}

function OxygenTankCol::onDamage(%this,%obj)
{	
	if(!isEventPending(%obj.AboutToExplode))
	{
		%obj.AboutToExplode = %obj.schedule(1900,Damage,%obj,%obj.getPosition(),%this.maxDamage,$DamageType::Suicide);
		%obj.playaudio(3,oxygentank_leak_sound);
		%obj.ContinueSearch = %obj.getDatablock().schedule(250,Distract,%obj);
	}

	if(%obj.getDamageLevel() >= %this.maxDamage)
	PropaneTankCol::onDamage(%this,%obj);

	Parent::onDamage(%this,%obj);
}

function OxygenTankCol::Distract(%this, %obj)
{	
	if(!isObject(%obj))
	return;

	cancel(%obj.ContinueSearch);
	%obj.ContinueSearch = %obj.getDatablock().schedule(500,Distract,%obj);

	%pos = %obj.getPosition();
	%radius = %this.DistractionRadius;

	%searchMasks = $TypeMasks::PlayerObjectType;
	InitContainerRadiusSearch(%pos, %radius, %searchMasks);

	while((%targetid = containerSearchNext()) != 0 )
	{
		if(!%targetid.getState() !$= "Dead" && %targetid.getClassName() $= "AIPlayer" && %targetid.hZombieL4BType && %targetid.hZombieL4BType < 5 && !%targetid.isBurning)
		{
			if(!%targetid.Distraction)
			{
				%targetid.Distraction = %obj.getID();
				%targetid.hSearch = 0;
			}
			else if(%targetid.Distraction == %obj.getID())
			{
				%targetid.setaimobject(%obj);
				%targetid.setmoveobject(%obj);
				%time1 = getRandom(1000,4000);
				%targetid.getDataBlock().schedule(%time1,onBotFollow,%targetid,%obj);
			}
		}
	}
}
function OxygenTankImage::onMount(%this,%obj,%slot)
{
	PropaneTankImage::onMount(%this,%obj,%slot);
}

function OxygenTankImage::onUnMount(%this,%obj,%slot)
{
	PropaneTankImage::onUnMount(%this,%obj,%slot);
}