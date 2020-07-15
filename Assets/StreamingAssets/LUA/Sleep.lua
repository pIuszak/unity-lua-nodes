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
    Brain.CreateNewNeuron(nodeName, inNode, valueNode, outNode)
end
local function isempty(s)
    return s == nil or s == ''
end
-- function main() is called every state chage
function main(args)
    if isempty(args[1]) then
        return nil
    end
    
    Brain.Sleep(args[1])
    --Node.NodeSetWaitTime(args[1])
end