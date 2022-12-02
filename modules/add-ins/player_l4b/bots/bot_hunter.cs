function ZombieHunterHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);
}

function ZombieHunterHoleBot::onAdd(%this,%obj,%style)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);	
}

function ZombieHunterHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	%limb = %obj.rgetDamageLocation(%position);
	if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact)
	if(%limb) %damage = %damage/5;
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function ZombieHunterHoleBot::onDamage(%this,%obj,%delta)
{
	Parent::onDamage(%this,%obj,%delta);	

    if(%delta > 5 && %obj.lastdamage+1000 < getsimtime())
	{			
		if(%obj.getstate() !$= "Dead") %obj.playaudio(0,"hunter_pain" @ getrandom(1,3) @ "_sound");
		else %obj.playaudio(0,"hunter_death" @ getrandom(1,3) @ "_sound");

		%obj.playthread(2,"plant");
		%obj.lastdamage = getsimtime();

		if(%obj.raisearms)
		{
			%obj.raisearms = false;	
			%obj.playthread(1,plant);
		}

		if(isObject(%obj.hEating))
		{
			%obj.hEating.isBeingStrangled = 0;
			L4B_SpecialsPinCheck(%obj,%obj.hEating);
		}		
	}
}

function ZombieHunterHoleBot::onPinLoop(%this,%obj,%col)
{
	if(L4B_SpecialsPinCheck(%obj,%col))
	{
		%obj.setenergylevel(0);
		%obj.unmount();

		if(%obj.getClassName() !$= "Player")
		{
			%obj.setmoveobject(%col);
			%obj.setaimobject(%col.gethackposition());
			%obj.hMeleeAttack(%col);			
		}
		
		%this.schedule(250,onPinLoop,%obj,%col);				
		%this.RBloodSimulate(%col, %col.gethackposition(), 1, 25);
		%col.damage(%obj.hFakeProjectile, %col.getposition(), $Pref::L4B::Zombies::SpecialsDamage/2.5, $DamageType::Hunter);
	}
	else if(%col.getState() $= "Dead") %this.rBloodDismember(%col,1,true,%col.gethackposition());
}

function ZombieHunterHoleBot::onBotLoop(%this,%obj)
{	
	switch$(%obj.hState)
	{
		case "Wandering":	%obj.isStrangling = false;
							%obj.hNoSeeIdleTeleport();

							if(getsimtime() >= %obj.lastidle+8000)
							{
								%obj.playaudio(0,"hunter_idle" @ getrandom(1,3) @ "_sound");
								%obj.playthread(3,"plant");
								%obj.lastidle = getSimTime();
							}
		default:
	}
}

function ZombieHunterHoleBot::onBotFollow( %this, %obj, %targ )
{	
	if(!isObject(%targ) || !isObject(%obj) || %obj.isStrangling || %obj.GetEnergyLevel() < %this.maxenergy)
	{
		if(isObject(%targ) && isObject(%obj) && !isEventPending(%obj.hAboutToAttack)) %this.schedule(500,onBotFollow,%obj,%targ);
		return;
	}

	if((%distance = vectordist(%obj.getposition(),%targ.getposition())) < 75)
	{
		if(!%obj.raisearms)
		{	
			%obj.playthread(1,"armReadyboth");
			%obj.raisearms = true;
		}
		
		%ray = containerRayCast(%obj.gethackposition(),%targ.gethackposition(),$TypeMasks::StaticObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType,%obj);	
		if(isObject(%ray) && %ray.getID() == %targ)
		{
			if(%distance > 15) %time = 1000;
			else %time = 500;

			%obj.hCrouch(%time);
			%obj.schedule(%time-50,hShootAim,%targ);
			%obj.hAboutToAttack = %obj.schedule(%time,hJump);
		}
	}
	else if(%obj.raisearms)
	{	
		%obj.playthread(1,"root");
		%obj.raisearms = false;
	}

	if(isObject(%targ) && isObject(%obj) && !isEventPending(%obj.hAboutToAttack)) %this.schedule(500,onBotFollow,%obj,%targ);
}

function ZombieHunterHoleBot::onBotMelee(%this,%obj,%col)
{
	%meleeimpulse = mClamp(%obj.hLastMeleeDamage, 1, 10);	
	%obj.playthread(2,"zAttack" @ getRandom(1,3));
	%obj.setaimobject(%col);
	
	if(%col.getType() & $TypeMasks::PlayerObjectType)
	{
		if(%col.getClassName() $= "Player") %col.spawnExplosion("ZombieHitProjectile",%meleeimpulse/2 SPC %meleeimpulse/2 SPC %meleeimpulse/2);
		%col.playthread(3,"plant");		
		%obj.playaudio(2,"hunter_hit" @ getrandom(1,3) @ "_sound");
	}
	else
	{ 
		%col.applyimpulse(%col.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),getrandom(100,100*%meleeimpulse)),"0" SPC "0" SPC getrandom(100,100*%meleeimpulse)));	
		%obj.playaudio(1,"melee_hit" @ getrandom(1,8) @ "_sound");
	}
}

