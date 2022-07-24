%pattern = "add-ons/gamemode_left4block/sound/*.wav";//Too lazy to write datablock files for the sounds, just took this from the Disease Gamemode
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/gamemode_left4block/sound/", "");
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

if(!isObject(swordExplosionParticle)) //I mean... Just in case right?
{
	datablock ParticleData(swordExplosionParticle)
	{
		dragCoefficient			= 2;
		gravityCoefficient		= 1.0;
		inheritedVelFactor		= 0.2;
		constantAcceleration	= 0.0;
		
		spinRandomMin			= -90;
		spinRandomMax			= 90;
		
		lifetimeMS			 	= 500;
		lifetimeVarianceMS		= 300;
		
		textureName				= "base/data/particles/chunk";
		
		colors[0]				= "0.7 0.7 0.9 0.9";
		colors[1]				= "0.9 0.9 0.9 0.0";
		sizes[0]				= 0.5;
		sizes[1]				= 0.25;
	};
	
	datablock ParticleEmitterData(swordExplosionEmitter)
	{
		ejectionPeriodMS	= 7;
		periodVarianceMS	= 0;
		ejectionVelocity	= 8;
		velocityVariance	= 1.0;
		ejectionOffset		= 0.0;
		thetaMin			= 0;
		thetaMax			= 60;
		phiReferenceVel		= 0;
		phiVariance			= 360;
		
		overrideAdvance		= false;
		
		particles			= "swordExplosionParticle";

		uiName				= "Sword Hit";
	};
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

datablock ParticleData(stunParticle)
{
	dragCoefficient      = 13;
	gravityCoefficient   = 0.2;
	inheritedVelFactor   = 1.0;
	constantAcceleration = 0.0;
	lifetimeMS           = 400;
	lifetimeVarianceMS   = 0;
	textureName          = "base/data/particles/star1";
	spinSpeed		   = 0.0;
	spinRandomMin		= 0.0;
	spinRandomMax		= 0.0;
	colors[0]     = "1 1 0.2 0.9";
	colors[1]     = "1 1 0.4 0.5";
	colors[2]     = "1 1 0.5 0";

	sizes[0]      = 0.5;
	sizes[1]      = 0.2;
	sizes[2]      = 0.1;

	times[0] = 0.0;
	times[1] = 0.5;
	times[2] = 1.0;

	useInvAlpha = false;
};
datablock ParticleEmitterData(stunEmitter)
{
	ejectionPeriodMS = 12;
	periodVarianceMS = 1;
	ejectionVelocity = 5.25;
	velocityVariance = 0.0;
	ejectionOffset   = 0.25;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = stunParticle;
};
datablock ShapeBaseImageData(stunImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = false;

	mountPoint = $HeadSlot;
	offset = "0 0 0.4";
	eyeOffset = "0 0 999";

	stateName[0]				= "Ready";
	stateTimeoutValue[0]		= 0.01;
	stateTransitionOnTimeout[0]	= "FireA";

	stateName[1]				= "FireA";
	stateEmitter[1]				= stunEmitter;
	stateEmitterTime[1]			= 1.2;
	stateTimeoutValue[1]		= 1.2;
	stateTransitionOnTimeout[1]	= "Done";
	stateWaitForTimeout[1]		= true;

	stateName[2]				= "Done";
	stateTimeoutValue[2]		= 0.01;
	stateTransitionOnTimeout[2]	= "FireA";
};