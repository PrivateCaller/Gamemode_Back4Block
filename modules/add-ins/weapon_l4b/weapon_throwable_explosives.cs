function PropaneTankItem::onPickup(%this, %obj, %user, %amount)
{  
	Parent::onPickup(%this, %obj, %user, %amount);
	
	for(%i=0;%i<%user.getDatablock().maxTools;%i++)
	{
		%toolDB = %user.tool[%i];
		if(%toolDB $= %this.getID())
		{
			servercmdUseTool(%user.client,%i);
			break;
		}
	}
}

function PropaneTankImage::onFire(%this, %obj, %slot)
{
	%obj.sourcerotation = %obj.gettransform();
	%muzzlepoint = %obj.getHackPosition();
	%muzzlevector = vectorScale(%obj.getEyeVector(),3);
	%muzzlepoint = VectorAdd (%muzzlepoint, %muzzlevector);
	%playerRot = rotFromTransform (%obj.getTransform());

	%item = new WheeledVehicle(ExplosiveItemVehicle) 
	{  
		rotation = getwords(%obj.sourcerotation,3,6);
		datablock  = %this.vehicle;
		sourceObject = %obj.client.player;
		minigame = getMinigameFromObject(%obj);
		brickGroup = %obj.client.brickGroup;
	};

	%item.schedule(60000,delete);
	%item.startfade(5000,55000,1);
	%item.setTransform (%muzzlepoint @ " " @ %playerRot);
	%item.applyimpulse(%item.gettransform(),vectoradd(vectorscale(%obj.getEyeVector(),10000),"0" SPC "0" SPC 5000));

	%obj.droppedExplosiveVeh = 1;
	%obj.playthread(1,root);
	%obj.unMountImage(0);
}

function PropaneTankCol::onAdd(%this,%obj)
{
	%obj.setNodeColor("ALL",%this.image.item.colorShiftColor);
	
	Parent::onAdd(%this,%obj);
}
function PropaneTankCol::onCollision(%this, %obj, %col, %fade, %pos, %norm)
{
	Parent::onCollision(%this, %obj, %col, %fade, %pos, %norm);
}

function PropaneTankCol::onDamage(%this,%obj)
{
	%c = new projectile()
	{
		datablock = PropaneTankFinalExplosionProjectile;
		initialPosition = %obj.getPosition();
		client = %obj.creator.client;
		sourceObject = %obj.sourceObject;
		damageType = $DamageType::Boomer;
	};

	Parent::onDamage(%this,%obj);
}

function PropaneTankImage::onMount(%this,%obj,%slot)
{
	Parent::onMount(%this,%obj,%slot);	
	%obj.playThread(1, armReadyRight);
}

function PropaneTankImage::onUnMount(%this,%obj,%slot)
{
	Parent::onUnMount(%this,%obj,%slot);

	if(!%obj.droppedExplosiveVeh )
	{
		%obj.sourcerotation = %obj.gettransform();
		%muzzlepoint = %obj.getHackPosition();
		%muzzlevector = vectorScale(%obj.getEyeVector(),3);
		%muzzlepoint = VectorAdd (%muzzlepoint, %muzzlevector);
		%playerRot = rotFromTransform (%obj.getTransform());
	
		%item = new WheeledVehicle(ExplosiveItemVehicle) 
		{  
			rotation = getwords(%obj.sourcerotation,3,6);
			datablock  = %this.vehicle;
			sourceObject = %obj.client.player;
			minigame = getMinigameFromObject(%obj);
			brickGroup = %obj.client.brickGroup;
		};
	
		%item.schedule(60000,delete);
		%item.startfade(5000,55000,1);
		%item.setTransform (%muzzlepoint @ " " @ %playerRot);
		%item.applyimpulse(%item.gettransform(),vectoradd(vectorscale(%obj.getEyeVector(),10000),"0" SPC "0" SPC 5000));
	}

	%obj.playThread(3,jump);
	%obj.playThread(2,activate2);
	%obj.droppedExplosiveVeh = 0;
}

function GasCanItem::onPickup(%this, %obj, %player)
{  
	PropaneTankItem::onPickup(%this, %obj, %player);
}

function GasCanImage::onFire(%this, %obj, %slot)
{
	PropaneTankImage::onFire(%this, %obj, %slot);
}

function GasCanCol::onAdd(%this,%obj)
{
	PropaneTankCol::onAdd(%this,%obj);
}
function GasCanCol::oncollision(%this, %obj, %col, %fade, %pos, %norm)
{
	propaneTankCol::oncollision(%this, %obj, %col, %fade, %pos, %norm);
}
function GasCanCol::onDamage(%this,%obj)
{
	createFireCircle(%obj.getPosition(),30,70,%obj.sourceobject.client,%obj,$DamageType::Molotov);
}
function GasCanImage::onMount(%this,%obj,%slot)
{
	PropaneTankImage::onMount(%this,%obj,%slot);
}

function GasCanImage::onUnMount(%this,%obj,%slot)
{
	PropaneTankImage::onUnMount(%this,%obj,%slot);
}

function BileOGasItem::onPickup(%this, %obj, %player)
{  
	PropaneTankItem::onPickup(%this, %obj, %player);
}

