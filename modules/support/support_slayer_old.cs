    //Old support for Slayer
	function MiniGameSO::Reset(%minigame,%client)
	{
		Parent::Reset(%minigame,%client);

		if(isObject(%minigameteam = %minigame.teams))
		{
			%teamsammount = %minigameteam.getCount();
			for(%i = 0; %i < %teamsammount; %i++)
			{				
				%teams = %minigameteam.getObject(%i);
				
				if(%teams.name $= "Survivors")
				%survivorteam = %teams;

				for(%j=0;%j<%minigame.numMembers;%j++)
				{
					if(isObject(%infectedclient = %minigame.member[%j]) && %infectedclient.getClassName() $= "GameConnection" && %infectedclient.isInInfectedTeam)
					{
						%survivorteam.addMember(%infectedclient, "Reset Minigame", 1, 1);
						%infectedclient.isInInfectedTeam = 0;
					}
				}
			}
		}
    }

    function holeZombieInfect(%obj, %col)
	{			
		if(isObject(%minigameteam = getMinigameFromObject(%col).teams) && %col.getClassName() $= "Player")
		{
			%minigame = getMinigameFromObject(%col);
			%minigame.L4B_PlaySound("survivor_turninfected" @ getRandom(1,3) @ "_sound",%col.client);

			%teamsammount = %minigameteam.getCount();
			for(%i = 0; %i < %teamsammount; %i++)
    		{
        		%teams = %minigameteam.getObject(%i);
				if(%teams.name $= "Infected") %infectedteam = %teams;

				if(isObject(%infectedteam))
				{
					%infectedteam.addMember(%col.client, "No Immunity", 1, 1);
					%clName = %col.client.name;

					%col.client.isInInfectedTeam = 1;
					L4B_checkAnyoneNotZombie(%col,%minigame);
				 	break;
				}
			}
		}

        Parent::holeZombieInfect(%obj, %col);
    }

	function L4B_CheckAnyoneNotZombie(%obj,%minigame)
	{
		%survivorteam = "";
		for(%i = 0; %i < %teamsammount = %minigame.teams.getCount(); %i++)
		{
			%teams = %minigame.teams.getObject(%i);
			if(%teams.name $= "Survivors")
			{
				%survivorteam = %teams;
				break;
			}
		}
		if(%survivorteam !$= "" && %survivorteam.numMembers <= 0) %minigame.endRound(%minigame.victoryCheck_Lives());
	}