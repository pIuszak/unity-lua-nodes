using System;
using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

public class RougeCraftApi : MonoBehaviour
{
    private void Start()
    {
	    string code = @"
	return function()
		local x = 0
		while true do
			x = x + 1
			coroutine.yield(x)
		end
	end
	";

// Load the code and get the returned function
	    Script script = new Script();
	    DynValue function = script.DoString(code);

// Create the coroutine in C#
	    DynValue coroutine = script.CreateCoroutine(function);

// Resume the coroutine forever and ever..
	    while (true)
	    {
		    DynValue x = coroutine.Coroutine.Resume();
		    Console.WriteLine("{0}", x);
	    }
 
    }
}
