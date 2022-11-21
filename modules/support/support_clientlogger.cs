$L4B_lastclientsloggedtime = getSimTime();    

function L4B_storeClientSnapshots()
{
	if(getSimTime() - $L4B_lastclientsloggedtime < 60000) return;
	$L4B_lastclientsloggedtime = getSimTime();

    echo("Storing" SPC ClientGroup.getCount() SPC "clients...");
    %root_object = JettisonObject();
    for(%i = 0; %i < ClientGroup.getCount(); %i++)
	if(isObject(%snapshot = ClientGroup.getObject(%i)))
    {
    	%file = new fileObject();
		%file.openForWrite("config/server/Left4Block/loggedplayers/" @ %snapshot.BL_ID @ ".txt");

        %file.writeLine(%snapshot.name);
		%file.writeLine(%snapshot.decalName);
		%file.writeLine(%snapshot.faceName);		
		%file.writeLine(%snapshot.accent);
		%file.writeLine(%snapshot.hat);
		%file.writeLine(%snapshot.accent);
		%file.writeLine(%snapshot.chest);
		%file.writeLine(%snapshot.pack);
		%file.writeLine(%snapshot.secondPack);
		%file.writeLine(%snapshot.larm);
		%file.writeLine(%snapshot.lhand);
		%file.writeLine(%snapshot.rarm);
		%file.writeLine(%snapshot.rhand);
		%file.writeLine(%snapshot.hip);
		%file.writeLine(%snapshot.lleg);
		%file.writeLine(%snapshot.rleg);		
		%file.writeLine(%snapshot.headColor);
		%file.writeLine(%snapshot.chestColor);
		%file.writeLine(%snapshot.hipColor);
		%file.writeLine(%snapshot.larmColor);
		%file.writeLine(%snapshot.rarmColor);
		%file.writeLine(%snapshot.lhandColor);
		%file.writeLine(%snapshot.rhandColor);		
		%file.writeLine(%snapshot.llegColor);
		%file.writeLine(%snapshot.rlegColor);		
		%file.writeLine(%snapshot.accentColor);
		%file.writeLine(%snapshot.hatColor);
		%file.writeLine(%snapshot.packColor);
		%file.writeLine(%snapshot.secondPackColor);

		%file.close();
		%file.delete();
    }

	L4B_loadClientSnapshots();
}

function L4B_loadClientSnapshots()
{		
	if(isObject($L4B_clientLog)) $L4B_clientLog.delete();
	$L4B_clientLog = new ScriptGroup();

	%appearancepath = "config/server/Left4Block/loggedplayers/*.txt";
	for(%appearancefile = findFirstFile(%appearancepath); isFile(%appearancefile); %appearancefile = findNextFile(%appearancepath))
	{
		%fileid = strreplace(filename(%appearancefile), ".txt", "");
		%file = new fileObject();
		%file.openForRead(%appearancefile);

		for(%i = 0; %i < ClientGroup.getCount(); %i++) 	
		if(ClientGroup.getObject(%i).BL_ID !$= %fileid)
		{
			%loggedclient = new ScriptObject()
			{
				name = %file.readLine();
				decalName = %file.readLine();
				faceName = %file.readLine();
				accent = %file.readLine();
				hat = %file.readLine();
				accent = %file.readLine();
				chest = %file.readLine();
				pack = %file.readLine();
				secondPack = %file.readLine();
				larm = %file.readLine();
				lhand = %file.readLine();
				rarm = %file.readLine();
				rhand = %file.readLine();
				hip = %file.readLine();
				lleg = %file.readLine();
				rleg = %file.readLine();
				headColor = %file.readLine();
				chestColor = %file.readLine();
				hipColor = %file.readLine();
				larmColor = %file.readLine();
				rarmColor = %file.readLine();
				lhandColor = %file.readLine();
				rhandColor = %file.readLine();		
				llegColor = %file.readLine();
				rlegColor = %file.readLine();
				accentColor = %file.readLine();
				hatColor = %file.readLine();
				packColor = %file.readLine();
				secondPackColor = %file.readLine();
			};

			$L4B_clientLog.add(%loggedclient);
		}

		%file.close();
		%file.delete();		
	}

	if($L4B_clientLog.getCount()) echo("Loading" SPC $L4B_clientLog.getCount() SPC "clients...");
}

