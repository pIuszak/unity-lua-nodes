-- Brain, provided by Flint Games

-- function config() is no argument function, 
-- called before main() to establish number of params for input and output

function config()
    local nodeName = "Hunger"
    local inNode = {}
    local valueNode = {}
    local outNode = { "Value"}
    Brain.CreateNewNeuron(nodeName, inNode, valueNode, outNode)
end

function main(args)
    return Brain.Hunger
end
