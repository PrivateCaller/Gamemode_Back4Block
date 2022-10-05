luaexec("./zombies.lua");
exec("./bots/bot_boomer.cs");
exec("./bots/bot_charger.cs");
exec("./bots/bot_common.cs");
exec("./bots/bot_hunter.cs");
exec("./bots/bot_jockey.cs");
exec("./bots/bot_smoker.cs");
exec("./bots/bot_spitter.cs");
exec("./bots/bot_tank.cs");
exec("./bots/bot_uncommon_ceda.cs");
exec("./bots/bot_uncommon_clown.cs");
exec("./bots/bot_uncommon_construction.cs");
exec("./bots/bot_uncommon_fallen.cs");
exec("./bots/bot_uncommon_jimmy_gibbs.cs");
exec("./bots/bot_uncommon_mud.cs");
exec("./bots/bot_uncommon_pirate.cs");
exec("./bots/bot_uncommon_police.cs");
exec("./bots/bot_uncommon_soldier.cs");
exec("./bots/bot_uncommon_toxic.cs");
exec("./bots/bot_witch.cs");
exec("./bots/bot_skeleton.cs");

function fxDTSBrick::RandomizeZombieSpecial(%obj) { %obj.hBotType = $hZombieSpecialType[getRandom(1,$hZombieSpecialTypeAmount)]; }

function fxDTSBrick::RandomizeZombieUncommon(%obj) { %obj.hBotType = $hZombieUncommonType[getRandom(1,$hZombieUncommonTypeAmount)]; }

function fxDTSBrick::zfakeKillBrick(%obj)
{		
	%obj.fakeKillBrick("0 0 1", "5");
	%obj.schedule(5100,disappear,-1);

	if(isObject(BreakBrickSet)) BreakBrickSet.add(%obj);
	else
	{
		new SimSet(BreakBrickSet);
		missionCleanup.add(BreakBrickSet);
		BreakBrickSet.add(%obj);
	}
}

function Player::hMeleeAttack(%obj,%col)
{						
	if(vectordist(%obj.getposition(),%col.getposition()) < 2) return AIPlayer::hMeleeAttack(%obj,%col);
}

function Player::StunnedSlowDown(%obj,%slowdowndivider)
{						
	if(!isObject(%obj) || %obj.getstate() $= "Dead") return;

	%datablock = %obj.getDataBlock();

	%obj.setMaxForwardSpeed(%datablock.MaxForwardSpeed/%slowdowndivider);
	%obj.setMaxSideSpeed(%datablock.MaxSideSpeed/%slowdowndivider);
	%obj.setMaxBackwardSpeed(%datablock.maxBackwardSpeed/%slowdowndivider);

	%obj.setMaxCrouchForwardSpeed(%datablock.maxForwardCrouchSpeed/%slowdowndivider);
  	%obj.setMaxCrouchBackwardSpeed(%datablock.maxSideCrouchSpeed/%slowdowndivider);
  	%obj.setMaxCrouchSideSpeed(%datablock.maxSideCrouchSpeed/%slowdowndivider);

 	%obj.setMaxUnderwaterBackwardSpeed(%datablock.MaxUnderwaterBackwardSpeed/%slowdowndivider);
  	%obj.setMaxUnderwaterForwardSpeed(%datablock.MaxUnderwaterForwardSpeed/%slowdowndivider);
  	%obj.setMaxUnderwaterSideSpeed(%datablock.MaxUnderwaterForwardSpeed/%slowdowndivider);

	cancel(%obj.resetSpeedSched);
	%obj.resetSpeedSched = %obj.schedule(2000,StunnedSlowDown,1);
}

