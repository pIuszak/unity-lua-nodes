using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
[MoonSharpUserData]
public class Node : DragDrop
{
    // [SerializeField] private Node[] InputNodes;
    [SerializeField] protected Unit Brain;
    [SerializeField] protected NodeSlot[] InNodeSlots;
    [SerializeField] protected NodeSlot[] OutNodeSlots; 
    [SerializeField] protected Node[] OutputNodes;
    [SerializeField] protected Slider MySlider;
    
    
    [SerializeField] private string FileName = "Test.lua";
    [NaughtyAttributes.ResizableTextArea] [SerializeField] protected string LuaCode;

    protected void GetData(NodeSlot[] data)
    {
        InNodeSlots = data;
        Execute();
    }

    protected void PassData()
    {
        foreach (var outputNode in OutputNodes)
        {
            GetData(OutNodeSlots);
        }
    }
    protected void Start()
    {
     //   LoadLua();
    }

    // extract value list from inNodeSlots
    protected int[] WrapData()
    {
        var length = InNodeSlots.Length;
        var ret = new int[length];

        for (var i = 0; i < length; i++)
        {
            ret[i] = InNodeSlots[i].Value;
        }
        
        return ret;
    }
    
    public string Name;
    // [SerializeField] protected NodeSlot[] InNodeSlots;
    // [SerializeField] protected NodeSlot[] OutNodeSlots;

    public GameObject NodePrefab;


    public Transform dragdrop;
    public void CreateNew(string nodeName, string[] inNodeSlotsNames, float[] inNodeSlotsValues,
        string[] outNodeSlotsNames, float[] outNodeSlotsValues)
    {

       var node =  Instantiate(NodePrefab,dragdrop);
       node.GetComponent<NodeConstructor>().CreateNode(nodeName,inNodeSlotsNames,inNodeSlotsValues,
       outNodeSlotsNames,  outNodeSlotsValues);
        
        
 
    }
    
    [UsedImplicitly]
    public void ConfigureNode()
    {
        
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "LUA");
        filePath = System.IO.Path.Combine(filePath, FileName);
        LuaCode = System.IO.File.ReadAllText(filePath);
        Script script = new Script();

        // Automatically register all MoonSharpUserData types
        UserData.RegisterAssembly();
        //todo dynamic arguments passing to/from LUA script
        //script.Globals["ApiMoveTo"] = (Func<float,float,int>) brain.ApiMoveTo;
        script.Globals["Node"] = this;
        // TODO: CHANGE TARGET TO ARRAY OF UNITS IN RANGE
        // script.Globals["Target"] = Brain.Targ;
        script.DoString(LuaCode);
        // if there is a slider, pass his value 
        var slider = MySlider ? MySlider.value : 0f;
        DynValue res = script.Call(script.Globals["config"]);
        

    }
    //this works for simple if and similar
    protected void Execute()
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
    protected void LoadLua()
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
    protected void CallbackTestDebug()
    {
        Debug.Log(CallbackTest());
    }

    protected double CallbackTest()
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