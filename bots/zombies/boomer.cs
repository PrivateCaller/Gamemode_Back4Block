
datablock fxDTSBrickData (BrickZombieBoomer_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
{
	uiName = "Zombie Boomer Hole";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_boomer";
	holeBot = "ZombieBoomerHoleBot";
};

datablock PlayerData(ZombieBoomerHoleBot : CommonZombieHoleBot)
{
	uiName = "Boomer Infected";
	jumpForce = 9*350;
	minImpactSpeed = 16;
	airControl = 0.01;
	speedDamageScale = 10;
	mass = 500;

	cameramaxdist = 2;
    cameraVerticalOffset = 1.1;
    cameraHorizontalOffset = 0.6;
    cameratilt = 0.1;
    maxfreelookangle = 2;

    maxForwardSpeed = 6;
    maxBackwardSpeed = 4;
    maxSideSpeed = 5;

 	maxForwardCrouchSpeed = 6;
    maxBackwardCrouchSpeed = 4;
    maxSideCrouchSpeed = 5;

	ShapeNameDistance = 100;
	hIsInfected = 1;
	hZombieL4BType = 5;
	hCustomNodeAppearance = 1;
	hPinCI = "<bitmapk:Add-Ons/Package_Left4Block/icons/ci_boomer2>";
	SpecialCPMessage = "Right click to vomit";
	hBigMeleeSound = "";
	hNeedsWeapons = 1;

	maxdamage = 150;//Thicc health
	hTickRate = 5000;

	hShoot = 1;
	hMaxShootRange = 2.5;//The range in which the bot will shoot the player
	hMoveSlowdown = 1;

	hName = "Boomer";//cannot contain spaces
	hAttackDamage = $Pref::Server::L4B2Bots::SpecialsDamage;

	rechargeRate = 1.25;
	maxenergy = 100;
	showEnergyBar = true;
};

function ZombieBoomerHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);

	%obj.setscale("1.6 2 1.1");
}

function ZombieBoomerHoleBot::onAdd(%this,%obj)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
}

function ZombieBoomerHoleBot::onBotMelee(%this,%obj,%col)
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
			datablock = BoomerVomitSpewedProjectile;
			initialPosition = %obj.getHackPosition();
			initialVelocity = %x SPC %y SPC (getRandom()*4);
			client = %obj.client;
			sourceObject = %obj;
			damageType = $DamageType::SpitAcidBall;
		};
	}
	%obj.playaudio(0,"boomer_vomit" @ getrandom(1,4) @ "_sound");
	%obj.playthread(3,"Plant");
}

	function ZombieBoomerHoleBot::onBotLoop(%this,%obj)
{
	%obj.hAttackDamage = $Pref::Server::L4B2Bots::SpecialsDamage;
	%obj.hLimitedLifetime();

	%obj.playthread(3,plant);
	if(!%obj.hFollowing)
	%obj.playaudio(0,"boomer_lurk" @ getrandom(1,4) @ "_sound");
	else %obj.playaudio(0,"boomer_recognize" @ getrandom(1,4) @ "_sound");
}

