luaexec("./script_director.lua");

registerOutputEvent(Minigame, "Director", "List Disable 0 Enable 1",0);
registerOutputEvent(Minigame, "RoundEnd");
registerOutputEvent(Minigame, "PanicRound");
registerOutputEvent(Player,RemoveItem,"datablock ItemData",1);

function Player::removeItem(%pl,%item,%cl)  
{
	if(isObject(%pl))
	{
		for(%i=0;%i<%pl.dataBlock.maxTools;%i++)
		{
			%tool = %pl.tool[%i];
			if(%tool == %item)
			{
				%pl.tool[%i] = 0;
				messageClient(%cl,'MsgItemPickup','',%i,0);
				if(%pl.currTool==%i) 
				{
					%pl.updateArm(0);
					%pl.unMountImage(0);
				}
			}
		}
	}
}

package L4B_Director
{
	function minigameCanDamage(%objA, %objB)
	{
		if(!isObject(%objA) || !isObject(%objB)) return false;

        if(%objA.getClassName() $= "GameConnection") %TargetA = %objA.player;
        else %TargetA = %objA;

        if(%objB.getClassName() $= "GameConnection") %TargetB = %objB.player;
        else %TargetB = %objB;

		if(%TargetA !$= %TargetB && getMiniGameFromObject(%TargetA) $= getMiniGameFromObject(%TargetB) && !checkHoleBotTeams(%TargetA,%TargetB)) return false;
		
		Parent::minigameCanDamage(%objA, %objB);
	}

    function MiniGameSO::endGame(%minigame)
    {
        %minigame.L4B_ClearData(%client);
		
		Parent::endGame(%minigame);
    }

    function MiniGameSO::Reset(%minigame,%client)
	{
        Parent::Reset(%minigame,%client);

		%currTime = getSimTime();
		if(%obj.lastResetTime + 5000 > %currTime) return;
		%minigame.lastResetTime = %currTime;

        %minigame.L4B_ClearData(%client); 
        %minigame.l4bMusic("musicdata_L4D_safearea" @ getRandom(1,4),true,"Music");
        %minigame.l4bMusic("musicdata_ambience_DASH_Liminal_" @ getRandom(1,3),true,"Ambience");
        %minigame.l4bMusic("game_start_sound",false,"Stinger1");
	}

	function MiniGameSO::checkLastManStanding(%minigame)
	{
		if(%minigame.RespawnTime > 0 || isEventPending(%minigame.resetSchedule)) return;

		for(%i = 0; %i < %minigame.numMembers; %i++) if(isObject(%player = %minigame.member[%i].player) && !%player.hIsInfected && !%player.getdataBlock().isDowned) %livePlayerCount++;

		if(!%livePlayerCount)
		{
			%minigame.VictoryTo = "Infected";
            %minigame.l4bMusic("game_lose_sound",false,"Music");
            %minigame.deletel4bMusic("Stinger1");
            %minigame.deletel4bMusic("Stinger2");
            %minigame.deletel4bMusic("Stinger3");
            %minigame.director(0,0);
			%minigame.scheduleReset(12000);
		}
	}

    function GameConnection::onClientLeaveGame (%client)
    {    
        Parent::onClientLeaveGame(%client);

        %client.deletel4bMusic("Music");
        %client.deletel4bMusic("Musi2");
        %client.deletel4bMusic("Stinger1");
        %client.deletel4bMusic("Stinger2");
        %client.deletel4bMusic("Stinger3");
        %client.deletel4bMusic("Ambience");        
    }
};
activatePackage(L4B_Director);

