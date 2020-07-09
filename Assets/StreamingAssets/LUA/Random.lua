-- base if func, provided by Delivr

function config()
    local nodeName = "Random"
    local inNode = {}
    local valueNode = { "min", "max" }
    local outNode = { "Target" }
    NodeManager.CreateNew(nodeName, inNode, valueNode, outNode)
end

function main(min, max)
    return { math.random(min, max), math.random(min, max) }
end
