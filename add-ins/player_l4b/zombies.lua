---@diagnostic disable: undefined-global, lowercase-global, redundant-parameter

function hNoSeeIdleTeleport(obj)

    if ts.getstate(obj) == "Dead" or ts.isobject(ts.call("getMinigameFromObject",obj)) == false or ts.callobj(obj,"hState") == "Following" then 
        ts.setobj(obj,"hTimeLeftToTele",0)
        return 
    end--If we are not in a minigame, dead or following a targetet, quickly return the function

    if tonumber(ts.getobj(obj,"hTimeLeftToTele")) == nil then ts.setobj(obj,"hTimeLeftToTele",0) end
    local hTimeLeftToTele = tonumber(ts.getobj(obj,"hTimeLeftToTele"))
    ts.setobj(obj,"hTimeLeftToTele",hTimeLeftToTele+1)    

    if hTimeLeftToTele >= 5 then--Only begin after a certain amount of time has passed, which is the amount of hole loops the bot does when its not in any of the returned states above
        ts.setobj(obj,"hTimeLeftToTele",0)

        if ts.isobject(ts.call("getMinigameFromObject",obj)) then
           local minigame = ts.call("getMinigameFromObject",obj)
           local membercount = tonumber(ts.getcallobj(minigame,"numMembers"))

            for memberloopcount = 0, membercount, 1 do
                if ts.isobject(ts.getcallobj(minigame,"member["..memberloopcount.."]")) then--Check if the member exists
                    local minigamemember = ts.getcallobj(minigame,"member["..memberloopcount.."]")

                    if ts.isobject(ts.getcallobj(minigamemember,"player")) then--Check if the player exists

                        local player = ts.getcallobj(minigamemember,"player")
                        local playereyepoint = ts.getcallobj(player,"getEyePoint()")
                        local headpos = ts.getcallobj(obj,"getMuzzlePoint(2)")
                        local raycast = ts.raycast(headpos,playereyepoint,ts.mask.obstruction,obj)

                        if ts.isobject(raycast) then--Are there no objects blocking us from the player's perspective?
                            cansee = false
                            break
                        else--We can be potentially observed, check if we are in the player's field of view
                            local playereyevector = ts.getcallobj(player,"getEyeVector()")
                            local posnormal = VectorNormalize(VectorSub(headpos,playereyepoint))

                            if tonumber(VectorDot(playereyevector,posnormal)) > 0.55 then--Can they see us?
                                cansee = true--We can be seen
                                break
                                else cansee = false--We cannot be seen                          
                            end
                        end
                    end
                end
            end
        end
        if cansee == false then ts.getcallobj(obj,"doMRandomTele(\"Horde\")") end--Teleport if we cannot be observed
    end  
end


function L4B_ZombieLunge(obj,target,power)
    
    if ts.isobject(obj) == false or ts.isobject(target) == false or ts.getstate(obj) == "Dead" or ts.getstate(target) == "Dead" and tonumber(ts.call("geWord",ts.callobj(obj,"getVelocity"),2)) ~= 0 then return end
    
        local targetpos = ts.getposition(target)
        local objpos = ts.getposition(obj)
        local distancepos = VectorSub(targetpos,objpos)
        local normvector = VectorNormalize(VectorAdd(distancepos,"0 0 "..0.15*VectorDist(targetpos,objpos)))
        local eye = VectorScale(normvector,2);

        ts.callobj(obj,"setvelocity",VectorScale(eye,power))
        ts.callobj(obj,"playthread",0,"jump")
end

function L4B_SpazzZombie(obj,count)
	
    if count == nil then count = 0 end
    if ts.isobject(obj) == false or ts.getstate(obj) == "Dead" or tonumber(count) >= 15 then return end

    ts.callobj(obj,"activateStuff")

    cancel(L4B_SpazzZombieSched)
    local L4B_SpazzZombieSched = schedule(math.random(100,200),L4B_SpazzZombie,obj,count+1)
end

function L4B_ZombieDropLoot(obj,lootitemdatablock,chance)
    
        --if ts.isobject(obj) == false or ts.isobject(lootitemdatablock) == false then return
        --elseif math.random(1,100) <= tonumber(chance) then
        --    local lootitem = ts.call("luaitem",obj,lootitemdatablock)
        --    ts.callobj(lootitem,"applyimpulse",ts.getposition(lootitem),VectorAdd(VectorScale(ts.callobj(obj,"getForwardVector"),math.random(1,4)),math.random(1,4).." 0 "..math.random(2,6)))
        --    local fadeSched = schedule(8000,ts.callobj(lootitem,"fadeOut"),lootitem)
        --    local delSched = schedule(8200,ts.callobj(lootitem,"delete"),lootitem)
        --end
end