function ZombieBoomerHoleBot::onBotFollow( %this, %obj, %targ )
{
	Parent::onBotFollow( %this, %obj, %targ );

	%obj.setaimobject(%targ);
	
	if(vectorDist(%obj.getposition(),%targ.getposition()) < 8)
	{
		%obj.getDatablock().Vomit(%obj);

		for (%n = 0; %n < 4; %n++)
		%obj.getDatablock().schedule(750 * %n,Vomit,%obj);
	}
}

    function ZombieBoomerHoleBot::onDamage(%this,%obj,%Am)
{
	%obj.setShapeNameHealth();
	
	if(%obj.getstate() $= "Dead")
	return;

	if(%obj.lastdamage+500 < getsimtime())
	{
		%obj.lastdamage = getsimtime();
		%obj.playthread(3,plant);
		%obj.playaudio(0,"boomer_pain" @ getrandom(1,4) @ "_sound");
	}

	if(%Obj.GetDamageLevel() > %obj.getDatablock().maxDamage/2)
	{
		L4B_SpecialsWarningLight(%obj);
		%obj.playaudio(3,"boomer_indigestion_loop_sound");
	}
	
	Parent::onDamage(%this,%obj,%Am);
}

	function ZombieBoomerHoleBot::onDisabled(%this, %obj)
{
	if(%obj.getstate() !$= "Dead")
	return Parent::onDisabled(%this,%obj);

	%obj.hideNode("ALL");
	for (%n = 0; %n < 4; %n++)
	{
		%obj.unMountImage(%n);
		%obj.playaudio(%n,"blank_sound");
	}

	if(isObject(%obj.light))
		%obj.light.delete();

	%b = new projectile()
	{
		datablock = goreModProjectile;
		initialPosition = %obj.getPosition();
		client = %obj.client;
		sourceObject = %obj;
		damageType = $DamageType::Boomer;
	};
	%c = new projectile()
	{
		datablock = BoomerProjectile;
		initialPosition = %obj.getPosition();
		client = %obj.client;
		sourceObject = %obj;
		damageType = $DamageType::Boomer;
	};

	if(isObject(%mg = getMinigameFromObject(%obj)) && %mg.BotDamage)
	for(%i=0;%i<25;%i++)
	{
		%rnd = getRandom();
		%dist = getRandom()*20;
		%x = mCos(%rnd*$PI*5)*%dist;
		%y = mSin(%rnd*$PI*5)*%dist;
		%p = new projectile()
		{
			datablock = BoomerVomitSpewedProjectile;
			initialPosition = %obj.getHackPosition();
			initialVelocity = %x SPC %y SPC (getRandom()*4);
			client = %obj.client;
			sourceObject = %obj;
			damageType = $DamageType::SpitAcidBall;
		};
	}

	Parent::onDisabled(%this,%obj);
}

//Boomer Functions
function BoomerProjectile::radiusDamage(%this, %obj, %col, %distanceFactor, %pos, %damageAmt)
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
      %damageType = $DamageType::Boomer;
      if(%col.getType() & $TypeMasks::PlayerObjectType)
      {
            %col.damage(%obj, %pos, %damageAmt/2, %damageType);

			if(%col.getState() !$= "Dead" && L4B_CheckifinMinigame(%obj.sourceObject, %col) && !%col.BoomerBiled) 
			{
			
			if(%col.getClassName() $= "AIPlayer" && %col.hType $= "Zombie")
			{
				%col.hType = "biled" @ getRandom(1,9999);
				%col.mountImage(BileStatusPlayerImage, 2);
				%col.BoomerBiled = 1;
			}

			if(%col.getClassName() $= "Player")
			{
				if($Pref::Server::L4B2Bots::MinigameMessages && %col.getState() !$= "Dead" && %obj.sourceObject.getDataBlock().hName)
				{
					chatMessageTeam(%col.client,'fakedeathmessage',"<color:FFFF00>" @ %obj.sourceObject.getDatablock().hName SPC "<bitmapk:Add-Ons/Package_Left4Block/icons/ci_boomer2>" SPC %col.client.name);
					MinigameSO::L4B_PlaySound(%col.client.minigame,"victim_needshelp_sound");
				}

				%col.setWhiteout(2);
				%col.mountImage(BileStatusPlayerImage, 2);	
				schedule(15000,0,L4B_BiledClear,%col,%this.sourceObject);
				%col.BoomerBiled = 1;

				L4B_ZombieMinionsAttack(%col);
			}
         }
      }
   }
}

function L4B_BiledClear(%targetid,%obj)
{
	if(isObject(%targetid) && %targetid.getState !$= "Dead" && %targetid.BoomerBiled)
	{
		%targetid.BoomerBiled = 0;
		%targetid.unMountImage(2);

		%targetid.setMaxForwardSpeed(%targetid.Datablock.maxForwardSpeed);

		%word = strLwr(getSubStr(%targetid.hType, 0, 5));
		if(%word $= "biled")
			%targetid.hType = "Zombie";
	}
}


