forceRequiredAddOn("Projectile_Radio_Wave");
exec("./datablocks.cs");

function PillsHereImage::onActivate(%this,%obj,%db) { serverplay3d("heal_pills_deploy" @ getRandom(1,3) @ "_sound", %obj.getPosition()); }

function PillsHereImage::onHack(%this, %obj, %slot)
{
	%obj.playThread(1, "root");
	%obj.schedule(32, "unMountImage", %slot);
	
	if(isObject(%client = %obj.client))
	serverCmdUnUseTool(%client);
}

function PillsHereImage::onUnUse(%this, %obj, %slot)
{
	%tool = %obj.tool[%obj.currTool];
	
	if(isObject(%tool) && %tool.getID() == %this.item.getID())
	{
		%armor = %obj.getDatablock();
		%damage = %obj.getDamageLevel();
		%maxDamage = %armor.maxDamage;
		%heal = %maxDamage / 2;

		%obj.playAudio(1,"heal_pills_pop_sound");
    	%obj.setenergylevel(%obj.getenergylevel()/0.65);

		%obj.setDamageLevel(%obj.getDamageLevel()/1.3);
		%obj.emote(HealImage, 1);
		
		%obj.playThread(2, "shiftAway");
		%obj.setWhiteOut((%maxDamage - %damage) / %maxDamage);
		
		if(isObject(%client = %obj.client))
		messageClient(%client, 'MsgItemPickup', '', %obj.currTool, 0);
		
		%obj.tool[%obj.currTool] = 0;
		%obj.weaponCount--;
		%obj.currTool = -1;
	}
}

function PillsHereImage::onUse(%this, %obj, %slot)
{
	if(%obj.getDamageLevel() > 5 || %obj.getDatablock().getName() $= "DownPlayerSurvivorArmor")
	%obj.setImageAmmo(%slot, false);
	else
	{
		%obj.setImageAmmo(%slot, true);
		
		%client = %obj.client;
		if(isObject(%client))
		commandToClient(%client, 'centerPrint', "\c5You are not injured.", 2);
	}
}

function GauzeImage::onActivate(%this,%obj)
{
	%obj.playThread(0,plant);
}
function GauzeImage::healLoop(%this, %obj)
{
	%bandageSlot = %obj.currTool;
	%client = %obj.client;
	%tool = %obj.tool[%bandageSlot];
	
	if(isObject(%tool) && %tool.getID() == %this.item.getID())
	{
		%time = 2.4;
		%obj.GauzeUse += 0.1;
		
		if(%obj.GauzeUse >= %time)
		{
				%obj.pseudoHealth = 0;
				%obj.setDamageLevel(%obj.getDamageLevel()/3.25);
				%obj.emote(HealImage, 1);
				%obj.tool[%bandageSlot] = 0;
				%obj.weaponCount--;
				%obj.setMaxForwardSpeed(%obj.getDatablock().maxForwardSpeed);
				
				if(isObject(%client))
				{
					messageClient(%client, 'MsgItemPickup', '', %bandageSlot, 0);
					%client.setControlObject(%obj);
				}
				
				%obj.unMountImage(%slot);
				%obj.playThread(0, "root");
				%obj.playThread(1, "root");
				
				%client.Gauzing = false;
				%obj.GauzeUse = false;
				cancel(%obj.GauzeSched);
		}
		else
		{
			if((%obj.GauzeUse * 10) % 10 == 0)
			{
				if(getRandom(0, 1)) %obj.playThread(0, "activate");
				
				else %obj.playThread(3, "activate2");
			}
			
			if(isObject(%client))
			{
				%bars = "<color:ffaaaa>";
				%div = 20;
				%tens = mFloor((%obj.GauzeUse / %time) * %div);
				
				for(%a = 0; %a < %div; %a++)
				{
					if(%a == (%div - %tens)) %bars = %bars @ "<color:aaffaa>";
					
					%bars = %bars @ "|";
				}
				
				commandToClient(%client, 'centerPrint', %bars, 0.25);
			}
			cancel(%obj.GauzeSched);
			%obj.GauzeSched = %this.schedule(100, "healLoop", %obj);
			
		}
	}
	else
	{
		if(isObject(%client))
		{
			commandToClient(%client, 'centerPrint', "<color:ffaaaa>Heal Aborted!", 1);
			%client.setControlObject(%obj);
			%client.player.playAudio(1,"heal_stop_sound");
			%obj.setMaxForwardSpeed(%obj.getDatablock().maxForwardSpeed);
		}
		cancel(%obj.GauzeSched);
	}
}

function GauzeImage::onMount(%this, %obj, %slot)
{
	parent::onMount(%this, %obj, %slot);
	
	%obj.GauzeUse = 0;
}

function GauzeImage::onReady(%this, %obj, %slot)
{
	%obj.GauzeUse = 0;
}

