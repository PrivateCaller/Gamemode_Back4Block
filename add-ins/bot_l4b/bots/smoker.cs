datablock fxDTSBrickData (BrickZombieSmoker_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
	uiName = "Zombie Smoker Hole";
	iconName = "Add-Ons/Gamemode_Left4Block/add-ins/bot_l4b/icons/icon_smoker";

	holeBot = "ZombieSmokerHoleBot";
};

datablock PlayerData(ZombieSmokerHoleBot : CommonZombieHoleBot)
{
	uiName = "Smoker Infected";
	minImpactSpeed = 24;
	speedDamageScale = 2;

	maxdamage = 100;//Health

	hName = "Smoker";//cannot contain spaces
	hAttackDamage = $Pref::Server::L4B2Bots::SpecialsDamage;
	hTickRate = 5000;

    maxForwardSpeed = 8;
    maxBackwardSpeed = 7;
    maxSideSpeed = 6;

 	maxForwardCrouchSpeed = 6;
    maxBackwardCrouchSpeed = 5;
    maxSideCrouchSpeed = 4;

	cameramaxdist = 4;
    cameraVerticalOffset = 0.9;
    cameraHorizontalOffset = 0.6;
    cameratilt = 0.1;
    maxfreelookangle = 2;

	ShapeNameDistance = 100;
	hIsInfected = 2;
	hZombieL4BType = 5;
	hCustomNodeAppearance = 1;
	hPinCI = "<bitmapk:Add-Ons/Gamemode_Left4Block/add-ins/bot_l4b/icons/ci_smoker2>";
	SpecialCPMessage = "Right click to shoot your tongue <br>\c6Pin non-infected by hitting them with your tongue";
	hBigMeleeSound = "";
	hNeedsWeapons = 1;

	rechargeRate = 0.75;
	maxenergy = 100;
	showEnergyBar = true;
};

function ZombieSmokerHoleBot::onAdd(%this,%obj,%style)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
}

function ZombieSmokerHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);

	%obj.mountImage(SmokeStatusPlayerImage, 1);
	%obj.setscale("0.95 1 1.25");
}

function ZombieSmokerHoleBot::onBotLoop(%this,%obj)
{
	%obj.hAttackDamage = $Pref::Server::L4B2Bots::SpecialsDamage;
	%obj.hLimitedLifetime();

	if(!%obj.hFollowing && %obj.lastidle+5000 < getsimtime() && %obj.getstate() !$= "Dead" && !%obj.isstrangling)
	{
		switch(getRandom(0,1))
		{
			case 0: %obj.playaudio(0,"smoker_lurk" @ getrandom(1,4) @ "_sound");
			case 1: %obj.playaudio(0,"smoker_gasp" @ getrandom(1,5) @ "_sound");
		}
		%obj.playthread(3,plant);
		%obj.lastidle = getsimtime();
	}
}

function ZombieSmokerHoleBot::onDisabled(%this,%obj)
{
	parent::onDisabled(%this,%obj);

	if(isObject(%obj.light))
	%obj.light.delete();

	if(isObject(getMiniGameFromObject(%obj)))
	{
		if(isObject(%obj.Prey))
		{
			%targ = %obj.Prey;
			%targ.isBeingStrangled = 0;
		
			%obj.Prey = 0;
		}
	}

	%p = new projectile()
	{
		datablock = SmokerSporeProjectile;
		initialPosition = %obj.getposition();
		client = %obj.client;
		sourceObject = %obj;
	};
	%p.schedule(1000,L4B_SmokerCoughEffect,0);

	%obj.unMountImage(0);
	%obj.playaudio(0,"smoker_death" @ getrandom(1,2) @ "_sound");
}

function ZombieSmokerHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this,%obj,%col);
}

