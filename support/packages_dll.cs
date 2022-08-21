function onObjectCollisionTest(%obj, %col)
{
	if(!isObject(%obj)|| !isObject(%col))
	return;

	%oscale = getWord(%obj.getScale(),2);
	%force = vectorDot(%obj.getVelocity(), %obj.getForwardVector());
	
	if(%obj.getType() & $TypeMasks::PlayerObjectType && %col.getType() & $TypeMasks::PlayerObjectType) 
	{
		if(%obj.getdataBlock().getName().isSurvivor && %col.getdataBlock().getName().isSurvivor)
		return false;
		
		if(vectordist(%obj.getposition(),%col.getposition()) < 6)
		if(%col.getdataBlock().getName() !$= "ZombieTankHoleBot" && %obj.getdataBlock().getName() $= "ZombieChargerHoleBot" && %oScale >= 1.1 && %force > 20 && %obj.hEating != %col)
		{
			if(%col.getdatablock().getName() !$= "ZombieChargerHoleBot")
			{				
				%obj.playaudio(3,"charger_smash_sound");			
				%forcecalc = %force/20;
				%obj.spawnExplosion(pushBroomProjectile,%forcecalc SPC %forcecalc SPC %forcecalc);
				%obj.playthread(2,"activate2");
				%normVec = VectorNormalize(vectorAdd(%obj.getForwardVector(),"0" SPC "0" SPC "0.25"));
				%eye = vectorscale(%normVec,%force/1.5);
				%col.setvelocity(%eye);

				if(checkHoleBotTeams(%obj,%col))
				%col.damage(%obj.hFakeProjectile, %col.getposition(),0.25, %obj.hDamageType);
			}
			return false;
		}
	}
	return true;
}

function onGameTick(%dt)
{
	
}