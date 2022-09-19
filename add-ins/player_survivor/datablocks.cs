%pattern = "add-ons/gamemode_left4block/add-ins/player_survivor/sound/*.wav";
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/gamemode_left4block/add-ins/player_survivor/sound/", "");
	%soundName = strreplace(%soundName, "/", "");
	%soundName = strreplace(%soundName, ".wav", "");
	%soundName = strreplace(%soundName, "quiet", "");
	%soundName = strreplace(%soundName, "normal", "");
	%soundName = strreplace(%soundName, "loud", "");

	//Check the names of the folders to determine what type of soundscape it will be, and check if it's a loopable sound or not
	if(strstr(%file,"normal") != -1)//Normal soundscape
	if(strstr(%file,"loop") != -1) eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioCloseLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClose3d; filename = \"" @ %file @ "\"; };");

	if(strstr(%file,"quiet") != -1)//Quiet soundscape
	if(strstr(%file,"loop") != -1) eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosestLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosest3d; filename = \"" @ %file @ "\"; };");

	if(strstr(%file,"loud") != -1)//Loudest Soundscape
	if(strstr(%file,"loop") != -1) eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioDefaultLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioDefault3d; filename = \"" @ %file @ "\"; };");

	%file = findNextFile(%pattern);
}

function generateCustomDecalList()
{
	if(isFile("add-ons/gamemode_left4block/add-ins/player_survivor/models/decallist.txt"))
	{
		%write = new FileObject();
		%write.openForWrite("add-ons/gamemode_left4block/add-ins/player_survivor/models/decallist.txt");		

		%decalpath = "add-ons/gamemode_left4block/add-ins/player_survivor/models/decals/*.png";
		for(%decal = findFirstFile(%decalpath); %decal !$= ""; %decal = findNextFile(%decalpath)) %write.writeLine("addExtraResource(\"" @ %decal @ "\");");
		
		%write.close();
		%write.delete();
	}
}
generateCustomDecalList();

datablock TSShapeConstructor(mMeleeDts) 
{
	baseShape = "./models/mmelee.dts";
	sequence0 = "./models/default.dsq";
	sequence1 = "./models/melee.dsq";
	sequence2 = "./models/actions.dsq";
	sequence3 = "./models/zombie.dsq";
};

datablock PlayerData(PlayerMeleeAnims : PlayerStandardArmor)
{
	shapeFile = "./models/mMelee.dts";
	canJet = true;
	uiName = "Melee Player";
};

datablock PlayerData(PlayerMeleeAnimsJet : PlayerMeleeAnims)
{
	canJet = true;
	uiName = "Melee Player Jet";
};

addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/zoey.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/witch.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/townsuit2.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/townsuit.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/shirtjacket.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/rochelle.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/polostriped.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/polo.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/piratestripes.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/nick.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/newoveralls.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/newhoodie.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/musclechest.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/louis.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/left4block.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/jgtuxedoShirt.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/jgtuxedo.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/hoodienozipperpockets.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/francis.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/flannel.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/ellis.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/coach.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/classicshirt.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/civilian.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/casualjacket.png");
addExtraResource("Add-Ons/Gamemode_Left4Block/add-ins/player_survivor/models/decals/bill.png");
addExtraResource("add-ons/gamemode_left4block/add-ins/player_survivor/models/decal.ifl");
addExtraResource("add-ons/gamemode_left4block/add-ins/player_survivor/models/face.ifl");
addExtraResource("add-ons/gamemode_left4block/add-ins/player_survivor/icons/DownCi.png");
addExtraResource("add-ons/gamemode_left4block/add-ins/player_survivor/icons/CI_VictimSaved.png");

datablock ParticleData(oxygenBubbleParticle : painMidParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= -2.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 800;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= false;
	animateTexture		= false;

	textureName		= "base/data/particles/bubble";
   
	colors[0]	= "0.2 0.6 1 0.4";
	colors[1]	= "0.2 0.6 1 0.8";
	colors[2]	= "0.2 0.6 1 0.8";
	sizes[0]	= 0.2;
	sizes[1]	= 0.4;
	sizes[2]	= 0.0;
	times[0]	= 0.0;
	times[1]	= 0.8;
   times[2]	= 1.0;
};

