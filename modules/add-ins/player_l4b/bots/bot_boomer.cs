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

	function ZombieBoomerHoleBot::onBotLoop(%this,%obj)
{
	if(%obj.getstate() !$= "Dead" && !isEventPending(%obj.vomitschedule) && %obj.lastIdle+5000 < getsimtime())
	{
		switch$(%obj.hState)
		{
			case "Wandering":	%obj.isStrangling = false;
								%obj.hNoSeeIdleTeleport();
								%obj.playaudio(0,"boomer_lurk" @ getrandom(1,4) @ "_sound");
			case "Following": 	%obj.playaudio(0,"boomer_recognize" @ getrandom(1,4) @ "_sound");
		}

		%obj.playthread(3,plant);
		%obj.lastIdle = getsimtime();		
	}
}

function ZombieBoomerHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this,%obj,%col);
}

function ZombieBoomerHoleBot::onBotFollow( %this, %obj, %targ )
{
	if((isObject(%obj) && %obj.getState() !$= "Dead" && %obj.hLoopActive) && (isObject(%targ) && %targ.getState() !$= "Dead") && (%distance = vectorDist(%obj.getposition(),%targ.getposition())) < 30)
	%this.schedule(750,onBotFollow,%obj,%targ);
	else return;
		
	if(%distance > 10 &&%distance < 20)
	{
		if(%obj.GetEnergyLevel() >= %this.maxenergy && !isEventPending(%obj.SpecialSched))
		{
			%this.onTrigger(%obj,4,1);
			%obj.setMoveX(0);
			%obj.setMoveY(0);
			%obj.setJumping(0);
			%obj.setCrouching(0);
			%obj.setaimobject(%targ);
		}
	}
	else if(%distance < 10)
	{		
		if(%obj.GetEnergyLevel() >= %this.maxenergy)
		{
			%obj.stopHoleLoop();
			%obj.hRunAwayFromPlayer(%targ);
			%obj.schedule(1500,startHoleLoop);
		}
		else
		{
			%this.onTrigger(%obj,0,true);
			%obj.setMoveX(0);
			%obj.setMoveY(1);
			%obj.setmoveobject(%targ);
		}
	}
}

function ZombieBoomerHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	%limb = %obj.rgetDamageLocation(%position);
	if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact)
	if(%limb) %damage = %damage/6;
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function ZombieBoomerHoleBot::onDamage(%this,%obj,%delta)
{	
	Parent::onDamage(%this,%obj,%delta);

	if(%delta > 5 && %obj.lastdamage+500 < getsimtime())
	{			
		if(%obj.getstate() !$= "Dead")
		{
			%obj.playaudio(0,"boomer_pain" @ getrandom(1,4) @ "_sound");
			if(%Obj.GetDamageLevel() > %obj.getDatablock().maxDamage/2) %obj.playaudio(3,"boomer_indigestion_loop_sound");
			%obj.playthread(2,"plant");
		}				 
		%obj.lastdamage = getsimtime();	
	}

	if(%obj.getState() $= "Dead") 
	{
		%obj.hideNode("ALL");
		%obj.schedule(50,delete);
		%this.doSplatterBlood(%obj,30);

		%datablock = "bloodHeadDebrisProjectile RBloodOrganProjectile 0 0 bloodHandDebrisProjectile bloodHandDebrisProjectile 0 bloodFootDebrisProjectile bloodFootDebrisProjectile";
		for(%i = 0; %i < getWordCount(%datablock); %i++) if(isObject(getWord(%datablock, %i)) && !%obj.limbDismemberedLevel[%i]) doGibLimbExplosion(getWord(%datablock, %i),%obj.getHackPosition(), getWord(%obj.getScale(), 2));
		for (%j = 0; %j < getRandom(10,15); %j++) doGibLimbExplosion("bloodDismemberProjectile",%obj.getHackPosition(), getWord(%obj.getScale(), 2));
	
		%b = new projectile()
		{
			datablock = BoomerProjectile;
			initialPosition = %obj.getPosition();
			sourceObject = %obj;
			scale = "1 1 1";
			damageType = $DamageType::Boomer;
		};
		%obj.unMountImage(0);
	}	
}

function ZombieBoomerHoleBot::RbloodDismember(%this,%obj,%limb,%doeffects,%position)
{
	return;	
}

function ZombieBoomerHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{		
	Parent::onTrigger(%this,%obj,%triggerNum,%val);

	if(!isObject(%obj) || %obj.getState() $= "Dead") return;

	if(isObject(%obj.hFollowing)) %targ = %obj.hFollowing;
	else if(isObject(%obj.lastactivated))
	{
		if(%obj.lastactivated.getType() & $TypeMasks::PlayerObjectType) %targ = %obj.lastactivated;
		else return;
	}
	else return;
		
	if(%val) switch(%triggerNum)
	{
		case 0: if(!isEventPending(%obj.MeleeSched))
				{
					%obj.playthread(2,"zAttack" @ getRandom(1,3));
					cancel(%obj.MeleeSched);
					%obj.MeleeSched = %this.schedule(350,Melee,%obj,%targ);
				}

		case 4: if(%obj.GetEnergyLevel() >= %this.maxenergy)
				{
					%obj.setenergylevel(0);
					%obj.stopaudio(0);
					%obj.playaudio(0,"boomer_warn_sound");
					%obj.playthread(1,"boomerwarn");
					if(%obj.getclassname() $= "AIPlayer") %obj.stopHoleLoop();
					%randomtime = getRandom(600,1000);
					%obj.SpecialSched = %this.schedule(%randomtime,Special,%obj,10);
				}
	}
}

