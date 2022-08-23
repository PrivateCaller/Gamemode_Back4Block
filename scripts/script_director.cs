
registerOutputEvent(Minigame, "Director", "List Disable 0 Enable 1",0);
registerOutputEvent(Minigame, "RoundEnd");

function MinigameSO::Director(%minigame,%enabled,%interval,%client)
{
    switch(%enabled)
    {
        case 0: if(isObject(l4b_music))
                l4b_music.delete();

                if(%minigame.DirectorStatus)
                {
                    %minigame.DirectorStatus = 0;
                    cancel(%minigame.directorSchedule);
                }
                return;

        case 1: if(!%minigame.DirectorStatus)
                {
                    %minigame.DirectorStatus = 1;

                    if(isObject(l4b_music)) 
                    l4b_music.delete();

                    %minigame.directorPlaySound("hordeincoming" @ getrandom(1,9) @ "_sound",%client);
                }
    }

    if(%minigame.DirectorStatus)
    {    
        if(%interval > 3)
        %interval = 0;

        switch(%interval)
        {
            case 1: %minigame.spawnZombies("Special",1,0,%client);
            case 2: %minigame.spawnZombies("Special",getRandom(1,2),0,%client);
            
                    if(%minigame.DirectorStatus == 2)
                    %minigame.spawnZombies("Horde",10,0,%client);

                    %minigame.SpawnStalkZombies();

            case 3: switch(%minigame.DirectorStatus)
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
                             
                                %round = 1;
                                if(getRandom(1,2) == 1)
                                %round = 2;
                                if(getRandom(1,10) == 1)
                                %round = 3;
                                if(getRandom(1,16) < $Pref::Server::L4B2Bots::TankRoundChance)
                                %round = 4;

                                if(%stressed)
                                %round = 0;

                                switch(%round)
                                {
                                    case 0: %minigame.BreakRound(%client);
                                    case 1: %minigame.HordeRound(1,%client);
                                    case 2: %minigame.HordeRound(2,%client);
                                    case 3: %minigame.WitchRound(%client);
                                    case 4: %minigame.TankRound(%client);
                                }
                        case 2: %minigame.SpawnStalkZombies();
                    }
        }

        cancel(%minigame.directorSchedule);
        %minigame.directorSchedule = %minigame.schedule(10000,Director,1,%interval++,%client);
    }
}

function MinigameSO::SpawnStalkZombies(%minigame)//Go after others who are on their own
{
    for(%i=0;%i <= %minigame.numMembers;%i++)//Calculate overall variables
    {    
        if(isObject(%mgmember = %minigame.member[%i]))
        {
            if(isObject(%player = %mgmember.player) && %player.getdataBlock().isSurvivor)
            {                                

                %survivorcount++;
                %survivorplayer[%sc++] = %player;

                //%minigame.survivorAreaZoneNum[%saz++] = %player.AreaZoneNum;
                //%minigame.survivorAreaZoneNumPlayer[%saz] = %player;
            }
        }
    }

    for (%j = 1; %j <= %sc; %j++) 
    {
        if(%survivorcount > 1 && %survivorplayer[%j].survivorAllyCount == 1 && %survivorplayer[%j].currAreaZone != %survivorplayer[%j+1].currAreaZone)
        {
            %minigame.spawnZombies(Horde,getRandom(10,20),%player.currAreaZone,%client);
            %minigame.spawnZombies(Special,getRandom(1,2),%player.currAreaZone,%client);
        }
    }

                    

    //if(%saz > 1)//Archiving this for another time
    //{
    //    %highestZone = 0;
    //    for(%sazc = 1; %sazc <= %saz; %sazc++)
    //    {    
    //        %previous = %minigame.survivorAreaZoneNum[%sazc-1];
    //        if(%minigame.survivorAreaZoneNum[%sazc] > %highestZone)//Find whoever is in the furthest zone
    //        {
    //            %highestZone = %minigame.survivorAreaZoneNum[%sazc];
    //            %highestZonePlayer = %minigame.survivorAreaZoneNumPlayer[%sazc];
    //        }
    //        
    //        if(%minigame.survivorAreaZoneNum[%sazc] == %previous)
    //        {
    //            %highzonecount++;
    //
    //            if(%highzonecount > %saz/1.333333)
    //            {
    //                %highestZone = 0;
    //                %highestZonePlayer = 0;
    //            }
    //        }
    //    }
    //    
    //    if(isObject(%highestZonePlayer))//Where are your friends when you need them?
    //    {
    //        %minigame.spawnZombies(Horde,getRandom(10,20),%highestZonePlayer.currAreaZone);
    //        %minigame.spawnZombies(Special,getRandom(1,4),%highestZonePlayer.currAreaZone);
    //    }
    //}
}

