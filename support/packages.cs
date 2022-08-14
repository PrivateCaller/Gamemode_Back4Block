// ============================================================
// 1. Main Package
// ============================================================
package L4B2Bots_Main
{
	function player::pickup(%obj,%item)
	{		
		Parent::pickup(%obj,%item);

		if(isObject(getMiniGameFromObject(%obj)) && %obj.getDatablock().isSurvivor)
		if(isObject(%item) && !%item.canPickup)
		%item.delete();
	}

	function fxDTSBrickData::onPlayerTouch(%data,%obj,%player)
	{
		Parent::onPlayerTouch(%data,%obj,%player);

		if(%player.getDatablock().isSurvivor)
		%obj.processInputEvent ("onSurvivorTouch");

		if(%player.getDatablock().getName() !$= "ZombieTankHoleBot")
		{
			if(%player.hZombieL4BType)
			%obj.processInputEvent ("onZombieTouch");
		}
		else %obj.processInputEvent ("onTankTouch");	
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
			%obj.hZombieL4BType = 0;
			%obj.hIsInfected = 0;

			if(%obj.getdatablock().hType)
			%obj.hType = %obj.getdatablock().hType;

			if(isObject(%obj.client))
			commandToClient( %obj.client, 'SetVignette', $EnvGuiServer::VignetteMultiply, $EnvGuiServer::VignetteColor );
		}
	}	
	
	function Armor::onBotMelee(%obj,%col)
	{
		//
	}

	function AIPlayer::hMeleeAttack(%obj,%col)
	{						
		if(%obj.getState() $= "Dead")
		return;

		if(%col.getType() & $TypeMasks::VehicleObjectType || %col.getType() & $TypeMasks::PlayerObjectType)
		{
			if(%obj.hState $= "Following" || %obj.Distraction)//Make sure it can damage even if it has a distraction it's following
			{
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
			if(strstr(%obj.getName(),"TeleventSetCheck") != -1 || strstr(%obj.getName(),"TeleventSetSubCheck") != -1)
			if(%player.client.currTeleSet !$= %obj.TeleSet)
			{
				%player.client.currTeleSet = %obj.TeleSet;
				%player.client.centerprint("\c2Set current checker <br>\c2" @ %obj.teleset.ParBrick,3);
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
		if(%objA.player != %objB)
		{
			if(%objA.getclassname() $= "GameConnection" || %objA.getclassname() $= "Player" || %objA.getclassname() $= "AIPlayer" || %objB.getclassname() $= "Player" || %objB.getclassname() $= "AIPlayer")
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
			%minigame.L4B_PlaySound("game_lose_sound");
			%minigame.scheduleReset(12000);
		}
	}

    function MiniGameSO::Reset(%minigame,%client)
	{
		Parent::Reset(%minigame,%client);
		L4B_DifficultyAdjustment();

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
			{
				%door.door(close);
			}
		}

		if(isObject(GlobalTeleSet))
		{
			for(%i = 0; %i < GlobalTeleSet.getCount(); %i++)
			{
				if(isObject(%set = GlobalTeleSet.getObject(%i)))
				{
					%set.firstentry = 0;
					%setlist[%s++] = %set;

					for(%j = 0; %j < %setlist[%s].getCount(); %j++)
					{
						if(isObject(%brick = %setlist[%s].getObject(%j)) && strstr(strlwr(%brick.getname()), "itemspawn") != -1)
						%brick.setItem(none);
					}
				}
			}
		}

		for(%i = 0; %i < %client.brickgroup.ntobjectcount_ammocrate; %i++)
		{
			if(isObject(%ammocrate = %client.brickgroup.ntobject_ammocrate_[%i]))
			%ammocrate.ammocrate.AmmoSupplyCount = 0;
		}

		for(%i = 0; %i < %client.brickgroup.ntobjectcount_healthlocker; %i++)
		{
			if(isObject(%healthlocker = %client.brickgroup.ntobject_healthlocker_[%i]))
			%healthlocker.healthlocker.LockerSupplyCount = 0;
		}
		
		%minigame.schedule(10,removeZombieBots,"Clear",%client);
		%minigame.L4B_PlaySound("game_start_sound");

        if(isObject(l4b_music)) 
        l4b_music.delete();
        %minigame.DirectorMusic("musicdata_L4D_safearea" @ getRandom(1,4),%client);

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
			}
		}

    	%minigame.UrgentRound = 0;
    	%minigame.SoldierTank = 0;
		%minigame.DirectorTankRound = 0;
        cancel(%minigame.hordeMusic);
        cancel(%minigame.hordeMusic1);
        cancel(%minigame.hordeMusic2);
        cancel(%minigame.hordeEndShed);
	}

    function MiniGameSO::endGame(%minigame)
    {
        Parent::endGame(%minigame);
        if(isObject(l4b_music)) 
        l4b_music.delete();
		
		%minigame.schedule(1,removeZombieBots,"Clear",%client);
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

if(isPackage(BotHolePackage))
{
	deactivatePackage(BotHolePackage);
	activatePackage(BotHolePackage);
	activatePackage(L4B2Bots_Main);
}

if(isPackage(holeZombiePackage))
deactivatePackage(holeZombiePackage);//New function was made for this add-on, should disable that old package or things go kablooey