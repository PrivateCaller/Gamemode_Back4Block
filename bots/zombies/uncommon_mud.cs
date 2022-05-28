datablock ParticleData(MudStatusParticle)
{
	dragCoefficient      = 0.2;
	gravityCoefficient   = 1;
	inheritedVelFactor   = 0.6;
	constantAcceleration = 0.0;
	spinRandomMin 	     = -90;
	spinRandomMax 		= 90;
	lifetimeMS           = 350;
	lifetimeVarianceMS   = 150;
	textureName          = "base/data/particles/cloud";
	colors[0]     = "0.239547 0.155146 0.116272 0.4";
	colors[1]     = "0.239547 0.155146 0.116272 0.4";
	colors[2]     = "0.239547 0.155146 0.116272 0.4";
//	colors[3]     = "0.4 0.6 0.4 0.0";
	sizes[0]      = 0.2;
	sizes[1]      = 0.25;
	sizes[2]      = 0.2;
//	sizes[3]      = 0.0;
	times[0]	  = 0.0;
	times[1]	  = 0.3;
	times[2]	  = 1;
//	times[3]	  = 1.0;

};

datablock ParticleEmitterData(MudStatusEmitter)
{
   ejectionPeriodMS = 2;	//7
   periodVarianceMS = 1;
   ejectionVelocity = 0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.9;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "MudStatusParticle";

   uiName = "";
};

datablock ParticleData(MudPulseParticle)
{
	dragCoefficient      = 0.2;
	gravityCoefficient   = 0.5;
	inheritedVelFactor   = 0.6;
	constantAcceleration = 0.0;
	spinRandomMin 	     = -90;
	spinRandomMax 		= 90;
	lifetimeMS           = 500;
	lifetimeVarianceMS   = 250;
	textureName          = "base/data/particles/cloud";
	colors[0]     = "0.239547 0.155146 0.116272 0.2";
	colors[1]     = "0.239547 0.155146 0.116272 0.2";
	colors[2]     = "0.239547 0.155146 0.116272 0.2";
//	colors[3]     = "0.4 0.6 0.4 0.0";
	sizes[0]      = 1.04;
	sizes[1]      = 1.08;
	sizes[2]      = 1.06;
//	sizes[3]      = 0.0;
	times[0]	  = 0.0;
	times[1]	  = 0.3;
	times[2]	  = 1;
//	times[3]	  = 1.0;

};

datablock ParticleEmitterData(MudPulseEmitter)
{
   ejectionPeriodMS = 8;	//7
   periodVarianceMS = 6;
   ejectionVelocity = 0.25;
   velocityVariance = 0.0;
   ejectionOffset   = 1;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "MudPulseParticle";

   uiName = "";
};

datablock ShapeBaseImageData(MudStatusPlayerImage)
{
   shapeFile = "base/data/shapes/empty.dts";
   emap = true;

   mountPoint = 2;
   offset = "0 0 -0.3";
   eyeOffset = 0;
   rotation = "0 0 0";

   correctMuzzleVector = true;

   className = "WeaponImage";

   item = "";
   ammo = " ";
   projectile = "";
   projectileType = Projectile;
   
   isPoison = 1;

   melee = false;
   armReady = false;

   doColorShift = false;
   colorShiftColor = "1 1 1 1";

	//lightType = "ConstantLight";
	//lightColor = "0 1 0";
	//lightRadius = 4;

	stateName[0]                   = "Wait";
	stateTimeoutValue[0]           = 0.2;
	stateEmitter[0]                = MudStatusEmitter;
	stateEmitterTime[0]            = 1;
	stateTransitionOnTimeout[0]    = "Poison";
	//stateSound[0]                  = MudPulseSound;

	stateName[1]                   = "Poison";
	//stateScript[1]                 = "Damage";
	stateEmitter[1]                = MudPulseEmitter;
	stateEmitterTime[1]            = 0.2;
	stateTimeoutValue[1]           = 0.1;
	stateTransitionOnTimeout[1]    = "Wait";
	//stateSound[1]                  = FireShotSound;	//No sound
};

datablock fxDTSBrickData (BrickMudZombie_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
	uiName = "Mud Zombie Hole";
	holeBot = "MudZombieHoleBot";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_mudman";
};

