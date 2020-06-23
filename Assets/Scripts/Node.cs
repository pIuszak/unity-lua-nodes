using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

/*
 * 1 get data from prevous node
 * 2 load lua script and compute or execute Unity API
 * 3 pass data to next node
 */

[ExecuteInEditMode]
public class Node : DragDrop
{
    // [SerializeField] private Node[] InputNodes;
    [SerializeField] private Unit Brain;
    [SerializeField] private NodeSlot[] InNodeSlots;
    [SerializeField] private NodeSlot[] OutNodeSlots; 
    [SerializeField] private Node[] OutputNodes;
    [SerializeField] private Slider MySlider;

    
    [SerializeField] private string FileName = "Test.lua";
    [NaughtyAttributes.ResizableTextArea] [SerializeField] private string LuaCode;

    private void GetData(NodeSlot[] data)
    {
        InNodeSlots = data;
        Test();
    }

    public void PassData()
    {
        foreach (var outputNode in OutputNodes)
        {
            GetData(OutNodeSlots);
        }
    }

    public void Compute()
    {
        Debug.Log("Bip Bop Compute");
        Test();
    }
    
    private void Start()
    {
        LoadLua();
    }

    // extract value list from inNodeSlots
    private int[] WrapData()
    {
        var length = InNodeSlots.Length;
        var ret = new int[length];

        for (var i = 0; i < length; i++)
        {
            ret[i] = InNodeSlots[i].Value;
        }
        
        return ret;
    }
    //this works for simple if and similar
    void Test()
    {
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "LUA");
        filePath = System.IO.Path.Combine(filePath, FileName);
        LuaCode = System.IO.File.ReadAllText(filePath);
        Script script = new Script();
        
        Debug.Log("FOR  : "+ InNodeSlots[0].Name);
        
        // Automatically register all MoonSharpUserData types
        UserData.RegisterAssembly();
        
        //todo dynamic arguments passing to/from LUA script
        //script.Globals["ApiMoveTo"] = (Func<float,float,int>) brain.ApiMoveTo;
        script.Globals["Brain"] = Brain;
        // TODO: CHANGE TARGET TO ARRAY OF UNITS IN RANGE
       // script.Globals["Target"] = Brain.Targ;
        script.DoString(LuaCode);
        
        // if there is a slider, pass his value 
        var slider = MySlider ? MySlider.value : 0f;

  

     DynValue res = script.Call(script.Globals["main"],  new int[2]{1,2}, slider);
     Debug.Log("LUA SAYS : " +res.Number);
    }

    // load lua code associated with this node  
    private void LoadLua()
    {
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "LUA");
        filePath = System.IO.Path.Combine(filePath, FileName);
        LuaCode = System.IO.File.ReadAllText(filePath);

        //  Debug.Log("My LUA Code");
        //  Debug.Log(myLuaCode);

        DynValue res = Script.RunString(LuaCode);
        Debug.Log(res.Number);

        // Instantiate the singleton
        //  new Actions( myLuaCode );
    }
        

    [Button]
    void CallbackTestDebug()
    {
        Debug.Log(CallbackTest());
    }

    private double CallbackTest()
    {
        string scriptCode = @"    
        -- defines a factorial function
        function fact (n)
            if (n == 0) then
                return 1
            else
                return Mul(n, fact(n - 1));
            end
        end";

        Script script = new Script();
        
      //  script.Globals["ApiMoveToRandom"] = (Func<int>) MyBrain.ApiMoveTo;

        script.DoString(scriptCode);

        DynValue res = script.Call(script.Globals["fact"], 4);

        return res.Number;
    }
}

