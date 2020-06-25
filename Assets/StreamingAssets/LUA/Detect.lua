-- base Eat func, provided by Delivr

-- function config() is no argument function, 
-- called before main() to establish numper of params for input and output
-- 
function config()
    local nodeName = "Detect"
    local inNode = {}
    local valueNode = {"Tag"}
    local outNode = {"Target"}
    NodeManager.CreateNew(nodeName, inNode,valueNode, outNode)
end


-- function main() is called every state chage
function main(args, slider)
    --todo add eating preferences based on hunger lvl etc. 
    --todo 

    if(Brain.GetDistanceFromTarget() <= 1) then
        --if food is close eat 
        Brain.Eat()
    end

    --if food is in range go to food
    if (Brain.GetDistanceFromTarget() > 1) then
        Brain.MoveTo(Brain.Target.GetPositionOfAxis(0), Brain.Target.GetPositionOfAxis(2))
    end
    --if food is in range eat

end