function ZombieChargerHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);
	
	%obj.setscale("1.15 1.15 1.15");
}

function ZombieChargerHoleBot::onAdd(%this,%obj)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
}


function ZombieChargerHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact) %damage = %damage/1.5;
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function L4B_holeChargerKill(%obj,%col)
{
	if(L4B_SpecialsPinCheck(%obj,%col))
	{
		%obj.mountImage(HateImage, 3);
		%obj.setenergylevel(0);

		if(%obj.getclassname() $= "AIPlayer")
		{
			%obj.stopHoleLoop();
			%obj.hClearMovement();
		}
		
		%obj.schedule(10,playThread,1,plant);
		%obj.schedule(10,playThread,2,shiftup);
		%obj.schedule(100,playThread,2,shiftdown);
		%obj.schedule(100,playaudio,3,"charger_smash_sound");
		%col.schedule(100,playThread,2,plant);
		%col.schedule(100,damage,%obj.hFakeProjectile, %col.getposition(), $Pref::Server::L4B2Bots::SpecialsDamage/1.5, $DamageType::Charger);
		%obj.schedule(100,spawnExplosion,pushBroomProjectile,"0.5 0.5 0.5");
		%obj.hSharkEatDelay = schedule(1250,0,L4B_holeChargerKill,%obj,%col);
		%obj.playaudio(0,"charger_pummel" @ getrandom(1,4) @ "_sound");
		%obj.schedule(50,setcrouching,1);
		%obj.schedule(125,setcrouching,0);
	}

}

function L4B_Charging(%obj,%targ)
{
	if(isObject(%obj) && %obj.getState() !$= "Dead")
	{
		if(isObject(%obj.light))
		%obj.light.delete();

		%obj.WalkAfterCharge = %obj.schedule(4000,setMaxForwardSpeed,9);
		%obj.WalkAfterCharge = %obj.schedule(4000,playthread,1,"root");
		%obj.playaudio(0,"charger_charge" @ getrandom(1,2) @ "_sound");
		%obj.mountImage(HateImage, 3);
		%obj.setMaxForwardSpeed(50);
		%obj.setenergylevel(0);

		if(%obj.getClassName() $= "AIPlayer")
		{
			%obj.stopHoleLoop();
			%obj.StartAfterCharge = %obj.schedule(4000,startHoleLoop);
			%obj.setmoveY(100);

			if(isObject(%targ) && %targ.getState() !$= "Dead")
			%obj.setAimLocation(%targ.getEyePoint());

			%obj.schedule(100,clearaim);
		}
	}
}

function ZombieChargerHoleBot::onBotLoop(%this,%obj)
{
	%obj.hAttackDamage = $Pref::Server::L4B2Bots::SpecialsDamage;
	%obj.hNoSeeIdleTeleport();
	
	if(!%obj.hFollowing)
	{
		%obj.setMaxForwardSpeed(9);
		%obj.playaudio(0,"charger_lurk" @ getrandom(1,4) @ "_sound");
		%obj.playthread(3,plant);	
		%obj.playthread(1,"root");
		%obj.raisearms = 0;
	}
}

function ZombieChargerHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(%obj.lastsaw+8000 < getsimtime() && vectorDist(%obj.getposition(),%targ.getposition()) > 8)
	{
		%obj.lastsaw = getsimtime();
		%obj.AboutToCharge = schedule(1000,0,L4B_Charging,%obj,%targ);
	
		%obj.playthread(1,"armReadyright");
		%obj.playthread(3,"plant");
		%obj.playaudio(0,"charger_warn" @ getrandom(1,3) @ "_sound");
	}
	if(!isEventPending(%obj.AboutToCharge)) %obj.playaudio(0,"charger_recognize" @ getrandom(1,4) @ "_sound");
}

function ZombieChargerHoleBot::onCollision(%this, %obj, %col, %fade, %pos, %norm)
{
	Parent::oncollision(%this, %obj, %col, %fade, %pos, %norm);
}

function ZombieChargerHoleBot::onImpact(%this, %obj, %col, %vec, %force)
{
	if(%obj.getstate() $= "Dead") return;
	if(%obj.getclassname() $= "AIPlayer") %obj.startHoleLoop();

	if(%force >= 20)
	{
		%forcecalc = %force/20;
		%oScale = 2*getWord(%obj.getScale(),0);
		%obj.spawnExplosion(pushBroomProjectile,%forcecalc SPC %forcecalc SPC %forcecalc);
		%obj.SpecialPinAttack(%col,%force);
		%obj.playaudio(3,"charger_smash_sound");

		%obj.setMaxForwardSpeed(9);
	}
	Parent::onImpact(%this, %obj, %col, %vec, %force);
}

function ZombieChargerHoleBot::onBotMelee(%this,%obj,%col)
{
	%obj.bigZombieMelee(%col);
}	

function ZombieChargerHoleBot::onDamage(%this,%obj,%source,%pos,%damage,%type)
{
	if(%obj.getstate() $= "Dead") return;

	if(%obj.lastdamage+1000 < getsimtime())//Check if the chest is the male variant and add a 1 second cooldown
	{
		%obj.playaudio(0,"charger_pain" @ getrandom(1,4) @ "_sound");
		%obj.lastdamage = getsimtime();
	}
	
	Parent::onDamage(%this,%obj,%source,%pos,%damage,%type);
}

