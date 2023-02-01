luaexec("./script_survivor.lua");
if(LoadRequiredAddOn("Support_BotHolePlus") == $Error::None) exec("./bots/survivorally.cs");

function SurvivorPlayer::L4BAppearance(%this,%obj,%client) 
{ 
	Parent::L4BAppearance(%this,%obj,%client); 

	switch(%client.Pack)
	{
		case 0:
		default: %obj.unHideNode("pack");
				 %obj.setNodeColor("pack",%client.PackColor);
	}

	switch(%client.secondPack)
	{
		case 0:
		default:
	}

	//Don't bother if they already have the hat
	if((isObject(%obj.getmountedImage(2)) && %obj.getmountedImage(2).getName() $= $L4BHat[%client.hat] @ "image") || %obj.limbDismemberedLevel[0]) return;

	switch$(%client.hat)
	{
		case 1: if(%client.accent)
				{
					%obj.mountImage("helmetimage",2,1,addTaggedString(luacall(getcolorname,%client.hatColor)));	
					%obj.currentHat = "helmet";
				}		
				else
				{
					%obj.mountImage("hoodieimage",2,1,addTaggedString(luacall(getcolorname,%client.hatColor)));	
					%obj.currentHat = "hoodie";
				}
		default: %obj.mountImage($L4BHat[%client.hat] @ "image",2,1,addTaggedString(luacall(getcolorname,%client.hatColor)));
				 %obj.currentHat = $L4BHat[%client.hat];
	}
}

function SurvivorPlayer::RbloodDismember(%this,%obj,%limb,%doeffects,%position)
{
	if(%limb != 7 || %limb != 8) Parent::RbloodDismember(%this,%obj,%limb,%doeffects,%position);
}

function SurvivorPlayer::onImpact(%this, %obj, %col, %vec, %force)
{
	luacall(Survivor_FallDamage,%obj,%vec,%force);
	Parent::onImpact(%this, %obj, %col, %vec, %force);
}

function SurvivorPlayer::onEnterLiquid(%this, %obj, %cov, %type)
{
	cancel(%obj.oxygenTick);
	%obj.oxygenTick = %obj.schedule(2500, oxygenTick);	
	Parent::onEnterLiquid(%this, %obj, %cov, %type);
}
function SurvivorPlayer::onLeaveLiquid(%this, %obj, %type)
{
	%obj.schedule(150,checkIfUnderwater);	
	Parent::onLeaveLiquid(%this, %obj, %cov, %type);
}

function SurvivorPlayer::onAdd(%this,%obj)
{	
	Parent::onAdd(%this,%obj);	
	if(isObject(%obj.client)) %obj.client.deletel4bMusic("Music");
}

function SurvivorPlayer::onTrigger (%this, %obj, %triggerNum, %val)
{
	Parent::onTrigger (%this, %obj, %triggerNum, %val);

	if(%obj.getClassName() $= "Player" && %obj.getState() !$= "Dead")
	{		
		switch(%triggerNum)
		{
			case 0: luacall(Survivor_LeftClick,%val,%obj);
			case 4: luacall(Survivor_Rightclick,%obj);
			default:
		}
	}
}

function SurvivorPlayer::onNewDataBlock(%this,%obj)
{	
	Parent::onNewDataBlock(%this,%obj);

	if(!isObject(%obj.billboardbot))
	{
		%obj.billboardbot = new Player() 
		{ 
			dataBlock = "EmptyPlayer";
			source = %obj;
			slotToMountBot = 5;
			lightToMount = "blankBillboard";
		};
	}
	
	%obj.SurvivorStress = 0;
	%obj.hType = "Survivors";
}

function SurvivorPlayer::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{	
	if(luacall(Survivor_DownCheck,%obj,%damage,%damageType)) return;
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function SurvivorPlayer::onDamage(%this,%obj,%delta)
{
	Parent::onDamage(%this,%obj,%delta);
	luacall(Survivor_DamageCheck,%obj,%delta);
}

function SurvivorPlayerAiming::onTrigger (%this, %obj, %triggerNum, %val) { SurvivorPlayer::onTrigger (%this, %obj, %triggerNum, %val); }

function SurvivorPlayerAiming::onEnterLiquid(%this, %obj, %cov, %type) { SurvivorPlayer::onEnterLiquid(%this, %obj, %cov, %type); }

function SurvivorPlayerAiming::onLeaveLiquid(%this, %obj, %type) { SurvivorPlayer::onLeaveLiquid(%this, %obj, %cov, %type); }

function SurvivorPlayerAiming::onImpact(%this, %obj, %col, %vec, %force) { SurvivorPlayer::onImpact(%this, %obj, %col, %vec, %force); }

function SurvivorPlayerAiming::onNewDataBlock(%this,%obj) { SurvivorPlayer::onNewDataBlock(%this,%obj); }

function SurvivorPlayerAiming::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc) { SurvivorPlayer::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc); }

function SurvivorPlayerAiming::onDamage(%this,%obj,%delta) { SurvivorPlayer::onDamage(%this,%obj,%delta); }

