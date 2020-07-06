-- base move func, provided by Delivr

-- function config() is no argument function, 
-- called before main() to establish numper of params for input and output
function config()
    local nodeName = "Repeat"
    local inNode = { "Action" }
    local valueNode = {}
    local outNode = {"Action"}
    NodeManager.CreateNew(nodeName, inNode, valueNode, outNode)
end

-- function main() is called every state chage
function main()
    Node.NodeSetWaitTime(10)
    --Brain.MoveTo(args, -args)

end
