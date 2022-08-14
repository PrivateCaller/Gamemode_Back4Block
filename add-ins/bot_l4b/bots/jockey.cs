datablock PlayerData(ZombieJockeyHoleBot : CommonZombieHoleBot)
{
	uiName = "Jockey Infected";
	jumpForce = 90*8;
	minImpactSpeed = 10;
	airControl = 0.1;
	speedDamageScale = 0.01;

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

	maxdamage = 125;//Health
	jumpForce = 100 * 10; //8.3 * 90;
	hTickRate = 5000;

	hName = "Jockey";//cannot contain spaces
	hAttackDamage = $Pref::Server::L4B2Bots::SpecialsDamage;

	ShapeNameDistance = 100;
	hIsInfected = 2;
	hZombieL4BType = 5;
	hCustomNodeAppearance = 0;
	hPinCI = "<bitmapk:Add-Ons/Gamemode_Left4Block/add-ins/bot_l4b/icons/ci_jockey2>";
	SpecialCPMessage = "Right click to leap <br>\c6Jump on non-infected to control them";
	hBigMeleeSound = "";

	rechargeRate = 1.5;
	maxenergy = 100;
	showEnergyBar = true;
};

function ZombieJockeyHoleBot::onAdd(%this,%obj)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
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
		if(%obj.getClassName() $= "AIPlayer")
		%obj.hRunAwayFromPlayer(%col);

		%col.damage(%obj.hFakeProjectile, %col.getposition(), $Pref::Server::L4B2Bots::SpecialsDamage/2, $DamageType::Jockey);
		%col.playaudio(1,"zombie_hit" @ getrandom(1,8) @ "_sound");
		%obj.playThread(2,shiftdown);
		%obj.playThread(3,talk);
		%col.playThread(2,plant);

		%obj.JockeyHurt = schedule(1000,0,L4B_holeJockeyKill,%obj,%col);
	}
}

	function ZombieJockeyHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this,%obj,%col);	
}

	function ZombieJockeyHoleBot::OnCollision(%this, %obj, %col, %fade, %pos, %norm)
{
	if(%obj.getState() $= "Dead")
	return Parent::OnCollision(%this, %obj, %col, %fade, %pos, %norm);
	
	%oScale = getWord(%obj.getScale(),0);
	if(%oScale >= 0.75 && %oScale <= 0.85 && !L4B_IsOnGround(%obj))
	%obj.SpecialPinAttack(%col);
	
	if(%obj.getClassName() $= "AIPlayer")
	%obj.hJump();

	Parent::oncollision(%this, %obj, %col, %fade, %pos, %norm);
}

	function ZombieJockeyHoleBot::onBotLoop(%this,%obj)
{
	%obj.hLimitedLifetime();
	%obj.hAttackDamage = $Pref::Server::L4B2Bots::SpecialsDamage;

	if(%obj.hEating)
	return;

	if(!%obj.hFollowing)
	{
		%obj.playaudio(0,"jockey_lurk" @ getrandom(1,4) @ "_sound");
		%obj.playThread(0,talk);
		%obj.setMaxForwardSpeed(9);
	}
	else %obj.playaudio(0,"jockey_recognize" @ getrandom(1,2) @ "_sound");
}

function ZombieJockeyHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(%obj.hEating)
	return;

	%obj.hJump();
	%obj.setMaxForwardSpeed(11);
	%obj.playThread(1,activate2);
	%obj.playThread(2,shiftUp);
	%obj.playThread(3,jump);

	%tPos = %obj.getPosition();//our position
	%oPos = %targ.getPosition();
	%dis = VectorSub(%oPos, %tPos); //displacement is distance and direction of object from us, it's pos - our pos
	%normVec = vectorscale(VectorNormalize(%dis),250); //get rid of the distance (setting it to 1) so we only have direction left
	
	%obj.schedule(500,applyImpulse,%targ.getPosition(), %normVec);
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
	if(%obj.getState() $= "Dead")
	return Parent::onImpact(%this, %obj, %col, %vec, %force);

	%obj.applyImpulse(%obj,"0 0 100");

	%oScale = getWord(%obj.getScale(),0);
	%forcescale = %oscale+%force/50;
	%obj.spawnExplosion(pushBroomProjectile,%forcescale SPC %forcescale SPC %forcescale);

	if(isObject(%obj.hFollowing) && %obj.hFollowing.getState() !$= "Dead" && !%obj.isStrangling && !%obj.hFollowing.isBeingStrangled)
	%obj.getDatablock().onBotFollow(%obj,%obj.hFollowing);

	Parent::onImpact(%this, %obj, %col, %vec, %force);
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

	function ZombieJockeyHoleBot::L4BSpecialAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
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
	
	%rLegColor = getRandom(0,1);
	if(%rLegColor)
	%rLegColor = %shoeColor;
	else %rLegColor = %skinColor;
	%lLegColor = getRandom(0,1);
	if(%lLegColor)
	%lLegColor = %shoeColor;
	else %lLegColor = %skinColor;
	%handColor = %skinColor;

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