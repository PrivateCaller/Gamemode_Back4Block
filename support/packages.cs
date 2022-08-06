// ============================================================
// 1. Main Package
// ============================================================
package L4B2Bots_Main
{
	function player::pickup(%obj,%item)
	{
		%par = Parent::pickup(%obj,%item);

		if(isObject(getMiniGameFromObject(%obj)) && %obj.getDatablock().isSurvivor)
		if(isObject(%item) && %par == 1)
		%item.delete();
	}

	function fxDTSBrick::onPlayerTouch (%obj, %player)
	{
		%data = %obj.getDataBlock ();
		%data.onPlayerTouch (%obj, %player);
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
		if(%objA.getclassname() $= "GameConnection" || %objA.getclassname() $= "Player" || %objA.getclassname() $= "AIPlayer")
		if(%objB.getclassname() $= "Player" || %objB.getclassname() $= "AIPlayer")
		{
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
		L4B_DifficultyAdjustment();

		if(isObject(directorBricks))
		for (%i = 0; %i < directorBricks.getCount(); %i++) 
        {
            %brick = directorBricks.getObject(%i);

            if(%brick.getdatablock().isZombieBrick)
            {
                cancel(%brick.spawnBots);
                %brick.BotHoleAmount = 0;
            }
        }

		%minigame.schedule(10,removeZombieBots,"Clear",%client);
		%minigame.L4B_PlaySound("game_start_sound");

        %directorinterval = $Pref::L4BDirector::Director_Interval*1000;

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
            %minigame.DirectorStatus = 1;
            cancel(%minigame.directorSchedule);

            %minigame.schedule(%directorinterval,directorAI,%client);

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
		
		for (%i = 0; %i < directorBricks.getCount(); %i++) 
        {
            %brick = directorBricks.getObject(%i);

            if(%brick.getdatablock().isZombieBrick)
            {
                cancel(%brick.spawnBots);
                %brick.BotHoleAmount = 0;
            }
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

if(isPackage(BotHolePackage))
{
	deactivatePackage(BotHolePackage);
	activatePackage(BotHolePackage);
	activatePackage(L4B2Bots_Main);
}

if(isPackage(holeZombiePackage))
deactivatePackage(holeZombiePackage);//New function was made for this add-on, should disable that old package or things go kablooey