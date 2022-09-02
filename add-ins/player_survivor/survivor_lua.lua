---@diagnostic disable: undefined-global

function Survivor_Rightclick(obj)
    if tonumber(ts.getobj(obj, "LastMeleeTime")) == nil or tonumber(ts.getobj(obj, "LastMeleeTime"))+400 < tonumber(ts.call("GetSimTime")) then
        ts.setobj(obj, "LastMeleeTime",ts.call("GetSimTime"))

        local pos = ts.callobj(obj,"getHackPosition")
        local radius = 5
        local mask = ts.get("TypeMasks::PlayerObjectType")
        ts.call("initContainerRadiusSearch", pos, radius, mask)
        local target = ts.call("containerSearchNext")

        ts.callobj(obj,"playThread",3,"activate2")--player functions
        ts.call("serverPlay3D","melee_swing" .. math.random(1,2) .. "_sound",pos)

        while ts.call("isObject", target) == "1" do
            if ts.callobj(target, "getID") ~= ts.callobj(obj, "getID") then

                local class = ts.callobj(target, "getClassName")
                local length = 5 * ts.call("getWord",ts.callobj(obj,"getScale"),2)
                local vec = ts.callobj(obj,"getEyeVector")
                local targetpos = ts.callobj(target,"getHackPosition")
                local line = VectorNormalize(VectorSub(targetpos,pos))
                local dot = VectorDot(line,vec)

                if tonumber(VectorDist(pos,targetpos)) < 5 and tonumber(dot) >= 0.45 then

                    local ray = ts.call("containerRayCast", pos, targetpos, ts.get("TypeMasks::All"), obj)

                    if(ts.call("isObject", ray) == "1") then

                        local class = ts.callobj(ray, "getClassName")
                        ts.call("LuaProjecitleRay",ts.call("posFromRaycast",ray),"SecondaryMeleeProjectile")

                        if class == "AIPlayer" or class == "Player" then

                            local zombietype = ts.getobj(ray,"hZombieL4BType")

                                ts.setobj(obj,"SurvivorStress",math.clamp(tonumber(ts.getobj(obj,"SurvivorStress"))+0.25,0,20))
                                ts.callobj(ray,"applyimpulse",ts.call("posFromRaycast",ray),VectorAdd(VectorScale(ts.callobj(obj,"getForwardVector"),"600"),"0 0 250"))
                                ts.callobj(ray,"setMoveY",-0.5)
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
end

function Survivor_LeftClick(val,obj)

    ts.setobj(obj,"isActivating",val)
    
    if tonumber(val) == 1 and tonumber(ts.call("isObject",ts.callobj(obj,"getMountedImage","0"))) == 0 and ts.getobj(obj, "LastActivated") ~= "0" and ts.getobj(obj, "LastActivated") ~= "" then

        if ts.call("isObject",ts.getobj(obj,"LastActivated")) then

            local client = ts.getobj(obj,"client")
            local touchedobj = ts.getobj(obj, "LastActivated")
            local touchedobjclass = ts.callobj(touchedobj, "getClassName")

            if touchedobjclass == "WheeledVehicle" then
                
                local otdatablockimage = ts.call("getValueFromDatablock",ts.callobj(touchedobj,"getDatablock"),"image")

                if otdatablockimage ~= "" then
                    ts.callobj(obj,"mountImage",otdatablockimage, 0);
                    ts.callobj(touchedobj,"delete")
                end

            elseif touchedobjclass == "Player" or touchedobjclass == "AIPlayer" then

                if tonumber(ts.call("getValueFromDatablock",ts.callobj(touchedobj,"getDatablock"),"isDowned")) ~= nil and tonumber(ts.call("getValueFromDatablock",ts.callobj(touchedobj,"getDatablock"),"isDowned")) == 1 then
                    Survivor_ReviveDowned(obj)
                end

                if tonumber(ts.getobj(touchedobj,"isbeingstrangled")) == 1 and tonumber(ts.getobj(touchedobj,"hisinfected")) ~= 1 then
                    if tonumber(ts.call("isObject",ts.getobj(touchedobj,"heater"))) == 1 and ts.call("callFunctionFromObjectDatablock",ts.getobj(touchedobj,"heater"),"getName") ~= "ZombieChargerHoleBot" then

                        local zombiepinner = ts.getobj(touchedobj,"heater")

                        ts.setobj(touchedobj,"isbeingstrangled",0)
                        ts.call("L4B_SpecialsPinCheck",touchedobj,zombiepinner)

                        ts.callobj(touchedobj,"playThread",3,"plant")
                        ts.callobj(obj,"playThread",3,"activate2")

                        if touchedobjclass == "Player" then

                            local touchedobjclient = ts.getobj(touchedobj,"client")
                            local touchedobjminigame = ts.getobj(touchedobjclient,"minigame")

                            ts.call("chatMessageTeam",touchedobjclient,'fakedeathmessage',"<color:00FF00> "..ts.getobj(client,"name").." <bitmapk:add-ons/Gamemode_Left4Block/add-ins/player_survivor/icons/CI_VictimSaved> "..ts.getobj(touchedobjclient,"name"));
                            ts.callobj(touchedobjminigame,"L4B_PlaySound","victim_saved_sound")                          

                        elseif touchedobjclass == "AIPlayer" then
                            ts.callobj(touchedobj,"resetHoleLoop")
                        end                      
                    end
                end
            end
        end
    elseif tonumber(val) == 0 and tonumber(ts.call("isObject",ts.getobj(obj,"LastActivated"))) == 1 then

        local touchedobj = ts.getobj(obj, "LastActivated")
        local touchedobjclass = ts.callobj(touchedobj, "getClassName")

        if touchedobjclass == "Player" or touchedobjclass == "AIPlayer" then
        
            if ts.call("getValueFromDatablock",ts.callobj(touchedobj,"getDatablock"),"isDowned") == "1" then

                ts.setobj(touchedobj,"isbeingsaved",0)
                ts.callobj(obj,"playthread",2,"root")
                ts.setobj(obj,"revivecounter",0)
            end
                
        end
    end
