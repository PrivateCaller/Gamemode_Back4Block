---@diagnostic disable: undefined-global

function Melee_SwingCheck(obj,this,slot)
    
    if ts.isobject(obj) == false or ts.getstate(obj) == "Dead" or ts.isobject(ts.callobj(obj,"getMountedImage",0)) == false then return end

    local imagestate = ts.callobj(obj,"getImageState",0)
    if ts.getcallobj(obj,"getMountedImage(0).meleeDamageDivisor") == "" or imagestate == "Ready" or imagestate == "StopFire" then return end
    local pos = ts.callobj(obj,"getMuzzlePoint",slot)
    local endpos = VectorAdd(pos,VectorScale(ts.callobj(obj,"getMuzzleVector",slot),2.5*tonumber(ts.call("getWord",ts.callobj(obj,"getScale"),2))))
    local mask = ts.mask.general

    local ray = ts.raycast(pos, endpos, mask, obj)

    if ts.isobject(ray) then

        local class = ts.callobj(ray, "getClassName")
        local raypos = ts.call("posFromRaycast",ray)            
        ts.call("LuaProjecitle",ts.call("posFromRaycast",ray),"SecondaryMeleeProjectile")

        if class == "AIPlayer" or class == "Player" then                        
            if ts.getstate(ray) ~= "Dead" and tonumber(ts.minigamecandamage(obj,ray)) == 1 then
                ts.call("serverPlay3D",ts.getcallobj(this,"meleeHitPlSound").."_hitpl"..math.random(1,2).."_sound",raypos)
                    if ts.getcallobj(ts.callobj(ray,"getID"),"getDatablock().getName()") ~= "ZombieTankHoleBot" then

                        if class == "AIPlayer" then
                            ts.callobj(ray,"setMoveY",-0.15)
                            ts.callobj(ray,"setMoveX",0)
                            ts.callobj(ray,"setAimObject",obj)
                        end
                        
                        ts.callobj(ray,"playThread",3,"zstumble"..math.random(1,4))
                        ts.callobj(ray,"damage",ts.getobj(obj,"client"),raypos,tonumber(ts.getcallobj(ts.callobj(ray,"getID"),"getDatablock().maxDamage"))/tonumber(ts.getcallobj(this,"meleeDamageDivisor")),tonumber(ts.getcallobj(this,"DamageType")))
                        ts.callobj(ray,"applyimpulse",ts.call("posFromRaycast",ray),VectorAdd(VectorScale(ts.callobj(obj,"getForwardVector"),"600"),"0 0 400"))

                        else ts.callobj(ray,"damage",ts.getobj(obj,"client"),raypos,tonumber(ts.getcallobj(ts.callobj(ray,"getID"),"getDatablock().maxDamage"))/25,tonumber(ts.getcallobj(this,"DamageType")))
                    end
            end
        elseif class == "fxDTSBrick" or class == "WheeledVehicle" or class == "fxPlane" then
            ts.call("serverPlay3D",ts.getcallobj(this,"meleeHitEnvSound").."_hitenv"..math.random(1,2).."_sound",raypos)
        end
    end

    schedule(50, Melee_SwingCheck,obj,this,slot)
end