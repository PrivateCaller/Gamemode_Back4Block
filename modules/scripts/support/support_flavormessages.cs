function L4B_isInFOV(%viewer, %target)
{    
    if(isObject(%viewer) && isObject(%target)) return vectorDot(%viewer.getEyeVector(), vectorNormalize(vectorSub(%target.getPosition(), %viewer.getPosition()))) >= 0.7;
}

package L4B_SupportMessages
{
    function Armor::Damage(%data, %obj, %sourceObject, %position, %damage, %damageType)
    {
        Parent::Damage(%data, %obj, %sourceObject, %position, %damage, %damageType);

        if(%obj.hType !$= "Zombie" || %obj.getState() !$= "Dead" || !isObject(%obj.hFollowing)) return;//Return if the object is not a zombie, not dead, is not targetting, 

        if(isObject(%sourceObject) && %sourceObject.getClassName() $= "Player") %source = %sourceObject;
        else if(isObject(%sourceObject.sourceObject) && %sourceObject.sourceObject.getClassName() $= "Player") %source = %sourceObject.sourceObject;
        else return;

        //Check if the minigame exists, along with the target, and make sure its a player and the killer isnt itself, and make sure the player is unaware of the bot
        if(isObject(%minigame = getMiniGameFromObject(%obj)) && isObject(%target = %obj.hFollowing) && %target.getClassName() $= "Player" && %source !$= %target && !L4B_isInFOV(%target, %obj))
        %minigame.L4B_ChatMessage(%source.client.name SPC "protected" SPC %target.client.name,"victim_revived_sound",false); 
        
    }
};
activatepackage(L4B_SupportMessages);