function MiniGameSO::L4B_ClearData(%minigame,%client)
{
    for(%i=0;%i<%minigame.numMembers;%i++)
    {
        if(isObject(%mgmember = %minigame.member[%i]))
        {
            %mgmember.delayMusicTime = 0;
            %aliveplayer++;
            %minigame.survivorStatHealthAverage = 100*%aliveplayer;
            %minigame.survivorStatStressAverage = 0;
        }
    }    
    
    %minigame.RoundType = "";
    %minigame.finalround = false;

    cancel(%minigame.spawn["Horde"]);
    cancel(%minigame.spawn["Special"]);
    cancel(%minigame.directorSchedule);
    cancel(%minigame.hordeEndShed);
    %minigame.deletel4bMusic("Music");
    %minigame.deletel4bMusic("Music2");
    %minigame.deletel4bMusic("Stinger1");
    %minigame.deletel4bMusic("Stinger2");
    %minigame.deletel4bMusic("Stinger3");
    %minigame.deletel4bMusic("Ambience");   

    if($Pref::L4B::Director::EnableOnMG) %miniGame.director(1,0);
    else %miniGame.director(0,0);
    

    for(%i = 0; %i < %client.brickgroup.ntobjectcount_progress_door; %i++) if(isObject(%door = %client.brickgroup.ntobject_progress_door_[%i])) %door.door(close);

    if(isObject(L4B_BotSet))
    {
        for(%z = 0; %z < L4B_BotSet.getCount(); %z++) 
        if(isObject(%bot = L4B_BotSet.getObject(%z))) %bot.schedule(10,delete);
        L4B_BotSet.delete();
    }        

    if(isObject(AreaZoneGroup))
    {
        for(%i = 0; %i < AreaZoneGroup.getCount(); %i++)
        {
            if(isObject(%zone = AreaZoneGroup.getObject(%i)))
            {
                %zone.firstentry = false;

                for(%j = 0; %j < %zone.simset.getCount(); %j++)
                if(isObject(%brick = %zone.simset.getObject(%j)) && %brick.getdataBlock().ZoneBrickType $= "item") %brick.setItem(none);
            }
        }
    }    
}

function MinigameSO::L4B_PlaySound(%minigame,%sound)
{
    for(%i=0;%i<%minigame.numMembers;%i++)
    if(isObject(%client = %minigame.member[%i]) && %client.getClassName() $= "GameConnection") %client.play2d(%sound.getID());    
}

$L4B_lastSupportMessageTime = getSimTime();
function MiniGameSO::L4B_ChatMessage(%miniGame, %text, %sound, %bypassdelay)
{
    if(!%bypassdelay && getSimTime() - $L4B_lastSupportMessageTime < 15000) return;
    
    announce(%text);
    %miniGame.l4bMusic(%sound,false,"Stinger3");
    $L4B_lastSupportMessageTime = getSimTime();
}