function Player::hChangeBotToInfectedAppearance(%obj)
{
	%this = %obj.getdataBlock();
	%obj.resetHoleLoop();
	if(!%this.hNeedsWeapons) %obj.setWeapon(1);

	%obj.hNeutralAttackChance = %this.hNeutralAttackChance;
	%obj.hSearch = %this.hSearch;
	%obj.hSearchRadius = %this.hSearchRadius;
	%obj.hSight = %this.hSight;
	%obj.hSearchFov = %this.hSearchFov;
	%obj.hSuperStacker = %this.hSuperStacker;
	%obj.hAttackDamage = $L4B_NormalDamage;
	%obj.hMelee = %this.hMelee;

	for(%a = 0; %a <= $aCL; %a++)
	{
		%cur = $avatarColorLoop[%a];
		%newskincolor = getWord(%obj.headColor,0)/1.5 SPC getWord(%obj.headColor,1)/1.15 SPC getWord(%obj.headColor,2)/1.5 SPC 1;
		eval("%objC = %obj." @ %cur @ ";");

		if(%objC $= %obj.headColor) eval("%obj." @ %cur @ " = %newskincolor;");

		%obj.faceName = "asciiTerror";
	}

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

function AIPlayer::hNoSeeIdleTeleport(%obj) { luacall(hNoSeeIdleTeleport,%obj); }

function Player::onL4BDatablockAttributes(%obj)
{
	%this = %obj.getdataBlock();
	%obj.setDamageLevel(0);
	%obj.schedule(10,setenergylevel,0);
	
	if(%obj.hZombieL4BType $= "Special") %obj.playaudio(3,strlwr(%obj.name) @ "_spawn" @ getRandom(1,2) @ "_sound");

	%obj.hIsInfected = %this.hIsInfected;
	%obj.hZombieL4BType = %this.hZombieL4BType;
	%obj.hType = "Zombie";
	%obj.isStrangling = 0;
	%obj.hEating = 0;
	%obj.hAttackDamage = %this.hAttackDamage;

	if(strlen(%this.hMeleeCI)) eval("%obj.hDamageType = $DamageType::" @ %this.hMeleeCI @ ";");
	else %obj.hDamageType = $DamageType::HoleMelee;

	if(%obj.getClassName() $= "Player")
	{
		%obj.client.isInInfectedTeam = 1;
		commandToClient(%obj.client, 'SetVignette', true, "0.25 0.15 0 1" );
		if(%obj.getdataBlock().getName() $= "CommonZombieHoleBot")
		schedule(10,0,commandToClient,%obj.client, 'centerPrint', "<just:right><font:impact:30>\c6You are \c0" @ %obj.getdataBlock().hName @ "\c6! <br><font:impact:20>\c6Left click to attack or <br>\c6Plant brick key to change zombie types", 5);
		else schedule(10,0,commandToClient,%obj.client, 'centerPrint', "<just:right><font:impact:30>\c6You are a \c0" @ %obj.getdataBlock().hName @ "\c6! <br><font:impact:20>\c6" @ %obj.getdataBlock().SpecialCPMessage @ "<br>\c6Plant brick key to change zombie types", 5);
	}	
}

function Player::hDefaultL4BAppearance(%obj)
{	
	%this = %obj.getDataBlock();
	%chest = getRandom(0,1);
	
	if(getRandom(1,50) == 1) %obj.shades = 1;
	if(getRandom(1,3) == 1)
	{
		%hat = $hZombieHat[getRandom(1,$hZombieHatAmount)];

		if(getRandom(1,12) == 1) %pack = $hZombiePack[getRandom(1,$hZombiePackAmount)];
		else %pack = 0;
	}
	else 
	{
		%hat = 0;
		%pack = 0;
	}

	%randmultiplier = getRandom(500,3000)*0.001;
	%randskin = $hZombieSkin[getRandom(1,$hZombieSkinAmount)];
	%skincolor = getWord(%randskin,0)*%randmultiplier SPC getWord(%randskin,1)*%randmultiplier SPC getWord(%randskin,2)*%randmultiplier SPC 1;
	%decal = $hZombieDecal[getRandom(1,$hZombieDecalAmount)];
	%face = "asciiTerror";
	
	%this.holeAppearance(%obj,%skinColor,%face,%decal,%hat,%pack,%chest);
}

function L4B_SpazzZombie(%obj) { luacall(L4B_SpazzZombie,%obj); }

function L4B_ZombieDropLoot(%obj,%lootitem,%chance) { luacall(L4B_ZombieDropLoot,%obj,%lootitem,%chance); }

function L4B_ZombieLunge(%obj,%target,%power) { luacall(L4B_ZombieLunge,%obj,%target,%power); }

function Player::bigZombieMelee(%obj)
{	
	%this = %obj.getdataBlock();
	%oscale = getWord(%obj.getScale(),2);
	%mask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	initContainerRadiusSearch(%obj.getEyePoint(),10,%mask);
	while(%hit = containerSearchNext())
	{
		if(%hit == %obj)
		continue;

		%line = vectorNormalize( vectorSub( %obj.getposition(), %hit.getposition()));
		%dot = vectorDot( %obj.getEyeVector(), %line );
		%obscure = containerRayCast(%obj.getEyePoint(),vectorAdd(%hit.getPosition(),"0 0 1.9"),$TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::FxBrickObjectType, %obj);

		if(isObject(%obscure) || ContainerSearchCurrRadiusDist() > 2 || %dot > 0.5)
		continue;

		if(%hit.getType() & $TypeMasks::PlayerObjectType && miniGameCanDamage(%obj,%hit) && checkHoleBotTeams(%obj,%hit))
		{
			if(%hit.getstate() $= "Dead")
			continue;

			%obj.playaudio(3,%this.hBigMeleeSound);
			%obj.playthread(1,"activate2");
			
			if(%this.getName() $= "ZombieTankHoleBot" || vectorDot(%obj.getVelocity(), %obj.getForwardVector()) < 20)
			%hit.applyimpulse(%hit.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),2000),"0 0 500"));
			else
			{
				%normVec = VectorNormalize(vectorAdd(%obj.getForwardVector(),"0" SPC "0" SPC "0.15"));
				%eye = vectorscale(%normVec,30);
				%hit.setvelocity(%eye/2);
			}
			%hit.damage(%obj.hFakeProjectile, %hit.getposition(), $Pref::Server::L4B2Bots::SpecialsDamage*%oScale, %obj.hDamageType);

			%p = new Projectile()
			{
				dataBlock = "BigZombieHitProjectile";
				initialPosition = %hit.getPosition();
				sourceObject = %obj;
				client = %obj.client;
			};
			MissionCleanup.add(%p);
			%p.explode();

			%c = new Projectile()
			{
				dataBlock = "pushBroomProjectile";
				initialPosition = %hit.getPosition();
				sourceObject = %obj;
				client = %obj.client;
			};
			MissionCleanup.add(%c);
			%c.explode();
			%c.setScale("2 2 2");
		}
		if(%hit.getType() & $TypeMasks::VehicleObjectType)
		{
			%obj.playaudio(3,%this.hBigMeleeSound);
			%obj.playthread(2,"activate2");

			%muzzlepoint = vectorSub(%obj.getHackPosition(),"0 0 0.5");
			%muzzlevector = vectorScale(%obj.getEyeVector(),2.5);
			%muzzlepoint = VectorAdd (%muzzlepoint, %muzzlevector);
			%hit.setTransform (%muzzlepoint @ " " @ rotFromTransform(%hit.getTransform()));
			%impulse = VectorNormalize(VectorAdd (%obj.getEyeVector(), "0 0 0.2"));
			%force = %hit.getDataBlock ().mass * 25;
			%scaleRatio = getWord (%obj.getScale (), 2) / getWord (%hit.getScale (), 2);
			%force *= %scaleRatio;
			%impulse = VectorScale (%impulse, %force);
			%hit.schedule(50,applyImpulse,%hit.getPosition(),%impulse);
			%hit.damage(%obj.hFakeProjectile, %hit.getposition(), 50*getWord(%obj.getScale(),0), $DamageType::Tank);

			%c = new Projectile()
			{
				dataBlock = "pushBroomProjectile";
				initialPosition = %hit.getPosition();
				sourceObject = %obj;
				client = %obj.client;
			};
			MissionCleanup.add(%c);
			%c.explode();
			%c.setScale("2 2 2");
		}
	}
}

