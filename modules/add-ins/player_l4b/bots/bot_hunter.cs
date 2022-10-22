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

function L4B_holeHunterKill(%obj,%col)
{
	if(L4B_SpecialsPinCheck(%obj,%col))
	{
		%obj.setenergylevel(0);
		if(%obj.getClassName() !$= "Player")
		{
			%obj.setmoveobject(%col);
			%obj.setaimobject(%col.gethackposition());
			%obj.hMeleeAttack(%col);
		}
		
		%obj.HunterHurt = schedule(500,0,L4B_holeHunterKill,%obj,%col);
		%obj.unmount();
		%col.damage(%obj.hFakeProjectile, %col.getposition(), $Pref::L4B::Zombies::SpecialsDamage/50, $DamageType::Hunter);
	}
}

function ZombieHunterHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	%limb = %obj.rgetDamageLocation(%position);
	if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact)
	if(%limb) %damage = %damage/5;
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function ZombieHunterHoleBot::onDamage(%this,%obj)
{
	if(%obj.getstate() $= "Dead")
	return;

    if(%obj.lastdamage+1000 < getsimtime())//Check if the chest is the female variant and add a 1 second cooldown
	{
		%obj.playaudio(0,"hunter_pain" @ getrandom(1,3) @ "_sound");
		%obj.playthread(2,"plant");
		%obj.lastdamage = getsimtime();
	}

	if(%obj.raisearms)
	{
		%obj.raisearms = 0;	
		%obj.playthread(1,plant);
	}

	if(isObject(%obj.hEating))
	{
		%obj.hEating.isBeingStrangled = 0;
		L4B_SpecialsPinCheck(%obj,%obj.hEating);
		%obj.lastpounce = getsimtime()+5000;
		cancel(%obj.hAbouttoattack);
	}

	Parent::onDamage(%this,%obj);
}

function ZombieHunterHoleBot::onDisabled(%this,%obj)
{
	if(%obj.getstate() !$= "Dead") return;

	%obj.playaudio(0,"hunter_death" @ getrandom(1,3) @ "_sound");

	if(isObject(%obj.hEating))
	{
		%obj.hEating.isBeingStrangled = 0;
		L4B_SpecialsPinCheck(%obj,%obj.hEating);
		cancel(%obj.hAbouttoattack);
	}

	Parent::onDisabled(%this,%obj);
}

function ZombieHunterHoleBot::onBotLoop(%this,%obj)
{
	%obj.hNoSeeIdleTeleport();
	
	if(!%obj.hFollowing)
	{
		%obj.setMaxForwardSpeed(9);
		%obj.raisearms = 0;
		%obj.isstrangling = 0;
		%obj.playthread(1,"root");
		%obj.playthread(0,root);

		if(getsimtime() >= %obj.lastidle+8000 && !%obj.isstrangling)
		{
			%obj.playaudio(0,"hunter_idle" @ getrandom(1,3) @ "_sound");
			%obj.playthread(3,"plant");
			%obj.lastidle = getSimTime();
		}
	}
}

function ZombieHunterHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(!%obj.raisearms)
	{	
		%obj.playthread(1,"armReadyboth");
		%obj.raisearms = 1;
	}

	if(%obj.lastpounce+5000 < getsimtime() && !%obj.isstrangling)
	{
		%obj.lastpounce = getsimtime();
	
		%obj.hCrouch(1750);
		%obj.playaudio(0,"hunter_recognize" @ getrandom(1,3) @ "_sound");
		%obj.schedule(900,hShootAim,%targ);
		%obj.hAbouttoattack = schedule(1000,0,L4B_HunterZombieLunge,%obj,%targ);
	}
}

function L4B_HunterZombieLunge(%obj,%targ)
{
	if(!isObject(%obj) || !isObject(%targ) || getWord(%obj.getvelocity(),2) >= 1 || %obj.getState() $= "Dead" || %obj.isstrangling) return;

	%obj.playaudio(0,"hunter_attack" @ getrandom(1,3) @ "_sound");
	%obj.playaudio(1,"hunter_lunge_sound");

	%obj.playthread(3,activate2);
	%obj.playthread(0,jump); 

	%dis = mClamp(vectordist(%targ.getposition(),%obj.getposition())*0.65, 30, 200);
	L4B_ZombieLunge(%obj,%targ,%dis);

	cancel(%obj.hAbouttoattack);
	%obj.spawnExplosion(pushBroomProjectile,%dis*0.01 SPC %dis*0.01 SPC %dis*0.01);
}

function ZombieHunterHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this,%obj,%col);
	%obj.playaudio(2,"hunter_hit" @ getrandom(1,3) @ "_sound");
}

	function ZombieHunterHoleBot::onImpact(%this, %obj, %col, %vec, %force)
{
	%oScale = getWord(%obj.getScale(),2);
	%forcescale = %force/25 * %oscale;
	%obj.spawnExplosion(pushBroomProjectile,%forcescale SPC %forcescale SPC %forcescale);
	%obj.setMaxForwardSpeed(9);
	
	if(%oScale >= 0.9 && %obj.getstate() !$= "Dead") %obj.SpecialPinAttack(%col,%force/2.5);

	Parent::onImpact(%this, %obj, %col, %vec, %force);
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

	// accent
	%obj.accentColor = %accentColor;
	%obj.accent =  %accent;
	
	// hat
	%obj.hatColor = %hatColor;
	%obj.hat = 0;
	
	// head
	%obj.headColor = %skinColor;
	%obj.faceName = %face;
	
	// chest
	%obj.chest =  %chest;

	%obj.decalName = %decal;
	%obj.chestColor = %shirtColor;
		
	// packs
	%obj.pack =  %pack;
	%obj.packColor =  %packColor;

	%obj.secondPack =  %pack2;
	%obj.secondPackColor =  %packColor;
		
	// left arm
	%obj.larm =  "0";
	%obj.larmColor = %larmColor;
	
	%obj.lhand =  0;
	%obj.lhandColor = %handColor;
	
	// right arm
	%obj.rarm =  "0";
	%obj.rarmColor = %rarmColor;
	
	%obj.rhandColor = %handColor;
	%obj.rhand = 0;
	
	// hip
	%obj.hip =  "0";
	%obj.hipColor = %pantsColor;
	
	// left leg
	%obj.lleg =  0;
	%obj.llegColor = %lLegColor;
	
	// right leg
	%obj.rleg =  0;
	%obj.rlegColor = %rLegColor;

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function ZombieHunterHoleBot::L4BAppearance(%this,%client,%obj)
{
	%obj.hideNode("ALL");
	%obj.unHideNode("chest");
	%obj.unHideNode("rhand");
	%obj.unHideNode("lhand");
	%obj.unHideNode(("rarm"));
	%obj.unHideNode(("larm"));
	%obj.unHideNode("headskin");
	%obj.unHideNode("pants");
	%obj.unHideNode("rshoe");
	%obj.unHideNode("lshoe");
	%obj.unhidenode("hoodie");
	%obj.unhidenode("gloweyes");
	%obj.setHeadUp(0);

	%headColor = %client.headcolor;
	%chestColor = %client.chestColor;
	%rarmcolor = %client.rarmColor;
	%larmcolor = %client.larmColor;
	%rhandcolor = %client.rhandColor;
	%lhandcolor = %client.lhandColor;
	%hipcolor = %client.hipColor;
	%rlegcolor = %client.rlegColor;
	%llegColor = %client.llegColor;
	%faceName = "asciiTerror";
	%obj.unhidenode("gloweyes");
	%obj.setnodeColor("gloweyes","1 1 0 1");	

	if(%obj.getclassname() $= "Player")
	{
		%skin = %client.headColor;
		%zskin = getWord(%skin,0)/2.75 SPC getWord(%skin,1)/1.5 SPC getWord(%skin,2)/2.75 SPC 1;			

		%headColor = %zskin;
		if(%client.chestColor $= %skin) %chestColor = %zskin;
		if(%client.rArmColor $= %skin) %rarmcolor = %zskin;
		if(%client.lArmColor $= %skin) %larmcolor = %zskin;
		if(%client.rhandColor $= %skin) %rhandcolor = %zskin;
		if(%client.lhandColor $= %skin) %lhandcolor = %zskin;
		if(%client.hipColor $= %skin) %hipcolor = %zskin;
		if(%client.rLegColor $= %skin) %rlegcolor = %zskin;
		if(%client.lLegColor $= %skin) %llegColor = %zskin;
	}			
	
	%obj.setFaceName(%faceName);
	%obj.setDecalName(%client.decalName);
	%obj.setNodeColor("headskin",%headColor);
	%obj.setNodeColor("hoodie",%client.hatColor);
	%obj.setNodeColor("chest",%chestColor);
	%obj.setNodeColor("pants",%hipColor);
	%obj.setNodeColor("rarm",%rarmColor);
	%obj.setNodeColor("larm",%larmColor);
	%obj.setNodeColor("rhand",%rhandColor);
	%obj.setNodeColor("lhand",%lhandColor);
	%obj.setNodeColor("rshoe",%rlegColor);
	%obj.setNodeColor("lshoe",%llegColor);
	%obj.setNodeColor("headpart1",%headColor);
	%obj.setNodeColor("headpart2",%headColor);
	%obj.setNodeColor("headpart3",%headColor);
	%obj.setNodeColor("headpart4",%headColor);
	%obj.setNodeColor("headpart5",%headColor);
	%obj.setNodeColor("headpart6",%headColor);
	%obj.setNodeColor("chestpart1",%chestColor);
	%obj.setNodeColor("chestpart2",%chestColor);
	%obj.setNodeColor("chestpart3",%chestColor);
	%obj.setNodeColor("chestpart4",%chestColor);
	%obj.setNodeColor("chestpart5",%chestColor);
	%obj.setNodeColor("pants",%hipColor);
	%obj.setNodeColor("pantswound",%hipColor);
	%obj.setnodeColor("gloweyes","1 1 0 1");	
	%obj.setNodeColor("rarmSlim","1 0.5 0.5 1");
	%obj.setNodeColor("larmSlim","1 0.5 0.5 1");	
	%obj.setNodeColor("headskullpart1","1 0.5 0.5 1");
	%obj.setNodeColor("headskullpart2","1 0.5 0.5 1");
	%obj.setNodeColor("headskullpart3","1 0.5 0.5 1");
	%obj.setNodeColor("headskullpart4","1 0.5 0.5 1");
	%obj.setNodeColor("headskullpart5","1 0.5 0.5 1");
	%obj.setNodeColor("headskullpart6","1 0.5 0.5 1");
	%obj.setNodeColor("headstump","1 0 0 1");
	%obj.setNodeColor("legstumpr","1 0 0 1");
	%obj.setNodeColor("legstumpl","1 0 0 1");
	%obj.setNodeColor("skeletonchest","1 0.5 0.5 1");
	%obj.setNodeColor("skelepants","1 0.5 0.5 1");
	%obj.setNodeColor("organs","1 0.6 0.5 1");
	%obj.setNodeColor("brain","1 0.75 0.746814 1");
}

function ZombieHunterHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{	
	CommonZombieHoleBot::onTrigger (%this, %obj, %triggerNum, %val);

	if(%obj.getClassName() $= "Player" && %obj.getstate() !$= "Dead")
	if(%obj.GetEnergyLevel() >= %this.maxenergy)
	{
		switch(%triggerNum)
		{
			case 3: if(%val)
					{
						%obj.playaudio(0,"hunter_recognize" @ getrandom(1,3) @ "_sound");
						%obj.BeginPounce = 1;
					}
					else {
							%obj.BeginPounce = 0;
							if(isObject(%obj.light))
							%obj.light.delete();
						 }

			case 2: if(%val && getWord(%obj.getvelocity(),2) <= 5)
					if(%obj.BeginPounce)
					{
						%obj.BeginPounce = 0;
						
						if(isObject(%obj.light))
						%obj.light.delete();

						%obj.setenergylevel(0);

						%obj.playaudio(0,"hunter_attack" @ getrandom(1,3) @ "_sound");
						%obj.playaudio(1,"hunter_lunge_sound");
						%obj.playthread(0,jump);
						%obj.playthread(1,activate2);

						%normVec = VectorNormalize(vectoradd(%obj.getEyeVector(),"0 0 0.005"));
						%eye = vectorscale(%normVec,50);
						%obj.setvelocity(%eye);
					}
			default:
		}
	}
	Parent::onTrigger (%this, %obj, %triggerNum, %val);
}