function ZombieSmokerHoleBot::onDamage(%this,%obj)
{
	
	if(%obj.getstate() !$= "Dead" && %obj.lastdamage+500 < getsimtime())//Check if the chest is the female variant and add a 1 second cooldown
	{
		%obj.playaudio(0,"smoker_pain" @ getrandom(1,4) @ "_sound");
		%obj.playthread(2,"plant");

		%obj.lastdamage = getsimtime();
	}

	Parent::OnDamage(%this,%obj);
}

function ZombieSmokerHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(%obj.getState() $= "Dead")
	return;

	%obj.setaimobject(%targ);

	if(!isEventPending(%obj.ShootTongue) && !%obj.isstrangling)
	%obj.playaudio(0,"smoker_recognize" @ getrandom(1,4) @ "_sound");

	if(vectorDist(%obj.getposition(),%targ.getposition()) > 12)
	{
		if(%obj.lastsaw+8000 < getsimtime() && getWord(%obj.getScale(),2) >= 1.05)
		{
			%obj.stopHoleLoop();
			%obj.hClearMovement();

			%obj.playaudio(0,"smoker_warn" @ getrandom(1,3) @ "_sound");
			%obj.ShootTongue = %obj.getDatablock().schedule(1500,ShootTongue,%obj);
			%obj.setaimobject(%targ);

			%obj.lastsaw = getsimtime();
		}
	}
	else if(%obj.getClassName() $= "AIPlayer")
	%obj.hRunAwayFromPlayer(%targ);
}

