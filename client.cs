function AddMeleeBind(%division, %name, %command)
{
	for(%i=0;%i<$remapCount;%i++)
	{
		if($remapDivision[%i] $= %division)
		{
			%foundDiv = 1;
			continue;
		}
		if(%foundDiv && $remapDivision[%i] !$= "")
		{
			%position = %i;
			break;
		}
	}
	if(!%foundDiv)
	{
		error("Division not found: " @ %division);
		return;
	}
	if(!%position)
	{
		$remapName[$remapCount] = %name;
		$remapCmd[$remapCount] = %command;
		$remapCount++;
		return;
	}
	for(%i=$remapCount;%i>%position;%i--)
	{
		$remapDivision[%i] = $remapDivision[%i - 1];
		$remapName[%i] = $remapName[%i - 1];
		$remapCmd[%i] = $remapCmd[%i - 1];
	}
	$remapDivision[%position] = "";
	$remapName[%position] = %name;
	$remapCmd[%position] = %command;
	$remapCount++;
}

function do_secondaryMelee(%x)
{   if(%x)
    commandToServer('SecondaryMelee');
}

if(!$SecondaryMeleeBound)
{
	AddMeleeBind("Action", "Melee", "do_secondaryMelee");
	$SecondaryMeleeBound = true;
}