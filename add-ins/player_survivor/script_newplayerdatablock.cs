//NOTE TO SELF: TSShapeConstructor has to be done BEFORE player datablock.
datablock TSShapeConstructor(mMeleeDts) 
{
	baseShape = "base/data/shapes/player/mmelee.dts";
	sequence0 = "base/data/shapes/player/default.dsq";
	sequence1 = "base/data/shapes/player/melee.dsq";
	sequence2 = "base/data/shapes/player/actions.dsq";
	sequence3 = "base/data/shapes/player/taunts.dsq";
};

datablock PlayerData(PlayerMeleeAnims : PlayerStandardArmor)
{
	shapeFile = "base/data/shapes/player/mMelee.dts";
	uniformCompatible = true;
	canJet = false;
	uiName = "";
};

package L4B2Bots_NewPlayerDatablock
{
    function applyCharacterPrefs(%client)
	{	
		if(!isObject(%client.Player) || %client.player.getdatablock().getName() $= "ZombieTankHoleBot")
		return;

		%client.applyBodyParts();
		%client.applyBodyColors();
	}

	function player::fixAppearance(%pl,%cl)
	{
		%pl.hideNode("ALL");
		%pl.unHideNode((%cl.chest ? "femChest" : "chest"));
		
		%pl.unHideNode((%cl.rhand ? "rhook" : "rhand"));
		%pl.unHideNode((%cl.lhand ? "lhook" : "lhand"));
		%pl.unHideNode((%cl.rarm ? "rarmSlim" : "rarm"));
		%pl.unHideNode((%cl.larm ? "larmSlim" : "larm"));
		%pl.unHideNode("headskin");

		if($pack[%cl.pack] !$= "none")
		{
			%pl.unHideNode($pack[%cl.pack]);
			%pl.setNodeColor($pack[%cl.pack],%cl.packColor);
		}
		if($secondPack[%cl.secondPack] !$= "none")
		{
			%pl.unHideNode($secondPack[%cl.secondPack]);
			%pl.setNodeColor($secondPack[%cl.secondPack],%cl.secondPackColor);
		}
		if($hat[%cl.hat] !$= "none")
		{
			%pl.unHideNode($hat[%cl.hat]);
			%pl.setNodeColor($hat[%cl.hat],%cl.hatColor);
		}
		if(%cl.hip)
		{
			%pl.unHideNode("skirthip");
			%pl.unHideNode("skirttrimleft");
			%pl.unHideNode("skirttrimright");
		}
		else
		{
			%pl.unHideNode("pants");
			%pl.unHideNode((%cl.rleg ? "rpeg" : "rshoe"));
			%pl.unHideNode((%cl.lleg ? "lpeg" : "lshoe"));
		
		}
		%pl.setHeadUp(0);
		if(%cl.pack+%cl.secondPack > 0) %pl.setHeadUp(1);
		
		if($hat[%cl.hat] $= "Helmet")
		{
			if(%cl.accent == 1 && $accent[4] !$= "none")
			{
				%pl.unHideNode($accent[4]);
				%pl.setNodeColor($accent[4],%cl.accentColor);
			}
		}
		else if($accent[%cl.accent] !$= "none" && strpos($accentsAllowed[$hat[%cl.hat]],strlwr($accent[%cl.accent])) != -1)
		{
			%pl.unHideNode($accent[%cl.accent]);
			%pl.setNodeColor($accent[%cl.accent],%cl.accentColor);
		}
	
		%pl.setFaceName(%cl.faceName);
		%pl.setDecalName(%cl.decalName);
		
		%pl.setNodeColor("headskin",%cl.headColor);
		
		%pl.setNodeColor("chest",%cl.chestColor);
		%pl.setNodeColor("femChest",%cl.chestColor);
		%pl.setNodeColor("pants",%cl.hipColor);
		%pl.setNodeColor("skirthip",%cl.hipColor);
		
		%pl.setNodeColor("rarm",%cl.rarmColor);
		%pl.setNodeColor("larm",%cl.larmColor);
		%pl.setNodeColor("rarmSlim",%cl.rarmColor);
		%pl.setNodeColor("larmSlim",%cl.larmColor);
		
		%pl.setNodeColor("rhand",%cl.rhandColor);
		%pl.setNodeColor("lhand",%cl.lhandColor);
		%pl.setNodeColor("rhook",%cl.rhandColor);
		%pl.setNodeColor("lhook",%cl.lhandColor);
		
		%pl.setNodeColor("rshoe",%cl.rlegColor);
		%pl.setNodeColor("lshoe",%cl.llegColor);
		%pl.setNodeColor("rpeg",%cl.rlegColor);
		%pl.setNodeColor("lpeg",%cl.llegColor);
		%pl.setNodeColor("skirttrimright",%cl.rlegColor);
		%pl.setNodeColor("skirttrimleft",%cl.llegColor);
	
		%pl.getDataBlock().hCustomNodeAppearance(%pl);
	
		if(isObject(%cl) && %cl.isAdmin || %cl.isSuper || %cl.player.shades)
		{
			%cl.player.unHideNode("shades");
			%cl.player.setNodeColor("shades","0.1 0.1 0.1 1");
		}

		if(%pl.hIsInfected)
		{
			%pl.unhidenode("gloweyes");
			%pl.setnodeColor("gloweyes","1 1 0 1");

			if(%pl.getClassName() $= "Player")
			{
				%skin = %cl.headColor;
				%zskin = getWord(%skin,0)/2.75 SPC getWord(%skin,1)/1.5 SPC getWord(%skin,2)/2.75 SPC 1;
				%pl.client.zombieColor = %zskin;
				%pl.setNodeColor("headskin", %zSkin);
	
				if(%cl.rArmColor $= %skin)
				{
					%pl.setNodeColor($RArm[0], %zSkin);
					%pl.setNodeColor($RArm[1], %zSkin);
				}
	
				if(%cl.lArmColor $= %skin)
				{
					%pl.setNodeColor($LArm[0], %zSkin);
					%pl.setNodeColor($LArm[1], %zSkin);
				}
	
				if(%cl.rHandColor $= %skin)
				{
					%pl.setNodeColor($RHand[0], %zSkin);
					%pl.setNodeColor($RHand[1], %zSkin);
				}
	
				if(%cl.lHandColor $= %skin)
				{
					%pl.setNodeColor($LHand[0], %zSkin);
					%pl.setNodeColor($LHand[1], %zSkin);
				}
	
				if(%cl.chestColor $= %skin)
				{
					%pl.setNodeColor($Chest[0], %zSkin);
					%pl.setNodeColor($Chest[1], %zSkin);
				}
	
				if(%cl.hipColor $= %skin)
				{
					%pl.setNodeColor($Hip[0], %zSkin);
					%pl.setNodeColor($Hip[1], %zSkin);
				}
	
				if(%cl.rLegColor $= %skin)
				{
					%pl.setNodeColor($RLeg[0], %zSkin);
					%pl.setNodeColor($RLeg[1], %zSkin);
				}
	
				if(%cl.lLegColor $= %skin)
				{
					%pl.setNodeColor($LLeg[0], %zSkin);
					%pl.setNodeColor($LLeg[1], %zSkin);
				}
	
				if($Pref::Server::L4B2Bots::::CustomStyle < 2)
				%pl.setFaceName("asciiTerror");
				else %pl.setFaceName($hZombieFace[getRandom(1,$hZombieFaceAmount)]);
	
				switch$(%pl.getDataBlock().getName())
				{
					case "ZombieHunterHoleBot": %pl.hideNode($hat[%cl.hat]);
												%pl.unhideNode($hat[1]);
												%pl.hidenode(Lhand);
    											%pl.hidenode(Rhand);
												%pl.setnodecolor($hat[1],%cl.chestColor);
												%pl.setNodeColor($RArm[0],%cl.chestColor);
												%pl.setNodeColor($RArm[1],%cl.chestColor);
												%pl.setNodeColor($LArm[0],%cl.chestColor);
												%pl.setNodeColor($LArm[1],%cl.chestColor);
												%pl.setDecalName("Hoodie");
	
					case "ZombieChargerHoleBot": %pl.setNodeColor($Chest[0], %zSkin);
												 %pl.setNodeColor($Chest[1], %zSkin);
												 %pl.setDecalName("worm_engineer");
												
												 %pl.HideNode("lhand_blood");
												 %pl.HideNode("rhand_blood");
												 %pl.bloody["rhand"] = false;
												 %pl.bloody["lhand"] = false;
												 
					case "ZombieJockeyHoleBot": %pl.setNodeColor($RLeg[0], %zSkin);
												%pl.setNodeColor($RLeg[1], %zSkin);
												%pl.setNodeColor($LLeg[0], %zSkin);
												%pl.setNodeColor($LLeg[1], %zSkin);
												%pl.setNodeColor($Chest[0], %zSkin);
												%pl.setNodeColor($Chest[1], %zSkin);
												%pl.setNodeColor($LHand[0], %zSkin);
												%pl.setNodeColor($LHand[1], %zSkin);
												%pl.setNodeColor($RHand[0], %zSkin);
												%pl.setNodeColor($RHand[1], %zSkin);
												%pl.setNodeColor($RArm[0], %zSkin);
												%pl.setNodeColor($RArm[1], %zSkin);
												%pl.setNodeColor($LArm[0], %zSkin);
												%pl.setNodeColor($LArm[1], %zSkin);
												%pl.setDecalName("AAA-None");
					case "ZombieBoomerHoleBot": %pl.hideNode($hat[%cl.hat]);
					default:
				}
			}			
		}		
	}

	function GameConnection::createPlayer (%client, %spawnPoint)
	{
		Parent::createPlayer (%client, %spawnPoint);

		if(isObject(%client.Player))
		%client.Player.spawnTime = getSimTime();
		
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
		if((%pl.getDatablock()).shapeFile $= "base/data/shapes/player/mmelee.dts")
		%pl.fixAppearance(%cl);
	}

	function GameConnection::applyBodyParts(%cl,%o) 
	{
		if(isObject(%cl.player) && %cl.player.getdataBlock().getName() !$= "ZombieTankHoleBot")
		parent::applyBodyParts(%cl,%o);
		
		if(isObject(%pl = %cl.player))
		if((%pl.getDatablock()).shapeFile $= "base/data/shapes/player/mmelee.dts")
		%pl.fixAppearance(%cl);
	}
};
activatePackage(L4B2Bots_NewPlayerDatablock);