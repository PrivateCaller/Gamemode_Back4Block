datablock PlayerData(ZombieFallenHoleBot : CommonZombieHoleBot)
{
	uiName = "";
	maxdamage = 250;//Health
	hName = "Infected" SPC "Fallen" SPC "Survivor";//cannot contain spaces... lies
	
	hIsInfected = 1;
	hZombieL4BType = "Normal";
	hPinCI = "";
	hBigMeleeSound = "";
};

function ZombieFallenHoleBot::onAdd(%this,%obj,%style)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
}

function ZombieFallenHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);
}

function ZombieFallenHoleBot::OnBotFollow(%this,%obj,%targ)
{
	if(%obj.hMelee)
	CommonZombieHoleBot::OnBotFollow(%this,%obj,%targ);
}

function ZombieFallenHoleBot::onBotLoop(%this,%obj)
{
	CommonZombieHoleBot::onBotLoop(%this,%obj);
}


function ZombieFallenHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact)
	%damage = %damage/4;
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function ZombieFallenHoleBot::onDamage(%this,%obj)
{
	%obj.playaudio(1,"KevlarHit" @ getrandom(1,3));
	CommonZombieHoleBot::OnDamage(%this,%obj);
	%obj.hMelee = 0;
}

function ZombieFallenHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this,%obj,%col);
}

function ZombieFallenHoleBot::onDisabled(%this,%obj)
{
	CommonZombieHoleBot::OnDisabled(%this,%obj);
	L4B_ZombieDropLoot(%obj,$L4B_Grenade[getRandom(1,$L4B_GrenadeAmount)],5);
	L4B_ZombieDropLoot(%obj,$L4B_Grenade[getRandom(1,$L4B_GrenadeAmount)],10);
	L4B_ZombieDropLoot(%obj,$L4B_Ammo[getRandom(1,$L4B_AmmoAmount)],15);
	L4B_ZombieDropLoot(%obj,$L4B_Ammo[getRandom(1,$L4B_AmmoAmount)],15);
	L4B_ZombieDropLoot(%obj,$L4B_Ammo[getRandom(1,$L4B_AmmoAmount)],15);
	L4B_ZombieDropLoot(%obj,$L4B_Ammo[getRandom(1,$L4B_AmmoAmount)],15);
	L4B_ZombieDropLoot(%obj,$L4B_PistolT1[getRandom(1,$L4B_PistolT1Amount)],5);
	L4B_ZombieDropLoot(%obj,$L4B_Medical[getRandom(1,$L4B_MedicalAmount)],5);
	L4B_ZombieDropLoot(%obj,$L4B_Medical[getRandom(1,$L4B_MedicalAmount)],10);
	L4B_ZombieDropLoot(%obj,$L4B_Medical[getRandom(1,$L4B_MedicalAmount)],15);
}

function ZombieFallenHoleBot::Appearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{	
	if(getRandom(1, 8) == 1 && isObject($L4B_clientLog) && $L4B_clientLog.getCount() > 0) L4B_pushZombifiedSnapshot(%obj);
	else
	{
		%hatColor = getRandomBotRGBColor();
		%packColor = getRandomBotRGBColor();
		%shirtColor = getRandomBotRGBColor();
		%pantsColor = getRandomBotPantsColor();
		%shoeColor = getRandomBotPantsColor();
		%accentColor = getRandomBotRGBColor();
		%handColor = %skinColor;

		%larmColor = %shirtColor;
		%rarmColor = %shirtColor;
		%rLegColor = %shoeColor;
		%lLegColor = %shoeColor;

		if(getRandom(1,3) == 1)
		{
			if(getRandom(1,2) == 1) %larmColor = %skinColor;
			if(getRandom(1,2) == 1) %rarmColor = %skinColor;
			if(getRandom(1,2) == 1) %rLegColor = %skinColor;
			if(getRandom(1,2) == 1) %lLegColor = %skinColor;
		}

		%obj.accentColor = %accentColor;
		%obj.accent =  "0";
		%obj.hatColor = %hatColor;
		%obj.hat = %hat;
		%obj.headColor = %skinColor;
		%obj.faceName = %face;
		%obj.chest =  %chest;
		%obj.decalName = "Hoodie";
		%obj.chestColor = %shirtColor;
		%obj.pack = 4;
		%obj.packColor =  %packColor;
		%obj.secondPack =  "0";
		%obj.secondPackColor =  %packColor;
		%obj.larm =  "0";
		%obj.larmColor = %larmColor;
		%obj.lhand =  "0";
		%obj.lhandColor = %handColor;
		%obj.rarm =  "0";
		%obj.rarmColor = %rarmColor;
		%obj.rhandColor = %handColor;
		%obj.rhand =  "0";
		%obj.hip =  "0";
		%obj.hipColor = %pantsColor;
		%obj.lleg =  "0";
		%obj.llegColor = %llegColor;
		%obj.rleg =  "0";
		%obj.rlegColor = %rlegColor;
		%obj.vestColor = getRandomBotRGBColor();

		GameConnection::ApplyBodyParts(%obj);
		GameConnection::ApplyBodyColors(%obj);
	}
}

function ZombieFallenHoleBot::hCustomNodeAppearance(%this,%obj)
{
	%obj.unhidenode(BallisticVest);
	%obj.setnodecolor(BallisticVest,%obj.vestColor);
}
