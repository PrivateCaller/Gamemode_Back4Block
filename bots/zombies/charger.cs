

datablock fxDTSBrickData (BrickZombieCharger_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
	uiName = "Zombie Charger Hole";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_charger";

	holeBot = "ZombieChargerHoleBot";
};

datablock PlayerData(ZombieChargerHoleBot : CommonZombieHoleBot)
{
	uiName = "Charger Infected";
	minImpactSpeed = 25;
	airControl = 0.1;
	speedDamageScale = 0.2;
	ShapeNameDistance = 50;
	maxdamage = 200;//Health
	hName = "Charger";//cannot contain spaces
	hTickRate = 5000;
	hMeleeCI = "Charger";
	hAttackDamage = $Pref::Server::L4B2Bots::SpecialsDamage;

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

	hCustomNodeAppearance = 1;
	ShapeNameDistance = 100;
	hIsInfected = 2;
	hZombieL4BType = 5;
	hPinCI = "<bitmapk:Add-Ons/Package_Left4Block/icons/ci_charger2>";
	SpecialCPMessage = "Right click to charge <br>\c6Charge to pin non-infected";
	hBigMeleeSound = "charger_punch1_sound";

	rechargeRate = 0.75;
	maxenergy = 100;
	showEnergyBar = true;
};

function ZombieChargerHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);
	
	%obj.setscale("1.15 1.15 1.15");
}

function ZombieChargerHoleBot::onAdd(%this,%obj)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
}

function L4B_holeChargerKill(%obj,%col)
{
	if(L4B_SpecialsPinCheck(%obj,%col))
	{
		%obj.mountImage(HateImage, 3);
		%obj.setenergylevel(0);

		if(%obj.getclassname() $= "AIPlayer")
		{
			%obj.stopHoleLoop();
			%obj.hClearMovement();
		}
		
		%obj.schedule(10,playThread,1,plant);
		%obj.schedule(10,playThread,2,shiftup);
		%obj.schedule(100,playThread,2,shiftdown);
		%obj.schedule(100,playaudio,3,"charger_smash_sound");
		%col.schedule(100,playThread,2,plant);
		%col.schedule(100,damage,%obj.hFakeProjectile, %col.getposition(), $Pref::Server::L4B2Bots::SpecialsPinDamage, $DamageType::Charger);
		%obj.schedule(100,spawnExplosion,pushBroomProjectile,"0.5 0.5 0.5");
		%obj.hSharkEatDelay = schedule(2000,0,L4B_holeChargerKill,%obj,%col);
		%obj.playaudio(0,"charger_pummel" @ getrandom(1,4) @ "_sound");
		%obj.schedule(50,setcrouching,1);
		%obj.schedule(125,setcrouching,0);
	}

}

function L4B_Charging(%obj,%targ)
{
	if(isObject(%obj) && %obj.getState() !$= "Dead")
	{
		if(isObject(%obj.light))
		%obj.light.delete();

		%obj.WalkAfterCharge = %obj.schedule(4000,setMaxForwardSpeed,9);
		%obj.WalkAfterCharge = %obj.schedule(4000,playthread,1,"root");
		%obj.playaudio(0,"charger_charge" @ getrandom(1,2) @ "_sound");
		%obj.mountImage(HateImage, 3);
		%obj.setMaxForwardSpeed(50);
		%obj.setenergylevel(0);

		if(%obj.getClassName() $= "AIPlayer")
		{
			%obj.stopHoleLoop();
			%obj.StartAfterCharge = %obj.schedule(4000,startHoleLoop);
			%obj.setmoveY(100);

			if(isObject(%targ) && %targ.getState() !$= "Dead")
			%obj.setAimLocation(%targ.getEyePoint());

			%obj.schedule(100,clearaim);
		}
	}
}

function ZombieChargerHoleBot::onBotLoop(%this,%obj)
{
	%obj.hAttackDamage = $Pref::Server::L4B2Bots::SpecialsDamage;
	%obj.hLimitedLifetime();
	
	if(!%obj.hFollowing)
	{
		%obj.setMaxForwardSpeed(9);
		%obj.playaudio(0,"charger_lurk" @ getrandom(1,4) @ "_sound");
		%obj.playthread(3,plant);	
		%obj.playthread(1,"root");
		%obj.raisearms = 0;
		
	}
}

function ZombieChargerHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(%obj.lastsaw+8000 < getsimtime() && !%targ.getDatablock().isDowned && !%obj.isstrangling)
	{
		%obj.lastsaw = getsimtime();

		L4B_SpecialsWarningLight(%obj);
		%obj.AboutToCharge = schedule(1500,0,L4B_Charging,%obj,%targ);
	
		%obj.playthread(1,"armReadyright");
		%obj.playthread(3,"plant");
		%obj.playaudio(0,"charger_warn" @ getrandom(1,3) @ "_sound");
	}
	if(!isEventPending(%obj.AboutToCharge))
	%obj.playaudio(0,"charger_recognize" @ getrandom(1,4) @ "_sound");
}

function ZombieChargerHoleBot::onImpact(%this, %obj, %col, %vec, %force)
{
	if(%obj.getstate() $= "Dead")
	return;

	cancel(%obj.StartAfterCharge);
	cancel(%obj.WalkAfterCharge);

	if(%obj.getclassname() $= "AIPlayer")
	%obj.startHoleLoop();

	if(%force >= 20)
	{
		%forcecalc = %force/20;
		%oScale = 2*getWord(%obj.getScale(),0);

		if(%oScale >= 1.1)
		%obj.SpecialPinAttack(%col,%force);
		
		%obj.playaudio(3,"charger_smash_sound");
		%obj.setMaxForwardSpeed(9);
		%obj.spawnExplosion(pushBroomProjectile,%forcecalc SPC %forcecalc SPC %forcecalc);

		if(%force >= 50)
		{
			%obj.spawnExplosion(TankLandProjectile,%forcecalc SPC %forcecalc SPC %forcecalc);
			
			if(!%obj.hEating)
			{
				if(%obj.getclassname() $= "AIPlayer")
				%obj.stopHoleLoop();

				L4B_SpazzZombieInitialize(%obj,0);
			}
		}
	}
	Parent::onImpact(%this, %obj, %col, %vec, %force);
}

