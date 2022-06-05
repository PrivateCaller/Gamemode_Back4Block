//AddDamageType ("Default", '<bitmap:base/client/ui/ci/skull> %1', '%2 <bitmap:base/client/ui/ci/skull> %1!', 1, 0);

datablock ItemData(crowbarItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/crowbar.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	uiName = "Crowbar";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_crowbar";
	doColorShift = true;
	colorShiftColor = "0.5 0.5 0.5 1";
	image = crowbarImage;
	canDrop = true;
};

datablock ShapeBaseImageData(crowbarImage)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/crowbar.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0.05 0.25";
	correctMuzzleVector = false;
	eyeOffset = "0 0 0";
	className = "WeaponImage";
	item = crowbarItem;
	ammo = " ";
	projectile = "";
	projectileType = Projectile;
	melee = true;
	hasLunge = true;
	doRetraction = false;
	armReady = false;
	doColorShift = crowbarItem.doColorShift;
	colorShiftColor = crowbarItem.colorShiftColor;

	meleeDamage = 32;
	meleeHitEnvSound = "crowbar";
	meleeHitPlSound = "crowbar";
	meleeDistanceMin = 0.8;
	meleeZombieStunChance = 25;

	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0;
	stateTransitionOnTimeout[0]      = "Ready";
	stateSound[0]                    = WeaponSwitchsound;

	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "PreFire";
	stateAllowImageChange[1]         = true;

	stateName[2]			= "PreFire";
	stateScript[2]                  = "onPreFire";
	stateAllowImageChange[2]        = false;
	stateTimeoutValue[2]            = 0.128045;
	stateTransitionOnTimeout[2]     = "Fire";

	stateName[3]                    = "Fire";
	stateTransitionOnTimeout[3]     = "CheckFire";
	stateFire[3]                    = false;
	stateAllowImageChange[3]        = false;
	stateSequence[3]                = "Fire";
	stateScript[3]                  = "onFire";
	stateTimeoutValue[3]            = 0.21;
	stateSound[3]					= "melee_swing_sound";

	stateName[4]			= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "PreFire";

	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Ready";
	stateTimeoutValue[5]            = 0.3;
	stateAllowImageChange[5]        = false;
	stateSequence[5]                = "StopFire";
	stateScript[5]                  = "onStopFire";
};

