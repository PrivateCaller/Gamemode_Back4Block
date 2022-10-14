function serverCmdClearDecals(%client) 
{	
	if(!%client.isAdmin) 
	{
		messageClient(%client, '', "\c3You are not allowed to use this command.");
		return;
	}
	else
	{
		if(isObject(DecalGroup)) %decals = DecalGroup.getCount();
		messageAll('MsgClearBricks', "\c3" @ %client.getPlayerName() @ "\c2 cleared all decals on the server." SPC "(" @ %decals @ " decals)");
		if(isObject(DecalGroup)) DecalGroup.deleteAll();
	}
}

function Player::SapHealth(%obj,%amount,%maxamount) 
{	
	if(!isObject(%obj) || %amount > %maxamount) return;

	%projectile = new Projectile() 
	{
		dataBlock = BloodDripProjectile;
		initialPosition = %obj.getMuzzlePoint(2);
		initialVelocity = "0 0 -2";
	};
	%obj.AddHealth(%obj.getDataBlock().maxDamage*-0.0005);

	cancel(%obj.SapHealthSched);
	%obj.schedule(500,SapHealth,%amount++,%maxamount);
}

function AIPlayer::SapHealth(%obj,%amount,%maxamount) { Player::SapHealth(%obj,%amount,%maxamount); }

function BloodDripProjectile::onCollision(%this, %obj, %col, %pos, %fade, %normal) { %obj.explode(); }

function ceilingBloodLoop(%obj) 
{
	if(!isObject(%obj)) return;	
	if(!%obj.delay) %obj.delay = 500;

	%projectile = new Projectile() 
	{
		dataBlock = BloodDripProjectile;
		initialPosition = %obj.getPosition();
		initialVelocity = "0 0 -2";
	};
	
	cancel(%obj.ceilingBloodSchedule);
	%obj.ceilingBloodSchedule = schedule(%obj.delay+=25, 0, ceilingBloodLoop, %obj);
}

function vectorToAxis(%vector) 
{
	%y = mRadToDeg(mACos(getWord(%vector, 2) / vectorLen(%vector))) % 360;
	%z = mRadToDeg(mATan(getWord(%vector, 1), getWord(%vector, 0)));
	%euler = vectorScale(0 SPC %y SPC %z, $pi / 180);
	return getWords(matrixCreateFromEuler(%euler), 3, 6);
}

function spawnDecal(%dataBlock, %position, %vector,%scale) 
{
	if(!isObject(MissionCleanup) || !isObject(%dataBlock) || %dataBlock.getClassName() !$= StaticShapeData) return;
	if(!isObject(DecalGroup)) MissionCleanup.add(new SimGroup(DecalGroup));	
	else if(DecalGroup.getCount() >= $Pref::Server::L4BBlood::BloodDecalsLimit) return; 

	%obj = new StaticShape() { dataBlock = %dataBlock; };
	if(getRandom(1,4) == 1)
	{
		%startpos = vectorAdd(%position, "0 0 0.1");
		%endpos = vectorAdd(%position, "0 0 -1");
		%raycheckbelow = containerRayCast(%startpos, %endpos, $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType);
		if(!isObject(%raycheckbelow)) %obj.ceilingBloodSchedule = schedule(getRandom(16, 500), 0, ceilingBloodLoop, %obj);		
	}

	%obj.setTransform(%position SPC vectorToAxis(%vector));
	%obj.setScale(getRandom(10,%scale)*0.1 SPC getRandom(10,%scale)*0.1 SPC 1);
	DecalGroup.add(%obj);
	%obj.setNodeColor("ALL", %dataBlock.colorShiftColor);
	if($Pref::Server::L4BBlood::BloodDecalsTimeout >= 0) %obj.schedule($Pref::Server::L4BBlood::BloodDecalsTimeout, delete);
	return %obj;
}

function Armor::doSplatterBlood(%this,%obj,%amount) 
{	
	%pos = %obj.getHackPosition();

	for (%i = 0; %i < getRandom(%amount); %i++) 
	{
		%negativevectorrandom = getRandom(-5,5) SPC getRandom(-5,5) SPC getRandom(-5,5);
		%cross = vectorScale(%negativevectorrandom,5);
		%stop = vectorAdd(%pos, %cross);
		%ray = containerRayCast(%pos, %stop, $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType);
		
		if(isObject(%ray) && vectorDist(posFromRaycast(%ray),%obj.getHackPosition()) < 5)
		{			
			if(getRandom(1,4) == 1) serverPlay3d("blood_spill_sound", getWords(%ray, 1, 3));
			spawnDecal(BloodDecal @ getRandom(1, 2),posFromRaycast(%ray),getWords(%ray, 4, 6),%amount*5);		
			%projectile = new Projectile()
			{
				dataBlock = bloodExplosionProjectile @ getRandom(1, 2);
				initialPosition = posFromRaycast(%ray);
			};
			MissionCleanup.add(%projectile);
			%projectile.explode();
		}
	}
}

