using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using UnityEngine;

/*
 * 1 get data from prevous node
 * 2 load lua script and compute or execute Unity API
 * 3 pass data to next node
 */

[ExecuteInEditMode]
public class Node : MonoBehaviour
{
    // [SerializeField] private Node[] InputNodes;
    
    [SerializeField] private NodeSlot[] InNodeSlots;
    
    [SerializeField] private Node[] OutputNodes;
    [SerializeField] private NodeSlot[] OutNodeSlots; 

    
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

    private void Compute()
    {
        
    }
    
    private void Start()
    {
        LoadLua();
    }
    
    void Test()
    {
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "LUA");
        filePath = System.IO.Path.Combine(filePath, FileName);
        LuaCode = System.IO.File.ReadAllText(filePath);

        Script script = new Script();

        script.DoString(LuaCode);
        Debug.Log("FOR  : "+InNodeSlots[0].Name);
        DynValue res = script.Call(script.Globals["func"], InNodeSlots[0].Value);

        Debug.Log("LUA SAYS : "+res.Number);
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
}

