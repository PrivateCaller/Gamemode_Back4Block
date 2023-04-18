function ZombieTankHoleBot::onImpact(%this, %obj, %col, %vec, %force)
{
	%oScale = 2*getWord(%obj.getScale(),0);
	%obj.spawnExplosion(pushBroomProjectile,%oScale SPC %oScale SPC %oScale);

	if(%force >= 25)
	{
		if(%obj.getClassName() $= "AIPlayer")
		%obj.setcrouching(1);
	}

	Parent::onImpact(%this, %obj, %col, %vec, %force);
}

function ZombieTankHoleBot::onBotFollow( %this, %obj, %targ )
{
	if(!isObject(%obj) || %obj.getState() $= "Dead" || !isObject(%obj.hFollowing) || %obj.hFollowing.getState() $= "Dead") return;

	cancel(%obj.hLastFollowSched);
	%obj.hLastFollowSched = %this.schedule(750,onBotFollow,%obj);

	if((%distance = vectordist(%obj.getposition(),%obj.hFollowing.getposition())) < 5)
	{		
		%this.onTrigger(%obj,0,true);
		%obj.setMoveX(0);
		%obj.setMoveY(1);
		%obj.setmoveobject(%obj.hFollowing);
	}

	if((getRandom(1,100) <= $Pref::L4B::Zombies::TankChance && getWord(%obj.getvelocity(),2) == 0 && vectorDist(%obj.getPosition(),%targ.getPosition()) >= 35) || %obj.tankstress >= 10)
	{
		%obj.setaimobject(%targ);
		%obj.mountImage(BoulderImage,0);
		%obj.schedule(2500,setImageTrigger,0,1);
		%obj.tankstress = 0;
	}

	if(!%obj.startMusic)
	{
		if(isObject(%minigame = getMiniGameFromObject(%obj)) && !%minigame.finalround) %minigame.l4bMusic("musicData_l4d_tank",true,"Music");
		%obj.startMusic = 1;
	}
}

function ZombieTankHoleBot::onNewDataBlock(%this,%obj)
{	
	Parent::onNewDataBlock(%this,%obj);
	CommonZombieHoleBot::onNewDataBlock(%this,%obj);
	%obj.hDefaultL4BAppearance(%obj);

	if(getRandom(1,8) == 1)
	{
		%scale = getRandom(14,15)*0.1;
		%obj.setscale(%scale SPC %scale SPC %scale);
	}
	else %obj.setscale("1.25 1.25 1.25");
}


function ZombieTankHoleBot::onAdd(%this,%obj)
{	
	Parent::onAdd(%this,%obj);
	CommonZombieHoleBot::onAdd(%this,%obj);
}

function ZombieTankHoleBot::onBotLoop(%this,%obj)
{
	%obj.hNoSeeIdleTeleport();

	if(!%obj.isBurning)
	{
		if(!%obj.hFollowing)
		%obj.playaudio(0,"tank_idle" @ getrandom(1,7) @ "_sound");
		else %obj.playaudio(0,"tank_yell" @ getrandom(1,6) @ "_sound");
	}
}	

function ZombieTankHoleBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	if(%damageType !$= $DamageType::FallDamage || %damageType !$= $DamageType::Impact) %damage = %damage/1.5;
	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function ZombieTankHoleBot::onDamage(%this,%obj,%Am,%Type )
{
    Parent::onDamage(%this,%obj,%Am,%Type);

	if(%obj.getstate() !$= "Dead" && %obj.lastdamage+750 < getsimtime())//Check if the chest is the female variant and add a 1 second cooldown
	{
		%obj.playaudio(0,"tank_pain" @ getrandom(1,5) @ "_sound");
		%obj.lastdamage = getsimtime();
		%obj.tankstress++;
	}
}

function ZombieTankHoleBot::onDisabled(%this,%obj)
{
	%obj.playaudio(0,"tank_death" @ getrandom(1,4) @ "_sound");
	Parent::OnDisabled(%this,%obj);
	
	if(isObject(%minigame = getMiniGameFromObject(%obj)) && !%minigame.finalround) %minigame.RoundEnd();

	if(isObject(%rock = %obj.getMountedImage(0)) && %rock.getName() $= "BoulderImage")
	{
		%obj.unMountImage(0);
		%rnd = getRandom();
		%dist = getRandom()*15;
		%x = mCos(%rnd*$PI*3)*%dist;
		%y = mSin(%rnd*$PI*3)*%dist;
		%p = new projectile()
		{
			datablock = BoulderProjectile;
			initialPosition = %obj.getHackPosition();
			initialVelocity = %x SPC %y SPC (getRandom()*4);
			client = %obj.sourceObject.client;
			sourceObject = %obj.sourceObject;
			damageType = $DamageType::BoulderDirect;
			scale = "4 4 4";
		};
		MissionCleanup.add(%p);
	}
}

function ZombieTankHoleBot::onBotMelee(%this,%obj,%col)
{

}