function Player::SpecialPinAttack(%obj,%col,%force)
{	
	if(!isObject(%col) || !isObject(%obj)) return;

	if(%col.getType() & $TypeMasks::PlayerObjectType && checkHoleBotTeams(%obj,%col) && miniGameCanDamage(%obj,%col))
	{	
		%shape = fileName(%col.getDataBlock().shapeFile);

		if(%obj.getState() !$= "Dead" && %col.getState() !$= "Dead" && !%obj.isStrangling && !%col.isBeingStrangled && %shape $= "newm.dts")
		{
			%col.isBeingStrangled = 1;
			%obj.isStrangling = 1;
			%obj.hEating = %col;
			%col.hEater = %obj;

			if(%obj.getClassName() $= "AIPlayer")
			{
				%obj.schedule(100,hClearMovement);
				%obj.stopHoleLoop();
				%obj.hIgnore = %col;
			}

			switch$(%col.getclassname())
			{
				case "Player":	chatMessageTeam(%col.client,'fakedeathmessage',"<color:FFFF00>" @ %obj.getDatablock().hName SPC %obj.getdataBlock().hPinCI SPC %col.client.name);
								%col.client.minigame.L4B_PlaySound("victim_needshelp_sound");							
								%col.client.camera.setOrbitMode(%col, %col.getTransform(), 0, 5, 0, 1);
								//Billboard_NeedySurvivor(%col, "Strangled");
								%col.client.setControlObject(%col.client.camera);
								ServerCmdUnUseTool (%target.client);

				case "AIPlayer": %col.stopHoleLoop();
			}

			switch$(%obj.getdataBlock().getName()) 
			{
				case "ZombieChargerHoleBot": %obj.mountObject(%col,0);
											 %obj.playthread(1,"root");
											 %obj.hSharkEatDelay = schedule(2000,0,L4B_holeChargerKill,%obj,%col);
											 %forcedam = %force/4;
											 %col.damage(%obj.hFakeProjectile, %col.getposition(),%forcedam, %obj.hDamageType);

											%p = new Projectile()
											{
												dataBlock = "BigZombieHitProjectile";
												initialPosition = %col.getPosition();
												sourceObject = %obj;
												client = %obj.client;
											};
											MissionCleanup.add(%p);
											%p.explode();

				case "ZombieHunterHoleBot": %obj.playthread(0,root);
											%col.playthread(0,death1);

											%phackloc = %col.getHackPosition();
											%obj.schedule(10,setvelocity,"0 0 0");
											%obj.schedule(5,setTransform,%phackloc SPC %phackloc);

											%obj.HunterHurt = schedule(100,0,L4B_holeHunterKill,%obj,%col);

											%forcedam = %force/4;
											%col.damage(%obj.hFakeProjectile, %col.getposition(),%forcedam, %obj.hDamageType);

				case "ZombieJockeyHoleBot":	%col.mountObject(%obj,2);
											%obj.setControlObject(%col);

											%obj.playthread(0,sit);
											%obj.playthread(1,armreadyboth);
											%obj.playaudio(0,"jockey_attack_loop" @ getrandom(1,2) @ "_sound");

											%obj.JockeyHurt = schedule(1000,0,L4B_holeJockeyKill,%obj,%col);

				case "ZombieSmokerHoleBot":  %obj.playaudio(1,"smoker_launch_tongue_sound");
											 %col.playaudio(2,"smoker_tongue_hit_sound");
											 %obj.playthread(2,"plant");
											 %obj.playthread(3,"shiftup");
											 %col.mountImage(ZombieSmokerConstrictImage, 2);
			}
		}
	}
}