function L4B_ZombieMinionsAttack(%targetid,%count)
{
	if(!%targetid.BoomerBiled || !isObject(%targetid) || %targetid.getclassname() $= "AIPlayer")
	return;
	
		%pos = %targetid.getPosition();
		%radius = 250;
		%searchMasks = $TypeMasks::PlayerObjectType;
		InitContainerRadiusSearch(%pos, %radius, %searchMasks);

		while((%targetzombie = containerSearchNext()) != 0 )
		{
			%word = strLwr(getSubStr(%targetzombie.hType, 0, 5));

			if(%targetzombie.getClassName() $= "AIPlayer" && %targetzombie.hType $= "Zombie" || %word $= "biled" && %targetid.getstate() !$= "Dead" && %targetzombie.hcanDistract >= 1 && !%targetzombie.isBurning)
			{
				if(%count < 14)
				{
					if(!%targetzombie.Distraction)
					{
						%targetzombie.hSearch = 0;
						%targetzombie.Distraction = %targetid.getID();
						%targetid.hReturnToSpawn = 0;
						%targetzombie.hClearMovement();
						%targetzombie.spawnExplosion(alarmProjectile,"1 1 1");
					}
					else if(%targetzombie.Distraction $= %targetid.getID())
					{
						%targetzombie.setmoveobject(%targetid);
						%targetzombie.setaimobject(%targetid);
					}
				}
				else
				{	
					%targetzombie.hSearch = 1;
					%targetzombie.Distraction = 0;
					%targetid.hReturnToSpawn = 0;
				}
			}
		}
		schedule(1000,0,L4B_ZombieMinionsAttack,%targetid,%count+1);
}