function MinigameSO::Director(%minigame,%enabled,%interval)
{
    switch(%enabled)
    {
        case 0: if(%minigame.DirectorStatus)
                {
                    %minigame.DirectorStatus = 0;
                    cancel(%minigame.directorSchedule);
                }
                return;

        case 1: if(!%minigame.DirectorStatus)
                {
                    %minigame.DirectorStatus = 1;
                    %minigame.l4bMusic("hordeincoming" @ getrandom(1,9) @ "_sound",false,"Stinger3");
                    %minigame.deletel4bMusic("Music");
                    %minigame.deletel4bMusic("Music2");
                }
    }

    if(%minigame.DirectorStatus)
    {    
        if(%interval > 3) 
        {
            %interval = 0;

            switch(%minigame.DirectorStatus)
            {
                case 0: return;
                case 1: for(%i=0;%i<%minigame.numMembers;%i++)//Calculate overall variables
                        {
                            %fixcount = %i+1;
                            if(isObject(%mgmember = %minigame.member[%i]))
                            {
                                %minigame.survivorStatHealthMax = 100*%fixcount;
                                %minigame.survivorStressMax = 20*%fixcount;
                                
                                if(isObject(%mgmember.player))
                                {
                                    if(%mgmember.player.getdataBlock().getName() $= "SurvivorPlayer")
                                    {                                    
                                        %health += %mgmember.player.getdamagelevel();
                                        %stresslevel += %mgmember.player.survivorStress;
                                        %mgmember.player.survivorStress = 0;
                                    }
                                    else
                                    {
                                        %health += 100;
                                        %stresslevel += 20;
                                    }
                                }
                                else
                                {
                                    %health += 100;
                                    %stresslevel += 20;
                                }
                                %health = mClamp(%health,0,%minigame.survivorStatHealthMax);
                                %stresslevel = mClamp(%stresslevel,0,%minigame.survivorStressMax);

                                %minigame.survivorStatHealthAverage = %minigame.survivorStatHealthMax-%health;
                                %minigame.survivorStatStressAverage = %stresslevel;

                                if(%minigame.survivorStatHealthAverage < %minigame.survivorStatHealthMax/3 || %minigame.survivorStatStressAverage > %minigame.survivorStressMax/1.5)
                                %stressed = true;
                            }
                        }
                        

                        %chance = getRandom(1,100);
                        if(%chance <= 80) %round = 1;
                        else if(%chance <= $Pref::L4B::Zombies::TankRoundChance) %round = 3;
                        else if(%chance <= 25) %round = 3;

                        if(%stressed || %minigame.roundscount > 3) %round = 0;

                        switch(%round)
                        {
                            case 0: %minigame.BreakRound();
                            case 1: %minigame.HordeRound();
                            case 2: %minigame.WitchRound();
                            case 3: %minigame.TankRound();
                        }
                case 2: 
            }            
        }

        if(%minigame.finalround)
        {
            %minigame.spawnZombies("Horde",10);
            %minigame.spawnZombies("Special",1);      
            if(getRandom(1,4) == 1 ) %minigame.spawnZombies("Tank",1);
        }
        else switch(%interval)
        {
            case 1: %cue = true;            
            case 2: if(getRandom(1,2) == 1) %cue = true;
            
                    if(!%stressed) %spawnchance = 4;
                    else %spawnchance = 3;

                    if(getRandom(1,%spawnchance) == 1) %minigame.spawnZombies("Horde",10,0);
                    if(getRandom(1,2) == 1) %minigame.spawnZombies("Special",getRandom(1,4),0);

                    %minigame.SpawnStalkZombies(); 
            case 3: if(getRandom(1,2) == 1) %cue = true;
            default:
        }
        
        if(%cue) switch$(%minigame.RoundType)
        {
            case "Break":   if(getRandom(1,2) == 1) %minigame.l4bMusic("zombiechoir_0" @ getrandom(1,6) @ "_sound",false,"Stinger1"); 
                            if(getRandom(1,8) == 1) %minigame.l4bMusic("aglimpseofhell_" @ getrandom(1,3) @ "_sound",false,"Stinger2");

            case "Horde":   if(getRandom(1,2) == 1) %minigame.l4bMusic("hordeslayer_0" @ getrandom(1,3) @ "_sound",false,"Stinger1"); 
                            if(getRandom(1,10) == 1 || %interval == 3) %minigame.l4bMusic("horde_danger_sound",false,"Stinger2");
            default:
        }        

        cancel(%minigame.directorSchedule);
        %minigame.directorSchedule = %minigame.schedule(10000,Director,1,%interval++);
    }
}

function MinigameSO::SpawnStalkZombies(%minigame)//Go after others who are on their own
{
    %highestZone = 0;
    for(%i=0;%i <= %minigame.numMembers;%i++)
    {    
        if(isObject(%minigame.member[%i]) && isObject(%player = %minigame.member[%i].player) && %player.getdataBlock().isSurvivor)
        {
            %survivorcount++;
            %survivorplayer[%survivorcount] = %player;

            if(%player.currentZoneNumber > %highestZone)//Find whoever is in the furthest zone
            {
                %highestZonePlayer = %player.currentZoneNumber;
                %highestZone = %player.currentZoneNumber;
            }            
        }
    }    
            

    if(%survivorcount > 1 && %highestZonePlayer.survivorAllyCount == 1)
    {
        %minigame.spawnZombies("Horde",getRandom(10,20),%player.currAreaZone);
        %minigame.spawnZombies("Special",getRandom(2,4),%player.currAreaZone);
    }
}

