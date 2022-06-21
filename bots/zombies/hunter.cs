datablock PlayerData(ZombieHunterHoleBot : CommonZombieHoleBot)
{
	uiName = "Hunter Infected";
	speedDamageScale = 0;

	cameramaxdist = 4;
    cameraVerticalOffset = 1;
    cameraHorizontalOffset = 0.6;
    cameratilt = 0.1;
    maxfreelookangle = 2;

    maxForwardSpeed = 8;
    maxBackwardSpeed = 7;
    maxSideSpeed = 6;

 	maxForwardCrouchSpeed = 6;
    maxBackwardCrouchSpeed = 5;
    maxSideCrouchSpeed = 4;

	ShapeNameDistance = 100;
	hIsInfected = 2;
	hZombieL4BType = 5;
	hCustomNodeAppearance = 1;
	hPinCI = "<bitmapk:Add-Ons/Package_Left4Block/icons/ci_hunter2>";
	SpecialCPMessage = "Hold shift, then press space to leap <br>\c6Pounce to pin non-infected";
	hBigMeleeSound = "";

	maxdamage = 125;//Health
	hTickRate = 5000;

	hName = "Hunter";//cannot contain spaces
	hStrafe = 0;//Randomly strafe while following player
	hAttackDamage = $L4B_SpecialsDamage;

	rechargeRate = 1.75;
	maxenergy = 100;
	showEnergyBar = true;
};

function ZombieHunterHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);
}

function ZombieHunterHoleBot::onAdd(%this,%obj,%style)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
}

function L4B_holeHunterKill(%obj,%col)
{
	if(L4B_SpecialsPinCheck(%obj,%col))
	{
		%obj.setenergylevel(0);
		if(%obj.getClassName() !$= "Player")
		{
			%obj.setmoveobject(%col);
			%obj.setaimobject(%col.gethackposition());
			%obj.hMeleeAttack(%col);
		}
		
		%obj.HunterHurt = schedule(1000,0,L4B_holeHunterKill,%obj,%col);
		%obj.unmount();
		%col.damage(%obj.hFakeProjectile, %col.getposition(), $L4B_SpecialsDamage/2, $DamageType::Hunter);
	}
}

function ZombieHunterHoleBot::onDamage(%this,%obj)
{
	%obj.setShapeNameHealth();
	
	if(%obj.getstate() $= "Dead")
	return;

    if(%obj.lastdamage+1000 < getsimtime())//Check if the chest is the female variant and add a 1 second cooldown
	{
		%obj.playaudio(0,"hunter_pain" @ getrandom(1,3) @ "_sound");
		%obj.playthread(2,"plant");
		%obj.lastdamage = getsimtime();
	}

	if(%obj.raisearms)
	{
		%obj.raisearms = 0;	
		%obj.playthread(1,plant);
	}

	if(isObject(%obj.hEating))
	{
		%obj.hEating.isBeingStrangled = 0;
		L4B_SpecialsPinCheck(%obj,%obj.hEating);
		%obj.lastpounce = getsimtime()+5000;
		cancel(%obj.hAbouttoattack);
	}

	Parent::onDamage(%this,%obj);
}

function ZombieHunterHoleBot::onDisabled(%this,%obj)
{
	if(%obj.getstate() !$= "Dead")
	return;

	%obj.playaudio(0,"hunter_death" @ getrandom(1,3) @ "_sound");

	if(isObject(%obj.hEating))
	{
		%obj.hEating.isBeingStrangled = 0;
		L4B_SpecialsPinCheck(%obj,%obj.hEating);
		cancel(%obj.hAbouttoattack);
	}

	Parent::onDisabled(%this,%obj);
}

