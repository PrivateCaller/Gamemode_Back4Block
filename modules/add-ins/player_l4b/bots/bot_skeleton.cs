function SkeletonHoleBot::onAdd(%this,%obj)
{
	Parent::onAdd(%this,%obj);
}

function SkeletonHoleBot::onNewDatablock(%this,%obj)
{
	if(getRandom(1,32) == 1)
	{
		if(getRandom(0,1))
		{
			%obj.armor = true;
			%obj.armorColor = getRandomBotPantsColor();
		}
		else
		{
			%obj.archer = true;
			%obj.archerColor = getRandomBotColor();
			%obj.mountImage(bowImage,0);
		}
	}

	if($L4B_CurrentMonth == 10 && getRandom(1,16) == 1) 
	{
		%obj.pumpkin = true;
		%obj.pumpkinColor = "1 0.355386 0.111334 1";
	}

	%this.L4BAppearance(%obj);
	Parent::onNewDatablock(%this,%obj);
}

function SkeletonHoleBot::onBotLoop( %this, %obj, %targ )
{
	if(%obj.hState !$= "Following" && %obj.raisearm)
	{
		%obj.playThread(1,"root");
		%obj.raisearm = 0;
	}
}

function SkeletonHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(!%obj.raisearm && isObject(%obj.getMountedImage(0)))
	{
		%obj.playThread(1,"armReadyRight");
		%obj.raisearm = 0;
	}	
	
	%obj.playthread(2,plant);
	%obj.playaudio(0,"skele_idle" @ getrandom(1,4) @ "_sound");
}

function SkeletonHoleBot::onDisabled(%this,%obj)
{
	if(!isEventPending(%obj.SkeleAssembleSchedule)) %this.Disassemble(%obj);
	%obj.unMountImage(0);
	%obj.unMountImage(1);

	Parent::OnDisabled(%this,%obj);
}

function SkeletonHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	if(%obj.armor)
	{
		%limb = %obj.rgetDamageLocation(%position);
		if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact)
		if(%limb == 1 || %limb == 6 || %limb == 0) %damage = %damage/8;
	}
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function SkeletonHoleBot::onDamage( %this, %obj, %am, %source)
{
	if(%obj.getstate() $= "Dead") return;

    if(%obj.lastdamage+1000 < getsimtime())//1 second cooldown
	{
		%obj.lastdamage = getsimtime();
		%obj.playaudio(0,"skele_hurt" @ getrandom(1,3) @ "_sound");
	}

	if(%obj.getDamagePercent() > 0.75) %this.Disassemble(%obj);

	Parent::OnDamage(%this, %obj, %am, %source);
}

function SkeletonHoleBot::onCollision(%this, %obj, %col, %fade, %pos, %norm)
{
	CommonZombieHoleBot::oncollision(%this, %obj, %col, %fade, %pos, %norm);
}

function SkeletonHoleBot::onBotMelee(%this,%obj,%col)
{
	CommonZombieHoleBot::onBotMelee(%this, %obj, %col);
	%obj.playaudio(3,"skele_attack_sound");
}

function SkeletonHoleBot::Disassemble(%this,%obj)
{	
	%obj.unMountImage(0);
	%obj.unMountImage(1);
	%obj.playThread(0,death1);
	%obj.playaudio(2,"brickbreaksound");
	%obj.SkeleAssembleSchedule = %this.schedule(6000,Reassemble,%obj,0);	
	%obj.stopHoleLoop();			
	%obj.Name = "Skeleton";
	%obj.hideNode("ALL");
	%obj.unhidenode(pants);
	%obj.unhidenode(chest);	

	%proj = new Projectile()
	{
		scale = %obj.getScale();
		dataBlock = SkeletalDeathExplosionProjectile;
		initialVelocity = %obj.getVelocity();
		initialPosition = %obj.getPosition();
		sourceObject = %obj;
		sourceSlot = 0;
		client = %obj.client;
	};
	MissionCleanup.add(%proj);
}

function SkeletonHoleBot::Reassemble(%this,%obj,%count)
{
	if(isObject(%obj) && %obj.getState() !$= "Dead")
	{
		if(%count != 8)
		{
			if(%count == 1) %obj.unhideNode("larm");			
			else if(%count == 2) %obj.unhideNode("rarm");
			else if(%count == 3) %obj.unhideNode("rhand");
			else if(%count == 4) %obj.unhideNode("lhand");
			else if(%count == 5) %obj.unhideNode("lshoe");
			else if(%count == 6) %obj.unhideNode("rshoe");
			else if(%count == 7) %obj.unhideNode("headskin");

			%obj.stopaudio(3);
			%obj.playthread(3,plant);
			%obj.playaudio(3,"skele_assemble" @ GetRandom(1,4) @ "_sound");
			%this.schedule(getRandom(125,250),Reassemble,%obj,%count+1);
		}
		else
		{
			%obj.lastdamage = getsimtime();
			%obj.StartHoleLoop();
			%obj.playthread(0,root);
			%obj.playthread(1,root);
			%obj.playthread(2,root);
			%obj.playthread(3,root);
			%obj.SetHealth(100);
			%obj.FakeDeath = 1;
			%obj.SkeleShaded = 0;
			%obj.SkeleArmor = 0;
			%obj.Name = %obj.getDatablock().hName;
		}
	}
}

function SkeletonHoleBot::L4BAppearance(%this,%obj)
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

	if(%obj.boneColor $= "") %obj.boneColor = "1 1 1 1";
	%obj.setNodeColor("headskin",%obj.boneColor);
	%obj.setNodeColor("chest",%obj.boneColor);
	%obj.setNodeColor("pants",%obj.boneColor);
	%obj.setNodeColor("rarm",%obj.boneColor);
	%obj.setNodeColor("larm",%obj.boneColor);
	%obj.setNodeColor("rhand",%obj.boneColor);
	%obj.setNodeColor("lhand",%obj.boneColor);
	%obj.setNodeColor("rshoe",%obj.boneColor);
	%obj.setNodeColor("lshoe",%obj.boneColor);
	%obj.setNodeColor("pants",%obj.boneColor);

	if(%obj.armor)
	{
		%obj.unhideNode("ballisticvest");
		%obj.unhideNode("ballistichelmet");
		%obj.setNodeColor("ballisticvest",%obj.armorColor);
		%obj.setNodeColor("ballistichelmet",%obj.armorColor);
	}
	else if(%obj.archer)
	{
		%obj.unhideNode("quiver");
		%obj.unhideNode("scouthat");
		%obj.setNodeColor("quiver",%obj.archerColor);
		%obj.setNodeColor("scouthat",%obj.archerColor);
	}

	if(%obj.pumpkin)
	{
		%obj.unhideNode("pumpkin_ascii");
		%obj.setNodeColor("pumpkin_ascii",%obj.pumpkinColor);
	}	
}