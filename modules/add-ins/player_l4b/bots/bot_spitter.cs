function ZombieSpitterHoleBot::onAdd(%this,%obj,%style)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
}

function ZombieSpitterHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);
	%obj.setscale("1 0.95 1.15");
}

function ZombieSpitterHoleBot::onBotLoop(%this,%obj)
{
	if(%obj.getstate() !$= "Dead" && %obj.lastIdle+5000 < getsimtime())
	{
		switch$(%obj.hState)
		{
			case "Wandering":	%obj.isStrangling = false;
								%obj.hNoSeeIdleTeleport();
								%obj.playaudio(0,"spitter_lurk" @ getrandom(1,3) @ "_sound");

			case "Following": 	%obj.playaudio(0,"spitter_recognize" @ getrandom(1,3) @ "_sound");
		}

		%obj.playthread(3,plant);
		%obj.lastIdle = getsimtime();		
	}	
}


function ZombieSpitterHoleBot::onBotFollow( %this, %obj, %targ )
{
	if((isObject(%obj) && %obj.getState() !$= "Dead" && %obj.hLoopActive) && (isObject(%targ) && %targ.getState() !$= "Dead") && (%distance = vectorDist(%obj.getposition(),%targ.getposition())) < 25)
	%this.schedule(500,onBotFollow,%obj,%targ);
	else return;

	if((%distance = vectorDist(%obj.getposition(),%targ.getposition())) > 10 && %distance < 25)
	{
		if(%obj.GetEnergyLevel() >= %this.maxenergy) %this.onTrigger(%obj,4,1);				
		%obj.setMoveX(0);
		%obj.setMoveY(0);
		%obj.setaimobject(%targ);
	}
	else if(%distance < 12) %obj.hRunAwayFromPlayer(%targ);
	
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
	Parent::OnDamage(%this,%obj);

    if(%obj.getstate() !$= "Dead")
	{	
		if(%delta > 5 && %obj.lastdamage+1000 < getsimtime())
		{
			%obj.playaudio(0,"spitter_pain" @ getrandom(1,3) @ "_sound");			

			%obj.playthread(2,"plant");			

			if(%obj.raisearms)
			{
				%obj.raisearms = false;	
				%obj.playthread(1,"root");
			}
			%obj.lastdamage = getsimtime();
		}
	}
	else
	{
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
		%obj.playaudio(0,"spitter_death" @ getrandom(1,2) @ "_sound");
	}
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
	%obj.hat =  0;

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function ZombieSpitterHoleBot::Spit(%this,%obj,%limit,%count)
{
	if(isObject(%obj) && %obj.getState() !$= "Dead" && %count <= 3)
	{
		%obj.setenergylevel(0);
		%obj.playaudio(2,"spitter_spit_sound");
		%obj.playthread(0,"plant");
		%obj.spitschedule = %this.schedule(100,Spit,%obj,%limit,%count+1);

		%pm = new projectile()
		{
			dataBlock = "SpitterSpitProjectile";
			initialVelocity = vectorAdd(vectorscale(%obj.getEyeVector(),50),"0 0 2.5");
			initialPosition = vectorAdd(%obj.getMuzzlePoint(2),"0 0 0.45");
			sourceObject = %obj;
			client = %obj.client;
		};
		MissionCleanup.add(%pm);
	}
}


function ZombieSpitterHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{	
	Parent::onTrigger(%this, %obj, %triggerNum, %val);

	CommonZombieHoleBot::onTrigger (%this, %obj, %triggerNum, %val);
	if(%obj.getstate() !$= "Dead") switch(%triggerNum)
	{
		case 4: if(%val && %obj.GetEnergyLevel() >= %this.maxenergy) %this.Spit(%obj,3);
		default:
	}	
}	

function SpitterSpitProjectile::onExplode(%obj,%this)
{
	for(%i=0;%i<4;%i++)
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
	
	if((%col.getType() & $TypeMasks::PlayerObjectType) && checkHoleBotTeams(%obj.sourceObject,%col)) %col.damage(%obj, %pos, %directDamage, %damageType);
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
   	if(%distanceFactor <= 0) return;
   	else if(%distanceFactor > 1) %distanceFactor = 1;
   	%damageAmt *= %distanceFactor;   
	if(%damageAmt && (%col.getType() & $TypeMasks::PlayerObjectType) && checkHoleBotTeams(%obj.sourceObject,%col)) %col.damage(%obj, %pos, %directDamage, $DamageType::SpitAcidBall);
   
}