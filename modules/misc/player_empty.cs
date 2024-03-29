datablock fxLightData(blankBillboard)
{
	LightOn = false;
	flareOn = true;
	flarebitmap = "base/data/shapes/blank.png";
	ConstantSize = 0.1;
    ConstantSizeOn = true;
	LinkFlare = false;
	AnimOffsets = false;
    FadeTime = 99999;
	
	blendMode = 1;
	flareColor = "1 1 1 1";	
};

datablock fxLightData(strangledBillboard : blankBillboard)
{
	flarebitmap = "./icons/icon_strangled.png";
};

datablock fxLightData(incappedBillboard : blankBillboard)
{
	flarebitmap = "./icons/icon_incapped.png";
};

datablock PlayerData(ItemHolsterPlayer)
{
    shapeFile = "./models/itemholsterplayer.dts";
    boundingBox = "4 4 4";

    splash = PlayerSplash;
    splashEmitter[0] = PlayerFoamDropletsEmitter;
    splashEmitter[1] = PlayerFoamEmitter;
    splashEmitter[2] = PlayerBubbleEmitter;

    mediumSplashSoundVelocity = 10;
    hardSplashSoundVelocity = 20;
    exitSplashSoundVelocity = 5;

    impactWaterEasy = Splash1Sound;
    impactWaterMedium = Splash1Sound;
    impactWaterHard = Splash1Sound;
    exitingWater = exitWaterSound;

    jetEmitter = playerJetEmitter;
    jetGroundEmitter = playerJetGroundEmitter;
    jetGroundDistance = 4;
    footPuffNumParts = 10;
    footPuffRadius = 0.25;
};

datablock PlayerData(emptyPlayer : playerStandardArmor)
{
	shapeFile = "base/data/shapes/empty.dts";
	boundingBox = "0.01 0.01 0.01";
	crouchboundingBox = "0.01 0.01 0.01";
	isEmptyPlayer = true;
	deathSound = "";
	painSound = "";
	useCustomPainEffects = true;
	mountSound = "";
	jumpSound = "";
	uiName = "";
};

function emptyPlayer::onAdd(%this, %obj) 
{
	%obj.setDamageLevel(%this.maxDamage);

	if(isObject(%source = %obj.source) && %obj.slotToMountBot)
	{
		%source.mountObject(%obj,%obj.slotToMountBot);
		
		if(%obj.imageToMount !$= "") 
		{
			if(%obj.imageColor !$= "") %obj.mountImage(%obj.imageToMount,0,1,%obj.imageColor);
			else %obj.mountImage(%obj.imageToMount,0);
		}
		if(%obj.lightToMount !$= "")
		{
			%billboard = new fxLight ("")
			{
				dataBlock = %obj.lightToMount;
				source = %source;
			};
			
			%obj.lightToMount = %billboard;
			MissionCleanup.add(%billboard);
			%billboard.setTransform(%obj.getTransform());
			%billboard.attachToObject(%obj);

			for(%i = 0; %i < clientgroup.getCount(); %i++) 			
			if(isObject(%client = clientgroup.getObject(%i)) && isObject(%client.player))
			%billboard.ScopeToClient(%client);
		}
	}
	else
	{
		%obj.delete();
		return;
	}
}

function emptyPlayer::onRemove(%this, %obj)
{
	if(isObject(%obj.lightToMount)) %obj.lightToMount.delete();
}
function emptyPlayer::doDismount(%this, %obj, %forced) 
{
	return;
}
function emptyPlayer::onDisabled(%this, %obj) 
{
	return;
}