function ZombieTankHoleBot::onCollision(%this,%obj,%col,%normal,%speed)
{
	Parent::onCollision(%this,%obj,%col,%normal,%speed);

	if(%col.getType() & $TypeMasks::PlayerObjectType) %obj.bigZombieMelee();
}

function ZombieTankHoleBot::onTrigger (%this, %obj, %triggerNum, %val)
{				
	Parent::onTrigger (%this, %obj, %triggerNum, %val);

	if(%obj.getstate() $= "Dead") return;
	
	if(%val) switch(%triggerNum)
	{
		case 0: if(!isObject(%obj.getMountedImage(0)))
				{
					%obj.playthread(2,"activate2");
					%obj.playthread(0,"jump");
					%obj.bigZombieMelee();
				}
		case 4: if(%obj.GetEnergyLevel() >= %this.maxenergy)
				%obj.mountImage(BoulderImage,0);
			
		default:
	}	
}

function ZombieTankHoleBot::RbloodDismember(%this,%obj,%limb,%doeffects,%position)
{
	//ouch
}

function ZombieTankHoleBot::holeAppearance(%this,%obj,%skinColor,%face,%decal,%hat,%pack,%chest)
{
	%obj.hidenode("BallisticHelmet");
	%obj.hidenode("BallisticVest");
	%obj.unhidenode("gloweyes");
	%obj.setnodeColor("gloweyes","1 1 0 1");

	%pantsrandmultiplier = getrandom(2,8)*0.25;
	%pantsColor = 0 SPC 0.141*%pantsrandmultiplier SPC 0.333*%pantsrandmultiplier SPC 1;

	%LegColorR = getRandom(0,1);
	if(%LegColorR)
	%LegColorR = %pantsColor;
	else %LegColorR = %skinColor;

	%LegColorL = getRandom(0,1);
	if(%LegColorL)
	%LegColorL = %pantsColor;
	else %LegColorL = %skinColor;
	
	%obj.headColor = %skinColor;
	%obj.headColor = %skinColor;
	%obj.hipColor = %pantsColor;
	%obj.rlegColor = %LegColorR;
	%obj.llegColor = %LegColorL;
	%obj.chestColor = %skincolor;
	%obj.rarmColor = %skincolor;
	%obj.larmColor = %skincolor;
	%obj.rhandColor = %skincolor;
	%obj.lhandColor = %skincolor;

	//Head
	%obj.setnodecolor("HeadSkin",%obj.headColor);

	//Lower Body
	%obj.setnodecolor("Pants",%obj.hipColor);
	%obj.setnodecolor("ShoeR",%obj.rlegColor);
	%obj.setnodecolor("ShoeL",%obj.llegColor);

	//Upper Body
	%obj.setnodecolor("Torso",%obj.chestColor);
	%obj.setnodecolor("armR",%obj.larmColor);
	%obj.setnodecolor("armL",%obj.rarmColor);
	%obj.setnodecolor("handR",%obj.rhandColor);
	%obj.setnodecolor("handL",%obj.lhandColor);

	if(isObject(getMiniGamefromObject(%obj)) && getMiniGamefromObject(%obj).SoldierTank)
	{
		%armorcolor = getRandomBotPantsColor();

		%obj.PantsColor = %armorcolor;
		%obj.TorsoColor = %armorcolor;
		%obj.HelmetColor = %armorcolor;
		%obj.VestColor = %armorcolor;

		%LegColorR = getRandom(0,1);
		if(%LegColorR)
		%LegColorR = %armorcolor;
		else %LegColorR = %skincolor;

		%LegColorL = getRandom(0,1);
		if(%LegColorL)
		%LegColorL = %armorcolor;
		else %LegColorL = %skincolor;

		%obj.ShoeRColor = %LegColorR;
		%obj.ShoeLColor = %LegColorL;

		%armRcolor = getRandom(0,1);
		if(%armRcolor)
		%armRcolor = %obj.ArmRColor;
		else %armRcolor = %armorcolor;

		%armLcolor = getRandom(0,1);
		if(%armLcolor)
		%armLcolor = %skincolor;
		else %armLcolor = %armorcolor;

		%obj.ArmRColor = %skincolor;
		%obj.ArmLColor = %armLcolor;

		//Vest
		%obj.setnodecolor("BallisticHelmet",%obj.HelmetColor);
		%obj.setnodecolor("BallisticVest",%obj.VestColor);
		%obj.unhidenode("BallisticHelmet");
		%obj.unhidenode("BallisticVest");

		//Lower Body
		%obj.setnodecolor("Pants",%obj.PantsColor);
		%obj.setnodecolor("ShoeR",%obj.ShoeRColor);
		%obj.setnodecolor("ShoeL",%obj.ShoeLColor);

		//Upper Body
		%obj.setnodecolor("Torso",%obj.TorsoColor);
		%obj.setnodecolor("armR",%obj.ArmRColor);
		%obj.setnodecolor("armL",%obj.ArmLColor);
	}
}