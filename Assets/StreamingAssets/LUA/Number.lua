-- base Eat func, provided by Delivr

-- function config() is no argument function, 
-- called before main() to establish numper of params for input and output
        
function config()
    local nodeName = "Number"
    local inNode = {}
    local valueNode = {"Value"}
    local outNode = {"Number"}
    NodeManager.CreateNew(nodeName, inNode, valueNode ,outNode)
end

-- function main() is called every state chage
function main(args)
    return 10
end