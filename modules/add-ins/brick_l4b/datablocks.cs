%pattern = "add-ons/gamemode_left4block/modules/add-ins/brick_l4b/sound/*.wav";
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	eval("datablock AudioProfile(" @ strreplace(filename(strlwr(%file)), ".wav", "") @ "_sound) { preload = true; description = AudioClosest3d; filename = \"" @ %file @ "\"; };");
	%file = findNextFile(%pattern);
}

datablock StaticShapeData(elevatorStaticShape)
{
	density = "1";
	drag = "0";
	dynamicType = "0";
	emap = "0";
	mass = "1";
	isInvincible = true;

	shapeFile = "./models/shape_elevator.dts";
};

datablock StaticShapeData(AmmoCrateShape : elevatorStaticShape)
{
	shapeFile = "./models/shape_ammocrate.dts";
	shapeBrickPos = "0 0 -0.1";
	isInteractiveShape = true;
};

datablock StaticShapeData(HealthLockerShape : AmmoCrateShape)
{
	shapeFile = "./models/shape_healthlocker.dts";
	shapeBrickPos = "0 0 -0.1";
};

datablock fxDTSBrickData(ElevatorBrick8x8)
{
    brickFile = "base/data/bricks/flats/8x8F.blb";
    category = "Special";
    subCategory = "Interactive";
    uiName = "Elevator";
	iconName = "add-ons/gamemode_left4block/modules/add-ins/brick_l4b/icons/icon_elevator";
    isElevator = true;
	indestructable = true;
	alwaysShowWireFrame = false;
};

datablock fxDTSBrickData (BrickAmmoCrateData : brick2x4fData)
{
	ShapeDatablock = AmmoCrateShape;
	category = "Special";
	subCategory = "Interactive";
	uiName = "Ammo Crate";
	iconName = "add-ons/gamemode_left4block/modules/add-ins/brick_l4b/icons/icon_ammocrate";
	indestructable = true;
	alwaysShowWireFrame = false;
};

datablock fxDTSBrickData (BrickLockerData : brick1x4fData)
{
	ShapeDatablock = HealthLockerShape;
	uiName = "Health Locker";
	iconName = "add-ons/gamemode_left4block/modules/add-ins/brick_l4b/icons/Icon_HealthLocker";
	category = "Special";
	subCategory = "Interactive";	
	indestructable = true;
	alwaysShowWireFrame = false;	
};
