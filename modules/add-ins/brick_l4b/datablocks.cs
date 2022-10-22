%pattern = "add-ons/gamemode_left4block/modules/add-ins/brick_l4b/sound/*.wav";
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strreplace(filename(strlwr(%file)), ".wav", "");
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosest3d; filename = \"" @ %file @ "\"; };");

	%file = findNextFile(%pattern);
}

datablock ItemData(RAmmoCrateItem)
{
	category = "Item";  // Mission editor category
	//className = "Item"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./models/RotCoAmmoCrate2.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "";
	iconName = "";
	doColorShift = false;
	isAmmoCrate = 1;

	 // Dynamic properties defined by the scripts
	image = "";
	canDrop = true;
};
datablock fxDTSBrickData (BrickAmmoCrateData)
{
	brickFile = "./models/ammocratebrick.blb";
	category = "Special";
	subCategory = "Interactive";
	uiName = "Ammo Crate";
	iconName = "add-ons/gamemode_left4block/modules/add-ins/brick_l4b/icons/icon_ammocrate";
	indestructable = 1;
	isAmmoCrate = 1;
};
datablock ItemData(RHealthLockerItem)
{
	category = "Item";  // Mission editor category
	//className = "Item"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./models/healthlocker15.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "";
	iconName = "";
	doColorShift = false;

	 // Dynamic properties defined by the scripts
	image = "";
	canDrop = true;
};
datablock fxDTSBrickData (BrickLockerData)
{
	brickFile = "./models/lockerbrick.blb";
	category = "Special";
	subCategory = "Interactive";
	uiName = "Health Locker";
	iconName = "add-ons/gamemode_left4block/modules/add-ins/brick_l4b/icons/Icon_HealthLocker";
	indestructable = 1;
};
