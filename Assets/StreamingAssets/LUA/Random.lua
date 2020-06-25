-- base if func, provided by Delivr

function config()
    local nodeName = "Random"
    local inNode = {}
    local valueNode = {"min", "max"}
    local outNode = {"Value"}
    NodeManager.CreateNew(nodeName, inNode, valueNode ,outNode)
end

function main(args)
    if args[1] > args[2] then
        return { 1.0, 0.0 }
    else
        return { 0.0, 1.0 }
    end
end
