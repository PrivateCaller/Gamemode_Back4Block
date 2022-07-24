datablock PlayerData(ZombieSpitterHoleBot : CommonZombieHoleBot)
{
	uiName = "Spitter Infected";
	minImpactSpeed = 16;
	speedDamageScale = 2;

	maxdamage = 100;//Health

    maxForwardSpeed = 8;
    maxBackwardSpeed = 7;
    maxSideSpeed = 6;

 	maxForwardCrouchSpeed = 6;
    maxBackwardCrouchSpeed = 5;
    maxSideCrouchSpeed = 4;

	cameramaxdist = 4;
    cameraVerticalOffset = 1;
    cameraHorizontalOffset = 0.6;
    cameratilt = 0.1;
    maxfreelookangle = 2;

	hName = "Spitter";//cannot contain spaces
	hTickRate = 5000;
	hShoot = 1;
	hMaxShootRange = 100;//The range in which the bot will shoot the player
	hTooCloseRange = 50;//in brick units
	hMoveSlowdown = 1;
	hAttackDamage = $L4B_SpecialsDamage;

	ShapeNameDistance = 100;
	hIsInfected = 1;
	hZombieL4BType = 5;
	hCustomNodeAppearance = 0;
	hPinCI = "<bitmapk:Add-Ons/Gamemode_Left4Block/add-ins/bot_l4b/icons/ci_spitter2>";
	SpecialCPMessage = "Right click to spit";
	hBigMeleeSound = "";
	hNeedsWeapons = 1;

	rechargeRate = 0.75;
	maxenergy = 100;
	showEnergyBar = true;
};

function ZombieSpitterHoleBot::onAdd(%this,%obj,%style)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
}

function ZombieSpitterHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);
	
	//%obj.mountImage(SpitAcidStatusPlayerImage, 1);
	%obj.setscale("1 0.95 1.15");
}

function ZombieSpitterHoleBot::onBotLoop(%this,%obj)
{
	%obj.hLimitedLifetime();

	if(!%obj.hFollowing)
	{
		if(%obj.lastidle+4000 < getsimtime())
		{
			%obj.playaudio(0,"spitter_lurk" @ getrandom(1,3) @ "_sound");
			%obj.lastidle = getsimtime();
		}
	}
	else if(!%obj.hReadyToSpit)
	%obj.playaudio(0,"spitter_recognize" @ getrandom(1,3) @ "_sound");
	else %obj.playaudio(0,"spitter_warn" @ getrandom(1,3) @ "_sound");
}


function ZombieSpitterHoleBot::onBotFollow( %this, %obj, %targ )
{
	%obj.setaimobject(%targ);
	
	if(vectorDist(%obj.getposition(),%targ.getposition()) < 48)
	{
		for (%n = 0; %n < 2; %n++)
		%obj.getDatablock().schedule(750 * %n,Spit,%obj);
	}

	Parent::onBotFollow(%this,%obj,%targ);
}

	function ZombieSpitterHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this,%obj,%col);
	for(%i=0;%i<5;%i++)
	{
		%rnd = getRandom();
		%dist = getRandom()*15;
		%x = mCos(%rnd*$PI*3)*%dist;
		%y = mSin(%rnd*$PI*3)*%dist;
		%p = new projectile()
		{
			datablock = SpitterSpewedProjectile;
			initialPosition = %obj.getPosition();
			initialVelocity = %x SPC %y SPC (getRandom()*4);
			client = %obj.client;
			sourceObject = %obj;
			damageType = $DamageType::SpitAcidBall;
		};
	}
}

function ZombieSpitterHoleBot::onDamage(%this,%obj)
{
	%obj.setShapeNameHealth();

	if(%obj.getstate() $= "Dead")
	return;

	if(%obj.lastdamage+500 < getsimtime())//Check if the chest is the female variant and add a 1 second cooldown
	{
		%obj.playaudio(0,"spitter_pain" @ getrandom(1,3) @ "_sound");
		if(%obj.raisearms)
		{
			%obj.raisearms = 0;
			%obj.playthread(1,"root");
		}

		%obj.playthread(2,"plant");
		%obj.lastdamage = getsimtime();
	}

	Parent::OnDamage(%this,%obj);
}

function ZombieSpitterHoleBot::onDisabled(%this,%obj)
{
	if(%obj.getstate() !$= "Dead")
	return Parent::OnDisabled(%this,%obj);
	
	%obj.playaudio(0,"spitter_death" @ getrandom(1,2) @ "_sound");

	for(%i=0;%i<25;%i++)
	{
		%rnd = getRandom();
		%dist = getRandom()*15;
		%x = mCos(%rnd*$PI*3)*%dist;
		%y = mSin(%rnd*$PI*3)*%dist;
		%p = new projectile()
		{
			datablock = SpitterSpewedProjectile;
			initialPosition = %obj.getPosition();
			initialVelocity = %x SPC %y SPC (getRandom()*4);
			client = %obj.client;
			sourceObject = %obj;
			damageType = $DamageType::SpitAcidBall;
		};
	}

	Parent::OnDisabled(%this,%obj);
}

