-- Brain, provided by Flint Games

-- function config() is no argument function, 
-- called before main() to establish number of params for input and output

function config()
    local nodeName = "Health"
    local inNode = {}
    local valueNode = {}
    local outNode = { "Value"}
    Brain.CreateNewNeuron(nodeName, inNode, valueNode, outNode)
end

function main(args)
    return Brain.Health
end