function BileOGasImage::onFire(%this, %obj, %slot)
{
	PropaneTankImage::onFire(%this, %obj, %slot);
}

function BileOGasCol::onAdd(%this,%obj)
{
	Parent::onAdd(%this,%obj);
	
	%obj.ContinueSearch = %obj.getDatablock().schedule(500,Distract,%obj);
	%obj.setScale("1 1 1");
}

function BileOGasCol::onDamage(%this,%obj)
{
	createFireCircle(%obj.getPosition(),30,70,%obj.sourceobject.client,%obj,$DamageType::Molotov);

	%c = new projectile()
	{
		datablock = BoomerProjectile;
		initialPosition = %obj.getPosition();
		client = %obj.sourceObject.client;
		sourceObject = %obj.sourceObject;
		damageType = $DamageType::Boomer;
	};
}

function BileOGasCol::oncollision(%this, %obj, %col, %fade, %pos, %norm)
{
	propaneTankCol::oncollision(%this, %obj, %col, %fade, %pos, %norm);
}

function BileOGasImage::onMount(%this,%obj,%slot)
{
	Parent::onMount(%this,%obj,%slot);
	%obj.playThread(1, armReadyRight);
	%obj.playaudio(3,weaponSwitchSound);
}

function BileOGasCol::Distract(%this, %obj)
{	
	if(!isObject(%obj))
	return;

	cancel(%obj.ContinueSearch);
	%obj.ContinueSearch = %obj.getDatablock().schedule(1000,Distract,%obj);

	%pos = %obj.getPosition();
	%radius = %this.DistractionRadius;

	%searchMasks = $TypeMasks::PlayerObjectType;
	InitContainerRadiusSearch(%pos, %radius, %searchMasks);

	while((%targetid = containerSearchNext()) != 0 )
	{
		if(!%targetid.getState() !$= "Dead" && %targetid.getClassName() $= "AIPlayer" && %targetid.hZombieL4BType $= "Normal" && !%targetid.isBurning)
		{
			if(!%targetid.Distraction)
			{
				%targetid.Distraction = %obj.getID();
				%targetid.hSearch = 0;
			}
			else if(%targetid.Distraction == %obj.getID())
			{
				%targetid.setaimobject(%obj);
				%targetid.setmoveobject(%obj);
				%time1 = getRandom(1000,4000);
				%targetid.getDataBlock().schedule(%time1,onBotFollow,%targetid,%obj);
			}
		}
	}
}

function BileOGasImage::onUnMount(%this,%obj,%slot)
{
	PropaneTankImage::onUnMount(%this,%obj,%slot);
}

function OxygenTankItem::onPickup(%this, %obj, %player)
{  
	PropaneTankItem::onPickup(%this, %obj, %player);
}

function OxygenTankImage::onFire(%this, %obj, %slot)
{
	PropaneTankImage::onFire(%this, %obj, %slot);
}

function OxygenTankCol::onAdd(%this,%obj)
{
	PropaneTankCol::onAdd(%this,%obj);
}

function OxygenTankCol::oncollision(%this, %obj, %col, %fade, %pos, %norm)
{
	PropaneTankCol::oncollision(%this, %obj, %col, %fade, %pos, %norm);
}

function OxygenTankCol::onDamage(%this,%obj)
{	
	if(!isEventPending(%obj.AboutToExplode))
	{
		%obj.AboutToExplode = %obj.schedule(1900,Damage,%obj,%obj.getPosition(),%this.maxDamage,$DamageType::Suicide);
		%obj.playaudio(3,oxygentank_leak_sound);
		%obj.ContinueSearch = %obj.getDatablock().schedule(250,Distract,%obj);
	}

	if(%obj.getDamageLevel() >= %this.maxDamage)
	PropaneTankCol::onDamage(%this,%obj);

	Parent::onDamage(%this,%obj);
}

function OxygenTankCol::Distract(%this, %obj)
{	
	if(!isObject(%obj))
	return;

	cancel(%obj.ContinueSearch);
	%obj.ContinueSearch = %obj.getDatablock().schedule(500,Distract,%obj);

	%pos = %obj.getPosition();
	%radius = %this.DistractionRadius;

	%searchMasks = $TypeMasks::PlayerObjectType;
	InitContainerRadiusSearch(%pos, %radius, %searchMasks);

	while((%targetid = containerSearchNext()) != 0 )
	{
		if(!%targetid.getState() !$= "Dead" && %targetid.getClassName() $= "AIPlayer" && %targetid.hZombieL4BType && %targetid.hZombieL4BType < 5 && !%targetid.isBurning)
		{
			if(!%targetid.Distraction)
			{
				%targetid.Distraction = %obj.getID();
				%targetid.hSearch = 0;
			}
			else if(%targetid.Distraction == %obj.getID())
			{
				%targetid.setaimobject(%obj);
				%targetid.setmoveobject(%obj);
				%time1 = getRandom(1000,4000);
				%targetid.getDataBlock().schedule(%time1,onBotFollow,%targetid,%obj);
			}
		}
	}
}
function OxygenTankImage::onMount(%this,%obj,%slot)
{
	PropaneTankImage::onMount(%this,%obj,%slot);
}

function OxygenTankImage::onUnMount(%this,%obj,%slot)
{
	PropaneTankImage::onUnMount(%this,%obj,%slot);
}