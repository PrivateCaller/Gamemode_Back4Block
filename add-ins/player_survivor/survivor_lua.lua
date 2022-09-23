---@diagnostic disable: undefined-global

function Survivor_Rightclick(obj)

    local lastmeleedelay = tonumber(ts.getobj(obj, "LastMeleeTime"))    
    local delaycount = tonumber(ts.getobj(obj, "LastMeleeCount"))

    if ts.getobj(obj, "LastMeleeCount") == "" or lastmeleedelay ~= nil and ts.getsimtime()-lastmeleedelay > 3000 then
        ts.setobj(obj, "LastMeleeCount",0)
        delaycount = tonumber(ts.getobj(obj, "LastMeleeCount"))
    end
    local newmeleedelay = 300 + delaycount*40
    
    if lastmeleedelay == nil or lastmeleedelay+newmeleedelay < ts.getsimtime() then
        ts.setobj(obj, "LastMeleeTime",ts.getsimtime())
        ts.setobj(obj, "LastMeleeCount",delaycount+1)

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

                if tonumber(VectorDist(pos,targetpos)) < 4 and tonumber(dot) >= 0.45 then

                    local ray = ts.call("containerRayCast", pos, targetpos, ts.get("TypeMasks::All"), obj)

                    if(ts.call("isObject", ray) == "1") then

                        local class = ts.callobj(ray, "getClassName")
                        ts.call("LuaProjecitle",ts.call("posFromRaycast",ray),"SecondaryMeleeProjectile")

                        if ts.call("minigameCanDamage",obj,ray) and ts.call("checkHoleBotTeams",obj,ray) and class == "AIPlayer" or class == "Player" then

                                ts.setobj(obj,"SurvivorStress",math.clamp(tonumber(ts.getobj(obj,"SurvivorStress"))+0.25,0,20))

                                if ts.getcallobj(ts.callobj(ray,"getID"),"getDatablock().resistMelee") ~= "1" then 
                                    
                                    if class == "AIPlayer" then
                                        ts.callobj(ray,"setMoveY",-0.5)
                                        ts.callobj(ray,"setMoveX",0)
                                        ts.callobj(ray,"setAimObject",obj)
                                    end

                                    ts.callobj(ray,"cancel","L4B_SpazzZombie")
                                    ts.callobj(ray,"playThread",3,"zstumble"..math.random(1,3))
                                    ts.getcallobj(ts.callobj(ray,"getDatablock"),"onDamage("..ts.callobj(ray,"getID")..")")
                                    ts.callobj(ray,"applyimpulse",ts.call("posFromRaycast",ray),VectorAdd(VectorScale(ts.callobj(obj,"getForwardVector"),"500"),"0 0 250"))
                                end

                                
                                ts.call("serverPlay3D","melee_hit" .. math.random(1,8) .. "_sound",ts.call("posFromRaycast",ray))
                                

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
                
                local otdatablockimage = ts.getcallobj(touchedobj,"getDatablock().image")

                if otdatablockimage ~= "" then
                    ts.callobj(obj,"mountImage",otdatablockimage, 0);
                    ts.callobj(touchedobj,"delete")
                end

            elseif touchedobjclass == "Player" or touchedobjclass == "AIPlayer" then

                if ts.getcallobj(touchedobj,"getdatablock().isDowned") == "1" then
                    Survivor_ReviveDowned(obj)
                end

                if tonumber(ts.getobj(touchedobj,"isbeingstrangled")) == 1 and tonumber(ts.getobj(touchedobj,"hisinfected")) ~= 1 then
                    if tonumber(ts.call("isObject",ts.getobj(touchedobj,"heater"))) == 1 and ts.getcallobj(ts.getobj(touchedobj,"heater"),"getDatablock().getName()") ~= "ZombieChargerHoleBot" then

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
        
            if ts.getcallobj(touchedobj,"getdatablock().isDowned") == "1" then

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

    if ts.getstate(obj) == "Dead" then return end
    
    local damagepercent = tonumber(ts.callobj(obj,"getDamagePercent"))
    local lastdamagetime = ts.getobj(obj, "lastdamage")
    local stresslevel = tonumber(ts.getobj(obj,"survivorstress"))
    
    if damagepercent < 0.5 then ts.callobj(obj,"SetDataBlock","SurvivorPlayer")
    elseif damagepercent > 0.5 then ts.callobj(obj,"SetDataBlock","SurvivorPlayerMed")
    elseif damagepercent > 0.75 then ts.callobj(obj,"SetDataBlock","SurvivorPlayerLow")
    end

    if tonumber(damage) ~= nil and tonumber(damage) > 0 then
        local quarterdamage = damage*0.25
        
        if tonumber(ts.getobj(obj,"survivorstress")) == nil then ts.setobj(obj,"survivorstress",0) end
		ts.setobj(obj,"survivorstress",math.clamp(stresslevel+quarterdamage,0,20))

        if tonumber(lastdamagetime) == nil or tonumber(lastdamagetime)+500 < tonumber(ts.call("GetSimTime")) then
            ts.setobj(obj, "lastdamage",ts.call("GetSimTime"))
        
			if tonumber(damage) >= 5 then ts.callobj(obj,"playaudio",0,"survivor_pain"..math.random(1,4).."_sound") end
            if tonumber(damage) >= 20 then ts.callobj(obj,"playaudio",0,"survivor_painmed"..math.random(1,4).."_sound") end
            if tonumber(damage) >= 30 then ts.callobj(obj,"playaudio",0,"survivor_painhigh"..math.random(1,4).."_sound") end
			if tonumber(ts.callobj(obj,"getWaterCoverage")) == 1 then ts.callobj(obj,"playaudio",0,"survivor_pain_underwater_sound") end
        end
    end