function ZombieHunterHoleBot::onBotLoop(%this,%obj)
{
	%obj.hAttackDamage = $L4B_SpecialsDamage;
	%obj.hLimitedLifetime();
	
	if(!%obj.hFollowing)
	{
		%obj.setMaxForwardSpeed(9);
		%obj.raisearms = 0;
		%obj.isstrangling = 0;
		%obj.playthread(1,"root");
		%obj.playthread(0,root);

		if(getsimtime() >= %obj.lastidle+8000 && !%obj.isstrangling)
		{
			%obj.playaudio(0,"hunter_idle" @ getrandom(1,3) @ "_sound");
			%obj.playthread(3,"plant");
			%obj.lastidle = getSimTime();
		}
	}
}

function ZombieHunterHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(!%obj.raisearms)
	{	
		%obj.playthread(1,"armReadyboth");
		%obj.raisearms = 1;
	}

	if(checkHoleBotTeams(%obj,%targ) && !%obj.isstrangling && !%targ.isBeingStrangled)
	{
		if(%obj.lastpounce+5000 < getsimtime())
		{
			%obj.lastpounce = getsimtime();
		
			%obj.hCrouch(1750);
			L4B_SpecialsWarningLight(%obj);
			%obj.playaudio(0,"hunter_recognize" @ getrandom(1,3) @ "_sound");
			%obj.hAbouttoattack = schedule(1250,0,L4B_HunterZombieLunge,%obj,%targ);
		}
	}
}

function L4B_HunterZombieLunge(%obj,%targ)
{
	if(!isObject(%obj) || !isObject(%targ) || !L4B_IsOnGround(%obj) || %obj.getState() $= "Dead" || %obj.isstrangling)
	return;

	%obj.playaudio(0,"hunter_attack" @ getrandom(1,3) @ "_sound");
	%obj.playaudio(1,"hunter_lunge_sound");

	if(isObject(%obj.light))
	%obj.light.delete();

	%obj.playthread(3,activate2);
	%obj.playthread(0,jump);

	%dissub = VectorSub(%targ.getposition(),%obj.getposition());
	%dis = vectordist(%targ.getposition(),%obj.getposition())*0.5;
	%normVec = VectorNormalize(vectoradd(%dissub,"0 0" SPC 0.125*vectordist(%targ.getposition(),%obj.getposition())));
	%eye = vectorscale(%normVec,50+%dis);
	%obj.setvelocity(%eye);

	cancel(%obj.hAbouttoattack);
	%obj.spawnExplosion(pushBroomProjectile,%dis*0.01 SPC %dis*0.01 SPC %dis*0.01);
}

function ZombieHunterHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this,%obj,%col);
	%obj.playaudio(2,"hunter_hit" @ getrandom(1,3) @ "_sound");
}

	function ZombieHunterHoleBot::onImpact(%this, %obj, %col, %vec, %force)
{
	%oScale = getWord(%obj.getScale(),0);
	%forcescale = %oscale+%force/50;
	%obj.spawnExplosion(pushBroomProjectile,%forcescale SPC %forcescale SPC %forcescale);
	%obj.setMaxForwardSpeed(9);

	if(%oScale >= 0.9 && %obj.getstate() !$= "Dead")
	%obj.SpecialPinAttack(%col,%force);

	if(isObject(%obj.hFollowing) && %obj.hFollowing.getState() !$= "Dead" && %obj.getState() !$= "Dead" && !%obj.isStrangling && !%obj.hFollowing.isBeingStrangled)
	{
		if(%obj.lastpounce+5000 < getsimtime())
		{
			%obj.getDatablock().onBotFollow(%obj,%obj.hFollowing);
			%obj.lastpounce = getsimtime();
		}
	}

	Parent::onImpact(%this, %obj, %col, %vec, %force);
}

