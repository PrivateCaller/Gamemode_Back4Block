LoadRequiredAddOn("Brick_Halloween");
datablock fxDTSBrickData (brickL4BDirectorData : brickSkullData)
{
	subCategory = "Left 4 BLock";
	uiName = "Director Block";
};

function brickL4BDirectorData::onPlant(%this, %obj)
{
    Parent::onPlant(%this, %obj);
    
    if(!isObject(directorBricks))
    {
        new SimSet(directorBricks);
        directorBricks.add(%obj);
        MissionCleanup.add(directorBricks);
    }
    else if(isObject(directorBricks))
    directorBricks.add(%obj);
}

function brickL4BDirectorData::onloadPlant(%this, %obj)
{
    Parent::onloadPlant(%this,%obj);
    %this.onPlant(%obj);
}

function brickL4BDirectorData::onDeath(%this, %obj)
{
	if(isObject(directorBricks) && directorBricks.isMember(%obj))
    directorBricks.remove(%obj);

	Parent::onDeath(%this,%brick);
}

function fxDTSBrick::spawnHoleBots(%obj,%count)
{
    if(%count >= %obj.BotHoleAmount)
    {
        %obj.BotHoleAmount = 0;
        return;
    }

    if(%obj.lastTypeSpawned $= "Tank")
    {
        %obj.respawnBot();
        %obj.hBot = 0;
    }
    else
    {
        %setcheck = "Bot_" @ %obj.lastTypeSpawned;
        switch$(%obj.lastTypeSpawned)
        {
            case "Horde": %maxcheck = $Pref::Server::L4B2Bots::MaxHorde;
            case "Special": %maxcheck = $Pref::Server::L4B2Bots::MaxSpecial;

        }

        if(isObject(%setcheck))
        if(%setcheck.getCount() <= %maxcheck)
        {
            %obj.respawnBot();
            %obj.hBot = 0;
        }
    }

    cancel(%obj.spawnBots);
    %obj.spawnBots = %obj.schedule(500,spawnHoleBots,%count++);
}

registerInputEvent("fxDTSBrick", "onDirectorInterval", "Self fxDTSBrick" TAB "MiniGame MiniGame");
registerOutputEvent(Minigame, "Director", "List Disable 0 Enable 1 Horde 2 Tank 3 Panic 4",0);
registerOutputEvent(Minigame, "RoundEnd");

function MinigameSO::director(%minigame,%choice,%client)
{
    switch(%choice)
    {
        case 0: if(isObject(l4b_music))
                l4b_music.delete();
        
                if(!%minigame.DirectorStatus)
                return;
                else
                {
                    %minigame.DirectorStatus = 0;
                    cancel(%minigame.directorSchedule);
                }

        case 1: if(%minigame.DirectorStatus)
                return;
                else
                {
                    %minigame.DirectorStatus = 1;

                    if(isObject(l4b_music)) 
                    l4b_music.delete();

                    if($Pref::L4BDirector::EnableCues)
                    %minigame.directorPlaySound("hordeincoming" @ getrandom(1,9) @ "_sound",%client);

                    cancel(%minigame.directorSchedule);
                    %minigame.directorSchedule = %minigame.schedule(10000,directorAI,%client);
                }
        case 2: if(%minigame.DirectorStatus)
                %minigame.HordeRound(getRandom(1,2),%client);
                
        case 3: if(%minigame.DirectorStatus)
                %minigame.TankRound(%client);

        case 4: if(%minigame.DirectorStatus)
                %minigame.PanicRound(%client);
    }

}

function MiniGameSO::directorAI(%minigame,%directorInterval,%client)
{    
    if(%directorInterval > 3)
    %directorInterval = 0;

    %minigame.DirectorProcessEvent("onDirectorInterval",%client);

    cancel(%minigame.directorSchedule);
    %minigame.directorSchedule = %minigame.schedule(10000,directorAI,%directorInterval++,%client);
    
    switch(%directorInterval)
    {
        case 1: %minigame.spawnZombies("Special",1,%client);
        case 2: %minigame.spawnZombies("Special",1,%client);

                if(%minigame.DirectorStatus == 2)
                %minigame.spawnZombies("Horde",25,%client);

        case 3: switch(%minigame.DirectorStatus)
                {
                    case 0: return;

                    case 1: for(%i=0;%i<%minigame.numMembers;%i++)
                            {
                                %fixcount = %i+1;
                                if(isObject(%mgmember = %minigame.member[%i]))
                                {
                                    %minigame.survivorStatHealthMax = 100*%fixcount;
                                    if(isObject(%mgmember.player) && %mgmember.player.getdataBlock().getName() !$= "DownPlayerSurvivorArmor")
                                    %health += %mgmember.player.getdamagelevel();
                                    else %health += 100;
                                    %health = mClamp(%health,0,%minigame.survivorStatHealthMax);

                                    %minigame.survivorStatHealthAverage = %minigame.survivorStatHealthMax-%health;
                                }
                            }

                            if(%minigame.survivorStatHealthAverage < %minigame.survivorStatHealthMax/2)
                            %round = getrandom(0,1);
                            else %round = getrandom(1,3);

                            switch(%round)
                            {
                                case 0: %minigame.BreakRound(%client);
                                case 1: %minigame.HordeRound(getRandom(1,2),%client);
                                case 2: %minigame.HordeRound(getRandom(1,2),%client);
                                case 3: %minigame.TankRound(%client);
                            }

                    case 2:   
                }
    }
}

