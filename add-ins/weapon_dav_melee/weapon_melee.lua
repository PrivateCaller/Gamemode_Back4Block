---@diagnostic disable: undefined-global

function Melee_SwingCheck(obj,this,slot)--Still not done, dont use this
    
    if ts.get("isObject",obj) ~= "1" or ts.get("isObject",ts.callobj(obj,"getMountedImage",0)) ~= "1" or ts.call("getValueFromDatablock",ts.callobj(obj,"getMountedImage"),"MeleeDamage") == "" then
        return
    end

    local pos = ts.callobj(obj,"getMuzzlePoint",slot)
    local radius = 2
    local mask = ts.get("TypeMasks::All")
    ts.call("initContainerRadiusSearch", pos, radius, mask)
    local target = ts.call("containerSearchNext")

    while ts.call("isObject", target) == "1" do
        if ts.callobj(target, "getID") ~= ts.callobj(obj, "getID") then

            local class = ts.callobj(target, "getClassName")
            local length = 2 * ts.call("getWord",ts.callobj(obj,"getScale"),2)
            local vec = ts.callobj(obj,"getEyeVector")
            local targetpos = ts.callobj(target,"getPosition")
            local line = VectorNormalize(VectorSub(targetpos,pos))
            local dot = VectorDot(line,vec)

            if tonumber(VectorDist(pos,targetpos)) < 5 and tonumber(dot) >= 0.45 then

                local ray = ts.call("containerRayCast", pos, targetpos, mask, obj)

                if(ts.call("isObject", ray) == "1") then

                    local class = ts.callobj(ray, "getClassName")
                    ts.call("LuaProjecitleRay",ts.call("posFromRaycast",ray),"SecondaryMeleeProjectile")

                    if class == "AIPlayer" or class == "Player" then

                        local zombietype = ts.getobj(ray,"hZombieL4BType")

                            ts.setobj(obj,"SurvivorStress",math.clamp(tonumber(ts.getobj(obj,"SurvivorStress"))+0.25,0,20))
                            ts.callobj(ray,"applyimpulse",ts.call("posFromRaycast",ray),VectorAdd(VectorScale(ts.callobj(obj,"getForwardVector"),"1200"),"0 0 500"))
                            ts.call("serverPlay3D","melee_hit" .. math.random(1,8) .. "_sound",ts.call("posFromRaycast",ray))
                            ts.call("callFunctionFromObjectDatablock",ray,"onDamage",ts.callobj(ray,"getID"))

                    elseif class == "fxDTSBrick" or class == "WheeledVehicle" or class == "fxPlane" then
                        ts.call("serverPlay3D","HitEnv_Sound",ts.call("posFromRaycast",ray))
                    end
                end
            end
        end
        target = ts.call("containerSearchNext")
    end
end