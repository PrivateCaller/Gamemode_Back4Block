registerOutputEvent(fxDTSBrick, "RandomizeZombieSpecial");
registerOutputEvent(fxDTSBrick, "RandomizeZombieUncommon");
registerInputEvent ("fxDTSBrick", "onZombieTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerInputEvent ("fxDTSBrick", "onTankTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerInputEvent ("fxDTSBrick", "onDoorClose", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerInputEvent ("fxDTSBrick", "onDoorOpen", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerOutputEvent ("fxDTSBrick", "zfakeKillBrick");
registerOutputEvent ("Player", "Safehouse","bool");
registerInputEvent ("fxDTSBrick", "onSurvivorTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB "Client GameConnection" TAB "MiniGame MiniGame");
registerOutputEvent(Minigame, "SafehouseCheck");

$hZombieHat[%c++] = 4;
$hZombieHat[%c++] = 6;
$hZombieHat[%c++] = 7;
$hZombieHatAmount = %c;
$hZombiePack[%d++] = 2;
$hZombiePack[%d++] = 4;
$hZombiePackAmount = %d;

exec("./datablocks.cs");
exec("./survivor.cs");
exec("./zombies.cs");

package L4B_PlayerBot
{
	function onObjectCollisionTest(%obj, %col)
	{	
		if(isObject(%obj) && isObject(%col))
		{
			if(%obj.getType() & $TypeMasks::PlayerObjectType && %col.getType() & $TypeMasks::PlayerObjectType) 
			{
				if(%obj.getdataBlock().getName().isSurvivor && %col.getdataBlock().getName().isSurvivor) return false;

				if(vectordist(%obj.getposition(),%col.getposition()) < 2 && %obj.getdataBlock().getName() $= "ZombieChargerHoleBot" && (%force = vectorDot(%obj.getVelocity(), %obj.getForwardVector())) > 20 && %col.getdataBlock().getName() !$= "ZombieTankHoleBot")
				{
					return false;
						
					%obj.playaudio(3,"charger_smash_sound");			
					%obj.playthread(2,"shiftUp");

					%eye = vectorscale(VectorNormalize(vectorAdd(%obj.getForwardVector(),"0" SPC "0" SPC "0.25")),%force);
					%col.setvelocity(%eye);					

					%col.damage(%obj.hFakeProjectile, %col.getposition(),5, %obj.hDamageType);
					%obj.spawnExplosion(pushBroomProjectile,"1 1 1");					
				}
			}
		return true;
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
		if(fileName(%col.getDataBlock().shapeFile) $= "skeleton.dts") return;

		if(%col.getDataBlock().usesL4Bappearance)
		{		
			%col.setDataBlock(CommonZombieHoleBot);
			switch$(%col.getclassname())
			{
				case "AIPlayer": %col.hChangeBotToInfectedAppearance();

				case "Player": 	if(isObject(%minigame = getMiniGameFromObject(%obj)))
								{
							   		%minigame.L4B_ChatMessage("<color:00FF00>" @ %obj.getDatablock().hName SPC "<bitmapk:Add-Ons/Gamemode_Left4Block/add-ins/player_l4b/icons/ci_infected>" SPC %col.client.name,"survivor_left4dead_sound",true);
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

	function AIPlayer::hMeleeAttack(%obj,%col)
	{						
		if(%obj.getState() $= "Dead" || ($admingod && %col.getclassname() $= "Player" && %col.client.isSuperAdmin)) return;

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
	
	function Armor::onImpact(%this, %obj, %col, %vec, %force)
	{
		if(%force < 40) serverPlay3D("impact_medium" @ getRandom(1,3) @ "_sound",%obj.getPosition());
		else serverPlay3D("impact_hard" @ getRandom(1,3) @ "_sound",%obj.getPosition());

		Parent::onImpact(%this, %obj, %col, %vec, %force);
	}

	function Armor::onNewDatablock(%this, %obj)
	{		
		Parent::onNewDatablock(%this, %obj);

		if(%this.hType !$= "Zombie")
		{
			%obj.hZombieL4BType = "";
			%obj.hIsInfected = "";

			if(%obj.getdatablock().hType !$= "") %obj.hType = %obj.getdatablock().hType;
			if(isObject(%obj.client)) commandToClient( %obj.client, 'SetVignette', $EnvGuiServer::VignetteMultiply, $EnvGuiServer::VignetteColor);
		}
	}		

	function Player::ActivateStuff (%player)
	{
		%client = %player.client;
		%player.lastActivated = 0;

		if(isObject(%client.miniGame) && %client.miniGame.WeaponDamage && getSimTime() - %client.lastF8Time < 5000 || %player.getDamagePercent () >= 1) return 0;	
		
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
								for(%i=0;%i<%player.getdataBlock().maxTools;%i++)
								if(%search.getDataBlock() == %player.tool[%i]) return;
								%player.pickup(%search);
							}

				default: if(isFunction(%search, onActivate)) %search.onActivate(%player, %client, %pos, %vec);
			}

			%player.lastActivated = %search;
			return %search;
		}
		else return 0;
	
	}

	function player::PutItemInSlot(%obj,%slot,%item)
	{
		if(%obj.tool[%slot] == 0)
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
		else return false;
	}

	function player::pickup(%obj,%item)
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

			if(%item.getdataBlock().L4Bitemslot $= "NoSlot")
			return Parent::pickup(%obj,%item);

			switch$(%item.getdataBlock().L4Bitemslot)
			{			            
        	    case "Secondary": %obj.PutItemInSlot(1,%item);
								  return;
				case "Grenade": %obj.PutItemInSlot(2,%item);
							  	return;
				case "Medical": %obj.PutItemInSlot(3,%item);
							  	return;
				case "Medical_Secondary": %obj.PutItemInSlot(4,%item);
							  			  return;
				default: %obj.PutItemInSlot(0,%item);
						 return;
			}
		}

		Parent::pickup(%obj,%item);
	}

	function Armor::onRemove(%this,%obj)
	{
		Parent::onRemove(%this,%obj);

		if(isObject(%obj.hatprop)) %obj.hatprop.delete();
		if(isObject(%obj.headbloodbot)) %obj.headbloodbot.delete();
		if(isObject(%obj.chestbloodbot)) %obj.chestbloodbot.delete();
		if(isObject(%obj.pantsbloodbot)) %obj.pantsbloodbot.delete();
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

	function Armor::onBotMelee(%obj,%col)
	{
		//stuff here
	}		
};

	function Armor::L4BAppearance(%this,%obj,%client)
	{
		%obj.hideNode("ALL");
		%obj.unHideNode((%client.chest ? "femChest" : "chest"));
		%obj.unHideNode("rhand");
		%obj.unHideNode("lhand");
		%obj.unHideNode((%client.rarm ? "rarmSlim" : "rarm"));
		%obj.unHideNode((%client.larm ? "larmSlim" : "larm"));
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

		if(%obj.getDatablock().hType $= "Zombie" && %obj.getclassname() $= "Player")
		{
			%obj.unhidenode("gloweyes");
			%obj.setnodeColor("gloweyes","1 1 0 1");

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
			%faceName = "asciiTerror";
		}

		if(%client.shades || %client.isAdmin || %client.isSuper)
		{
			%obj.unHideNode("shades");
			%obj.setNodeColor("shades","0.1 0.1 0.1 1");
		}

		if(%client.armor)
		{
			%obj.unHideNode("ballisticvest");
			%obj.setNodeColor("ballisticvest",%client.armorColor);
		}			

		if($Pack[%client.Pack] !$= "none")
		{
			switch(%client.Pack)
			{
				case 1: %obj.unHideNode("armor");
						%obj.setNodeColor("armor",%client.PackColor);
				case 2: %obj.unHideNode("alicepack");
						%obj.setNodeColor("alicepack",%client.PackColor);
				case 4: %obj.unHideNode("pack");
						%obj.setNodeColor("pack",%client.PackColor);
			}
		}

		if($secondPack[%client.secondPack] !$= "none")
		{
			switch(%client.secondPack)
			{
				case 1: %obj.unHideNode("epaulets");
						%obj.setNodeColor("epaulets",%client.secondPackColor);
				case 6: %obj.unHideNode("shoulderpads");
						%obj.setNodeColor("shoulderpads",%cl.secondPackColor);
			}
		}

		if($hat[%client.hat] !$= "none")
		{
			switch(%client.hat)
			{
				case 1: %obj.unHideNode("helmet");
						%obj.setNodeColor("helmet",%client.hatColor);

						if(%client.accent == 1)
						{
							%obj.unHideNode("visor");
							%obj.setNodeColor("visor",%client.accentColor);
						}

				case 4:	%obj.unHideNode("scoutHat");
						%obj.setNodeColor("scoutHat",%client.hatColor);
				case 6:	%obj.unHideNode("copHat");
						%obj.setNodeColor("copHat",%client.hatColor);
				case 7:	%obj.unHideNode("knitHat");
						%obj.setNodeColor("knitHat",%client.hatColor);
			}
		}

		%obj.setFaceName(%faceName);
		%obj.setDecalName(%client.decalName);
		%obj.setNodeColor("headskin",%headcolor);
		%obj.setNodeColor("chest",%chestcolor);
		%obj.setNodeColor("femChest",%chestcolor);
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

		if(%obj.getDatablock().hType $= "Zombie")
		{
			%obj.unhidenode("gloweyes");
			%obj.setnodeColor("gloweyes","1 1 0 1");	
		}					
	}

if(isPackage(holeZombiePackage)) deactivatePackage(holeZombiePackage);
if(isPackage(BotHolePackage))
{
	deactivatePackage(L4B_PlayerBot);
	deactivatePackage(BotHolePackage);
	activatePackage(BotHolePackage);
	activatePackage(L4B_PlayerBot);
}

