function Player::Safehouse(%player)
{
	%minigame = getMiniGameFromObject(%player);
	if(%player.hType !$= "Survivors" || isEventPending(%minigame.resetSchedule))
	return;

	%player.InSafehouse = 1;
	cancel(%player.NoSafeHouseSchedule);
	%player.NoSafeHouseSchedule = %player.schedule(500,NoSafeHouse);

	for(%i = 0; %i < %minigame.numMembers; %i++)
	{
		%client = %minigame.member[%i];

		if(isObject(%player = %client.player) && !%player.hIsInfected && %player.getdataBlock().getname() !$= "DownPlayerSurvivorArmor")
		%livePlayerCount++;

		if(isObject(%player) && %player.InSafehouse)
		%safehousecount++;
	}
	
	if(%safehousecount >= %livePlayerCount && isObject(%minigame))
	{
		%minigame.SurvivorWin();
		return;
	}
}

function Player::NoSafeHouse(%player)
{
	%player.InSafehouse = 0;
}
registerOutputEvent ("Player", "Safehouse");

function MinigameSO::SurvivorWin(%minigame,%client)
{
	if(isEventPending(%minigame.resetSchedule))	
	return;
	
	if(isObject(l4b_music))
	l4b_music.delete();

	//%minigame.schedule(3000,chatMessageAll,0,'<font:impact:25>\c6Resetting minigame in 5 seconds.');
	//%minigame.schedule(7750,chatMessageAll,0,'<font:impact:25>\c6Resetting minigame.');
   	%minigame.scheduleReset(8000);
	%minigame.L4B_PlaySound("game_win_sound");
	%minigame.DirectorProcessEvent("onSurvivorsWin",%client);

    for(%i=0;%i<%minigame.numMembers;%i++)
    {
		%member = %minigame.member[%i];

		if(isObject(%member.player))
		{
			if(%member.player.hType $= "Survivors")
			%member.player.emote(winStarProjectile, 1);

			%member.Camera.setOrbitMode(%member.player, %member.player.getTransform(), 0, 5, 0, 1);
			%member.setControlObject(%member.Camera);
		}
    }
}

registerInputEvent("fxDTSBrick", "onSurvivorsWin", "Self fxDTSBrick" TAB "MiniGame MiniGame");
registerInputEvent("fxDTSBrick", "onSurvivorsLose", "Self fxDTSBrick" TAB "MiniGame MiniGame");

function SurvivorPlayerDmg(%this,%obj,%am)
{
	if(%obj.getstate() $= "Dead")
	return;
	
	if(!%obj.getWaterCoverage() $= 1)
	{
		if(%am >=5 && %obj.lastdamage+500 < getsimtime())
		{
			%obj.playaudio(0,"survivor_pain" @ getRandom(1, 4) @ "_sound");

			if(%am >= 20)
			{
				%obj.playaudio(0,"survivor_painmed" @ getRandom(1, 4) @ "_sound");

				if(%am >= 30)
				%obj.playaudio(0,"survivor_painhigh" @ getRandom(1, 4) @ "_sound");
			}

			%obj.lastdamage = getsimtime();
		}
	}
	else %obj.playaudio(0,"survivor_pain_underwater_sound");

	if(%Obj.GetDamageLevel() <= 50 && %this.getName() !$= "SurvivorPlayer")
	{
		%Obj.SetDataBlock("SurvivorPlayer");
		return;
	}
	else if(%Obj.GetDamageLevel() >= 50 && %this.getName() !$= "SurvivorPlayerMed")
	{
		%Obj.SetDataBlock("SurvivorPlayerMed");

		if(%obj.getdamagelevel() >= 75 && %this.getName() !$= "SurvivorPlayerLow")
		%Obj.SetDataBlock("SurvivorPlayerLow");

		return;
	}
}

function SurvivorPlayer::onDamage(%this,%obj,%am)
{
	Parent::onDamage(%this,%obj,%am);
	SurvivorPlayerDmg(%this,%obj,%am);
}
function SurvivorPlayerMed::OnDamage(%This,%Obj,%Am)
{
	SurvivorPlayer::onDamage(%this,%obj,%am);
}
function SurvivorPlayerLow::OnDamage(%This,%Obj,%Am)
{
	SurvivorPlayer::onDamage(%this,%obj,%am);
}

function SurvivorPlayer::onCollision(%this,%obj,%col,%vec,%speed)
{
	Parent::onCollision(%this,%obj,%col,%vec,%speed);

	if(isObject(%col) && %col.getType() & $TypeMasks::PlayerObjectType)
	L4B_SaveVictim(%obj,%col);
}

