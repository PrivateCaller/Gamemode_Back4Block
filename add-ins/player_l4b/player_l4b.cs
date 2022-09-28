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
luaexec("./survivor.lua");
luaexec("./zombies.lua");
exec("./bots/common.cs");
exec("./bots/uncommon_soldier.cs");
exec("./bots/uncommon_fallen.cs");
exec("./bots/uncommon_mud.cs");
exec("./bots/uncommon_construction.cs");
exec("./bots/uncommon_clown.cs");
exec("./bots/uncommon_jimmy_gibbs.cs");
exec("./bots/uncommon_ceda.cs");
exec("./bots/uncommon_toxic.cs");
exec("./bots/uncommon_pirate.cs");
exec("./bots/uncommon_police.cs");
exec("./bots/boomer.cs");
exec("./bots/charger.cs");
exec("./bots/hunter.cs");
exec("./bots/jockey.cs");
exec("./bots/smoker.cs");
exec("./bots/spitter.cs");
exec("./bots/tank.cs");
exec("./bots/witch.cs");

if(LoadRequiredAddOn("Support_BotHolePlus") == $Error::None)
{
	exec("./bots/survivor.cs");
	exec("./add-ins/support_packages/support_afk_system.cs");
}

package L4B_NewPlayerDatablock
{
	function Armor::onRemove(%this,%obj)
	{
		Parent::onRemove(%this,%obj);

		if(isObject(%obj.hatprop)) %obj.hatprop.delete();
		if(isObject(%obj.headbloodbot)) %obj.headbloodbot.delete();
		if(isObject(%obj.chestbloodbot)) %obj.chestbloodbot.delete();
		if(isObject(%obj.pantsbloodbot)) %obj.pantsbloodbot.delete();
	}

    function applyCharacterPrefs(%client)
	{	
		if(!isObject(%client.Player) || %client.player.getdatablock().getName() $= "ZombieTankHoleBot") return;

		%client.applyBodyParts();
		%client.applyBodyColors();
	}

	function player::fixAppearance(%pl,%cl)
	{
		%pl.hideNode("ALL");
		%pl.unHideNode((%cl.chest ? "femChest" : "chest"));
		%pl.unHideNode("rhand");
		%pl.unHideNode("lhand");
		%pl.unHideNode((%cl.rarm ? "rarmSlim" : "rarm"));
		%pl.unHideNode((%cl.larm ? "larmSlim" : "larm"));
		%pl.unHideNode("headskin");
		%pl.unHideNode("pants");
		%pl.unHideNode("rshoe");
		%pl.unHideNode("lshoe");
		%pl.setHeadUp(0);

		if(%cl.isAdmin || %cl.isSuper) %pl.shades = 1;
		if(%pl.shades)
		{
			%pl.unHideNode("shades");
			%pl.setNodeColor("shades","0.1 0.1 0.1 1");
		}	

		if($Pack[%cl.Pack] !$= "none")
		{
			switch(%cl.Pack)
			{
				case 1: %pl.unHideNode("armor");
						%pl.setNodeColor("armor",%cl.PackColor);
				case 2: %pl.unHideNode("alicepack");
						%pl.setNodeColor("alicepack",%cl.PackColor);
				case 4: %pl.unHideNode("pack");
						%pl.setNodeColor("pack",%cl.PackColor);
			}
		}

		if($secondPack[%cl.secondPack] !$= "none")
		{
			switch(%cl.secondPack)
			{
				case 1: %pl.unHideNode("epaulets");
						%pl.setNodeColor("epaulets",%cl.secondPackColor);
				case 6: %pl.unHideNode("shoulderpads");
						%pl.setNodeColor("shoulderpads",%cl.secondPackColor);
			}
		}

		if($hat[%cl.hat] !$= "none")
		{
			switch(%cl.hat)
			{
				case 1: %pl.unHideNode("helmet");
						%pl.setNodeColor("helmet",%cl.hatColor);

						if(%cl.accent == 1)
						{
							%pl.unHideNode("visor");
							%pl.setNodeColor("visor",%cl.accentColor);
						}

				case 4:	%pl.unHideNode("scoutHat");
						%pl.setNodeColor("scoutHat",%cl.hatColor);
				case 6:	%pl.unHideNode("copHat");
						%pl.setNodeColor("copHat",%cl.hatColor);
				case 7:	%pl.unHideNode("knitHat");
						%pl.setNodeColor("knitHat",%cl.hatColor);
			}
		}

		if(%pl.getDatablock().hType $= "Zombie")
		{
			%pl.unhidenode("gloweyes");
			%pl.setnodeColor("gloweyes","1 1 0 1");

			if(%pl.getClassName() $= "Player")
			{
				%skin = %cl.headColor;
				%zskin = getWord(%skin,0)/2.75 SPC getWord(%skin,1)/1.5 SPC getWord(%skin,2)/2.75 SPC 1;
				%pl.client.zombieColor = %zskin;
				%pl.setNodeColor("headskin", %zSkin);

				if(%cl.rArmColor $= %skin) %pl.setNodeColor((%cl.rarm ? "rarmSlim" : "rarm"), %zSkin);
				if(%cl.lArmColor $= %skin) %pl.setNodeColor((%cl.larm ? "larmSlim" : "larm"), %zSkin);
				if(%cl.rhandColor $= %skin) %pl.setNodeColor("rhand", %zSkin);
				if(%cl.lhandColor $= %skin) %pl.setNodeColor("lhand", %zSkin);
				if(%cl.chestColor $= %skin) %pl.setNodeColor((%cl.chest ? "femChest" : "chest"), %zSkin);
				if(%cl.hipColor $= %skin) %pl.setNodeColor("pants", %zSkin);
				if(%cl.rLegColor $= %skin) %pl.setNodeColor("rshoe", %zSkin);
				if(%cl.lLegColor $= %skin) %pl.setNodeColor("lshoe", %zSkin);
				%pl.setFaceName("asciiTerror");

				//switch$(%pl.getDataBlock().getName())
				//{
				//	case "ZombieHunterHoleBot": %pl.hideNode("RarmSlim");
				//								%pl.hideNode("LarmSlim");
				//								%pl.unhideNode("helmet");
				//								%pl.unhideNode("LhandWitch");
				//								%pl.unhideNode("RhandWitch");
				//								%pl.setNodeColor("LhandWitch",%zSkin);
				//								%pl.setNodeColor("RhandWitch",%zSkin);
				//								%pl.unhideNode("Larm");
				//								%pl.unhideNode("Rarm");
				//								%pl.setNodeColor("Larm",%cl.lArmColor);
				//								%pl.setNodeColor("Rarm",%cl.rArmColor);
				//								%pl.setDecalName("Hoodie");	
				//	//case "ZombieChargerHoleBot": %pl.setNodeColor($Chest[0], %zSkin);
				//	//							 %pl.setNodeColor($Chest[1], %zSkin);
				//	//							 %pl.setDecalName("worm_engineer");						 
				//	//case "ZombieJockeyHoleBot": %pl.setNodeColor($RLeg[0], %zSkin);
				//	//							%pl.setNodeColor($RLeg[1], %zSkin);
				//	//							%pl.setNodeColor($LLeg[0], %zSkin);
				//	//							%pl.setNodeColor($LLeg[1], %zSkin);
				//	//							%pl.setNodeColor($Chest[0], %zSkin);
				//	//							%pl.setNodeColor($Chest[1], %zSkin);
				//	//							%pl.setNodeColor($LHand[0], %zSkin);
				//	//							%pl.setNodeColor($LHand[1], %zSkin);
				//	//							%pl.setNodeColor($RHand[0], %zSkin);
				//	//							%pl.setNodeColor($RHand[1], %zSkin);
				//	//							%pl.setNodeColor($RArm[0], %zSkin);
				//	//							%pl.setNodeColor($RArm[1], %zSkin);
				//	//							%pl.setNodeColor($LArm[0], %zSkin);
				//	//							%pl.setNodeColor($LArm[1], %zSkin);
				//	//							%pl.setDecalName("AAA-None");
				//	default:
			}	
		}			

		%pl.setFaceName(%cl.faceName);
		%pl.setDecalName(%cl.decalName);
		%pl.setNodeColor("headskin",%cl.headColor);
		%pl.setNodeColor("chest",%cl.chestColor);
		%pl.setNodeColor("femChest",%cl.chestColor);
		%pl.setNodeColor("pants",%cl.hipColor);
		%pl.setNodeColor("rarm",%cl.rarmColor);
		%pl.setNodeColor("larm",%cl.larmColor);
		%pl.setNodeColor("rarmSlim",%cl.rarmColor);
		%pl.setNodeColor("larmSlim",%cl.larmColor);
		%pl.setNodeColor("rhand",%cl.rhandColor);
		%pl.setNodeColor("lhand",%cl.lhandColor);
		%pl.setNodeColor("rshoe",%cl.rlegColor);
		%pl.setNodeColor("lshoe",%cl.llegColor);
		%pl.setNodeColor("headpart1",%cl.headColor);
		%pl.setNodeColor("headpart2",%cl.headColor);
		%pl.setNodeColor("headpart3",%cl.headColor);
		%pl.setNodeColor("headpart4",%cl.headColor);
		%pl.setNodeColor("headpart5",%cl.headColor);
		%pl.setNodeColor("headpart6",%cl.headColor);
		%pl.setNodeColor("headskullpart1","1 0.5 0.5 1");
		%pl.setNodeColor("headskullpart2","1 0.5 0.5 1");
		%pl.setNodeColor("headskullpart3","1 0.5 0.5 1");
		%pl.setNodeColor("headskullpart4","1 0.5 0.5 1");
		%pl.setNodeColor("headskullpart5","1 0.5 0.5 1");
		%pl.setNodeColor("headskullpart6","1 0.5 0.5 1");
		%pl.setNodeColor("headstump","1 0 0 1");
		%pl.setNodeColor("legstumpr","1 0 0 1");
		%pl.setNodeColor("legstumpl","1 0 0 1");
		%pl.setNodeColor("chestpart1",%cl.chestColor);
		%pl.setNodeColor("chestpart2",%cl.chestColor);
		%pl.setNodeColor("chestpart3",%cl.chestColor);
		%pl.setNodeColor("chestpart4",%cl.chestColor);
		%pl.setNodeColor("chestpart5",%cl.chestColor);
		%pl.setNodeColor("pants",%cl.hipColor);
		%pl.setNodeColor("pantswound",%cl.hipColor);
		%pl.setNodeColor("skeletonchest","1 0.5 0.5 1");
		%pl.setNodeColor("skelepants","1 0.5 0.5 1");
		%pl.setNodeColor("organs","1 0.6 0.5 1");
		%pl.setNodeColor("brain","1 0.75 0.746814 1");		

		%pl.getDataBlock().hCustomNodeAppearance(%pl);
	}

	function Player::woundappearance(%obj,%type)
	{		
		switch$(%type)
		{
			case "chest": 	%chestbloodbot = new Player() { dataBlock = "EmptyPlayer"; };
							%obj.chestbloodbot = %chestbloodbot;
							%obj.mountobject(%chestbloodbot,2);
							%chestbloodbot.mountImage(RBloodLargeImage,0);
			
							if(!%obj.chest)
							{								
								if(!%chestwoundonce)
								{
									%obj.HideNode("chest");
									%obj.unHideNode("chestpart1");
									%obj.unHideNode("chestpart2");
									%obj.unHideNode("chestpart3");
									%obj.unHideNode("chestpart4");
									%obj.unHideNode("chestpart5");
									%obj.unHideNode("chestpart6");
									%obj.unHideNode("chestpart7");
									%obj.unHideNode("chestpart8");			

									%chestwoundonce = true;
									%chestwoundmaxcount = getRandom(4,8);

									for (%cwc = 0; %cwc < %chestwoundmaxcount; %cwc++) 
									{
										%chestremove = getRandom(8);
										%obj.HideNode("chestpart" @ %chestremove);

										if(%chestremove)
										{				
											%obj.unHideNode("skeletonchest");
											%obj.unhidenode("organs");
										}
									}
								}
								else
								{
									%obj.unHideNode("chest");
									%obj.HideNode("chestpart1");
									%obj.HideNode("chestpart2");
									%obj.HideNode("chestpart3");
									%obj.HideNode("chestpart4");
									%obj.HideNode("chestpart5");
									%obj.HideNode("chestpart6");
									%obj.HideNode("chestpart7");
									%obj.HideNode("chestpart8");
								}
							}
							else 
							{
								if(!%chestwoundonce)
								{
									%obj.HideNode("femchest");
									%obj.unHideNode("femchestpart1");
									%obj.unHideNode("femchestpart2");
									%obj.unHideNode("femchestpart3");
									%obj.unHideNode("femchestpart4");
									%obj.unHideNode("femchestpart5");
									%obj.unHideNode("femchestpart6");
									%obj.unHideNode("femchestpart7");
									%obj.unHideNode("femchestpart8");			

									%chestwoundonce = true;
									%chestwoundmaxcount = getRandom(4,8);

									for (%cwc = 0; %cwc < %chestwoundmaxcount; %cwc++) 
									{
										%chestremove = getRandom(8);
										%obj.HideNode("femchestpart" @ %chestremove);

										%obj.unHideNode("skeletonchest");
										%obj.unhidenode("organs");
									}
								}
								else
								{
									%obj.unHideNode("femchest");
									%obj.HideNode("femchestpart1");
									%obj.HideNode("femchestpart2");
									%obj.HideNode("femchestpart3");
									%obj.HideNode("femchestpart4");
									%obj.HideNode("femchestpart5");
									%obj.HideNode("femchestpart6");
									%obj.HideNode("femchestpart7");
									%obj.HideNode("femchestpart8");
								}
							}
							
			case "head": 	%obj.HideNode("headskin");
							%obj.HideNode("gloweyes");

							%headbloodbot = new Player() { dataBlock = "EmptyPlayer"; };
							%obj.headbloodbot = %headbloodbot;
							%obj.mountobject(%headbloodbot,5);
							%headbloodbot.mountImage(RBloodLargeImage,0);

							if(%obj.getClassName() $= "AIPlayer" && %obj.hat)
							{
								%hat = new Player() 
								{ 
									dataBlock = "PlayerHatModel"; 
									currentHat = %obj.hat;
									color = %obj.hatcolor;
								};
								%obj.hatprop = %hat;
								%hat.hideNode("ALL");
								%hat.unhideNode($hat[%hat.currentHat]);
								%hat.setNodeColor($hat[%hat.currentHat],%hat.color);	
								%hat.setTransform(%obj.getTransform());
								%hat.setDamageLevel(%hat.getdataBlock().maxDamage);
								%hat.position = vectorAdd(%obj.getMuzzlePoint(2),"0 0 0.35");
								
								%objhalfvelocity = "\"" @ getWord(%obj.getVelocity(),0)/2 SPC getWord(%obj.getVelocity(),1)/2 SPC getWord(%obj.getVelocity(),2)/2 @ "\"";
								%hat.setvelocity(vectorAdd(%objhalfvelocity,getRandom(-8,8) SPC getRandom(-8,8) SPC getRandom(5,10)));
							}	

							if(getRandom(1,0) == 1) 
							{
								%obj.unhidenode("brain");
								%obj.unHideNode("headpart1");
								%obj.unHideNode("headpart2");
								%obj.unHideNode("headpart3");
								%obj.unHideNode("headpart4");
								%obj.unHideNode("headpart5");
								%obj.unHideNode("headpart6");
								%obj.unHideNode("headskullpart1");
								%obj.unHideNode("headskullpart2");
								%obj.unHideNode("headskullpart3");
								%obj.unHideNode("headskullpart4");
								%obj.unHideNode("headskullpart5");
								%obj.unHideNode("headskullpart6");	

								%headwoundmaxcount = getRandom(3);
								for (%hwc = 0; %hwc < %headwoundmaxcount; %hwc++) 
								{
									%headremove = getRandom(1,6);
									%obj.HideNode("headpart" @ %headremove);
									%obj.HideNode("headskullpart" @ %headremove);
								}
							}
							else %obj.unhideNode("headstump");
							
														
			case "rightarm":	%obj.unhidenode("rarmSlim");
								%obj.setNodeColor("rarmSlim","1 0 0 1");

			case "leftarm":		%obj.unhidenode("larmSlim");
								%obj.setNodeColor("larmSlim","1 0 0 1");

			case "rightleg":	%obj.unhideNode("legstumpr");
								%obj.setNodeColor("legstumpr","1 0 0 1");

			case "leftleg":		%obj.unhideNode("legstumpl");
								%obj.setNodeColor("legstumpl","1 0 0 1");

			case "hip":			%pantsbloodbot = new Player() { dataBlock = "EmptyPlayer"; };
								%obj.pantsbloodbot = %pantsbloodbot;
								%obj.mountobject(%pantsbloodbot,7);
								%pantsbloodbot.mountImage(RBloodLargeImage,0);
			
								%obj.HideNode("pants");
								%obj.unHideNode("skelepants");
								%obj.unHideNode("pantswound");

								if(%obj.getClassName() $= "AIPlayer") %obj.setcrouching(1);
								%obj.nolegs = 1;
		}
	}

	function AIPlayer::woundappearance(%obj,%type) { Player::woundappearance(%obj,%type); }

	function GameConnection::createPlayer (%client, %spawnPoint)
	{
		Parent::createPlayer (%client, %spawnPoint);

		if(isObject(%client.Player)) %client.Player.spawnTime = getSimTime();
		
		if(%client.setZombieBlock)
		if(isObject(%client.Player))
		{
			%client.player.setDataBlock($hZombieSpecialType[getRandom(1,$hZombieSpecialTypeAmount)]);
			commandToClient (%client, 'ShowEnergyBar', true);
			%client.setZombieBlock = 0;
		}
	}

	function GameConnection::applyBodyColors(%cl,%o) 
	{
		if(isObject(%cl.player) && %cl.player.getdataBlock().getName() !$= "ZombieTankHoleBot")
		parent::applyBodyColors(%cl,%o);
		
		if(isObject(%pl = %cl.player))
		if(fileName(%pl.getDataBlock().shapeFile) $= "newm.dts")
		%pl.fixAppearance(%cl);
	}

	function GameConnection::applyBodyParts(%cl,%o) 
	{
		if(isObject(%cl.player) && %cl.player.getdataBlock().getName() !$= "ZombieTankHoleBot")
		parent::applyBodyParts(%cl,%o);
		
		if(isObject(%pl = %cl.player))
		if(fileName(%pl.getDataBlock().shapeFile) $= "newm.dts")
		%pl.fixAppearance(%cl);
	}
};
activatePackage(L4B_NewPlayerDatablock);