function MeleeSwingCheck(%obj,%this,%slot)
{   
	%pos = %obj.getMuzzlePoint(%slot);
	%radius = 2;
	%searchMasks = $TypeMasks::StaticObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	InitContainerRadiusSearch(%pos, %radius, %searchMasks);
	while (%target = containerSearchNext())
   	{
      	if(%target == %obj)
      	continue;
	
      	%len = 2 * getWord(%obj.getScale (), 2);
      	%vec = %obj.getMuzzleVector(%slot);
      	%beam = vectorScale(%vec,%len);
      	%end = vectorAdd(%pos,%beam);
      	%ray = containerRayCast(%pos,%end,%searchMasks,%obj); //fire raycast
      	//%line = vectorNormalize(vectorSub(%pos,posFromRaycast(%ray)));
		//%dot = vectorDot(%vec,%line);

     	if(vectorDist(%pos,posFromRaycast(%ray)) > %this.meleeDistanceMin)
		continue;

     	if(%ray.getType() & $TypeMasks::FxBrickObjectType || %ray.getType() & $TypeMasks::StaticObjectType)
     	{
			%obj.playthread(2, "meleeHitFail");
			serverPlay3D(%this.meleeHitEnvSound @ "_hitenv" @ getRandom(1,2) @ "_sound",posFromRaycast(%ray));

			%p = new projectile()
			{
				datablock = "SecondaryMeleeSmallProjectile";
				initialPosition = posFromRaycast(%ray);
			};
			return;
     	}
		 
     	if(%target.getType() & $TypeMasks::VehicleObjectType)
     	{
     	   	%damage = mClamp(%target.getdatablock().maxDamage/15, 30, %target.getdatablock().maxDamage/2);     	   	

			if(L4B_CheckifinMinigame(%obj,%target))
			{
     	   		%target.damage(%obj, %target.getposition(), %damage, $DamageType::Default);
				%target.applyimpulse(posFromRaycast(%ray),vectoradd(vectorscale(%vec,2000),"0 0 750"));
			}
     	   	serverPlay3D(%this.meleeHitEnvSound @ "_hitenv" @ getRandom(1,2) @ "_sound",posFromRaycast(%ray));

			%p = new projectile()
			{
				datablock = "SecondaryMeleeSmallProjectile";
				initialPosition = posFromRaycast(%ray);
			};
     	}

		if(%target.getType() & $TypeMasks::PlayerObjectType)
		{
			if(%target.getstate() $= "Dead")
			continue;

			%damage = %target.getdatablock().maxDamage/8;
			if(%obj.MeleePowerSwing)
			{
				%damagepower = %damage+%target.getdatablock()/4;
				serverPlay3D("hitenv_sound",posFromRaycast(%ray));
				%obj.MeleePowerSwing = 0;

				%p = new projectile()
				{
					datablock = "SecondaryMeleeProjectile";
					initialPosition = posFromRaycast(%ray);
				};
			}
			else 
			{
				%p = new projectile()
				{
					datablock = "SecondaryMeleeSmallProjectile";
					initialPosition = posFromRaycast(%ray);
				};
				%damagepower = %damage;
			}

			if(vectorDot(%target.getforwardvector(),%obj.getforwardvector()) > 0.65)         
			%damageclamp = mClamp(%damagepower*1.5, %this.meleeDamage, %target.getdatablock().maxDamage);
			else %damageclamp = mClamp(%damagepower, %this.meleeDamage, %target.getdatablock().maxDamage);
			serverPlay3D(%this.meleeHitPlSound @ "_hitpl" @ getRandom(1,2) @ "_sound",posFromRaycast(%ray));
			
			if(L4B_CheckifinMinigame(%obj,%target))
			{
				if(%target.getMountedImage(0) == RiotShieldimage.getID())
				{					
					%sVec = %target.getForwardVector();
					%aimVec = %obj.getForwardVector();
					%reflect = (vectorDot(%sVec, %aimVec) < 0);

					if(!%reflect)
					{
						%target.damage(%obj, %target.getposition(), %damageclamp, $DamageType::Default);
						%target.applyimpulse(posFromRaycast(%ray),vectoradd(vectorscale(%vec,1000),"0 0 750"));

						if(getRandom(1,100) <= %this.meleeZombieStunChance && %target.hZombieL4BType & %target.hZombieL4BType < 5)
						{
							if(%target.getclassName() $= "AIPlayer")
							%target.stopHoleLoop();
					
							%target.emote(winStarProjectile, 1);
							L4B_SpazzZombieInitialize(%target,1);
							%target.mountImage(stunImage,2);
							schedule(1000,0,serverCmdSit,%target);
						}
					}
					else
					{
						%target.applyimpulse(posFromRaycast(%ray),vectoradd(vectorscale(%vec,250),"0 0 187"));
						serverPlay3d("riotshield_block_sound",%obj.getPosition());
					}
				}
				else
				{
					%target.damage(%obj, %target.getposition(), %damageclamp, $DamageType::Default);
					%target.applyimpulse(posFromRaycast(%ray),vectoradd(vectorscale(%vec,1000),"0 0 750"));
					serverPlay3D(%this.meleeHitPlSound @ "_hitpl" @ getRandom(1,2) @ "_sound",posFromRaycast(%ray));

					if(getRandom(1,100) <= %this.meleeZombieStunChance && %target.hZombieL4BType & %target.hZombieL4BType < 5)
					{
						if(%target.getclassName() $= "AIPlayer")
						%target.stopHoleLoop();
				
						%target.emote(winStarProjectile, 1);
						L4B_SpazzZombieInitialize(%target,1);
						%target.mountImage(stunImage,2);
						schedule(1000,0,serverCmdSit,%target);
					}
				}
			}
		}
   } 
   return;
}

function crowbarImage::onFire(%this, %obj, %slot)
{
	if(%obj.getstate() $= "Dead")
	return;

	MeleeSwingCheck(%obj,%this,%slot);
}

