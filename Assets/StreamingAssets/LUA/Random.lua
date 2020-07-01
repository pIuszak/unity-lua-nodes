-- base if func, provided by Delivr

function config()
    local nodeName = "Random"
    local inNode = {}
    local valueNode = { "min", "max" }
    local outNode = { "Target" }
    NodeManager.CreateNew(nodeName, inNode, valueNode, outNode)
end

--todo for test 
function main(min, max)
    return math.random(min, max);
    -- return { math.random(-10, 10), math.random(-10, 10) }
    --    if args[1] > args[2] then
    --        return { 1.0, 0.0 }
    --    else
    --        return { 0.0, 1.0 }
    --    end
end
