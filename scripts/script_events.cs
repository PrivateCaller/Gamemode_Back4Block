function fxDTSBrick::RandomizeZombieSpecial(%obj)
{
	%type[ %ntype++ ] = "ZombieChargerHoleBot";
	%type[ %ntype++ ] = "ZombieBoomerHoleBot";
	%type[ %ntype++ ] = "ZombieSpitterHoleBot";
	%type[ %ntype++ ] = "ZombieHunterHoleBot";
	%type[ %ntype++ ] = "ZombieSmokerHoleBot";
	%type[ %ntype++ ] = "ZombieJockeyHoleBot";
	%type = %type[ getRandom( 1, %ntype ) ];
	%obj.hBotType = %type;	
}
registerOutputEvent(fxDTSBrick, "RandomizeZombieSpecial");

function fxDTSBrick::RandomizeZombieUncommon(%obj)
{
	%type[ %ntype++ ] = "ZombieConstructionHoleBot";
	%type[ %ntype++ ] = "ZombieFallenHoleBot";
	%type[ %ntype++ ] = "ZombieCedaHoleBot";
	%type[ %ntype++ ] = "ZombieSoldierHoleBot";
	%type[ %ntype++ ] = "MudZombieHoleBot";
	%type[ %ntype++ ] = "ZombieClownHoleBot";
	%type[ %ntype++ ] = "ZombieJimmyHoleBot";
	%type[ %ntype++ ] = "ToxicZombieHoleBot";
	%type[ %ntype++ ] = "ZombiePirateHoleBot";

	//if($AddOn__Bot_Zombie_L4B2_EXT2 $= "1")
	//{
	//	%type[ %ntype++ ] = "ToxicZombieHoleBot";
	//	%type[ %ntype++ ] = "ZombieNaziHoleBot";
	//	%type[ %ntype++ ] = "ZombieSpaceHoleBot";
	//	%type[ %ntype++ ] = "BurningZombieHoleBot";
	//	%type[ %ntype++ ] = "HeadcrabZombieHoleBot";
	//}
//
	//if($AddOn__Bot_SkeletonRev $= "1")
	//%type[ %ntype++ ] = "RandomSkeletonHoleBot";

	%type = %type[ getRandom( 1, %ntype ) ];
	%obj.hBotType = %type;

}
registerOutputEvent(fxDTSBrick, "RandomizeZombieUncommon");

function fxDTSBrickData::onZombieTouch(%data,%obj,%player)
{
	$InputTarget_["Self"] = %obj;

	if(isObject(%player.client))
	{
	$InputTarget_["Player"] = %player;
	$InputTarget_["Client"] = %player.client;
	}

	if ( $Server::LAN )
	{
		$InputTarget_["MiniGame"] = getMiniGameFromObject (%player.client);
	}
	else if ( getMiniGameFromObject(%obj) == getMiniGameFromObject(%player.client) )
	{
		$InputTarget_["MiniGame"] = getMiniGameFromObject (%obj);
	}
	else
	{
		$InputTarget_["MiniGame"] = 0;
	}

	if ( !isObject(%player.client) )
	{
		$InputTarget_["Bot"] = %player;
	}
}
registerInputEvent ("fxDTSBrick", "onZombieTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB 
	"Client GameConnection" TAB "MiniGame MiniGame");

function fxDTSBrickData::onTankTouch(%data,%obj,%player)
{
	fxDTSBrickData::onZombieTouch(%data,%obj,%player);
}
registerInputEvent ("fxDTSBrick", "onTankTouch", "Self fxDTSBrick" TAB "Player Player" TAB "Bot Bot" TAB 
	"Client GameConnection" TAB "MiniGame MiniGame");

