%pattern = "add-ons/gamemode_left4block/add-ins/weapon_riot_shield/sound/*.wav";//Too lazy to write datablock files for the sounds, just took this from the Disease Gamemode
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strlwr(%file);
	%soundName = strreplace(%soundName, "add-ons/gamemode_left4block/add-ins/weapon_riot_shield/sound/", "");
	%soundName = strreplace(%soundName, "/", "");
	%soundName = strreplace(%soundName, ".wav", "");

	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClosest3d; filename = \"" @ %file @ "\"; };");

	%file = findNextFile(%pattern);
}

datablock ItemData(RiotShieldItem)
{
	category		= "Weapon";
	className		= "Weapon";

	shapeFile		= "./models/Riotshield.dts";
	rotate			= false;
	mass			= 1;
	density			= 0.2;
	elasticity		= 0.2;
	friction		= 0.6;
	emap			= true;

	uiName			= "Riot Shield";
	iconName		= "./icons/icon_riotshield";
	doColorShift	= false;

	image			= RiotShieldImage;
	canDrop			= true;
};

datablock ShapeBaseImageData(RiotShieldImage)
{
	shapeFile		= "./models/Riotshield.dts";
	emap							= true;

	mountPoint						= 0;
	offset							= "0 0 0";
	eyeOffset						= 0;
	rotation						= eulerToMatrix( "0 0 0" );
	scale							= "3 3 3";
	correctMuzzleVector				= true;

	className						= "WeaponImage";
	item							= " ";
	ammo							= " ";
	projectile						= "";
	projectileType					= "Projectile";

	melee							= false;
	armReady						= true;

	doColorShift					= false;
	colorShiftColor					= "0.196 0.196 0.196 0.5";

	stateName[0]					= "Activate";
	stateSound[0]					= weaponSwitchSound;
	stateTransitionOnTimeout[0]		= "Ready";
	stateTimeoutValue[0]			= 0.5;

	stateName[1]					= "Ready";
	stateTransitionOnTriggerDown[1]	= "Attack";

	stateName[2]					= "Attack";
	stateTimeoutValue[2]			= 0.75;
	stateScript[2]					= "onAttack";
	stateTransitionOnTimeout[2]		= "Ready";
};

function RiotShieldImage::onAttack(%this, %obj, %slot)
{	
	%obj.playthread(1,"armReady");
	%obj.doMelee();
}