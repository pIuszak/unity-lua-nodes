-- base move func, provided by Delivr

-- function config() is no argument function, 
-- called before main() to establish numper of params for input and output
function config()
    local nodeName = "Repeat"
    local inNode = { "Action" }
    local valueNode = {}
    local outNode = {}
    NodeManager.CreateNew(nodeName, inNode, valueNode, outNode)
end

-- function main() is called every state chage
function main(args)
    Node.Repeat()
    --Brain.MoveTo(args, -args)

end
