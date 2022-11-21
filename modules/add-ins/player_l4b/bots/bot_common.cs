function CommonZombieHoleBot::onAdd(%this,%obj)
{			
	Parent::onAdd(%this,%obj);
	%obj.hDefaultL4BAppearance();
}

function CommonZombieHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	%obj.onL4BDatablockAttributes();
	%obj.setscale("1 1 1");
	%obj.schedule(10,setenergylevel,%this.maxenergy);
}

function CommonZombieHoleBot::onDamage(%this,%obj)
{
	if(%obj.getstate() $= "Dead") return;

	if(%obj.lastdamage+1250 < getsimtime())
	{
		%obj.lastdamage = getsimtime();
		%obj.playthread(2,"plant");

		if(%obj.raisearms)
		{
			%obj.raisearms = 0;
			%obj.playthread(1,"root");
		}

		if(%obj.getWaterCoverage() == 1)
		{
			%obj.emote(oxygenBubbleImage, 1);
			serverPlay3D("drown_bubbles_sound",%obj.getPosition());
		}
		else if(%obj.isBurning)
		{
			switch(%obj.chest)	
			{
				case 0: %obj.playaudio(0,"zombiemale_ignite" @ getrandom(1,5) @ "_sound");
				case 1: %obj.playaudio(0,"zombiefemale_ignite1" @ getrandom(1,5) @ "_sound");
			}

			%obj.MaxSpazzClick = getrandom(16,32);
			L4B_SpazzZombie(%obj,0);
			
		}
		else switch(%obj.chest)	
		{
			case 0: %obj.playaudio(0,"zombiemale_pain" @ getrandom(1,8) @ "_sound");
			case 1: %obj.playaudio(0,"zombiefemale_pain" @ getrandom(1,8) @ "_sound");
		}
	}

	Parent::OnDamage(%this,%obj);
}

function CommonZombieHoleBot::onImpact(%this, %obj, %col, %vec, %force)
{
	luacall(Survivor_FallDamage,%obj,%vec,%force);
	Parent::onImpact(%this, %obj, %col, %vec, %force);
}

function CommonZombieHoleBot::onDisabled(%this,%obj)
{
	Parent::OnDisabled(%this,%obj);

	if(%obj.getstate() !$= "Dead") return;	
	if(isObject(%obj.client)) commandToClient(%obj.client,'SetVignette',$EnvGuiServer::VignetteMultiply,$EnvGuiServer::VignetteColor);

	if(isObject(%minigame = getMiniGameFromObject(%obj)) && %minigame.DirectorStatus == 2) 
	{
		%minigame.zhordecount--;
		if(%minigame.zhordecount < 1)
		{
			%minigame.RoundEnd();
			%minigmae.zhordecount = 0;
		}
	}	

	if(%obj.getWaterCoverage() == 1) serverPlay3D("die_underwater_bubbles_sound",%obj.getPosition());	
	else switch(%obj.chest)
	{
		case 0: %obj.playaudio(0,"zombiemale_death" @ getrandom(1,10) @ "_sound");
		case 1: %obj.playaudio(0,"zombiefemale_death" @ getrandom(1,10) @ "_sound");
	}
}

function CommonZombieHoleBot::onBotLoop(%this,%obj)
{
	if(%obj.getWaterCoverage() == 1) %obj.Damage(%obj,%obj.getPosition(),%obj.getdatablock().maxDamage/1.25,$DamageType::Suicide);

	if(%obj.hState !$= "Following")
	{		
		if(!isObject(%obj.distraction)) %obj.hSearch = 1;

		%obj.raisearms = 0;
		%obj.playthread(1,"root");
		%obj.setMaxForwardSpeed(7);
		%obj.setmaxUnderwaterForwardSpeed(7);
		%obj.hNoSeeIdleTeleport();
	}
}

function CommonZombieHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(!isObject(%obj) || %obj.getState() $= "Dead") return;

	%obj.playthread(2,plant);
	%obj.setMaxForwardSpeed(11);
	%obj.setmaxUnderwaterForwardSpeed(11);
	
	switch(%obj.chest)
	{
		case 0: %obj.playaudio(0,"zombiemale_attack" @ getrandom(1,10) @ "_sound");
		case 1: %obj.playaudio(0,"zombiefemale_attack" @ getrandom(1,12) @ "_sound");
	}

	if(isObject(%targ) && vectordist(%obj.getposition(),%targ.getposition()) < 15)
	{
		if(!%obj.raisearms)
		{	
			%obj.playthread(1,"armReadyboth");
			%obj.raisearms = 1;
		}

		if(getRandom(1,4) == 1) L4B_SpazzZombie(%obj,0);
	}
	else if(%obj.raisearms)
	{	
		%obj.playthread(1,"root");
		%obj.raisearms = 0;
	}	
}