function ZombieBoomerHoleBot::L4BSpecialAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{
	%hatColor = getRandomBotRGBColor();
	%packColor = getRandomBotRGBColor();
	%shirtColor = getRandomBotRGBColor();
	%accentColor = getRandomBotRGBColor();
	%pantsColor = getRandomBotPantsColor();
	%shoeColor = getRandomBotPantsColor();
	%handColor = %skinColor;
				
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

	%obj.llegColor =  %lLegColor;
	%obj.secondPackColor =  "0 0.435 0.831 1";
	%obj.lhand =  "0";
	%obj.hip =  "0";
	%obj.faceName =  %face;
	%obj.rarmColor =  %rarmColor;
	%obj.hatColor =  %hatColor;
	%obj.hipColor =  %pantsColor;
	%obj.chest =  "0";
	%obj.rarm =  "0";
	%obj.packColor =  "0.2 0 0.8 1";
	%obj.pack =  %pack;
	%obj.decalName =  %decal;
	%obj.larmColor =  %larmColor;
	%obj.secondPack =  "0";
	%obj.larm =  "0";
	%obj.chestColor =  %shirtColor;
	%obj.accentColor =  "0.990 0.960 0 0.700";
	%obj.rhandColor =  %skinColor;
	%obj.rleg =  "0";
	%obj.rlegColor =  %rLegColor;
	%obj.accent =  "1";
	%obj.headColor =  %skinColor;
	%obj.rhand =  "0";
	%obj.lleg =  "0";
	%obj.lhandColor =  %skinColor;
	%obj.hat =  0;

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function ZombieBoomerHoleBot::hCustomNodeAppearance(%this,%obj)
{
	if(%obj.getClassName() $= "Player")
	{
		%chestcolor = %obj.client.chestColor;
		%headColor = %obj.client.zombieColor;
		%hipColor = %obj.client.hipColor;		
	}
	else
	{
		%chestcolor = %obj.chestColor;
		%headColor = %obj.headColor;
		%hipColor = %obj.hipColor;		
	}

	%boomerboilneckcolor = getWord(%headColor,0)*0.5 SPC getWord(%headColor,1)*0.5 SPC getWord(%headColor,2)*0.5 SPC 1;
	%boomerboilchestcolor = getWord(%headColor,0)*0.5 SPC getWord(%headColor,1)*0.5 SPC getWord(%headColor,2)*0.5 SPC 1;

	%obj.hidenode(chest);
	%obj.hidenode(pants);
	
	%obj.unhidenode(boomerchest);
	%obj.setnodeColor(boomerchest,%chestcolor);
	%obj.unhidenode(boomerboilchest);
	%obj.setnodeColor(boomerboilchest,%boomerboilchestcolor);
	%obj.unhidenode(boomercheststomach);
	%obj.setnodeColor(boomercheststomach,%headColor);	
	%obj.unhidenode(boomerpants);
	%obj.setnodeColor(boomerpants,%hipColor);	
	%obj.unhidenode(boomerboilneck);
	%obj.setnodeColor(boomerboilneck,%boomerboilneckcolor);
}


function BoomerVomitSpewedProjectile::onExplode(%obj,%this)
{
	Parent::onExplode(%obj,%this);

	%pos = %this.getPosition();
    %radius = 1;
    %searchMasks = $TypeMasks::PlayerObjectType;
    InitContainerRadiusSearch(%pos, %radius, %searchMasks);
    while ((%targetid = containerSearchNext()) != 0)
    {
        if(%targetid.getType() & $TypeMasks::PlayerObjectType)
        {
			if(isObject(getMinigameFromObject(%targetid,%this.sourceObject)) && checkHoleBotTeams(%this.sourceObject,%targetid) && miniGameCanDamage(%this.sourceObject,%targetid))
			{
				%targetid.setWhiteout(2);

				if(%targetid.BoomerBiled)
				return Parent::onExplode(%obj,%this);
				else
				{
					if($Pref::Server::L4B2Bots::MinigameMessages && isObject(%targetid.client))
					{
						chatMessageTeam(%targetid.client,'fakedeathmessage',"<color:FFFF00>" @ %this.sourceObject.getDatablock().hName SPC "<bitmapk:Add-Ons/Package_Left4Block/icons/ci_boomer2>" SPC %targetid.client.name);
						MinigameSO::L4B_PlaySound(%targetid.client.minigame,"victim_needshelp_sound");
					}

					%targetid.mountImage(BileStatusPlayerImage, 2);
					schedule(15000,0,L4B_BiledClear,%targetid,%obj);	
					L4B_ZombieMinionsAttack(%targetid);
					%targetid.BoomerBiled = 1;
				}
			}
        }
    }
}
//Boomer Vomit Weapon
function BoomerVomitProjectile::onExplode(%obj,%this)
{
	for(%i=0;%i<15;%i++)
	{
		%rnd = getRandom();
		%dist = getRandom()*20;
		%x = mCos(%rnd*$PI*3)*%dist;
		%y = mSin(%rnd*$PI*3)*%dist;
		%p = new projectile()
		{
			datablock = BoomerVomitSpewedProjectile;
			initialPosition = %this.getPosition();
			initialVelocity = %x SPC %y SPC (getRandom()*4);
			client = %this.sourceObject.client;
			sourceObject = %this.sourceObject;
			damageType = $DamageType::SpitAcidBall;
		};
	}
	BoomerVomitSpewedProjectile::onExplode(%obj,%this);
}

function ZombieBoomerHoleBot::Vomit(%this, %obj)
{
	if(!isObject(%obj) || %obj.getstate() $= "Dead")
	return;

	if(isObject(%obj.light))
	%obj.light.delete();
	
	%obj.setenergylevel(0);
	%obj.playaudio(0,"boomer_vomit" @ getrandom(1,4) @ "_sound");
	%obj.playthread(0,"jump");
	%obj.playthread(1,"jump");
	%obj.playthread(2,"activate2");

	%muzzle = vectorAdd(%obj.getMuzzlePoint(2),"0 0 0.35");
	%velocity = vectorScale(%obj.getEyeVector(),20);
	%p = new Projectile()
	{
		dataBlock = "BoomerVomitProjectile";
		initialVelocity = %velocity;
		initialPosition = %muzzle;
		sourceObject = %obj;
		client = %obj.client;
	};
	MissionCleanup.add(%p);
}

function ZombieBoomerHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{	
	CommonZombieHoleBot::onTrigger (%this, %obj, %triggerNum, %val);
	if(%obj.getClassName() $= "Player" && %obj.getstate() !$= "Dead")
	{
		switch(%triggerNum)
		{
			case 4: if(%val && %obj.GetEnergyLevel() >= %this.maxenergy)
					%obj.getDatablock().Vomit(%obj);
		}
	}

	Parent::onTrigger (%this, %obj, %triggerNum, %val);
}