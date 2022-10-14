registerOutputEvent(Minigame, "Director", "List Disable 0 Enable 1",0);
registerOutputEvent(Minigame, "RoundEnd");

package L4B_Director
{
	function minigameCanDamage(%objA, %objB)
	{
		if(!isObject(%objA) || !isObject(%objB)) return false;
		
		if(%objA.getclassname() $= "GameConnection") %TargetA = %objA.player;
		else %TargetA = %objA;
		if(%objB.getclassname() $= "GameConnection") %TargetB = %objB.player;
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
        %minigame.L4B_PlaySound("game_start_sound");
	}

	function MiniGameSO::checkLastManStanding(%minigame)
	{
		if(%minigame.RespawnTime > 0 || isEventPending(%minigame.resetSchedule)) return;

		for(%i = 0; %i < %minigame.numMembers; %i++)
		if(isObject(%player = %minigame.member[%i].player)) 
        if(!%player.hIsInfected && !%player.getdataBlock().isDowned) %livePlayerCount++;

		if(%livePlayerCount <= 0)
		{
			%minigame.L4B_PlaySound("game_lose_sound");
            %minigame.deletel4bMusic("Music");
            %minigame.deletel4bMusic("Trigger1");
            %minigame.deletel4bMusic("Trigger2");
            %minigame.deletel4bMusic("Trigger3");
			%minigame.scheduleReset(12000);
		}
	}

    function GameConnection::onClientLeaveGame (%client)
    {    
        Parent::onClientLeaveGame(%client);

        if(isObject(%client.l4bMusic["Music"])) %client.deletel4bMusic("Music");
        if(isObject(%client.l4bMusic["Stinger1"])) %client.deletel4bMusic("Stinger1");
        if(isObject(%client.l4bMusic["Stinger2"])) %client.deletel4bMusic("Stinger2");
        if(isObject(%client.l4bMusic["Stinger3"])) %client.deletel4bMusic("Stinger3");
        if(isObject(%client.l4bMusic["Ambience"])) %client.deletel4bMusic("Ambience");        
    }
};
activatePackage(L4B_Director);

function MiniGameSO::L4B_ClearData(%minigame,%client)
{
    %minigame.DirectorTankRound = 0;

    if($Pref::L4BDirector::EnableOnMG)
    {
        %minigame.DirectorStatus = 1;

        cancel(%minigame.directorSchedule);
        %minigame.schedule(10000,directorAI,%client);

        if($Pref::L4BDirector::EnableCues) %minigame.directorPlaySound("hordeincoming" @ getrandom(1,9) @ "_sound",%client);
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

    cancel(%minigame.spawn["Horde"]);
    cancel(%minigame.spawn["Special"]);
    cancel(%minigame.directorSchedule);
    cancel(%minigame.hordeEndShed);
    %minigame.deletel4bMusic("Music");
    %minigame.deletel4bMusic("Stinger1");
    %minigame.deletel4bMusic("Stinger2");
    %minigame.deletel4bMusic("Stinger3");
    %minigame.deletel4bMusic("Ambience");   

    for(%i = 0; %i < %client.brickgroup.ntobjectcount_progress_door; %i++) if(isObject(%door = %client.brickgroup.ntobject_progress_door_[%i])) %door.door(close);

    if(isObject(MainAreaZone)) MainAreaZone.delete();

    if(isObject(L4B_BotSet))
    {
        for(%z = 0; %z < L4B_BotSet.getCount(); %z++) 
        if(isObject(%bot = L4B_BotSet.getObject(%z))) %bot.schedule(10,delete);
        L4B_BotSet.delete();
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
                if(isObject(%brick = %setlist[%s].getObject(%j)) && strstr(strlwr(%brick.getname()), "_item") != -1) %brick.setItem(none);					
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
    
    announce("[" @ %text @ "]");
    %miniGame.L4B_PlaySound(%sound);
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
                    %minigame.L4B_PlaySound("hordeincoming" @ getrandom(1,9) @ "_sound",%client);
                    %minigame.deletel4bMusic("Music");
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
                                %stressed = 1;
                            }
                        }
                        
                        if(getRandom(1,2) == 1)
                        {
                            %chance = getRandom(1,100);
                            if(%chance <= 65) %round = 1;
                            else if(%chance <= $Pref::Server::L4B2Bots::TankRoundChance) %round = 3;
                            else if(%chance <= 10) %round = 3;

                            if(%stressed) %round = 0;
                        }
                        else %round = 0;

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

        if(%interval == 2)
        {
            if(!%stressed) %spawnchance = 4;
            else %spawnchance = 3;

            if(getRandom(1,%spawnchance) == 1) %minigame.spawnZombies("Horde",10,0);
            if(getRandom(1,2) == 1) %minigame.spawnZombies("Special",getRandom(1,4),0);

            %minigame.SpawnStalkZombies();
        }        

        cancel(%minigame.directorSchedule);
        %minigame.directorSchedule = %minigame.schedule(10000,Director,1,%interval++);
    }
}

