luaexec("./script_zombies.lua");

function fxDTSBrick::RandomizeZombieSpecial(%obj) { %obj.hBotType = $hZombieSpecialType[getRandom(1,$hZombieSpecialTypeAmount)]; }

function fxDTSBrick::RandomizeZombieUncommon(%obj) { %obj.hBotType = $hZombieUncommonType[getRandom(1,$hZombieUncommonTypeAmount)]; }

function fxDTSBrick::zfakeKillBrick(%obj)
{		
	%obj.fakeKillBrick("0 0 1", "5");
	%obj.schedule(5100,disappear,-1);
	%obj.setName("_breakbrick");

	$InputTarget_["Self"] = %obj;
	%obj.processInputEvent("onzFakeKillBrick");
}

registerInputEvent("fxDTSBrick","onzFakeKillBrick","Self fxDTSBrick");

function Player::hChangeBotToInfectedAppearance(%obj)
{
	%this = %obj.getdataBlock();
	%obj.resetHoleLoop();

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

	%obj.hIsInfected = %this.hIsInfected;
	%obj.hZombieL4BType = %this.hZombieL4BType;
	%obj.hType = "Zombie";
	%obj.isStrangling = false;
	%obj.hEating = 0;
	%obj.hAttackDamage = %this.hAttackDamage;

	if(strlen(%this.hMeleeCI)) eval("%obj.hDamageType = $DamageType::" @ %this.hMeleeCI @ ";");
	else %obj.hDamageType = $DamageType::HoleMelee;

	%obj.setDamageLevel(0);
	%obj.schedule(10,setenergylevel,0);	
	if(%obj.hZombieL4BType $= "Special") %obj.playaudio(3,strlwr(%obj.name) @ "_spawn" @ getRandom(1,2) @ "_sound");	

	if(%obj.getClassName() $= "Player")
	{
		%obj.client.isInInfectedTeam = true;
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
		%hat = getRandom(1,8);
		if(getRandom(1,12) == 1) %pack = 1;
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
	if(!isObject(%obj) || %obj.getDataBlock().hZombieL4BType !$= "Special") return;
	%this = %obj.getdataBlock();
	%oscale = getWord(%obj.getScale(),2);
	%mask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	initContainerRadiusSearch(%obj.getEyePoint(),10,%mask);
	while(%hit = containerSearchNext())
	{		
		%obscure = containerRayCast(%obj.getEyePoint(),%hit.getWorldBoxCenter(),$TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::FxBrickObjectType, %obj);
		%dot = vectorDot(%obj.getEyeVector(),vectorNormalize(vectorSub(%hit.getposition(),%obj.getposition())));
		
		if(%hit == %obj || isObject(%obscure) || ContainerSearchCurrRadiusDist() > 6 || %dot < 0.5) continue;

		if(%hit.getType() & $TypeMasks::PlayerObjectType)
		{			
			if(!miniGameCanDamage(%obj,%hit) || %hit.getstate() $= "Dead" || %hit.getdataBlock().getName() $= "ZombieTankHoleBot") continue;

			%obj.playaudio(3,%this.hBigMeleeSound);
			%obj.playthread(1,"activate2");			
			%hit.setvelocity(vectorscale(VectorNormalize(vectorAdd(%obj.getForwardVector(),"0" SPC "0" SPC "0.15")),30)/2);
			%hit.damage(%obj.hFakeProjectile, %hit.getposition(), $Pref::L4B::Zombies::SpecialsDamage*25 * %oScale, %obj.hDamageType);

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

function AIPlayer::bigZombieMelee(%obj) { Player::bigZombieMelee(%obj); }

function Player::SpecialPinAttack(%obj,%col,%force)
{	
	if(!isObject(%col) || !isObject(%obj)) return false;

	if(%col.getType() & $TypeMasks::PlayerObjectType && checkHoleBotTeams(%obj,%col) && miniGameCanDamage(%obj,%col))
	{	
		if(%obj.getState() !$= "Dead" && %col.getState() !$= "Dead" && !%obj.isStrangling && !%col.isBeingStrangled && %col.getdataBlock().isSurvivor)
		{			
			%col.isBeingStrangled = true;
			%obj.isStrangling = true;
			%obj.hEating = %col;
			%col.hEater = %obj;
			%obj.getDataBlock().schedule(100,onPinLoop,%obj,%col);
			%pinmusic = "musicData_" @ %obj.getDataBlock().hName @ "_pin";

			if(%obj.getClassName() $= "AIPlayer")
			{
				if(%obj.getdataBlock().getName() !$= "ZombieChargerHoleBot")
				{
					%obj.schedule(100,hClearMovement);
					%obj.stopHoleLoop();
				}				
				%obj.hIgnore = %col;
			}

			if(%obj.getdataBlock().getName() $= "ZombieHunterHoleBot" && vectorDot(%obj.getForwardVector(),%col.getForwardVector()) > 0) return false;

			switch$(%obj.getdataBlock().getName()) 
			{
				case "ZombieChargerHoleBot": %obj.mountObject(%col,8);
											%obj.playthread(0,"chargedidle");
											%forcedam = %force/2;
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
											%pinmusic = "";

				case "ZombieHunterHoleBot": %obj.playthread(0,root);
											%col.playthread(0,death1);
											%obj.schedule(25,setvelocity,"0 0 0");
											%obj.schedule(50,setTransform,getWords(%col.getTransform(),0,2) SPC getWords(%obj.getTransform(),3,6));											
											//%obj.schedule(100,setaimlocation,vectorAdd(vectorAdd(%col.getMuzzlePoint(2),vectorScale(%col.getForwardVector(),-0.5))),"0 0 -2.5");
											%obj.setMoveX(0);
											%obj.setMoveY(0);											

											%forcedam = %force/4;
											%col.damage(%obj.hFakeProjectile, %col.getposition(),%forcedam, %obj.hDamageType);

				case "ZombieJockeyHoleBot":	%col.mountObject(%obj,2);
											%obj.setControlObject(%col);

											%obj.playthread(0,sit);
											%obj.playthread(1,armreadyboth);
											%obj.playaudio(0,"jockey_attack_loop" @ getrandom(1,2) @ "_sound");

				case "ZombieSmokerHoleBot":  %obj.playaudio(1,"smoker_launch_tongue_sound");
											 %col.playaudio(2,"smoker_tongue_hit_sound");
											 %obj.playthread(2,"plant");
											 %obj.playthread(3,"shiftup");
											 %col.mountImage("ZombieSmokerConstrictImage",3);
											 %pinmusic = "musicData_smoker_tonguepin";
			}

			if(isObject(%col.billboardbot.lightToMount)) %col.billboardbot.lightToMount.setdatablock("strangledBillboard");

			switch$(%col.getclassname())
			{
				case "Player":	if(isObject(%minigame = getMiniGameFromObject(%col)))
								%minigame.L4B_ChatMessage("<color:FFFF00>" @ %obj.getDatablock().hName SPC %obj.getdataBlock().hPinCI SPC %col.client.name,"victim_needshelp_sound",true);
										
								%col.client.camera.setOrbitMode(%col, %col.getTransform(), 0, 5, 0, 1);
								%col.client.l4bMusic(%pinmusic, true, "Music");
								%col.client.setControlObject(%col.client.camera);
								ServerCmdUnUseTool (%target.client);

								%pos = %col.getPosition();
								%radius = 15;
								%searchMasks = $TypeMasks::PlayerObjectType;
								InitContainerRadiusSearch(%pos, %radius, %searchMasks);

								while((%targetid = containerSearchNext()) != 0)
								{
									if(%targetid == %col) continue;
									else if(%targetid.getclassname() $= "Player" && %targetid.getdataBlock().isSurvivor) %targetid.client.centerPrint("<color:00e100><font:impact:40>" @ %col.client.name SPC "<color:FFFFFF>is in trouble <br>(and is very close to you!)",2);
								}

				case "AIPlayer": %col.stopHoleLoop();
			}

			return true;	
		}
		else return false;
	}
	else return false;
}

function L4B_SpecialsPinCheck(%obj,%col)
{
	if((isObject(%obj) && isObject(%col) && !miniGameCanDamage(%obj,%col)) || (!isObject(%obj) || %obj.getstate() $= "Dead" || !%obj.isStrangling) || (!isObject(%col) || !%col.isBeingStrangled || %col.hIsInfected || %col.getState() $= "Dead") || 
	(%obj.getdatablock().getName() $= "ZombieJockeyHoleBot" && %col.getdatablock().isDowned))
	{
		if(isObject(%obj))
		{
			%obj.isStrangling = false;	
			%obj.hIgnore = 0;
			%obj.hEating = 0;
			%obj.hitMusic = false;
			%obj.stopAudio(0);
			%obj.playthread(0,"root");

			if(%obj.getState() !$= "Dead")
			{
				switch$(%obj.getdataBlock().getName())
				{					
					case "ZombieJockeyHoleBot": %obj.unmount();
												%obj.setControlObject(%obj);
												%obj.hIgnore = %col;
					default:
				}

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
			%col.isBeingStrangled = false;
			%col.hEater = 0;
			if(isObject(%col.billboardbot.lightToMount) && !%col.getdataBlock().isDowned) %col.billboardbot.lightToMount.setdatablock("blankBillboard");

			if(%col.getstate() !$= "Dead")
			{
				switch$(%col.getClassName())
				{
					case "Player":	%col.client.schedule(100,setControlObject,%col);
									%col.client.deletel4bMusic("Music");

					case "AIPlayer": %col.setControlObject(%col);
									 %col.resetHoleLoop();
				}
				
				if(%col.getdataBlock().isDowned) %col.playthread(0,"sit");
				else %col.playthread(0,"root");
			}
		}
		return false;
	}
	else return true;
}