function crowbarImage::onPreFire(%this, %obj, %slot)
{
	if(%obj.getstate() $= "Dead")
	return;

	%obj.playthread(1, "meleeArmRaise");
	if(getWord(%obj.getvelocity(),2) == 0)
	{
		%rand = getRandom(1,3);
		%obj.playthread(2, "meleeSwing" @ %rand);

	   	if(%rand == 3)
		{
   			%obj.MeleePowerSwing = 1;
			%obj.playthread(3, "plant");
		}
   		else %obj.MeleePowerSwing = 0;
	}
	else
	{
		%obj.playthread(3, "plant");
		%obj.playthread(2, "meleeSwing3");
		%obj.MeleePowerSwing = 1;
	}
}

function crowbarImage::onStopFire(%this, %obj, %slot)
{
   %obj.playthread(1, "root");
}


datablock ItemData(macheteItem : crowbarItem)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/machete.dts";
	uiName = "Steel Machete";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_machete";
	colorShiftColor = "0.5 0.5 0.5 1";
	image = macheteImage;
};

datablock ShapeBaseImageData(macheteImage : crowbarImage)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/machete.dts";
	offset = "0 -0.025 0.1825";
	item = macheteItem;
	doColorShift = macheteItem.doColorShift;
   	colorShiftColor = macheteItem.colorShiftColor;

	meleeDamage = 75;
	meleeHitEnvSound = "machete";
	meleeHitPlSound = "machete";
	meleeDistanceMin = 1.25;
	meleeZombieStunChance = 10;
};

function macheteImage::onFire(%this, %obj, %slot)
{
	MeleeSwingCheck(%obj,%this,%slot);
}

function macheteImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

function macheteImage::onStopFire(%this, %obj, %slot)
{
	crowbarImage::onStopFire(%this, %obj, %slot);
}


datablock ItemData(hatchetItem : crowbarItem)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/hatchet.dts";
	uiName = "Steel Hatchet";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_hatchet";
	colorShiftColor = "0.6 0.6 0.6 1";
	image = hatchetImage;
};

datablock ShapeBaseImageData(hatchetImage : crowbarImage)
{
   	shapeFile = "Add-Ons/Package_Left4Block/models/melee/hatchet.dts";
   	offset = "0 -0.01 0.4";
   	item = hatchetItem;
   	doColorShift = hatchetItem.doColorShift;
   	colorShiftColor = hatchetItem.colorShiftColor;

   	meleeDamage = 60;
	meleeHitEnvSound = "crowbar";
	meleeHitPlSound = "machete";
	meleeDistanceMin = 0.9;
	meleeZombieStunChance = 15;
};

function hatchetImage::onFire(%this, %obj, %slot)
{
	MeleeSwingCheck(%obj,%this,%slot);
}

function hatchetImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

function hatchetImage::onStopFire(%this, %obj, %slot)
{
	crowbarImage::onStopFire(%this, %obj, %slot);
}


datablock ItemData(pipewrenchItem : crowbarItem)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/pipewrench.dts";
	uiName = "Pipewrench";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_pipewrench";
	colorShiftColor = "0.35 0.35 0.35 1";
	image = pipewrenchImage;
};

datablock ShapeBaseImageData(pipewrenchImage : crowbarImage)
{
  	shapeFile = "Add-Ons/Package_Left4Block/models/melee/pipewrench.dts";
  	offset = "0 0.02 -0.05`";
  	item = pipewrenchItem;
  	doColorShift = pipewrenchItem.doColorShift;
  	colorShiftColor = pipewrenchItem.colorShiftColor;

   	meleeDamage = 24;
	meleeHitEnvSound = "crowbar";
	meleeHitPlSound = "crowbar";
	meleeDistanceMin = 1;
	meleeZombieStunChance = 75;
};

function pipewrenchImage::onFire(%this, %obj, %slot)
{
	MeleeSwingCheck(%obj,%this,%slot);
}

function pipewrenchImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

function pipewrenchImage::onStopFire(%this, %obj, %slot)
{
	crowbarImage::onStopFire(%this, %obj, %slot);
}


