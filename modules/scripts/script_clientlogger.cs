$L4B_ReferenceAppearance[%ra = 0] = "0";
$L4B_ReferenceAppearance[%ra++] = "0.000 0.200 0.640 0.700";
$L4B_ReferenceAppearance[%ra++] = "0";
$L4B_ReferenceAppearance[%ra++] = "0.9 0.9 0.9 1";
$L4B_ReferenceAppearance[%ra++] = "AAA-None";
$L4B_ReferenceAppearance[%ra++] = "smiley";
$L4B_ReferenceAppearance[%ra++] = "0";
$L4B_ReferenceAppearance[%ra++] = "1 1 0 1";
$L4B_ReferenceAppearance[%ra++] = "1 0.878 0.611 1";
$L4B_ReferenceAppearance[%ra++] = "0";
$L4B_ReferenceAppearance[%ra++] = "0 0 1 1";
$L4B_ReferenceAppearance[%ra++] = "0";
$L4B_ReferenceAppearance[%ra++] = "0.9 0 0 1";
$L4B_ReferenceAppearance[%ra++] = "0";
$L4B_ReferenceAppearance[%ra++] = "1 0.878 0.611 1";
$L4B_ReferenceAppearance[%ra++] = "0";
$L4B_ReferenceAppearance[%ra++] = "0 0 1 1";
$L4B_ReferenceAppearance[%ra++] = "0";
$L4B_ReferenceAppearance[%ra++] = "0 0.435 0.831 1";
$L4B_ReferenceAppearance[%ra++] = "0";
$L4B_ReferenceAppearance[%ra++] = "0.9 0 0 1";
$L4B_ReferenceAppearance[%ra++] = "0";
$L4B_ReferenceAppearance[%ra++] = "1 0.878 0.611 1";
$L4B_ReferenceAppearance[%ra++] = "0";
$L4B_ReferenceAppearance[%ra++] = "0 0 1 1";
$L4B_ReferenceAppearance[%ra++] = "0";
$L4B_ReferenceAppearance[%ra++] = "0 1 0 1";
$L4B_ReferenceAppearanceTags = "accent accentColor chest chestColor decalName faceName hat hatColor headColor hip hipColor larm larmColor lhand lhandColor lleg llegColor pack packColor rarm rarmColor rhand rhandColor rleg rlegColor secondPack secondPackColor";

function L4B_storeClientSnapshot(%client)
{
	if(!isObject(%client)) return;
	
	echo("Storing" SPC %client.name @ "'s appearance...");
	for(%apc = 0; %apc < getWordCount($L4B_ReferenceAppearanceTags); %apc++) if(%client.getField(getWord($L4B_ReferenceAppearanceTags,%apc)) $= $L4B_ReferenceAppearance[%apc]) %defaultappearancecount++;

	if(%defaultappearancecount <= 25) //No point in saving a default appearance, this checks it
	{
		%file = new fileObject();
		%file.openForWrite("config/server/Left4Block/loggedplayers/" @ %client.BL_ID @ ".txt");
		%file.writeLine(%client.name);

		for(%rat = 0; %rat < getWordCount($L4B_ReferenceAppearanceTags); %rat++)
		%file.writeLine(%client.getField(getWord($L4B_ReferenceAppearanceTags,%rat)));

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
		%loggedclient = new ScriptObject();	

		%loggedclient.setField("name",%file.readLine());
		for(%apc = 0; %apc < getWordCount($L4B_ReferenceAppearanceTags); %apc++)
		%loggedclient.setField(getWord($L4B_ReferenceAppearanceTags,%apc),%file.readLine());	

		$L4B_clientLog.add(%loggedclient);
		%file.close();
		%file.delete();
	}

	if($L4B_clientLog.getCount()) echo("Loading" SPC $L4B_clientLog.getCount() SPC "client's appearances...");
}

function L4B_pushClientSnapshot(%obj,%sourceClient,%zombify)
{
	if(!isObject(%sourceClient) || %sourceClient.getClassName() !$= "GameConnection")
	{
		if(isObject(ClientGroup) && ClientGroup.getCount()) for(%i = 0; %i < ClientGroup.getCount(); %i++) 
		if(isObject(%client = ClientGroup.getObject(%i))) %clientlist[%cl++] = %client;
		
		if(isObject($L4B_clientLog) && $L4B_clientLog.getCount()) for(%i = 0; %i < $L4B_clientLog.getCount(); %i++) 
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
	%obj.stolenname = %sourceClient.name;
	
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
	function GameConnection::onClientEnterGame(%client)
	{
		parent::onClientEnterGame(%client);
		L4B_storeClientSnapshot(%client);
	}

	function GameConnection::onClientLeaveGame(%client)
	{
		parent::onClientLeaveGame(%client);
		L4B_storeClientSnapshot(%client);		
	}
};
activatePackage(L4B_ClientLogger);