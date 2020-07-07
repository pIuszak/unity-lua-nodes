-- base if func, provided by Delivr

--function main(args)
--    if args[1] > args[2] then
--        return { 1.0, 0.0 }
--    else
--        return { 0.0, 1.0 }
--    end
--end

function config()
    local nodeName = "if A > B"
    local inNode = {"A", "B", "Action"}
    local valueNode = { }
    local outNode = { "true", "false" }
    NodeManager.CreateNew(nodeName, inNode, valueNode, outNode)
end

local function isempty(s)
    return s == nil or s == ''
end
-- 
function main(A, B)

    if isempty(A) then
        return nil
    end
    if isempty(B) then
        return nil
    end
        if A > B then
            Node.BlockNode(2)
        else
            Node.BlockNode(1)
        end
end