function CommonZombieHoleBot::onCollision(%this, %obj, %col, %fade, %pos, %norm)
{
	if(%obj.lastattackis < getSimTime() && %col.getType() & $TypeMasks::PlayerObjectType && checkHoleBotTeams(%obj,%col) && minigameCanDamage(%obj,%col))
	{
		%obj.hMeleeAttack(%col);
		%obj.lastattackis = getSimTime()+750;
	}
	
	Parent::onCollision(%this, %obj, %col, %fade, %pos, %norm);
}

function CommonZombieHoleBot::onBotMelee(%this,%obj,%col)
{		
	%meleeimpulse = mClamp(%obj.hLastMeleeDamage, 1, 10);
	%obj.playaudio(1,"melee_hit" @ getrandom(1,8) @ "_sound");
	%obj.playthread(2,"zAttack" @ getRandom(1,3));
	%obj.setaimobject(%col);
	
	if(%col.getType() & $TypeMasks::PlayerObjectType)
	{
		if(%col.getClassName() $= "Player") %col.spawnExplosion("ZombieHitProjectile",%meleeimpulse/2 SPC %meleeimpulse/2 SPC %meleeimpulse/2);
		%col.playthread(3,"plant");
		%col.StunnedSlowDown(3);
		%col.setVelocity(vectorscale(%obj.getEyeVector(),-10));
	}
	else %col.applyimpulse(%col.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),getrandom(100,100*%meleeimpulse)),"0" SPC "0" SPC getrandom(100,100*%meleeimpulse)));
}

function CommonZombieHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{
	Parent::onTrigger (%this, %obj, %triggerNum, %val);

	if(%obj.getClassName() $= "Player" && %obj.getstate() !$= "Dead")
	{
		switch(%triggerNum)
		{
			case 0: if(%val && %obj.getEnergyLevel() > 25)
					{
						if(!%obj.IsZombieArmsUp)
						{
							%obj.playthread(2,armReadyboth);
							%obj.IsZombieArmsUp = 1;
						}

						if(isObject(%touchedobj = %obj.lastactivated) && checkHoleBotTeams(%obj,%touchedobj)) %obj.hMeleeAttack(%touchedobj);

						cancel(%obj.ZombieLowerArmsSchedule);
						%obj.ZombieLowerArmsSchedule = %obj.schedule(500,ZombieLowerArms);
					}
			default:
		}
	}
}

function Player::ZombieLowerArms(%player)
{
	if(isObject(%player))
	{
		%player.playthread(2,root);
		%player.IsZombieArmsUp = 0;
	}
}

function CommonZombieHoleBot::holeAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{	
	//if(getRandom(1,8) == 1)
	//{ 
	//	L4B_pushClientSnapshot(%obj,0,true);
	//	return;
	//}

	%shirtColor = getRandomBotRGBColor();
	%accentColor = getRandomBotRGBColor();
	%pantsColor = getRandomBotPantsColor();
	%shoeColor = getRandomBotPantsColor();
	%hatColor = getRandomBotRGBColor();
	%packColor = getRandomBotRGBColor();
	
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

	%pack2 = 0;
	%accent = 0;
	%obj.accentColor = %accentColor;
	%obj.accent =  %accent;
	%obj.hatColor = %hatColor;
	%obj.hat = %hat;
	%obj.headColor = %skinColor;
	%obj.faceName = %face;
	%obj.chest =  %chest;
	%obj.decalName = %decal;
	%obj.chestColor = %shirtColor;
	%obj.pack =  0;
	%obj.packColor =  %packColor;
	%obj.secondPack =  %pack2;
	%obj.secondPackColor =  %packColor;
	%obj.larm =  "0";
	%obj.larmColor = %larmColor;
	%obj.lhand =  0;
	%obj.lhandColor = %handColor;
	%obj.rarm =  "0";
	%obj.rarmColor = %rarmColor;
	%obj.rhandColor = %handColor;
	%obj.rhand = 0;
	%obj.hip =  "0";
	%obj.hipColor = %pantsColor;
	%obj.lleg =  0;
	%obj.llegColor = %lLegColor;
	%obj.rleg =  0;
	%obj.rlegColor = %rLegColor;

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function CommonZombieHoleBot::L4BAppearance(%this,%obj,%client) { SurvivorPlayer::L4BAppearance(%this,%obj,%client); }