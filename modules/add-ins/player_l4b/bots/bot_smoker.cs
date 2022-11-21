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
	%obj.hAttackDamage = $Pref::L4B::Zombies::SpecialsDamage;
	%obj.hNoSeeIdleTeleport();

	if(%obj.getstate() !$= "Dead" && !%obj.isstrangling && %obj.lastidle+5000 < getsimtime())
	{
		if(%obj.hState !$= "Following")
		{
			switch(getRandom(0,1))
			{
				case 0: %obj.playaudio(0,"smoker_lurk" @ getrandom(1,4) @ "_sound");
				case 1: %obj.playaudio(0,"smoker_gasp" @ getrandom(1,5) @ "_sound");
			}
		}
		else %obj.playaudio(0,"smoker_recognize" @ getrandom(1,4) @ "_sound");

		%obj.playthread(3,plant);
		%obj.lastidle = getsimtime();
	}
}

function ZombieSmokerHoleBot::onDisabled(%this,%obj)
{
	Parent::onDisabled(%this,%obj);

	if(isObject(%obj.tongue)) %obj.tongue.delete();
	
	%p = new projectile()
	{
		datablock = SmokerSporeProjectile;
		initialPosition = %obj.getposition();
		client = %obj.client;
		sourceObject = %obj;
	};
	%p.schedule(1000,SmokerExplosionCoughEffect);

	%obj.unMountImage(0);
	%obj.playaudio(0,"smoker_death" @ getrandom(1,2) @ "_sound");
}

function ZombieSmokerHoleBot::onRemove(%this,%obj)
{
	if(isObject(%obj.tongue)) %obj.tongue.delete();
	Parent::onRemove(%this,%obj);
}

function ZombieSmokerHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this,%obj,%col);
}

function ZombieSmokerHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	%limb = %obj.rgetDamageLocation(%position);
	if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact)
	if(%limb) %damage = %damage/4;
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function ZombieSmokerHoleBot::onDamage(%this,%obj)
{	
	if(%obj.getstate() !$= "Dead" && %obj.lastdamage+500 < getsimtime())//Check if the chest is the female variant and add a 1 second cooldown
	{
		%obj.schedule(1,playaudio,0,"smoker_pain" @ getrandom(1,4) @ "_sound");
		%obj.playthread(2,"plant");

		%obj.lastdamage = getsimtime();
	}

	Parent::OnDamage(%this,%obj);
}