function MinigameSO::BreakRound(%minigame,%client)
{    
    %minigame.directorPlaySound("zombiechoir_0" @ getrandom(1,6) @ "_sound",%client);

    cancel(%minigame.choirSFX);
    %minigame.choirSFX = %minigame.schedule(15000,directorPlaySound,"zombiechoir_0" @ getrandom(1,6) @ "_sound",%client);

    %minigame.DirectorMusic("musicdata_L4D_background" @ getRandom(1,3),false,1,%client);

    if(isObject(l4b_music))
    l4b_music.schedule(30000,delete);
}

function MinigameSO::WitchRound(%minigame,%client)
{
    if(!%minigame.DirectorStatus || !isObject(MainAreaZone))
    return;    

    for(%i = 0; %i < MainAreaZone.getCount(); %i++) 
    {				
        if(strstr(strlwr(MainAreaZone.getObject(%i).getName()),"witch") != -1)
        %spawn++;
    }
    if(!%spawn)
    return;

    %minigame.spawnZombies("Witch",1,0,%client);
    %minigame.DirectorStatus = 2;
}

function MinigameSO::TankRound(%minigame,%client)
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

        %minigame.spawnZombies("Tank",1,0,%client);
    } 
}

function MinigameSO::PanicRound(%minigame,%client)
{
    if(!%minigame.DirectorStatus || !isObject(MainAreaZone))
    return;    

    for(%i = 0; %i < MainAreaZone.getCount(); %i++) 
    {				
        if(strstr(strlwr(MainAreaZone.getObject(%i).getName()),"horde") != -1)
        %spawn++;
    }
    if(!%spawn)
    return;
    
    %minigame.DirectorStatus = 2;

    if($Pref::L4BDirector::EnableCues)
    %minigame.DirectorMusic("musicdata_L4D_skin_on_our_teeth",true,1,%client);

    %minigame.spawnZombies("Horde",25,0,%client);
    %minigame.DirectorStatus = 0;
    cancel(%minigame.directorSchedule);
}

function MiniGameSO::HordeRound(%minigame,%type,%client)
{
    if(!%minigame.DirectorStatus || !isObject(MainAreaZone))
    return;    

    for(%i = 0; %i < MainAreaZone.getCount(); %i++) 
    {				
        if(strstr(strlwr(MainAreaZone.getObject(%i).getName()),"horde") != -1)
        %spawn++;
    }
    if(!%spawn)
    return;
    
    if(getRandom(1,10) == 1)
    {
        %minigame.UrgentRound = 1;
        %type = 2;
    }
    else %minigame.UrgentRound = 0;

    switch(%type)
    {
        case 1: %minigame.spawnZombies("Horde",25,0,%client);
        case 2: %minigame.DirectorStatus = 2;
                if(%minigame.UrgentRound)
                {
                    if(isObject(%minigame.member0.player))
                    %pos = %minigame.member0.player.getPosition();
                    else %pos = "0 0 0";

                    %musicnum = "musicdata_L4D_horde_urgent";
                    new AudioEmitter(l4b_music_urgent)
                    {
                        position = %pos;
                        profile = %musicnum.getID();
                        isLooping = false;
                        is3D = 0;
                        volume = 1;
                        useProfileDescription = "0";
                        type = 9;
                        outsideAmbient = "1";
                        referenceDistance = "2";
                        maxDistance = 999999;
                        enableVisualFeedback = "0";
                    };

                    %minigame.schedule(4000,DirectorMusic,"musicdata_L4D_horde_combat",true,1,%client);
                    %minigame.schedule(4000,directorPlaySound,"drum_suspense_end_sound",%client);

                    %minigame.schedule(9000,directorPlaySound,"hordeslayer_0" @ getRandom(1,3) @ "_sound",%client);
                    %minigame.schedule(19000,directorPlaySound,"hordeslayer_0" @ getRandom(1,3) @ "_sound",%client);
                    %minigame.schedule(19000,directorPlaySound,"horde_danger_sound",%client);
                    %minigame.directorPlaySound("zombiehorde_sound",%client);
                    %minigame.spawnZombies("Horde",50,0,%client);
                }
                else
                {
                    %minigame.spawnZombies("Horde",25,0,%client);
                    %minigame.directorPlaySound("hordeincoming" @ getrandom(1,9) @ "_sound",%client);

                    %minigame.schedule(9000,directorPlaySound,"hordeslayer_0" @ getRandom(1,3) @ "_sound",%client);
                    %minigame.schedule(19000,directorPlaySound,"hordeslayer_0" @ getRandom(1,3) @ "_sound",%client);
                    %minigame.schedule(19000,directorPlaySound,"horde_danger_sound",%client);
                    %minigame.schedule(4000,DirectorMusic,"musicdata_L4D_horde_combat",true,1,%client);
                    %minigame.schedule(4000,directorPlaySound,"drum_suspense_end_sound",%client);
                }

                cancel(%minigame.hordeEndShed);
                %minigame.hordeEndShed = %minigame.schedule(30000,roundEnd,%client);
    }
}

