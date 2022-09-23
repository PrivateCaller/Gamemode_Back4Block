---@diagnostic disable: undefined-global

function Melee_SwingCheck(obj,this,slot)
    
    if ts.isobject(obj) == false or ts.getstate(obj) == "Dead" or ts.isobject(ts.callobj(obj,"getMountedImage",0)) == false then return end

    local imagestate = ts.callobj(obj,"getImageState",0) 
    if ts.getcallobj(obj,"getMountedImage(0).meleeDamageDivisor") == "" or imagestate == "Ready" or imagestate == "StopFire" then return end

    local pos = ts.callobj(obj,"getMuzzlePoint",slot)
    local radius = 1
    local mask = ts.mask.general
    ts.call("initContainerRadiusSearch", pos, radius, mask)
    local target = ts.call("containerSearchNext")

    while ts.isobject(target) do
        if ts.callobj(target,"getID") ~= ts.callobj(obj,"getID") then

            local targetpos = ts.getposition(target)
            local ray = ts.raycast(pos, targetpos, mask, obj)

            if ts.isobject(ray) then
                
                local class = ts.callobj(ray, "getClassName")
                local raypos = ts.call("posFromRaycast",ray)
                local rayobject = ts.callobj(ray,"getID")

                if tonumber(VectorDist(pos,raypos)) < 1 then

                    ts.call("LuaProjecitle",ts.call("posFromRaycast",ray),"SecondaryMeleeProjectile")

                    if class == "AIPlayer" or class == "Player" then
                        if ts.getstate(ray) ~= "Dead" then--and ts.call("minigameCanDamage",obj,ray) and ts.call("checkHoleBotTeams",obj,ray) then
                            ts.call("serverPlay3D",ts.getcallobj(this,"meleeHitPlSound").."_hitpl"..math.random(1,2).."_sound",raypos)

                            if ts.getobj(ray,"hZombieL4BType") ~= "" then                            
                                if ts.getcallobj(ts.callobj(ray,"getID"),"getDatablock().getName()") ~= "ZombieTankHoleBot" then
                                    
                                    ts.callobj(ray,"playThread",3,"zstumble"..math.random(1,4))
                                    ts.callobj(ray,"damage",obj,raypos,tonumber(ts.getcallobj(rayobject,"getDatablock().maxDamage"))/tonumber(ts.getcallobj(this,"meleeDamageDivisor")),ts.get("DamageType::Default"))
                                    ts.callobj(ray,"applyimpulse",ts.call("posFromRaycast",ray),VectorAdd(VectorScale(ts.callobj(obj,"getForwardVector"),"600"),"0 0 400"))

                                    else ts.callobj(ray,"damage",obj,raypos,tonumber(ts.getcallobj(rayobject,"getDatablock().maxDamage"))/25,ts.get("DamageType::Default"))
                                end
                            end
                        end
                    elseif class == "fxDTSBrick" or class == "WheeledVehicle" or class == "fxPlane" then
                        ts.call("serverPlay3D",ts.getcallobj(this,"meleeHitEnvSound").."_hitenv"..math.random(1,2).."_sound",raypos)
                    end
                end
            end
        end
        target = ts.call("containerSearchNext")
    end

    schedule(75, Melee_SwingCheck,obj,this,slot)
end