function SurvivorPlayerMed::onCollision(%this,%obj,%col)
{
	Parent::onCollision(%this, %obj, %col, %fade, %pos, %norm);
	if(isObject(%col) && %col.getType() & $TypeMasks::PlayerObjectType)
	L4B_SaveVictim(%obj,%col);
}
function SurvivorPlayerLow::onCollision(%this,%obj,%col)
{
	Parent::onCollision(%this, %obj, %col, %fade, %pos, %norm);
	if(isObject(%col) && %col.getType() & $TypeMasks::PlayerObjectType)
	L4B_SaveVictim(%obj,%col);
}

function SurvivorPlayer::onEnterLiquid(%this, %obj, %cov, %type)
{
	cancel(%obj.oxygenTick);
	%obj.oxygenTick = %obj.schedule(2500, oxygenTick);
	
	parent::onEnterLiquid(%this, %obj, %cov, %type);
}

function SurvivorPlayer::onLeaveLiquid(%this, %obj, %type)
{
	%obj.schedule(150,checkIfUnderwater,%obj);
	
	parent::onLeaveLiquid(%this, %obj, %cov, %type);
}

function SurvivorPlayerMed::onEnterLiquid(%this, %obj, %cov, %type)
{
	SurvivorPlayer::onEnterLiquid(%this, %obj, %cov, %type);
	Parent::onEnterLiquid(%this, %obj, %cov, %type);
}

function SurvivorPlayerMed::onLeaveLiquid(%this, %obj, %type)
{
	SurvivorPlayer::onLeaveLiquid(%this, %obj, %type);
	Parent::onLeaveLiquid(%this, %obj, %cov, %type);
}

function SurvivorPlayerLow::onEnterLiquid(%this, %obj, %cov, %type)
{
	SurvivorPlayer::onEnterLiquid(%this, %obj, %cov, %type);
	Parent::onEnterLiquid(%this, %obj, %cov, %type);
}

function SurvivorPlayerLow::onLeaveLiquid(%this, %obj, %type)
{
	SurvivorPlayer::onLeaveLiquid(%this, %obj, %type);
	Parent::onLeaveLiquid(%this, %obj, %cov, %type);
}

function SurvivorPlayer::onAdd(%this,%obj)
{
	%obj.client.NotifyOfImmunity = 0;

	if(%obj.getClassName() $= "Player" && isObject(getMinigameFromObject(%obj)))
	{	
		if(!%obj.hIsImmune)
		if(!%obj.client.NotifyOfImmunity)
		{
			%obj.client.Play2d("survivor_notimmune_sound");
			%obj.client.NotifyOfImmunity = 1;
			GameConnection::ChatMessage (%obj.client, "\c3You are not immune, find a Panacea Syringe to gain immunity from the zombie infection.");
		}
	}
	
	Parent::onAdd(%this,%obj);
}

function SurvivorPlayer::onTrigger (%this, %obj, %triggerNum, %val)
{
	Parent::onTrigger (%this, %obj, %triggerNum, %val);

	if(%obj.getClassName() $= "Player" && %obj.getState() !$= "Dead")
	{		
		switch(%triggerNum)
		{
			case 0: if(!isObject(%obj.getMountedImage(0)))
					{
						if(%val)
						{
							%eye = %obj.getEyePoint(); //eye point
							%scale = getWord (%obj.getScale (), 2);
							%len = $Game::BrickActivateRange * %scale; //raycast length
							%masks = $TypeMasks::FxBrickObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType;
							%vec = %obj.getEyeVector(); //eye vector
							%beam = vectorScale(%vec,%len); //lengthened vector (for calculating the raycast's endpoint)
							%end = vectorAdd(%eye,%beam); //calculated endpoint for raycast
							%ray = containerRayCast(%eye,%end,%masks,%obj); //fire raycast
							%ray = isObject(firstWord(%ray)) ? %ray : 0; //if raycast hit - keep ray, else set it to zero

							if(!%ray) //if Beam Check fcn returned "0" (found nothing)
							return Parent::onTrigger (%this, %obj, %triggerNum, %val); //stop here

							%target = firstWord(%ray);

							if(%target.getType() & $TypeMasks::PlayerObjectType)
							{
								L4B_SaveVictim(%obj,%target);
								L4B_ReviveDowned(%obj);
							}
						}
						else
						{
							if(%obj.issavior)
							{
								if(%obj.getmountedimage(0) == 0)
								%obj.stopthread(1);
								%obj.issavior = 0;
								%obj.isSaving.isBeingSaved = 0;
								%obj.isSaving = 0;
							}
							%obj.savetimer = 0;
							cancel(%obj.savesched);
						}
					}
			case 4: if(%val)
					%obj.meleeTrigger();
			default:
		}
	}
}

