// ============================================================
// 1. Main Package
// ============================================================
package L4B2Bots_Main
{	
	function serverLoadSaveFile_End()
	{
		parent::serverLoadSaveFile_End();

	    %count = $LoadingBricks_BrickGroup.getCount();
        if(%count < 1) 
		return;
        
		for(%i=0;%i<%count;%i++)
        {
			%brick = $LoadingBricks_BrickGroup.getObject(%i);

			if(%brick.getdataBlock().IsTeleBrick)
			{
				if(strstr(%brick.getName(),"SetCheck") != -1)
				{
					%bricknamefix1 = strreplace(%brick.getName(), "_TeleventSetCheck", "");
					%bricknamefix2 = strreplace(%bricknamefix1, "_", "");
					%name = %bricknamefix2 @ "_TeleventSet";

					%brick.teleset = new SimSet(%name);
					%brick.teleset.ParBrick = %brick;
					%mainbrick = %brick;
					checkTeleAndSubCheckBricks(%mainbrick,%name);
				}
			}
		}
	}

	function AIPlayer::hMeleeAttack(%obj,%col)
	{		
		if(%obj.getState() $= "Dead")
		return;

		if(%col.getType() & $TypeMasks::VehicleObjectType || %col.getType() & $TypeMasks::PlayerObjectType)
		{
			if(%obj.hState $= "Following" || %obj.Distraction )//Make sure it can damage even if it has a distraction it's following
			{
				if(isObject(%col.getmountedimage(0)) && %col.getMountedImage(0) == RiotShieldimage.getID())
				{
					%reflect = (vectorDot(%col.getForwardVector(), %obj.getForwardVector()) < 0);
					if(%reflect)
					{
						%obj.playthread(2,activate2);
						serverPlay3d("riotshield_block_sound",%col.getposition());
						return;
					}
					else
					{
						if(%obj.getDataBlock().hMelee $= "2")
						%obj.getDataBlock().onBotMelee(%obj,%col);//Used for L4B zombie bots
						%obj.playthread(2,activate2);

						%damage = %obj.hAttackDamage*getWord(%obj.getScale(),0);
						%damagefinal = getRandom(%damage/4,%damage);

						%col.damage(%obj.hFakeProjectile, %col.getposition(), %damagefinal, %obj.hDamageType);

						%obj.hlastmeleedamage = %damagefinal;
						%obj.lastattacked = getsimtime()+1000;
					}
				}
				else
				{
					if(%obj.getDataBlock().hMelee $= "2")
					%obj.getDataBlock().onBotMelee(%obj,%col);//Used for L4B zombie bots
					%obj.playthread(2,activate2);

					%damage = %obj.hAttackDamage*getWord(%obj.getScale(),0);
					%damagefinal = getRandom(%damage/4,%damage);

					%col.damage(%obj.hFakeProjectile, %col.getposition(), %damagefinal, %obj.hDamageType);

					%obj.hlastmeleedamage = %damagefinal;
					%obj.lastattacked = getsimtime()+1000;
				}
			}
		}
	}

	function checkTeleAndSubCheckBricks(%mainbrick,%name)
	{
		%count = $LoadingBricks_BrickGroup.getCount();
		for(%i=0;%i<%count;%i++)
        {
			%brick = $LoadingBricks_BrickGroup.getObject(%i);
			if(%brick.getdataBlock().IsTeleBrick && strstr(%brick.getname(),%name) != -1)
			{
				if(strstr(%brick.getName(),"SetBrick") != -1)
				%mainbrick.teleset.add(%brick);

				if(strstr(%brick.getName(),"SetSubCheck") != -1)
				%brick.teleset = %mainbrick.teleset;
			}
		}
	}
	
	function fxDTSBrick::onActivate (%obj, %player, %client, %pos, %vec)
	{
		if(%obj.getdataBlock().IsTeleBrick && %obj.getgroup().bl_id == %player.client.bl_id)
		{
			if(strstr(%obj.getName(),"TeleventSetCheck") != -1 || strstr(%obj.getName(),"TeleventSetSubCheck") != -1)
			if(%player.client.currTeleSet !$= %obj.TeleSet)
			{
				%player.client.currTeleSet = %obj.TeleSet;
				%player.client.centerprint("\c2Set current checker <br>\c2" @ %obj.teleset.ParBrick,3);
			}
		}
		Parent::onActivate (%obj, %player, %client, %pos, %vec);
	}	
	
	function Armor::onImpact(%this, %obj, %col, %vec, %force)
	{
		if(%force < 40)
		serverPlay3D("impact_medium" @ getRandom(1,3) @ "_sound",%obj.getPosition());
		else serverPlay3D("impact_hard" @ getRandom(1,3) @ "_sound",%obj.getPosition());
		Parent::onImpact(%this, %obj, %col, %vec, %force);
	}

	function Projectile::onAdd(%obj)
	{
		if(%obj.getdataBlock().isDistraction)
		%obj.schedule(%obj.getDataBlock().distractionDelay,%obj.getDataBlock().distractionFunction,0);

		Parent::onAdd(%obj,%datablock);
	}

	function Observer::onTrigger(%this, %obj, %a, %b)
	{
		%client = %obj.getControllingClient();

		if(isObject(%client.player) && %client.player.isBeingStrangled)
		{
			if(%b)
			%client.player.activateStuff();	
			return;
		}

		Parent::onTrigger(%this, %obj, %a, %b);
	}

	function fxDTSBrickData::onPlayerTouch(%data,%obj,%player)
	{
		Parent::onPlayerTouch(%data,%obj,%player);
	
		if(%player.getDatablock().getName() !$= "ZombieTankHoleBot")
		{
			if(%player.hZombieL4BType)
			%obj.processInputEvent ("onZombieTouch");
		}
		else %obj.processInputEvent ("onTankTouch");	
	}

	function ProjectileData::damage(%this,%obj,%col,%fade,%pos,%normal) //Blocking damage. Original script by Space Guy. I've gutted out a few major bits and added in my own.
	{		
		if(%col.getType() & $TypeMasks::PlayerObjectType) //If they aren't holding a shield or carrying one on their back...
		{
			if(%col.getMountedImage(0) != RiotShieldimage.getID())
			return Parent::damage(%this,%obj,%col,%fade,%pos,%normal); //Play it normally and leave.
			else
			{
				%aimVec = %col.getForwardVector();
				%reflect = ((vectorDot(vectorNormalize(%obj.getVelocity()),%aimVec) < 0 && %col.getMountedImage(0) == RiotShieldimage.getID())); //If the shield is facing the projectile...

				if(!%reflect)
				return Parent::damage(%this,%obj,%col,%fade,%pos,%normal);
				else 
				{
					serverPlay3d("riotshield_block_sound",%pos); //Play a sound.
					return;
				}
			}
		}
		Parent::damage(%this,%obj,%col,%fade,%pos,%normal); //Play it normally and leave.
	}

	function ProjectileData::onCollision (%this, %obj, %col, %fade, %pos, %normal, %velocity)
	{
		if(%this.directDamage && %col.getType() & $TypeMasks::PlayerObjectType)
		{
			if(!%obj.sourceObject.hIsInfected && %col.isBeingStrangled && %col.hEater.getDataBlock().getName() $= "ZombieSmokerHoleBot")
			{
				%col.isBeingStrangled = 0;
				%col.hEater.SmokerTongueTarget = 0;
				L4B_SpecialsPinCheck(%col.hEater,%col);
				%col.hEater.damage(%obj, %pos, %this.directDamage/2, %this.directDamageType);
			}
		}		
		
		if(%col.getType() & $TypeMasks::PlayerObjectType) //If they aren't holding a shield or carrying one on their back...
		{
			if(%col.getMountedImage(0) != RiotShieldimage.getID())
			return Parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity); //Play it normally and leave.
			else
			{
				%aimVec = %col.getForwardVector();
				%reflect = ((vectorDot(vectorNormalize(%obj.getVelocity()),%aimVec) < 0 && %col.getMountedImage(0) == RiotShieldimage.getID())); //If the shield is facing the projectile...

				if(!%reflect)
				return Parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
				else 
				{
					serverPlay3d("riotshield_block_sound",%pos); //Play a sound.
					return;
				}
			}
		}

		Parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
	}

	function serverCmdUseTool(%client, %tool)
	{		
		if(!%client.player.isBeingStrangled)
		return parent::serverCmdUseTool(%client, %tool);
	}

	function ServerCmdDropTool (%client, %position)
	{
		if(isObject(%client.player))
		%client.player.playthread(3,"activate");

		Parent::ServerCmdDropTool (%client, %position);
	}

	function ServerCmdPlantBrick (%client)
	{
		Parent::ServerCmdPlantBrick (%client);

		if(isObject(%obj = %client.player) && %obj.hIsInfected && %obj.spawnTime+5000 > getSimTime())
		{
			%client.setZombieBlock = 1;
			%client.spawnPlayer();
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

	function applyCharacterPrefs(%client)
	{	
		if(!isObject(%client.Player) || %client.player.getdatablock().getName() $= "ZombieTankHoleBot")
		return;

		%client.applyBodyParts();
		%client.applyBodyColors();
	}

	function gameConnection::applyBodyColors(%cl,%o) 
	{
		if(isObject(%cl.player) && %cl.player.getdataBlock().getName() !$= "ZombieTankHoleBot")
		parent::applyBodyColors(%cl,%o);
		
		if(isObject(%pl = %cl.player))
		if((%pl.getDatablock()).shapeFile $= "base/data/shapes/player/mmelee.dts")
		%pl.fixAppearance(%cl);
	}

	function gameConnection::applyBodyParts(%cl,%o) 
	{
		if(isObject(%cl.player) && %cl.player.getdataBlock().getName() !$= "ZombieTankHoleBot")
		parent::applyBodyParts(%cl,%o);
		
		if(isObject(%pl = %cl.player))
		if((%pl.getDatablock()).shapeFile $= "base/data/shapes/player/mmelee.dts")
		%pl.fixAppearance(%cl);
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
		if(%cl.pack+%cl.secondPack > 0)
			%pl.setHeadUp(1);
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
	
		if (%pl.bloody["lshoe"])
			%pl.unHideNode("lshoe_blood");
		if (%pl.bloody["rshoe"])
			%pl.unHideNode("rshoe_blood");
		if (%pl.bloody["lhand"])
			%pl.unHideNode("lhand_blood");
		if (%pl.bloody["rhand"])
			%pl.unHideNode("rhand_blood");
		if (%pl.bloody["chest_front"])
			%pl.unHideNode((%cl.chest ? "fem" : "") @ "chest_blood_front");
		if (%pl.bloody["chest_back"])
			%pl.unHideNode((%cl.chest ? "fem" : "") @ "chest_blood_back");
	
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
	
		//Set blood colors.
		%pl.setNodeColor("lshoe_blood", "0.7 0 0 1");
		%pl.setNodeColor("rshoe_blood", "0.7 0 0 1");
		%pl.setNodeColor("lhand_blood", "0.7 0 0 1");
		%pl.setNodeColor("rhand_blood", "0.7 0 0 1");
		%pl.setNodeColor("chest_blood_front", "0.7 0 0 1");
		%pl.setNodeColor("chest_blood_back", "0.7 0 0 1");
		%pl.setNodeColor("femchest_blood_front", "0.7 0 0 1");
		%pl.setNodeColor("femchest_blood_back", "0.7 0 0 1");
	
		if(%pl.getDataBlock().hCustomNodeAppearance)
		%pl.getDataBlock().hCustomNodeAppearance(%pl);
	
		if(isObject(%cl) && %cl.isAdmin || %cl.isSuper)
		{
			%cl.player.unHideNode("shades");
			%cl.player.setNodeColor("shades","0.1 0.1 0.1 1");
		}

		if($Pref::Server::L4B2Bots::CustomStyle < 2 && %pl.hIsInfected)
		{
			%pl.unhidenode("gloweyes");
			%pl.setnodeColor("gloweyes","1 1 0 1");
		}

		if(isObject(%pl) && %pl.getClassName() $= "Player" && %pl.getDataBlock().hType $= "zombie")
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

			%pl.MeleePlayerBloodify(50);

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

	function minigameCanDamage(%objA, %objB)
	{	
		if(%objA.getclassname() $= "GameConnection" || %objA.getclassname() $= "Player" || %objA.getclassname() $= "AIPlayer")
		if(%objB.getclassname() $= "Player" || %objB.getclassname() $= "AIPlayer")
		{
			if(%objA !$= %objB && %objA.player !$= %objB)
			if(%objA.hType $= %objB.hType || %objA.player.hType $= %objB.hType)
			return;
		}

		Parent::minigameCanDamage(%objA, %objB);
	}

	function MiniGameSO::checkLastManStanding(%minigame)
	{
		if(%minigame.RespawnTime > 0 || isEventPending (%minigame.resetSchedule))
		return;

		for(%i = 0; %i < %minigame.numMembers; %i++)
		{
			%client = %minigame.member[%i];

			if(isObject(%player = %client.player) && !%player.hIsInfected && %player.getdataBlock().getname() !$= "DownPlayerSurvivorArmor")
			%livePlayerCount++;
		}

		if(%livePlayerCount <= 0)
		{
			if(isObject(l4b_music)) 
			l4b_music.delete();
			%minigame.DirectorProcessEvent("onSurvivorsLose",%client);
			%minigame.L4B_PlaySound("game_lose_sound");
			%minigame.scheduleReset(12000);
		}
	}

    function MiniGameSO::Reset(%minigame,%client)
	{
		Parent::Reset(%minigame,%client);

		%minigame.L4B_PlaySound("game_start_sound");

        %directorinterval = $Pref::L4BDirector::Director_Interval*1000;
        %directorintervalhalf = %directorinterval/2;

        if(isObject(l4b_music)) 
        l4b_music.delete();

        if($Pref::L4BDirector::EnableCues)
        {
            if(isObject(%minigame.member0.player))
            %pos = %minigame.member0.player.getPosition();
            else %pos = "0 0 0";

            %musicnum = "musicdata_L4D_safearea";
            new AudioEmitter(l4b_music)
            {
                position = %pos;
                profile = %musicnum.getID();
                isLooping= true;
                is3D = 0;
                volume = 0.75;
			    useProfileDescription = "0";
			    type = "0";
			    outsideAmbient = "1";
			    referenceDistance = "2";
			    maxDistance = 999999;
			    enableVisualFeedback = "0";
            };
        }

        if($Pref::L4BDirector::EnableOnMG)
        {
            %minigame.isDirectorEnabled = 1;
            cancel(%minigame.directorSchedule);

            cancel(directorSpecialSchedule);
            %minigame.directorSpecialSchedule = %minigame.schedule($Pref::L4BDirector::Director_Interval/2,directorSpecial,%client);

            %minigame.schedule(%directorinterval,directorChoose,%client);
            %minigame.schedule(%directorintervalhalf,directorSpecial,%client);

            if($Pref::L4BDirector::EnableCues)
            %minigame.directorPlaySound("hordeincoming" @ getrandom(1,9) @ "_sound",%client);

            if($Pref::L4BDirector::AllowMGMessages)
            MiniGameSO::ChatMsgAll (%minigame, "\c3Director activated.", %client);

            if(isObject(l4b_music)) 
            l4b_music.delete();
        }
        else
        {
            cancel(directorSpecialSchedule);
            %minigame.isDirectorEnabled = 0;
            cancel(%minigame.directorSchedule);
        }
        
		%minigame.DirectorTankRound = 0;
        cancel(%minigame.hordeMusic);
        cancel(%minigame.hordeMusic1);
        cancel(%minigame.hordeMusic2);
        cancel(%minigame.TriggerHordeEndEvent);
	}

    function MiniGameSO::endGame(%minigame)
    {
        Parent::endGame(%minigame);
        if(isObject(l4b_music)) 
        l4b_music.delete();
    }
};

if(isPackage(BotHolePackage))
{
	deactivatePackage(BotHolePackage);
	activatePackage(BotHolePackage);
	activatePackage(L4B2Bots_Main);
}

// ============================================================
// 2. New Zombie Infection
// ============================================================

package L4B_ZombieInfection
{
	function Armor::onNewDatablock(%this, %obj)
	{		
		Parent::onNewDatablock(%this, %obj);

		if(%this.hType !$= "Zombie")
		{
			%obj.hZombieL4BType = 0;
			%obj.hIsInfected = 0;

			if(%obj.getdatablock().hType)
			%obj.hType = %obj.getdatablock().hType;

			if(isObject(%obj.client))
			commandToClient( %obj.client, 'SetVignette', $EnvGuiServer::VignetteMultiply, $EnvGuiServer::VignetteColor );
		}
	}

	function holeZombieInfect(%obj, %col)
	{			
		if(%col.getDataBlock().shapeFile $= "base/data/shapes/player/m.dts" || %col.getDataBlock().shapeFile $= "base/data/shapes/player/mmelee.dts")
		{
			%col.setDataBlock(CommonZombieHoleBot);

			switch$(%col.getclassname())
			{
				case "AIPlayer":%col.hChangeBotToInfectedAppearance();

				case "Player": %minigame = getMinigameFromObject(%col);
							   %minigame.L4B_PlaySound("survivor_turninfected" @ getRandom(1,3) @ "_sound",%col.client);
							   %minigame.checkLastManStanding();
			}
		}
		else if(%col.getDataBlock().shapeFile $= "Add-Ons/Bot_Shark/shark.dts")
		{
			%col.setnodecolor("chest", %newskincolor);
			%col.setnodecolor("head", %newskincolor);
		}
		else %col.setnodecolor("ALL", %newskincolor);
	}

};
activatePackage(L4B_ZombieInfection);

if(isPackage(holeZombiePackage))
deactivatePackage(holeZombiePackage);

// ============================================================
// 4. Flash Grenade Support
// ============================================================

package L4B_FlashGrenadeSupport
{
	function flashGrenadeProjectile::onExplode(%this,%obj)
	{
	   parent::onExplode(%this, %obj);
	   initContainerRadiusSearch(%obj.getPosition(),30,$TypeMasks::PlayerObjectType);
	   while((%target = ContainerSearchNext()) != 0)
	   {
	      if(!isObstruction(%obj.getPosition(),%target) && isObject(getMinigameFromObject(%obj,%target)))
	      {
				%angle = calculateAngle(%target,%obj.getPosition());
				if(%angle < 100 && %angle > -100 || %angle > -360 && %angle < -260 || %angle < 360 && %angle > 260)
				{
					if(%angle < 0)
					%angle = mAbs(%angle);

					if(%angle > 180)
					%angle = %angle/6;

					if(%angle < 10)
					%start = 2;
					else
					%start = mFloatLength(1/%angle*25,4);

					if(%start < 0.2)
					%start = 0.2;
					%target.setWhiteout(%start);
					%sched = %start*1000;

					setTime(%target.client,0.2);
					%target.timeSched = schedule(%sched,0,"setTime",%target.client,1);
				}
			}

			if(%target.isHoleBot && %target.hZombieL4BType && %target.hZombieL4BType < 5)
			{
				%target.stopHoleLoop();
				L4B_SpazzZombieInitialize(%target,0);
			}
		
		}
	}
};
if(LoadRequiredAddOn("Weapon_FlashGrenade") == $Error::None)
{
	activatePackage(L4B_FlashGrenadeSupport);
	eval("flashgrenadeProjectile.lifetime = 3500;");
	eval("flashgrenadeProjectile.fadeDelay = 4000;");
	eval("flashgrenadeProjectile.armingDelay = 3500;");
	eval("flashgrenadeProjectile.bounceElasticity = 0.25;");
	eval("flashgrenadeProjectile.isDistraction = 1;");
	eval("flashgrenadeProjectile.distractionLifetime = 3;");
	eval("flashgrenadeProjectile.distractionDelay = 0;");
	eval("flashgrenadeProjectile.DistractionFunction = BileBombDistract;");
	eval("flashgrenadeProjectile.DistractionRadius = 50;");
}

// ============================================================
// 5. Flare Gun Support
// ============================================================

package L4B_FlareGunSupport
{
	function flareGunProjectile::onCollision(%db,%proj,%hit,%fade,%pos,%normal)
	{	
		if(%hit.hZombieL4BType)
		%hit.flamer_burnStart(4);

		%pos = %proj.getPosition();
		%radius = 1000;
	    %searchMasks = $TypeMasks::PlayerObjectType;
	    InitContainerRadiusSearch(%pos, %radius, %searchMasks);
	    while((%targetid = containerSearchNext()) != 0 )
	    {
			if(%targetid.getClassName() $= "AIPlayer" && %targetid.hZombieL4BType !$= 5 && !%targetid.isBurning)
			{
				%targetid.Distraction = 0;
				%targetid.hSearch = 1;
	    	}
	    }
	    cancel(%proj.ContinueSearch);
	
		parent::onCollision(%db,%proj,%hit,%fade,%pos,%normal);
	}

};
if(LoadRequiredAddOn("Weapon_SWeps_FlareGun") == $Error::None && LoadRequiredAddOn("Weapon_SWeps") == $Error::None)
{
	activatePackage(L4B_FlareGunSupport);
	eval("flareGunProjectile.isDistraction = 1;");
	eval("flareGunProjectile.distractionLifetime = 10;");
	eval("flareGunProjectile.distractionDelay = 500;");
	eval("flareGunProjectile.DistractionFunction = BileBombDistract;");
	eval("flareGunProjectile.DistractionRadius = 1000;");
}

// ============================================================
// 6. SWeps Flames
// ============================================================

package L4B_SWepFlamesBurnsZombs
{
	function player::flamer_burnStart(%pl,%tick)
	{
		if(!%pl.getDatablock().noBurning)
		Parent::flamer_burnStart(%pl,%tick);
	}

	function player::flamer_burn(%pl,%tick)
	{
		if(%pl.getDataBlock().hType $= "Zombie")
		{
			cancel(%pl.flamerClearBurnSched);
			%pl.isBurning = 1; 

			cancel(%pl.burnSched);
			if(!isObject(%pl.getMountedImage(3)))
			%pl.mountImage(flamerFleshBurningImage,3);
			
			if(!%pl.isPlayingBurningSound)
			{
				%pl.playAudio(3,fleshFireLoopSound);
				%pl.isPlayingBurningSound = 1;
			}
			
			%dmg = mClamp(%pl.getdataBlock().maxDamage/25,10,%pl.getdataBlock().maxDamage);
			if(%pl.isCrouched())
			%dmg *= 0.47619;
			if(!%pl.noFireBurning)
			{
				%pl.damage(%pl.lastFireAttacker,%pl.getPosition(),%dmg,%pl.lastBurnDmgType);
				if(%pl.getclassname() $= "AIPlayer" && %pl.hZombieL4BType && %pl.hZombieL4BType < 5)
				{
					%pl.hRunAwayFromPlayer(%pl);
					%pl.stopHoleLoop();
				}
				%pl.playThread(2,plant);
			}
		
			%pl.burnSched = %pl.schedule(500,flamer_burn,%tick);
		}
		else
		Parent::flamer_burn(%pl,%tick);
	}

	function molotov_explode(%pos,%obj,%cl)
	{
		Parent::molotov_explode(%pos,%obj,%cl);

		//for (%n = 0; %n < 2; %n++)
		//schedule(3500 * %n, 0, createFireCircle, %pos,30,40,%cl,%obj,$DamageType::Molotov);
	}

	function flamerProjectile::damage(%this,%obj,%col,%fade,%pos,%normal)
	{
		if(!%col.getDatablock().noBurning)
		Parent::damage(%this,%obj,%col,%fade,%pos,%normal);
	}

	function molotovProjectile::damage(%this,%obj,%col,%fade,%pos,%normal)
	{
		if(!%col.getDatablock().noBurning)
		Parent::damage(%this,%obj,%col,%fade,%pos,%normal);	
	}
};
if(LoadRequiredAddOn("Weapon_SWeps_EXT") == $Error::None)
activatePackage(L4B_SWepFlamesBurnsZombs);

// ============================================================
// 12. Explosive Brick Group Object
// ============================================================

if(LoadRequiredAddOn("Weapon_SWeps_EXT") == $Error::None)
$Item_Explosives_isSWepsExtOn = 1;
else $Item_Explosives_isSWepsExtOn = 0;

package ExplosivesBrickGroupToObject
{
    function getBrickGroupFromObject(%obj)
	{	
		if (!isObject (%obj))
		{
			error ("ERROR: getBrickGroupfromObject() - \"" @ %obj @ "\" is not an object");
			return -1;
		}
		%brickGroup = -1;
		if (%obj.getClassName () $= "GameConnection")
		{
			%brickGroup = %obj.brickGroup;
		}
		else if (%obj.getClassName () $= "SimGroup")
		{
			%brickGroup = %obj;
		}
		else if (%obj.getType () & $TypeMasks::PlayerObjectType)
		{
			if (isObject (%obj.client))
			{
				%brickGroup = %obj.client.brickGroup;
			}
			else if (isObject (%obj.spawnBrick))
			{
				%brickGroup = %obj.spawnBrick.getGroup ();
			}
		}
		else if (%obj.getType () & $TypeMasks::ItemObjectType)
		{
			if (isObject (%obj.spawnBrick))
			{
				%brickGroup = %obj.spawnBrick.getGroup ();
			}
			else 
			{
				%brickGroup = "BrickGroup_" @ %obj.bl_id;
			}
		}
		else if (%obj.getType () & $TypeMasks::FxBrickAlwaysObjectType)
		{
			%brickGroup = %obj.getGroup ();
		}
		else if (%obj.getType () & $TypeMasks::VehicleObjectType)
		{
			if(isObject(%obj.spawnBrick))
			%brickGroup = %obj.spawnBrick.getGroup();

			if(isObject(%obj.brickGroup))
			%brickGroup = %obj.brickGroup;
		}
		else if (%obj.getType () & $TypeMasks::ProjectileObjectType)
		{
			if (isObject (%obj.client))
			{
				%brickGroup = %obj.client.brickGroup;
			}
		}
		else 
		{
			if (isObject (%obj.spawnBrick))
			{
				%brickGroup = %obj.spawnBrick.getGroup ();
			}
			if (isObject (%obj.client))
			{
				%brickGroup = %obj.client.brickGroup;
			}
			if (%obj.getGroup ().bl_id !$= "")
			{
				%brickGroup = %obj.getGroup ();
			}
		}
		return %brickGroup;
    }
};
activatePackage(ExplosivesBrickGroupToObject);

// ============================================================
// 13. Player Name and Appearance Logger
// ============================================================

package L4B_ClientLogger
{
	function GameConnection::onClientEnterGame(%this)
	{
		parent::onClientEnterGame(%this);
		for(%i = 0; %i < $L4B_clientLog.getCount(); %i++)
		{
			if($L4B_clientLog.getObject(%i).blid $= %this.getBLID())
			{
				return;
			}
		}
		L4B_createClientSnapshot(%this);
		L4B_storeLoggedClients();
	}
};
activatePackage(L4B_ClientLogger);