function ZombieChargerHoleBot::onBotMelee(%this,%obj,%col)
{
	%oscale = getWord(%obj.getScale(),2);

	if(%oScale >= 1.1)
	%obj.bigZombieMelee();	
}	

function ZombieChargerHoleBot::onDamage(%this,%obj,%source,%pos,%damage,%type)
{
	%obj.setShapeNameHealth();
	
	if(%obj.getstate() $= "Dead")
	return;

	if(%obj.lastdamage+1000 < getsimtime())//Check if the chest is the male variant and add a 1 second cooldown
	{
		%obj.playaudio(0,"charger_pain" @ getrandom(1,4) @ "_sound");
		%obj.lastdamage = getsimtime();
	}
	
	Parent::onDamage(%this,%obj,%source,%pos,%damage,%type);
}

function ZombieChargerHoleBot::onDisabled(%this,%obj)
{
	if(%obj.getstate() !$= "Dead")
	return;
	
	%obj.playaudio(0,"charger_die" @ getrandom(1,2) @ "_sound");

	if(isObject(%obj.hEating))
	{
		%obj.hEating.isBeingStrangled = 0;
		L4B_SpecialsPinCheck(%obj,%obj.hEating);
	}

	Parent::onDisabled(%this,%obj,%a);
}

function ZombieChargerHoleBot::L4BSpecialAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{	
	%hatColor = getRandomBotRGBColor();
	%packColor = getRandomBotRGBColor();
	%pack2Color = getRandomBotRGBColor();
	%accentColor = getRandomBotRGBColor();
	%pantsrandmultiplier = getrandom(2,8)*0.25;
	%pantsColorRand = getRandomBotRGBColor();
	%pantsColor = getWord(%pantsColorRand,0)*%pantsrandmultiplier SPC getWord(%pantsColorRand,1)*%pantsrandmultiplier SPC getWord(%pantsColorRand,2)*%pantsrandmultiplier SPC 1;
	%shoeColor = %pantsColor;

	%shirtColor = %skinColor;
	%larmColor = %shirtColor;
	%chargerhandColor = getWord(%skinColor,0)*0.5 SPC getWord(%skinColor,1)*0.5 SPC getWord(%skinColor,2)*0.5 SPC 1;
	%rarmColor = %chargerhandColor;
	%handColor = %skinColor;

	%rLegColor = getRandom(0,1);
	if(%rLegColor)
	%rLegColor = %shoeColor;
	else %rLegColor = %skinColor;
	%lLegColor = getRandom(0,1);
	if(%lLegColor)
	%lLegColor = %shoeColor;
	else %lLegColor = %skinColor;

	%obj.bloody["rhand"] = false;
	%obj.bloody["lhand"] = false;

	%obj.accentColor = %accentColor;
	%obj.accent =  0;
	%obj.hatColor = %hatColor;
	%obj.hat = %hat;
	%obj.headColor = %skinColor;
	%obj.faceName = %face;
	%obj.chest =  0;
	%obj.decalName = "worm_engineer";
	%obj.chestColor = %shirtColor;
	%obj.pack =  %pack;
	%obj.packColor =  %packColor;
	%obj.secondPack =  0;
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

function ZombieChargerHoleBot::hCustomNodeAppearance(%this,%obj)
{
	%obj.unhidenode(RarmCharger);
	%obj.unhidenode(Lhandsmall);
	%obj.unhidenode(Larmsmall);
	%obj.hidenode(Lhand);
	%obj.hidenode(Larm);
	%obj.hidenode(Rhand);

	if(%obj.getClassName() $= "Player")
	{
		%chargerhandColor = getWord(%obj.client.zombieColor,0)*0.5 SPC getWord(%obj.client.zombieColor,1)*0.5 SPC getWord(%obj.client.zombieColor,2)*0.5 SPC 1;
		%obj.setnodeColor(Rarm,%chargerhandColor);
		%obj.setnodeColor(RarmCharger,%chargerhandColor);

		%obj.setnodeColor(Lhandsmall,%obj.client.zombieColor);
		%obj.setnodeColor(Larmsmall,%obj.client.zombieColor);
	}
	else
	{
		%obj.setnodeColor(RarmCharger,%obj.rarmColor);
		%obj.setnodeColor(Lhandsmall,%obj.lhandColor);
		%obj.setnodeColor(Larmsmall,%obj.larmColor);
	}
}

function ZombieChargerHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{	
	if(%obj.getClassName() $= "Player" && %obj.getstate() !$= "Dead")
	{
		if(!%obj.hEating && !%obj.SpazzOff)
		{
			switch(%triggerNum)
			{
				case 0: %obj.playthread(1,"activate2");
				case 4: if(%obj.GetEnergyLevel() >= %this.maxenergy && %val)
						{
							if(!isEventPending(%obj.AboutToCharge))
							{
								%obj.playthread(1,"armReadyright");
								L4B_SpecialsWarningLight(%obj);
								%obj.playaudio(0,"charger_warn" @ getrandom(1,3) @ "_sound");
								%obj.setMaxForwardSpeed(9);

								%obj.AboutToCharge = schedule(1500,0,L4B_Charging,%obj,%targ);
							}
						}	
				default:
			}
		}
	}
	Parent::onTrigger (%this, %obj, %triggerNum, %val);
}