function MinigameSO::RoundEnd(%minigame,%client)
{    
    if(isObject(l4b_music))
    l4b_music.delete();
    
    %minigame.directorPlaySound("drum_suspense_end_sound",%client);
    %minigame.DirectorStatus = 1;
    %minigame.UrgentRound = 0;
    %minigame.SoldierTank = 0;
}

function MinigameSO::spawnZombies(%minigame,%type,%amount,%spawnset,%client)
{    
    if(!isObject(MainAreaZone) || MainAreaZone.getCount() <= 0)
    return;

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
    for (%b = 0; %b < %amount; %b++) 
    {
        if(%b > %amount)
        break;
        
        %spawnbrick = %spawnlist[getRandom(1,%sb)];

        switch$(%type)
        {
            case "Horde": %bottype = "CommonZombieHoleBot";
                          if(getRandom(1,4) == 1)
                          %bottype = $hZombieUncommonType[getRandom(1,$hZombieUncommonTypeAmount)];
            case "Wander": %bottype = "CommonZombieHoleBot";
                           if(getRandom(1,4) == 1)
                           %bottype = $hZombieUncommonType[getRandom(1,$hZombieUncommonTypeAmount)];
            case "Tank": %bottype = "ZombieTankHoleBot";
            case "Witch": %bottype = "ZombieWitchHoleBot";
            case "Special": %bottype = $hZombieSpecialType[getRandom(1,$hZombieSpecialTypeAmount)];
        }

        %bot = new AIPlayer()
        {
            dataBlock = %bottype;
            path = "";
            spawnBrick = %spawnbrick;

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
        %spawnbrick.processInputEvent("onDirectorBotSpawn",%spawnbrick.getgroup().client);

        if(strlen(%bottype.hMeleeCI))
        eval("%bot.hDamageType = $DamageType::" @ %bottype.hMeleeCI @ ";");
        else %bot.hDamageType = $DamageType::HoleMelee;
        %bot.setTransform(%spawnbrick.getposition() SPC getwords(%spawnbrick.gettransform(),3,6));

        if(isObject(L4B_BotSet))
        L4B_BotSet.add(%bot);
        else
        {
            new SimSet(L4B_BotSet);
            missionCleanup.add(L4B_BotSet);
            L4B_BotSet.add(%bot);
        }
    }
}
 registerInputEvent("fxDTSBrick","onDirectorBotSpawn","Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "Bot Bot" TAB "MiniGame MiniGame");

function MiniGameSO::DirectorMusic(%minigame,%music,%loopable,%volume,%client)
{
    if(isObject(l4b_music)) 
    l4b_music.delete();

    if(isObject(%minigame.member[0].player))
    %pos = %minigame.member[0].player.getPosition();
    else %pos = "0 0 0";

    new AudioEmitter(l4b_music)
    {
        position = %pos;
        profile = %music.getID();
        isLooping= true;
        is3D = 0;
        volume = %volume;
        useProfileDescription = "0";
        type = 9;
        outsideAmbient = "1";
        referenceDistance = "2";
        maxDistance = 999999;
        enableVisualFeedback = "0";
    };
}

function MinigameSO::directorPlaySound(%minigame,%sound,%client)
{
    for(%i=0;%i<%minigame.numMembers;%i++)
    {
        %cl=%minigame.member[%i];

        if(isObject(%cl) && %cl.getClassName() $= "GameConnection")
        %cl.play2d(%sound.getID());
    }
}