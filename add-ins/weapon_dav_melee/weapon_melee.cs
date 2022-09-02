//AddDamageType ("Default", '<bitmap:base/client/ui/ci/skull> %1', '%2 <bitmap:base/client/ui/ci/skull> %1!', 1, 0);

%pattern = "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/sound/*.wav";//Too lazy to write datablock files for the sounds, just took this from the Disease Gamemode
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/gamemode_left4block/add-ins/weapon_dav_melee/sound/", "");
	%soundName = strreplace(%soundName, "/", "");
	%soundName = strreplace(%soundName, ".wav", "");
	%soundName = strreplace(%soundName, "quiet", "");
	%soundName = strreplace(%soundName, "normal", "");
	%soundName = strreplace(%soundName, "loud", "");

	//Check the names of the folders to determine what type of soundscape it will be, and check if it's a loopable sound or not
	if(strstr(%file,"normal") != -1)//Normal soundscape
	if(strstr(%file,"loop") != -1)
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioCloseLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClose3d; filename = \"" @ %file @ "\"; };");

	if(strstr(%file,"quiet") != -1)//Quiet soundscape
	if(strstr(%file,"loop") != -1)
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosestLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosest3d; filename = \"" @ %file @ "\"; };");

	if(strstr(%file,"loud") != -1)//Loudest Soundscape
	if(strstr(%file,"loop") != -1)
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioDefaultLooping3d; filename = \"" @ %file @ "\"; };");
	else eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioDefault3d; filename = \"" @ %file @ "\"; };");

	%file = findNextFile(%pattern);
}

datablock ItemData(crowbarItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system
	shapeFile = "./models/model_crowbar.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	uiName = "Crowbar";
	iconName = "./icons/icon_crowbar";
	doColorShift = true;
	colorShiftColor = "0.5 0.5 0.5 1";
	image = crowbarImage;
	canDrop = true;
};

datablock ShapeBaseImageData(crowbarImage)
{
	shapeFile = "./models/model_crowbar.dts";
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

	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0;
	stateTransitionOnTimeout[0]      = "Ready";
	stateSound[0]                    = WeaponSwitchsound;

	stateName[1]                     = "Ready";
	stateScript[1]                  = "onReady";
	stateTransitionOnTriggerDown[1]  = "PreFire";
	stateAllowImageChange[1]         = true;

	stateName[2]					= "PreFire";
	stateScript[2]                  = "onPreFire";
	stateAllowImageChange[2]        = false;
	stateTimeoutValue[2]            = 0.095;
	stateTransitionOnTimeout[2]     = "Fire";

	stateName[3]                    = "Fire";
	stateTransitionOnTimeout[3]     = "CheckFire";
	stateFire[3]                    = false;
	stateSequence[3]                = "Fire";
	stateScript[3]                  = "onFire";
	stateTimeoutValue[3]            = 0.215;

	stateName[4]			= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "StopFire";

	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Ready";
	stateTimeoutValue[5]            = 0.275;
	stateSequence[5]                = "StopFire";
	stateScript[5]                  = "onStopFire";
};

