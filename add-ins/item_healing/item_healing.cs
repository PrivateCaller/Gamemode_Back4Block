%pattern = "add-ons/gamemode_left4block/add-ins/item_healing/sound/*.wav";
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/gamemode_left4block/add-ins/item_healing/sound/", "");
	%soundName = strreplace(%soundName, "/", "");
	%soundName = strreplace(%soundName, ".wav", "");
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosest3d; filename = \"" @ %file @ "\"; };");

	%file = findNextFile(%pattern);
}


datablock ParticleData(HealParticle)
{
		dragCoefficient      = 5;
		gravityCoefficient   = -0.2;
		inheritedVelFactor   = 0;
		constantAcceleration = 0;
		lifetimeMS           = 1000;
		lifetimeVarianceMS   = 500;
		useInvAlpha          = false;
		textureName          = "./icons/heal";
		colors[0]     = "1 1 1 1";
		colors[1]     = "1 1 1 0.5";
		colors[2]     = "0 0 0 0";
		sizes[0]      = 0.4;
		sizes[1]      = 0.6;
		sizes[2]      = 0.4;
		times[0]      = 0;
		times[1]      = 0.2;
		times[2]      = 1;
};

datablock ParticleEmitterData(HealEmitter)
{
		ejectionPeriodMS = 35;
		periodVarianceMS = 0;
		ejectionVelocity = 0.5;
		ejectionOffset   = 1;
		velocityVariance = 0.49;
		thetaMin         = 0;
		thetaMax         = 120;
		phiReferenceVel  = 0;
		phiVariance      = 360;
		overrideAdvance = false;
		particles = "HealParticle";

		uiName = "";
};

datablock ShapeBaseImageData(HealImage)
{
		shapeFile = "base/data/shapes/empty.dts";
		emap = false;

		mountPoint = $HeadSlot;

		stateName[0]			= "Ready";
		stateTransitionOnTimeout[0]	= "FireA";
		stateTimeoutValue[0]		= 0.01;

		stateName[1]			= "FireA";
		stateTransitionOnTimeout[1]	= "Done";
		stateWaitForTimeout[1]		= True;
		stateTimeoutValue[1]		= 0.350;
		stateEmitter[1]			= HealEmitter;
		stateEmitterTime[1]		= 0.350;

		stateName[2]			= "Done";
		stateScript[2]			= "onDone";
};

function HealImage::onDone(%data, %player, %slot)
{
	%player.unMountImage(%slot);
}



datablock DebrisData(PillsHereDebris)
{
		shapeFile = "./models/cap.2.dts";
		lifetime = 5.0;
		minSpinSpeed = -400.0;
		maxSpinSpeed = 200.0;
		elasticity = 0.5;
		friction = 0.2;
		numBounces = 3;
		staticOnMaxBounce = true;
		snapOnMaxBounce = false;
		fade = true;
		gravModifier = 2;
};

datablock ItemData(PillsHereItem)
{
		category = "Weapon";
		className = "Weapon";
		
		shapeFile = "./models/pills.4.dts";
		rotate = false;
		mass = 1;
		density = 0.2;
		elasticity = 0.2;
		friction = 0.6;
		emap = true;
		
		uiName = "Pills";
		iconName = "./icons/Icon_Pills";
		doColorShift = true;
		colorShiftColor = "0.8 0.8 0.8 1";
		
		image = PillsHereImage;
		canDrop = true;
};

datablock ShapeBaseImageData(PillsHereImage)
{
		shapeFile = "./models/pills.4.dts";
		emap = true;
		mountPoint = 0;
		offset = "0 0 0";
		eyeOffset = 0;
		rotation = eulerToMatrix( "0 0 0" );
		
		className = "WeaponImage";
		item = PillsHereItem;
		isHealing = 1;
		
		armReady = true;
		
		doColorShift = PillsHereItem.doColorShift;
		colorShiftColor = PillsHereItem.colorShiftColor;
		
		casing = PillsHereDebris;
		shellExitDir		= "1.0 1.0 1.0";
		shellExitOffset		= "0 0 0";
		shellExitVariance	= 5;	
		shellVelocity		= 10;
		
		stateName[0]					= "Activate";
		stateScript[0]					= "onActivate";
		stateTimeoutValue[0]			= 0.15;
		stateTransitionOnTimeout[0]		= "Ready";

		stateName[1]					= "Ready";
		stateAllowImageChange[1]		= true;
		stateSequence[1]				= "Ready";
		stateTransitionOnTriggerDown[1]	= "Use";
		
		stateName[2]					= "Use";
		stateScript[2]					= "onUse";
		stateTransitionOnTriggerUp[2]	= "Done";
		
		stateName[3]					= "Done";
		stateTransitionOnAmmo[3]		= "Ready";
		stateTransitionOnNoAmmo[3]		= "UnUse";
		
		stateName[4]					= "UnUse";
		stateEjectShell[4]				= true;
		stateScript[4]					= "onUnUse";
		stateSequence[4]				= "Open";
		stateTimeoutValue[4]			= 0.5;
		stateTransitionOnTimeout[4]		= "Hack";
		stateWaitForTimeout[4]			= true;
		
		stateName[5]					= "Hack";
		stateScript[5]					= "onHack";
};

