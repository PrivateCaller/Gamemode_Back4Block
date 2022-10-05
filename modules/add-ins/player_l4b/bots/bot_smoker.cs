function ZombieSmokerHoleBot::onAdd(%this,%obj,%style)
{
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
}

function ZombieSmokerHoleBot::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);

	%obj.mountImage(SmokeStatusPlayerImage, 1);
	%obj.setscale("0.95 1 1.25");
}

function ZombieSmokerHoleBot::onBotLoop(%this,%obj)
{
	%obj.hAttackDamage = $Pref::Server::L4B2Bots::SpecialsDamage;
	%obj.hNoSeeIdleTeleport();

	if(!%obj.hFollowing && %obj.lastidle+5000 < getsimtime() && %obj.getstate() !$= "Dead" && !%obj.isstrangling)
	{
		switch(getRandom(0,1))
		{
			case 0: %obj.playaudio(0,"smoker_lurk" @ getrandom(1,4) @ "_sound");
			case 1: %obj.playaudio(0,"smoker_gasp" @ getrandom(1,5) @ "_sound");
		}
		%obj.playthread(3,plant);
		%obj.lastidle = getsimtime();
	}
}

function ZombieSmokerHoleBot::onDisabled(%this,%obj)
{
	parent::onDisabled(%this,%obj);

	if(isObject(%obj.light))
	%obj.light.delete();

	if(isObject(getMiniGameFromObject(%obj)))
	{
		if(isObject(%obj.Prey))
		{
			%targ = %obj.Prey;
			%targ.isBeingStrangled = 0;
		
			%obj.Prey = 0;
		}
	}

	%p = new projectile()
	{
		datablock = SmokerSporeProjectile;
		initialPosition = %obj.getposition();
		client = %obj.client;
		sourceObject = %obj;
	};
	%p.schedule(1000,L4B_SmokerCoughEffect,0);

	%obj.unMountImage(0);
	%obj.playaudio(0,"smoker_death" @ getrandom(1,2) @ "_sound");
}

function ZombieSmokerHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this,%obj,%col);
}

function ZombieSmokerHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact) %damage = %damage/1.5;
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function ZombieSmokerHoleBot::onDamage(%this,%obj)
{
	
	if(%obj.getstate() !$= "Dead" && %obj.lastdamage+500 < getsimtime())//Check if the chest is the female variant and add a 1 second cooldown
	{
		%obj.playaudio(0,"smoker_pain" @ getrandom(1,4) @ "_sound");
		%obj.playthread(2,"plant");

		%obj.lastdamage = getsimtime();
	}

	Parent::OnDamage(%this,%obj);
}

function ZombieSmokerHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(%obj.getState() $= "Dead")
	return;

	%obj.setaimobject(%targ);

	if(!isEventPending(%obj.ShootTongue) && !%obj.isstrangling)
	%obj.playaudio(0,"smoker_recognize" @ getrandom(1,4) @ "_sound");

	if(vectorDist(%obj.getposition(),%targ.getposition()) > 12)
	{
		if(%obj.lastsaw+8000 < getsimtime() && getWord(%obj.getScale(),2) >= 1.05)
		{
			%obj.hClearMovement();
			%obj.stopHoleLoop();			

			%obj.playaudio(0,"smoker_warn" @ getrandom(1,3) @ "_sound");
			%obj.ShootTongue = %obj.getDatablock().schedule(1500,ShootTongue,%obj);
			%obj.setaimobject(%targ);

			%obj.lastsaw = getsimtime();
		}
	}
	else if(%obj.getClassName() $= "AIPlayer")
	%obj.hRunAwayFromPlayer(%targ);
}

function ZombieSmokerHoleBot::holeAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{	
	%pack2 = 0;
	%accent = 0;
	%hatColor = getRandomBotColor();
	%packColor = getRandomBotColor();
	%shirtColor = getRandomBotColor();
	%accentColor = getRandomBotColor();
	%pantsColor = getRandomBotPantsColor();
	%shoeColor = getRandomBotPantsColor();

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

	%obj.llegColor =  %llegColor;
	%obj.secondPackColor =  "0 0.435 0.831 1";
	%obj.lhand =  "0";
	%obj.hip =  "0";
	%obj.faceName =  %face;
	%obj.rarmColor =  %rarmColor;
	%obj.hatColor =  %hatcolor;
	%obj.hipColor =  %pantsColor;
	%obj.chest =  "0";
	%obj.rarm =  "0";
	%obj.packColor =  "1 1 1 1";
	%obj.pack =  %pack;
	%obj.decalName =  "classicshirt";
	%obj.larmColor =  %larmColor;
	%obj.secondPack =  "0";
	%obj.larm =  "0";
	%obj.chestColor =  %shirtColor;
	%obj.accentColor =  "1 1 1 1";
	%obj.rhandColor =  %skinColor;
	%obj.rleg =  "0";
	%obj.rlegColor =  %rlegColor;
	%obj.accent =  "1";
	%obj.headColor =  %skinColor;
	%obj.rhand =  "0";
	%obj.lleg =  "0";
	%obj.lhandColor =  %skinColor;
	%obj.hat =  %hat;

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function ZombieSmokerHoleBot::L4BAppearance(%this,%client,%obj)
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
	%obj.unhidenode("smokerboil");
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
		if(%client.rlegcolor $= %skin) %rlegcolor = %zskin;
		if(%client.llegcolor $= %skin) %llegColor = %zskin;
	}

	%obj.setFaceName("asciiTerror");
	%obj.setDecalName(%client.decalName);
	%obj.setNodeColor("headskin",%headColor);
	%obj.setNodeColor("smokerboil",%headColor);
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

