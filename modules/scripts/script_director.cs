registerOutputEvent(Minigame, "Director", "List Disable 0 Enable 1",0);
registerOutputEvent(Minigame, "RoundEnd");
registerOutputEvent(Minigame, "PanicRound");
registerOutputEvent(Minigame, "SafehouseCheck");
$L4B_Music["Music"] = 0;
$L4B_Music["Music2"] = 0;
$L4B_Music["Stinger1"] = 0;
$L4B_Music["Stinger2"] = 0;
$L4B_Music["Stinger3"] = 0;
$L4B_Music["Ambience"] = 0;

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
    %minigame.hordecount = 0;
    %minigame.directorMusicActive = false;
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
    %miniGame.director(0,0);

    if(isObject(%brickgroup = %client.brickgroup) && %brickgroup.getCount())
    for(%i = 0; %i < %brickgroup.getcount(); %i++) if(isObject(%brick = %brickgroup.getObject(%i)))
    {            
        if(%brick.getName() $= "_breakbrick")
        {
            %brick.setRendering(1);
            %brick.setRaycasting(1);
            %brick.setColliding(1);
            %brick.setName("");
        }
        if(%brick.getdataBlock().isOpen) %brick.door(close);
    }
    
    if(isObject(Director_ZombieGroup)) Director_ZombieGroup.delete();
    if(isObject(AreaZoneGroup)) for(%i = 0; %i < AreaZoneGroup.getCount(); %i++)
    if(isObject(%zone = AreaZoneGroup.getObject(%i)))
    {
        %zone.firstentry = false;
        %zone.presenceallentered = false;
        for(%j = 0; %j < %zone.simset.getCount(); %j++)
        if(isObject(%brick = %zone.simset.getObject(%j)) && %brick.getdataBlock().ZoneBrickType $= "item") %brick.setItem(none);
    }       
}

function MiniGameSO::SafehouseCheck(%minigame,%client)
{
	for(%i = 0; %i < %minigame.numMembers; %i++)
	{
		%client = %minigame.member[%i];
		if(isObject(%player = %client.player) && !%player.hIsInfected && %player.getdataBlock().getname() !$= "SurvivorPlayerDowned") %livePlayerCount++;
		if(isObject(%player) && %player.InSafehouse) %safehousecount++;
	}
	
	if(%safehousecount >= %livePlayerCount && isObject(%minigame))
	{
		if(isEventPending(%minigame.resetSchedule))	return;

   		%minigame.VictoryTo = "Survivors";
		%minigame.scheduleReset(8000);
		%minigame.l4bMusic("game_win_sound",false,"Music");
		%minigame.deletel4bMusic("Stinger1");
		%minigame.deletel4bMusic("Stinger2");
		%minigame.deletel4bMusic("Stinger3");	

    	for(%i=0;%i<%minigame.numMembers;%i++)
    	if(isObject(%member = %minigame.member[%i]) && isObject(%member.player))
        {
            if(%member.player.hType $= "Survivors") %member.player.emote(winStarProjectile, 1);	
            %member.Camera.setOrbitMode(%member.player, %member.player.getTransform(), 0, 5, 0, 1);
            %member.setControlObject(%member.Camera);
        }
		return true;
	}
}

function MinigameSO::L4B_PlaySound(%minigame,%sound)
{
    for(%i=0;%i<%minigame.numMembers;%i++) if(isObject(%client = %minigame.member[%i]) && %client.getClassName() $= "GameConnection") 
    %client.play2d(%sound.getID());    
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
                                    if(%mgmember.player.getdataBlock().isSurvivor)
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
                        if(%chance <= 40) %round = 2;
                        
                        if(%stressed) %round = 0;
                    

                        switch(%round)
                        {
                            case 0: %minigame.BreakRound();
                            case 1: %minigame.HordeRound();
                            case 2: %minigame.TankRound();
                        }                        
                case 2: 
            }            
        }

        if(%minigame.RoundType $= "Panic")
        {
            %minigame.spawnZombies("Horde",10);
            %minigame.spawnZombies("Special",2);      
            if(getRandom(1,10) == 1 ) %minigame.spawnZombies("Tank",1);
        }
        else switch(%interval)
        {
            case 1: %cue = true;            
            case 2: if(getRandom(1,2) == 1) %cue = true;
            
                    %minigame.spawnZombies("Horde",getRandom(2,5));
                    %minigame.spawnZombies("Special",getRandom(1,2));

            case 3: if(getRandom(1,2) == 1) %cue = true;
            default:
        }
        
        if(%cue && %minigame.directorMusicActive) switch$(%minigame.RoundType)
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

