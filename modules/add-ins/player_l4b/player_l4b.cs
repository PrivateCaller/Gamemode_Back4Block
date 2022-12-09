exec("./datablocks.cs");
exec("./script_survivor.cs");
exec("./script_zombies.cs");

registerOutputEvent(fxDTSBrick, "RandomizeZombieSpecial");
registerOutputEvent(fxDTSBrick, "RandomizeZombieUncommon");
registerInputEvent ("fxDTSBrick", "onZombieTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerInputEvent ("fxDTSBrick", "onTankTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerInputEvent ("fxDTSBrick", "onDoorClose", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerInputEvent ("fxDTSBrick", "onDoorOpen", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerOutputEvent ("fxDTSBrick", "zfakeKillBrick");

registerOutputEvent ("Player", "Safehouse","bool");
registerInputEvent ("fxDTSBrick", "onSurvivorTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerOutputEvent(Player,RemoveItem,"datablock ItemData",1);

function Player::Safehouse(%player,%bool)
{
	%minigame = getMiniGameFromObject(%player);
	if(%player.hType !$= "Survivors" || isEventPending(%minigame.resetSchedule)) return;

	if(%bool) %player.InSafehouse = 1;
	else %player.InSafehouse = 0;
}

function Player::RemoveItem(%player,%item,%client)  
{
	if(isObject(%player)) for(%i=0;%i<%player.dataBlock.maxTools;%i++)
    {
        %tool = %player.tool[%i];
        if(%tool == %item)
        {
            %player.tool[%i] = 0;
            messageClient(%client,'MsgItemPickup','',%i,0);
            if(%player.currTool == %i)
            {
                %player.updateArm(0);
                %player.unMountImage(0);
            }
        }
    }
}

function Player::checkIfUnderwater(%obj)
{
	if(%obj.getWaterCoverage() == 0)
	{
		if(%obj.oxygenCount == 6 && %obj.getState() !$= "Dead") %obj.playaudio(0,"survivor_painhigh" @ getRandom(1, 4) @ "_sound");
		%obj.oxygenCount = 0;
	}
   	cancel(%obj.oxygenTick);
}

function Player::oxygenTick(%obj)
{   
	if(!isObject(%obj) && %obj.getState() $= "Dead") return;
	
	if(%obj.getWaterCoverage() == 1)
	{
		%obj.oxygenCount = mClamp(%obj.oxygenCount++, 0, 6);	

		if(%obj.oxygenCount == 6) %obj.Damage(%obj, %obj.getPosition (), 25, $DamageType::Suicide);
		
		%obj.lastwatercoverage = getsimtime();
		%bubblepitch = 0.125*%obj.oxygenCount;
		%obj.emote(oxygenBubbleImage, 1);
		%obj.playthread(3,plant);

		if($oldTimescale $= "")
		$oldTimescale = getTimescale();
  		setTimescale(0.25+%bubblepitch);
  		serverPlay3D("drown_bubbles_sound",%obj.getPosition());
  		setTimescale($oldTimescale);
	}
	
	cancel(%obj.oxygenTick);
	%obj.oxygenTick = %obj.schedule(2500, oxygenTick);
}

function Armor::L4BAppearance(%this,%obj,%client)
{
		%obj.hideNode("ALL");
		%obj.unHideNode((%client.chest ? "femChest" : "chest"));
		%obj.unHideNode("rhand");
		%obj.unHideNode("lhand");
		%obj.unHideNode("rarm");
		%obj.unHideNode("larm");
		%obj.unHideNode("headskin");
		%obj.unHideNode("pants");
		%obj.unHideNode("rshoe");
		%obj.unHideNode("lshoe");
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
		%faceName = %client.faceName;

		if(%obj.getDatablock().hType $= "Zombie")
		{
			%obj.unhidenode("gloweyes");
			%obj.setnodeColor("gloweyes","1 1 0 1");

			if(%obj.getclassname() $= "Player")
			{
				%skin = %client.headColor;
				%zskin = getWord(%skin,0)/2.75 SPC getWord(%skin,1)/1.5 SPC getWord(%skin,2)/2.75 SPC 1;
				%client.zombieskincolor = %zskin;

				%headColor = %zskin;
				if(%client.chestColor $= %skin) %chestColor = %zskin;
				if(%client.rArmColor $= %skin) %rarmcolor = %zskin;
				if(%client.lArmColor $= %skin) %larmcolor = %zskin;
				if(%client.rhandColor $= %skin) %rhandcolor = %zskin;
				if(%client.lhandColor $= %skin) %lhandcolor = %zskin;
				if(%client.hipColor $= %skin) %hipcolor = %zskin;
				if(%client.rLegColor $= %skin) %rlegcolor = %zskin;
				if(%client.lLegColor $= %skin) %llegColor = %zskin;
				%faceName = "asciiTerror";
			}
		}		

		%obj.setFaceName(%faceName);
		%obj.setDecalName(%client.decalName);
		%obj.setNodeColor("headskin",%headcolor);
		%obj.setNodeColor((%client.chest ? "femChest" : "chest"),%chestcolor);
		%obj.setNodeColor("pants",%hipcolor);
		%obj.setNodeColor("rarm",%rarmcolor);
		%obj.setNodeColor("larm",%larmcolor);
		%obj.setNodeColor("rhand",%rhandcolor);
		%obj.setNodeColor("lhand",%lhandcolor);
		%obj.setNodeColor("rshoe",%rlegcolor);
		%obj.setNodeColor("lshoe",%llegcolor);
		%obj.setNodeColor("headpart1",%headcolor);
		%obj.setNodeColor("headpart2",%headcolor);
		%obj.setNodeColor("headpart3",%headcolor);
		%obj.setNodeColor("headpart4",%headcolor);
		%obj.setNodeColor("headpart5",%headcolor);
		%obj.setNodeColor("headpart6",%headcolor);
		%obj.setNodeColor("chestpart1",%chestcolor);
		%obj.setNodeColor("chestpart2",%chestcolor);
		%obj.setNodeColor("chestpart3",%chestcolor);
		%obj.setNodeColor("chestpart4",%chestcolor);
		%obj.setNodeColor("chestpart5",%chestcolor);
		%obj.setNodeColor("pants",%hipcolor);
		%obj.setNodeColor("pantswound",%hipcolor);

		%bloodcolor = "1 0.5 0.5 1";
		%obj.setNodeColor("rarmSlim",%bloodcolor);
		%obj.setNodeColor("larmSlim",%bloodcolor);
		%obj.setNodeColor("headskullpart1",%bloodcolor);
		%obj.setNodeColor("headskullpart2",%bloodcolor);
		%obj.setNodeColor("headskullpart3",%bloodcolor);
		%obj.setNodeColor("headskullpart4",%bloodcolor);
		%obj.setNodeColor("headskullpart5",%bloodcolor);
		%obj.setNodeColor("headskullpart6",%bloodcolor);
		%obj.setNodeColor("headstump",%bloodcolor);
		%obj.setNodeColor("legstumpr",%bloodcolor);
		%obj.setNodeColor("legstumpl",%bloodcolor);
		%obj.setNodeColor("skeletonchest",%bloodcolor);
		%obj.setNodeColor("skeletonchestpiece1",%bloodcolor);
		%obj.setNodeColor("skeletonchestpiece2",%bloodcolor);		
		%obj.setNodeColor("skelepants",%bloodcolor);
		%obj.setNodeColor("organs","1 0.6 0.5 1");
		%obj.setNodeColor("brain","1 0.75 0.746814 1");		

		for(%limb = 0; %limb <= 8; %limb++) if(%obj.limbDismemberedLevel[%limb]) %this.RbloodDismember(%obj,%limb,false);
}

function Armor::onBotMelee(%obj,%col)
{

}

function Armor::onPinLoop(%this,%obj,%col)
{

}

package Player_L4B
{
	function onObjectCollisionTest(%obj, %col)//This function is part of the ObjectCollision.dll, please do not modify it unless you know what you are doing
	{
		if(isObject(%obj) && isObject(%col))
		{
			if(%obj.getType() & $TypeMasks::PlayerObjectType && %col.getType() & $TypeMasks::PlayerObjectType) 
			{
				if(%obj.getdataBlock().isSurvivor && %col.getdataBlock().isSurvivor) return false;

				switch$(%obj.getdataBlock().getName())
				{
					case "ZombieChargerHoleBot": if(vectordist(%obj.getposition(),%col.getposition()) < 2.5 && (%force = mFloor(vectorDot(getWords(%obj.getVelocity(),0,1), %obj.getForwardVector()))) > 15)
												{
													if(%col != %obj && %col.getdataBlock().getName() !$= "ZombieTankHoleBot")
													{
														if(!%obj.hEating)
														{
															%obj.SpecialPinAttack(%col,%force);
															%obj.playaudio(3,"charger_smash_sound");
														}

														if(%col != %obj.hEating)
														{
															%obj.playaudio(3,"charger_smash_sound");			
															%obj.playthread(2,"shiftUp");
															%col.setvelocity(vectorscale(VectorNormalize(vectorAdd(%obj.getForwardVector(),"0" SPC "0" SPC "0.25")),%force));
															%col.damage(%obj.hFakeProjectile, %col.getposition(),5, %obj.hDamageType);
															%obj.spawnExplosion(pushBroomProjectile,"1 1 1");
														}

														return false;
													}
												}
					case "ZombieHunterHoleBot": if(%obj.hEating == %col) return false;
				}
				//if(%obj.hIsInfected && %col.hIsInfected) return false;
			}

			return true;
		}
	}

	function WeaponImage::onMount(%this,%obj,%slot)
	{		
		Parent::onMount(%this,%obj,%slot);
		if(%obj.getdataBlock().isSurvivor && !%obj.getdataBlock().isDowned && (%obj.getMountedImage(0).armReady || %obj.getMountedImage(0).melee)) %obj.setDataBlock("SurvivorPlayerAiming");
	}

	function WeaponImage::onUnMount(%this,%obj,%slot)
	{		
		Parent::onUnMount(%this,%obj,%slot);
		if(%obj.getdataBlock().isSurvivor && !%obj.getdataBlock().isDowned) %obj.setDataBlock("SurvivorPlayer");
	}	
	
	function AIPlayer::setCrouching(%player,%bool)
	{
		if(%player.nolegs) %bool = true; 
		Parent::setCrouching(%player,%bool);
	}

	function AIPlayer::setJumping(%player,%bool)
	{
		if(%player.nolegs) %bool = false;
		Parent::setJumping(%player,%bool);
	}

	function AIPlayer::hLoop(%obj)
	{
		if(!isObject(%obj)) return;
		else Parent::hLoop(%obj);

		if(%obj.getDataBlock().hZombieL4BType !$= "")//Bypass the default garbage
		{
			%obj.setMoveTolerance(2.5);
			%obj.setMoveSlowdown(0);
		}
	}

	function AIPlayer::hMeleeAttack(%obj,%col)
	{
		if(%obj.getState() $= "Dead") return;

		if(%col.getType() & $TypeMasks::VehicleObjectType || %col.getType() & $TypeMasks::PlayerObjectType)
		{
			if(%obj.hState $= "Following" || %obj.Distraction)
			{
				%damage = %obj.hAttackDamage*getWord(%obj.getScale(),0);
				%damagefinal = getRandom(%damage/4,%damage);
				%obj.hlastmeleedamage = %damagefinal;
				%obj.lastattacked = getsimtime()+1000;

				%obj.playthread(2,activate2);
				%col.damage(%obj.hFakeProjectile, %col.getposition(), %damagefinal, %obj.hDamageType);
				%obj.getDataBlock().onBotMelee(%obj,%col);				
			}
		}
	}

	function Player::ActivateStuff (%player)//Not parenting, I made an overhaul of this function so it might cause compatibility issues...
	{
		%client = %player.client;
		%player.lastActivated = 0;

		if(isObject(%client.miniGame) && %client.miniGame.WeaponDamage && getSimTime() - %client.lastF8Time < 5000 || %player.getDamagePercent () >= 1) return false;
		
		if(getSimTime() - %player.lastActivateTime <= 320) %player.activateLevel += 1;
		else %player.activateLevel = 0;		
		%player.lastActivateTime = getSimTime();
		
		if(%player.activateLevel > 4) %player.playThread (3, activate2);
		else %player.playThread (3, activate);		

		%start = %player.getEyePoint ();
		%vec = %player.getEyeVector ();
		%scale = getWord (%player.getScale (), 2);
		%end = VectorAdd (%start, VectorScale (%vec, 3 * %scale));
		%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::ItemObjectType;

		if(%player.isMounted()) %exempt = %player.getObjectMount();
		else %exempt = %player;
		%search = containerRayCast (%start, %end, %mask, %exempt);
		
		if(isObject(%search))
		{
			%pos = getWords (%search, 1, 3);
			switch$(%search.getclassname()) 
			{
				case "fxDtsBrick":%search.onActivate (%player, %client, %pos, %vec);
				case "Item":if(%search.canPickup)
							{
								if(%search.getdataBlock().throwableExplosive)
								{									            
									%player.mountImage(%search.getdataBlock().image,0);
									%search.delete();
									return;
								}

								for(%i=0;%i<%player.getdataBlock().maxTools;%i++)
								if(%search.getDataBlock() == %player.tool[%i]) return;
								%player.pickup(%search);
							}
				case "WheeledVehicle": 	if(isFunction(%search, onActivate)) %search.onActivate(%player, %client, %pos, %vec);
										if(isObject(%search.getdatablock().image))
										{
											%player.mountImage(%search.getdataBlock().image,0);
											%search.delete();
										}

				default: if(isFunction(%search, onActivate)) %search.onActivate(%player, %client, %pos, %vec);
			}

			%player.lastActivated = %search;
			return true;
		}
		else return false;
	
	}

	function Player::PutItemInSlot(%obj,%slot,%item)
	{
		if(!%obj.tool[%slot])
		{
			%obj.tool[%slot] = %item.getDataBlock();
			messageClient(%obj.client,'MsgItemPickup','',%slot,%item.getDataBlock());

			if(isObject(%brick = %item.spawnBrick) && strstr(strlwr(%brick.getName()), "nodis") != -1)
			{
				%item.fadeOut();
				%item.respawn();
			}
			else %item.delete();

			return true;
		}
		else 
		{
			%obj.client.centerprint("<color:FFFFFF><font:impact:40>Drop slot<color:FFFF00>" SPC %slot+1 SPC "<color:FFFFFF>to pickup <color:FFFF00>" @ %item.getdataBlock().uiName,2);
			return false;
		}
	}

	function Player::Pickup(%obj,%item)
	{		
		if(%obj.getDatablock().isSurvivor)
		{
			if(!isObject(%item) || !%item.canPickup || !miniGameCanUse(%obj,%item)) return;
			
			if(%item.getdatablock().throwableExplosive)
			{
				ServerCmdUnUseTool(%obj.client);
				%obj.mountImage(%item.getdatablock().image, 0);
				
				%item.delete();
				return;
			}

			if(%item.getdataBlock().L4Bitemslot $= "NoSlot") return Parent::pickup(%obj,%item);

			switch$(%item.getdataBlock().L4Bitemslot)
			{
        	    case "Secondary": %obj.PutItemInSlot(1,%item);
				case "Grenade": %obj.PutItemInSlot(2,%item);
				case "Medical": %obj.PutItemInSlot(3,%item);
				case "Medical_Secondary": %obj.PutItemInSlot(4,%item);
				default: %obj.PutItemInSlot(0,%item);
			}
			return;
		}

		Parent::Pickup(%obj,%item);
	}

	function Armor::onRemove(%this,%obj)
	{
		if(isObject(%obj.hatprop)) %obj.hatprop.delete();
		for(%i = 0; %i < %obj.getMountedObjectCount(); %i++) if(isObject(%obj.getMountedObject(%i)) && %obj.getMountedObject(%i).getDataBlock().getName() $= "EmptyPlayer") %obj.getMountedObject(%i).delete();
		
		Parent::onRemove(%this,%obj);
	}

	function Armor::onImpact(%this, %obj, %col, %vec, %force)
	{
		Parent::onImpact(%this, %obj, %col, %vec, %force);

		if(%force < 40) serverPlay3D("impact_medium" @ getRandom(1,3) @ "_sound",%obj.getPosition());
		else serverPlay3D("impact_hard" @ getRandom(1,3) @ "_sound",%obj.getPosition());

		%oScale = getWord(%obj.getScale(),2);
		%forcescale = %force/25 * %oscale;
		%obj.spawnExplosion(pushBroomProjectile,%forcescale SPC %forcescale SPC %forcescale);

		if(%obj.getState() !$= "Dead" && getWord(%vec,2) > %obj.getdataBlock().minImpactSpeed)
        {
			serverPlay3D("impact_fall_sound",%obj.getPosition());
			if(%obj.getClassName() $= "Player") %obj.spawnExplosion("ZombieHitProjectile",%force/15 SPC %force/15 SPC %force/15);

		}		
	}

	function Armor::onNewDatablock(%this, %obj)
	{		
		Parent::onNewDatablock(%this, %obj);

		if(%this.hType !$= "Zombie")
		{
			%obj.hZombieL4BType = "";
			%obj.hIsInfected = "";

			if(%obj.getdatablock().hType !$= "") %obj.hType = %obj.getdatablock().hType;
			if(isObject(%obj.client) && %obj.getDamagePercent() == 1) commandToClient( %obj.client, 'SetVignette', $EnvGuiServer::VignetteMultiply, $EnvGuiServer::VignetteColor);
		}
	}

	function serverCmdUseTool(%client, %tool)
	{		
		if(!%client.player.isBeingStrangled) return parent::serverCmdUseTool(%client, %tool);
	}

	function ServerCmdDropTool (%client, %position)
	{
		if(isObject (%player = %client.player) && isObject(%item = %player.tool[%position]) && %item.canDrop)
		%player.playthread(3,"activate");
		%bool = Parent::ServerCmdDropTool (%client, %position);
	}

	function holeZombieInfect(%obj, %col)
	{					
		return;
		if(fileName(%col.getDataBlock().shapeFile) $= "skeleton.dts") return;

		if(%col.getDataBlock().usesL4Bappearance)
		{		
			%col.isBeingStrangled = false;

			%col.setDataBlock(CommonZombieHoleBot);
			switch$(%col.getclassname())
			{
				case "AIPlayer": %col.hChangeBotToInfectedAppearance();

				case "Player": 	if(isObject(%minigame = getMiniGameFromObject(%obj)))
								{
							   		%col.client.deletel4bMusic("Music");
									%col.client.deletel4bMusic("Music2");									
									%minigame.L4B_ChatMessage("\c0" @ %obj.getDatablock().hName SPC "<bitmapk:Add-Ons/Gamemode_Left4Block/add-ins/player_l4b/icons/ci_infected>" SPC %col.client.name,"survivor_left4dead_sound",true);
							   		%minigame.checkLastManStanding();
								}

							   	for(%i = 0; %i < %col.getdatablock().maxTools; %i++) 
							   	{
									%col.tool[%i] = 0;
									messageClient(%col.client,'MsgItemPickup','',%i,0);
							   	}
							   
							   	if(isObject(%col.getMountedImage(0)))
							   	{
									ServerCmdDropTool(%col.client, %col.getHackPosition());
							   		%col.playthread(1,root);
									L4B_ZombieDropLoot(%col,%col.getMountedImage(0).item,100);
							   	}
			}
		}
	}

	function Observer::onTrigger(%this, %obj, %a, %b)
	{
		%client = %obj.getControllingClient();

		if(isObject(%client.player) && %client.player.isBeingStrangled)
		{
			if(%b) %client.player.activateStuff();	
			return;
		}

		Parent::onTrigger(%this, %obj, %a, %b);
	}

	function fxDTSBrick::onActivate (%obj, %player, %client, %pos, %vec)
	{
		if(%obj.getdataBlock().IsZoneBrick && %obj.getgroup().bl_id == %player.client.bl_id)
		{
			if(strstr(strlwr(%obj.getName()),"_main") != -1)
			if(%player.client.currAreaZone !$= %obj.AreaZone)
			{
				%player.client.currAreaZone = %obj.AreaZone;
				%player.client.centerprint("\c2Set current checker <br>\c2" @ %obj.AreaZone.ParBrick,3);

				%removenoctnum = getSubStr(%obj.getName(), 0, strstr(strlwr(%obj.getName()),"_ct")+3);
				%num = strreplace(%obj.getName(), %removenoctnum, "");
				%player.client.AZCount = %num;
			}
		}

		if(%player.hType $= "Zombie" && %obj.getdatablock().isDoor)
		{
			%obj.playSound("BrickRotateSound");
			
			if(%obj.getdatablock().uiName !$= "L4D Door" && %player.breakDoorTime < getSimTime())
			{
				if(%obj.breakDoorCount < 10) %obj.breakDoorCount++;
				else
				{
					%obj.zfakeKillBrick();
					%obj.breakDoorCount = 0;
				}

				%obj.playSound("BrickBreakSound");
				%player.breakDoorTime = getSimTime()+250;
			}
			return;
		}

		Parent::onActivate (%obj, %player, %client, %pos, %vec);
	}

	function fxDTSBrickData::onPlayerTouch(%data,%obj,%player)
	{
		Parent::onPlayerTouch(%data,%obj,%player);

		if(%player.getDatablock().isSurvivor) %obj.processInputEvent("onSurvivorTouch");
		
		if(%player.hType $= "zombie") 
		{
			%obj.processInputEvent("onZombieTouch");
			if(%player.getDatablock().getName() $= "ZombieTankHoleBot") %obj.processInputEvent("onTankTouch");
		}			
	}

	function GameConnection::createPlayer (%client, %spawnPoint)
	{
		Parent::createPlayer (%client, %spawnPoint);
		
		if(%client.team $= "zombie" && isObject(%client.Player))
		{
			%client.player.setDataBlock($hZombieSpecialType[getRandom(1,$hZombieSpecialTypeAmount)]);
			commandToClient (%client, 'ShowEnergyBar', true);
		}
	}

	function GameConnection::applyBodyColors(%client,%o) 
	{
		Parent::applyBodyColors(%client,%o);
		
		if(isObject(%player = %client.player) && %player.getDataBlock().usesL4Bappearance) %player.getDataBlock().L4BAppearance(%player,%client);
	}

	function GameConnection::applyBodyParts(%client,%o) 
	{
		Parent::applyBodyParts(%client,%o);
		
		if(isObject(%player = %client.player) && %player.getDataBlock().usesL4Bappearance) %player.getDataBlock().L4BAppearance(%player,%client);
	}
};

if(isPackage(holeZombiePackage)) deactivatePackage(holeZombiePackage);
if(isPackage(BotHolePackage))
{
	deactivatePackage(Player_L4B);
	deactivatePackage(BotHolePackage);
	activatePackage(BotHolePackage);
	activatePackage(Player_L4B);
}