function PillsHereImage::onActivate(%this,%obj,%db)
{
	serverplay3d("heal_pills_deploy" @ getRandom(1,3) @ "_sound", %obj.getPosition());
}

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





datablock ItemData(GauzeItem)
{
		category = "Weapon";
		className = "Weapon";
		
		shapeFile = "./models/gauze.dts";
		rotate = false;
		mass = 1;
		density = 0.2;
		elasticity = 0.2;
		friction = 0.6;
		emap = true;
		
		uiName = "Gauze";
		iconName = "./icons/Icon_Gauze";
		doColorShift = false;
		
		image = GauzeImage;
		canDrop = true;
		l4ditemtype = "heal_full";
};


datablock ShapeBaseImageData(GauzeImage)
{
		shapeFile = "./models/gauze.dts";
		emap = true;
		mountPoint = 0;
		offset = "0 0 0";
		eyeOffset = 0;
		rotation = eulerToMatrix("0 0 0");
		
		className = "WeaponImage";
		item = GauzeItem;
		isHealing = 1;
		
		armReady = true;
		doColorShift = false;
		
		stateName[0]					= "Activate";
		stateSound[0]					= "heal_stop_sound";
		stateTimeoutValue[0]			= 0.15;
		stateSequence[0]				= "Ready";
		stateTransitionOnTimeout[0]		= "Ready";

		stateName[1]					= "Ready";
		stateAllowImageChange[1]		= true;
		stateScript[1]					= "onReady";
		stateTransitionOnTriggerDown[1]	= "Use";
		
		stateName[2]					= "Use";
		stateScript[2]					= "onUse";
		stateTransitionOnTriggerUp[2]	= "Ready";
};


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


datablock ItemData(SyringeAntidoteItem)
{
	uiName = "Syringe Antidote";
	iconName = "./icons/icon_syringered";
	image = SyringeAntidoteImage;
	category = Weapon;
	className = Weapon;
	shapeFile = "./models/syringered.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0;
	friction = 0.6;
	emap = true;
	doColorShift = true;
	colorShiftColor = "1 1 1 1";
	canDrop = true;
	gc_syringe = 1;
};

datablock shapeBaseImageData(SyringeAntidoteImage)
{
	shapeFile = "./models/syringered.dts";
	emap = true;
	correctMuzzleVector = false;
	isHealing = 1;
	className = "WeaponImage";
	item = SyringeAntidoteItem;
	ammo = "";
	projectile = "";
	projectileType = Projectile;
	melee = false;
	doReaction = false;
	armReady = true;
	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	stateName[0] = "Activate";
	stateTimeoutValue[0] = 0.2;
	stateTransitionOnTimeout[0] = "Ready";
	stateSound[0] = "heal_syringe_pickup_sound";

	stateName[1] = "Ready";
	stateTransitionOnTriggerDown[1] = "Fire";
	stateAllowImageChange[1] = true;

	stateName[2] = "Fire";
	stateTransitionOnTimeout[2] = "Ready";
	stateTimeoutValue[2] = 0.2;
	stateFire[2] = true;
	stateScript[2] = "onFire";
	stateWaitForTimeout[2] = true;
	stateAllowImageChange[2] = false;
};

function SyringeAntidoteImage::onFire(%this,%obj,%slot)
{
	SyringeAntidoteImage::onSelfUse(%this,%obj,%slot);
}

function SyringeAntidoteImage::onSelfUse(%this,%obj,%slot)
{
	%client = %obj.client;
	
	if(%obj.getDamageLevel() < 5 && %obj.getDatablock().getName() !$= "DownPlayerSurvivorArmor")
	{
		if(isObject(%client))
		commandToClient(%client, 'centerPrint', "\c5You are not injured.", 1);
  	}
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
	
		if(%obj.getenergylevel() < 100 && %obj.getDatablock().getName() $= "DownPlayerSurvivorArmor")
		%obj.setenergylevel(%obj.getenergylevel()/0.65);
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