function MinigameSO::BreakRound(%minigame)
{    
    if(!%minigame.DirectorStatus) return;

    %minigame.RoundType = "Break";
    %minigame.deletel4bMusic("Music");
    %minigame.deletel4bMusic("Music2");
    %minigame.deletel4bMusic("Stinger1");
    %minigame.deletel4bMusic("Stinger2");
    %minigame.deletel4bMusic("Stinger3");
    %minigame.l4bMusic("nm_quarantine_" @ getRandom(1,3) @ "_sound",false,"Music");
}

function MinigameSO::WitchRound(%minigame)
{
    if(!%minigame.DirectorStatus || !%minigame.spawnZombies("Witch",1) || %minigame.directorTankRound > $Pref::L4B::Zombies::TankRounds)
    {
        %miniGame.HordeRound();
        return;
    }
    
    %minigame.RoundType = "Witch";
    %minigame.L4B_ChatMessage("[A witch is nearby]","victim_needshelp_sound",true); 
    %minigame.DirectorStatus = 2;
}

function MinigameSO::TankRound(%minigame)
{
    if(!%minigame.DirectorStatus || !%minigame.spawnZombies("Tank",1) || %minigame.directorTankRound > $Pref::L4B::Zombies::TankRounds)
    {
        %miniGame.HordeRound();
        return;
    }

    %minigame.directorTankRound++;
    %minigame.DirectorStatus = 2;
    %minigame.zhordecount = 9999;

    %minigame.spawnZombies("Tank",1,0);
    %minigame.L4B_ChatMessage("[A tank is nearby]","victim_needshelp_sound",true);  
}

function MinigameSO::PanicRound(%minigame)
{
    if(!%minigame.DirectorStatus) return;
    
    %minigame.zhordecount = 9999;
    %minigame.DirectorStatus = 2;
    %minigame.finalround = true;
    %minigame.RoundType = "";
    %minigame.L4B_ChatMessage("[They're coming!] <bitmapk:Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_skull2>","hordeincoming" @ getrandom(1,9) @ "_sound",true); 
    %minigame.l4bMusic("musicData_L4D_horde_urgent",false,"Music2");
    %minigame.schedule(15000,l4bMusic,"musicData_L4D_horde_combat",true,"Music");
    %minigame.schedule(15000,l4bMusic,"drum_suspense_end_sound",false,"Stinger1");
}

function MiniGameSO::HordeRound(%minigame)
{
    %random = getRandom(25,35);    
    if(!%minigame.DirectorStatus || !%minigame.spawnZombies("Horde",%random+5,0)) return;
    
    %minigame.DirectorStatus = 2;
    %minigame.RoundType = "Horde";
    %minigame.zhordecount = %random;
    %minigame.L4B_ChatMessage("[They're coming...]","hordeincoming" @ getrandom(1,9) @ "_sound",true); 
    %minigame.schedule(4000,l4bMusic,"musicData_l4d_horde_combat",true,"Music");
    %minigame.schedule(4000,l4bMusic,"drum_suspense_end_sound",false,"Stinger1");
}

function MinigameSO::RoundEnd(%minigame)
{        
    %minigame.RoundType = "";
    %minigame.l4bMusic("drum_suspense_end_sound",false,"Stinger3");
    %minigame.deletel4bMusic("Music");
    %minigame.deletel4bMusic("Music2");
    %minigame.DirectorStatus = 1;
}