function MinigameSO::BreakRound(%minigame)
{    
    if(!%minigame.DirectorStatus || %minigame.directorMusicActive) return;

    %minigame.RoundType = "Break";
    %minigame.deletel4bMusic("Music");
    %minigame.deletel4bMusic("Music2");
    %minigame.deletel4bMusic("Stinger1");
    %minigame.deletel4bMusic("Stinger2");
    %minigame.deletel4bMusic("Stinger3");
    %minigame.l4bMusic("musicData_L4D_quarantine_" @ getRandom(1,3),false,"Music");
}

function MinigameSO::WitchRound(%minigame)
{
    if(!%minigame.DirectorStatus || !%minigame.spawnZombies("Witch",1)) return;
        
    %minigame.RoundType = "Witch";
    %minigame.DirectorStatus = 2;
}

function MinigameSO::TankRound(%minigame)
{
    if(!%minigame.DirectorStatus || %minigame.directorTankRound > 1 || !%minigame.spawnZombies("Tank",1)) return;    

    %minigame.directorTankRound++;
    %minigame.RoundType = "Tank";
    %minigame.DirectorStatus = 2;
}

function MinigameSO::PanicRound(%minigame)
{
    if(!%minigame.DirectorStatus) return;

    %minigame.DirectorStatus = 2;
    %minigame.RoundType = "Panic";
    %minigame.L4B_ChatMessage("[They're coming!] <bitmapk:Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_skull2>","hordeincoming" @ getrandom(1,9) @ "_sound",true); 
    %minigame.l4bMusic("musicData_L4D_horde_urgent",false,"Music2");
}

function MiniGameSO::HordeRound(%minigame)
{
    %random = getRandom(25,35);    
    if(!%minigame.DirectorStatus || !%minigame.spawnZombies("Horde",%random+5,0)) return;
    
    %minigame.DirectorStatus = 2;
    %minigame.RoundType = "Horde";
    %minigame.L4B_ChatMessage("[They're coming...]","hordeincoming" @ getrandom(1,9) @ "_sound",true); 
}

function MinigameSO::RoundEnd(%minigame)
{
    if(!%minigame.DirectorStatus) return;
    
    %minigame.RoundType = "";
    %minigame.directorMusicActive = false;
    %minigame.l4bMusic("drum_suspense_end_sound",false,"Stinger1");
    %minigame.deletel4bMusic("Music");
    %minigame.deletel4bMusic("Music2");
    %minigame.DirectorStatus = 1;
}

