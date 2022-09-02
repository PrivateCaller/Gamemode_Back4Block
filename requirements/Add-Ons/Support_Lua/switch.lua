--- Evaluates ``term``, matching the expression's value to the ``cases`` clause, and executes the statements associated with the case.
---
--- To do not execute other cases, return the string 'break'
--- ```lua
--- switch(term, {
---     ['case'] = function()
---         -- ...
---         return "break"
---     end,
---     default = function() -- optional
---         -- ...
---     end
--- })
--- ```
---@param term any
---@param cases table
local function switch(term, cases)
    assert(type(cases) == "table", "bad argument #1 to 'switch': table expected, got "..type(cases))
    local gotoDefault = true

    -- in string/boolean cases
    for value, case in pairs(cases) do
        assert(type(case) == "function", "case "..tostring(case).." is not a function")
        if term == value then
            gotoDefault = false

            local exec = case()

            if exec == "break" then
                break
            end
        end
    end

    if cases.default ~= nil and type(cases.default) == "function" and gotoDefault == true then
        cases.default()
    end
end

local grade = 'A'

switch(grade, {
    ['A'] = function ()
        print("awesome!")
    end,
    ['B'] = function ()
        print("not bad!")
    end,
    ['C'] = function ()
        print('crap.')
    end
})