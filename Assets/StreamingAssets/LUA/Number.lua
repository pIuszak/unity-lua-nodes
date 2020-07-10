function config()
    local nodeName = "Number"
    local inNode = {}
    local valueNode = { "float" }
    local outNode = { "Value" }
    NodeManager.CreateNew(nodeName, inNode, valueNode, outNode)
end

function main(args)
    return {tonumber(args[1])}
end
