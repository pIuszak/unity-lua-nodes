-- base if func, provided by Delivr

function config()
    local nodeName = "Random"
    local inNode = {}
    local valueNode = { "min", "max" }
    local outNode = { "Target" }
    Brain.CreateNewNeuron(nodeName, inNode, valueNode, outNode)
end

function main(args)
    return { math.random(tonumber(args[1]), tonumber(args[2])), math.random(tonumber(args[1]), tonumber(args[2])) }
end
