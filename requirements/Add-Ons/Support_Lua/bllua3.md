# BlLua3
Simple Lua interface for TorqueScript  
by Redo (12878)

## How to install
#### Easy way:
1. Install RedBlocklandLoader from https://gitlab.com/Eagle517/redblocklandloader/-/releases
1. Download and place bllua3.dll in the modules folder

#### Hard way:
1. Have GCC installed
1. Compile and install RedBlocklandLoader
1. Clone this repo
1. Run compile.bat
1. Put the resulting bllua3.dll in the modules folder

## Documentation
#### In TorqueScript
The API exposed to TS consists of only five functions:  
  
`luaeval(string)` - Execute Lua code and return the result  
    `luaeval("print(\"Hello, world.\")");` -> `Hello, world.`  
    `echo(luaeval("return 5*5"));` -> `25`  
  
`luacall(name, args)` - Call a global Lua function and return the result (Much faster than luaeval)  
    `luacall("print", "Hello, world.");` -> `Hello, world.`  
    `echo(luacall("user_square_lua", 5));` -> `25`  
    `echo(luacall("user_multiply_lua", 2, 3));` -> `6`  
  
`luaexec(filename)` - Execute a Lua code file within the Blockland game directory  
    `luaexec("Add-ons/Client_LuaTest/test.lua");` - Executes `test.lua`  
  
`luaset(name, value)` - Set the value of a Lua global variable  
    `luaset("some_lua_var", "hello, world");` - set the Lua variable `some_lua_var` to `"hello, world"`  
  
`luaget(name)` - Get the value of a Lua global variable  
    `echo(luaget("some_lua_var"));` -> `hello, world` (assuming the above code was also run)


#### In Lua
There is also an API exposed to Lua. It provides the following functions:  
  
`ts.eval(string)` - Eval TS code and return the result  
    `ts.eval("echo(\"Hello, world.\")")` -> `Hello, world.`  
    `print(ts.eval("mPow(5, 2);"))` -> `25`  
    This can be used in conjunction with Lua's double bracket syntax to eval multi-line blocks of TS code,  
    useful for writing glue code within Lua files.
  
`ts.call(name, args...)` - Call a TS function and return the result (Much faster than ts.eval)  
    `ts.call("echo", "Hello, world.")` -> `Hello, world.`  
    `local client = ts.call("findClientByName", name)` - Get a client with the given name  
    All return values from TS calls are strings, so you will have to either  
    rely on lua's type coercion or use tonumber if you expect a numeric value.  
    Returned object IDs can be can be passed straight into ts.call or ts.callobj without issue.  
  
`ts.callobj(object, name, args...)` - Call a TS function on an object and return the result  
    `ts.callobj("Sky", "delete")` - Delete the sky - equivalent to `Sky.delete();` in TS  
    `local color = tonumber(ts.callobj(brick, "getColorId"))` - Get the color ID of a brick  
    The object can be an object ID as a number or string, or an object name string.  
  
`ts.set(name, value)` - Set the value of a TS global variable  
    `ts.set("Pref::server::adminPassword", "cake")` - Set the admin password to `cake`  
  
`ts.get(name)` - Get the value of a TS global variable  
    `print(ts.get("pi"))` -> `3.14159` (The value of the TS global variable `$pi`)  
  
`ts.getobj(object, name)` - Get the value of a field in a TS object  
    `if ts.getobj(client, "isAdmin")~="1" then return end` - Abort if the `client` object is not admin  
  
`ts.setobj(object, name, value)` - Set the value of a field in a TS object  
    `ts.setobj(client, "isAdmin", 0)` - Make `client` not admin  
  
`schedule(time, function, [args...])` - Schedule a lua function call using the TS scheduler  
    `schedule(1000, print, "hi")` - print `hi` after 1 second (1000 ms)  
    Returns an identifier which can be used to cancel the schedule  
  
`cancel(schedule)` - Cancel a lua schedule  
    `local quit_sched = schedule(5000, ts.call, "quit")` - Quit after 5 seconds, unless...  
    `cancel(quit_sched)` - Cancel that schedule and don't quit  
  
For convenience, you can eval lua in the in-game console by prepending a `'` (single quote).  
    `echo("hello world from ts");` -> `hello world from ts`  
    `'print("hello world from lua")` -> `hello world from lua`  
  
**Notes about I/O in Lua**  
  
For safety, I/O functions are sanitized; io.open and io.lines  
    accept Blockland-relative paths according to the same rules as for opening files in TS;  
    with the exception that they can also open .lua files and some binary formats.  
    `local f = io.open("config/client/data.txt", "a"); f:write("some data\n"); f:close();`  
    `for line in io.lines("saves/House.bls") do print(line) end`  
  
Lua I/O functions work on files inside zips, but can only read, not write.  
    The current implementation also does not allow Lua to read binary files inside zips  
    (though accessing binary files outside zips is fine), as it cannot handle null bytes  
    or distinguish between LF and CRLF.  
  
The normal Lua dofile function is also modified to accept Blockland-relative paths -  
    it is implemented the same as the TS API's luaexec function.  
    `dofile("Add-ons/Script_LuaTest/test.lua")` - Executes test.lua  
    It is possible to exec lua files inside zips, just like normal.  
  
All Lua I/O functions are able to use relative paths just like in TS.  
    `dofile("./test2.lua")`  
    `local fi = io.open("./data/some_data.dat")`  
    luaexec also works with relative paths.  
    `luaexec("./component.lua");`  
