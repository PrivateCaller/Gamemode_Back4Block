datablock fxDTSBrickData (BrickL4BZombie_HoleSpawnData)
{
	brickFile = "Add-ons/Bot_Hole/4xSpawn.blb";
	category = "Special";
	subCategory = "Left 4 Block";
	uiName = "L4B Zombie Hole";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_zombie";

	bricktype = 2;
	cancover = 0;
	orientationfix = 1;
	indestructable = 1;

	isBotHole = 1;
	isZombieBrick = 1;
	holeBot = "CommonZombieHoleBot";
};

function BrickL4BZombie_HoleSpawnData::onPlant(%this, %obj)
{
	if(!isObject(directorBricks))
    {
        new SimSet(directorBricks);
        directorBricks.add(%obj);
        MissionCleanup.add(directorBricks);
    }
    else if(isObject(directorBricks))
    directorBricks.add(%obj);

    Parent::onPlant(%this,%obj);
}

function BrickL4BZombie_HoleSpawnData::onloadPlant(%this, %obj)
{
	BrickL4BZombie_HoleSpawnData::onPlant(%this,%obj);
}