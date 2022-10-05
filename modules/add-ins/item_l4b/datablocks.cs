%pattern = "add-ons/gamemode_left4block/modules/add-ins/item_l4b/sound/*.wav";
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/gamemode_left4block/modules/add-ins/item_l4b/sound/", "");
	%soundName = strreplace(%soundName, "/", "");
	%soundName = strreplace(%soundName, ".wav", "");
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosest3d; filename = \"" @ %file @ "\"; };");

	%file = findNextFile(%pattern);
}


datablock ParticleData(HealParticle)
{
		dragCoefficient      = 5;
		gravityCoefficient   = -0.2;
		inheritedVelFactor   = 0;
		constantAcceleration = 0;
		lifetimeMS           = 1000;
		lifetimeVarianceMS   = 500;
		useInvAlpha          = false;
		textureName          = "./icons/heal";
		colors[0]     = "1 1 1 1";
		colors[1]     = "1 1 1 0.5";
		colors[2]     = "0 0 0 0";
		sizes[0]      = 0.4;
		sizes[1]      = 0.6;
		sizes[2]      = 0.4;
		times[0]      = 0;
		times[1]      = 0.2;
		times[2]      = 1;
};

datablock ParticleEmitterData(HealEmitter)
{
		ejectionPeriodMS = 35;
		periodVarianceMS = 0;
		ejectionVelocity = 0.5;
		ejectionOffset   = 1;
		velocityVariance = 0.49;
		thetaMin         = 0;
		thetaMax         = 120;
		phiReferenceVel  = 0;
		phiVariance      = 360;
		overrideAdvance = false;
		particles = "HealParticle";

		uiName = "";
};

datablock ShapeBaseImageData(HealImage)
{
		shapeFile = "base/data/shapes/empty.dts";
		emap = false;

		mountPoint = $HeadSlot;

		stateName[0]			= "Ready";
		stateTransitionOnTimeout[0]	= "FireA";
		stateTimeoutValue[0]		= 0.01;

		stateName[1]			= "FireA";
		stateTransitionOnTimeout[1]	= "Done";
		stateWaitForTimeout[1]		= True;
		stateTimeoutValue[1]		= 0.350;
		stateEmitter[1]			= HealEmitter;
		stateEmitterTime[1]		= 0.350;

		stateName[2]			= "Done";
		stateScript[2]			= "onDone";
};

function HealImage::onDone(%data, %player, %slot)
{
	%player.unMountImage(%slot);
}



datablock DebrisData(PillsHereDebris)
{
		shapeFile = "./models/cap.2.dts";
		lifetime = 5.0;
		minSpinSpeed = -400.0;
		maxSpinSpeed = 200.0;
		elasticity = 0.5;
		friction = 0.2;
		numBounces = 3;
		staticOnMaxBounce = true;
		snapOnMaxBounce = false;
		fade = true;
		gravModifier = 2;
};

datablock ItemData(PillsHereItem)
{
		category = "Weapon";
		className = "Weapon";
		
		shapeFile = "./models/pills.4.dts";
		rotate = false;
		mass = 1;
		density = 0.2;
		elasticity = 0.2;
		friction = 0.6;
		emap = true;
		
		uiName = "Pills";
		iconName = "./icons/Icon_Pills";
		doColorShift = true;
		colorShiftColor = "0.8 0.8 0.8 1";
		
		image = PillsHereImage;
		canDrop = true;
};

datablock ShapeBaseImageData(PillsHereImage)
{
		shapeFile = "./models/pills.4.dts";
		emap = true;
		mountPoint = 0;
		offset = "0 0 0";
		eyeOffset = 0;
		rotation = eulerToMatrix( "0 0 0" );
		
		className = "WeaponImage";
		item = PillsHereItem;
		isHealing = 1;
		
		armReady = true;
		
		doColorShift = PillsHereItem.doColorShift;
		colorShiftColor = PillsHereItem.colorShiftColor;
		
		casing = PillsHereDebris;
		shellExitDir		= "1.0 1.0 1.0";
		shellExitOffset		= "0 0 0";
		shellExitVariance	= 5;	
		shellVelocity		= 10;
		
		stateName[0]					= "Activate";
		stateScript[0]					= "onActivate";
		stateTimeoutValue[0]			= 0.15;
		stateTransitionOnTimeout[0]		= "Ready";

		stateName[1]					= "Ready";
		stateAllowImageChange[1]		= true;
		stateSequence[1]				= "Ready";
		stateTransitionOnTriggerDown[1]	= "Use";
		
		stateName[2]					= "Use";
		stateScript[2]					= "onUse";
		stateTransitionOnTriggerUp[2]	= "Done";
		
		stateName[3]					= "Done";
		stateTransitionOnAmmo[3]		= "Ready";
		stateTransitionOnNoAmmo[3]		= "UnUse";
		
		stateName[4]					= "UnUse";
		stateEjectShell[4]				= true;
		stateScript[4]					= "onUnUse";
		stateSequence[4]				= "Open";
		stateTimeoutValue[4]			= 0.5;
		stateTransitionOnTimeout[4]		= "Hack";
		stateWaitForTimeout[4]			= true;
		
		stateName[5]					= "Hack";
		stateScript[5]					= "onHack";
};

