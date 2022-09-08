%pattern = "add-ons/gamemode_left4block/add-ins/player_survivor/sound/*.wav";//Too lazy to write datablock files for the sounds, just took this from the Disease Gamemode
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
	jumpforce = 100*8.5;
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
	usesL4DItems = 1;
	isSurvivor = 1;
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
	maxForwardSpeed = 6;
   	maxBackwardSpeed = 2;
   	maxSideSpeed = 3;
 	maxForwardCrouchSpeed = 3;
    maxBackwardCrouchSpeed = 1;
    maxSideCrouchSpeed = 2;

	uiName = "";
};
datablock PlayerData(SurvivorPlayerLow : SurvivorPlayer)
{
	canPhysRoll = false;
	maxForwardSpeed = 3;
   	maxBackwardSpeed = 1;
   	maxSideSpeed = 1;

	maxForwardCrouchSpeed = 2;
   	maxBackwardCrouchSpeed = 1;
   	maxSideCrouchSpeed = 1;

	uiName = "";
};

datablock PlayerData(SurvivorPlayerDowned : SurvivorPlayerLow)
{	
   	runForce = 100 * 60;
   	maxForwardSpeed = 0;
   	maxBackwardSpeed = 0;
   	maxSideSpeed = 0;

   	maxForwardCrouchSpeed = 0;
   	maxBackwardCrouchSpeed = 0;
   	maxSideCrouchSpeed = 0;

   	jumpForce = 0; //8.3 * 90;

	firstpersononly = 1;

	uiName = "";
	maxenergy = 100;
	maxDamage = 200;
	showEnergyBar = true;

	jumpSound 		= "";
	PainSound		= "";
	DeathSound		= "";
	useCustomPainEffects = true;
	isDowned = 1;

   	runSurfaceAngle  = 5;
   	jumpSurfaceAngle = 5;
   	isSurvivor = 1;
   	rechargerate = 0;
};