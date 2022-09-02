ts.eval("function LuaProjecitleRay(%pos,%datablock) {%p = new projectile() {datablock= %datablock; initialPosition = %pos;}; %p.explode();}")
ts.eval("function getValueFromDatablock(%data, %value){eval(\"return \" @ %data @ \".\" @ %value @ \";\");}")
ts.eval("function callFunctionFromObjectDatablock(%obj,%function,%arguements){eval(\"return \" @ %obj.getdatablock() @ \".\" @ %function @ \"(\" @ %arguements @ \");\");}")
	
function talk(string)
	return ts.call("talk",string)
end

function ts.getPosition(obj) return vector(ts.callobj(obj, "getPosition")) end

function ts.isObject(obj)
	return ts.call("isObject", obj) == "1"
end

function ts.delete(obj)
	if ts.isObject(obj) then ts.callobj(obj, "delete") end
end

function ts.allBricks()
	local mbg = "MainBrickGroup"
	local mbgc = tonumber(ts.callobj(mbg, "getCount"))
	local bgi = 0
	local bg = ts.callobj(mbg, "getObject", bgi)
	local bgc = tonumber(ts.callobj(bg, "getCount"))
	local bricki = 0
	return function()
		if bricki >= bgc then
			repeat
				bgi = bgi + 1
				if bgi >= mbgc then return nil end
				bg = ts.callobj(mbg, "getObject", bgi)
				bgc = tonumber(ts.callobj(bg, "getCount"))
				bricki = 0
			until (bgc>0)
		end
		local brick = ts.callobj(bg, "getObject", bricki)
		bricki = bricki + 1
		return tonumber(brick)
	end
end
function ts.getBricksByName(searchName, regex)
	searchName = searchName:lower()
	local namedBricks = {}
	for brick in ts.allBricks() do
		local name = ts.callobj(brick, "getName"):lower()
		if (name ~= "") then
			name = name:gsub("^_", "")
			if
				((not regex) and name == searchName   ) or
				( regex      and name:find(searchName))
			then
				table.insert(namedBricks, tonumber(brick))
			end
		end
	end
	return namedBricks
end

function ts.allClients()
	local cg = "ClientGroup"
	local cgc = tonumber(ts.callobj(cg, "getCount"))
	local ci = 0
	return function()
		if ci >= cgc then return nil end
		local client = ts.callobj(cg, "getObject", ci)
		ci = ci + 1
		return tonumber(client)
	end
end
function ts.allPlayers()
	local ac = ts.allClients()
	return function()
		repeat
			local client = ac()
			if client==nil then return nil end
			player = tonumber(ts.getobj(client, "player"))
		until (ts.isObject(player))
		return tonumber(player)
	end
end

function ts.raycast(pos1, pos2, mask, ignore) check(pos1, "vector", 1) check(pos2, "vector", 2) check(mask, "number", 3)
	local ray = ts.call("containerRaycast", pos1:tsString(), pos2:tsString(), mask, unpack(ignore))
	local hitStr, posStr, normStr = ray:match("^([^ ]+) ([^ ]+ [^ ]+ [^ ]+) ([^ ]+ [^ ]+ [^ ]+)$")
	if hitStr then
		local hit = tonumber(hitStr)
		local pos = vector(posStr)
		local norm = vector(normStr)
		return hit, pos, norm
	else
		return nil
	end
end

ts.mask = {
	brick = tonumber(ts.get("TypeMasks::FxBrickObjectType")),
	brickAlways = tonumber(ts.get("TypeMasks::FxBrickAlwaysObjectType")),
	player = tonumber(ts.get("TypeMasks::PlayerObjectType")),
	terrain = tonumber(ts.get("TypeMasks::TerrainObjectType")),
	static = tonumber(ts.get("TypeMasks::StaticObjectType")),
}
ts.mask.obstruction = ts.mask.brick + ts.mask.terrain + ts.mask.static

function ts.getTimeMs()
	return tonumber(ts.call("getSimTime"))
end