end

function Survivor_DownCheck(obj,damage,damageType)
    
    if ts.getstate(obj) == "Dead" or tonumber(damage) == nil then return end
    
    local damagelevel = tonumber(ts.callobj(obj,"getdamagelevel"))
    local damagedlevel = tonumber(damage)+damagelevel
    local maxdamage = tonumber(ts.getcallobj(obj,"getdatablock().maxDamage"))

    if damageType == ts.get("DamageType::Fall") then
        ts.call("serverPlay3D","impact_fall_sound",ts.callobj(obj,"getHackPosition"))
        ts.call("LuaProjecitle",ts.callobj(obj,"getHackPosition"),"ZombieHitProjectile")
    end    

    if ts.getstate(obj) ~= "Dead" and ts.callobj(obj,"getWaterCoverage") ~= "1" and damagedlevel >= maxdamage and tonumber(damage) < maxdamage/1.333 then

		ts.callobj(obj,"setdamagelevel",0)
        ts.callobj(obj,"setenergylevel",100)
        ts.callobj(obj,"SetDataBlock","SurvivorPlayerDowned")
		return true

        else return false
    end
end

function Survivor_FallDamage(obj,vector,force)--Default falling damage is garbage so I made my own version thats more suited for the player

    local pos = ts.callobj(obj,"getPosition")
    local minimpactspeed = tonumber(ts.getcallobj(obj,"getdatablock().minimpactspeed"))
    local vectorz = tonumber(ts.call("getword",vector,2))
    local vectorzquarter = vectorz/2
    local forcequarter = tonumber(force)/2
    local falldamage = vectorzquarter*forcequarter

    if vectorz > minimpactspeed and vectorz < 20 then ts.callobj(obj,"damage",obj,pos,falldamage/3.75,ts.get("DamageType::Fall"))
    elseif vectorz > 20 and vectorz < 25 then ts.callobj(obj,"damage",obj,pos,falldamage/1.5,ts.get("DamageType::Fall"))
    elseif vectorz > 25 then ts.callobj(obj,"damage",obj,pos,falldamage*2.5,ts.get("DamageType::Fall"))
    end

end