function SurvivorPlayerAiming::L4BAppearance(%this,%obj,%client) { SurvivorPlayer::L4BAppearance(%this,%obj,%client); }

function SurvivorPlayerAiming::RbloodDismember(%this,%obj,%limb,%doeffects,%position) { SurvivorPlayer::RbloodDismember(%this,%obj,%limb,%doeffects,%position); }

function SurvivorPlayerDowned::L4BAppearance(%this,%obj,%client) { SurvivorPlayer::L4BAppearance(%this,%obj,%client); }

function SurvivorPlayerDowned::onImpact(%this, %obj, %col, %vec, %force) { SurvivorPlayer::onImpact(%this, %obj, %col, %vec, %force); }

function SurvivorPlayerDowned::onDisabled(%this,%obj) 
{
	Parent::onDisabled(%this,%obj);
	
	if(isObject(%client = %obj.client))
	{
		%client.delayMusicTime = getSimTime();
		%client.l4bMusic("musicData_L4D_death",false,"Music");
		%client.deletel4bMusic("Stinger1");
		%client.deletel4bMusic("Stinger2");
		%client.deletel4bMusic("Stinger3");
	}	 
	
	if(%obj.getWaterCoverage() == 1)
	{
		%obj.playaudio(0,"survivor_death_underwater" @ getRandom(1, 2) @ "_sound");
		%obj.emote(oxygenBubbleImage, 1);
		serverPlay3D("drown_bubbles_sound",%obj.getPosition());
		serverPlay3D("die_underwater_bubbles_sound",%obj.getPosition());
	}
	else %obj.playaudio(0,"survivor_death" @ getRandom(1, 8) @ "_sound");

	if(isObject(%obj.billboardbot.lightToMount)) %obj.billboardbot.delete();
}

function SurvivorPlayer::onDisabled(%this,%obj,%state) { SurvivorPlayerDowned::onDisabled(%this,%obj,%state); }

function SurvivorPlayerAiming::onDisabled(%this,%obj,%state) { SurvivorPlayerDowned::onDisabled(%this,%obj,%state); }

function SurvivorPlayerDowned::RbloodDismember(%this,%obj,%limb,%doeffects,%position) { SurvivorPlayer::RbloodDismember(%this,%obj,%limb,%doeffects,%position); }

function SurvivorPlayerDowned::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc)
{
	if(!%obj.isBeingStrangled) %damage = %damage/3.25;//Make the downed player last a little longer if they aren't pinned	
	Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType,%damageLoc);
}

function SurvivorPlayerDowned::onDamage(%this, %obj, %delta)
{
	Parent::onDamage(%this, %obj, %delta);
	if(%delta > %this.maxDamage/15 && %obj.getState () !$= "Dead") %obj.playaudio(0, "survivor_painhigh" @ getRandom(1, 4) @ "_sound");
}

function SurvivorPlayerDowned::onNewDataBlock(%this,%obj)
{	
	
	if(!%obj.hEater) %obj.playthread(0,sit);
	%obj.lastcry = getsimtime();
	%obj.playaudio(0,"survivor_pain_high1_sound");
	%this.DownLoop(%obj);

	if(isObject(%obj.billboardbot.lightToMount)) %obj.billboardbot.lightToMount.setdatablock("incappedBillboard");
	
	if(%obj.getClassName() $= "Player" && isObject(%minigame = getMinigameFromObject(%obj)))
	{		
		%minigame.L4B_ChatMessage("<color:FFFF00><bitmapk:Add-Ons/Gamemode_Left4Block/modules/add-ins/player_l4b/icons/ci_down>" SPC %obj.client.name,"victim_needshelp_sound",true);
		%minigame.checkLastManStanding();
	}
	
	Parent::onNewDataBlock(%this,%obj);
}

function SurvivorPlayerDowned::DownLoop(%this,%obj)
{ 
	if(isobject(%obj) && %obj.getstate() !$= "Dead" && %obj.getdataBlock().isDowned)
	{
		if(!%obj.isBeingSaved)
		{
			%obj.addhealth(-5);
			%obj.setdamageflash(0.25);

			if(%obj.lastcry+10000 < getsimtime())
			{
				%obj.lastcry = getsimtime();
				%obj.playaudio(0,"survivor_pain_high1_sound");
				%obj.playthread(3,"plant");
			}			
		}
	
		cancel(%obj.downloop);
		%obj.downloop = %this.schedule(1000,DownLoop,%obj);
	}
	else return;
}

function Player::sapHealth(%obj,%threshold)
{
	if(isObject(%obj) && %obj.getState() !$= "Dead" && %obj.getDamageLevel() > 50 && (%obj.getDataBlock().maxDamage-%obj.getDamageLevel()) >= %threshold)
	{
		%obj.addhealth(-0.5);		
		%obj.playthread(3,"plant");
		%obj.playaudio(0,"norm_cough" @ getrandom(1,3) @ "_sound");

		cancel(%obj.sapHealthSchedule);
		%obj.sapHealthSchedule = %obj.schedule(getRandom(2000,3000),sapHealth,%threshold);
	}
}
