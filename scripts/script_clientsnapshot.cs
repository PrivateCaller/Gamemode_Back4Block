$L4B_clientLog = new ScriptGroup();
MissionCleanup.add($L4B_clientLog);

function L4B_loadClientSnapshots()
{
    echo("Loading saved clients...");
    if(!isFile("config/server/L4B2_Bots/loggedplayers.txt"))
    {
    	echo("None found.");
    	return;
    }
    %file = new fileObject();
    %file.openForRead("config/server/L4B2_Bots/loggedplayers.txt");
    while(!%file.isEOF())
    {
    	%client_info = %file.readLine();
    	%client_object = new ScriptObject() 
    	{
    		//It is important that these are "getRecord" and not "getWord".
    		name = getRecord(%client_info, 0);
    		blid = getRecord(%client_info, 1);
    		accent = getRecord(%client_info, 2);
    		hat = getRecord(%client_info, 3);
    		chest = getRecord(%client_info, 4);
    		decalName = getRecord(%client_info, 5);
    		pack =  getRecord(%client_info, 6);
    		secondPack = getRecord(%client_info, 7);
    		larm = getRecord(%client_info, 8);
    		lhand = getRecord(%client_info, 9);
    		rarm = getRecord(%client_info, 10);
    		rhand = getRecord(%client_info, 11);
    		hip = getRecord(%client_info, 12);
    		lleg = getRecord(%client_info, 13);
    		rleg = getRecord(%client_info, 14);
    		accentColor = getRecord(%client_info, 15);
    		hatColor = getRecord(%client_info, 16);
    		packColor = getRecord(%client_info, 17);
    		secondPackColor = getRecord(%client_info, 18);
    		skinColor = getRecord(%client_info, 19);
    	};
    	$L4B_clientLog.add(%client_object);
    }
    %file.close();
    %file.delete();
    echo($L4B_clientLog.getCount() SPC "clients found.");
}
L4B_loadClientSnapshots();

package L4B_ClientLogger
{
	function GameConnection::onClientEnterGame(%this)
	{
		parent::onClientEnterGame(%this);
		for(%i = 0; %i < $L4B_clientLog.getCount(); %i++)
		{
			if($L4B_clientLog.getObject(%i).blid $= %this.getBLID())
			{
				return;
			}
		}
		L4B_createClientSnapshot(%this);
		L4B_storeLoggedClients();
	}
};
activatePackage(L4B_ClientLogger);