function ZombieSmokerHoleBot::onBotFollow( %this, %obj, %targ )
{
	if((!isObject(%obj) || %obj.getState() $= "Dead") || (!isObject(%targ) || %targ.getState() $= "Dead")) return;

	if(vectorDist(%obj.getposition(),%targ.getposition()) > 12)
	{
		if(%obj.lastsaw+8000 < getsimtime() && getWord(%obj.getScale(),2) >= 1.05)
		{
			%obj.hClearMovement();
			%obj.stopHoleLoop();			
			
			%obj.playaudio(0,"smoker_warn" @ getrandom(1,3) @ "_sound");
			%obj.setaimobject(%targ);
			%obj.schedule(1450,hShootAim,%targ);

			%obj.ShootTongue = %obj.getDatablock().schedule(1500,ShootTongue,%obj);
			%obj.lastsaw = getsimtime();
		}
	}
	else
	{
		%obj.hRunAwayFromPlayer(%targ);	
		%this.schedule(1000,onBotFollow,%obj,%targ);
	}
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

function ZombieSmokerHoleBot::L4BAppearance(%this,%obj,%client)
{
	Parent::L4BAppearance(%this,%obj,%client);
	%obj.unhidenode("smokerboil");
	
	switch$(%obj.getClassName())
	{
		case "Player": %obj.setNodeColor("smokerboil",%client.zombieskincolor);
		case "AIPlayer": %obj.setNodeColor("smokerboil",%client.headcolor);
	}
}

function ZombieSmokerHoleBot::ShootTongue(%this, %obj)
{
	if(!isObject(%obj) || %obj.getstate() $= "Dead") return;	
	
	if(isObject(%obj.GHRope)) %obj.GHRope.delete();
	%obj.playthread(3,"Plant");
	%obj.playthread(2,"Plant");
	%obj.playaudio(1,"smoker_launch_tongue_sound");

	%muzzle = vectorAdd(%obj.getMuzzlePoint(2),"0 0 0.35");
	%velocity = vectorAdd(vectorscale(%obj.getEyeVector(),100),"0 0 3.75");
	if(isObject(%obj.hFollowing)) %velocity = vectorAdd(%velocity,getWord(%obj.hFollowing.getVelocity(),0)/2 SPC getWord(%obj.hFollowing.getVelocity(),1)/2 SPC getWord(%obj.hFollowing.getVelocity(),2)/2);
	
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
	%obj.setenergylevel(0);
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
	if(!isObject(%obj)) return;
	
	%smoker = %obj.source;
	%end = %obj.end;

	if((isObject(%end) && (%end.getClassName() $= "Player" || %end.getClassName() $= "AIPlayer") && !L4B_SpecialsPinCheck(%smoker,%end)) || !isObject(%smoker) || !isObject(%end))
	{
		%obj.delete();
		return;
	}

	if(%end.getType() & $TypeMasks::ProjectileObjectType) %endpos = %end.getPosition();
	else if(%end.getType() & $TypeMasks::PlayerObjectType)
	{
		%smoker.playthread(1,"activate2");
		%endpos = vectorSub(%end.getmuzzlePoint(2),"0 0 0.2");

		if(%smoker.getclassname() $= "AIPlayer") %smoker.setaimlocation(%endpos);

		if(%end.lastchokecough+getrandom(250,500) < getsimtime() && getWord(%end.getVelocity(), 2) >= 0.5)
		{
			%end.playaudio(0,"norm_cough" @ getrandom(1,3) @ "_sound");
			%end.playthread(2,"plant");
			%end.lastchokecough = getsimtime();
			%end.damage(%smoker.hFakeProjectile, %end.getposition(), 3, $DamageType::SmokerConstrict);

			if(%smoker.lastdamage+getRandom(1000,2500) < getsimtime())
			{
				%smoker.playaudio(0,"smoker_pain" @ getrandom(1,4) @ "_sound");
				%smoker.lastdamage = getsimtime();
				%smoker.playthread(2,"plant");
			}
		}

		%DisSub = vectorSub(%end.getPosition(),%smoker.getposition());
		%DistanceNormal = vectorNormalize(%DisSub);

		if(getWord(%end.getvelocity(),2) != 0) %force = 10;
		else %force = 5;
		%newvelocity = vectorscale(%DistanceNormal,-%force);

		if(vectorDist(%end.gethackposition(),%smoker.gethackposition()) > 5)
		%zinfluence = getWord(%end.getVelocity(), 2) + getWord(%newvelocity, 2)/7.5;
		else %zinfluence = getWord(%end.getVelocity(), 2);

		%end.setVelocity(getWords(%newvelocity, 0, 1) SPC %zinfluence);
	}

	if(%smoker.getclassname() $= "AIPlayer") %smoker.setaimlocation(%endpos);

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

function SmokerTongueShape::onRemove(%this,%obj)
{		
	%smoker = %obj.source;
	%end = %obj.end;

	if(isObject(%smoker))
	{
		%smoker.isStrangling = false;

		serverPlay3D("smoker_tongue_reel_sound",%smoker.getposition());			
		if(%smoker.getClassName() $= "AIPlayer" && %smoker.getState() !$= "Dead")
		{
			%smoker.startHoleLoop();
			%smoker.hRunAwayFromPlayer(%smoker);				
		}
	}		
	
	if((isObject(%end) && (%end.getClassName() $= "Player" || %end.getClassName() $= "AIPlayer")))
	{
		%end.isBeingStrangled = false;
		if(isObject(%end.getMountedImage(2)) && %end.getMountedImage(2).getName() $= "ZombieSmokerConstrictImage") %end.unMountImage(2);
		if(%end.getState() $= "Dead") serverPlay3D("victim_smoked_sound",%end.getHackPosition());

		L4B_SpecialsPinCheck(%smoker,%end);
	}
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
	explodeOnDeath = true;

	uiName = "";
};

function SmokerTongueProjectile::onCollision(%this,%proj,%col,%fade,%pos,%normal)
{
	%obj = %proj.sourceObject;
	if(!isObject(%obj.tongue)) return;
	serverPlay3D("smoker_tongue_hit_sound",%pos);
	
	if(%col.getType() & $Typemasks::PlayerObjectType)
	{
		%col.dismount();
		%obj.tongue.end = %col;
		%obj.SpecialPinAttack(%col);
		return;
	}
	else %obj.tongue.delete();

	Parent::onCollision(%this,%proj,%col,%fade,%pos,%normal);
}

function Projectile::SmokerExplosionCoughEffect(%obj)
{
	if(!isObject(%obj)) return;

	%pos = %obj.getPosition();
	%radius = 5;
	%searchMasks = $TypeMasks::PlayerObjectType;
	InitContainerRadiusSearch(%pos, %radius, %searchMasks);
	while((%target = containerSearchNext()) != 0 )
	{
		if(!%target.hIsInfected && %target.getstate() !$= "Dead")
		{
			%randomtime = getRandom(400,800);
			%target.schedule(%randomtime,playaudio,2,"norm_cough" @ getrandom(1,3) @ "_sound");
			%target.schedule(%randomtime,playthread,3,"plant");
		}
	}
	%obj.schedule(750,SmokerExplosionCoughEffect);
}