function ZombieHunterHoleBot::L4BSpecialAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{	
	%clothesrandmultiplier = getrandom(2,8)*0.25;
	%shirtColor = 0.075 SPC 0.125*%clothesrandmultiplier SPC 0.1875*%clothesrandmultiplier SPC 1;
	%pantsColor = 0.15 SPC 0.125*%clothesrandmultiplier SPC 0.05*%clothesrandmultiplier SPC 1;
	%hatColor = %shirtColor;
	%shoeColor = getRandomBotPantsColor();
	%packColor = getRandomBotRGBColor();
	%pack2Color = getRandomBotRGBColor();
	%accentColor = getRandomBotRGBColor();		
	%decal = "Hoodie";
	%hat = 1;
	%pack = 0;
	%pack2 = 0;
	%accent = 0;
	%chest = 0;

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

	// accent
	%obj.accentColor = %accentColor;
	%obj.accent =  %accent;
	
	// hat
	%obj.hatColor = %hatColor;
	%obj.hat = %hat;
	
	// head
	%obj.headColor = %skinColor;
	%obj.faceName = %face;
	
	// chest
	%obj.chest =  %chest;

	%obj.decalName = %decal;
	%obj.chestColor = %shirtColor;
		
	// packs
	%obj.pack =  %pack;
	%obj.packColor =  %packColor;

	%obj.secondPack =  %pack2;
	%obj.secondPackColor =  %packColor;
		
	// left arm
	%obj.larm =  "0";
	%obj.larmColor = %larmColor;
	
	%obj.lhand =  0;
	%obj.lhandColor = %handColor;
	
	// right arm
	%obj.rarm =  "0";
	%obj.rarmColor = %rarmColor;
	
	%obj.rhandColor = %handColor;
	%obj.rhand = 0;
	
	// hip
	%obj.hip =  "0";
	%obj.hipColor = %pantsColor;
	
	// left leg
	%obj.lleg =  0;
	%obj.llegColor = %lLegColor;
	
	// right leg
	%obj.rleg =  0;
	%obj.rlegColor = %rLegColor;

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function ZombieHunterHoleBot::hCustomNodeAppearance(%this,%obj)
{
	if(%obj.getClassName() $= "Player")
	{
		%handColorL = %obj.client.zombieColor;
		%handColorR = %obj.client.zombieColor;
	}
	else
	{
		%handColorL = %obj.lhandColor;
		%handColorR = %obj.rhandColor;
	}

	%obj.unhidenode(Lhand);
    %obj.unhidenode(Rhand);

    %obj.unhidenode(LhandWitch);
    %obj.unhidenode(RhandWitch);
    %obj.setnodecolor(LhandWitch,%handColorL);
    %obj.setnodecolor(RhandWitch,%handColorR);

    %obj.unhidenode(LhandWitchClaws);
    %obj.unhidenode(RhandWitchClaws);
	%obj.setnodecolor(RhandWitchClaws,"1 0.25 0.25 1");
    %obj.setnodecolor(LhandWitchClaws,"1 0.25 0.25 1");
}

function ZombieHunterHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{	
	CommonZombieHoleBot::onTrigger (%this, %obj, %triggerNum, %val);

	if(%obj.getClassName() $= "Player" && %obj.getstate() !$= "Dead")
	if(%obj.GetEnergyLevel() >= %this.maxenergy)
	{
		switch(%triggerNum)
		{
			case 3: if(%val)
					{
						L4B_SpecialsWarningLight(%obj);

						%obj.playaudio(0,"hunter_recognize" @ getrandom(1,3) @ "_sound");
						%obj.BeginPounce = 1;
					}
					else {
							%obj.BeginPounce = 0;
							if(isObject(%obj.light))
							%obj.light.delete();
						 }

			case 2: if(%val && L4B_IsOnGround(%obj) || L4B_IsOnWall(%obj))
					if(%obj.BeginPounce)
					{
						%obj.BeginPounce = 0;
						
						if(isObject(%obj.light))
						%obj.light.delete();

						%obj.setenergylevel(0);

						%obj.playaudio(0,"hunter_attack" @ getrandom(1,3) @ "_sound");
						%obj.playaudio(1,"hunter_lunge_sound");
						%obj.playthread(0,jump);
						%obj.playthread(1,activate2);

						%normVec = VectorNormalize(vectoradd(%obj.getEyeVector(),"0 0 0.005"));
						%eye = vectorscale(%normVec,50);
						%obj.setvelocity(%eye);
					}
			default:
		}
	}
	Parent::onTrigger (%this, %obj, %triggerNum, %val);
}