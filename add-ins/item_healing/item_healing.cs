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

if(!isObject(HealParticle))
{
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
}

if(!isObject(HealEmitter))
{
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
}

if(!isObject(HealImage))
{
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
}

function HealImage::onDone(%data, %player, %slot)
{
	%player.unMountImage(%slot);
}

if(!isObject(ZombieMedpackItem))
{
	datablock ItemData(ZombieMedpackItem)
	{
		category = "Weapon";
		className = "Weapon";
		
		shapeFile = "./models/medpack.1.dts";
		rotate = false;
		mass = 1;
		density = 0.2;
		elasticity = 0.2;
		friction = 0.6;
		emap = true;
		
		uiName = "Medpack";
		iconName = "./icons/Icon_Medpack";
		doColorShift = false;
		
		image = ZombieMedpackImage;
		canDrop = true;
		l4ditemtype = "heal_full";
	};
}

if(!isObject(ZombieMedpackImage))
{
	datablock ShapeBaseImageData(ZombieMedpackImage)
	{
		shapeFile = "./models/medpack.1.dts";
		emap = true;
		mountPoint = 0;
		offset = "0 0 0";
		eyeOffset = 0;
		rotation = eulerToMatrix("0 0 0");
		
		className = "WeaponImage";
		item = ZombieMedpackItem;
		isHealing = 1;
		
		armReady = true;
		doColorShift = false;
		
		stateName[0]					= "Activate";
		stateSound[0]					= "heal_medpack_unholster_sound";
		stateScript[0]					= "onActivate";
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
}

function ZombieMedpackImage::onActivate(%this,%obj)
{
	%obj.playThread(3,plant);
}
function ZombieMedpackImage::healLoop(%this, %obj)
{
	%bandageSlot = %obj.currTool;
	%client = %obj.client;
	%tool = %obj.tool[%bandageSlot];
	
	if(isObject(%tool) && %tool.getID() == %this.item.getID())
	{
		%time = 3.4;
		%obj.zombieMedpackUse += 0.1;
		
		if(%obj.zombieMedpackUse >= %time)
		{
				%obj.pseudoHealth = 0;
				%obj.setDamageLevel(0);
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
				%obj.playThread(2, "root");
				%obj.playThread(1, "root");

				%client.zombieMedpackting = false;
				%obj.zombieMedpackUse = false;
				cancel(%obj.zombieMedpackSched);
		}
		else
		{
			if((%obj.zombieMedpackUse * 10) % 10 == 0)
			{
				if(getRandom(0, 2))
				{
					%obj.playThread(3, "shiftright");
				}
				else
				{
					%obj.playThread(3, "activate2");
				}
			}
			
			if(isObject(%client))
			{
				%bars = "<color:ffaaaa>";
				%div = 20;
				%tens = mFloor((%obj.zombieMedpackUse / %time) * %div);
				
				for(%a = 0; %a < %div; %a++)
				{
					if(%a == (%div - %tens))
					{
						%bars = %bars @ "<color:aaffaa>";
					}
					
					%bars = %bars @ "|";
				}
				
				commandToClient(%client, 'centerPrint', %bars, 0.25);
			}
			cancel(%obj.zombieMedpackSched);
			%obj.zombieMedpackSched = %this.schedule(100, "healLoop", %obj);
			
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
		cancel(%obj.zombieMedpackSched);
	}
}

function ZombieMedpackImage::onReady(%this, %obj, %slot)
{
	%obj.zombieMedpackUse = 0;
}

function ZombieMedpackImage::onUse(%this, %obj, %slot)
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
			%client.zombieMedpackting = true;
			%client.player.playAudio(1,"heal_medpack_bandaging_sound");
			%this.healLoop(%obj);
			%obj.setMaxForwardSpeed(%obj.getDatablock().maxForwardSpeed/4);
		}
	}
}

function ZombieMedpackImage::onMount(%this, %obj, %slot)
{
	parent::onMount(%this, %obj, %slot);
	
	%obj.zombieMedpackUse = 0;
	%obj.playThread(2, "armReadyBoth");
}

function ZombieMedpackImage::onUnMount(%this, %obj, %slot)
{
	%obj.playThread(2, "root");
}

function ZombieMedpackImage::onReady(%this, %obj, %slot)
{
	%obj.zombieMedpackUse = 0;
}



if(!isObject(ZombiePillsDebris))
{
	datablock DebrisData(ZombiePillsDebris)
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
}

if(!isObject(ZombiePillsItem))
{
	datablock ItemData(ZombiePillsItem)
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
		
		image = ZombiePillsImage;
		canDrop = true;
	};
}