function SurvivorPlayerMed::onTrigger (%this, %obj, %triggerNum, %val)
{
	Parent::onTrigger (%this, %obj, %triggerNum, %val);
	SurvivorPlayer::onTrigger (%this, %obj, %triggerNum, %val);
}

function SurvivorPlayerLow::onTrigger (%this, %obj, %triggerNum, %val)
{
	Parent::onTrigger (%this, %obj, %triggerNum, %val);
	SurvivorPlayer::onTrigger (%this, %obj, %triggerNum, %val);
}

function SurvivorPlayer::onNewDataBlock(%this,%obj)
{	
	Parent::onNewDataBlock(%this,%obj);

	if($Pref::SurvivorPlayer::SurvivorImmunity)
	%obj.hIsImmune = 1;

	%obj.hType = "Survivors";
	%obj.BrickScanCheck();

	if(!%obj.hIsImmune && %obj.getdamagelevel() <= 0)
	commandToClient(%obj.client, 'SetVignette', false, "0 0 0 1");
}

function SurvivorPlayerMed::onNewDataBlock(%this,%obj)
{	
	Parent::onNewDataBlock(%this,%obj);
	%obj.BrickScanCheck();

	if(!%obj.hIsImmune && %obj.getdamagelevel() <= 0)
	commandToClient(%obj.client, 'SetVignette', false, "0 0 0 1");
}
		
function SurvivorPlayerLow::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	%obj.BrickScanCheck();

	if(!%obj.hIsImmune && %obj.getdamagelevel() <= 0)
	commandToClient(%obj.client, 'SetVignette', false, "0 0 0 1");
}

function SurvivorPlayerLow::onDisabled(%this,%obj,%state)
{
	%obj.playaudio(0,"survivor_death" @ getRandom(1, 8) @ "_sound");

	if(%obj.getWaterCoverage() == 1 && %obj.getenergylevel() == 0)
	{
		%obj.playaudio(0,"survivor_death_underwater" @ getRandom(1, 2) @ "_sound");
		%obj.emote(oxygenBubbleImage, 1);
		serverPlay3D("drown_bubbles_sound",%obj.getPosition());
		serverPlay3D("die_underwater_bubbles_sound",%obj.getPosition());
	}

	Parent::onDisabled(%this,%obj,%state);
}

function SurvivorPlayerMed::onDisabled(%this,%obj,%state)
{
	SurvivorPlayerLow::onDisabled(%this,%obj,%state);
	Parent::onDisabled(%this,%obj,%state);
}

function SurvivorPlayer::onDisabled(%this,%obj,%state)
{
	SurvivorPlayerLow::onDisabled(%this,%obj,%state);
	Parent::onDisabled(%this,%obj,%state);
}

function Player::oxygenTick(%obj)
{   
	if(!isObject(%obj) && %obj.getState() $= "Dead")
	return;
	
	if(%obj.getWaterCoverage() == 1)
	{
		%obj.lastwatercoverage = getsimtime();

		%obj.setenergylevel(%obj.getenergylevel()-12.5);
		%obj.emote(oxygenBubbleImage, 1);
		serverPlay3D("drown_bubbles_sound",%obj.getPosition());
		%obj.playthread(3,plant);

		if(%obj.getenergylevel() == 0)
		%obj.Damage(%obj, %obj.getPosition (), 25, $DamageType::Suicide);
	}
	else 
	{
		if(%obj.getenergylevel() <= 5 && %obj.getState() !$= "Dead")
		%obj.playaudio(0,"survivor_painhigh" @ getRandom(1, 4) @ "_sound");

		%obj.setenergylevel(%obj.getDatablock().maxenergy);
	}
	
	cancel(%obj.oxygenTick);
	%obj.oxygenTick = %obj.schedule(2500, oxygenTick);
}