datablock ItemData(baseballbatItem : crowbarItem)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/baseballbat.dts";
	uiName = "Baseball Bat";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_baseballbat";
	colorShiftColor = "0.45 0.35 0.25 1";
	image = baseballbatImage;
};

datablock ShapeBaseImageData(baseballbatImage : crowbarImage)
{
  	shapeFile = "Add-Ons/Package_Left4Block/models/melee/baseballbat.dts";
  	offset = "0 0.02 -0.05`";
  	item = baseballbatItem;
  	doColorShift = baseballbatItem.doColorShift;
  	colorShiftColor = baseballbatItem.colorShiftColor;

   	meleeDamage = 30;
	meleeHitEnvSound = "baseballbat";
	meleeHitPlSound = "baseballbat";
	meleeDistanceMin = 1.25;
	meleeZombieStunChance = 80;
};

function baseballbatImage::onFire(%this, %obj, %slot)
{
	MeleeSwingCheck(%obj,%this,%slot);
}

function baseballbatImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

function baseballbatImage::onStopFire(%this, %obj, %slot)
{
	crowbarImage::onStopFire(%this, %obj, %slot);
}


datablock ItemData(fryingpanItem : crowbarItem)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/fryingpan.dts";
	uiName = "Frying Pan";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_fryingpan";
	colorShiftColor = "0.25 0.25 0.25 1";
	image = fryingpanImage;
};

datablock ShapeBaseImageData(fryingpanImage : crowbarImage)
{
   	shapeFile = "Add-Ons/Package_Left4Block/models/melee/fryingpan.dts";
   	offset = "0 0.02 -0.05`";
   	item = fryingpanItem;
   	doColorShift = fryingpanItem.doColorShift;
   	colorShiftColor = fryingpanItem.colorShiftColor;

   	meleeDamage = 28;
	meleeHitEnvSound = "fryingpan";
	meleeHitPlSound = "fryingpan";
	meleeDistanceMin = 1;
	meleeZombieStunChance = 75;
};

function fryingpanImage::onFire(%this, %obj, %slot)
{
	MeleeSwingCheck(%obj,%this,%slot);
}

function fryingpanImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

function fryingpanImage::onStopFire(%this, %obj, %slot)
{
	crowbarImage::onStopFire(%this, %obj, %slot);
}


datablock ItemData(tireironItem : crowbarItem)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/tireiron.dts";
	uiName = "Tire Iron";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_tireiron";
	colorShiftColor = "0.3 0.3 0.3 1";
	image = tireironImage;
};

datablock ShapeBaseImageData(tireironImage : crowbarImage)
{
   	shapeFile = "Add-Ons/Package_Left4Block/models/melee/tireiron.dts";
   	offset = "0 0.02 -0.2";
   	item = tireironItem;
   	doColorShift = tireironItem.doColorShift;
   	colorShiftColor = tireironItem.colorShiftColor;

   	meleeDamage = 30;
	meleeHitEnvSound = "crowbar";
	meleeHitPlSound = "crowbar";
	meleeDistanceMin = 1.25;
	meleeZombieStunChance = 28;
};

function tireironImage::onFire(%this, %obj, %slot)
{
	MeleeSwingCheck(%obj,%this,%slot);
}

function tireironImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

function tireironImage::onStopFire(%this, %obj, %slot)
{
	crowbarImage::onStopFire(%this, %obj, %slot);
}


datablock ItemData(paddleItem : crowbarItem)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/paddle.dts";
	uiName = "Paddle";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_paddle";
	colorShiftColor = "0.3 0.3 0.3 1";
	image = paddleImage;
};

datablock ShapeBaseImageData(paddleImage : crowbarImage)
{
   	shapeFile = "Add-Ons/Package_Left4Block/models/melee/paddle.dts";
   	offset = "0 0.02 0.25";
   	item = paddleItem;
   	doColorShift = paddleItem.doColorShift;
   	colorShiftColor = paddleItem.colorShiftColor;

	meleeDamage = 32;
	meleeHitEnvSound = "baseballbat";
	meleeHitPlSound = "baseballbat";
	meleeDistanceMin = 1;
	meleeZombieStunChance = 50;
};