function vectorClosestPointAngle(%vector, %startPoint, %point)
{
	%vN = vectorNormalize(%vector);
	%v2 = vectorSub(%point, %startPoint);
	%dot = vectorDot(%vN, vectorNormalize(%v2));
	%closestPoint = vectorAdd(%startPoint, vectorScale(%vN, %dot * vectorLen(%v2)));
	%angle = mACos(%dot);
	return %closestPoint SPC %angle;
}

function Player::rgetDamageLocation(%obj, %position)
{	
	%scale = getWord(%obj.getScale(), 2);
	%forwardVector = %obj.getForwardVector();						// direction the bot is facing
	%sideVector = vectorCross(%forwardVector, %obj.getUpVector());  // vector facing directly to the sides of the bot
	%centerPoint = %obj.getWorldBoxCenter();								// middle reference point
	%CPR = vectorClosestPointAngle(%sideVector, %centerPoint, %position);	// where we are on the side-to-side vector
	%closestPoint = getWords(%CPR, 0, 2);									// closest point to where our round hit on the side-to-side vector
	%angle = mRadToDeg(getWord(%CPR, 3));									// angle from the reference point. 90 is center, 0 is left, 180 is right.
	%distanceFromCenter = vectorDist(%closestPoint, %centerPoint);			// distance from the reference point to the side-to-side vector. 0 is center, anything above .4 is an arm.
	%zLocation = getWord(%position, 2);					// Z position of our impact
	%zWorldBox = getword(%obj.getWorldBoxCenter(), 2);	// Where the middle of our player is on the z axis
	
	if(%zLocation > %zWorldBox - 3.3 * %scale) %limb = 0;//head
	else if(%zLocation > %zWorldBox - 4.2 * %scale)
	{
		if(%distanceFromCenter < (0.4 * %scale)) %limb = 1;// Torso	
		else if(%angle < 90)
		{
			if(%zLocation > %zWorldBox - 3.825 * %scale) %limb = 2;// Right Shoulder
			else if(%zLocation > %zWorldBox - 4.2 * %scale) %limb = 4;// Right Hand
		} 
		else
		{
			if(%zLocation > %zWorldBox - 3.825 * %scale) %limb = 3;// Left Shoulder
			else if(%zLocation > %zWorldBox - 4.2 * %scale) %limb = 5;// Left Hand
		}
	}
	else if(%zLocation > %zWorldBox - 4.6 * %scale) %limb = 6;
	else
	{
		if(%angle < 90) %limb = 7;// Right Leg
		else %limb = 8;// Left Leg
	}
	return %limb;
}

// functions for spawning blood effects
function doBloodExplosion(%position, %scale)
{
	%bloodExplosionProjectile = new Projectile()
	{
		datablock = bloodBurstFinalExplosionProjectile;
		initialPosition = %position;
	};
	MissionCleanup.add(%bloodExplosionProjectile);
	%bloodExplosionProjectile.setScale(%scale SPC %scale SPC %scale);
	%bloodExplosionProjectile.explode();
}
function doBloodDismemberExplosion(%position, %scale)
{
	%bloodDismemberProjectile = new Projectile()
	{
		datablock = bloodDismemberProjectile;
		initialPosition = %position;
	};
	MissionCleanup.add(%bloodDismemberProjectile);
	%bloodDismemberProjectile.setScale(%scale SPC %scale SPC %scale);
	%bloodDismemberProjectile.explode();
}
function doGibLimbsExplosion(%position, %scale)
{
	for(%i = 0; %i < 3; %i++)
	{
		%datablock = $RBloodGib[%i];
		%bloodGibLimbsProjectile = new Projectile()
		{
			datablock = %datablock;
			initialPosition = %position;
		};
		MissionCleanup.add(%bloodGibLimbsProjectile);
		%bloodGibLimbsProjectile.setScale(%scale SPC %scale SPC %scale);
		%bloodGibLimbsProjectile.explode();
	}
}

