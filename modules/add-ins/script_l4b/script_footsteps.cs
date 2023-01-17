function Armor::FootstepLoop(%this,%obj)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead" || !%obj.getDatablock().isSurvivor) return;
	
	cancel(%obj.FootstepLoop);
	%obj.FootstepLoop = %this.schedule(320,FootstepLoop,%obj);

	%pos = %obj.getPosition();
	%vel = %obj.getVelocity();
	%isground = footplacecheck(%obj); //check for solid ground
	%localforwardspeed = mCeil(vectorDot(%obj.getVelocity(), %obj.getForwardVector()));

	if(mAbs(getWord(%vel,0)) < 0.5 && mAbs(getWord(%vel,1)) < 0.5 || isObject(%obj.getObjectMount()) || %obj.isStepProning || !%isGround || %obj.isSwimming || %obj.isStepJetting) //if hasn't moved, or is crouching, or is midair, or is swimming, or is jetting
	{
		cancel(%obj.FootstepLoop);
		%obj.FootstepLoop = %this.schedule(320,FootstepLoop,%obj);
	}
	else if(mAbs(getWord(%vel,0)) > 3 || mAbs(getWord(%vel,1)) > 3) serverplay3d("movestep" @ getRandom(1,4) @ "_sound",%pos);
	else serverplay3d("movequietstep" @ getRandom(1,4) @ "_sound",%pos);

	if(%localforwardspeed < 0)
	{
		cancel(%obj.FootstepLoop);
		%obj.FootstepLoop = %this.schedule(240,FootstepLoop,%obj);
	}

	if(%obj.lastStepTime < getSimTime())
	{
		%obj.lastStepTime = getSimTime()+1250;

		if(%obj.getdataBlock().isSurvivor && isObject(getMiniGameFromObject(%obj)))
		{
			if(getword(%obj.getvelocity(),2) < -15)
			{
				%obj.playthread(2,"side");
				L4B_SpazzZombie(%obj,0);
				if(!%obj.isFalling)
				{
					%obj.playaudio(0,"survivor_pain_high1_sound");
					%obj.isFalling = 1;
				}
			}
			else if(%obj.isFalling)
			{
				L4B_SpazzZombie(%obj,15);
				%obj.playthread(2,"root");
				%obj.isFalling = 0;
			}

			%obj.AreaZoneNum = 0;
			%survivorsfound = 1;
			%obj.survivorAllyCount = %survivorsfound;

			InitContainerRadiusSearch(%obj.getPosition(), 25, $TypeMasks::FxBrickObjectType | $TypeMasks::PlayerObjectType);
			while(%scan = containerSearchNext())
			{
				if(%scan == %obj || VectorDist(getWord(%obj.getPosition(),2), getWord(%scan.getPosition(),2)) > 5) continue;
				if(%scan.getType() & $TypeMasks::PlayerObjectType && %scan.getdataBlock().isSurvivor)
				{
					%survivorsfound++;
					%obj.survivorAllyCount = %survivorsfound;
				} 
			}
		}	
	}
}

function footplacecheck(%obj)
{
	%pos0 = %obj.getPosition();
	%pos1 = %obj.getHackPosition();
	for(%a=0;%a<2;%a++) //ensure he's not stuck in the ground, with 2 checks
	{
		initContainerBoxSearch(%pos[%a], "1.25 1.25 0.1", $TypeMasks::fxBrickObjectType|$Typemasks::TerrainObjectType);
		%col[%a] = containerSearchNext();
	}
	return isObject(%col0) && %col0 != %col1; //if on the ground, and ground is not overlapping him
}

package Footsteps
{
	function Armor::onEnterLiquid(%this,%obj,%coverage,%type)
	{
		Parent::onEnterLiquid(%this,%obj,%coverage,%type);
		%obj.isSwimming = true; //note when underwater
	}
	function Armor::onLeaveLiquid(%this,%obj,%coverage,%type)
	{
		Parent::onLeaveLiquid(%this,%obj,%coverage,%type);
		%obj.isSwimming = false; //note when out of water
	}

	function Armor::onAdd(%this,%obj)
	{
		Parent::onAdd(%this,%obj);
		%obj.FootstepLoop = %this.schedule(320,FootstepLoop,%obj);
	}
	function Armor::onTrigger(%this,%obj,%slot,%val)
	{
		Parent::onTrigger(%this,%obj,%slot,%val);
		if(%slot == 3) %obj.isStepProning = %val;
		if(%slot == 4 && %this.canJet) %obj.isStepJetting = %val;
	}
};
activatepackage(Footsteps);