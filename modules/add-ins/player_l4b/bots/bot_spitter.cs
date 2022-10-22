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
	%obj.hNoSeeIdleTeleport();

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
	if((!isObject(%obj) || %obj.getState() $= "Dead") || (!isObject(%targ) || %targ.getState() $= "Dead")) return;

	if((%distance = vectorDist(%obj.getposition(),%targ.getposition())) > 10 && %distance < 25)
	{
		%obj.setaimobject(%targ);
		%obj.hClearMovement();
		
		for (%n = 0; %n < 4; %n++) 
		{
			%obj.schedule(650 * %n,hShootAim,%targ);
			%obj.getDatablock().schedule(750 * %n,Spit,%obj);
		}
	}
	else if(%distance < 12) 
	{
		%obj.hRunAwayFromPlayer(%targ);	
		%this.schedule(1000,onBotFollow,%obj,%targ);
	}
	else %this.schedule(1000,onBotFollow,%obj,%targ);
}

	function ZombieSpitterHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this,%obj,%col);
}

function ZombieSpitterHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	%limb = %obj.rgetDamageLocation(%position);
	if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact)
	if(%limb) %damage = %damage/6;
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function ZombieSpitterHoleBot::onDamage(%this,%obj)
{
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

function ZombieSpitterHoleBot::holeAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{
	%pantsColor = getRandomBotPantsColor();
	%shoeColor = getRandomBotPantsColor();

	%handColor = %skinColor;
	%larmColor = %shirtColor;
	%rarmColor = %shirtColor;
	%rLegColor = %shoeColor;
	%lLegColor = %shoeColor;

	if(getRandom(1,4) == 1)
	{
		if(getRandom(1,0)) %larmColor = %skinColor;
		if(getRandom(1,0)) %rarmColor = %skinColor;
		if(getRandom(1,0)) %rLegColor = %skinColor;
		if(getRandom(1,0)) %lLegColor = %skinColor;
	}

	%obj.llegColor =  %lLegColor;
	%obj.secondPackColor =  "0 0.435 0.831 1";
	%obj.lhand =  "0";
	%obj.hip =  "0";
	%obj.rarmColor =  %skinColor;
	%obj.hatColor =  "1 1 1 1";
	%obj.hipColor =  %pantsColor;
	%obj.chest =  "1";
	%obj.decalName =  "witch";
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
	if(!isObject(%obj) || %obj.getstate() $= "Dead") return;
	
	%obj.setenergylevel(0);
	%obj.playaudio(2,"spitter_spit_sound");
	%obj.playthread(0,"plant");

	%muzzle = vectorAdd(%obj.getMuzzlePoint(2),"0 0 0.35");
	%vector = vectorAdd(vectorscale(%obj.getEyeVector(),40),"0 0 2.5");
	if(isObject(%obj.hFollowing)) %velocity = vectorAdd(%velocity,getWord(%obj.hFollowing.getVelocity(),0)/2 SPC getWord(%obj.hFollowing.getVelocity(),1)/2 SPC getWord(%obj.hFollowing.getVelocity(),2)/2);

	%pm = new projectile()
	{
		dataBlock = "SpitterSpitProjectile";
		initialVelocity = %vector;
		initialPosition = %muzzle;
		sourceObject = %obj;
		client = %obj.client;
	};
	MissionCleanup.add(%pm);
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
	for(%i=0;%i<8;%i++)
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
	%col.damage(%obj, %pos, %directDamage, %damageType);
}

function SpitterSpewedProjectile::damage(%this,%obj,%col,%fade,%pos,%normal)
{
   %damageType = $DamageType::SpitAcidBall;

   %scale = getWord(%obj.getScale(), 2);
   %directDamage = mClampF(%this.directDamage, -100, 100) * %scale;

   	if(%col.getType() & $TypeMasks::PlayerObjectType)
	if(checkHoleBotTeams(%obj.sourceObject,%col))
	%col.damage(%obj, %pos, %directDamage, %damageType);
}

//Don't package this since we have our own damage system, which I got from Badspot's modification topic
function SpitterSpewedProjectile::radiusDamage(%this, %obj, %col, %distanceFactor, %pos, %damageAmt)
{
   //validate distance factor
   if(%distanceFactor <= 0) return;
   else if(%distanceFactor > 1) %distanceFactor = 1;
   %damageAmt *= %distanceFactor;
   if(%damageAmt)
   {
      //use default damage type if no damage type is given
         %damageType = $DamageType::SpitAcidBall;
        if(%col.getType() & $TypeMasks::PlayerObjectType)
        {
			if(checkHoleBotTeams(%obj.sourceObject,%col)) %col.damage(%obj, %pos, %directDamage, %damageType);
			//Toxicity(%col,%obj.sourceObject);
        }
   }
}