function Player::bigZombieMelee(%obj)
{
	%this = %obj.getdataBlock();
	%oscale = getWord(%obj.getScale(),2);
	%mask = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	initContainerRadiusSearch(%obj.getEyePoint(),10,%mask);
	while(%hit = containerSearchNext())
	{
		if(%hit == %obj)
		continue;

		%obscure = containerRayCast(%obj.getEyePoint(),vectorAdd(%hit.getPosition(),"0 0 1.9"),$TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::FxBrickObjectType, %obj);
		if(isObject(%obscure))
		continue;

		%line = vectorNormalize( vectorSub( %obj.getposition(), %hit.getposition()));
		%dot = vectorDot( %obj.getEyeVector(), %line );

		if(ContainerSearchCurrRadiusDist() < 2 && %dot < -0.5)
		{
			if(%hit.getType() & $TypeMasks::PlayerObjectType && checkHoleBotTeams(%obj,%hit))
			{
				if(%hit.getstate() $= "Dead")
				continue;

				%obj.playaudio(3,%this.hBigMeleeSound);
				%obj.playthread(1,"activate2");
				%hit.applyimpulse(%hit.getposition(),vectoradd(vectorscale(%obj.getforwardvector(),2000),"0 0 500"));
				%hit.damage(%obj.hFakeProjectile, %hit.getposition(), $Pref::Server::L4B2Bots::SpecialsDamage*%oScale, %obj.hDamageType);

				%p = new Projectile()
				{
					dataBlock = "BigZombieHitProjectile";
					initialPosition = %hit.getPosition();
					sourceObject = %obj;
					client = %obj.client;
				};
				MissionCleanup.add(%p);
				%p.explode();

				%c = new Projectile()
				{
					dataBlock = "pushBroomProjectile";
					initialPosition = %hit.getPosition();
					sourceObject = %obj;
					client = %obj.client;
				};
				MissionCleanup.add(%c);
				%c.explode();
				%c.setScale("2 2 2");
			}
			if(%hit.getType() & $TypeMasks::VehicleObjectType)
			{
				%obj.playaudio(3,%this.hBigMeleeSound);
				%obj.playthread(2,"activate2");

				%muzzlepoint = vectorSub(%obj.getHackPosition(),"0 0 0.5");
				%muzzlevector = vectorScale(%obj.getEyeVector(),2.5);
				%muzzlepoint = VectorAdd (%muzzlepoint, %muzzlevector);
				%hit.setTransform (%muzzlepoint @ " " @ rotFromTransform(%hit.getTransform()));
				%impulse = VectorNormalize(VectorAdd (%obj.getEyeVector(), "0 0 0.2"));
				%force = %hit.getDataBlock ().mass * 25;
				%scaleRatio = getWord (%obj.getScale (), 2) / getWord (%hit.getScale (), 2);
				%force *= %scaleRatio;
				%impulse = VectorScale (%impulse, %force);
				%hit.schedule(50,applyImpulse,%hit.getPosition(),%impulse);
				%hit.damage(%obj.hFakeProjectile, %hit.getposition(), 50*getWord(%obj.getScale(),0), $DamageType::Tank);

				%c = new Projectile()
				{
					dataBlock = "pushBroomProjectile";
					initialPosition = %hit.getPosition();
					sourceObject = %obj;
					client = %obj.client;
				};
				MissionCleanup.add(%c);
				%c.explode();
				%c.setScale("2 2 2");
			}
		}
	}
}

function Player::ToxifyHealth ( %Player, %amt )
{
	if ( %Player.getDamagePercent() >= 1.0 )
	{
		return;
	}

	if ( %amt > 0.0 && %Player.isToxic == 0)
	{
		%Player.setDamageLevel (%Player.getDamageLevel() - %amt);
		if(!%Player.Toxified)
		{
		   schedule(500,0,ToxicityE,%Player);
		   %Player.Toxified = 1;
		}
	}
	else if(%Player.isToxic == 0)
	{
		%Player.Damage (%Player.hFakeProjectile, %Player.getPosition(), %amt * -1, $DamageType::Default);
		if(!%Player.Toxified)
		{
			schedule(500,0,ToxicityE,%Player);
			%Player.Toxified = 1;
		}
	}
}
registerOutputEvent ("Player", "ToxifyHealth", "int -1000 1000 25");
registerOutputEvent ("Bot", "ToxifyHealth", "int -1000 1000 25");

function ToxicityE(%Player)
{
      if(isObject(%Player) && %Player.getState() !$= "Dead" && %Player.Toxified)//For bots/Players that aren't part of the team the obj is in   
      {
         if(%Player.ToxicityCount <= "16")
         {
            %Player.ToxicityCount++;
            %Player.ToxicESchedule = schedule(500,0,ToxicityE,%Player);
            %Player.mountImage(SpitAcidStatusPlayerImage, 3);

			%Player.damage(%Player, %Player.getposition(), 2, $DamageType::Toxic);

            if(%Player.hCanDistract)
            %Player.hRunAwayFromPlayer(%Player);

            else if(!isObject(%Player))
            return;
         }
         else
         {
            %Player.ToxicityCount = 0;
            if(isObject(%Player.getMountedImage(3)) && %Player.getMountedImage(3).getName() $= "SpitAcidStatusPlayerImage")
            %Player.unMountImage(3);

            %Player.Toxified = 0;
         }
      }
}