%pattern = "add-ons/gamemode_left4block/add-ins/script_director/sound/*.wav";//Too lazy to write datablock files for the sounds, just took this from the Disease Gamemode
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/gamemode_left4block/add-ins/script_director/sound/", "");
	%soundName = strreplace(%soundName, "/", "");
	%soundName = strreplace(%soundName, ".wav", "");
	%soundName = strreplace(%soundName, "quiet", "");
	%soundName = strreplace(%soundName, "normal", "");
	%soundName = strreplace(%soundName, "loud", "");

	//Check the names of the folders to determine what type of soundscape it will be, and check if it's a loopable sound or not
	if(strstr(%file,"normal") != -1)//Normal soundscape
	if(strstr(%file,"loop") != -1)
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioCloseLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClose3d; filename = \"" @ %file @ "\"; };");

	if(strstr(%file,"quiet") != -1)//Quiet soundscape
	if(strstr(%file,"loop") != -1)
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosestLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosest3d; filename = \"" @ %file @ "\"; };");

	if(strstr(%file,"loud") != -1)//Loudest Soundscape
	if(strstr(%file,"loop") != -1)
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioDefaultLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioDefault3d; filename = \"" @ %file @ "\"; };");

	%file = findNextFile(%pattern);
}

registerOutputEvent(Minigame, "Director", "List Disable 0 Enable 1",0);
registerOutputEvent(Minigame, "RoundEnd");

function MinigameSO::L4B_PlaySound(%minigame,%sound)
{
    for(%i=0;%i<%minigame.numMembers;%i++)
    {
        %cl=%minigame.member[%i];

        if(isObject(%cl) && %cl.getClassName() $= "GameConnection") %cl.play2d(%sound.getID());
    }
}

function MinigameSO::Director(%minigame,%enabled,%interval)
{
    switch(%enabled)
    {
        case 0: if(isObject(l4b_music)) l4b_music.delete();

                if(%minigame.DirectorStatus)
                {
                    %minigame.DirectorStatus = 0;
                    cancel(%minigame.directorSchedule);
                }
                return;

        case 1: if(!%minigame.DirectorStatus)
                {
                    %minigame.DirectorStatus = 1;
                    if(isObject(l4b_music)) l4b_music.delete();
                    %minigame.L4B_PlaySound("hordeincoming" @ getrandom(1,9) @ "_sound",%client);
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
                            if(%chance <= 40) %round = 1;
                            else if(%chance <= $Pref::Server::L4B2Bots::TankRoundChance) %round = 2;
                            else if(%chance <= 20) %round = 3;

                            if(%stressed) %round = 0;

                            switch(%round)
                            {
                                case 0: %minigame.BreakRound();
                                case 1: %minigame.HordeRound();
                                case 2: %minigame.WitchRound();
                                case 3: %minigame.TankRound();
                            }
                        }
                case 2: 
            }            
        }

        if(%interval == 2)
        {
            if(!%stressed) %spawnchance = 4;
            else %spawnchance = 3;

            if(getRandom(1,%spawnchance) == 1) %minigame.spawnZombies("Horde",10,0);
            if(getRandom(1,%spawnchance-2) == 1) %minigame.spawnZombies("Special",getRandom(1,2),0);

            %minigame.SpawnStalkZombies();
        }        

        cancel(%minigame.directorSchedule);
        %minigame.directorSchedule = %minigame.schedule(10000,Director,1,%interval++);
    }
}

function MinigameSO::SpawnStalkZombies(%minigame)//Go after others who are on their own
{
    for(%i=0;%i <= %minigame.numMembers;%i++)//Calculate overall variables
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
    //%minigame.L4B_PlaySound("zombiechoir_0" @ getrandom(1,6) @ "_sound");
    //%minigame.choirSFX = %minigame.schedule(15000,L4B_PlaySound,"zombiechoir_0" @ getrandom(1,6) @ "_sound");
    //%minigame.DirectorMusic("musicdata_L4D_background" @ getRandom(1,3),false,1);
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
        if(strstr(strlwr(MainAreaZone.getObject(%i).getName()),"horde") != -1)
        %spawn++;
    }
    
    if(!%spawn)
    return;
    
    %minigame.DirectorStatus = 2;

    if($Pref::L4BDirector::EnableCues)
    %minigame.DirectorMusic("musicdata_L4D_skin_on_our_teeth",true,1);

    announce("[Time to escape]");

    %minigame.spawnZombies("Horde",25,0);
    %minigame.DirectorStatus = 2;
    cancel(%minigame.directorSchedule);
}

function MiniGameSO::HordeRound(%minigame)
{
    if(!%minigame.DirectorStatus || !isObject(MainAreaZone)) return;

    for(%i = 0; %i < MainAreaZone.getCount(); %i++) 
    {				
        if(strstr(strlwr(MainAreaZone.getObject(%i).getName()),"horde") != -1) %spawn++;
    }
    if(!%spawn) return;

    %minigame.zhordecount = 1; 
    %minigame.DirectorStatus = 2;

    %random = getRandom(30,45);
    %minigame.zhordecount = %random;
    %minigame.spawnZombies("Horde",%random,0);
    %minigame.L4B_PlaySound("hordeincoming" @ getrandom(1,9) @ "_sound");
    announce("[Incoming]");

    //%minigame.schedule(9000,L4B_PlaySound,"hordeslayer_0" @ getRandom(1,3) @ "_sound");
    //%minigame.schedule(19000,L4B_PlaySound,"hordeslayer_0" @ getRandom(1,3) @ "_sound");
    //%minigame.schedule(19000,L4B_PlaySound,"horde_danger_sound");

    %minigame.schedule(4000,L4B_PlaySound,"drum_suspense_end_sound");
    %minigame.schedule(4000,DirectorMusic,"musicdata_L4D_horde_combat",true,1);
    
}

function MinigameSO::RoundEnd(%minigame)
{    
    if(isObject(l4b_music))
    l4b_music.delete();
    
    %minigame.L4B_PlaySound("drum_suspense_end_sound");
    %minigame.DirectorStatus = 1;
    %minigame.UrgentRound = 0;
    %minigame.SoldierTank = 0;
}

function MinigameSO::spawnZombies(%minigame,%type,%amount,%spawnset,%count)
{    
    if(!isObject(MainAreaZone) || MainAreaZone.getCount() <= 0) return;

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
                    case "Horde": %bottype = "CommonZombieHoleBot";
                                  if(getRandom(1,16) == 1)
                                  %bottype = $hZombieUncommonType[getRandom(1,$hZombieUncommonTypeAmount)];
                    case "Wander": %bottype = "CommonZombieHoleBot";
                                   if(getRandom(1,16) == 1)
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
        else if(%count < %amount)
        {
            %spawnbrick = %spawnlist[getRandom(1,%sb)];

            switch$(%type)
            {
                case "Horde": %bottype = "CommonZombieHoleBot";
                                if(getRandom(1,8) == 1)
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

            cancel(%minigame.spawn[%type]);
            %minigame.spawn[%type] = %minigame.scheduleNoQuota(100,spawnZombies,%type,%amount,%spawnset,%count++);
        }
    }
}
registerInputEvent("fxDTSBrick","onDirectorBotSpawn","Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "Bot Bot" TAB "MiniGame MiniGame");

function MiniGameSO::DirectorMusic(%minigame,%music,%loopable,%volume)
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