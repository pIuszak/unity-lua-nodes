-- base if func, provided by Delivr

function config()
    
end

function main(args, slider)
    if args[1] < slider  then
        return true
    else
        return false
    end
end