function ZombieBoomerHoleBot::Melee(%this,%obj,%targ)
{
	CommonZombieHoleBot::Melee(%this,%obj,%targ);
}

function ZombieBoomerHoleBot::Special(%this,%obj,%limit,%count)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead") return;
	
	if(%count <= 10)
	{
		if(%count == 1)
		{
			%obj.playaudio(0,"boomer_vomit" @ getrandom(1,4) @ "_sound");
			%obj.playthread(1,"boomervomit");
		}

		%obj.playthread(2,"plant");
		%obj.SpecialSched = %this.schedule(100,Special,%obj,10,%count+1);
		%p = new Projectile()
		{
			dataBlock = "BoomerVomitProjectile";
			initialVelocity = VectorAdd(vectorScale(%obj.getEyeVector(),20),"0 0 2.5");
			initialPosition = vectorAdd(%obj.getMuzzlePoint(2),"0 0 0.45");
			sourceObject = %obj;
			client = %obj.client;
		};
		MissionCleanup.add(%p);
	}
	else if(%obj.getclassname() $= "AIPlayer") %obj.schedule(1500,startHoleLoop);
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

function BoomerProjectile::onExplode(%this,%obj)
{
	Parent::onExplode(%this,%obj);

    InitContainerRadiusSearch(%obj.getPosition(), 5, $TypeMasks::PlayerObjectType);
    while ((%targetid = containerSearchNext()) != 0)
    {
        if((%targetid.getType() & $TypeMasks::PlayerObjectType) && checkHoleBotTeams(%obj.sourceObject,%targetid) && miniGameCanDamage(%obj.sourceObject,%targetid))
        {
			%targetid.setWhiteout(2);
			if(%targetid.BoomerBiled) return Parent::onExplode(%this,%obj);
			else
			{
				if(!%targetid.BoomerBiled)
				{
					if(isObject(%targetid.client) && isObject(%minigame = getMiniGameFromObject(%targetid))) 
					{
						%minigame.L4B_ChatMessage("<color:FFFF00>" @ %obj.sourceObject.getDatablock().hName SPC %obj.sourceObject.getdataBlock().hPinCI SPC %targetid.client.name,"victim_needshelp_sound",true);
						if(%miniGame.DirectorStatus != 2 && %minigame.RoundType !$= "Horde") %minigame.schedule(1000,HordeRound);
					}
					%targetid.vomitbot = new Player() 
					{ 
						dataBlock = "EmptyPlayer";
						source = %targetid;
						slotToMountBot = 2;
						imageToMount = "BileStatusPlayerImage";
					};
					%targetid.BoomerBiled = true;
				}
			}
        }
    }
}

function BoomerVomitProjectile::onExplode(%this,%obj)
{
	Parent::onExplode(%this,%obj);

    InitContainerRadiusSearch(%obj.getPosition(), 1, $TypeMasks::PlayerObjectType);
    while ((%targetid = containerSearchNext()) != 0)
    {
        if((%targetid.getType() & $TypeMasks::PlayerObjectType) && checkHoleBotTeams(%obj.sourceObject,%targetid) && miniGameCanDamage(%obj.sourceObject,%targetid))
        {
			%targetid.setWhiteout(2);
			if(%targetid.BoomerBiled) return Parent::onExplode(%this,%obj);
			else
			{
				if(!%targetid.BoomerBiled)
				{
					if(isObject(%targetid.client) && isObject(%minigame = getMiniGameFromObject(%targetid))) 
					{
						%minigame.L4B_ChatMessage("<color:FFFF00>" @ %obj.sourceObject.getDatablock().hName SPC %obj.sourceObject.getdataBlock().hPinCI SPC %targetid.client.name,"victim_needshelp_sound",true);
						if(%miniGame.DirectorStatus != 2 && %minigame.RoundType !$= "Horde") %minigame.schedule(1000,HordeRound);
					}
					%targetid.vomitbot = new Player() 
					{ 
						dataBlock = "EmptyPlayer";
						source = %targetid;
						slotToMountBot = 2;
						imageToMount = "BileStatusPlayerImage";
					};
					%targetid.BoomerBiled = true;
				}
			}
        }
    }
}

function BileStatusPlayerImage::onPulse(%this,%obj,%slot)
{
	if(%obj.PulseCount <= 15) %obj.PulseCount++;
	else 
	{
		%obj.source.BoomerBiled = false;
		%obj.delete();	
	}
}