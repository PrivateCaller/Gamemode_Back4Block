$L4B_lastSupportMessageTime = getSimTime();
function MiniGameSO::chatSupportMessage(%obj, %text, %sound)
{
    announce("[" @ %text @ "]");
    if(%sound $= "")
    {
        %sound = victim_revived_sound;
    }
    %obj.L4B_PlaySound("victim_revived_sound");
    $L4B_lastSupportMessageTime = getSimTime();
}

package Gamemode_Left4Block_FlavorMessages
{
    function Armor::Damage(%data, %obj, %sourceObject, %position, %damage, %damageType)
    {
        parent`::Damage(%data, %obj, %sourceObject, %position, %damage, %damageType);
        if(%obj.getState() $= "Dead" || getSimTime() - $L4B_lastSupportMessageTime < 30000)
	    {
            //If the bot is dead or it's been less than 30 seconds since the last support message, return.
		    return;
	    }
        %minigame = getMiniGameFromObject(%obj);
        %target = %obj.hFollowing;
        //If the bot is a zombie, in a mingame, has a human target, was dealt enough damage to die and isn't being looked at by said target, play a support message.
        if(%obj.hType $= "zombie" && %minigame && %target && %target.getClassName() $= "Player" && (%obj.getDamageLevel() + %damage / getWord(%obj.getScale(), 2) >= %obj.getDatablock().maxDamage) && !L4B_isInFOV(%obj.hFollowing, %obj))
        {
            %minigame.chatSupportMessage(%obj, %sourceObject.sourceClient.getName() SPC "protected" SPC %obj.hFollowing.client.getName());
        }
    }
};
activatepackage(Gamemode_Left4Block_FlavorMessages);