function ZombieHunterHoleBot::onImpact(%this, %obj, %col, %vec, %force)
{
	Parent::onImpact(%this, %obj, %col, %vec, %force);

	if(%oScale = getWord(%obj.getScale(),0) >= 0.9) 
	if(!%obj.SpecialPinAttack(%col,%force/2.5))
	{
		%obj.playThread(3,"zstumble" @ getrandom(1,3));
		%this.onDamage(%obj,10);
		%obj.setMoveY(-0.375);
		%obj.setMoveX(0);
		%obj.setAimObject(%col);
	}
}

function ZombieHunterHoleBot::holeAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{	
	%clothesrandmultiplier = getrandom(2,8)*0.25;
	%shirtColor = 0.075 SPC 0.125*%clothesrandmultiplier SPC 0.1875*%clothesrandmultiplier SPC 1;
	%pantsColor = 0.15 SPC 0.125*%clothesrandmultiplier SPC 0.05*%clothesrandmultiplier SPC 1;
	%hatColor = %shirtColor;
	%shoeColor = getRandomBotPantsColor();
	%packColor = getRandomBotRGBColor();
	%pack2Color = getRandomBotRGBColor();
	%accentColor = getRandomBotRGBColor();		
	%decal = "Hoodie";
	%hat = 1;
	%pack = 0;
	%pack2 = 0;
	%accent = 0;
	%chest = 0; 
	%handColor = %skinColor;
	%larmColor = %shirtColor;
	%rarmColor = %shirtColor;
	%rLegColor = %shoeColor;
	%lLegColor = %shoeColor;

	if(getRandom(1,4) == 1)
	{
		if(getRandom(1,0)) %larmColor = %skinColor;
		if(getRandom(1,0)) %rarmColor = %skinColor;
		if(getRandom(1,0)) %rLegColor = %skinColor;
		if(getRandom(1,0)) %lLegColor = %skinColor;
	}

	%obj.accentColor = %accentColor;
	%obj.accent =  %accent;
	%obj.hatColor = %hatColor;
	%obj.hat = 0;
	%obj.headColor = %skinColor;
	%obj.faceName = %face;
	%obj.chest =  %chest;
	%obj.decalName = %decal;
	%obj.chestColor = %shirtColor;
	%obj.pack =  %pack;
	%obj.packColor =  %packColor;
	%obj.secondPack =  %pack2;
	%obj.secondPackColor =  %packColor;
	%obj.larm =  "0";
	%obj.larmColor = %larmColor;
	%obj.lhand =  0;
	%obj.lhandColor = %handColor;
	%obj.rarm =  "0";
	%obj.rarmColor = %rarmColor;
	%obj.rhandColor = %handColor;
	%obj.rhand = 0;
	%obj.hip =  "0";
	%obj.hipColor = %pantsColor;
	%obj.lleg =  0;
	%obj.llegColor = %lLegColor;
	%obj.rleg =  0;
	%obj.rlegColor = %rLegColor;
	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function ZombieHunterHoleBot::L4BAppearance(%this,%obj,%client)
{
	Parent::L4BAppearance(%this,%obj,%client);
	%obj.unhideNode("hoodie");
	%obj.setNodeColor("hoodie",%client.hatColor);
}

function ZombieHunterHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{	
	CommonZombieHoleBot::onTrigger (%this, %obj, %triggerNum, %val);

	if(%obj.getstate() !$= "Dead" && %obj.GetEnergyLevel() >= %this.maxenergy)
	{
		if(%val) switch(%triggerNum)
		{
			case 3: %obj.playaudio(0,"hunter_recognize" @ getrandom(1,3) @ "_sound");
					%obj.BeginPounce = true;
									
			case 2: if(getWord(%obj.getvelocity(),2) <= 5 && %obj.BeginPounce)
					{
						%obj.BeginPounce = false;
						%obj.setenergylevel(0);
						%obj.playaudio(0,"hunter_attack" @ getrandom(1,3) @ "_sound");
						%obj.playaudio(1,"hunter_lunge_sound");
						%obj.playthread(0,"jump");
						%obj.playthread(1,"activate2");											
						%normVec = VectorNormalize(%obj.getEyeVector());
						%eye = VectorAdd(vectorscale(%normVec,100),"0 0 1.25");
						%obj.setvelocity(%eye);
					}
		}
		else %obj.BeginPounce = false;
	}
	Parent::onTrigger (%this, %obj, %triggerNum, %val);
}