datablock fxDTSBrickData (BrickZombieConstruction_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
	uiName = "Construction Zombie Hole";
	holeBot = "ZombieConstructionHoleBot";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_construction";
};

datablock PlayerData(ZombieConstructionHoleBot : CommonZombieHoleBot)
{
	uiName = "";
	maxdamage = 35;//Health
	hName =  "Infected" SPC "Construction";//cannot contain spaces
	ShapeNameDistance = 15;
	hIsInfected = 1;
	hZombieL4BType = 4;
	hCustomNodeAppearance = 1;
	hPinCI = "";
	hBigMeleeSound = "";
};

function ZombieConstructionHoleBot::onAdd(%this,%obj,%style)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);

	if(getRandom(0,100) <= 25)
	{
		switch(getRandom(0,4))
		{
			case 0: %obj.mountImage(wrenchImage, 0);
			case 1: %obj.mountImage(hammerImage, 0);
			case 2: %obj.mountImage(ConstructionConeSpeakerImage, 0);
					if($L4B2Bots::UncommonWarningLight)
					L4B_SpecialsWarningLight(%obj);
			case 3: serverCmdUseSprayCan(%obj,getRandom(0,27));
			case 4: %obj.mountImage(PrintGunImage, 0);
		}
	}
}

function ZombieConstructionHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);
}

function ZombieConstructionHoleBot::onDamage(%this,%obj)
{
	CommonZombieHoleBot::OnDamage(%this,%obj);
}

function ZombieConstructionHoleBot::onDisabled(%this,%obj)
{
	CommonZombieHoleBot::OnDisabled(%this,%obj);
	if(isObject(%obj.light))
	%obj.light.delete();
}

function ZombieConstructionHoleBot::onBotLoop(%this,%obj)
{
	CommonZombieHoleBot::onBotLoop(%this,%obj);	
}

function ZombieConstructionHoleBot::onBotFollow( %this, %obj, %targ )
{
	CommonZombieHoleBot::onBotFollow( %this, %obj, %targ );

	if(isObject(%obj.getmountedImage(0)) && %obj.getmountedImage(0).getName() $= "ConstructionConeSpeakerImage")
	{
		%obj.playThread(3, leftrecoil);
		%obj.playAudio(0, "zombiemale_attackcone" @ getRandom(1,2) @ "_sound");
		L4B_CommonZombDistraction(%obj);
	}
}

function ZombieConstructionHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this,%obj,%col);
}

function ZombieConstructionHoleBot::L4BUncommonAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{	
	%clothesrandmultiplier = getrandom(75,200)*0.01;
	%hatColor = getRandomBotRGBColor();
	%packColor = getRandomBotRGBColor();
	%pack2Color = getRandomBotRGBColor();
	%accentColor = getRandomBotRGBColor();

	%shirtColor = 0.108411*%clothesrandmultiplier SPC 0.258824*%clothesrandmultiplier SPC 0.556075*%clothesrandmultiplier SPC 1;
	%pantsColor = 0 SPC 0.141176*%clothesrandmultiplier SPC 0.333333*%clothesrandmultiplier SPC 1;
	%larmColor = getRandom(0,1);
	if(%larmColor)
	%larmColor = %shirtColor;
	else %larmColor = %skinColor;
	%rarmColor = getRandom(0,1);
	if(%rarmColor)
	%rarmColor = %shirtColor;
	else %rarmColor = %skinColor;
	%rLegColor = getRandom(0,1);
	if(%rLegColor)
	%rLegColor = %pantsColor;
	else %rLegColor = %skinColor;
	%lLegColor = getRandom(0,1);
	if(%lLegColor)
	%lLegColor = %pantsColor;
	else %lLegColor = %skinColor;
	%handColor = %skinColor;
	%decal = "worm_engineer";
	%hat = 0;
	%pack = 0;
	%pack2 = 0;
	%accent = 0;
	
	// accent
	%obj.accentColor = %accentColor;
	%obj.accent =  %accent;
	
	// hat
	%obj.hatColor = %hatColor;
	%obj.hat = %hat;
	
	// head
	%obj.headColor = %skinColor;
	%obj.faceName = %face;
	
	// chest
	%obj.chest =  "0";

	%obj.decalName = %decal;
	%obj.chestColor = %shirtColor;
		
	// packs
	%obj.pack =  %pack;
	%obj.packColor =  %packColor;

	%obj.secondPack =  %pack2;
	%obj.secondPackColor =  %packColor;
		
	// left arm
	%obj.larm =  "0";
	%obj.larmColor = %larmColor;
	
	%obj.lhand =  "0";
	%obj.lhandColor = %handColor;
	
	// right arm
	%obj.rarm =  "0";
	%obj.rarmColor = %rarmColor;
	
	%obj.rhandColor = %handColor;
	%obj.rhand =  "0";
	
	// hip
	%obj.hip =  "0";
	%obj.hipColor = %pantsColor;
	
	// left leg
	%obj.lleg =  "0";
	%obj.llegColor = %llegColor;
	
	// right leg
	%obj.rleg =  "0";
	%obj.rlegColor = %rlegColor;

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function ZombieConstructionHoleBot::hCustomNodeAppearance(%this,%obj)
{
	%obj.unhidenode(ConstructionHelmetBuds);
	%obj.setnodecolor(ConstructionHelmetBuds,getRandomBotPantsColor());
	%obj.unhidenode(ConstructionHelmet);
	%obj.setnodecolor(ConstructionHelmet,"0.8 0.8 0.1 1");
	%obj.unhidenode(ConstructionVest);
	%obj.setnodecolor(ConstructionVest,"0.8 0.8 0.1 1");
}