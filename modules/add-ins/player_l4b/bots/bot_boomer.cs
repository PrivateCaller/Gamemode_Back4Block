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
	%obj.hAttackDamage = $Pref::L4B::Zombies::SpecialsDamage;
	%obj.hNoSeeIdleTeleport();

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

		for (%n = 0; %n < 1; %n++)
		%obj.getDatablock().schedule(750 * %n,Vomit,%obj);
	}
}

function ZombieBoomerHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	%limb = %obj.rgetDamageLocation(%position);
	if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact)
	if(%limb) %damage = %damage/6;
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function ZombieBoomerHoleBot::onDamage(%this,%obj,%Am)
{	
	if(%obj.getstate() $= "Dead") return;

	if(%obj.lastdamage+500 < getsimtime())
	{
		%obj.lastdamage = getsimtime();
		%obj.playthread(3,plant);
		%obj.playaudio(0,"boomer_pain" @ getrandom(1,4) @ "_sound");
	}

	if(%Obj.GetDamageLevel() > %obj.getDatablock().maxDamage/2)
	%obj.playaudio(3,"boomer_indigestion_loop_sound");
	
	Parent::onDamage(%this,%obj,%Am);
}

	function ZombieBoomerHoleBot::onDisabled(%this, %obj)
{
	if(%obj.getstate() !$= "Dead") return;

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

	%obj.hideNode("ALL");
	%obj.unhideNode("pants");
	%obj.unhideNode("RShoe");
	%obj.unhideNode("LShoe");

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

			if(%col.getState() !$= "Dead" && miniGameCanDamage(%obj.sourceObject, %col) && !%col.BoomerBiled) 
			{
			
			if(%col.getClassName() $= "AIPlayer" && %col.hType $= "Zombie")
			{
				%col.hType = "biled" @ getRandom(1,9999);
				%col.mountImage(BileStatusPlayerImage, 2);
				%col.BoomerBiled = 1;
			}

			if(%col.getClassName() $= "Player")
			{
				if(%col.getState() !$= "Dead" && %obj.sourceObject.getDataBlock().hName)
				{
					chatMessageTeam(%col.client,'fakedeathmessage',"<color:FFFF00>" @ %obj.sourceObject.getDatablock().hName SPC "<bitmapk:Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_boomer2>" SPC %col.client.name);
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

function ZombieBoomerHoleBot::holeAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{
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

function ZombieBoomerHoleBot::L4BAppearance(%this,%obj,%client)
{
	%obj.hideNode("ALL");
	%obj.unHideNode("boomerchest");
	%obj.unHideNode("boomercheststomach");	
	%obj.unHideNode("rhand");
	%obj.unHideNode("lhand");
	%obj.unHideNode(("rarm"));
	%obj.unHideNode(("larm"));
	%obj.unHideNode("headskin");
	%obj.unHideNode("pants");
	%obj.unHideNode("rshoe");
	%obj.unHideNode("lshoe");
	%obj.unhidenode("gloweyes");
	%obj.setHeadUp(0);

	%headColor = %client.headcolor;
	%chestColor = %client.chestColor;
	%rarmcolor = %client.rarmColor;
	%larmcolor = %client.larmColor;
	%rhandcolor = %client.rhandColor;
	%lhandcolor = %client.lhandColor;
	%hipcolor = %client.hipColor;
	%rlegcolor = %client.rlegColor;
	%llegColor = %client.llegColor;	

	if(%obj.getDatablock().hType $= "Zombie" && %obj.getclassname() $= "Player")
	{
		%skin = %client.headColor;
		%zskin = getWord(%skin,0)/2.75 SPC getWord(%skin,1)/1.5 SPC getWord(%skin,2)/2.75 SPC 1;			

		%headColor = %zskin;
		if(%client.chestColor $= %skin) %chestColor = %zskin;
		if(%client.rArmColor $= %skin) %rarmcolor = %zskin;
		if(%client.lArmColor $= %skin) %larmcolor = %zskin;
		if(%client.rhandColor $= %skin) %rhandcolor = %zskin;
		if(%client.lhandColor $= %skin) %lhandcolor = %zskin;
		if(%client.hipColor $= %skin) %hipcolor = %zskin;
		if(%client.rLegColor $= %skin) %rlegcolor = %zskin;
		if(%client.lLegColor $= %skin) %llegColor = %zskin;
	}		

	%obj.setnodeColor("gloweyes","1 1 0 1");
	%obj.setFaceName("asciiTerror");
	%obj.setDecalName(%client.decalName);
	%obj.setNodeColor("headskin",%headColor);
	%obj.setNodeColor("boomerchest",%chestColor);
	%obj.setNodeColor("boomercheststomach",%headColor);
	%obj.setNodeColor("pants",%hipColor);
	%obj.setNodeColor("rarm",%rarmColor);
	%obj.setNodeColor("larm",%larmColor);
	%obj.setNodeColor("rhand",%rhandColor);
	%obj.setNodeColor("lhand",%lhandColor);
	%obj.setNodeColor("rshoe",%rlegColor);
	%obj.setNodeColor("lshoe",%llegColor);
	%obj.setNodeColor("pants",%hipColor);	
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
			if(checkHoleBotTeams(%this.sourceObject,%targetid) && miniGameCanDamage(%this.sourceObject,%targetid))
			{
				%targetid.setWhiteout(2);

				if(%targetid.BoomerBiled)
				return Parent::onExplode(%obj,%this);
				else
				{
					if(isObject(%targetid.client))
					{
						chatMessageTeam(%targetid.client,'fakedeathmessage',"<color:FFFF00>" @ %this.sourceObject.getDatablock().hName SPC "<bitmapk:Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_boomer2>" SPC %targetid.client.name);
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
	if(!isObject(%obj) || %obj.getstate() $= "Dead") return;
	
	%obj.setenergylevel(0);
	%obj.playaudio(0,"boomer_vomit" @ getrandom(1,4) @ "_sound");
	%obj.playthread(0,"jump");
	%obj.playthread(1,"jump");
	%obj.playthread(2,"activate2");

	%muzzle = vectorAdd(%obj.getMuzzlePoint(2),"0 0 0.35");
	%velocity = vectorScale(%obj.getEyeVector(),20);
	%velocity = getProjectileVector(%obj.hFollowing, %velocity, 1, %muzzle);
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