end

function Survivor_ReviveDowned(obj)
    
    if tonumber(ts.getobj(obj,"isActivating")) == 0 then
        return
    end

    local victim = ts.getobj(obj,"LastActivated")
    local dot = VectorDot(VectorNormalize(VectorSub(ts.callobj(victim,"getHackPosition"),ts.callobj(obj,"getHackPosition"))),ts.callobj(obj,"getEyeVector"))
    local client = ts.getobj(obj,"client")
    local victimclient = ts.getobj(victim,"client")
    local objminigame = ts.getobj(obj,"minigame")
    
    if tonumber(dot) >= 0.45 then

        if tonumber(ts.getobj(obj,"revivecounter")) == nil then
            ts.setobj(obj,"revivecounter",0)
        end
        
        if tonumber(ts.getobj(obj,"revivecounter")) < 4 then
            
            local RevivePlayerSched = schedule(1000, Survivor_ReviveDowned, obj)
            ts.setobj(obj,"revivecounter",ts.getobj(obj,"revivecounter")+1)

            if ts.getobj(victim,"isbeingsaved") ~= obj then
                ts.callobj(obj,"playthread",2,"armreadyright")
                ts.setobj(victim,"isbeingsaved",obj)
            end

        else
            ts.callobj(obj,"playthread",2,"root")
            ts.callobj(victim,"playthread",0,"root")
            
            ts.setobj(obj,"revivecounter",0)
            ts.call("chatMessageTeam",victimclient,'fakedeathmessage',"<color:00FF00> "..ts.getobj(client,"name").." <bitmapk:add-ons/Gamemode_Left4Block/add-ins/player_survivor/icons/CI_VictimSaved> "..ts.getobj(victimclient,"name"));
            ts.callobj(client,"play2d","victim_revived_sound")
            ts.callobj(victimclient,"play2d","victim_revived_sound")

            ts.setobj(victim,"lastdamage",ts.call("getsimtime"))
            ts.callobj(victim,"SetDataBlock","SurvivorPlayerLow")
            ts.callobj(victim,"sethealth",25)
            ts.setobj(victim,"isbeingsaved",0)
        end
    else
        ts.setobj(touchedobj,"isbeingsaved",0)
        ts.callobj(obj,"playthread",2,"root")
        ts.setobj(obj,"revivecounter",0)
    end
end

function Survivor_DamageCheck(obj,damage)

    local damagepercent = tonumber(ts.callobj(obj,"getDamagePercent"))
    local damageamount = tonumber(damage)
    local quarterdamage = damageamount*0.25
    local lastdamagetime = ts.getobj(obj, "lastdamage")
    local stresslevel = tonumber(ts.getobj(obj,"survivorstress"))
    
    if damagepercent < 0.5 then ts.callobj(obj,"SetDataBlock","SurvivorPlayer")
    elseif damagepercent > 0.5 then ts.callobj(obj,"SetDataBlock","SurvivorPlayerMed")
    elseif damagepercent > 0.75 then ts.callobj(obj,"SetDataBlock","SurvivorPlayerLow")
    end

    if damageamount      > 0 then
        if tonumber(ts.getobj(obj,"survivorstress")) == nil then ts.setobj(obj,"survivorstress",0) end
		ts.setobj(obj,"survivorstress",math.clamp(stresslevel+quarterdamage,0,20))

        if tonumber(lastdamagetime) == nil or tonumber(lastdamagetime)+500 < tonumber(ts.call("GetSimTime")) then
            ts.setobj(obj, "lastdamage",ts.call("GetSimTime"))
        
			if damageamount >= 5 then ts.callobj(obj,"playaudio",0,"survivor_pain"..math.random(1,4).."_sound") end
            if damageamount >= 20 then ts.callobj(obj,"playaudio",0,"survivor_painmed"..math.random(1,4).."_sound") end
            if damageamount >= 30 then ts.callobj(obj,"playaudio",0,"survivor_painhigh"..math.random(1,4).."_sound") end
			if tonumber(ts.callobj(obj,"getWaterCoverage")) == 1 then ts.callobj(obj,"playaudio",0,"survivor_pain_underwater_sound") end
        end
    end
end

function Survivor_DownCheck(obj,damage,damageType)

    local damagelevel = tonumber(ts.callobj(obj,"getdamagelevel"))
    local damageamount = tonumber(damage)
    local objdatablock = ts.callobj(obj,"getDatablock")
    local objdatablockmaxdamage = tonumber(ts.call("getValueFromDatablock",objdatablock,"maxDamage"))

    if damageType == ts.get("DamageType::Fall") then
        ts.call("serverPlay3D","impact_fall_sound",ts.callobj(obj,"getHackPosition"))
        ts.call("LuaProjecitleRay",ts.callobj(obj,"getHackPosition"),"ZombieHitProjectile")
    end    

    if ts.callobj(obj,"getstate") ~= "Dead" and damagelevel+damageamount >= objdatablockmaxdamage and damageamount <= objdatablockmaxdamage/1.333 and tonumber(ts.callobj(obj,"getWaterCoverage")) ~= 1 then

		ts.callobj(obj,"setdamagelevel",0)
        ts.callobj(obj,"setenergylevel",100)
        ts.callobj(obj,"SetDataBlock","SurvivorPlayerDowned")
		return true

        else return false
    end
end