function ZombieChargerHoleBot::onDisabled(%this,%obj)
{
	if(%obj.getstate() !$= "Dead") return;
	
	%obj.playaudio(0,"charger_die" @ getrandom(1,2) @ "_sound");

	if(isObject(%obj.hEating))
	{
		%obj.hEating.isBeingStrangled = 0;
		L4B_SpecialsPinCheck(%obj,%obj.hEating);
	}

	Parent::onDisabled(%this,%obj,%a);
}

function ZombieChargerHoleBot::holeAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{	
	%hatColor = getRandomBotRGBColor();
	%packColor = getRandomBotRGBColor();
	%pack2Color = getRandomBotRGBColor();
	%accentColor = getRandomBotRGBColor();
	%pantsrandmultiplier = getrandom(2,8)*0.25;
	%pantsColorRand = getRandomBotRGBColor();
	%pantsColor = getWord(%pantsColorRand,0)*%pantsrandmultiplier SPC getWord(%pantsColorRand,1)*%pantsrandmultiplier SPC getWord(%pantsColorRand,2)*%pantsrandmultiplier SPC 1;
	%shoeColor = %pantsColor;

	%shirtColor = %skinColor;
	%larmColor = %shirtColor;
	%chargerhandColor = getWord(%skinColor,0)*0.5 SPC getWord(%skinColor,1)*0.5 SPC getWord(%skinColor,2)*0.5 SPC 1;
	%rarmColor = %chargerhandColor;
	%handColor = %skinColor;

	%rLegColor = getRandom(0,1);
	if(%rLegColor)
	%rLegColor = %shoeColor;
	else %rLegColor = %skinColor;
	%lLegColor = getRandom(0,1);
	if(%lLegColor)
	%lLegColor = %shoeColor;
	else %lLegColor = %skinColor;

	%obj.bloody["rhand"] = false;
	%obj.bloody["lhand"] = false;

	%obj.accentColor = %accentColor;
	%obj.accent =  0;
	%obj.hatColor = %hatColor;
	%obj.hat = %hat;
	%obj.headColor = %skinColor;
	%obj.faceName = %face;
	%obj.chest =  0;
	%obj.decalName = "worm_engineer";
	%obj.chestColor = %shirtColor;
	%obj.pack =  %pack;
	%obj.packColor =  %packColor;
	%obj.secondPack =  0;
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

function ZombieChargerHoleBot::L4BAppearance(%this,%client,%obj)
{
	%obj.hideNode("ALL");
	%obj.unHideNode("chest");
	%obj.unHideNode("LchargerArm");
	%obj.unHideNode("RarmCharger");	
	%obj.unHideNode("RarmCharger");	
	%obj.unHideNode(("rarm"));
	%obj.unHideNode("headskin");
	%obj.unHideNode("pants");
	%obj.unHideNode("rshoe");
	%obj.unHideNode("lshoe");
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

	if(%obj.getDatablock().hType $= "Zombie" && %obj.getclassname() $= "Player")
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

	%obj.setFaceName("asciiTerror");
	%obj.setDecalName(%client.decalName);	
	%obj.setNodeColor("LchargerArm",%larmColor);
	%obj.setNodeColor("RarmCharger",%rarmColor);
	%obj.setNodeColor("headskin",%headColor);
	%obj.setNodeColor("chest",%chestColor);
	%obj.setNodeColor("pants",%hipColor);
	%obj.setNodeColor("rarm",%rarmColor);
	%obj.setNodeColor("rarmSlim",%rarmColor);
	%obj.setNodeColor("larmSlim",%larmColor);
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
	%obj.setNodeColor("headskullpart1","1 0.5 0.5 1");
	%obj.setNodeColor("headskullpart2","1 0.5 0.5 1");
	%obj.setNodeColor("headskullpart3","1 0.5 0.5 1");
	%obj.setNodeColor("headskullpart4","1 0.5 0.5 1");
	%obj.setNodeColor("headskullpart5","1 0.5 0.5 1");
	%obj.setNodeColor("headskullpart6","1 0.5 0.5 1");
	//%obj.setNodeColor("headstump","1 0 0 1");
	%obj.setNodeColor("legstumpr","1 0 0 1");
	%obj.setNodeColor("legstumpl","1 0 0 1");
	%obj.setNodeColor("skeletonchest","1 0.5 0.5 1");
	%obj.setNodeColor("skelepants","1 0.5 0.5 1");
	%obj.setNodeColor("organs","1 0.6 0.5 1");
	%obj.setNodeColor("brain","1 0.75 0.746814 1");
}

function ZombieChargerHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{	
	if(%obj.getClassName() $= "Player" && %obj.getstate() !$= "Dead")
	{
		if(%val && !%obj.hEating)
		{
			switch(%triggerNum)
			{
				case 0: if(%obj.getEnergyLevel() > 25 && isObject(%touchedobj = %obj.lastactivated) && checkHoleBotTeams(%obj,%touchedobj)) %obj.hMeleeAttack(%touchedobj);


				case 4: if(%obj.GetEnergyLevel() >= %this.maxenergy && %val)
						{
							if(!isEventPending(%obj.AboutToCharge))
							{
								%obj.playthread(1,"armReadyright");
								%obj.playaudio(0,"charger_warn" @ getrandom(1,3) @ "_sound");
								%obj.setMaxForwardSpeed(9);

								%obj.AboutToCharge = schedule(1500,0,L4B_Charging,%obj,%targ);
							}
						}	
				default:
			}
		}
	}
	Parent::onTrigger (%this, %obj, %triggerNum, %val);
}