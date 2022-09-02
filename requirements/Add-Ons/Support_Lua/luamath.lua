function math.clamp(value,min,max)
    return math.min(math.max(value,min),max)
end

function strToTable(s)
    local tuple = {}
    local i = 1
    for w in string.gmatch(s, "%S+") do
        tuple[i] = tonumber(w)
        i = i + 1
    end
    return tuple
end

function VectorAdd(str_one, str_two)
    local a = strToTable(str_one)
    local b = strToTable(str_two)
    return tostring(a[1] + b[1]) .. ' ' .. tostring(a[2] + b[2]) .. ' ' .. tostring(a[3] + b[3])
end

function VectorSub(str_one, str_two)
    local a = strToTable(str_one)
    local b = strToTable(str_two)
    return tostring(a[1] - b[1]) .. ' ' .. tostring(a[2] - b[2]) .. ' ' .. tostring(a[3] - b[3])
end

function VectorScale(str, f)
    local a = strToTable(str)
    return tostring(a[1] * f) .. ' ' .. tostring(a[2] * f) .. ' ' .. tostring(a[3] * f)
end

function VectorLength(str)
    local a = strToTable(str)
    return tostring(math.sqrt(a[1] ^ 2 + a[2] ^ 2 + a[3] ^ 2))
end

function VectorNormalize(str)
    local l = tonumber(VectorLength(str))
    return VectorScale(str, 1 / l)
end

function VectorDot(str_one, str_two)
    local a = strToTable(str_one)
    local b = strToTable(str_two)
    return tostring(a[1] * b[1] + a[2] * b[2] + a[3] * b[3])
end

function VectorDist(str_one, str_two)
    return VectorLength(VectorSub(str_one, str_two))
end

function VectorCross(str_one, str_two)
    local a = strToTable(str_one)
    local b = strToTable(str_two)
    return tostring(a[2] * b[3] - a[3] * b[2]) .. ' ' .. tostring(a[3] * b[1] - a[1] * b[3]) .. ' ' .. tostring(a[1] * b[2] - a[2] * b[1])
end