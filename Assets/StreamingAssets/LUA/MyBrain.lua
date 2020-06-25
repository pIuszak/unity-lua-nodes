-- Brain, provided by Flint Games

-- function config() is no argument function, 
-- called before main() to establish number of params for input and output

function config()
    local nodeName = "MyBrain"
    local inNode = {}
    local valueNode = {}
    local outNode = { "Health", "Hunger", "Stamina" }
    NodeManager.CreateNew(nodeName, inNode,valueNode, outNode)
end


-- function main() is called every state chage
function main(args, slider)
end

