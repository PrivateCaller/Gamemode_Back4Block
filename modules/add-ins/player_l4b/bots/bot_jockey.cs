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

	
	if(getWord(%obj.getvelocity(),2) != 0) if((%oScale = getWord(%obj.getScale(),0)) == 0.75) %obj.SpecialPinAttack(%col);
	else if(checkHoleBotTeams(%obj,%col)) %obj.hJump();

	Parent::onCollision(%this, %obj, %col, %fade, %pos, %norm);
}

function ZombieJockeyHoleBot::onBotLoop(%this,%obj)
{	
	switch$(%obj.hState)
	{
		case "Wandering":	%obj.setMaxForwardSpeed(9);
							%obj.isStrangling = false;
							%obj.hNoSeeIdleTeleport();
							%obj.playThread(0,talk);
							%sound = "jockey_lurk" @ getrandom(1,4) @ "_sound";				
		case "Following": 	%sound = "jockey_recognize" @ getrandom(1,2) @ "_sound";
	}

	if(getsimtime() >= %obj.LastLoopSound+4000)
	{
		%obj.playaudio(0,%sound);		
		%obj.LastLoopSound = getSimTime();
	}
}

function ZombieJockeyHoleBot::onBotFollow(%this,%obj,%targ)
{
	if(!isObject(%targ) || !isObject(%obj) || %obj.isStrangling || %obj.GetEnergyLevel() < %this.maxenergy)
	{
		if(isObject(%targ) && isObject(%obj)) %this.schedule(500,onBotFollow,%obj,%targ);
		return;
	}
	
	if(VectorDist(%obj.getposition(), %targ.getposition()) < 20 && getWord(%obj.getvelocity(),2) <= 5)
	{	
		%obj.hJump();
		%obj.schedule(325,hShootAim,%targ);
		%this.schedule(375,onTrigger,%obj,4,1);
	}
	
	%this.schedule(500,onBotFollow,%obj,%targ);
}

function ZombieJockeyHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	%limb = %obj.rgetDamageLocation(%position);
	if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact)
	if(%limb) %damage = %damage/6;
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function ZombieJockeyHoleBot::onDamage(%this,%obj,%delta)
{	
	if(%delta > 5 && %obj.lastdamage+500 < getsimtime())
	{
		if(%obj.getstate() !$= "Dead") %obj.playaudio(0,"jockey_pain" @ getrandom(1,4) @ "_sound");
		else %obj.playaudio(0,"jockey_death" @ getrandom(1,3) @ "_sound");

		if(%obj.raisearms)
		{
			%obj.raisearms = 0;
			%obj.playthread(1,"root");
			%obj.playthread(2,"plant");
		}

		if(isObject(%obj.hEating))
		{
			%obj.hEating.isBeingStrangled = false;
			L4B_SpecialsPinCheck(%obj,%obj.hEating);
		}

		%obj.lastdamage = getsimtime();
	}
	Parent::onDamage(%this,%obj,%source,%pos,%damage,%type);
}

function ZombieJockeyHoleBot::onDisabled(%this, %obj)
{
	Parent::onDisabled(%this,%obj);
}

function ZombieJockeyHoleBot::onImpact(%this, %obj, %col, %vec, %force)
{
	Parent::onImpact(%this, %obj, %col, %vec, %force);	
}

function ZombieJockeyHoleBot::onTrigger(%this, %obj, %triggerNum, %val)
{	
	if(%obj.getstate() !$= "Dead")
	{
		if(%val) switch(%triggerNum)
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