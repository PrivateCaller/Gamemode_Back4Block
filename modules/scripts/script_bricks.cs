function InteractiveBrickAnim(%obj,%oc)
{
	if(!isObject(%obj)) return;
	
	if(%oc)
	{
		if(!%obj.isopen)
		{
			%obj.playaudio(2,"lockeropen_sound");
			%obj.playthread(1,"open");
			%obj.isopen = 1;
		}
	}
	else
	{
		%obj.playthread(1,"close");
		cancel(%obj.slowd);
		%obj.isopen = 0;
		%obj.isshutting = 1;
		%obj.slowd = schedule(500,0,%obj.isshutting = 0,%obj);
		%obj.playaudio(2,"lockerclose_sound");
	}
}