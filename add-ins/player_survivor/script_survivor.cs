luaexec("./survivor_lua.lua");
exec("./datablocks.cs");

function Player::Safehouse(%player,%bool)
{
	%minigame = getMiniGameFromObject(%player);
	if(%player.hType !$= "Survivors" || isEventPending(%minigame.resetSchedule)) return;

	if(%bool) %player.InSafehouse = 1;
	else %player.InSafehouse = 0;
}

registerOutputEvent ("Player", "Safehouse","bool");
registerInputEvent ("fxDTSBrick", "onSurvivorTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerOutputEvent(Minigame, "SafehouseCheck");

package L4B2Bots_NewPlayerDatablock
{
    function applyCharacterPrefs(%client)
	{	
		if(!isObject(%client.Player) || %client.player.getdatablock().getName() $= "ZombieTankHoleBot")
		return;

		%client.applyBodyParts();
		%client.applyBodyColors();
	}

	function player::fixAppearance(%pl,%cl)
	{
		%pl.hideNode("ALL");
		%pl.unHideNode((%cl.chest ? "femChest" : "chest"));
		
		%pl.unHideNode((%cl.rhand ? "rhook" : "rhand"));
		%pl.unHideNode((%cl.lhand ? "lhook" : "lhand"));
		%pl.unHideNode((%cl.rarm ? "rarmSlim" : "rarm"));
		%pl.unHideNode((%cl.larm ? "larmSlim" : "larm"));
		%pl.unHideNode("headskin");

		if($pack[%cl.pack] !$= "none")
		{
			%pl.unHideNode($pack[%cl.pack]);
			%pl.setNodeColor($pack[%cl.pack],%cl.packColor);
		}
		if($secondPack[%cl.secondPack] !$= "none")
		{
			%pl.unHideNode($secondPack[%cl.secondPack]);
			%pl.setNodeColor($secondPack[%cl.secondPack],%cl.secondPackColor);
		}
		if($hat[%cl.hat] !$= "none")
		{
			%pl.unHideNode($hat[%cl.hat]);
			%pl.setNodeColor($hat[%cl.hat],%cl.hatColor);
		}
		if(%cl.hip)
		{
			%pl.unHideNode("skirthip");
			%pl.unHideNode("skirttrimleft");
			%pl.unHideNode("skirttrimright");
		}
		else
		{
			%pl.unHideNode("pants");
			%pl.unHideNode((%cl.rleg ? "rpeg" : "rshoe"));
			%pl.unHideNode((%cl.lleg ? "lpeg" : "lshoe"));
		
		}
		%pl.setHeadUp(0);
		if(%cl.pack+%cl.secondPack > 0) %pl.setHeadUp(1);
		
		if($hat[%cl.hat] $= "Helmet")
		{
			if(%cl.accent == 1 && $accent[4] !$= "none")
			{
				%pl.unHideNode($accent[4]);
				%pl.setNodeColor($accent[4],%cl.accentColor);
			}
		}
		else if($accent[%cl.accent] !$= "none" && strpos($accentsAllowed[$hat[%cl.hat]],strlwr($accent[%cl.accent])) != -1)
		{
			%pl.unHideNode($accent[%cl.accent]);
			%pl.setNodeColor($accent[%cl.accent],%cl.accentColor);
		}
	
		%pl.setFaceName(%cl.faceName);
		%pl.setDecalName(%cl.decalName);
		
		%pl.setNodeColor("headskin",%cl.headColor);
		
		%pl.setNodeColor("chest",%cl.chestColor);
		%pl.setNodeColor("femChest",%cl.chestColor);
		%pl.setNodeColor("pants",%cl.hipColor);
		%pl.setNodeColor("skirthip",%cl.hipColor);
		
		%pl.setNodeColor("rarm",%cl.rarmColor);
		%pl.setNodeColor("larm",%cl.larmColor);
		%pl.setNodeColor("rarmSlim",%cl.rarmColor);
		%pl.setNodeColor("larmSlim",%cl.larmColor);
		
		%pl.setNodeColor("rhand",%cl.rhandColor);
		%pl.setNodeColor("lhand",%cl.lhandColor);
		%pl.setNodeColor("rhook",%cl.rhandColor);
		%pl.setNodeColor("lhook",%cl.lhandColor);
		
		%pl.setNodeColor("rshoe",%cl.rlegColor);
		%pl.setNodeColor("lshoe",%cl.llegColor);
		%pl.setNodeColor("rpeg",%cl.rlegColor);
		%pl.setNodeColor("lpeg",%cl.llegColor);
		%pl.setNodeColor("skirttrimright",%cl.rlegColor);
		%pl.setNodeColor("skirttrimleft",%cl.llegColor);
	
		%pl.getDataBlock().hCustomNodeAppearance(%pl);
	
		if(isObject(%cl) && %cl.isAdmin || %cl.isSuper || %cl.player.shades)
		{
			%cl.player.unHideNode("shades");
			%cl.player.setNodeColor("shades","0.1 0.1 0.1 1");
		}

		if(%pl.hIsInfected)
		{
			%pl.unhidenode("gloweyes");
			%pl.setnodeColor("gloweyes","1 1 0 1");

			if(%pl.getClassName() $= "Player")
			{
				%skin = %cl.headColor;
				%zskin = getWord(%skin,0)/2.75 SPC getWord(%skin,1)/1.5 SPC getWord(%skin,2)/2.75 SPC 1;
				%pl.client.zombieColor = %zskin;
				%pl.setNodeColor("headskin", %zSkin);
	
				if(%cl.rArmColor $= %skin)
				{
					%pl.setNodeColor($RArm[0], %zSkin);
					%pl.setNodeColor($RArm[1], %zSkin);
				}
	
				if(%cl.lArmColor $= %skin)
				{
					%pl.setNodeColor($LArm[0], %zSkin);
					%pl.setNodeColor($LArm[1], %zSkin);
				}
	
				if(%cl.rHandColor $= %skin)
				{
					%pl.setNodeColor($RHand[0], %zSkin);
					%pl.setNodeColor($RHand[1], %zSkin);
				}
	
				if(%cl.lHandColor $= %skin)
				{
					%pl.setNodeColor($LHand[0], %zSkin);
					%pl.setNodeColor($LHand[1], %zSkin);
				}
	
				if(%cl.chestColor $= %skin)
				{
					%pl.setNodeColor($Chest[0], %zSkin);
					%pl.setNodeColor($Chest[1], %zSkin);
				}
	
				if(%cl.hipColor $= %skin)
				{
					%pl.setNodeColor($Hip[0], %zSkin);
					%pl.setNodeColor($Hip[1], %zSkin);
				}
	
				if(%cl.rLegColor $= %skin)
				{
					%pl.setNodeColor($RLeg[0], %zSkin);
					%pl.setNodeColor($RLeg[1], %zSkin);
				}
	
				if(%cl.lLegColor $= %skin)
				{
					%pl.setNodeColor($LLeg[0], %zSkin);
					%pl.setNodeColor($LLeg[1], %zSkin);
				}
	
				if($Pref::Server::L4B2Bots::::CustomStyle < 2)
				%pl.setFaceName("asciiTerror");
				else %pl.setFaceName($hZombieFace[getRandom(1,$hZombieFaceAmount)]);
	
				switch$(%pl.getDataBlock().getName())
				{
					case "ZombieHunterHoleBot": %pl.hideNode($hat[%cl.hat]);
												%pl.unhideNode($hat[1]);
												%pl.hidenode(Lhand);
    											%pl.hidenode(Rhand);
												%pl.setnodecolor($hat[1],%cl.chestColor);
												%pl.setNodeColor($RArm[0],%cl.chestColor);
												%pl.setNodeColor($RArm[1],%cl.chestColor);
												%pl.setNodeColor($LArm[0],%cl.chestColor);
												%pl.setNodeColor($LArm[1],%cl.chestColor);
												%pl.setDecalName("Hoodie");
	
					case "ZombieChargerHoleBot": %pl.setNodeColor($Chest[0], %zSkin);
												 %pl.setNodeColor($Chest[1], %zSkin);
												 %pl.setDecalName("worm_engineer");
												
												 %pl.HideNode("lhand_blood");
												 %pl.HideNode("rhand_blood");
												 %pl.bloody["rhand"] = false;
												 %pl.bloody["lhand"] = false;
												 
					case "ZombieJockeyHoleBot": %pl.setNodeColor($RLeg[0], %zSkin);
												%pl.setNodeColor($RLeg[1], %zSkin);
												%pl.setNodeColor($LLeg[0], %zSkin);
												%pl.setNodeColor($LLeg[1], %zSkin);
												%pl.setNodeColor($Chest[0], %zSkin);
												%pl.setNodeColor($Chest[1], %zSkin);
												%pl.setNodeColor($LHand[0], %zSkin);
												%pl.setNodeColor($LHand[1], %zSkin);
												%pl.setNodeColor($RHand[0], %zSkin);
												%pl.setNodeColor($RHand[1], %zSkin);
												%pl.setNodeColor($RArm[0], %zSkin);
												%pl.setNodeColor($RArm[1], %zSkin);
												%pl.setNodeColor($LArm[0], %zSkin);
												%pl.setNodeColor($LArm[1], %zSkin);
												%pl.setDecalName("AAA-None");
					case "ZombieBoomerHoleBot": %pl.hideNode($hat[%cl.hat]);
					default:
				}
			}			
		}		
	}

	function GameConnection::createPlayer (%client, %spawnPoint)
	{
		Parent::createPlayer (%client, %spawnPoint);

		if(isObject(%client.Player))
		%client.Player.spawnTime = getSimTime();
		
		if(%client.setZombieBlock)
		if(isObject(%client.Player))
		{
			%client.player.setDataBlock($hZombieSpecialType[getRandom(1,$hZombieSpecialTypeAmount)]);
			commandToClient (%client, 'ShowEnergyBar', true);
			%client.setZombieBlock = 0;
		}
	}

	function GameConnection::applyBodyColors(%cl,%o) 
	{
		if(isObject(%cl.player) && %cl.player.getdataBlock().getName() !$= "ZombieTankHoleBot")
		parent::applyBodyColors(%cl,%o);
		
		if(isObject(%pl = %cl.player))
		if(fileName (%pl.getDataBlock().shapeFile) $= "mmelee.dts")
		%pl.fixAppearance(%cl);
	}

	function GameConnection::applyBodyParts(%cl,%o) 
	{
		if(isObject(%cl.player) && %cl.player.getdataBlock().getName() !$= "ZombieTankHoleBot")
		parent::applyBodyParts(%cl,%o);
		
		if(isObject(%pl = %cl.player))
		if(fileName (%pl.getDataBlock().shapeFile) $= "mmelee.dts")
		%pl.fixAppearance(%cl);
	}
};
activatePackage(L4B2Bots_NewPlayerDatablock);

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
		if(isEventPending(%minigame.resetSchedule))	
		return;

		if(isObject(l4b_music))
		l4b_music.delete();

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

function SurvivorPlayer::onCollision(%this,%obj,%col,%vec,%speed)
{	
	Parent::onCollision(%this,%obj,%col,%vec,%speed);
}

function SurvivorPlayer::onImpact(%this, %obj, %col, %vec, %force)
{
	luacall(Survivor_FallDamage,%obj,%vec,%force);
	Parent::onImpact(%this, %obj, %col, %vec, %force);
}

function SurvivorPlayerMed::onImpact(%this, %obj, %col, %vec, %force)
{
	luacall(Survivor_FallDamage,%obj,%vec,%force);
	Parent::onImpact(%this, %obj, %col, %vec, %force);
}

function SurvivorPlayerLow::onImpact(%this, %obj, %col, %vec, %force)
{
	luacall(Survivor_FallDamage,%obj,%vec,%force);
	Parent::onImpact(%this, %obj, %col, %vec, %force);
}

function SurvivorPlayerMed::onCollision(%this,%obj,%col)
{
	Parent::onCollision(%this, %obj, %col, %fade, %pos, %norm);
}
function SurvivorPlayerLow::onCollision(%this,%obj,%col)
{
	Parent::onCollision(%this, %obj, %col, %fade, %pos, %norm);
}

function SurvivorPlayer::onEnterLiquid(%this, %obj, %cov, %type)
{
	cancel(%obj.oxygenTick);
	%obj.oxygenTick = %obj.schedule(2500, oxygenTick);
	
	parent::onEnterLiquid(%this, %obj, %cov, %type);
}

function SurvivorPlayer::onLeaveLiquid(%this, %obj, %type)
{
	%obj.schedule(150,checkIfUnderwater);
	
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

	if(%obj.getClassName() $= "Player" && isObject(getMinigameFromObject(%obj)))
	{	
		if(!%obj.hIsImmune)
		{
			%obj.client.Play2d("survivor_notimmune_sound");
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

	if($Pref::Server::L4B2Bots::SurvivorImmunity)
	%obj.hIsImmune = 1;
	
	%obj.SurvivorStress = 0;

	%obj.hType = "Survivors";
	%obj.BrickScanCheck();

	if(isObject(%obj.client.deathMusic))
	%obj.client.deathMusic.delete();

	if(!%obj.hIsImmune && %obj.getdamagelevel() <= 0)
	commandToClient(%obj.client, 'SetVignette', false, "0 0 0 1");
}

function SurvivorPlayerMed::onNewDataBlock(%this,%obj)
{	
	Parent::onNewDataBlock(%this,%obj);
	%obj.BrickScanCheck();

	if(isObject(%obj.client.deathMusic))
	%obj.client.deathMusic.delete();

	if(!%obj.hIsImmune && %obj.getdamagelevel() <= 0)
	commandToClient(%obj.client, 'SetVignette', false, "0 0 0 1");
}
		
function SurvivorPlayerLow::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	%obj.BrickScanCheck();

	if(isObject(%obj.client.deathMusic))
	%obj.client.deathMusic.delete();

	if(!%obj.hIsImmune && %obj.getdamagelevel() <= 0)
	commandToClient(%obj.client, 'SetVignette', false, "0 0 0 1");
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
	
	%obj.client.deathMusic = new AudioEmitter("DeathMusic")
	{
		profile = "musicdata_L4D_death";
		isLooping = false;
        position = "9e9 9e9 9e9";
        volume = 1;
        type = 10;
        useProfileDescription = false;
        is3D = false;
	};
	%obj.client.deathMusic.setNetFlag(6, true);
	%obj.client.deathMusic.scopeToClient(%obj.client);

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
		%obj.oxygenCount = mClamp(%obj.oxygenCount++, 0, 6);	

		if(%obj.oxygenCount == 6)
		%obj.Damage(%obj, %obj.getPosition (), 25, $DamageType::Suicide);
		
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

function SurvivorPlayerDowned::ondisabled(%this,%obj)
{	
	%obj.playaudio(0,"survivor_death" @ getRandom(1, 8) @ "_sound");
	commandToClient(%obj.client,'SetVignette',$EnvGuiServer::VignetteMultiply,$EnvGuiServer::VignetteColor);

	%obj.client.deathMusic = new AudioEmitter("DeathMusic")
	{
		profile = "musicdata_L4D_death";
		isLooping = false;
        position = "9e9 9e9 9e9";
        volume = 1;
        type = 10;
        useProfileDescription = false;
        is3D = false;
	};
	%obj.client.deathMusic.setNetFlag(6, true);
	%obj.client.deathMusic.scopeToClient(%obj.client);

	Parent::onDisabled(%this, %obj);
}

function SurvivorPlayerDowned::onNewDataBlock(%this,%obj)
{
	if(isObject(%obj.client.deathMusic)) %obj.client.deathMusic.delete();
	
	if(%obj.getClassName() $= "Player")
	{
		chatMessageTeam(%obj.client,'fakedeathmessage',"<color:FFFF00><bitmapk:add-ons/gamemode_left4block/add-ins/player_survivor/icons/downci>" SPC %obj.client.name);

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