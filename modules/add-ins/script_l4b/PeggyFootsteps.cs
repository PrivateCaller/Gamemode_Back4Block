
//+++ When any lifeform spawns, create a footstep loop.
deactivatepackage(peggsteps);
package peggsteps
{
	function Armor::onNewDatablock(%this, %obj)
	{
		// creatures like horses won't make footsteps, but other AI will
		if ( %this.rideable )	return parent::onNewDatablock(%this, %obj);
		%obj.touchcolor = "";
		%obj.surface = parseSoundFromNumber($Pref::Server::PF::defaultStep, %obj);
		%obj.isSlow = 0;
		%obj.peggstep = schedule(50,0,PeggFootsteps,%obj);
		return parent::onNewDatablock(%this, %obj);
	}

	// I don't actually know how this built-in function works, but I know this will work.
	function onMissionEnded(%this, %a, %b, %c, %d)
	{
		$PFGlassInit = false;
		$PFRTBInit = false;
		return parent::onMissionEnded(%this, %a, %b, %c, %d);
	}
};
activatepackage(peggsteps);


//--------------------------------------------------------------------------------------
//		Footstep Playback:
//--------------------------------------------------------------------------------------

//+++ Landing from a fall
function Armor::onLand(%data, %obj, %horiz)
{
	if ( !$Pref::Server::PF::landingFX ) return;

	if ( %horiz > $Pref::Server::PF::minLandSpeed + 16 ) serverplay3d(LandHeavy_Sound, %obj.getHackPosition());
	else if ( %horiz > $Pref::Server::PF::minLandSpeed + 8 ) serverplay3d($LandMedium[getRandom(1,3)], %obj.getHackPosition());	
	else if ( %horiz >= $Pref::Server::PF::minLandSpeed ) serverplay3d($LandLite[getRandom(1,3)], %obj.getHackPosition());
	
}

//+++ Drop some rad peggstep noise in here!
function PeggFootsteps(%obj, %lastVert)
{
	cancel(%obj.peggstep);
	if($Pref::Server::PF::footstepsEnabled == 1 && isObject(%obj))
	{
		if ( %obj.isMounted() )
		{
			%obj.peggstep = schedule(50,0,PeggFootsteps,%obj);
			return;
		}
		//! Ripped from Hata's support_footstep.cs
		%vel = %obj.getVelocity();
		%vert = getWord(%vel, 2);
		%horiz = vectorLen(setWord(%vel, 2, 0));

		%pos = %obj.getPosition();
		initContainerBoxSearch(%pos, "1.25 1.25 0.1", $TypeMasks::fxBrickObjectType | $Typemasks::TerrainObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxPlaneObjectType);
		%col = containerSearchNext();
		//	echo(%type);
		if( isObject(%col) )
		{
			%type = %col.getClassName();
			if ( %type $= "fxDTSbrick" && %col.isRendering() )
			{
					%obj.lastBrick =  %col;
					// by default, the surface isn't decided yet, and will be decided by the color
					%obj.touchColor = getColorIDTable(%col.getColorId());
					%obj.surface = "";
					// check to see if there is a custom sound based on the brick's special FX
					if ( $Pref::Server::PF::brickFXSounds::enabled )
					{
						// if there's a color fx
						switch ( %col.getColorFxID() )
						{
							case 1:
								%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::pearlStep, %obj);
								%obj.touchColor = "";
							case 2:
								%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::chromeStep, %obj);
								%obj.touchColor = "";
							case 3:
								%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::glowStep, %obj);
								%obj.touchColor = "";
							case 4:
								%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::blinkStep, %obj);
								%obj.touchColor = "";
							case 5:
								%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::swirlStep, %obj);
								%obj.touchColor = "";
							case 6:
								%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::rainbowStep, %obj);
								%obj.touchColor = "";
						}
						// if there's a shape fx, which takes priority over color fx
						if ( %col.getShapeFxID() )
						{
							%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::unduloStep, %obj);
							%obj.touchColor = "";
						}
						// if the preference for the shape or color fx is default, then just play the regular sound that would be made based on color
						if ( %obj.surface $= "color" )
						{
							%obj.touchColor = getColorIDTable(%col.getColorId());
						}
					}
					// check to see if the brick has an event based custom sound
					if ( %col.customStep !$= "" )
					{
						%obj.touchColor = "";
						%obj.surface = %col.customStep;
					}
			}
			else if ( %type $= "fxPlane" )
			{
				%obj.touchColor = "";
				%obj.surface = parseSoundFromNumber($Pref::Server::PF::terrainStep, %obj);
			}
			else if ( %type $= "WheeledVehicle" || %type $= "FlyingVehicle" )
			{
				%obj.touchColor = "";
				%obj.surface = parseSoundFromNumber($Pref::Server::PF::vehicleStep, %obj);
			}
			else
			{
				%obj.touchColor = "";
				%obj.surface = parseSoundFromNumber($Pref::Server::PF::defaultStep, %obj);
			}
			if ( %obj.getWaterCoverage() > 0 )
			{
				%obj.surface = "water";
				%obj.touchColor = "";
			}
			if ( !%isGround && mAbs(%lastVert) > $Pref::Server::PF::minLandSpeed * getWord(%obj.getScale(), 1) && $Pref::Server::PF::landingFX )
			{
				%obj.getDatablock().onLand(%obj, mAbs(%lastVert));
			}
			%isGround = true;
		}
		else
		{
			%isGround = false;
		}

		%obj.isSlow = ( mAbs(%horiz) < $Pref::Server::PF::runningMinSpeed * getWord(%obj.getScale(), 0) || %obj.isCrouched() );

		if( %obj.getWaterCoverage() > 0 && $Pref::Server::PF::waterSFX == 1 && mAbs(%horiz) > 0.1 && !%isGround )
		{
			%obj.touchColor = "";
			%obj.surface = "under water";
			serverplay3d(checkPlayback(%obj), %obj.getHackPosition());
			%obj.peggstep = schedule(500 * getWord(%obj.getScale(), 0), 0, PeggFootsteps, %obj);
		}
		else if( mFloor(%horiz) == 0 || !%isGround )
		{
			%obj.peggstep = schedule(50, 0, PeggFootsteps, %obj, %vert);
		}
		else if ( %isGround && mAbs(%horiz) > 0 )
		{
			%obj.peggstep = schedule(320 * getWord(%obj.getScale(), 0), 0, PeggFootsteps, %obj, %vert);
			serverplay3d(checkPlayback(%obj), %obj.getHackPosition());
		}

		// Thanks Queuenard!
		%obj.peggstep = schedule(1000, 0, PeggFootsteps, %obj);
		return;
	}
}