function Player::woundappearance(%obj,%type)
{
	if(%obj.getdataBlock().getName() $= "ZombieBoomerHoleBot") return;

	switch$(%type)
	{
		case "chest": 	//%chestbloodbot = new Player() { dataBlock = "EmptyPlayer"; };
						//%obj.chestbloodbot = %chestbloodbot;
						//%obj.mountobject(%chestbloodbot,2);
						//%chestbloodbot.mountImage(RBloodLargeImage,0);
		
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

						if(getRandom(1,1)) 
						{
							%obj.unhidenode("brain");
							if(getRandom(0,1)) %obj.unHideNode("headpart3");
							if(getRandom(0,1)) %obj.unHideNode("headpart1");
							if(getRandom(0,1)) %obj.unHideNode("headpart4");
							if(getRandom(0,1)) %obj.unHideNode("headpart5");
							if(getRandom(0,1)) %obj.unHideNode("headpart6");
							if(getRandom(0,1)) %obj.unHideNode("headskullpart1");
							if(getRandom(0,1)) %obj.unHideNode("headskullpart3");
							if(getRandom(0,1)) %obj.unHideNode("headpart1");
							if(getRandom(0,1)) %obj.unHideNode("headpart1");
							if(getRandom(0,1)) %obj.unHideNode("headpart1");
							if(getRandom(0,1)) %obj.unHideNode("headskullpart4");
							if(getRandom(0,1)) %obj.unHideNode("headskullpart5");
							if(getRandom(0,1)) %obj.unHideNode("headskullpart6");
						}
						else %obj.unhideNode("headstump");
						
													
		case "rightarm":	%obj.unhidenode("rarmSlim");
							%obj.setNodeColor("rarmSlim","1 0.5 0.5 1");

		case "leftarm":		%obj.unhidenode("larmSlim");
							%obj.setNodeColor("larmSlim","1 0.5 0.5 1");

		case "rightleg":	%obj.unhideNode("legstumpr");
							%obj.setNodeColor("legstumpr","1 0.5 0.5 1");

		case "leftleg":		%obj.unhideNode("legstumpl");
							%obj.setNodeColor("legstumpl","1 0.5 0.5 1");

		case "hip":			//%pantsbloodbot = new Player() { dataBlock = "EmptyPlayer"; };
							//%obj.pantsbloodbot = %pantsbloodbot;
							//%obj.mountobject(%pantsbloodbot,7);
							//%pantsbloodbot.mountImage(RBloodLargeImage,0);
		
							%obj.HideNode("pants");
							%obj.unHideNode("skelepants");
							%obj.unHideNode("pantswound");

							if(%obj.getClassName() $= "AIPlayer") %obj.setcrouching(1);
							%obj.nolegs = 1;
	}
}

function AIPlayer::woundappearance(%obj,%type) { Player::woundappearance(%obj,%type); }

function Armor::RBloodSimulate(%this, %obj, %position, %damagetype, %damage)
{
	//if(!%damageType.rBlood) %damage = %damage/4;
	%effectPosition = %position;
	if(%obj.lastDamaged < getSimTime())
	{
		for(%i = 0; %i < mCeil(%damage / 25); %i++)
		{			
			doBloodExplosion(%effectPosition, getWord(%obj.getScale(), 2));
			if($Pref::Server::L4BBlood::BloodDecals) %this.doSplatterBlood(%obj,5);
		}
		serverPlay3D("blood_impact" @ getRandom(1,4) @ "_sound", %effectPosition);
		%obj.lastDamaged = getSimTime()+50;
	}

	if(%damage >= %this.maxDamage/2 || %obj.limbShotgunStrike >= 2)
	{
		%limb = %obj.rgetDamageLocation(%position);
		%time = mClampF(%obj.getDamagePercent()*10, 0, 10);
		%obj.SapHealth(0,25);
		%obj.markLimbForDismember[%limb] = true;
	} 
	if(%damage > %this.maxDamage*2.5) %obj.markForGibExplosion = true;
}

$RBloodLimbString0 = "headskin helmet pointyhelmet flarehelmet scouthat bicorn cophat knithat plume triplume septplume visor shades gloweyes ballistichelmet constructionhelmet constructionhelmetbuds";
$RBloodLimbString1 = "armor bucket cape pack quiver tank";
$RBloodLimbString2 = "rarm rarmslim rhand_blood rarmcharger";
$RBloodLimbString3 = "larm larmslim larmsmall";
$RBloodLimbString4 = "rhook rhand rhandwitchclaws rhandwitch";
$RBloodLimbString5 = "lhand lhook lhand_blood lhandwitchclaws lhandwitch lhandsmall";
$RBloodLimbString6 = "";
$RBloodLimbString7 = "rshoe rpeg rshoe_blood";
$RBloodLimbString8 = "lshoe lpeg lshoe_blood";