function MinigameSO::SpawnStalkZombies(%minigame)//Go after others who are on their own
{
    for(%i=0;%i <= %minigame.numMembers;%i++)
    {    
        if(isObject(%minigame.member[%i]) && isObject(%player = %minigame.member[%i].player) && %player.getdataBlock().isSurvivor)
        {
            %survivorcount++;
            %survivorplayer[%survivorcount] = %player;   
        }
    }

    for (%j = 1; %j <= %survivorcount; %j++) 
    {
        if(%survivorcount > 1 && %survivorplayer[%j].survivorAllyCount == 1 && %survivorplayer[%j].currAreaZone != %survivorplayer[%j+1].currAreaZone)
        {
            %minigame.spawnZombies(Horde,getRandom(10,20),%player.currAreaZone);
            %minigame.spawnZombies(Special,getRandom(1,2),%player.currAreaZone);
        }
    }
}

function MinigameSO::BreakRound(%minigame)
{    
    %minigame.deletel4bMusic("Music");
    %minigame.deletel4bMusic("Stinger1");
    %minigame.deletel4bMusic("Stinger2");
    %minigame.deletel4bMusic("Stinger3");
    
    if(getRandom(1,8) == 1) 
    {
        %isPlayingMusic = true;
        
        %minigame.l4bMusic("nm_quarantine_" @ getRandom(1,3) @ "_sound",false,"Music");
        if(getRandom(1,8) == 1) %minigame.l4bMusic["Stinger1"] = %minigame.schedule(15000,L4B_PlaySound,"zombiechoir_0" @ getrandom(1,6) @ "_sound",false,"Stinger1");
        if(getRandom(1,8) == 1) %minigame.l4bMusic["Stinger2"] = %minigame.schedule(5000,L4B_PlaySound,"zombiechoir_0" @ getrandom(1,6) @ "_sound",false,"Stinger2");        
    }

    if(!%isPlayingMusic && getRandom(1,8) == 1) %minigame.l4bMusic["Stinger1"] = %minigame.schedule(15000,L4B_PlaySound,"aglimpseofhell_" @ getrandom(1,3) @ "_sound",false,"Stinger1");
}

function MinigameSO::WitchRound(%minigame)
{
    if(!%minigame.DirectorStatus || !isObject(MainAreaZone))
    return;    

    for(%i = 0; %i < MainAreaZone.getCount(); %i++) 
    {				
        if(strstr(strlwr(MainAreaZone.getObject(%i).getName()),"witch") != -1)
        %spawn++;
    }

    if(!%spawn) return;

    announce("[A witch is nearby]");
    %minigame.spawnZombies("Witch",1,0);
    %minigame.DirectorStatus = 2;
}

function MinigameSO::TankRound(%minigame)
{
    if(!%minigame.DirectorStatus || !isObject(MainAreaZone))
    return;    

    for(%i = 0; %i < MainAreaZone.getCount(); %i++) 
    {				
        if(strstr(strlwr(MainAreaZone.getObject(%i).getName()),"tank") != -1)
        %spawn++;
    }

    if(!%spawn)
    return;

    if(%minigame.directorTankRound < $Pref::Server::L4B2Bots::TankRounds)
    {
        %minigame.directorTankRound++;
        %minigame.DirectorStatus = 2;
        
        if(getRandom(1,15) == 1)
        %minigame.SoldierTank = 1;
        else %minigame.SoldierTank = 0;

        %minigame.spawnZombies("Tank",1,0);
        announce("[A tank is nearby]");
    } 
}