function L4B_SpecialsPinCheck(%obj,%col)
{
	if(!miniGameCanDamage(%obj,%col) || !isObject(%obj) || %obj.getstate() $= "Dead" || !isObject(%col) || (isObject(%col) & %col.getState() $= "Dead" || !%col.isBeingStrangled || %col.hIsInfected))
	{
		if(isObject(%obj))
		{
			%obj.isStrangling = 0;	
			%obj.hIgnore = 0;
			%obj.hEating = 0;
			%obj.stopAudio(0);
	
			if(%obj.getdataBlock().getName() $= "ZombieJockeyHoleBot")
			{
				%obj.dismount();
				%obj.setControlObject(%obj);
			}

			if(%obj.getState() !$= "Dead")
			{
				%obj.playThread(1,root);

				if(%obj.getClassName() $= "AIPlayer")
				{
					if(isObject(%col)) %obj.hRunAwayFromPlayer(%col);
					else %obj.hRunAwayFromPlayer(%obj);

					%obj.schedule(4000,resetHoleLoop);
				}
			}
		}

		if(isObject(%col))
		{
			%col.isBeingStrangled = 0;

			if(%col.getstate() !$= "Dead")
			{
				switch$(%col.getClassName())
				{
					case "Player": %col.client.schedule(100,setControlObject,%col);
					case "AIPlayer": %col.setControlObject(%col);
									 %col.resetHoleLoop();
				}
				%col.playthread(0,root);
			}
		}
		return false;
	}
	else return true;
}