function MinigameSO::spawnZombies(%minigame,%type,%amount,%spawnzone,%count)
{
    if(!isObject(%spawnzone))//Just in case the zone wasn't listed then we can just choose from the area zone group and prioritize spawns
    {
        if(%type $= "Horde" || %type $= "Wander") %priority = 2;//Common zombies don't get the high priority so just make them spawn whereever
        else %priority = 1;//Specials, tanks and witches spawn with higher priority

        switch(%priority)
        {
            case 1: if(%minigame.numMembers) for(%i=0;%i <= %minigame.numMembers;%i++)//Create a list if there are lone survivors
                    if(isObject(%minigame.member[%i]) && isObject(%player = %minigame.member[%i].player) && %player.getdataBlock().isSurvivor && %player.currentZone.presencecount == 1)
                    {
                        %lonelysurvivorzone[%lsz++] = %player.currentZone;
                        %spawnzone = %lonelysurvivorzone[getRandom(1,%lsz)];
                    }

                    if(!%lsz)//Make a generic list in case there are no lone survivors
                    {
                        for(%i = 0; %i < AreaZoneGroup.getcount(); %i++) if(isObject(AreaZoneGroup.getObject(%i)) && AreaZoneGroup.getObject(%i).presencecount)
                        %activezones[%az++] = AreaZoneGroup.getObject(%i);
                        %spawnzone = %activezones[getRandom(1,%az)];
                    }
                    
                    if(!%spawnzone) return;

            case 2: for(%i = 0; %i < AreaZoneGroup.getcount(); %i++) if(isObject(AreaZoneGroup.getObject(%i)) && AreaZoneGroup.getObject(%i).presencecount)
                    {
                        %activezones[%az++] = AreaZoneGroup.getObject(%i);
                        %spawnzone = %activezones[getRandom(1,%az)];
                    }
                    if(!%spawnzone) return;
        }
    }    
    
    for(%i = 0; %i < %spawnzone.simset.getcount(); %i++)
    if(isObject(%setbrick = %spawnzone.simset.getObject(%i)) && %setbrick.getdataBlock().ZoneBrickType $= "spawner" && strstr(strlwr(%setbrick.getName()),"_" @ strlwr(%type)) != -1)
    {
        %spawnlist[%sb++] = %setbrick;
        %spawnlist[%sb].currentset = %spawnzone;
    }

    if(%sb && %count < %amount)
    {
        %random = getRandom(1,%sb);
        %spawnbrick = %spawnlist[%random];
        %zone = %spawnlist[%random].currentset;

        switch$(%type)
        {
            case "Wander": %bottype = "CommonZombieHoleBot";
            case "Horde": %bottype = "CommonZombieHoleBot";
                            //if(getRandom(1,8) == 1) %bottype = $hZombieUncommonType[getRandom(1,$hZombieUncommonTypeAmount)];
                            //if(getRandom(1,16) == 1 && $L4B_CurrentMonth == 10) %bottype = "SkeletonHoleBot";
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
            currentZone = %zone;
            isHoleBot = 1;
        };

        if(strlen(%bottype.hMeleeCI)) eval("%bot.hDamageType = $DamageType::" @ %bottype.hMeleeCI @ ";");
        else %bot.hDamageType = $DamageType::HoleMelee;        
        
        if(!isObject(Director_ZombieGroup))
        {
            new SimGroup(Director_ZombieGroup);
            missionCleanup.add(Director_ZombieGroup);
        }
        
        Director_ZombieGroup.add(%bot);
        %bot.doMRandomTele(%spawnbrick);

        cancel(%minigame.spawn[%type]);
        %minigame.spawn[%type] = %minigame.scheduleNoQuota(50,spawnZombies,%type,%amount,%spawnzone,%count++);
        return true;
    }
    else return false;
}
registerInputEvent("fxDTSBrick","onBotTeleSpawn","Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "Bot Bot" TAB "MiniGame MiniGame");
registerOutputEvent("Bot","doMRandomTele","string 20 100");
registerOutputEvent("Player","doMRandomTele","string 20 100");

function Player::doMRandomTele(%obj,%targetbrick)
{			
    if(!isObject(%targetbrick))
	{		
		if(isObject(AreaZoneGroup) && AreaZoneGroup.getCount()) 
		{
			for(%i = 0; %i < AreaZoneGroup.getCount(); %i++) if(isObject(%zone = AreaZoneGroup.getObject(%i)) && %zone.presencecount)
			for(%j = 0; %j < %zone.simset.getCount(); %j++) if(isObject(%brick = %zone.simset.getObject(%j)) && %brick.getdataBlock().ZoneBrickType $= "spawner" && strstr(strlwr(%brick.getName()),"_horde") != -1)
			{
				%teleportlist[%tb++] = %brick;
				%teleportlistzone[%tb] = %zone;
			}
			%random = getRandom(1,%tb);
			if(!%tb) return false;
			else 
            {
                %targetbrick = %teleportlist[%random];
                %obj.currentZone = %teleportlistzone[%random];
                %obj.spawnType = "Horde";
            }
		}
		else return false;        
	}	
    
    %obj.settransform(vectorAdd(getwords(%targetbrick.gettransform(),0,2),"0 0 0.25"));
    %obj.setvelocity(%obj.getvelocity());

    $InputTarget_["Self"] = %targetbrick;
    switch$(%obj.getclassname())
    {
        case "Player":	$InputTarget_["Player"] = %obj;
                        $InputTarget_["Client"] = %obj;
        case "AIPlayer": $InputTarget_["Bot"] = %obj;
    }
    $InputTarget_["MiniGame"] = getMiniGameFromObject(%obj);
    %targetbrick.processInputEvent("onBotTeleSpawn",%targetbrick.getgroup().client);
}

function AIPlayer::doMRandomTele(%obj,%targetbrick)
{
    Player::doMRandomTele(%obj,%targetbrick);
}

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

function L4B_isInFOV(%viewer, %target)
{    
    if(isObject(%viewer) && isObject(%target)) return vectorDot(%viewer.getEyeVector(), vectorNormalize(vectorSub(%target.getPosition(), %viewer.getPosition()))) >= 0.7;
}