function MinigameSO::PanicRound(%minigame)
{
    if(!%minigame.DirectorStatus || !isObject(MainAreaZone))
    return;    

    for(%i = 0; %i < MainAreaZone.getCount(); %i++) 
    {				
        if(strstr(strlwr(MainAreaZone.getObject(%i).getName()),"horde") != -1) %spawn++;
    }
    
    if(!%spawn) return;
    
    %minigame.DirectorStatus = 2;

    announce("[Time to escape]");

    %minigame.spawnZombies("Horde",25,0);
    %minigame.DirectorStatus = 2;
    cancel(%minigame.directorSchedule);
}

function MiniGameSO::HordeRound(%minigame)
{
    if(!%minigame.DirectorStatus || !isObject(MainAreaZone)) return;

    for(%i = 0; %i < MainAreaZone.getCount(); %i++) if(strstr(strlwr(MainAreaZone.getObject(%i).getName()),"horde") != -1) %spawn++;
    if(!%spawn) return;

    %minigame.zhordecount = 1; 
    %minigame.DirectorStatus = 2;

    %random = getRandom(35,45);
    %minigame.zhordecount = %random;
    %minigame.spawnZombies("Horde",%random+5,0);
    %minigame.L4B_PlaySound("hordeincoming" @ getrandom(1,9) @ "_sound");
    announce("[They're coming...]");
    %minigame.schedule(4000,l4bMusic,"musicData_l4d_horde_combat",true,"Music");
    %minigame.schedule(4000,l4bMusic,"drum_suspense_end_sound");
}

function MinigameSO::RoundEnd(%minigame)
{        
    %minigame.L4B_PlaySound("drum_suspense_end_sound");
    %minigame.deletel4bMusic("Music");
    %minigame.DirectorStatus = 1;
    %minigame.UrgentRound = 0;
    %minigame.SoldierTank = 0;
}

