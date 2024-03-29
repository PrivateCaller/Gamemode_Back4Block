---@diagnostic disable: undefined-global, param-type-mismatch, lowercase-global
function Melee_SwingCheck(obj,this,slot)

    if ts.isobject(obj) == false or ts.getstate(obj) == "Dead" or ts.isobject(ts.callobj(obj,"getMountedImage",0)) == false then return end

    local imagestate = ts.callobj(obj,"getImageState",0)
    if ts.getcallobj(obj,"getMountedImage(0).meleeDamageDivisor") == "" or imagestate == "Ready" or imagestate == "StopFire" then return end
    local radius = 5
    local pos = ts.callobj(obj,"getMuzzlePoint",slot)
    local endpos = VectorAdd(pos,VectorScale(ts.callobj(obj,"getMuzzleVector",slot),2.5*tonumber(ts.call("getWord",ts.callobj(obj,"getScale"),2))))
    local mask = ts.mask.general

    ts.call("initContainerRadiusSearch", pos, radius, mask)
    local scanned = ts.call("containerSearchNext")

    while ts.isobject(scanned) do

        if ts.callobj(scanned,"getID") ~= ts.callobj(obj,"getID") then
            local scannedpos = ts.callobj(scanned,"getWorldBoxCenter")

            local ray = ts.raycast(pos, scannedpos, mask, obj)
            if ts.isobject(ray) and tonumber(VectorDist(pos,ts.call("posFromRaycast",ray))) < 1 then

                local class = ts.callobj(ray, "getClassName")
                local raypos = ts.call("posFromRaycast",ray)                

                if tonumber(ts.getobj(obj,"lastmeleeenvhitdelay")) == nil then ts.setobj(obj,"lastmeleeenvhitdelay",0) end                
                if class == "AIPlayer" or class == "Player" then hitdelay = 15
                elseif class == "fxDTSBrick" or class == "WheeledVehicle" or class == "fxPlane" then hitdelay = 50 end

                if tonumber(ts.getobj(obj,"lastmeleeenvhitdelay"))+hitdelay < ts.getsimtime() then
                    ts.call("LuaProjecitle",ts.call("posFromRaycast",ray),"SecondaryMeleeProjectile")
                    ts.setobj(obj, "lastmeleeenvhitdelay",ts.getsimtime())
                    if class == "AIPlayer" or class == "Player" then
                        ts.call("serverPlay3D",ts.getcallobj(this,"meleeHitPlSound").."_hitpl"..math.random(1,2).."_sound",raypos)
                        if ts.getstate(ray) ~= "Dead" and tonumber(ts.minigamecandamage(obj,ray)) == 1 then
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
                                ts.call("LuaProjecitle",ts.call("posFromRaycast",ray),"SecondaryMeleeProjectile")
                        end
                        elseif class == "fxDTSBrick" or class == "WheeledVehicle" or class == "fxPlane" then
                        ts.call("serverPlay3D",ts.getcallobj(this,"meleeHitEnvSound").."_hitenv"..math.random(1,2).."_sound",raypos)
                    end
                end
            end
        end
        scanned = ts.call("containerSearchNext")
    end

    schedule(50, Melee_SwingCheck,obj,this,slot)
end