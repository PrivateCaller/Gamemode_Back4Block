%pattern = "./sounds/*.wav";
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strreplace(filename(strlwr(%file)), ".wav", "");

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

if(isFile(%facefilepath = findFirstFile("./models/face.ifl")))//Faces
{
	%write = new FileObject();
	%write.openForWrite(%facefilepath);
	%write.writeLine("base/data/shapes/player/faces/smiley.png");
	%write.writeLine("Add-Ons/Face_Default/smileyRedBeard2.png");
	%write.writeLine("Add-Ons/Face_Default/smileyRedBeard.png");
	%write.writeLine("Add-Ons/Face_Default/smileyPirate3.png");
	%write.writeLine("Add-Ons/Face_Default/smileyPirate2.png");
	%write.writeLine("Add-Ons/Face_Default/smileyPirate1.png");
	%write.writeLine("Add-Ons/Face_Default/smileyOld.png");
	%write.writeLine("Add-Ons/Face_Default/smileyFemale1.png");
	%write.writeLine("Add-Ons/Face_Default/smileyEvil2.png");
	%write.writeLine("Add-Ons/Face_Default/smileyEvil1.png");
	%write.writeLine("Add-Ons/Face_Default/smileyCreepy.png");
	%write.writeLine("Add-Ons/Face_Default/smileyBlonde.png");
	%write.writeLine("Add-Ons/Face_Default/memeYaranika.png");
	%write.writeLine("Add-Ons/Face_Default/memePBear.png");
	%write.writeLine("Add-Ons/Face_Default/memeHappy.png");
	%write.writeLine("Add-Ons/Face_Default/memeGrinMan.png");
	%write.writeLine("Add-Ons/Face_Default/memeDesu.png");
	%write.writeLine("Add-Ons/Face_Default/memeCats.png");
	%write.writeLine("Add-Ons/Face_Default/memeBlockMongler.png");
	%write.writeLine("Add-Ons/Face_Default/asciiTerror.png");
	
	%facetexturepath = "./models/faces/*.png";
	for(%facetexturefile = findFirstFile(%facetexturepath); %facetexturefile !$= ""; %facetexturefile = findNextFile(%facetexturepath))
	{		
		addExtraResource(%facetexturefile);
		%write.writeLine(%facetexturefile);
	}

	%write.close();
	%write.delete();
	addExtraResource(%facefilepath);
}

if(isFile(%decalfilepath = findFirstFile("./models/decal.ifl")))//Decals
{
	%write = new FileObject();
	%write.openForWrite(%decalfilepath);
	%write.writeLine("base/data/shapes/players/decals/AAA-none.png");
	
	%decalspath = "./models/decals/*.png";
	for(%decalsfile = findFirstFile(%decalspath); %decalsfile !$= ""; %decalsfile = findNextFile(%decalspath))
	{		
		if(strstr(strlwr(%decalsfile), "tailor") == -1) $hZombieDecal[$hZombieDecalAmount] = fileName(%decalsfile);
		addExtraResource(%decalsfile);
		%write.writeLine(%decalsfile);
	}

	%write.close();
	%write.delete();
	addExtraResource(%decalfilepath);
}

%cipath = "./icons/ci/*.png";
for(%cifile = findFirstFile(%cipath); %cifile !$= ""; %cifile = findNextFile(%cipath))
addExtraResource(%cifile);

%imageskinpath = "./models/hats/*.png";
for(%imageskinfile = findFirstFile(%imageskinpath); %imageskinfile !$= ""; %imageskinfile = findNextFile(%imageskinpath))
addExtraResource(%imageskinfile);

%hatimagepath = "./models/hats/*.dts";
for(%hatimagefile = findFirstFile(%hatimagepath); %hatimagefile !$= ""; %hatimagefile = findNextFile(%hatimagepath)) 
eval("datablock shapeBaseImageData(" @ strreplace(fileName(%hatimagefile),".dts","") @ "image) { shapefile = \"" @ %hatimagefile @ "\"; mountPoint = 6; offset = \"0 0 0\"; eyeOffset = \"0 0 10\"; doColorShift = false; className = \"WeaponImage\"; armReady = false; };");

datablock ExplosionData(ZombieHitExplosion)
{
	shakeCamera = true;
	camShakeDuration = 1;
	camShakeRadius = 1;
	camShakeFreq = "3 3 3";
	camShakeAmp = "0.6 0.6 0.6";
};
datablock ProjectileData(ZombieHitProjectile)
{
   explosion = ZombieHitExplosion;
};

datablock ExplosionData(BigZombieHitExplosion)
{
   shakeCamera = true;
   camShakeFreq = "10.0 10.0 100.0";
   camShakeAmp = "1 1 2";
   camShakeDuration = 0.75;
   camShakeRadius = 1.0;
};
datablock ProjectileData(BigZombieHitProjectile)
{
   explosion = BigZombieHitExplosion;
};

datablock ParticleData(L4B_stunParticle)
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
datablock ParticleEmitterData(L4B_stunEmitter)
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
	particles = L4B_stunParticle;
};
datablock ShapeBaseImageData(L4B_stunImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = false;

	mountPoint = 5;
	offset = "0 0 0";
	eyeOffset = "0 0 999";

	stateName[0]				= "Ready";
	stateTimeoutValue[0]		= 0.01;
	stateTransitionOnTimeout[0]	= "FireA";

	stateName[1]				= "FireA";
	stateEmitter[1]				= L4B_stunEmitter;
	stateEmitterTime[1]			= 1.2;
	stateTimeoutValue[1]		= 1.2;
	stateTransitionOnTimeout[1]	= "Done";
	stateWaitForTimeout[1]		= true;

	stateName[2]				= "Done";
	stateTimeoutValue[2]		= 0.01;
	stateTransitionOnTimeout[2]	= "FireA";
};

datablock ParticleData(SecondaryMeleeFlashParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = -0.5;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 80;
	lifetimeVarianceMS   = 15;
	textureName          = "base/data/particles/star1";
	spinSpeed		= 10.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.6 0.6 0.1 0.9";
	colors[1]     = "0.6 0.6 0.6 0.0";
	sizes[0]      = 1.0;
	sizes[1]      = 2.0;

	useInvAlpha = false;
};
datablock ParticleEmitterData(SecondaryMeleeFlashEmitter)
{
	ejectionPeriodMS = 3;
	periodVarianceMS = 0;
	ejectionVelocity = 1.0;
	velocityVariance = 1.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 90;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = SecondaryMeleeFlashParticle;
};

datablock ExplosionData(SecondaryMeleeExplosion)
{
	soundProfile = "";
	lifeTimeMS = 150;

	particleDensity = 5;
	particleRadius = 0.2;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeDuration = 1;
	camShakeRadius = 2.5;

	camShakeFreq = "1.5 1.5 1.5";
	camShakeAmp = "0.75 0.75 0.75";
	particleEmitter = SecondaryMeleeFlashEmitter;

	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0 0 0";
	lightEndColor = "0 0 0";
};
datablock ProjectileData(SecondaryMeleeProjectile)
{
	explosion = SecondaryMeleeExplosion;
};

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

function oxygenBubbleImage::onDone(%this,%obj,%slot) { %obj.unMountImage(%slot); }

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