function paddleImage::onFire(%this, %obj, %slot)
{
	MeleeSwingCheck(%obj,%this,%slot);
}

function paddleImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

function paddleImage::onStopFire(%this, %obj, %slot)
{
	crowbarImage::onStopFire(%this, %obj, %slot);
}


datablock ItemData(batonItem : crowbarItem)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/baton.dts";
	uiName = "Baton";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_baton";
	colorShiftColor = "0.125 0.125 0.125 1";
	image = batonImage;
};

datablock ShapeBaseImageData(batonImage : crowbarImage)
{
   	shapeFile = "Add-Ons/Package_Left4Block/models/melee/baton.dts";
   	offset = "0 -0.25 0.35";
   	item = batonItem;
   	doColorShift = batonItem.doColorShift;
   	colorShiftColor = batonItem.colorShiftColor;

	meleeDamage = 30;
	meleeHitEnvSound = "baseballbat";
	meleeHitPlSound = "crowbar";
	meleeDistanceMin = 1.25;
	meleeZombieStunChance = 75;
};

function batonImage::onFire(%this, %obj, %slot)
{
	MeleeSwingCheck(%obj,%this,%slot);
}

function batonImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

function batonImage::onStopFire(%this, %obj, %slot)
{
	crowbarImage::onStopFire(%this, %obj, %slot);
}


datablock ItemData(icepickItem : crowbarItem)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/icepick.dts";
	uiName = "Icepick";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_icepick";
	colorShiftColor = "0.5 0.25 0.25 1";
	image = icepickImage;
};

datablock ShapeBaseImageData(icepickImage : crowbarImage)
{
   	shapeFile = "Add-Ons/Package_Left4Block/models/melee/icepick.dts";
   	offset = "0 0.05 0.5";
   	item = icepickItem;
   	doColorShift = icepickItem.doColorShift;
   	colorShiftColor = icepickItem.colorShiftColor;

	meleeDamage = 50;
	meleeHitEnvSound = "crowbar";
	meleeHitPlSound = "machete";
	meleeDistanceMin = 1.5;
	meleeZombieStunChance = 40;
};

function icepickImage::onFire(%this, %obj, %slot)
{
	MeleeSwingCheck(%obj,%this,%slot);
}

function icepickImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

function icepickImage::onStopFire(%this, %obj, %slot)
{
	crowbarImage::onStopFire(%this, %obj, %slot);
}


datablock ItemData(leadpipeItem : crowbarItem)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/leadpipe.dts";
	uiName = "Leadpipe";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_leadpipe";
	colorShiftColor = "0.2 0.2 0.2 1";
	image = leadpipeImage;
};

datablock ShapeBaseImageData(leadpipeImage : crowbarImage)
{
   	shapeFile = "Add-Ons/Package_Left4Block/models/melee/leadpipe.dts";
   	offset = "0 0.025 0.325";
   	item = leadpipeItem;
   	doColorShift = leadpipeItem.doColorShift;
   	colorShiftColor = leadpipeItem.colorShiftColor;

	meleeDamage = 50;
	meleeHitEnvSound = "crowbar";
	meleeHitPlSound = "crowbar";
	meleeDistanceMin = 1.5;
	meleeZombieStunChance = 40;
};

function leadpipeImage::onFire(%this, %obj, %slot)
{
	MeleeSwingCheck(%obj,%this,%slot);
}

function leadpipeImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

function leadpipeImage::onStopFire(%this, %obj, %slot)
{
	crowbarImage::onStopFire(%this, %obj, %slot);
}


datablock ItemData(shovelItem : crowbarItem)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/shovel.dts";
	uiName = "Shovel";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_shovel";
	colorShiftColor = "0.35 0.35 0.35 1";
	image = shovelImage;
};

datablock ShapeBaseImageData(shovelImage : crowbarImage)
{
   	shapeFile = "Add-Ons/Package_Left4Block/models/melee/shovel.dts";
   	offset = "0 0 0.5";
   	item = shovelItem;
   	doColorShift = shovelItem.doColorShift;
   	colorShiftColor = shovelItem.colorShiftColor;

	meleeDamage = 40;
	meleeHitEnvSound = "crowbar";
	meleeHitPlSound = "crowbar";
	meleeDistanceMin = 1.5;
	meleeZombieStunChance = 60;
};

