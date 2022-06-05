//datablock fxDTSBrickData (BrickZombieSoldier_HoleSpawnData : BrickCommonZombie_HoleSpawnData)
//{
//	uiName = "Zombie Soldier Hole";
//	iconName = "Add-Ons/Package_Left4Block/icons/icon_soldier";
// 
//	holeBot = "ZombieSoldierHoleBot";
//};


datablock PlayerData(ZombieSoldierHoleBot : CommonZombieHoleBot)
{
	uiName = "";
	maxdamage = 250;//Health
	hName = "Infected" SPC "Soldier";//cannot contain spaces//except it can lmao

	ShapeNameDistance = 15;
	hIsInfected = 1;
	hZombieL4BType = 4;
	hCustomNodeAppearance = 1;
	hPinCI = "";
	hBigMeleeSound = "";

	hShootTimes = 4;
	hMaxShootRange = 60;
};

function ZombieSoldierHoleBot::onAdd(%this,%obj,%style)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);

	if(getRandom(0,100) <= 5)
	{
		switch(getRandom(0,2))
		{
			case 0: %obj.mountImage(batonImage, 0);
			case 1: %obj.mountImage(riotshieldImage, 0);
					%obj.playthread(1,"armReadyBoth");
		}
	}
}

function ZombieSoldierHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);
}

function ZombieSoldierHoleBot::onBotLoop(%this,%obj)
{
	CommonZombieHoleBot::onBotLoop(%this,%obj);
}

function ZombieSoldierHoleBot::onBotFollow( %this, %obj, %targ )
{
	CommonZombieHoleBot::onBotFollow(%this,%obj,%targ);
}

function ZombieSoldierHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this,%obj,%col);
}

function ZombieSoldierHoleBot::onDamage(%this,%obj)
{
	%obj.playaudio(2,"kevlarhit" @ getrandom(1,3) @ "_sound");
	CommonZombieHoleBot::OnDamage(%this,%obj);
}

function ZombieSoldierHoleBot::onDisabled(%this,%obj)
{
	CommonZombieHoleBot::OnDisabled(%this,%obj);
}

function ZombieSoldierHoleBot::L4BUncommonAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{	
	%obj.suitColor = getRandomBotPantsColor();
	%uniformColor = %obj.suitColor;
	%larmColor = getRandom(0,1);
	if(%larmColor)
	%larmColor = %uniformColor;
	else %larmColor = %skinColor;
	%rarmColor = getRandom(0,1);
	if(%rarmColor)
	%rarmColor = %uniformColor;
	else %rarmColor = %skinColor;
	%rLegColor = getRandom(0,1);
	if(%rLegColor)
	%rLegColor = %uniformColor;
	else %rLegColor = %skinColor;
	%lLegColor = getRandom(0,1);
	if(%lLegColor)
	%lLegColor = %uniformColor;
	else %lLegColor = %skinColor;
	%handColor = %skinColor;

	%obj.llegColor =  %lLegColor;
	%obj.secondPackColor =  "1 1 1 1";
	%obj.lhand =  "0";
	%obj.hip =  "0";
	%obj.faceName =  %face;
	%obj.rarmColor =  %rArmColor;
	%obj.hatColor =  "1 1 1 1";
	%obj.hipColor =  %uniformColor;
	%obj.chest =  "0";
	%obj.rarm =  "0";
	%obj.packColor =  "1 1 1 1";
	%obj.pack =  "0";
	%obj.decalName =  "AAA-None";
	%obj.larmColor =  %lArmColor;
	%obj.secondPack =  "0";
	%obj.larm =  "0";
	%obj.chestColor =  %uniformColor;
	%obj.accentColor =  "1 1 1 1";
	%obj.rhandColor =  %handColor;
	%obj.rleg =  "0";
	%obj.rlegColor =  %rLegColor;
	%obj.accent =  "1";
	%obj.headColor =  %skinColor;
	%obj.rhand =  "0";
	%obj.lleg =  "0";
	%obj.lhandColor =  %handColor;
	%obj.hat =  "0";

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function ZombieSoldierHoleBot::hCustomNodeAppearance(%this,%obj)
{
	%suitcolor = getWord(%obj.suitColor,0)*0.5 SPC getWord(%obj.suitColor,1)*0.5 SPC getWord(%obj.suitColor,2)*0.5 SPC 1;
	%obj.unhidenode(BallisticHelmet);
	%obj.setnodecolor(BallisticHelmet,%suitcolor);
	%obj.unhidenode(BallisticVest);
	%obj.setnodecolor(BallisticVest,%suitcolor);
}