datablock PlayerData(MudZombieHoleBot : CommonZombieHoleBot)
{
	uiName = "";
	
	jumpForce = 0;
    maxForwardSpeed = 0;
    maxBackwardSpeed = 0;
    maxSideSpeed = 0;

 	maxForwardCrouchSpeed = 4;
    maxBackwardCrouchSpeed = 2;
    maxSideCrouchSpeed = 2;

	drag = 0.02;//0.02
	density = 0.8;

	ShapeNameDistance = 15;
	hIsInfected = 1;
	hZombieL4BType = 4;
	hCustomNodeAppearance = 0;
	hPinCI = "";
	hBigMeleeSound = "";

	maxUnderwaterSideSpeed = 5;
	maxUnderwaterForwardSpeed = 10;
	maxUnderwaterBackwardSpeed = 5;

	hName = "Infected" SPC "Mudman";//cannot contain spaces
};

function MudZombieHoleBot::onAdd(%this,%obj,%style)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
}

function MudZombieHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);
	%obj.mountImage(MudStatusPlayerImage,1);
}

function MudZombieHoleBot::onDamage(%this,%obj)
{
	CommonZombieHoleBot::OnDamage(%this,%obj);
}

function MudZombieHoleBot::onDisabled(%this,%obj)
{
	CommonZombieHoleBot::OnDisabled(%this,%obj);
}

function MudZombieHoleBot::onBotLoop(%this,%obj)
{
	%obj.hLimitedLifetime();
	%obj.setCrouching(1);
}

function MudZombieHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this,%obj,%col);

	if(!%col.BoomerBiled)
	{
		%col.mountImage(MudStatusPlayerImage, 2);
		%col.playAudio(3,"mud_splat_sound");

		%col.setMaxForwardSpeed(%col.Datablock.maxForwardSpeed-5);
		%col.setMaxSideSpeed(%col.Datablock.MaxSideSpeed-5);
		%col.setMaxBackwardSpeed(%col.Datablock.MaxBacwardkSpeed-5);

		cancel(%col.ClearMud);
		%col.ClearMud = schedule(5000,0,MudClear,%col,%obj);
		%col.ZombieMud = 1;
	}
}

function MudClear(%targetid,%obj)
{
	if(isObject(%targetid) && %targetid.getState !$= "Dead" && %targetid.ZombieMud)
	{
		%targetid.ZombieMud = 0;
		%targetid.unMountImage(2);
		%targetid.setMaxForwardSpeed(%col.Datablock.maxForwardSpeed);
		%targetid.setMaxSideSpeed(%targetid.Datablock.MaxSideSpeed);
		%targetid.setMaxBackwardSpeed(%targetid.Datablock.MaxBacwardkSpeed);
	}
}

function MudZombieHoleBot::L4BUncommonAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{
	%mudColor = "0.539547 0.455146 0.416272 1.000000";
	%randmultiplier = getRandom(200,1000)*0.001;
	%skincolor = getWord(%mudColor,0)*%randmultiplier SPC getWord(%mudColor,1)*%randmultiplier SPC getWord(%mudColor,2)*%randmultiplier SPC 1;

	//Appearance zombie
	%obj.llegColor =  %skinColor;
	%obj.secondPackColor =  "0 0 0 0";
	%obj.lhand =  "0";
	%obj.hip =  "0";
	%obj.faceName =  %face;
	%obj.rarmColor =  %skinColor;
	%obj.hatColor =  "0 0 0 0";
	%obj.hipColor =  %skinColor;
	%obj.chest =  %chest;
	%obj.rarm =  "0";
	%obj.packColor =  "0 0 0 0";
	%obj.pack =  "0";
	%obj.decalName =  "AAA-None";
	%obj.larmColor =  %skinColor;
	%obj.secondPack =  "0";
	%obj.larm =  "0";
	%obj.chestColor =  %skinColor;
	%obj.accentColor =  "0 0 0 0";
	%obj.rhandColor =  %skinColor;
	%obj.rleg =  "0";
	%obj.rlegColor =  %skinColor;
	%obj.accent =  "1";
	%obj.headColor =  %skinColor;
	%obj.rhand =  "0";
	%obj.lleg =  "0";
	%obj.lhandColor =  %skinColor;
	%obj.hat =  "0";

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}