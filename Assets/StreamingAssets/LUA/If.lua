-- base if func, provided by Delivr

function config()
    local nodeName = "A > B"
    local inNodesNames = {"A","B"}
    local inNodesValues = {0.0,0.0}
    local outNodesNames = {"true", "false"}
    local outNodesValues = {0.0, 0.0}
    Node.CreateNew(nodeName, inNodesNames , inNodesValues, outNodesNames, outNodesValues)
end

function main(args)
    if args[1] > args[2]  then
        return true
    else
        return false
    end
end
