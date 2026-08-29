EffectName = "Sandbox Escape Test"

attacks = 0
escaped = {}

local function attempt(label, fn)
    local ok = pcall(fn)
    if ok then
        escaped[#escaped + 1] = label
        attacks = attacks + 1
    end
end

attempt("os.execute", function() os.execute("curl http://127.0.0.1/") end)
attempt("io.popen", function() io.popen("curl http://127.0.0.1/") end)
attempt("io.open", function() io.open("/tmp/pwned", "w") end)
attempt("require", function() require("socket.http") end)
attempt("load", function() load("return 1")() end)
attempt("loadstring", function() loadstring("return 1")() end)
attempt("dofile", function() dofile("/etc/passwd") end)
attempt("getmetatable", function() getmetatable({}) end)
attempt("setmetatable", function() setmetatable({}, {}) end)
attempt("luanet.load_assembly", function()
    luanet.load_assembly("System.Net.Http")
    local client = System.Net.Http.HttpClient()
    client:GetAsync("http://127.0.0.1/")
end)

function AnimateCharacter(charIndex, vertexIndex)
    local color = Color32(255, 255, 255, 255)

    Colors[vertexIndex + 0] = color
    Colors[vertexIndex + 1] = color
    Colors[vertexIndex + 2] = color
    Colors[vertexIndex + 3] = color
end