datablock ItemData(GauzeItem)
{
		category = "Weapon";
		className = "Weapon";
		
		shapeFile = "./models/gauze.dts";
		rotate = false;
		mass = 1;
		density = 0.2;
		elasticity = 0.2;
		friction = 0.6;
		emap = true;
		
		uiName = "Gauze";
		iconName = "./icons/Icon_Gauze";
		doColorShift = false;
		
		image = GauzeImage;
		canDrop = true;
		l4ditemtype = "heal_full";
};


datablock ShapeBaseImageData(GauzeImage)
{
		shapeFile = "./models/gauze.dts";
		emap = true;
		mountPoint = 0;
		offset = "0 0 0";
		eyeOffset = 0;
		rotation = eulerToMatrix("0 0 0");
		
		className = "WeaponImage";
		item = GauzeItem;
		isHealing = 1;
		
		armReady = true;
		doColorShift = false;
		
		stateName[0]					= "Activate";
		stateSound[0]					= "heal_stop_sound";
		stateTimeoutValue[0]			= 0.15;
		stateSequence[0]				= "Ready";
		stateTransitionOnTimeout[0]		= "Ready";

		stateName[1]					= "Ready";
		stateAllowImageChange[1]		= true;
		stateScript[1]					= "onReady";
		stateTransitionOnTriggerDown[1]	= "Use";
		
		stateName[2]					= "Use";
		stateScript[2]					= "onUse";
		stateTransitionOnTriggerUp[2]	= "Ready";
};

datablock ItemData(SyringeAntidoteItem)
{
	uiName = "Syringe Antidote";
	iconName = "./icons/icon_syringered";
	image = SyringeAntidoteImage;
	category = Weapon;
	className = Weapon;
	shapeFile = "./models/syringered.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0;
	friction = 0.6;
	emap = true;
	doColorShift = true;
	colorShiftColor = "1 1 1 1";
	canDrop = true;
	gc_syringe = 1;
};

datablock shapeBaseImageData(SyringeAntidoteImage)
{
	shapeFile = "./models/syringered.dts";
	emap = true;
	correctMuzzleVector = false;
	isHealing = 1;
	className = "WeaponImage";
	item = SyringeAntidoteItem;
	ammo = "";
	projectile = "";
	projectileType = Projectile;
	melee = false;
	doReaction = false;
	armReady = true;
	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	stateName[0] = "Activate";
	stateTimeoutValue[0] = 0.2;
	stateTransitionOnTimeout[0] = "Ready";
	stateSound[0] = "heal_syringe_pickup_sound";

	stateName[1] = "Ready";
	stateTransitionOnTriggerDown[1] = "Fire";
	stateAllowImageChange[1] = true;

	stateName[2] = "Fire";
	stateTransitionOnTimeout[2] = "Ready";
	stateTimeoutValue[2] = 0.2;
	stateFire[2] = true;
	stateScript[2] = "onFire";
	stateWaitForTimeout[2] = true;
	stateAllowImageChange[2] = false;
};

$Defibrillator::ChargeTime = 3.4; //Requires restart.
addDamageType("Defibrillator", '<bitmap:Add-Ons/Gamemode_left4block/modules/add-ins/item_l4b/icons/CI_Defib> %1', '%2 <bitmap:Add-Ons/Gamemode_left4block/modules/add-ins/item_l4b/icons/CI_Defib> %1', 0.5, 1);

datablock ProjectileData(DefibrillatorProjectile : radioWaveProjectile)
{
	directDamage = 125;
	directDamageType = $DamageType::Defibrillator;
	radiusDamageType = $DamageType::Defibrillator;

	collideWithPlayers = true;

	lifetime = 100;

	uiName = "";
};

datablock ItemData(DefibrillatorItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/Defib_Right.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Defibrillator";
	iconName = "./icons/icon_Defib";
	doColorShift = false;

	image = DefibrillatorImage;
	canDrop = true;
};