function GauzeImage::onUnMount(%this, %obj, %slot)
{
	%obj.playThread(0, "root");
	parent::onUnMount(%this, %obj, %slot);
}

function GauzeImage::onUse(%this, %obj, %slot)
{
	%client = %obj.client;
	
	if(%obj.getDamageLevel() < 5 || %obj.getDatablock().getName() $= "DownPlayerSurvivorArmor")
	{
		if(isObject(%client))
		{
			if(%obj.getDatablock().getName() $= "DownPlayerSurvivorArmor")
			commandToClient(%client, 'centerPrint', "\c5Cannot use while down.", 1);
			else commandToClient(%client, 'centerPrint', "\c5You are not injured.", 1);
		}
	}
	else
	{
		if(isObject(%client))
		{
			%client.Gauzing = true;
			%client.zombieMedpackHelpTime = $sim::time;
		}

		%client.player.playAudio(1,"heal_gauze_bandaging_sound");
		%this.healLoop(%obj);
	}
}

function SyringeAntidoteImage::onFire(%this,%obj,%slot)
{
	if(isObject(%client = %obj.client) && %obj.getDamageLevel() < 5) commandToClient(%client, 'centerPrint', "\c5You are not injured.", 1);
  	else
  	{
  	    %obj.playThread(3, plant);
  	    cancel(%obj.gc_poisoning2);
  	    %obj.setDamageFlash(0);
  	    %obj.setDamageLevel(%obj.getDamageLevel()/1.7);
  	    %obj.emote(HealImage, 1);
  	    %obj.playaudio(2, "heal_syringe_stab_sound");
  	    %currSlot = %obj.currTool;
  	    %obj.tool[%currSlot] = 0;
  	    %obj.weaponCount--;
  	    messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
  	    serverCmdUnUseTool(%obj.client);
	
		if(%obj.getenergylevel() < 100 && %obj.getDatablock().isDowned) %obj.setenergylevel(%obj.getenergylevel()/0.65);
  	}
}
function GauzeImage::onTrigger(%this, %obj, %trigger, %state)
{
	if(isObject(%client = %obj.getControllingClient()) && %client.Gauzing)
	{
		if(%trigger == 0 && !%state)
		{
			%client.player.GauzeUse = 0;
			%client.Gauzing = false;
			cancel(%client.player.GauzeSched);
			%client.setControlObject(%client.player);
			commandToClient(%client, 'centerPrint', "<color:ffaaaa>Heal Aborted!", 1);
			%obj.setMaxForwardSpeed(%obj.getDatablock().maxForwardSpeed);
			%client.player.playAudio(1,"heal_stop_sound");
		}
	}
	
	return parent::onTrigger(%this, %obj, %trigger, %state);
}

function ZombieMedpackImage::onTrigger(%this, %obj, %trigger, %state)
{
	if(isObject(%client = %obj.getControllingClient()) && %client.zombieMedpackting)
	{
		if(%trigger == 0 && !%state)
		{
			%obj.playThread(2, "armReadyBoth");
			%client.player.zombieMedpackUse = 0;
			%client.zombieMedpackting = false;
			cancel(%client.player.zombieMedpackSched);
			%client.setControlObject(%client.player);
			commandToClient(%client, 'centerPrint', "<color:ffaaaa>Heal Aborted!", 1);
			%client.player.playAudio(1,"heal_stop_sound");
			%obj.setMaxForwardSpeed(%obj.getDatablock().maxForwardSpeed);
		}
	}
	
	return parent::onTrigger(%this, %obj, %trigger, %state);
}

function DefibrillatorImage::onFire(%data, %obj, %slot)
{
	parent::onFire(%data, %obj, %slot);

	%obj.playThread(0, plant);
	%obj.playThread(2, shiftDown);
}

function LeftHandDefibrillatorImage::onFire(%data, %obj, %slot)
{
	parent::onFire(%data, %obj, %slot);
}

function DefibrillatorImage::onMount(%data, %obj, %slot)
{
	parent::onMount(%data, %obj, %slot);

	%obj.mountImage(LeftHandDefibrillatorImage, 1);
	%obj.playThread(1, armReadyBoth);
}

function DefibrillatorImage::onUnMount(%data, %obj, %slot)
{
	parent::onUnMount(%data, %obj, %slot);

	if(isObject(%obj.getMountedImage(LeftHandDefibrillatorImage.mountPoint)) && %obj.getMountedImage(LeftHandDefibrillatorImage.mountPoint).getName() $= "LeftHandDefibrillatorImage")
	%obj.unMountImage(LeftHandDefibrillatorImage.mountPoint);
	
}

function LeftHandDefibrillatorImage::onUnMount(%data, %obj, %slot)
{
	parent::onUnMount(%data, %obj, %slot);
}