datablock ParticleEmitterData(oxygenBubbleEmitter : painMidEmitter)
{
   ejectionPeriodMS = 5;
   periodVarianceMS = 0;
   ejectionVelocity = 6;
   velocityVariance = 2;
   ejectionOffset   = 0.2;
   thetaMin         = 0;
   thetaMax         = 105;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;

   particles = oxygenBubbleParticle;

   uiName = "Oxygen Bubbles";
};

datablock ShapeBaseImageData(oxygenBubbleImage : painMidImage)
{
	stateTimeoutValue[1] = 0.05;
	stateEmitter[1] = oxygenBubbleEmitter;
	stateEmitterTime[1]	= 0.05;
};

function oxygenBubbleImage::onDone(%this,%obj,%slot)
{
	%obj.unMountImage(%slot);
}

datablock PlayerData(downedMount)
{
    shapeFile = "./models/downedmounts.dts";
	boundingBox = vectorScale("20 20 20", 4);
};

datablock PlayerData(SurvivorPlayer : PlayerMeleeAnims)
{
	canPhysRoll = true;
	canJet = false;
	runForce = 100 * 45;
	jumpforce = 100*9.25;
	jumpDelay = 25;
	minimpactspeed = 15;
	speedDamageScale = 0.25;
	mass = 105;
	airControl = 0.05;

	cameramaxdist = 2.25;
    cameraVerticalOffset = 1;
    cameraHorizontalOffset = 0.75;
    cameratilt = 0;
    maxfreelookangle = 2;

    maxForwardSpeed = 9;
	maxSideSpeed = 7;
    maxBackwardSpeed = 6;

 	maxForwardCrouchSpeed = 5;
	maxSideCrouchSpeed = 4;
    maxBackwardCrouchSpeed = 3;
    
	groundImpactMinSpeed = 5;
	groundImpactShakeFreq = "4.0 4.0 4.0";
	groundImpactShakeAmp = "1.0 1.0 1.0";
	groundImpactShakeDuration = 0.8;
	groundImpactShakeFalloff = 15;

	uiName = "Survivor Player";
	usesL4DItems = true;
	isSurvivor = true;
	firstpersononly = true;
	hType = "Survivors";
	maxtools = 5;
	maxWeapons = 5;

	jumpSound 		= JumpSound;
	PainSound		= "";
	DeathSound		= "";
	useCustomPainEffects = true;
	rechargeRate = 0.025;
	maxenergy = 100;
	showEnergyBar = true;
};
datablock PlayerData(SurvivorPlayerMed : SurvivorPlayer)
{
	maxForwardSpeed = SurvivorPlayer.maxForwardSpeed/1.375;
   	maxBackwardSpeed = SurvivorPlayer.maxBackwardSpeed/1.375;
   	maxSideSpeed = SurvivorPlayer.maxSideSpeed/1.375;
 	maxForwardCrouchSpeed = SurvivorPlayer.maxForwardCrouchSpeed/1.375;
    maxBackwardCrouchSpeed = SurvivorPlayer.maxBackwardCrouchSpeed/1.375;
    maxSideCrouchSpeed = SurvivorPlayer.maxSideCrouchSpeed/1.375;

	uiName = "";
};
datablock PlayerData(SurvivorPlayerLow : SurvivorPlayer)
{
	maxForwardSpeed = SurvivorPlayer.maxForwardSpeed/1.75;
   	maxBackwardSpeed = SurvivorPlayer.maxBackwardSpeed/1.75;
   	maxSideSpeed = SurvivorPlayer.maxSideSpeed/1.75;
 	maxForwardCrouchSpeed = SurvivorPlayer.maxForwardCrouchSpeed/1.75;
    maxBackwardCrouchSpeed = SurvivorPlayer.maxBackwardCrouchSpeed/1.75;
    maxSideCrouchSpeed = SurvivorPlayer.maxSideCrouchSpeed/1.75;

	uiName = "";
};

datablock PlayerData(SurvivorPlayerDowned : SurvivorPlayerLow)
{	
   	runForce = SurvivorPlayer.runForce;
   	maxForwardSpeed = 0;
   	maxBackwardSpeed = 0;
   	maxSideSpeed = 0;
   	maxForwardCrouchSpeed = 0;
   	maxBackwardCrouchSpeed = 0;
   	maxSideCrouchSpeed = 0;
   	jumpForce = 0;
	rechargerate = 0;
	isDowned = true;
	uiName = "";
};