function MinigameSO::BreakRound(%minigame,%client)
{    
    if(getRandom(1,100) <= 25)
    {
        if($Pref::L4BDirector::EnableCues)
        {
            %minigame.directorPlaySound("zombiechoir_0" @ getrandom(1,6) @ "_sound",%client);

            cancel(%minigame.choirSFX);
            %minigame.choirSFX = %minigame.schedule(15000,directorPlaySound,"zombiechoir_0" @ getrandom(1,6) @ "_sound",%client);

            %minigame.DirectorMusic("musicdata_L4D_background" @ getRandom(1,3),%client);
            l4b_music.schedule(30000,delete);
        }
    }
}

function MiniGameSO::hordeMusic(%minigame,%client)
{
    %minigame.DirectorMusic("musicData_L4D_drum_suspense" @ getRandom(1,2),%client);
    %minigame.directorPlaySound("drum_suspense_end_sound",%client);
}

function MinigameSO::TankRound(%minigame,%client)
{
    if(!%minigame.DirectorStatus)
    return;

    if(%minigame.directorTankRound < $Pref::Server::L4B2Bots::TankRounds)
    {
        %minigame.directorTankRound++;
        %minigame.DirectorStatus = 2;

        if($Pref::L4BDirector::EnableCues)
        %minigame.DirectorMusic("musicdata_L4D_tank",%client);

        %minigame.spawnZombies("Horde",50,%client);
        %minigame.spawnZombies("Special",1,%client);
        %minigame.spawnZombies("Tank",1,%client);
    } 
}

function MinigameSO::PanicRound(%minigame,%client)
{
    %minigame.DirectorStatus = 2;

    if($Pref::L4BDirector::EnableCues)
    %minigame.DirectorMusic("musicdata_L4D_skin_on_our_teeth",%client);

    %minigame.spawnZombies("Horde",100,%client);
    %minigame.spawnZombies("Special",4,%client);
    %minigame.DirectorStatus = 0;
    cancel(%minigame.directorSchedule);
}

function MiniGameSO::HordeRound(%minigame,%type,%client)
{
    if(!%minigame.DirectorStatus)
    return;

    %minigame.DirectorStatus = 2;

    switch(%type)
    {
        case 1: if($Pref::L4BDirector::EnableCues)
                {
                    %minigame.directorPlaySound("hordeincoming" @ getrandom(1,9) @ "_sound",%client);

                    cancel(%minigame.hordeMusic);
                    %minigame.hordeMusic = %minigame.schedule(4000,hordeMusic,%client);

                    cancel(%minigame.hordeEndShed);
                    %minigame.hordeEndShed = %minigame.schedule(30000,roundEnd,%client);
                }

                %minigame.spawnZombies("Horde",50,%client);

        case 2: %minigame.spawnZombies("Horde",25,%client);
    }
}

function MinigameSO::RoundEnd(%minigame,%client)
{    
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
    
    %minigame.directorPlaySound("drum_suspense_end_sound",%client);
    %minigame.DirectorStatus = 1;
}

//Reusable functions
function MiniGameSO::removeZombieBots(%minigame, %mode, %client)
{
    if(isObject(directorBricks) && directorBricks.getCount() > 0)
    {
        for (%i = 0; %i < directorBricks.getCount(); %i++) 
        {
            %brick = directorBricks.getObject(%i);
            if(%brick.getdatablock().isZombieBrick)
            if(isObject(%brick.hBot))
            {
                if(%mode $= "clear")
                {
                    %brick.hBot.schedule(100,delete);
                    %brick.hBot = 0;
                    %brick.itemRespawnTime = 360000;
                    cancel(%brick.hModS);
                }
                else if(%mode $= "dissipate")
                {
                    %brick.itemRespawnTime = 360000;
                    cancel(%brick.hModS);
                }
            }
        }
    }
}

function MinigameSO::DirectorProcessEvent(%minigame,%event,%client)
{
    if(isObject(directorBricks))
    {
        for(%i = 0; %i < directorBricks.getCount(); %i++)
        {
            %brick = directorBricks.getObject(%i);
            %brick.processInputEvent(%event,%client);
            %brick.setColorFX(2);
            %brick.schedule(250,setColorFX,0);
        }
    }
}

function MinigameSO::spawnZombies(%minigame,%type,%amount,%client)
{
    if(isObject(directorBricks) && directorBricks.getCount() > 0)
    {
        for (%i = 0; %i < directorBricks.getCount(); %i++) 
        {
            %brick = directorBricks.getObject(%i);

            if(%brick.getdatablock().isZombieBrick && strstr(%brick.getname(), %type) != -1)
            {
                %brick.itemRespawnTime = %respawntime;
                %brick.BotHoleAmount = %amount;
                %brick.lastTypeSpawned = %type;
                %brick.spawnHoleBots(0);

                break;
            }
        }
    }
}

function MiniGameSO::DirectorMusic(%minigame,%music,%client)
{
    if(isObject(l4b_music)) 
    l4b_music.delete();

    if(isObject(%minigame.member0.player))
    %pos = %minigame.member0.player.getPosition();
    else %pos = "0 0 0";

    new AudioEmitter(l4b_music)
    {
        position = %pos;
        profile = %music.getID();
        isLooping= true;
        is3D = 0;
        volume = 1;
        useProfileDescription = "0";
        type = "0";
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