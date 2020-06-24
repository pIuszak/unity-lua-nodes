-- base Eat func, provided by Delivr

-- function config() is no argument function, 
-- called before main() to establish numper of params for input and output
-- 
function config()
end

-- function main() is called every state chage
function main(args, slider)
    --todo add eating preferences based on hunger lvl etc. 
    --todo 
    if(Brain.GetDistanceFromTarget() <= 1) then
        Brain.Attack()
    end

    --if unit is in range go to unit
    if (Brain.GetDistanceFromTarget() > 1) then
        Brain.MoveTo(Brain.Target.GetPositionOfAxis(0), Brain.Target.GetPositionOfAxis(2))
    end
    --if food is in range eat
  
end