function L4B_pushClientSnapshot(%obj,%sourceClient,%zombify)
{
	if(!isObject(%sourceClient) || %sourceClient.getClassName() !$= "GameConnection")
	{
		if(isObject(ClientGroup) && ClientGroup.getCount())
		for (%i = 0; %i < ClientGroup.getCount(); %i++) 
		if(isObject(%client = ClientGroup.getObject(%i))) %clientlist[%cl++] = %client;
		
		if(isObject($L4B_clientLog) && $L4B_clientLog.getCount())
		for (%i = 0; %i < $L4B_clientLog.getCount(); %i++) 
		if(isObject(%loggedclient = $L4B_clientLog.getObject(%i))) %clientlist[%cl++] = %loggedclient;

		if(!%cl) return false;

		%sourceClient = %clientlist[(getRandom(1,%cl))];
	}

	%skin = %sourceClient.headColor;
	%zskin = getWord(%skin,0)/2.75 SPC getWord(%skin,1)/1.5 SPC getWord(%skin,2)/2.75 SPC 1;

	%obj.headColor = %zskin;
	%obj.chestColor = %sourceClient.chestColor;
	%obj.hipColor = %sourceClient.hipColor;
	%obj.rArmColor = %sourceClient.rarmColor;
	%obj.lArmColor = %sourceClient.larmColor;
	%obj.rhandColor = %sourceClient.rhandColor;
	%obj.lhandColor = %sourceClient.lhandColor;
	%obj.rlegColor = %sourceClient.rlegColor;
	%obj.llegColor = %sourceClient.llegColor;
	
	if(%zombify)
	{
		if(%sourceClient.chestColor $= %skin) %obj.chestColor = %zskin;
		if(%sourceClient.rArmColor $= %skin) %obj.rArmColor = %zskin;
		if(%sourceClient.lArmColor $= %skin) %obj.lArmColor = %zskin;
		if(%sourceClient.rhandColor $= %skin) %obj.rhandColor = %zskin;
		if(%sourceClient.lhandColor $= %skin) %obj.lhandColor = %zskin;
		if(%sourceClient.hipColor $= %skin)	%obj.hipColor = %zskin;
		if(%sourceClient.rLegColor $= %skin) %obj.rlegColor = %zskin;
		if(%sourceClient.lLegColor $= %skin) %obj.llegColor = %zskin;
	}
    
	%obj.secondPackColor = %sourceClient.secondPackColor;
	%obj.lhand = %sourceClient.lhand;
	%obj.hip = %sourceClient.hip;
	%obj.faceName = "asciiterror";
	%obj.hatColor = %sourceClient.hatColor;
	%obj.chest = %sourceClient.chest;
	%obj.rarm = %sourceClient.rarm;
	%obj.packColor = %sourceClient.packColor;
	%obj.pack = %sourceClient.pack;
	%obj.decalName = %sourceClient.decalName;
	%obj.secondPack = %sourceClient.secondPack;
	%obj.larm = %sourceClient.larm;
	%obj.accentColor = %sourceClient.accentColor;
	%obj.rleg = %sourceClient.rleg;
	%obj.accent = %sourceClient.accent;
	%obj.rhand = %sourceClient.rhand;
	%obj.lleg = %sourceClient.lleg;
	%obj.hat = %sourceClient.hat;

	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
}

package L4B_ClientLogger
{
	function GameConnection::onClientEnterGame(%this)
	{
		parent::onClientEnterGame(%this);
		L4B_storeClientSnapshots();
	}

	function GameConnection::onClientLeaveGame(%this)
	{
		parent::onClientLeaveGame(%this);
		L4B_storeClientSnapshots();
	}
};
activatePackage(L4B_ClientLogger);