// ============================================================
// 1. Main Package
// ============================================================

package L4B2Bots_Main
{
	function onObjectCollisionTest(%obj, %col)
	{
		if(!isObject(%obj)|| !isObject(%col))
		return;
	
		%oscale = getWord(%obj.getScale(),2);
		%force = vectorDot(%obj.getVelocity(), %obj.getForwardVector());
		
		if(%obj.getType() & $TypeMasks::PlayerObjectType && %col.getType() & $TypeMasks::PlayerObjectType) 
		{
			if(%obj.getdataBlock().getName().isSurvivor && %col.getdataBlock().getName().isSurvivor)
			return false;
			
			if(%obj.getdataBlock().getName() $= "ZombieChargerHoleBot" && vectordist(%obj.getposition(),%col.getposition()) < 6)
			if(%col.getdataBlock().getName() !$= "ZombieTankHoleBot" && %oScale >= 1.1 && %force > 20 && %obj.hEating != %col)
			{
				if(%col.getdatablock().getName() !$= "ZombieChargerHoleBot")
				{				
					%obj.playaudio(3,"charger_smash_sound");			
					%forcecalc = %force/20;
					%obj.spawnExplosion(pushBroomProjectile,%forcecalc SPC %forcecalc SPC %forcecalc);
					%obj.playthread(2,"activate2");
					%normVec = VectorNormalize(vectorAdd(%obj.getForwardVector(),"0" SPC "0" SPC "0.25"));
					%eye = vectorscale(%normVec,%force/2);
					%col.setvelocity(%eye);
	
					if(checkHoleBotTeams(%obj,%col))
					%col.damage(%obj.hFakeProjectile, %col.getposition(),0.25, %obj.hDamageType);
				}
				return false;
			}
		}
		return true;
	}

	function Player::ActivateStuff (%player)
	{
		%client = %player.client;
		%player.lastActivated = 0;

		if(isObject(%client.miniGame) && %client.miniGame.WeaponDamage && getSimTime() - %client.lastF8Time < 5000 || %player.getDamagePercent () >= 1)
		return 0;	

		%start = %player.getEyePoint ();
		%vec = %player.getEyeVector ();
		%scale = getWord (%player.getScale (), 2);
		%end = VectorAdd (%start, VectorScale (%vec, 10 * %scale));
		%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::PlayerObjectType;

		if(%player.isMounted())
		%exempt = %player.getObjectMount();
		else %exempt = %player;

		%search = containerRayCast (%start, %end, %mask, %exempt);
		%victim = getWord(%search, 0);

		if(getSimTime() - %player.lastActivateTime <= 320)
		%player.activateLevel += 1;
		else %player.activateLevel = 0;
		
		%player.lastActivateTime = getSimTime ();

		if(%player.activateLevel >= 5)
		%player.playThread (3, activate2);
		else %player.playThread (3, activate);
		
		if(%victim)
		{
			%pos = getWords (%search, 1, 3);
			if(%victim.getType () & $TypeMasks::FxBrickObjectType)
			{
				%diff = VectorSub (%start, %pos);
				%len = VectorLen (%diff);
				if(%len <= $Game::BrickActivateRange * %scale)
				%victim.onActivate (%player, %client, %pos, %vec);
				
			}
			else if(isFunction(%victim, onActivate)) %victim.onActivate (%player, %client, %pos, %vec);

			%player.lastActivated = %victim;
			return %victim;
		}
		else return 0;
		
	}

	function player::PutItemInSlot(%obj,%slot,%item)
	{
		if(%obj.tool[%slot] == 0)
		{
			%obj.tool[%slot] = %item.getDataBlock();
			messageClient(%obj.client,'MsgItemPickup','',%slot,%item.getDataBlock());

			if(isObject(%brick = %item.spawnBrick) && strstr(strlwr(%brick.getName()), "nodis") != -1)
			{
				%item.fadeOut();
				%item.respawn();
			}
			else %item.delete();

			return true;
		}
		else return false;
	}

	function player::pickup(%obj,%item)
	{		
		if(%obj.getDatablock().isSurvivor)
		{
			if(!isObject(%item) || !%item.canPickup || !miniGameCanUse(%obj,%item))
			return;
			else if(%item.getdatablock().throwableExplosive)
			{
				ServerCmdUnUseTool(%obj.client);
				%obj.mountImage(%item.getdatablock().image, 0);
				
				%item.delete();
				return;
			}

			if(%item.getdataBlock().L4Bitemnoslot)
			return Parent::pickup(%obj,%item);

			switch$(%item.getdataBlock().L4Bitemslot)
			{			            
        	    case "Secondary": %obj.PutItemInSlot(1,%item);
								  return;
				case "Grenade": %obj.PutItemInSlot(2,%item);
							  	return;
				case "Medical": %obj.PutItemInSlot(3,%item);
							  	return;
				case "Medical_Secondary": %obj.PutItemInSlot(4,%item);
							  			  return;
				default: %obj.PutItemInSlot(0,%item);
						 return;
			}
		}

		Parent::pickup(%obj,%item);
	}

	function fxDTSBrickData::onPlayerTouch(%data,%obj,%player)
	{
		Parent::onPlayerTouch(%data,%obj,%player);

		if(%player.getDatablock().isSurvivor) %obj.processInputEvent("onSurvivorTouch");
		
		if(%player.hZombieL4BType !$= "") 
		{
			%obj.processInputEvent("onZombieTouch");
			if(%player.getDatablock().getName() $= "ZombieTankHoleBot") %obj.processInputEvent("onTankTouch");
		}
			
	}	
	
	function Armor::onImpact(%this, %obj, %col, %vec, %force)
	{
		if(%force < 40)
		serverPlay3D("impact_medium" @ getRandom(1,3) @ "_sound",%obj.getPosition());
		else serverPlay3D("impact_hard" @ getRandom(1,3) @ "_sound",%obj.getPosition());

		Parent::onImpact(%this, %obj, %col, %vec, %force);
	}

	function Armor::onNewDatablock(%this, %obj)
	{		
		Parent::onNewDatablock(%this, %obj);

		if(%this.hType !$= "Zombie")
		{
			%obj.hZombieL4BType = "";
			%obj.hIsInfected = "";

			if(%obj.getdatablock().hType !$= "") %obj.hType = %obj.getdatablock().hType;
			if(isObject(%obj.client)) commandToClient( %obj.client, 'SetVignette', $EnvGuiServer::VignetteMultiply, $EnvGuiServer::VignetteColor);
		}

		if(isObject(%hEater = %obj.hEater) && %hEater.getDataBlock().getName() $= "ZombieJockeyHoleBot") %obj.mountObject(hEater,2);
	}	
	
	function Armor::onBotMelee(%obj,%col)
	{
		//stuff here
	}

	function Armor::hCustomNodeAppearance(%obj,%col)
	{
		//stuff here
	}

	function AIPlayer::hMeleeAttack(%obj,%col)
	{						
		if(%obj.getState() $= "Dead") return;

		if(%col.getType() & $TypeMasks::VehicleObjectType || %col.getType() & $TypeMasks::PlayerObjectType)
		{
			if(%obj.hState $= "Following" || %obj.Distraction)
			{
				%damage = %obj.hAttackDamage*getWord(%obj.getScale(),0);
				%damagefinal = getRandom(%damage/4,%damage);
				%obj.hlastmeleedamage = %damagefinal;
				%obj.lastattacked = getsimtime()+1000;

				%col.damage(%obj.hFakeProjectile, %col.getposition(), %damagefinal, %obj.hDamageType);
				%obj.getDataBlock().onBotMelee(%obj,%col);
				%obj.playthread(2,activate2);
			}
		}
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

	function fxDTSBrick::onActivate (%obj, %player, %client, %pos, %vec)
	{
		if(%obj.getdataBlock().IsZoneBrick && %obj.getgroup().bl_id == %player.client.bl_id)
		{
			if(strstr(strlwr(%obj.getName()),"_main") != -1)
			if(%player.client.currAreaZone !$= %obj.AreaZone)
			{
				%player.client.currAreaZone = %obj.AreaZone;
				%player.client.centerprint("\c2Set current checker <br>\c2" @ %obj.AreaZone.ParBrick,3);

				%removenoctnum = getSubStr(%obj.getName(), 0, strstr(strlwr(%obj.getName()),"_ct")+3);
				%num = strreplace(%obj.getName(), %removenoctnum, "");
				%player.client.AZCount = %num;
			}
		}
		Parent::onActivate (%obj, %player, %client, %pos, %vec);
	}		

	function Projectile::onAdd(%obj)
	{
		if(%obj.getdataBlock().isDistraction)
		%obj.schedule(%obj.getDataBlock().distractionDelay,%obj.getDataBlock().distractionFunction,0);

		Parent::onAdd(%obj,%datablock);
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

	function minigameCanDamage(%objA, %objB)
	{
		if(!isObject(%objA) || !isObject(%objB))
		return;

		if(%objA.player !$= %objB)
		{
			if((%objA.getclassname() $= "GameConnection" || %objA.getclassname() $= "Player" || %objA.getclassname() $= "AIPlayer") && (%objB.getclassname() $= "Player" || %objB.getclassname() $= "AIPlayer"))
			if(%objA.hType $= %objB.hType || %objA.player.hType $= %objB.hType)
			return;
		}

		Parent::minigameCanDamage(%objA, %objB);
	}

	function MiniGameSO::checkLastManStanding(%minigame)
	{
		if(%minigame.RespawnTime > 0 || isEventPending(%minigame.resetSchedule))
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
			%minigame.L4B_PlaySound("game_lose_sound");
			%minigame.scheduleReset(12000);
		}
	}

    function MiniGameSO::Reset(%minigame,%client)
	{
		if(isObject(MainAreaZone))
		MainAreaZone.delete();

		if(isObject(L4B_BotSet))
		{
			for(%z = 0; %z < L4B_BotSet.getCount(); %z++)
			{	
				if(isObject(%bot = L4B_BotSet.getObject(%z)))
				%bot.schedule(10,delete);
			}
		}
		
		Parent::Reset(%minigame,%client);

		for(%i = 0; %i < %client.brickgroup.ntobjectcount_breakbrick; %i++)
		{
			if(isObject(%breakbrick = %client.brickgroup.ntobject_breakbrick_[%i]))
			{
				%breakbrick.setRendering(1);
				%breakbrick.setRayCasting(1);
				%breakbrick.setColliding(1);
			}
		}

		for(%i = 0; %i < %client.brickgroup.ntobjectcount_progress_door; %i++)
		{
			if(isObject(%door = %client.brickgroup.ntobject_progress_door_[%i]))
			%door.door(close);
		}

		if(isObject(GlobalAreaZone))
		{
			for(%i = 0; %i < GlobalAreaZone.getCount(); %i++)
			{
				if(isObject(%set = GlobalAreaZone.getObject(%i)))
				{
					%set.firstentry = 0;
					%setlist[%s++] = %set;

					for(%j = 0; %j < %setlist[%s].getCount(); %j++)
					{
						if(isObject(%brick = %setlist[%s].getObject(%j)) && strstr(strlwr(%brick.getname()), "_item") != -1)
						%brick.setItem(none);
					}
				}
			}
		}

		%minigame.L4B_PlaySound("game_start_sound");

        if(isObject(l4b_music)) 
        l4b_music.delete();
        %minigame.DirectorMusic("musicdata_L4D_safearea" @ getRandom(1,4),true,1,%client);

        if($Pref::L4BDirector::EnableOnMG)
        {
            %minigame.DirectorStatus = 1;

            cancel(%minigame.directorSchedule);
            %minigame.schedule(10000,directorAI,%client);

            if($Pref::L4BDirector::EnableCues)
            %minigame.directorPlaySound("hordeincoming" @ getrandom(1,9) @ "_sound",%client);

            if(isObject(l4b_music)) 
            l4b_music.delete();
        }
        else
        {
            %minigame.DirectorStatus = 0;
            cancel(%minigame.directorSchedule);
        }
        
		for(%i=0;%i<%minigame.numMembers;%i++)
		{
			if(isObject(%mgmember = %minigame.member[%i]))
			{
				%aliveplayer++;
				%minigame.survivorStatHealthAverage = 100*%aliveplayer;
				%minigame.survivorStatStressAverage = 0;

				if(isObject(%mgmember.deathMusic))
				%mgmember.deathMusic.delete();
			}
		}

    	%minigame.UrgentRound = 0;
    	%minigame.SoldierTank = 0;
		%minigame.DirectorTankRound = 0;
		cancel(%minigame.spawn["Horde"]);
		cancel(%minigame.spawn["Special"]);
		cancel(%minigame.directorSchedule);
        cancel(%minigame.hordeEndShed);
	}

    function MiniGameSO::endGame(%minigame)
    {
		if(isObject(MainAreaZone))
		MainAreaZone.delete();

		if(isObject(L4B_BotSet))
		{
			for(%z = 0; %z < L4B_BotSet.getCount(); %z++)
			{	
				if(isObject(%bot = L4B_BotSet.getObject(%z)))
				%bot.schedule(10,delete);
			}
		}		
		
		for(%i=0;%i<%minigame.numMembers;%i++)
		{
			if(isObject(%mgmember = %minigame.member[%i]))
			{
				if(isObject(%mgmember.deathMusic))
				%mgmember.l4bmusic[%smc].delete();
			}
		}		
		
		Parent::endGame(%minigame);
    }

	function holeZombieInfect(%obj, %col)
	{			
		if(%col.getDataBlock().shapeFile $= "base/data/shapes/player/m.dts" || %col.getDataBlock().shapeFile $= "base/data/shapes/player/mmelee.dts")
		{		
			switch$(%col.getclassname())
			{
				case "AIPlayer":%col.hChangeBotToInfectedAppearance();

				case "Player": %minigame = getMinigameFromObject(%col);
							   %minigame.L4B_PlaySound("survivor_turninfected_sound",%col.client);
							   %minigame.checkLastManStanding();
							   chatMessageTeam(%col.client,'fakedeathmessage',"<color:00FF00>" @ %obj.getDatablock().hName SPC "<bitmapk:Add-Ons/Gamemode_Left4Block/add-ins/bot_l4b/icons/ci_infected>" SPC %col.client.name);

							   for (%i = 0; %i < %col.getdatablock().maxTools; %i++) 
							   {
									%col.tool[%i] = 0;
									messageClient(%col.client,'MsgItemPickup','',%i,0);
							   }
							   
							   if(isObject(%col.getMountedImage(0)))
							   {
							   		ServerCmdDropTool(%col.client, %col.getHackPosition());
							   		%col.playthread(1,root);

									L4B_ZombieDropLoot(%col,%col.getMountedImage(0).item,100);
							   }
			}
			%col.setDataBlock(CommonZombieHoleBot);
		}
		else if(%col.getDataBlock().shapeFile $= "Add-Ons/Bot_Shark/shark.dts")
		{
			%col.setnodecolor("chest", %newskincolor);
			%col.setnodecolor("head", %newskincolor);
		}
		else %col.setnodecolor("ALL", %newskincolor);
	}
};

if(isPackage(BotHolePackage))
{
	deactivatePackage(BotHolePackage);
	activatePackage(BotHolePackage);
	activatePackage(L4B2Bots_Main);
}

if(isPackage(aeAmmo))
{
	deactivatePackage(aeAmmo);
	activatePackage(L4B2Bots_Main);
	activatePackage(aeAmmo);
}

if(isPackage(holeZombiePackage))
deactivatePackage(holeZombiePackage);//New function was made for this add-on, should disable that old package or things go kablooey