function ZombieSmokerHoleBot::ShootTongue(%this, %obj)
{
	if(!isObject(%obj) || %obj.getstate() $= "Dead")
	return;

	if(isObject(%obj.light))
	%obj.light.delete();
	
	%obj.setenergylevel(0);
	if(isObject(%obj.GHRope))
	%obj.GHRope.delete();

	%obj.playthread(3,"Plant");
	%obj.playthread(2,"Plant");
	%obj.playaudio(1,"smoker_launch_tongue_sound");

	%muzzle = vectorAdd(%obj.getMuzzlePoint(2),"0 0 0.35");
	%velocity = vectorscale(getProjectileVector(%obj.hFollowing, 1000, %p.gravityMod * %p.isBallistic, %muzzle),125);
	%p = new Projectile()
	{
		dataBlock = "SmokerTongueProjectile";
		initialVelocity = %velocity;
		initialPosition = vectorAdd(%obj.getMuzzlePoint(2),"0 0 0.35");
		sourceObject = %obj;
		client = %obj.client;
	};
	MissionCleanup.add(%p);

	%tongue = new StaticShape()
	{
		dataBlock = SmokerTongueShape;
		source = %obj;
		end = %p;
	};	
	%obj.tongue = %tongue;
	%p.tongue = %tongue;
}

function SmokerTongueShape::onAdd(%this,%obj)
{
	Parent::onAdd(%this,%obj);
	%obj.setNodeColor("ALL","1 0 0 1");
	%this.onTongueLoop(%obj);
	MissionCleanup.add(%obj);
}

function ZombieSmokerHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{	
	if(%obj.getClassName() $= "Player" && %obj.getstate() !$= "Dead")
	{
		switch(%triggerNum)
		{
			case 0: CommonZombieHoleBot::onTrigger(%this, %obj, %triggerNum, %val);
			case 4: if(%obj.GetEnergyLevel() >= %this.maxenergy && !%obj.isStrangling)
					{
						if(!isEventPending(%obj.ShootTongue))
						{
							%obj.ShootTongue = %obj.getDatablock().schedule(1500,ShootTongue,%obj);
							%obj.playaudio(0,"smoker_warn" @ getrandom(1,3) @ "_sound");
						}
					}
			default:
		}
	}
	Parent::onTrigger (%this, %obj, %triggerNum, %val);
}

