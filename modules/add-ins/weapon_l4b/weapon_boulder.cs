//projectile
datablock ProjectileData(BoulderProjectile)
{
   projectileShapeName = "./models/BoulderProjectile.dts";
   directDamage        = 30;
   directDamageType  = $DamageType::BoulderDirect;
   radiusDamageType  = $DamageType::BoulderRadius;
   impactImpulse	   = 500;
   verticalImpulse	   = 150;
   explosion           = BoulderExplosion;
   particleEmitter     = boulderTrailEmitter;

   brickExplosionRadius = 0;
   brickExplosionImpact = true; //destroy a brick if we hit it directly?
   brickExplosionForce  = 0;
   brickExplosionMaxVolume = 0;
   brickExplosionMaxVolumeFloating = 0;

   muzzleVelocity      = 20;
   velInheritFactor    = 1;

   armingDelay         = 0;
   lifetime            = 20000;
   fadeDelay           = 19500;
   bounceElasticity    = 0;
   bounceFriction      = 0;
   isBallistic         = true;
   gravityMod = 1;

   hasLight    = false;
   uiName = "";
};

datablock ProjectileData(BoulderProjectile1 : BoulderProjectile)
{
   directDamage        = 0;
   impactImpulse	   = 0;
   verticalImpulse	   = 0;
   explosion           = BoulderExplosion1;

};

datablock ShapeBaseImageData(BoulderImage)
{
   shapeFile = "./models/Boulder.dts";
   emap = true;
   mountPoint = 0;
   offset = "-1.625 0 0";
   rotation				= eulerToMatrix( "90 0 90" );
   correctMuzzleVector = true;
   className = "WeaponImage";

   // Projectile && Ammo.
   item = BoulderItem;
   ammo = " ";
   projectile = "";
   projectileType = Projectile;

   //melee particles shoot from eye node for consistancy
   melee = true;
   //raise your arm up or not
   armReady = true;

   isChargeWeapon = true;

   //casing = " ";
   doColorShift = true;
   colorShiftColor = "0.400 0.196 0 1.000";

	stateName[0]			= "Activate";
	stateTimeoutValue[0]		= 1.5;
	stateTransitionOnTimeout[0]	= "Ready";
	stateSequence[0]		= "ready";
   stateScript[0]			= "onActivate";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]			= "Ready";
	stateTransitionOnTriggerDown[1]	= "Fire";
	stateAllowImageChange[1]	= true;

	stateName[2]			= "Fire";
	stateTransitionOnTimeout[2]	= "Ready";
	stateFire[2]			= true;
	stateSequence[2]		= "fire";
	stateScript[2]			= "onFire";
	stateWaitForTimeout[2]		= true;
   stateTimeoutValue[2]		= 5;
	stateAllowImageChange[2]	= false;
	stateSound[2]				= "";
};

function BoulderImage::onActivate(%this, %obj, %slot)
{
   %obj.playaudio(1, "tank_rock_grab_sound");
   %obj.playthread(1, spearReady);
   %obj.spawnExplosion(BoulderProjectile1,1); //boom
   %obj.setenergylevel(0);
}

function BoulderProjectile::onExplode(%this,%obj)
{
	Parent::onExplode(%this,%obj);
}

function BoulderImage::onFire(%this, %obj, %slot)
{
   %obj.schedule(75,TankThrowBoulder);
   %obj.playthread(1, spearThrow);
   %obj.playaudio(1, "tank_rock_toss" @ getRandom(1,3) @ "_sound");
   return;
}


function Player::TankThrowBoulder(%obj)
{
   %obj.playthread(2, "activate2");
   %obj.playthread(3, "activate2");
   %obj.playthread(0, "jump");

   if(isObject(%targ = %obj.hFollowing))
   %velocity = vectorscale(%obj.getEyeVector(),10+vectorDist(%obj.getHackPosition(),%targ.getHackPosition())*0.75);
   else %velocity = vectorscale(%obj.getEyeVector(),75);
   
   %p = new Projectile()
   {
      dataBlock = "BoulderProjectile";
      initialVelocity = vectorAdd(%velocity,"0 0 2.5");
      initialPosition = %obj.getHackPosition();
      sourceObject = %obj;
      client = %obj.client;
      scale = "4 4 4";
   };
   MissionCleanup.add(%p);
   %obj.unMountImage(0);
}