function shovelImage::onFire(%this, %obj, %slot)
{
	MeleeSwingCheck(%obj,%this,%slot);
}

function shovelImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

function shovelImage::onStopFire(%this, %obj, %slot)
{
	crowbarImage::onStopFire(%this, %obj, %slot);
}


datablock ItemData(spikebatItem : crowbarItem)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/spikebat.dts";
	uiName = "Spikebat";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_spikebat";
	colorShiftColor = "0.25 0.45 0.35 1";
	image = spikebatImage;
};

datablock ShapeBaseImageData(spikebatImage : crowbarImage)
{
   	shapeFile = "Add-Ons/Package_Left4Block/models/melee/spikebat.dts";
   	offset = "0 0 0.5";
   	item = spikebatItem;
   	doColorShift = spikebatItem.doColorShift;
   	colorShiftColor = spikebatItem.colorShiftColor;

	meleeDamage = 70;
	meleeHitEnvSound = "baseballbat";
	meleeHitPlSound = "spikebat";
	meleeDistanceMin = 2;
	meleeZombieStunChance = 50;
};

function spikebatImage::onFire(%this, %obj, %slot)
{
	MeleeSwingCheck(%obj,%this,%slot);
}

function spikebatImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

function spikebatImage::onStopFire(%this, %obj, %slot)
{
	crowbarImage::onStopFire(%this, %obj, %slot);
}


datablock ItemData(golfclubItem : crowbarItem)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/golfclub.dts";
	uiName = "Golfclub";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_golfclub";
	colorShiftColor = "0.5 0.5 0.5 1";
	image = golfclubImage;
};

datablock ShapeBaseImageData(golfclubImage : crowbarImage)
{
   	shapeFile = "Add-Ons/Package_Left4Block/models/melee/golfclub.dts";
   	offset = "0 0 0.25";
   	item = golfclubItem;
   	doColorShift = golfclubItem.doColorShift;
   	colorShiftColor = golfclubItem.colorShiftColor;

	meleeDamage = 30;
	meleeHitEnvSound = "crowbar";
	meleeHitPlSound = "crowbar";
	meleeDistanceMin = 1.5;
	meleeZombieStunChance = 30;
};

function golfclubImage::onFire(%this, %obj, %slot)
{
	MeleeSwingCheck(%obj,%this,%slot);
}

function golfclubImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

function golfclubImage::onStopFire(%this, %obj, %slot)
{
	crowbarImage::onStopFire(%this, %obj, %slot);
}

datablock ItemData(blackhammerItem : crowbarItem)
{
	shapeFile = "Add-Ons/Package_Left4Block/models/melee/hammer.dts";
	uiName = "Black Hammer";
	iconName = "Add-Ons/Package_Left4Block/icons/icon_hammer";
	colorShiftColor = "0.5 0.5 0.5 1";
	image = blackhammerImage;
};

datablock ShapeBaseImageData(blackhammerImage : crowbarImage)
{
   	shapeFile = "Add-Ons/Package_Left4Block/models/melee/hammer.dts";
   	offset = "0 0 0.6";
   	item = blackhammerItem;
   	doColorShift = blackhammerItem.doColorShift;
   	colorShiftColor = blackhammerItem.colorShiftColor;

	meleeDamage = 30;
	meleeHitEnvSound = "crowbar";
	meleeHitPlSound = "crowbar";
	meleeDistanceMin = 1.1;
	meleeZombieStunChance = 25;
};

function blackhammerImage::onFire(%this, %obj, %slot)
{
	MeleeSwingCheck(%obj,%this,%slot);
}

function blackhammerImage::onPreFire(%this, %obj, %slot)
{
	crowbarImage::onPreFire(%this, %obj, %slot);
}

function blackhammerImage::onStopFire(%this, %obj, %slot)
{
	crowbarImage::onStopFire(%this, %obj, %slot);
}