function DefibrillatorProjectile::onCollision(%data, %proj, %col, %fade, %pos, %norm)
{
	parent::onCollision(%data, %proj, %col, %fade, %pos, %norm);

	%client = %proj.client;

	if(!isObject(%client) && isObject(%proj.sourceObject))
	{
		if(%proj.sourceObject.getClassName() $= "Player") %client = %proj.sourceObject.client;
		else if(%proj.sourceObject.getClassName() $= "gameConnection") %client = %proj.sourceObject;		
	}

	if(!isObject(%client) || !isObject(%client.player)) return;
	

	initContainerRadiusSearch(%pos, 0.1, $TypeMasks::CorpseObjectType);

	if(!isObject(%body = containerSearchNext()) || %body.getClassName() !$= "Player" || !isObject(%bodyClient = %body.prevClient) || isObject(%client.slyrTeam) || isObject(%bodyClient.slyrTeam) && %client.slyrTeam != %bodyClient.slyrTeam)
	return;	

	%count = %body.getDataBlock().maxTools;
	for(%i = 0; %i < %count; %i++) %tool[%i] = %body.tool[%i];
	

	%bodyCurrTool = %body.currTool;
	%bodyData = %body.getDataBlock();
	%bodyPos = %body.getTransform();
	%bodyClient.spawnPlayer(); //The body will disppear here, so we need to save some info before we do this.
	%bodyClient.player.setDataBlock(%bodyData);
	%bodyClient.player.setTransform(%bodyPos);
	%bodyClient.player.emote(HealImage, 1);

	%bodyClient.player.clearTools();
	%count = %bodyClient.player.getDataBlock().maxTools;

	for(%i = 0; %i < %count; %i++)
	{
		%bodyClient.player.tool[%i] = %tool[%i];
		messageClient(%bodyClient, 'MsgItemPickup', "", %i, %tool[%i], 1);

		if(isObject(%tool[%i]) && %tool[%i].className $= "Weapon") %bodyClient.player.weaponCount++;
	}

	%bodyClient.player.currTool = %bodyCurrTool;
	%bodyClient.player.updateArm(%bodyClient.player.tool[%bodyCurrTool].image);
	%bodyClient.player.mountImage(%bodyClient.player.tool[%bodyCurrTool].image, %bodyClient.player.tool[%bodyCurrTool].image.mountPoint);
	schedule(500, %client.player, eval, %client.player @ ".tool[" @ %client.player.currTool @ "] = 0;");
	schedule(500, %client.player, messageClient, %client, 'MsgItemPickup', "", %client.player.currTool, 0, 1);
	schedule(500, %client.player, serverCmdUnUseTool, %client);
	
}

function DefibrillatorProjectile::damage(%data, %player, %col, %fade, %pos, %norm)
{
	Parent::damage(%data, %player, %col, %fade, %pos, %norm);
}

package Defibrillator
{
	function player::removeBody(%player)
	{
		if(%player.getClassName() $= "AIPlayer" || $ConfirmRemoveBody) //Using a global variable for this because we can't rely on others who package this function to parent an extra variable for no reason.
		{
			$ConfirmRemoveBody = false;
			Parent::removeBody(%player);
		}
	}

	function gameConnection::onDeath(%client, %sourcePlayer, %sourceClient, %damageType, %damageArea)
	{
		%client.corpse = %client.player;
		%client.player.prevClient = %client;

		parent::onDeath(%client, %sourcePlayer, %sourceClient, %damageType, %damageArea);

		if(isObject(%client.minigame))
		{
			if(isObject(%client.slyrTeam) && %client.slyrTeam.respawnTime_player >= 0)
			{
				%time = %client.slyrTeam.respawnTime_player < 1 ? 1 : %client.slyrTeam.respawnTime_player;
				%time *= 1000;
			}

			else
			{
				%time = %client.minigame.respawnTime_player < 1 ? 1 : %client.minigame.respawnTime_player;
				%time *= 1000;
			}
		}

		else
		{
			%time = $Game::MinRespawnTime;
		}
	}

	function MiniGameSO::removeMember(%mini, %client)
	{
		if(isObject(%client) && %mini.isMember(%client) && isObject(%client.corpse))
		{
			$ConfirmRemoveBody = true;
			%client.corpse.removeBody();
		}

		parent::removeMember(%mini, %client);
	}

	function gameConnection::spawnPlayer(%client)
	{
		if(isObject(%client.corpse))
		{
			%client.corpse.delete();
		}

		parent::spawnPlayer(%client);
	}

	function gameConnection::onClientLeaveGame(%client)
	{
		if(isObject(%client.corpse))
		{
			$ConfirmRemoveBody = true;
			%client.corpse.removeBody();
		}

		parent::onClientLeaveGame(%client);
	}
};
activatePackage(Defibrillator);	