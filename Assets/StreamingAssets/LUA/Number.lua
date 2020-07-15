function config()
    local nodeName = "Number"
    local inNode = {}
    local valueNode = { "float" }
    local outNode = { "Value" }
    Brain.CreateNewNeuron(nodeName, inNode, valueNode, outNode)
end

function main(args)
    return {tonumber(args[1])}
end
