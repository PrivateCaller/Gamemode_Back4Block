
LoadRequiredAddOn("Brick_Halloween");
datablock fxDTSBrickData (brickL4BDirectorData : brickSkullData)
{
	subCategory = "Interactive";
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

registerInputEvent("fxDTSBrick", "onDirectorInterval", "Self fxDTSBrick" TAB "MiniGame MiniGame");
registerInputEvent("fxDTSBrick", "onDirectorLvlShStart", "Self fxDTSBrick" TAB "MiniGame MiniGame");
registerInputEvent("fxDTSBrick", "onDirectorLvlShEnd", "Self fxDTSBrick" TAB "MiniGame MiniGame");
registerInputEvent("fxDTSBrick", "onDirectorLvlLhStart", "Self fxDTSBrick" TAB "MiniGame MiniGame");
registerInputEvent("fxDTSBrick", "onDirectorLvlLhEnd", "Self fxDTSBrick" TAB "MiniGame MiniGame");
registerInputEvent("fxDTSBrick", "onDirectorLvlTankStart", "Self fxDTSBrick" TAB "MiniGame MiniGame");
registerInputEvent("fxDTSBrick", "onDirectorLvlTankEnd", "Self fxDTSBrick" TAB "MiniGame MiniGame");
registerInputEvent("fxDTSBrick", "onDirectorLvlPanicStart", "Self fxDTSBrick" TAB "MiniGame MiniGame");
registerInputEvent("fxDTSBrick", "onDirectorLvlPanicEnd", "Self fxDTSBrick" TAB "MiniGame MiniGame");
registerInputEvent("fxDTSBrick", "onDirectorSpecial", "Self fxDTSBrick" TAB "MiniGame MiniGame");
registerOutputEvent(Minigame, "Director", "List Disable 0 Enable 1 HordeS 2 HordeL 3 Tank 4 Panic 5",0);
registerOutputEvent(Minigame, "TankRoundEnd");
registerOutputEvent(Minigame, "PanicRoundEnd");

function MinigameSO::director(%minigame,%choice,%client)
{   
    %directorinterval = $Pref::L4BDirector::Director_Interval*1000;
    %directorintervalhalf = %directorinterval/2;
    
    switch(%choice)
    {
        case 0: if(isObject(l4b_music))
                l4b_music.delete();
        
                if(!%minigame.isDirectorEnabled)
                return;
                else
                {
                    if($Pref::L4BDirector::AllowMGMessages)
                    MiniGameSO::ChatMsgAll (%minigame, "<bitmapk:add-ons/package_left4block/icons/ci_skull> Director", %client);

                    %minigame.isDirectorEnabled = 0;
                    cancel(%minigame.directorSchedule);
		        	cancel(directorSpecialSchedule);
                }

        case 1: if(%minigame.isDirectorEnabled)
                return;
                else
                {
                    %minigame.isDirectorEnabled = 1;

                    if(isObject(l4b_music)) 
                    l4b_music.delete();

                    if($Pref::L4BDirector::AllowMGMessages)
                    MiniGameSO::ChatMsgAll (%minigame, "\c3Director activated.", %client);

                    if($Pref::L4BDirector::EnableCues)
                    %minigame.directorPlaySound("hordeincoming" @ getrandom(1,9) @ "_sound",%client);

                    cancel(directorSpecialSchedule);
                    %minigame.directorSpecialSchedule = %minigame.schedule(%directorintervalhalf,directorSpecial,%client);
                    cancel(%minigame.directorSchedule);
                    %minigame.directorSchedule = %minigame.schedule(%directorinterval,directorChoose,%client);
                }
        case 2: if(%minigame.isDirectorEnabled)
                %minigame.SmallHorde(%client);
        case 3: if(%minigame.isDirectorEnabled)
                %minigame.LargeHorde(%client);
        case 4: if(%minigame.isDirectorEnabled)
                %minigame.TankRound(%client);
        case 5: if(%minigame.isDirectorEnabled)
                %minigame.PanicRound(%client);
    }

}

function MiniGameSO::directorSpecial(%minigame,%client)
{
    if(!%minigame.isDirectorEnabled)
    return;
    
    %directorinterval = $Pref::L4BDirector::Director_Interval*1000;
    %directorintervalhalf = %directorinterval/2;

    cancel(directorSpecialSchedule);
    %minigame.directorSpecialSchedule = %minigame.schedule(%directorintervalhalf,directorSpecial,%client);
    %minigame.DirectorProcessEvent("onDirectorSpecial",%client);
}

function MiniGameSO::directorChoose(%minigame,%client)
{
    if(!%minigame.isDirectorEnabled)
    return;

    %directorinterval = $Pref::L4BDirector::Director_Interval*1000;
    %directorintervalhalf = %directorinterval/2;
    %minigame.DirectorProcessEvent("onDirectorInterval",%client);

    cancel(%minigame.directorSchedule);
    %minigame.directorSchedule = %minigame.schedule($Pref::L4BDirector::Director_Interval*1000,directorChoose,%client);

	%tankhealthscale = %minigame.numMembers*1000;
	$hTankHealth = 2000+%tankhealthscale;
	eval("ZombieTankHoleBot.maxDamage =" @ $hTankHealth @ ";");

	for(%i=0;%i<%minigame.numMembers;%i++)
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

	%round = 0;
	if(%minigame.survivorStatHealthAverage < %minigame.survivorStatHealthMax/2)
    %chance = 40;
	else %chance = 80;

	if(getRandom(1,100) <= %chance)
    %round = getrandom(1,3);

    switch(%round)
    {
        case 0: if($Pref::L4BDirector::AllowMGMessages)
                MiniGameSO::ChatMsgAll (%minigame, "\c2Director <bitmapk:add-ons/package_left4block/icons/ci_peace> \c2Break", %client);
                
                if(getRandom(1,100) <= 25)
                {
                    if($Pref::L4BDirector::EnableCues)
                    {
                        %minigame.directorPlaySound("zombiechoir_0" @ getrandom(1,6) @ "_sound",%client);

                        cancel(%minigame.choirSFX);
                        %minigame.choirSFX = %minigame.schedule(%directorintervalhalf,directorPlaySound,"zombiechoir_0" @ getrandom(1,6) @ "_sound",%client);

                        %minigame.DirectorMusic("musicdata_L4D_background" @ getRandom(1,3),%client);
                        l4b_music.schedule(%directorinterval,delete);
                    }
                }

        case 1: %minigame.SmallHorde(%client);
        case 2: %minigame.LargeHorde(%client);
        case 3: %minigame.TankRound(%client);
    }
}

function MiniGameSO::SmallHorde(%minigame,%client)
{
    if(!%minigame.isDirectorEnabled)
    return;

    %directorinterval = $Pref::L4BDirector::Director_Interval*1000;

    if($Pref::L4BDirector::AllowMGMessages)
    MiniGameSO::ChatMsgAll(%minigame,"\c3Director <bitmapk:add-ons/package_left4block/icons/ci_skull2> \c3Small Horde",%client);

    %minigame.DirectorProcessEvent("onDirectorLvlShStart",%client);
    %minigame.schedule(%directorinterval,DirectorProcessEvent,"onDirectorLvlShEnd",%client);
}

function MiniGameSO::LargeHorde(%minigame,%client)
{
    if(!%minigame.isDirectorEnabled)
    return;

    %directorinterval = $Pref::L4BDirector::Director_Interval*1000;
    
    if($Pref::L4BDirector::EnableCues)
    {
        %minigame.directorPlaySound("hordeincoming" @ getrandom(1,9) @ "_sound",%client);

        cancel(%minigame.hordeMusic);
        %minigame.hordeMusic = %minigame.schedule(4000,hordeMusic,%client);

        cancel(%minigame.hordeEndShed);
        %minigame.hordeEndShed = %minigame.schedule(%directorinterval,hordeEnd,%client);
    }

    if($Pref::L4BDirector::AllowMGMessages)
    MiniGameSO::ChatMsgAll (%minigame,"Director <bitmapk:add-ons/package_left4block/icons/ci_skull> Large Horde", %client);

    %minigame.DirectorProcessEvent("onDirectorLvlLhStart",%client);

    cancel(%minigame.directorSchedule);
    %minigame.directorSchedule = %minigame.schedule(%directorinterval+5000,directorChoose,%client);
}

function MiniGameSO::hordeMusic(%minigame,%client)
{
    %directorinterval = $Pref::L4BDirector::Director_Interval*1000;
    %directorintervalhalf = %directorinterval/2;

    %minigame.DirectorMusic("musicData_L4D_drum_suspense" @ getRandom(1,2),%client);
    %minigame.directorPlaySound("drum_suspense_end_sound",%client);

    cancel(%minigame.hordeMusic1);
    %minigame.hordeMusic1 = %minigame.schedule(%directorintervalhalf-9000,directorPlaySound,"hordeslayer_0" @ getrandom(1,3) @ "_sound",%client);

    cancel(%minigame.hordeMusic2);
    %minigame.hordeMusic2 = %minigame.schedule(%directorintervalhalf-4000,directorPlaySound,"hordeslayer_0" @ getrandom(1,3) @ "_sound",%client);
}

function MinigameSO::hordeEnd(%minigame,%client)
{    
    %minigame.directorPlaySound("drum_suspense_end_sound",%client);
    %minigame.DirectorProcessEvent("onDirectorLvlLhEnd",%client);

    if(isObject(l4b_music))
    l4b_music.delete();
}

function MinigameSO::TankRound(%minigame,%client)
{
    if(!%minigame.isDirectorEnabled)
    return;

    if(%minigame.directorTankRound < $Pref::L4BDirector::TankRounds)
    {
        %minigame.directorTankRound++;

        if($Pref::L4BDirector::AllowMGMessages)
        MiniGameSO::ChatMsgAll (%minigame,"Director <bitmapk:add-ons/package_left4block/icons/ci_skull> Tank", %client);

        if($Pref::L4BDirector::EnableCues)
        %minigame.DirectorMusic("musicdata_L4D_tank",%client);

        %minigame.DirectorProcessEvent("onDirectorLvlTankStart",%client);
        %minigame.isDirectorEnabled = 0;
        cancel(%minigame.directorSchedule);
    } 
}

function MinigameSO::TankRoundEnd(%minigame,%client)
{
    if(isObject(l4b_music))
    l4b_music.delete();
    
    %minigame.DirectorProcessEvent("onDirectorLvlTankEnd",%client);

    %minigame.isDirectorEnabled = 1;
    cancel(%minigame.directorSchedule);
    %minigame.directorSchedule = %minigame.schedule($Pref::L4BDirector::Director_Interval*1000,directorChoose,%client);
}

function MinigameSO::PanicRound(%minigame,%client)
{
    if($Pref::L4BDirector::AllowMGMessages)
    MiniGameSO::ChatMsgAll (%minigame,"Director <bitmapk:add-ons/package_left4block/icons/ci_skull> Panic", %client);

    if($Pref::L4BDirector::EnableCues)
    %minigame.DirectorMusic("musicdata_L4D_skin_on_our_teeth",%client);

    %minigame.DirectorProcessEvent("onDirectorLvlPanicStart",%client);
    %minigame.isDirectorEnabled = 0;
    cancel(%minigame.directorSchedule);
}

function MinigameSO::PanicRoundEnd(%minigame,%client)
{
    if(isObject(l4b_music))
    l4b_music.delete();
    
    %minigame.DirectorProcessEvent("onDirectorLvlPanicEnd",%client);
    %minigame.directorPlaySound("drum_suspense_end_sound",%client);

    %minigame.isDirectorEnabled = 1;
    cancel(%minigame.directorSchedule);
    %minigame.directorSchedule = %minigame.schedule($Pref::L4BDirector::Director_Interval*1000,directorChoose,%client);
}

//Reusable functions
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