function ZombieSmokerHoleBot::L4BSpecialAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{	
	%pack2 = 0;
	%accent = 0;
	%hatColor = getRandomBotColor();
	%packColor = getRandomBotColor();
	%shirtColor = getRandomBotColor();
	%accentColor = getRandomBotColor();
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

	%obj.llegColor =  %llegColor;
	%obj.secondPackColor =  "0 0.435 0.831 1";
	%obj.lhand =  "0";
	%obj.hip =  "0";
	%obj.faceName =  %face;
	%obj.rarmColor =  %rarmColor;
	%obj.hatColor =  %hatcolor;
	%obj.hipColor =  %pantsColor;
	%obj.chest =  "0";
	%obj.rarm =  "0";
	%obj.packColor =  "1 1 1 1";
	%obj.pack =  %pack;
	%obj.decalName =  %decal;
	%obj.larmColor =  %larmColor;
	%obj.secondPack =  "0";
	%obj.larm =  "0";
	%obj.chestColor =  %shirtColor;
	%obj.accentColor =  "1 1 1 1";
	%obj.rhandColor =  %skinColor;
	%obj.rleg =  "0";
	%obj.rlegColor =  %rlegColor;
	%obj.accent =  "1";
	%obj.headColor =  %skinColor;
	%obj.rhand =  "0";
	%obj.lleg =  "0";
	%obj.lhandColor =  %skinColor;
	%obj.hat =  %hat;

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function ZombieSmokerHoleBot::hCustomNodeAppearance(%this,%obj)
{
	if(%obj.getClassName() $= "AIPlayer")
	%smokerboilcolor = getWord(%obj.headColor,0)*0.75 SPC getWord(%obj.headColor,1)*0.75 SPC getWord(%obj.headColor,2)*0.75 SPC 1;
	else %smokerboilcolor = getWord(%obj.client.zombieColor,0)*0.75 SPC getWord(%obj.client.zombieColor,1)*0.75 SPC getWord(%obj.client.zombieColor,2)*0.75 SPC 1;

	%obj.unhidenode(smokerboil);
	%obj.setnodeColor(smokerboil,%smokerboilcolor);
	%obj.unhidenode(smokertongue);
	%obj.setnodeColor(smokertongue,"1 0 0 1");
}

function ZombieSmokerHoleBot::ShootTongue(%this, %obj)
{
	if(!isObject(%obj) || %obj.getstate() $= "Dead")
	return;

	if(isObject(%obj.light))
	%obj.light.delete();
	
	%obj.setenergylevel(0);
	if(isObject(%obj.GHRope))
	%obj.GHRope.delete();

	%obj.playthread(3,"Plant");
	%obj.playthread(2,"Plant");
	%obj.playaudio(1,"smoker_launch_tongue_sound");

	%p = new Projectile()
	{
		dataBlock = "SmokerTongueProjectile";
		initialVelocity = vectorScale(%obj.getEyeVector(),125);
		initialPosition = vectorAdd(%obj.getMuzzlePoint(2),"0 0 0.35");
		sourceObject = %obj;
		client = %obj.client;
	};
	MissionCleanup.add(%p);

	%tongue = new StaticShape()
	{
		dataBlock = SmokerTongueShape;
		source = %obj;
		end = %p;
	};	
	%obj.tongue = %tongue;
	%p.tongue = %tongue;
}

function SmokerTongueShape::onAdd(%this,%obj)
{
	Parent::onAdd(%this,%obj);
	%obj.setNodeColor("ALL","1 0 0 1");
	%this.onTongueLoop(%obj);
	MissionCleanup.add(%obj);
}

function ZombieSmokerHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{	
	if(%obj.getClassName() $= "Player" && %obj.getstate() !$= "Dead")
	{
		switch(%triggerNum)
		{
			case 0: CommonZombieHoleBot::onTrigger(%this, %obj, %triggerNum, %val);
			case 4: if(%obj.GetEnergyLevel() >= %this.maxenergy && !%obj.isStrangling)
					{
						if(!isEventPending(%obj.ShootTongue))
						{
							%obj.ShootTongue = %obj.getDatablock().schedule(1500,ShootTongue,%obj);
							%obj.playaudio(0,"smoker_warn" @ getrandom(1,3) @ "_sound");
						}
					}
			default:
		}
	}
	Parent::onTrigger (%this, %obj, %triggerNum, %val);
}

function SmokerTongueShape::onTongueLoop(%this,%obj)
{		
	%smoker = %obj.source;
	%end = %obj.end;
	
	if(!isObject(%obj) || !isObject(%smoker) || %smoker.getState() $= "Dead" || !isObject(%end) || isObject(%end) && (%end.getClassName() $= "Player" || %end.getClassName() $= "AIPlayer") && !L4B_SpecialsPinCheck(%smoker,%end))//In case one of these are false, return the function and delete the shape
	{
		if(isObject(%obj))
		{
			if(isObject(%smoker))//Just in case the smoker is still around
			{
				%smoker.playaudio(3,"smoker_tongue_reel_sound");

				if(%smoker.getstate() !$= "Dead" && %smoker.getclassname() $= "AIPlayer")
				{
					%smoker.startHoleLoop();
					%smoker.hRunAwayFromPlayer(%smoker);
				}
			}

			if(isObject(%end) && (%end.getClassName() $= "Player" || %end.getClassName() $= "AIPlayer"))
			{
				if(%end.getState() !$= "Dead")
				%end.unMountImage(2);
				else
				{
					serverPlay3D("victim_smoked_sound",%end.getHackPosition());
					%end.unMountImage(2);
				}
			}

			%obj.delete();
			return;
		}
	}

	switch$(%end.getClassName())
	{
		case "Projectile": %endpos = %end.getPosition();
							if(%smoker.getclassname() $= "AIPlayer")
							%smoker.setaimlocation(%endpos);
		case "Player": %endpos = %end.getHackPosition();

						if(%smoker.getclassname() $= "AIPlayer")
						%smoker.setaimlocation(%endpos);

						if(getWord(%end.getVelocity(), 2) < 3 && vectordist(%smoker.getposition(),%end.getposition()) >= 1)
						{
							if(%end.lastchokecough+getrandom(500,750) < getsimtime())
							{
								%end.playaudio(0,"norm_cough" @ getrandom(1,3) @ "_sound");
								%end.playthread(2,"plant");
								%end.lastchokecough = getsimtime();
								%end.damage(%smoker.hFakeProjectile, %end.getposition(), 3, $DamageType::SmokerConstrict);
							}
						}

						%DisSub = vectorSub(%end.getPosition(),%smoker.getposition());
						%DistanceNormal = vectorNormalize(%DisSub);

						if(L4B_IsOnGround(%end))
						%force = 10;
						else %force = 5;

						%end.setVelocity(vectorscale(%DistanceNormal,-%force));

		case "AIPlayer": %endpos = %end.getHackPosition();

						if(%smoker.getclassname() $= "AIPlayer")
						%smoker.setaimlocation(%endpos);

						 if(getWord(%end.getVelocity(), 2) < 3 && vectordist(%smoker.getposition(),%end.getposition()) >= 1)
						 {
						 	if(%end.lastchokecough+getrandom(500,750) < getsimtime())
						 	{
						 		%end.playaudio(0,"norm_cough" @ getrandom(1,3) @ "_sound");
						 		%end.playthread(2,"plant");
						 		%end.lastchokecough = getsimtime();
						 		%end.damage(%smoker.hFakeProjectile, %end.getposition(), 3, $DamageType::SmokerConstrict);
						 	}
						 }
 
						 %DisSub = vectorSub(%end.getPosition(),%smoker.getposition());
						 %DistanceNormal = vectorNormalize(%DisSub);
 
						 if(L4B_IsOnGround(%end))
						 %force = 10;
						 else %force = 5;
 
						 %end.setVelocity(vectorscale(%DistanceNormal,-%force));
	}

	//Calculate the position and scale between the smoker and victim
	%head = %smoker.getmuzzlePoint(2);
	%vector = vectorNormalize(vectorSub(%endpos,%head));
	%relative = "0 1 0";
	%xyz = vectorNormalize(vectorCross(%relative,%vector));
	%u = mACos(vectorDot(%relative,%vector)) * -1;
	%obj.setTransform(vectorScale(vectorAdd(vectorAdd(%head,"0 0 0.5"),%endpos),0.5) SPC %xyz SPC %u);
	%obj.setScale(0.2 SPC vectorDist(%head,%endpos) * 2 SPC 0.2);

	%obj.TongueLoop = %this.schedule(25,onTongueLoop,%obj);
}

datablock ProjectileData(SmokerTongueProjectile)
{
	projectileShapeName = "base/data/shapes/empty.dts";
	directDamage        = 0;
	directDamageType    = $DamageType::Direct;
	particleEmitter     = "";
	explosion           = "";

	muzzleVelocity      = 100;
	velInheritFactor    = 0;

	armingDelay         = 0;
	lifetime            = 1000;
	fadeDelay           = 750;
	gravityMod 			= 1;
	isBallistic         = true;
	explodeOnDeath = false;

	uiName = "";
};

function SmokerTongueProjectile::onCollision(%this,%proj,%col,%fade,%pos,%normal)
{
	%obj = %proj.sourceObject;
	serverPlay3D("smoker_tongue_hit_sound",%pos);

	cancel(%obj.SmokerTongueReturn);
	if(%col.getType() & $Typemasks::PlayerObjectType && L4B_SpecialsPinCheck(%obj,%col))
	{
		%col.dismount();
		%obj.tongue.end = %col;
		%obj.SpecialPinAttack(%col);
	}

	Parent::onCollision(%this,%proj,%col,%fade,%pos,%normal);
}

function Projectile::SmokerExplosionCoughEffect(%obj)
{
	if(!isObject(%obj))
	return;

	%pos = %obj.getPosition();
	%radius = 5;
	%searchMasks = $TypeMasks::PlayerObjectType;
	InitContainerRadiusSearch(%pos, %radius, %searchMasks);
	while((%target = containerSearchNext()) != 0 )
	{
		if(%target.hZombieL4BType && %target.getstate() !$= "Dead")
		{
			%randomtime = getRandom(400,800);
			%target.schedule(%randomtime,playaudio,2,"norm_cough" @ getrandom(1,3) @ "_sound");
			%target.schedule(%randomtime,playthread,3,"plant");
		}
	}
	%obj.schedule(750,L4B_SmokerCoughEffect,%obj);
}