function SmokerTongueShape::onTongueLoop(%this,%obj)
{		
	%smoker = %obj.source;
	%end = %obj.end;
	
	if(!isObject(%obj) || !isObject(%smoker) || %smoker.getState() $= "Dead" || !isObject(%end) || isObject(%end) && (%end.getClassName() $= "Player" || %end.getClassName() $= "AIPlayer") && !L4B_SpecialsPinCheck(%smoker,%end))//In case one of these are false, return the function and delete the shape
	{
		if(isObject(%obj))
		{
			if(isObject(%smoker))//Just in case the smoker is still around
			{
				%smoker.playaudio(3,"smoker_tongue_reel_sound");

				if(%smoker.getstate() !$= "Dead" && %smoker.getclassname() $= "AIPlayer")
				{
					%smoker.startHoleLoop();
					%smoker.hRunAwayFromPlayer(%smoker);
				}
			}

			if(isObject(%end) && (%end.getClassName() $= "Player" || %end.getClassName() $= "AIPlayer"))
			{
				if(isObject(%end.getMountedImage(2)) && %end.getMountedImage(2).getID() == ZombieSmokerConstrictImage.getID())
				%end.unMountImage(2);
				
				if(%end.getState() $= "Dead")
				serverPlay3D("victim_smoked_sound",%end.getHackPosition());
			}

			%obj.delete();
			return;
		}
	}

	if(%end.getType() & $TypeMasks::ProjectileObjectType)
	%endpos = %end.getPosition();
	else if(%end.getType() & $TypeMasks::PlayerObjectType)
	{
		%smoker.playthread(1,"activate2");
		%endpos = vectorSub(%end.getmuzzlePoint(2),"0 0 0.2");

		if(%smoker.getclassname() $= "AIPlayer")
		%smoker.setaimlocation(%endpos);

		if(%end.lastchokecough+getrandom(250,500) < getsimtime() && getWord(%end.getVelocity(), 2) >= 0.5)
		{
			%end.playaudio(0,"norm_cough" @ getrandom(1,3) @ "_sound");
			%end.playthread(2,"plant");
			%end.lastchokecough = getsimtime();
			%end.damage(%smoker.hFakeProjectile, %end.getposition(), 3, $DamageType::SmokerConstrict);

			if(%smoker.lastdamage+getRandom(1000,2500) < getsimtime())//Check if the chest is the female variant and add a 1 second cooldown
			{
				%smoker.playaudio(0,"smoker_pain" @ getrandom(1,4) @ "_sound");
				%smoker.lastdamage = getsimtime();
				%smoker.playthread(2,"plant");
			}
		}

		%DisSub = vectorSub(%end.getPosition(),%smoker.getposition());
		%DistanceNormal = vectorNormalize(%DisSub);

		if(getWord(%end.getvelocity(),2) != 0)
		%force = 10;
		else %force = 5;
		%newvelocity = vectorscale(%DistanceNormal,-%force);

		if(vectorDist(%end.gethackposition(),%smoker.gethackposition()) > 5)
		%zinfluence = getWord(%end.getVelocity(), 2) + getWord(%newvelocity, 2)/7.5;
		else %zinfluence = getWord(%end.getVelocity(), 2);

		%end.setVelocity(getWords(%newvelocity, 0, 1) SPC %zinfluence);
	}

	if(%smoker.getclassname() $= "AIPlayer")
	%smoker.setaimlocation(%endpos);

	//Calculate the position and scale between the smoker and victim
	%head = %smoker.getmuzzlePoint(2);
	%vector = vectorNormalize(vectorSub(%endpos,%head));
	%relative = "0 1 0";
	%xyz = vectorNormalize(vectorCross(%relative,%vector));
	%u = mACos(vectorDot(%relative,%vector)) * -1;
	%obj.setTransform(vectorScale(vectorAdd(vectorAdd(%head,"0 0 0.5"),%endpos),0.5) SPC %xyz SPC %u);
	%obj.setScale(0.2 SPC vectorDist(%head,%endpos) * 2 SPC 0.2);

	%obj.TongueLoop = %this.schedule(33,onTongueLoop,%obj);
}

datablock ProjectileData(SmokerTongueProjectile)
{
	projectileShapeName = "base/data/shapes/empty.dts";
	directDamage        = 0;
	directDamageType    = $DamageType::Direct;
	particleEmitter     = "";
	explosion           = "";

	muzzleVelocity      = 100;
	velInheritFactor    = 0;

	armingDelay         = 0;
	lifetime            = 1000;
	fadeDelay           = 750;
	gravityMod 			= 1;
	isBallistic         = true;
	explodeOnDeath = false;

	uiName = "";
};

function SmokerTongueProjectile::onCollision(%this,%proj,%col,%fade,%pos,%normal)
{
	%obj = %proj.sourceObject;
	serverPlay3D("smoker_tongue_hit_sound",%pos);

	cancel(%obj.SmokerTongueReturn);
	if(%col.getType() & $Typemasks::PlayerObjectType)
	{
		%col.dismount();
		%col.playthread(0,"side");
		%obj.tongue.end = %col;
		%obj.SpecialPinAttack(%col);
	}

	Parent::onCollision(%this,%proj,%col,%fade,%pos,%normal);
}

function Projectile::SmokerExplosionCoughEffect(%obj)
{
	if(!isObject(%obj))
	return;

	%pos = %obj.getPosition();
	%radius = 5;
	%searchMasks = $TypeMasks::PlayerObjectType;
	InitContainerRadiusSearch(%pos, %radius, %searchMasks);
	while((%target = containerSearchNext()) != 0 )
	{
		if(%target.hZombieL4BType && %target.getstate() !$= "Dead")
		{
			%randomtime = getRandom(400,800);
			%target.schedule(%randomtime,playaudio,2,"norm_cough" @ getrandom(1,3) @ "_sound");
			%target.schedule(%randomtime,playthread,3,"plant");
		}
	}
	%obj.schedule(750,L4B_SmokerCoughEffect,%obj);
}