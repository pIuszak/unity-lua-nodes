-- base Eat func, provided by Delivr

-- function config() is no argument function, 
-- called before main() to establish numper of params for input and output
-- 

-- base move func, provided by Delivr

-- function config() is no argument function, 
-- called before main() to establish numper of params for input and output
-- base move func, provided by Delivr

-- function config() is no argument function, 
-- called before main() to establish numper of params for input and output
function config()
    local nodeName = "Sleep"
    local inNode = { "Action", "Value" }
    local valueNode = {}
    local outNode = {"Action"}
    NodeManager.CreateNew(nodeName, inNode, valueNode, outNode)
end

-- function main() is called every state chage
function main(args)
    Brain.Sleep(args)
    Node.NodeSetWaitTime(args)
end