function MinigameSO::spawnZombies(%minigame,%type,%amount,%spawnzone,%count)
{    
    if(!isObject(%spawnzone))
    {
        if(!isObject(AreaZoneGroup) || !AreaZoneGroup.getCount()) return false;
        
        for(%i = 0; %i < AreaZoneGroup.getcount(); %i++) 
        {
            if(isObject(%zone = AreaZoneGroup.getObject(%i)) && %zone.presencecount && isObject(%simset = %zone.simset)) for(%j = 0; %j < %simset.getcount(); %j++)
            if(isObject(%setbrick = %simset.getObject(%j)) && %setbrick.getdataBlock().ZoneBrickType $= "spawner" && strstr(strlwr(%setbrick.getName()),"_" @ strlwr(%type)) != -1)
            {
                %spawnlist[%sb++] = %setbrick;
                %spawnlist[%sb].currentset = %zone;
            }
        }    
    }
    else 
    {     
        for(%i = 0; %i < %spawnzone.simset.getcount(); %i++)
        if(isObject(%setbrick = %spawnzone.simset.getObject(%i)) && %setbrick.getdataBlock().ZoneBrickType $= "spawner" && strstr(strlwr(%setbrick.getName()),"_" @ strlwr(%type)) != -1)
        {
            %spawnlist[%sb++] = %setbrick;
            %spawnlist[%sb].currentset = %spawnzone;
        }
    }

    if(%sb)
    {
        if(%type $= "Wander")
        {
            for (%b = 1; %b <= %sb; %b++) 
            {
                %spawnbrick = %spawnlist[%b];
                if(%b > %sb || !isObject(%spawnbrick)) break;

                if(getRandom(1))
                {
                    %bottype = "CommonZombieHoleBot";

                    %bot = new AIPlayer()
                    {
                        dataBlock = %bottype;
                        path = "";
                        spawnBrick = %spawnbrick;
                        spawnType = %type;
    
                        Name = %bottype.hName;
                        currentZone = %spawnlist[%sb].currentset;
                        hType = %bottype.hType;
                        hSearchRadius = %bottype.hSearchRadius;
                        hSearch = %bottype.hSearch;
                        hSight = %bottype.hSight;
                        hWander = %bottype.hWander;
                        hGridWander = false;
                        hReturnToSpawn = false;
                        hSpawnDist = %bottype.hSpawnDist;
                        hMelee = %bottype.hMelee;
                        hAttackDamage = %bottype.hAttackDamage;
                        hSpazJump = false;
                        hSearchFOV = %bottype.hSearchFOV;
                        hFOVRadius = %bottype.hFOVRadius;
                        hTooCloseRange = %bottype.hTooCloseRange;
                        hAvoidCloseRange = %bottype.hAvoidCloseRange;
                        hShoot = %bottype.hShoot;
                        hMaxShootRange = %bottype.hMaxShootRange;
                        hStrafe = %bottype.hStrafe;
                        hAlertOtherBots = %bottype.hAlertOtherBots;
                        hIdleAnimation = %bottype.hIdleAnimation;
                        hSpasticLook = %bottype.hSpasticLook;
                        hAvoidObstacles = %bottype.hAvoidObstacles;
                        hIdleLookAtOthers = %bottype.hIdleLookAtOthers;
                        hIdleSpam = %bottype.hIdleSpam;
                        hAFKOmeter = %bottype.hAFKOmeter + getRandom( 0, 2 );
                        hHearing = %bottype.hHearing;
                        hIdle = %bottype.hIdle;
                        hSmoothWander = %bottype.hSmoothWander;
                        hEmote = %bottype.hEmote;
                        hSuperStacker = %bottype.hSuperStacker;
                        hNeutralAttackChance = %bottype.hNeutralAttackChance;
                        hFOVRange = %bottype.hFOVRange;
                        hMoveSlowdown = 0;
                        hMaxMoveSpeed = 1;
                        hActivateDirection = %bottype.hActivateDirection;
                        hGridPosition = %spawnbrick.getPosition();
                        isHoleBot = 1;
                    };

                    $InputTarget_["Self"] = %spawnbrick;
                    switch$(%bot.getclassname())
                    {
                        case "Player":	$InputTarget_["Player"] = %spawnbrick.getgroup().client.player;
                                        $InputTarget_["Client"] = %spawnbrick.getgroup().client;
                        case "AIPlayer": $InputTarget_["Bot"] = %bot;
                    }
                    $InputTarget_["MiniGame"] = getMiniGameFromObject(%obj);
                    %spawnbrick.processInputEvent("onBotTeleSpawn",%spawnbrick.getgroup().client);

                    if(strlen(%bottype.hMeleeCI))
                    eval("%bot.hDamageType = $DamageType::" @ %bottype.hMeleeCI @ ";");
                    else %bot.hDamageType = $DamageType::HoleMelee;
                    %bot.setTransform(%spawnbrick.getposition() SPC getwords(%spawnbrick.gettransform(),3,6));

                    if(isObject(%bot))
                    {
                        if(!isObject(L4B_BotSet))
                        {
                            new SimSet(L4B_BotSet);
                            missionCleanup.add(L4B_BotSet);                        
                        }
                        else if(!L4B_BotSet.isMember(%bot)) L4B_BotSet.add(%bot);
                    }
                }
            }            
        }
        else if(%count < %amount)
        {
            %random = getRandom(1,%sb);
            %spawnbrick = %spawnlist[%random];
            %zone = %spawnlist[%random].currentset;

            switch$(%type)
            {
                case "Horde": %bottype = "CommonZombieHoleBot";
                              if(getRandom(1,8) == 1) %bottype = $hZombieUncommonType[getRandom(1,$hZombieUncommonTypeAmount)];
                              if(getRandom(1,16) == 1 && $L4B_CurrentMonth == 10) %bottype = "SkeletonHoleBot";
                case "Tank": %bottype = "ZombieTankHoleBot";
                case "Witch": %bottype = "ZombieWitchHoleBot";    
                case "Special": %bottype = $hZombieSpecialType[getRandom(1,$hZombieSpecialTypeAmount)];
                
            }

            %bot = new AIPlayer()
            {
                dataBlock = %bottype;
                path = "";
                spawnBrick = %spawnbrick;
                spawnType = %type;

                Name = %bottype.hName;
                currentZone = %zone;
                hType = %bottype.hType;
                hSearchRadius = %bottype.hSearchRadius;
                hSearch = %bottype.hSearch;
                hSight = %bottype.hSight;
                hWander = %bottype.hWander;
                hGridWander = false;
                hReturnToSpawn = false;
                hSpawnDist = %bottype.hSpawnDist;
                hMelee = %bottype.hMelee;
                hAttackDamage = %bottype.hAttackDamage;
                hSpazJump = false;
                hSearchFOV = %bottype.hSearchFOV;
                hFOVRadius = %bottype.hFOVRadius;
                hTooCloseRange = %bottype.hTooCloseRange;
                hAvoidCloseRange = %bottype.hAvoidCloseRange;
                hShoot = %bottype.hShoot;
                hMaxShootRange = %bottype.hMaxShootRange;
                hStrafe = %bottype.hStrafe;
                hAlertOtherBots = %bottype.hAlertOtherBots;
                hIdleAnimation = %bottype.hIdleAnimation;
                hSpasticLook = %bottype.hSpasticLook;
                hAvoidObstacles = %bottype.hAvoidObstacles;
                hIdleLookAtOthers = %bottype.hIdleLookAtOthers;
                hIdleSpam = %bottype.hIdleSpam;
                hAFKOmeter = %bottype.hAFKOmeter + getRandom( 0, 2 );
                hHearing = %bottype.hHearing;
                hIdle = %bottype.hIdle;
                hSmoothWander = %bottype.hSmoothWander;
                hEmote = %bottype.hEmote;
                hSuperStacker = %bottype.hSuperStacker;
                hNeutralAttackChance = %bottype.hNeutralAttackChance;
                hFOVRange = %bottype.hFOVRange;
                hMoveSlowdown = false;
                hMaxMoveSpeed = 1;
                hActivateDirection = %bottype.hActivateDirection;
                hGridPosition = %spawnbrick.getPosition();
                isHoleBot = 1;
            };

            $InputTarget_["Self"] = %spawnbrick;
            switch$(%bot.getclassname())
            {
                case "Player":	$InputTarget_["Player"] = %spawnbrick.getgroup().client.player;
                                $InputTarget_["Client"] = %spawnbrick.getgroup().client;
                case "AIPlayer": $InputTarget_["Bot"] = %bot;
            }
            $InputTarget_["MiniGame"] = getMiniGameFromObject(%obj);
            %spawnbrick.processInputEvent("onBotTeleSpawn",%spawnbrick.getgroup().client);

            if(strlen(%bottype.hMeleeCI))
            eval("%bot.hDamageType = $DamageType::" @ %bottype.hMeleeCI @ ";");
            else %bot.hDamageType = $DamageType::HoleMelee;
            %bot.setTransform(%spawnbrick.getposition() SPC getwords(%spawnbrick.gettransform(),3,6));

            if(isObject(%bot))
            {
                if(!isObject(L4B_BotSet))
                {
                    new SimSet(L4B_BotSet);
                    missionCleanup.add(L4B_BotSet);                        
                }
                else if(!L4B_BotSet.isMember(%bot)) L4B_BotSet.add(%bot);
            }

            cancel(%minigame.spawn[%type]);
            %minigame.spawn[%type] = %minigame.scheduleNoQuota(500,spawnZombies,%type,%amount,%spawnzone,%count++);
        }
        return true;
    }
    else return false;
}
registerInputEvent("fxDTSBrick","onBotTeleSpawn","Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "Bot Bot" TAB "MiniGame MiniGame");

$L4B_Music["Music"] = 0;
$L4B_Music["Music2"] = 0;
$L4B_Music["Stinger1"] = 0;
$L4B_Music["Stinger2"] = 0;
$L4B_Music["Stinger3"] = 0;
$L4B_Music["Ambience"] = 0;

function MiniGameSO::l4bMusic(%minigame, %datablock, %loopable, %type)
{
    for(%i = 0; %i < %minigame.numMembers; %i++)
    if(isObject(%player = %minigame.member[%i].player)) 
    if(!%player.hIsInfected && !%player.getdataBlock().isDowned) %livePlayerCount++;
    
    for(%i = 0; %i < %minigame.numMembers; %i++)   
    if(isObject(%mgmember = %minigame.member[%i]))
    {
        if((isObject(%player = %mgmember.player) && %player.isBeingStrangled)) return;
        %mgmember.l4bMusic(%dataBlock,%loopable,%type);
    }
}

function GameConnection::l4bMusic(%client, %datablock, %loopable, %type)
{   
    if(!isObject(%datablock) || getSimTime() - %mgmember.delayMusicTime < 50000) return;
    if(isObject(%client.l4bMusic[%type])) %client.l4bMusic[%type].delete();

    switch$(%type)
    {
        case "Music": %channel = 10;
        case "Music2": %channel = 10;
        case "Stinger1": %channel = 11;
        case "Stinger2": %channel = 11;
        case "Stinger3": %channel = 11;
        case "Ambience": %channel = 12;
        default: return;
    }

    %client.l4bMusic[%type] = new AudioEmitter(l4b_music)
    {
        profile = %datablock;
        isLooping= %loopable;
        position = "9e9 9e9 9e9";
        is3D = false;
        useProfileDescription = false;
        type = %channel;
    };
    %client.l4bMusic[%type].setNetFlag(6, true);
    %client.l4bMusic[%type].scopeToClient(%client);
}

function MiniGameSO::deletel4bMusic(%minigame, %type)
{
    for(%i = 0; %i < %minigame.numMembers; %i++)   
    if(isObject(%mgmember = %minigame.member[%i])) %mgmember.deletel4bMusic(%type);
}

function GameConnection::deletel4bMusic(%client, %type)
{
    if(isObject(%client.l4bMusic[%type])) %client.l4bMusic[%type].delete();   
}

function GameConnection::musicCatchUp(%client)
{   
    %music_tags = "Music Music2 Stinger1 Stinger2 Stinger3 Ambience";
    for(%i = 0; %i < getWordCount(%music_tags); %i++)
    if(isObject(%music_object = $L4B_Music[getWord(%music_tags, %i)])) %music_object.scopeToClient(%client);
}