function ZombieSpitterHoleBot::L4BSpecialAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{
	%pantsColor = getRandomBotPantsColor();
	%shoeColor = getRandomBotPantsColor();

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
	%rLegColor = %shoeColor;
	else %rLegColor = %skinColor;
	%lLegColor = getRandom(0,1);
	if(%lLegColor)
	%lLegColor = %shoeColor;
	else %lLegColor = %skinColor;
	%handColor = %skinColor;

	%obj.llegColor =  %lLegColor;
	%obj.secondPackColor =  "0 0.435 0.831 1";
	%obj.lhand =  "0";
	%obj.hip =  "0";
	%obj.rarmColor =  %skinColor;
	%obj.hatColor =  "1 1 1 1";
	%obj.hipColor =  %pantsColor;
	%obj.chest =  "1";
	%obj.decalName =  "worm-engineer";
	%obj.rarm =  "0";
	%obj.packColor =  "0.2 0 0.8 1";
	%obj.pack =  "0";
	%obj.larmColor =  %skinColor;
	%obj.secondPack =  "0";
	%obj.larm =  "0";
	%obj.chestColor =  %skinColor;
	%obj.accentColor =  "0.990 0.960 0 0.700";
	%obj.rhandColor =  %skinColor;
	%obj.rleg =  "0";
	%obj.rlegColor =  %rLegColor;
	%obj.accent =  "1";
	%obj.headColor =  %skinColor;
	%obj.faceName = %face;
	%obj.rhand =  "0";
	%obj.lleg =  "0";
	%obj.lhandColor =  %skinColor;
	%obj.hat =  %hat;

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function ZombieSpitterHoleBot::Spit(%this, %obj)
{
	if(!isObject(%obj) || %obj.getstate() $= "Dead")
	return;

	if(isObject(%obj.light))
	%obj.light.delete();
	
	%obj.setenergylevel(0);
	%obj.playaudio(2,"spitter_spit_sound");
	%obj.playthread(0,"plant");

	%muzzle = vectorAdd(%obj.getMuzzlePoint(2),"0 0 0.35");
	%velocity = vectorScale(%obj.getEyeVector(),40);

	%pm = new projectile()
	{
		dataBlock = "SpitterSpitProjectile";
		initialVelocity = %velocity;
		initialPosition = %muzzle;
		sourceObject = %obj;
		client = %obj.client;
	};
	MissionCleanup.add(%pm);

	%shellcount = 4;
	for(%shell=0; %shell<%shellcount; %shell++)
	{
		%x = (getRandom() - 0.5) * 10 * $pi * 0.001;
		%y = (getRandom() - 0.5) * 10 * $pi * 0.001;
		%z = (getRandom() - 0.5) * 10 * $pi * 0.001;
		%mat = MatrixCreateFromEuler(%x @ " " @ %y @ " " @ %z);
		%velocity = MatrixMulVector(%mat, %velocity);

		%p = new projectile()
		{
			dataBlock = "SpitterSpewedProjectile";
			initialVelocity = %velocity;
			initialPosition = %muzzle;
			sourceObject = %obj;
			client = %obj.client;
		};
		MissionCleanup.add(%p);
	}
}


function ZombieSpitterHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{	
	CommonZombieHoleBot::onTrigger (%this, %obj, %triggerNum, %val);
	if(%obj.getClassName() $= "Player" && %obj.getstate() !$= "Dead")
	{
		switch(%triggerNum)
		{
			case 4: if(%val && %obj.GetEnergyLevel() >= %this.maxenergy)
					%obj.getDatablock().Spit(%obj);
			default:
		}
	}

	Parent::onTrigger (%this, %obj, %triggerNum, %val);
}	

function SpitterSpitProjectile::onExplode(%obj,%this)
{
	for(%i=0;%i<3;%i++)
	{
		%rnd = getRandom();
		%dist = getRandom()*15;
		%x = mCos(%rnd*$PI*3)*%dist;
		%y = mSin(%rnd*$PI*3)*%dist;
		%p = new projectile()
		{
			datablock = SpitterSpewedProjectile;
			initialPosition = %this.getPosition();
			initialVelocity = %x SPC %y SPC (getRandom()*4);
			client = %this.sourceObject.client;
			sourceObject = %this.sourceObject;
			damageType = $DamageType::SpitAcidBall;
		};
	}
	Parent::onExplode(%obj,%this);
}

function SpitterSpitProjectile::damage(%this,%obj,%col,%fade,%pos,%normal)
{
   %damageType = $DamageType::SpitAcidBall;

   %scale = getWord(%obj.getScale(), 2);
   %directDamage = mClampF(%this.directDamage, -100, 100) * %scale;

	if(%col.getType() & $TypeMasks::PlayerObjectType)
		if(checkHoleBotTeams(%obj.sourceObject,%col))
		{
			%col.damage(%obj, %pos, %directDamage, %damageType);
			%col.Toxified = 1;
			ToxicityE(%col);
		}
}

function SpitterSpewedProjectile::damage(%this,%obj,%col,%fade,%pos,%normal)
{
   %damageType = $DamageType::SpitAcidBall;

   %scale = getWord(%obj.getScale(), 2);
   %directDamage = mClampF(%this.directDamage, -100, 100) * %scale;

   if(%col.getType() & $TypeMasks::PlayerObjectType)
		if(checkHoleBotTeams(%obj.sourceObject,%col))
		{
			%col.damage(%obj, %pos, %directDamage, %damageType);
			%col.Toxified = 1;
			ToxicityE(%col);
		}
}

//Don't package this since we have our own damage system, which I got from Badspot's modification topic
function SpitterSpewedProjectile::radiusDamage(%this, %obj, %col, %distanceFactor, %pos, %damageAmt)
{
   //validate distance factor
   if(%distanceFactor <= 0)
      return;
   else if(%distanceFactor > 1)
      %distanceFactor = 1;

   %damageAmt *= %distanceFactor;

   if(%damageAmt)
   {
      //use default damage type if no damage type is given
         %damageType = $DamageType::SpitAcidBall;
        if(%col.getType() & $TypeMasks::PlayerObjectType)
        {
			if(checkHoleBotTeams(%obj.sourceObject,%col))
			{
				%col.damage(%obj, %pos, %directDamage, %damageType);
			}
			//Toxicity(%col,%obj.sourceObject);
        }
   }
}