datablock ShapeBaseImageData(DefibrillatorImage)
{
	shapeFile = "./models/Defib_Right.dts";
	emap = true;

	mountPoint = 0;
	offset = "0 0.05 0.05";
	eyeOffset = 0;
	rotation = "1 0 0 0";

	correctMuzzleVector = true;

	className = "WeaponImage";

	item = DefibrillatorItem;
	ammo = " ";
	projectile = DefibrillatorProjectile;
	projectileType = Projectile;

	melee = false;
	armReady = true;

	doColorShift = false;

	stateName[0]			= "Activate";
	stateTimeoutValue[0]		= 0.5;
	stateTransitionOnTimeout[0]	= "Ready";
	stateSequence[0]		= "ready";
	stateSound[0]			= weaponSwitchSound;

	stateName[1]			= "Ready";
	stateTransitionOnTriggerDown[1]	= "Charge";
	stateAllowImageChange[1]	= true;
	
	stateName[2]                    = "Charge";
	stateTransitionOnTimeout[2]	= "Charge2";
	stateTimeoutValue[2]            = $Defibrillator::ChargeTime / 2;
	stateWaitForTimeout[2]		= false;
	stateTransitionOnTriggerUp[2]	= "AbortCharge";
	//stateScript[2]                  = "onCharge";
	stateAllowImageChange[2]        = false;
	stateSound[2]			= "defib_ready_sound";
	
	stateName[3]			= "AbortCharge";
	stateTransitionOnTimeout[3]	= "Ready";
	stateTimeoutValue[3]		= 0.5;
	stateWaitForTimeout[3]		= true;
	//stateScript[3]			= "onAbortedCharge";
	stateAllowImageChange[3]	= false;

	stateName[4]			= "Charge2";
	stateTimeoutValue[4]		= $Defibrillator::ChargeTime / 2;
	stateTransitionOnTimeout[4]	= "Fire";
	stateTransitionOnTriggerUp[4]	= "AbortCharge";
	//stateScript[4]		= "onCharge2";
	stateAllowImageChange[4]	= false;
	stateSound[4]			= "defib_charge_sound";

	stateName[5]			= "Fire";
	stateTransitionOnTimeout[5]	= "Ready";
	stateTimeoutValue[5]		= 0.5;
	stateFire[5]			= true;
	stateSequence[5]		= "fire";
	stateScript[5]			= "onFire";
	stateWaitForTimeout[5]		= true;
	stateAllowImageChange[5]	= false;
	stateSound[5]			= "defib_fire_sound";
};

datablock ShapeBaseImageData(LeftHandDefibrillatorImage : DefibrillatorImage)
{
	shapeFile = "./models/Defib_Left.dts";

	mountPoint = 1;

	armReady = false;

	stateName[0]			= "Activate";
	stateTimeoutValue[0]		= DefibrillatorImage.stateTimeoutValue[0];
	stateTransitionOnTimeout[0]	= "Ready";
	stateSequence[0]		= "ready";

	stateName[1]			= "Ready";
	stateTransitionOnTriggerDown[1]	= "Charge";
	stateAllowImageChange[1]	= true;
	
	stateName[2]                    = "Charge";
	stateTransitionOnTimeout[2]	= "Charge2";
	stateTimeoutValue[2]            = DefibrillatorImage.stateTimeoutValue[2];
	stateWaitForTimeout[2]		= false;
	stateTransitionOnTriggerUp[2]	= "AbortCharge";
	stateScript[2]                  = "onCharge";
	stateAllowImageChange[2]        = false;
	
	stateName[3]			= "AbortCharge";
	stateTransitionOnTimeout[3]	= "Ready";
	stateTimeoutValue[3]		= DefibrillatorImage.stateTimeoutValue[3];
	stateWaitForTimeout[3]		= true;
	stateScript[3]			= "onAbortedCharge";
	stateAllowImageChange[3]	= false;

	stateName[4]			= "Charge2";
	stateTimeoutValue[4]		= DefibrillatorImage.stateTimeoutValue[4];
	stateTransitionOnTimeout[4]	= "Fire";
	stateTransitionOnTriggerUp[4]	= "AbortCharge";
	stateScript[4]			= "onCharge2";
	stateAllowImageChange[4]	= false;

	stateName[5]			= "Fire";
	stateTransitionOnTimeout[5]	= "Ready";
	stateTimeoutValue[5]		= 1.25;
	stateFire[5]			= true;
	stateSequence[5]		= "fire";
	stateScript[5]			= "onFire";
	stateWaitForTimeout[5]		= true;
	stateAllowImageChange[5]	= false;
};