function Player::SurvivorDownedCheck(%obj,%damage,%damageType)
{	
	if(%damageType $= $DamageType::Fall)	
	{
		serverPlay3D("impact_fall_sound",%obj.getPosition());
		serverPlay3D("victim_smoked_sound",%obj.getPosition());

		if(isObject(ZombieHitProjectile))
		%p = new Projectile()
		{
			dataBlock = "ZombieHitProjectile";
			initialPosition = %obj.getPosition();
			sourceObject = %obj;
		};
		MissionCleanup.add(%p);
		%p.explode();
	}

	if($Pref::SurvivorPlayer::EnableDowning)
	{
		if(%obj.getstate() !$= "Dead" && %obj.getdamagelevel()+%damage >= %obj.getdataBlock().maxDamage && %damage <= %obj.getDatablock().maxDamage/1.333 && %obj.getWaterCoverage() != 1)
		{
			%obj.setdamagelevel(0);
			%obj.setenergylevel(100);
			%obj.isdowned = 1;
			%obj.changedatablock("DownPlayerSurvivorArmor");
			return 1;
		}
		else return 0;
	}
	else return 0;
}

function SurvivorPlayer::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	if(%obj.SurvivorDownedCheck(%damage,%damageType))
	return;

	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function SurvivorPlayerMed::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	if(%obj.SurvivorDownedCheck(%damage,%damageType))
	return;

	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function SurvivorPlayerLow::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	if(%obj.SurvivorDownedCheck(%damage,%damageType))
	return;

	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function energydamageloop(%obj)
{ 
	if(isobject(%obj) && %obj.getstate() !$= "Dead" && %obj.getdataBlock().getName() $= "DownPlayerSurvivorArmor")
	{
		if(%obj.getenergylevel() == 0 && !%obj.isBeingSaved)
		{
			%obj.kill();
			return;
		}

		cancel(%obj.energydeath1);
		cancel(%obj.energydeath2);
		cancel(%obj.energydeath3);
		
		if(%obj.getClassName() $= "Player")
		{
			%obj.client.Play2d("survivor_heartbeat_sound");
			%obj.setdamageflash(16/%obj.getenergylevel());
		}

		if(%obj.getenergylevel() >= 75)//1
		{
			%obj.energydeath1 = schedule(750,0,energydamageloop,%obj);

			%obj.setenergylevel(%obj.getenergylevel()-1);
		}
		if(%obj.getenergylevel() <= 75)//2
		{
			%obj.energydeath2 = schedule(625,0,energydamageloop,%obj);

			%obj.setenergylevel(%obj.getenergylevel()-1);
		}
		if(%obj.getenergylevel() <= 50)//3
		{
			%obj.setenergylevel(%obj.getenergylevel()-1);

			%obj.energydeath3 = schedule(500,0,energydamageloop,%obj);
		}

		if(%obj.lastcry+10000 < getsimtime())
		{
			%obj.lastcry = getsimtime();
			%obj.playaudio(0,"survivor_pain_high1_sound");
		}
	}
	else 
	{
		%obj.kill();
		return;
	}

}

function DownPlayerSurvivorArmor::ondisabled(%this,%obj,%n)
{	
	%obj.playaudio(0,"survivor_death" @ getRandom(1, 8) @ "_sound");
	commandToClient(%obj.client,'SetVignette',$EnvGuiServer::VignetteMultiply,$EnvGuiServer::VignetteColor);

	Parent::onDisabled(%this, %obj);
}

function DownPlayerSurvivorArmor::onNewDataBlock(%this,%obj)
{
	if(%obj.getClassName() $= "Player")
	{
		chatMessageTeam(%obj.client,'fakedeathmessage',"<color:FFFF00><bitmapk:add-ons/package_left4block/icons/downci>" SPC %obj.client.name);

		if(isObject(%obj.billboard) && $L4B_hasSelectiveGhosting)
		Billboard_DeallocFromPlayer($L4B::Billboard_SO, %obj);

		if($L4B_hasSelectiveGhosting)
		Billboard_MountToPlayer(%obj, $L4B::Billboard_SO, incappedBillboard);

		%minigame = %obj.client.minigame;
		%minigame.L4B_PlaySound("victim_needshelp_sound");
		%minigame.checkLastManStanding();
	}

	%obj.playthread(0,sit);

	%obj.lastcry = getsimtime();
	%obj.playaudio(0,"survivor_pain_high1_sound");
	energydamageloop(%obj);
	
	Parent::onNewDataBlock(%this,%obj);
}

function Player::checkIfUnderwater(%this, %obj)
{
   if(%obj.getWaterCoverage() == 0)
   {
      if(%obj.getenergylevel() <= 5 && %obj.getState() !$= "Dead")
      %obj.playaudio(0,"survivor_painhigh" @ getRandom(1, 4) @ "_sound");
   
      %obj.setenergylevel(%obj.getDatablock().maxenergy);
   }
   cancel(%obj.oxygenTick);
}