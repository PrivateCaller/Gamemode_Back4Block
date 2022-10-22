function L4B_isInFOV(%viewer, %target)
{    
    if(isObject(%viewer) && isObject(%target)) return vectorDot(%viewer.getEyeVector(), vectorNormalize(vectorSub(%target.getPosition(), %viewer.getPosition()))) >= 0.7;
}

package L4B_SupportMessages
{
    function Armor::Damage(%data, %obj, %sourceObject, %position, %damage, %damageType)
    {
        Parent::Damage(%data, %obj, %sourceObject, %position, %damage, %damageType);

        if(!isObject(%minigame = getMiniGameFromObject(%obj)) || %obj.hType !$= "Zombie" || %obj.getState() !$= "Dead") return;//Return if the object is not a zombie, not dead, is not targetting, 

        if(isObject(%sourceObject) && %sourceObject.getClassName() $= "Player") %source = %sourceObject;
        else if(isObject(%sourceObject.sourceObject) && %sourceObject.sourceObject.getClassName() $= "Player") %source = %sourceObject.sourceObject;
        else return;
        
        //When the bot is a special and dies
        if(%obj.getdataBlock().hZombieL4BType $= "Special") 
        %minigame.L4B_ChatMessage("\c0" @ %source.client.name SPC "<bitmapk:" @ $DamageType::MurderBitmap[%damageType] @ ">" SPC %obj.getdataBlock().hName @ "","victim_revived_sound",true); 

        //When a player kills a zombie the victim is unaware of
        if(isObject(%target = %obj.hFollowing) && %target.getClassName() $= "Player" && %source !$= %target && !L4B_isInFOV(%target, %obj))
        %minigame.L4B_ChatMessage("<color:00FF00>" @ %source.client.name SPC "protected" SPC %target.client.name,"victim_revived_sound",false); 
               
    }
};
activatepackage(L4B_SupportMessages);