function MeleeSwingCheck(%obj,%this,%slot)
{   	
	if(!isObject(%obj) || !isObject(%obj.getMountedImage(0)) || %obj.getMountedImage(0).meleeDamage $= "") return;
	
	%pos = %obj.getMuzzlePoint(%slot);
	%radius = 2;
	%searchMasks = $TypeMasks::StaticObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	InitContainerRadiusSearch(%pos, %radius, %searchMasks);
	while (%target = containerSearchNext())
   	{
      	if(%target == %obj) continue;
	
      	%len = 2 * getWord(%obj.getScale (), 2);
      	%vec = %obj.getMuzzleVector(%slot);
      	%beam = vectorScale(%vec,%len);
      	%end = vectorAdd(%pos,%beam);
      	%ray = containerRayCast(%pos,%end,%searchMasks,%obj); //fire raycast

     	if(vectorDist(%pos,posFromRaycast(%ray)) > 1.5) continue;

     	if(%ray.getType() & $TypeMasks::FxBrickObjectType || %ray.getType() & $TypeMasks::StaticObjectType)
     	{
			if(%obj.lastmeleehit+75 < getsimtime())
			{
				%obj.lastmeleehit = getsimtime();
				serverPlay3D(%this.meleeHitEnvSound @ "_hitenv" @ getRandom(1,2) @ "_sound",posFromRaycast(%ray));

				%p = new projectile()
				{
					datablock = "SecondaryMeleeProjectile";
					initialPosition = posFromRaycast(%ray);
				};
				%p.explode();
			}
     	}
		 
     	if(%ray.getType() & $TypeMasks::VehicleObjectType)
     	{
     	   	%damage = mClamp(%ray.getdatablock().maxDamage/15, 30, %ray.getdatablock().maxDamage/2);     	   	

			if(minigameCanDamage(%obj,%ray))
			{
     	   		%ray.damage(%obj, posFromRaycast(%ray), %damage, $DamageType::Default);
				%ray.applyimpulse(posFromRaycast(%ray),vectoradd(vectorscale(%vec,2000),"0 0 750"));
			}
     	   	serverPlay3D(%this.meleeHitEnvSound @ "_hitenv" @ getRandom(1,2) @ "_sound",posFromRaycast(%ray));

			%p = new projectile()
			{
				datablock = "SecondaryMeleeProjectile";
				initialPosition = posFromRaycast(%ray);
			};
			%p.explode();
     	}

		if(%ray.getType() & $TypeMasks::PlayerObjectType)
		{
			if(%ray.getstate() $= "Dead") continue;

			%damage = %ray.getdatablock().maxDamage/8;
			if(%obj.MeleePowerSwing)
			{
				%damagepower = %damage+%ray.getdatablock()/4;
				%obj.MeleePowerSwing = 0;
			}
			else %damagepower = %damage;

			%p = new projectile()
			{
				datablock = "SecondaryMeleeProjectile";
				initialPosition = posFromRaycast(%ray);
			};
			%p.explode();

			if(vectorDot(%ray.getforwardvector(),%obj.getforwardvector()) > 0.65)         
			%damageclamp = mClamp(%damagepower*1.5, %this.meleeDamage, %ray.getdatablock().maxDamage);
			else %damageclamp = mClamp(%damagepower, %this.meleeDamage, %ray.getdatablock().maxDamage);
			serverPlay3D(%this.meleeHitPlSound @ "_hitpl" @ getRandom(1,2) @ "_sound",posFromRaycast(%ray));
			
			if(minigameCanDamage(%obj,%ray) && checkHoleBotTeams(%obj,%ray))
			{
				if(%ray.getdatablock().getName() $= "ZombieTankHoleBot")
				%ray.damage(%obj,posFromRaycast(%ray),150,$damageclamp::Default);
				else %ray.damage(%obj,posFromRaycast(%ray),%damageclamp,$DamageType::Default);

				%ray.applyimpulse(posFromRaycast(%ray),vectoradd(vectorscale(%vec,1000),"0 0 750"));
				serverPlay3D(%this.meleeHitPlSound @ "_hitpl" @ getRandom(1,2) @ "_sound",posFromRaycast(%ray));
			}
		}
   	}

	cancel(%obj.meleechecksched);
   	return %obj.meleechecksched = schedule(50,0,MeleeSwingCheck,%obj,%this,%slot);
}

function crowbarImage::onReady(%this, %obj, %slot)
{
	if(%obj.getstate() $= "Dead") return;
	%obj.playthread(1, "root");
}

function crowbarImage::onFire(%this, %obj, %slot)
{
	if(%obj.getstate() $= "Dead") return;
	%obj.meleechecksched = schedule(20,0,MeleeSwingCheck,%obj,%this,%slot);
}

function crowbarImage::onPreFire(%this, %obj, %slot)
{
	if(%obj.getstate() $= "Dead") return;

	serverPlay3D("melee_swing_sound",%obj.gethackposition());
	%obj.playthread(1, "meleeRaise");

	if(getWord(%obj.getvelocity(),2) == 0)
	{
		%rand = getRandom(1,2);
		%obj.playthread(2, "meleeSwing" @ %rand);

	   	if(%rand == 3 && getRandom(1,8) == 1)
		{
			%obj.playthread(2, "meleeSwing3");
   			%obj.MeleePowerSwing = 1;
			%obj.playthread(2, "plant");
		}
   		else %obj.MeleePowerSwing = 0;
	}
	else
	{
		%obj.playthread(2, "meleeSwing3");
		%obj.MeleePowerSwing = 1;
	}
}

function crowbarImage::onStopFire(%this, %obj, %slot)
{
   cancel(%obj.meleechecksched);
}