function ZombieJockeyHoleBot::onAdd(%this,%obj)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
}

function ZombieJockeyHoleBot::doDismount(%this, %obj, %forced) 
{ 
	if(isObject(%obj.hEating)) return Parent::doDismount(%this, %obj, %forced);
}

function ZombieJockeyHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);
	%obj.setscale("0.75 0.75 0.75");
}

function L4B_holeJockeyKill(%obj,%col)
{
	if(L4B_SpecialsPinCheck(%obj,%col))
	{
		if(%obj.getClassName() $= "AIPlayer") %obj.hRunAwayFromPlayer(%col);

		%col.damage(%obj.hFakeProjectile, %col.getposition(), $Pref::L4B::Zombies::SpecialsDamage/1.25, $DamageType::Jockey);
		%obj.playthread(2,"zAttack" @ getRandom(1,3));
		%obj.playThread(3,talk);
		%col.playThread(2,plant);
		%obj.playaudio(1,"melee_hit" @ getrandom(1,8) @ "_sound");

		%obj.JockeyHurt = schedule(1000,0,L4B_holeJockeyKill,%obj,%col);
	}
}

	function ZombieJockeyHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this,%obj,%col);	
}

	function ZombieJockeyHoleBot::OnCollision(%this, %obj, %col, %fade, %pos, %norm)
{
	Parent::OnCollision(%this, %obj, %col, %fade, %pos, %norm);	


	if(%obj.getState $= "Dead" || %col.getdatablock().isDowned) return;

	if(checkHoleBotTeams(%obj,%col)) %obj.hJump();	
	if((%oScale = getWord(%obj.getScale(),0)) == 0.75 && getWord(%obj.getvelocity(),2) != 0) %obj.SpecialPinAttack(%col);
	

	Parent::oncollision(%this, %obj, %col, %fade, %pos, %norm);
}

	function ZombieJockeyHoleBot::onBotLoop(%this,%obj)
{
	%obj.hNoSeeIdleTeleport();

	if(%obj.hEating) return;

	if(!%obj.hFollowing)
	{
		%obj.playaudio(0,"jockey_lurk" @ getrandom(1,4) @ "_sound");
		%obj.playThread(0,talk);
	}
	else %obj.playaudio(0,"jockey_recognize" @ getrandom(1,2) @ "_sound");
}

function ZombieJockeyHoleBot::onBotFollow( %this, %obj, %targ )
{
	if((!isObject(%obj) || %obj.getState() $= "Dead") || (!isObject(%targ) || %targ.getState() $= "Dead")) return;
	
	if(VectorDist(%obj.getposition(), %targ.getposition()) < 15)
	{
		L4B_ZombieLunge(%obj,%targ,5);
		%obj.setvelocity(vectorAdd(%obj.getVelocity(),"0 0 7.5"));

		%obj.playThread(1,activate2);
		%obj.playThread(2,shiftUp);
		%obj.playThread(3,jump);
	}
	else if(VectorDist(%obj.getposition(), %targ.getposition()) < 25) %this.schedule(1000,onBotFollow,%obj,%targ);
}

function ZombieJockeyHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	%limb = %obj.rgetDamageLocation(%position);
	if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact)
	if(%limb) %damage = %damage/6;
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function ZombieJockeyHoleBot::onDamage(%this,%obj,%source,%pos,%damage,%type)
{	
	if(isObject(%obj.hEating))
	{
		%obj.hEating.isBeingStrangled = 0;
		L4B_SpecialsPinCheck(%obj,%obj.hEating);
	}

	if(%obj.getstate() $= "Dead")
	return;

	if(%obj.lastdamage+500 < getsimtime())
	{
		%obj.playaudio(0,"jockey_pain" @ getrandom(1,4) @ "_sound");

		if(%obj.raisearms)
		{
			%obj.raisearms = 0;
			%obj.playthread(1,"root");
			%obj.playthread(2,"plant");
		}

		%obj.lastdamage = getsimtime();
	}
	parent::onDamage(%this,%obj,%source,%pos,%damage,%type);
}

	function ZombieJockeyHoleBot::onDisabled(%this, %obj)
{
	if(isObject(%obj.hEating))
	{
		%obj.hEating.isBeingStrangled = 0;
		L4B_SpecialsPinCheck(%obj,%obj.hEating);
	}
	
	if(%obj.getstate() !$= "Dead")
	return;

	%obj.playaudio(0,"jockey_death" @ getrandom(1,3) @ "_sound");

	parent::onDisabled(%this,%obj);
}

	function ZombieJockeyHoleBot::onImpact(%this, %obj, %col, %vec, %force)
{
	Parent::onImpact(%this, %obj, %col, %vec, %force);
	
	if(%obj.getState() $= "Dead") return;
	else 
	{
		%forcescale = %oscale+%force/50;
		%obj.spawnExplosion(pushBroomProjectile,%forcescale SPC %forcescale SPC %forcescale);

		if(isObject(%obj.hFollowing) && %obj.hFollowing.getState() !$= "Dead") %this.onBotFollow(%obj,%obj.hFollowing);
	}
}

function ZombieJockeyHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{	
	if(%obj.getClassName() $= "Player" && %obj.getstate() !$= "Dead")
	{
		switch(%triggerNum)
		{
			case 0: CommonZombieHoleBot::onTrigger(%this, %obj, %triggerNum, %val);
			case 4: if(%obj.GetEnergyLevel() >= %this.maxenergy && !%obj.isStrangling)
					{
						%normVec = VectorNormalize(vectoradd(%obj.getEyeVector(),"0 0 0.005"));
						%eye = vectorscale(%normVec,20);
						%obj.setvelocity(%eye);

						%obj.playthread(2,"activate2");
						%obj.playthread(0,"jump");
						%obj.setenergylevel(0);
					}
			default:
		}
	}
	Parent::onTrigger (%this, %obj, %triggerNum, %val);
}

function ZombieJockeyHoleBot::holeAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{
	%decal = "AAA-None";
	%hat = 0;	
	%pack = 0;
	%pack2 = 0;
	%accent = 0;
	%hatColor = getRandomBotRGBColor();
	%packColor = getRandomBotRGBColor();
	%shirtColor = getRandomBotRGBColor();
	%accentColor = getRandomBotRGBColor();
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

	%obj.llegColor =  %llegColor;
	%obj.secondPackColor =  %pack2Color;
	%obj.lhand =  "0";
	%obj.hip =  "0";
	%obj.faceName =  %face;
	%obj.rarmColor =  %skinColor;
	%obj.hatColor =  %hatcolor;
	%obj.hipColor =  %pantsColor;
	%obj.chest =  "0";
	%obj.rarm =  "0";
	%obj.packColor =  %packColor;
	%obj.pack =  "0";
	%obj.decalName =  %decal;
	%obj.larmColor =  %skinColor;
	%obj.secondPack =  "0";
	%obj.larm =  "0";
	%obj.chestColor =  %skinColor;
	%obj.accentColor =  %accentColor;
	%obj.rhandColor =  %skinColor;
	%obj.rleg =  "0";
	%obj.rlegColor =  %rlegColor;
	%obj.accent =  "1";
	%obj.headColor =  %skinColor;
	%obj.rhand =  "0";
	%obj.lleg =  "0";
	%obj.lhandColor =  %skinColor;
	%obj.hat =  "0";

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}