luaexec("./survivor.lua");
if(LoadRequiredAddOn("Support_BotHolePlus") == $Error::None) exec("./bots/survivorally.cs");

function removeAreaFromZone(%brick)
{
    if(isObject(%brick.AreaZone))
    {
        for(%i = 0; %i < %brick.AreaZone.getcount(); %i++)
        {
            %areazone = %brick.AreaZone.getObject(%i);

            if(isObject(MainAreaZone) && MainAreaZone.isMember(%areazone))
            MainAreaZone.remove(%areazone);
        }
    }
}

registerInputEvent("fxDTSBrick","onAZFirstEntry","Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "Bot Bot" TAB "MiniGame MiniGame");

function Player::Safehouse(%player,%bool)
{
	%minigame = getMiniGameFromObject(%player);
	if(%player.hType !$= "Survivors" || isEventPending(%minigame.resetSchedule)) return;

	if(%bool) %player.InSafehouse = 1;
	else %player.InSafehouse = 0;
}

function PlayerHatModel::doDismount(%this, %obj, %forced) { return; }

function MiniGameSO::SafehouseCheck(%minigame,%client)
{
	for(%i = 0; %i < %minigame.numMembers; %i++)
	{
		%client = %minigame.member[%i];

		if(isObject(%player = %client.player) && !%player.hIsInfected && %player.getdataBlock().getname() !$= "SurvivorPlayerDowned")
		%livePlayerCount++;

		if(isObject(%player) && %player.InSafehouse)
		%safehousecount++;
	}
	
	if(%safehousecount >= %livePlayerCount && isObject(%minigame))
	{
		if(isEventPending(%minigame.resetSchedule))	return;

		if(isObject(l4b_music)) l4b_music.delete();

   		%minigame.scheduleReset(8000);
		%minigame.L4B_PlaySound("game_win_sound");

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
		return;
	}
}

function SurvivorPlayer::L4BAppearance(%this,%obj,%client) { Parent::L4BAppearance(%this,%obj,%client); }
function SurvivorPlayerMed::L4BAppearance(%this,%obj,%client) { SurvivorPlayer::L4BAppearance(%this,%obj,%client); }
function SurvivorPlayerLow::L4BAppearance(%this,%obj,%client) { SurvivorPlayer::L4BAppearance(%this,%obj,%client); }

function SurvivorPlayer::onImpact(%this, %obj, %col, %vec, %force)
{
	luacall(Survivor_FallDamage,%obj,%vec,%force);
	Parent::onImpact(%this, %obj, %col, %vec, %force);
}

function SurvivorPlayerMed::onImpact(%this, %obj, %col, %vec, %force) { SurvivorPlayer::onImpact(%this, %obj, %col, %vec, %force); }
function SurvivorPlayerLow::onImpact(%this, %obj, %col, %vec, %force) { SurvivorPlayer::onImpact(%this, %obj, %col, %vec, %force); }

function SurvivorPlayer::onEnterLiquid(%this, %obj, %cov, %type)
{
	cancel(%obj.oxygenTick);
	%obj.oxygenTick = %obj.schedule(2500, oxygenTick);	
	Parent::onEnterLiquid(%this, %obj, %cov, %type);
}

function SurvivorPlayer::onLeaveLiquid(%this, %obj, %type)
{
	%obj.schedule(150,checkIfUnderwater);	
	Parent::onLeaveLiquid(%this, %obj, %cov, %type);
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
	if(%obj.getClassName() $= "Player" && isObject(getMinigameFromObject(%obj)) && !%obj.hIsImmune)
	{	
		%obj.client.Play2d("survivor_notimmune_sound");
		GameConnection::ChatMessage (%obj.client, "\c3You are not immune, depleted energy results in infection.");
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
			case 0: luacall(Survivor_LeftClick,%val,%obj);
			case 4: luacall(Survivor_Rightclick,%obj);
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

	if($Pref::Server::L4B2Bots::SurvivorImmunity) %obj.hIsImmune = 1;
	%obj.SurvivorStress = 0;
	%obj.hType = "Survivors";

	if(!%obj.hIsImmune && %obj.getdamagelevel() <= 0) commandToClient(%obj.client, 'SetVignette', false, "0 0 0 1");
}

function SurvivorPlayerMed::onNewDataBlock(%this,%obj)
{	
	SurvivorPlayer::onNewDataBlock(%this,%obj);
}
		
function SurvivorPlayerLow::onNewDataBlock(%this,%obj)
{
	SurvivorPlayer::onNewDataBlock(%this,%obj);
}

function SurvivorPlayerLow::onDisabled(%this,%obj,%state)
{
	%obj.playaudio(0,"survivor_death" @ getRandom(1, 8) @ "_sound");

	if(%obj.getWaterCoverage() == 1)
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
	if(!isObject(%obj) && %obj.getState() $= "Dead") return;
	
	if(%obj.getWaterCoverage() == 1)
	{
		%obj.oxygenCount = mClamp(%obj.oxygenCount++, 0, 6);	

		if(%obj.oxygenCount == 6) %obj.Damage(%obj, %obj.getPosition (), 25, $DamageType::Suicide);
		
		%obj.lastwatercoverage = getsimtime();
		%bubblepitch = 0.125*%obj.oxygenCount;
		%obj.emote(oxygenBubbleImage, 1);
		%obj.playthread(3,plant);

		if($oldTimescale $= "")
		$oldTimescale = getTimescale();
  		setTimescale(0.25+%bubblepitch);
  		serverPlay3D("drown_bubbles_sound",%obj.getPosition());
  		setTimescale($oldTimescale);
	}
	
	cancel(%obj.oxygenTick);
	%obj.oxygenTick = %obj.schedule(2500, oxygenTick);
}

function SurvivorPlayer::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	if(luacall(Survivor_DownCheck,%obj,%damage,%damageType)) return;
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function SurvivorPlayer::onDamage(%this,%obj,%delta)
{
	Parent::onDamage(%this,%obj,%delta);
	luacall(Survivor_DamageCheck,%obj,%delta);
}

function SurvivorPlayerMed::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	if(luacall(Survivor_DownCheck,%obj,%damage,%damageType)) return;
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function SurvivorPlayerMed::onDamage(%this,%obj,%delta)
{
	Parent::onDamage(%this,%obj,%delta);
	luacall(Survivor_DamageCheck,%obj,%delta);
}

function SurvivorPlayerLow::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	if(luacall(Survivor_DownCheck,%obj,%damage,%damageType)) return;
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function SurvivorPlayerLow::onDamage(%this,%obj,%delta)
{
	Parent::onDamage(%this,%obj,%delta);
	luacall(Survivor_DamageCheck,%obj,%delta);
}

function energydamageloop(%obj)
{ 
	if(isobject(%obj) && %obj.getstate() !$= "Dead" && %obj.getdataBlock().getName() $= "SurvivorPlayerDowned")
	{
		if(%obj.getClassName() $= "Player")
		{
			%obj.client.Play2d("survivor_heartbeat_sound");
			%obj.setdamageflash(16/%obj.getenergylevel());
		}

		if(%obj.lastcry+10000 < getsimtime())
		{
			%obj.lastcry = getsimtime();
			%obj.playaudio(0,"survivor_pain_high1_sound");
		}

		%obj.setenergylevel(%obj.getenergylevel()-2);
	
		if(%obj.getenergylevel() == 0 && !%obj.isBeingSaved)
		{
			%obj.kill();
			return;
		}
	
		cancel(%obj.energydeath);
		%obj.energydeath = schedule(750,0,energydamageloop,%obj);
	}
	else return;

}

function SurvivorPlayerDowned::onImpact(%this, %obj, %col, %vec, %force)
{
	luacall(Survivor_FallDamage,%obj,%vec,%force);
	Parent::onImpact(%this, %obj, %col, %vec, %force);
}

function SurvivorPlayerDowned::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage/4,%damageType,%damageLoc);
}

function SurvivorPlayerDowned::onDisabled(%this,%obj)
{	
	%obj.playaudio(0,"survivor_death" @ getRandom(1, 8) @ "_sound");
	commandToClient(%obj.client,'SetVignette',$EnvGuiServer::VignetteMultiply,$EnvGuiServer::VignetteColor);

	Parent::onDisabled(%this, %obj);
}

function SurvivorPlayerDowned::onNewDataBlock(%this,%obj)
{
	if(isObject(%minigame = getMiniGameFromObject(%obj))) %minigame.checkLastManStanding();	
	
	if(%obj.getClassName() $= "Player")
	{
		chatMessageTeam(%obj.client,'fakedeathmessage',"<color:FFFF00><bitmapk:Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_down>" SPC %obj.client.name);

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

function Player::checkIfUnderwater(%obj)
{
   if(%obj.getWaterCoverage() == 0)
   {
      if(%obj.oxygenCount == 6 && %obj.getState() !$= "Dead")
      %obj.playaudio(0,"survivor_painhigh" @ getRandom(1, 4) @ "_sound");

	  %obj.oxygenCount = 0;
   }
   cancel(%obj.oxygenTick);
}