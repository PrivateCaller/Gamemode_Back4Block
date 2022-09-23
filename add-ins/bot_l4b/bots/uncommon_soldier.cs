datablock PlayerData(ZombieSoldierHoleBot : CommonZombieHoleBot)
{
	uiName = "";
	hName = "Infected" SPC "Soldier";//cannot contain spaces//except it can lmao

	hIsInfected = 1;
	hZombieL4BType = "Normal";
	hPinCI = "";
	hBigMeleeSound = "";
	maxdamage = 250;//Health

	hShootTimes = 4;
	hMaxShootRange = 60;
};

function ZombieSoldierHoleBot::onAdd(%this,%obj,%style)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
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

function ZombieSoldierHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact) %damage = %damage/4;
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function ZombieSoldierHoleBot::onDamage(%this,%obj)
{
	%obj.playaudio(2,"kevlarhit" @ getrandom(1,3) @ "_sound");
	CommonZombieHoleBot::OnDamage(%this,%obj);
}

function ZombieSoldierHoleBot::onDisabled(%this,%obj)
{
	CommonZombieHoleBot::OnDisabled(%this,%obj);

	L4B_ZombieDropLoot(%obj,$L4B_Ammo[getRandom(1,$L4B_AmmoAmount)],25);
	L4B_ZombieDropLoot(%obj,$L4B_Ammo[getRandom(1,$L4B_AmmoAmount)],25);
	L4B_ZombieDropLoot(%obj,$L4B_PistolT1[getRandom(1,$L4B_PistolT2Amount)],25);
	L4B_ZombieDropLoot(%obj,$L4B_Medical[getRandom(1,$L4B_MedicalAmount)],5);
}

function ZombieSoldierHoleBot::Appearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{	
	%obj.suitColor = getRandomBotPantsColor();
	%uniformColor = %obj.suitColor;
	%larmColor = %uniformColor;
	%rarmColor = %uniformColor;
	%rLegColor = %uniformColor;
	%lLegColor = %uniformColor;
	%handColor = %skinColor;
	
	if(getRandom(1,3) == 1)
	{
		if(getRandom(1,2) == 1) %larmColor = %skinColor;
		if(getRandom(1,2) == 1) %rarmColor = %skinColor;
		if(getRandom(1,2) == 1) %rLegColor = %skinColor;
		if(getRandom(1,2) == 1) %lLegColor = %skinColor;
	}

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