if(!isObject(ZombiePillsImage))
{
	datablock ShapeBaseImageData(ZombiePillsImage)
	{
		shapeFile = "./models/pills.4.dts";
		emap = true;
		mountPoint = 0;
		offset = "0 0 0";
		eyeOffset = 0;
		rotation = eulerToMatrix( "0 0 0" );
		
		className = "WeaponImage";
		item = ZombiePillsItem;
		isHealing = 1;
		
		armReady = true;
		
		doColorShift = ZombiePillsItem.doColorShift;
		colorShiftColor = ZombiePillsItem.colorShiftColor;
		
		casing = ZombiePillsDebris;
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
}

function ZombiePillsImage::onActivate(%this,%obj,%db)
{
	serverplay3d("heal_pills_deploy" @ getRandom(1,3) @ "_sound", %obj.getPosition());
}

function ZombiePillsImage::onHack(%this, %obj, %slot)
{
	%obj.playThread(1, "root");
	%obj.schedule(32, "unMountImage", %slot);
	
	if(isObject(%client = %obj.client))
	serverCmdUnUseTool(%client);
}

function ZombiePillsImage::onUnUse(%this, %obj, %slot)
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

function ZombiePillsImage::onUse(%this, %obj, %slot)
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



datablock ItemData(RedPotionItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/redpotion.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Red Potion";
	iconName = "./icons/Icon_RedPotion";
	doColorShift = false;

	image = RedPotionImage;
	canDrop = true;
};

datablock ShapeBaseImageData(RedPotionImage)
{
	shapeFile = "./models/redpotion.dts";
	emap = true;

	mountPoint = 0;
	offset = "-0.05 0.08 0.1";
	eyeOffset = 0;
	rotation = "0 0 0 1";

	className = "WeaponImage";
	item = RedPotionItem;
	isHealing = 1;

	armReady = true;

	doColorShift = false;

	stateName[0]			= "Ready";
	stateTransitionOnTriggerDown[0]	= "Fire";
	stateAllowImageChange[0]	= true;

	stateName[1]			= "Fire";
	stateScript[1]			= "onFire";
};

function RedPotionImage::onFire(%data, %obj, %slot)
{
	%client = %obj.client;
	
	if(%obj.getDamageLevel() < 5 && %obj.getDatablock().getName() !$= "DownPlayerSurvivorArmor")
	{
		if(isObject(%client))
		commandToClient(%client, 'centerPrint', "\c5You are not injured.", 1);
	}
	else
	{
		cancel(%obj.gc_poisoning2);
		%obj.setDamageFlash(0);
		%obj.setDamageLevel(%obj.getDamageLevel()/1.5);
		%obj.playthread(3, plant);
		%obj.emote(HealImage, 1);
		%obj.playaudio(2, "heal_potion_drink_sound");
		%currSlot = %obj.currTool;
		%obj.tool[%currSlot] = 0;
		%obj.weaponCount--;
		messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
		serverCmdUnUseTool(%obj.client);
      
		if(%obj.getenergylevel() < 100 && %obj.getDatablock().getName() $= "DownPlayerSurvivorArmor")
		%obj.setenergylevel(%obj.getenergylevel()/0.8);
	}
}



if(!isObject(GauzeItem))
{
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
}

if(!isObject(GauzeImage))
{
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
				if(getRandom(0, 1))
				{
					%obj.playThread(0, "activate");
				}
				else
				{
					%obj.playThread(3, "activate2");
				}
			}
			
			if(isObject(%client))
			{
				%bars = "<color:ffaaaa>";
				%div = 20;
				%tens = mFloor((%obj.GauzeUse / %time) * %div);
				
				for(%a = 0; %a < %div; %a++)
				{
					if(%a == (%div - %tens))
					{
						%bars = %bars @ "<color:aaffaa>";
					}
					
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
		%obj.setMaxForwardSpeed(%obj.getDatablock().maxForwardSpeed/2);
	}
}



datablock ItemData(coughsyrupItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/coughsyrup.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Cough Syrup";
	iconName = "./icons/Icon_coughsyrup";
	doColorShift = false;

	image = coughsyrupImage;
	canDrop = true;
};

datablock ShapeBaseImageData(coughsyrupImage)
{
	shapeFile = "./models/coughsyrup.dts";
	emap = true;

	mountPoint = 0;
	offset = "-0.05 0.08 0.1";
	eyeOffset = 0;
	rotation = "0 0 0 1";

	className = "WeaponImage";
	isHealing = 1;
	item = coughsyrupItem;

	armReady = true;

	doColorShift = false;

	stateName[0]			= "Ready";
	stateTransitionOnTriggerDown[0]	= "Fire";
	stateAllowImageChange[0]	= true;

	stateName[1]			= "Fire";
	stateScript[1]			= "onFire";
};

function coughsyrupImage::onFire(%data, %obj, %slot)
{
	%client = %obj.client;
	
	if(%obj.getDamageLevel() < 5 && %obj.getDatablock().getName() !$= "DownPlayerSurvivorArmor")
	{
		if(isObject(%client))
		commandToClient(%client, 'centerPrint', "\c5You are not injured.", 1);
	}
	else
	{
		cancel(%obj.gc_poisoning2);
		%obj.setDamageFlash(0);
		%obj.setDamageLevel(%obj.getDamageLevel()/1.675);
		%obj.playthread(3, plant);
		%obj.emote(HealImage, 1);
		%obj.playaudio(2, "heal_potion_drink_sound");
		%currSlot = %obj.currTool;
		%obj.tool[%currSlot] = 0;
		%obj.weaponCount--;
		messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
		serverCmdUnUseTool(%obj.client);
      	%obj.setenergylevel(%obj.getenergylevel()/0.25);
	}
}

datablock ItemData(gc_SyringePanaceaItem)
{
  uiName = "Syringe Panacea";
  iconName = "./icons/Icon_SyringeOrange";
  image = gc_SyringePanaceaImage;
  category = Weapon;
  className = Weapon;
  shapeFile = "./models/SyringeOrange.dts";
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

datablock shapeBaseImageData(gc_SyringePanaceaImage)
{
  	shapeFile = "./models/SyringeOrange.dts";
  	emap = true;
  	correctMuzzleVector = false;
  	isHealing = 1;
  	className = "WeaponImage";
  	item = gc_SyringePanaceaItem;
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

function gc_SyringePanaceaImage::onFire(%this,%obj,%slot)
{
	gc_SyringePanaceaImage::onSelfUse(%this,%obj,%slot);
}

function gc_SyringePanaceaImage::onSelfUse(%this,%obj,%slot)
{
    if(%obj.hIsInfected)
    return;

	%client = %obj.client;
	
	if(!%obj.hIsImmune)
	{
        %obj.hIsImmune = 1;
        if(%obj.getClassName() $= "Player")
        {
          %client.Play2d("survivor_immunity_sound");
          GameConnection::ChatMessage (%obj.client, "\c2Immunity gained!");
		  commandToClient(%obj.client,'SetVignette',$EnvGuiServer::VignetteMultiply,$EnvGuiServer::VignetteColor);
		}
	}

	%obj.playThread(3, plant);
	cancel(%obj.gc_poisoning2);
	%obj.setDamageFlash(0);
	%obj.setDamageLevel(%obj.getDamageLevel()/2.15);
	%obj.emote(HealImage, 1);
	%obj.playaudio(2, "heal_syringe_stab_sound");
	%currSlot = %obj.currTool;
	%obj.tool[%currSlot] = 0;
	%obj.weaponCount--;
	messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
	serverCmdUnUseTool(%obj.client);
	%obj.setenergylevel(%obj.getenergylevel()/0.35);

	if(%obj.getDatablock().getName() $= "DownPlayerSurvivorArmor")
	{
		centerprintcounter(%obj,%obj.savetimer);
		%obj.isdowned = 0;
		%obj.SetDataBlock("SurvivorPlayerLow");
		SurvivorPlayer_HeartBeat(%obj.client,1);
		%obj.lastdamage = getsimtime();
		%obj.sethealth(25);

		%obj.playthread(0,root);
		%obj.client.centerprint("<color:00fa00>You were saved by yourself",5);
		//chatMessageTeam(%target.client,'fakedeathmessage',"<color:00fa00>" @ %this.client.name SPC "<bitmap:add-ons/player_Survivor/Reviver><bitmap:add-ons/player_Survivor/Revived>" SPC %target.client.name);
		%obj.savetimer = 0;
		cancel(%obj.energydeath1);
		cancel(%obj.energydeath2);
		cancel(%obj.energydeath3);
	}
}

datablock ItemData(gc_SyringeAntidoteItem)
{
	uiName = "Syringe Antidote";
	iconName = "./icons/icon_syringered";
	image = gc_SyringeAntidoteImage;
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

datablock shapeBaseImageData(gc_SyringeAntidoteImage)
{
	shapeFile = "./models/syringered.dts";
	emap = true;
	correctMuzzleVector = false;
	isHealing = 1;
	className = "WeaponImage";
	item = gc_SyringeAntidoteItem;
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

function gc_SyringeAntidoteImage::onFire(%this,%obj,%slot)
{
	gc_SyringeAntidoteImage::onSelfUse(%this,%obj,%slot);
}

function gc_SyringeAntidoteImage::onSelfUse(%this,%obj,%slot)
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