function MinigameSO::spawnZombies(%minigame,%type,%amount,%spawnset,%count)
{    
    if(!isObject(MainAreaZone) || !MainAreaZone.getCount()) return;

    if(!isObject(%spawnset))
    {
        for (%i = 0; %i < MainAreaZone.getcount(); %i++) 
        {
            if(isObject(%setbrick = MainAreaZone.getObject(%i)) && strstr(strlwr(%setbrick.getName()),"_spawn") != -1)
            if(strstr(strlwr(%setbrick.getName()),"_" @ strlwr(%type)) != -1)
            %spawnlist[%sb++] = %setbrick;
        }
    }
    else
    {
        for (%i = 0; %i < %spawnset.getcount(); %i++) 
        {
            if(isObject(%setbrick = %spawnset.getObject(%i)) && strstr(strlwr(%setbrick.getName()),"_spawn") != -1)
            if(strstr(strlwr(%setbrick.getName()),"_" @ strlwr(%type)) != -1)
            %spawnlist[%sb++] = %setbrick;
        }
    }

    if(%sb)
    {
        if(%type $= "Wander" || %type $= "Tank" || %type $= "Witch")
        {
            for (%b = 0; %b < %amount; %b++) 
            {
                if(%b > %amount)
                break;

                %spawnbrick = %spawnlist[getRandom(1,%sb)];

                switch$(%type)
                {
                    case "Wander": %bottype = "CommonZombieHoleBot";
                                   if(getRandom(1,16) == 1)
                                   %bottype = $hZombieUncommonType[getRandom(1,$hZombieUncommonTypeAmount)];
                    case "Tank": %bottype = "ZombieTankHoleBot";
                    case "Witch": %bottype = "ZombieWitchHoleBot";
                }

                %bot = new AIPlayer()
                {
                    dataBlock = %bottype;
                    path = "";
                    spawnBrick = %spawnbrick;
                    spawnType = %type;

                    Name = %bottype.hName;
                    hType = %bottype.hType;
                    hSearchRadius = %bottype.hSearchRadius;
                    hSearch = %bottype.hSearch;
                    hSight = %bottype.hSight;
                    hWander = %bottype.hWander;
                    hGridWander = %bottype.hGridWander;
                    hReturnToSpawn = %bottype.hReturnToSpawn;
                    hSpawnDist = %bottype.hSpawnDist;
                    hMelee = %bottype.hMelee;
                    hAttackDamage = %bottype.hAttackDamage;
                    hSpazJump = %bottype.hSpazJump;
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
                    hMoveSlowdown = %bottype.hMoveSlowdown;
                    hMaxMoveSpeed = 1.0;
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
                %spawnbrick.processInputEvent("onDirectorBotTeleSpawn",%spawnbrick.getgroup().client);

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
        else if(%count < %amount)
        {
            %spawnbrick = %spawnlist[getRandom(1,%sb)];

            switch$(%type)
            {
                case "Horde":   %bottype = "CommonZombieHoleBot";
                                if(getRandom(1,8) == 1) %bottype = $hZombieUncommonType[getRandom(1,$hZombieUncommonTypeAmount)];
                                if(getRandom(1,16) == 1 && $L4B_CurrentMonth == 10) %bottype = "SkeletonHoleBot";
    

                case "Special": %bottype = $hZombieSpecialType[getRandom(1,$hZombieSpecialTypeAmount)];
            }

            %bot = new AIPlayer()
            {
                dataBlock = %bottype;
                path = "";
                spawnBrick = %spawnbrick;
                spawnType = %type;

                Name = %bottype.hName;
                hType = %bottype.hType;
                hSearchRadius = %bottype.hSearchRadius;
                hSearch = %bottype.hSearch;
                hSight = %bottype.hSight;
                hWander = %bottype.hWander;
                hGridWander = %bottype.hGridWander;
                hReturnToSpawn = %bottype.hReturnToSpawn;
                hSpawnDist = %bottype.hSpawnDist;
                hMelee = %bottype.hMelee;
                hAttackDamage = %bottype.hAttackDamage;
                hSpazJump = %bottype.hSpazJump;
                hSearchFOV = false;
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
                hMoveSlowdown = %bottype.hMoveSlowdown;
                hMaxMoveSpeed = 1.0;
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
            %spawnbrick.processInputEvent("onDirectorBotTeleSpawn",%spawnbrick.getgroup().client);

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
            %minigame.spawn[%type] = %minigame.scheduleNoQuota(100,spawnZombies,%type,%amount,%spawnset,%count++);
        }
    }
}
registerInputEvent("fxDTSBrick","onDirectorBotTeleSpawn","Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "Bot Bot" TAB "MiniGame MiniGame");

function MiniGameSO::l4bMusic(%minigame,%datablock,%loopable,%type)
{
    for(%i=0;%i<%minigame.numMembers;%i++)    
    if(isObject(%mgmember = %minigame.member[%i])) %mgmember.l4bMusic(%datablock,%loopable,%type);    
}

function GameConnection::l4bMusic(%client,%datablock,%loopable,%type)
{   
    if(!isObject(%datablock)) return;

    switch$(%type)
    {
        case "Music": %channel = 10;
        case "Stinger1": %channel = 11;
        case "Stinger2": %channel = 11;
        case "Stinger3": %channel = 11;
        case "Ambience": %channel = 12;
        default: return;
    }



    if(isObject(%client.l4bMusic[%type])) %client.l4bMusic[%type].delete();

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

function MiniGameSO::deletel4bMusic(%minigame,%type)
{
    for(%i=0;%i<%minigame.numMembers;%i++)
    if(isObject(%mgmember = %minigame.member[%i])) %mgmember.deletel4bMusic(%type);
}

function GameConnection::deletel4bMusic(%client,%type)
{
    switch$(%type)
    {
        case "Music":
        case "Stinger1":
        case "Stinger2":
        case "Stinger3":
        case "Ambience":
        default: return;
    }

    if(isObject(%client.l4bMusic[%type])) %client.l4bMusic[%type].delete();
}