package RBloodPackage
{
	function MiniGameSO::reset(%this, %client) 
	{
		Parent::reset(%this, %client);

		%currTime = getSimTime();
		if(%obj.lastResetTime + 5000 > %currTime) return;
		%minigame.lastResetTime = %currTime;
		
		if(isObject(DecalGroup)) DecalGroup.deleteAll();
	}

	function ProjectileData::radiusDamage(%this, %obj, %col, %distanceFactor, %pos, %damageAmt)
	{	
		if((vectorDist(%pos, %col.getHackPosition()) / getWord(%col.getScale(), 2)) < 5) %col.markForGibExplosion = true;
		
		Parent::radiusDamage(%this, %obj, %col, %distanceFactor, %pos, %damageAmt);
	}	

	function Armor::Damage(%this, %obj, %sourceObject, %position, %damage, %damageType)
	{						
		if(!$Pref::Server::L4BBlood::Enable || !%this.enableRBlood || %damage < %this.maxDamage/8 || %damageType == $DamageType::Lava || %damageType == $DamageType::Suicide) 
		return Parent::Damage(%this, %obj, %sourceObject, %position, %damage, %damageType);

		%rbloodPosition = %position;
		%scale = getWord(%obj.scale, 2);
		%randomString = ((getRandom() - 0.5) * 1 * %scale) SPC ((getRandom() - 0.5) * 1 * %scale) SPC ((getRandom() - 0.5) * 2 * %scale);
		if(%rbloodPosition $= "" || %rbloodPosition $= "0 0 0" || vectorDist(%rbloodPosition, %obj.getHackPosition()) > (1.5 * %scale)) %rbloodPosition = vectorAdd(%obj.getHackPosition(), %randomString);
		%obj.limbShotgunStrike++;
		if(%obj.lastHitTime+5 < getSimTime())
		{
			%obj.limbShotgunStrike = 0;
			%obj.lastHitTime = getSimTime();
		}
		%this.RBloodSimulate(%obj, %rbloodPosition, %damagetype, %damage);
		
		Parent::Damage(%this, %obj, %sourceObject, %position, %damage, %damageType);
	}

	function Armor::onDisabled(%this, %obj, %state)
	{
		Parent::onDisabled(%this, %obj, %state);
		
		%position = %obj.getHackPosition();
		if(%obj.markForGibExplosion)
		{
			%obj.SapHealth(0,25);			
			%obj.hideNode("ALL");
			%obj.schedule(50,delete);
			doBloodDismemberExplosion(%position, 1.5);
			if($Pref::Server::L4BBlood::BloodDecals) %this.doSplatterBlood(%obj,10);
			serverPlay3D("blood_explosion" @ getRandom(1,2) @ "_sound", %position);
			doGibLimbsExplosion(%position, getWord(%obj.scale, 2));
		}
		else
		for(%limb = 0; %limb <= 8; %limb++)
		{			
			if(%obj.markLimbForDismember[%limb])
			{
				for(%i = 0; %i < getWordCount($RBloodLimbString[%limb]); %i++) %obj.hideNode(getWord($RBloodLimbString[%limb], %i));
				
				doBloodExplosion(%position, 1.5);
				if($Pref::Server::L4BBlood::BloodDecals) %this.doSplatterBlood(%obj,5);
				serverPlay3D("blood_dismember" @ getRandom(1,4) @ "_sound", %position);				

				switch(%limb)
				{
					case 0: %obj.schedule(1,woundappearance,"head");//head
							%obj.schedule(1,stopAudio,0);
							%obj.SapHealth(0,15);
					case 1: %obj.woundappearance("chest");//chest
							%obj.SapHealth(0,15);
					case 2: %obj.schedule(1,woundappearance,"rightarm");//rightarm
					case 3: %obj.schedule(1,woundappearance,"leftarm");//leftarm
					case 4: //%obj.schedule(1,woundappearance,"rightarm");//righthand
					case 5: //%obj.schedule(1,woundappearance,"leftarm");//lefthand
					case 6: %obj.woundappearance("hip");//hip
							%obj.SapHealth(0,15);
					case 7: %obj.schedule(1,woundappearance,"rightleg");//rightleg
					case 8: %obj.schedule(1,woundappearance,"leftleg");//leftleg
				}
			}
		}
	}
};
activatePackage(RBloodPackage);