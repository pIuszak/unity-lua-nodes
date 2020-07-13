-- base if func, provided by Delivr

--function main(args)
--    if args[1] > args[2] then
--        return { 1.0, 0.0 }
--    else
--        return { 0.0, 1.0 }
--    end       
--end

function config()
    local nodeName = "Detect"
    local inNode = { "Action" }
    local valueNode = { "Tag" }
    local outNode = { "true", "false" }
    NodeManager.CreateNew(nodeName, inNode, valueNode, outNode)
end

local function isempty(s)
    return s == nil or s == ''
end
-- 
function main(args)
    
    if isempty(Brain.Detect("Egg")) then
        Node.BlockNode(0)
        return 
    else
        Node.BlockNode(1)
        return { 10, 14 }
    end
end

