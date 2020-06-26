using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class NodeManager : MonoBehaviour
{
    public GameObject NodePrefab;
    public Transform dragdrop;

    [NaughtyAttributes.ResizableTextArea] [SerializeField]
    protected string LuaCode;

    public List<Node> NodesMemory = new List<Node>();
    private string currentScriptName;
    private Vector3 currentScriptPosition;
    
     // todo merge with CreateNodeFromConfig !!!!
    [UsedImplicitly]
    public void CreateNode(string FileName)
    {
        currentScriptName = FileName;
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "LUA");
        filePath = System.IO.Path.Combine(filePath, FileName);
        LuaCode = System.IO.File.ReadAllText(filePath);
        Script script = new Script();

        // Automatically register all MoonSharpUserData types
        UserData.RegisterAssembly();
        //todo dynamic arguments passing to/from LUA script
        //script.Globals["ApiMoveTo"] = (Func<float,float,int>) brain.ApiMoveTo;
        script.Globals["NodeManager"] = this;
        // TODO: CHANGE TARGET TO ARRAY OF UNITS IN RANGE
        // script.Globals["Target"] = Brain.Targ;
        script.DoString(LuaCode);
        // if there is a slider, pass his value 
        // var slider = MySlider ? MySlider.value : 0f;
        DynValue res = script.Call(script.Globals["config"]);
    }


    [UsedImplicitly]
    // invoked in config() in every lua script+
    public void CreateNew(string nodeName, string[] inNodeSlotsNames, string[] valNodeSlotsNames,
        string[] outNodeSlotsNames)
    {
        var node = Instantiate(NodePrefab, dragdrop);
        node.transform.localPosition = currentScriptPosition;
        // node.transform.localPosition.
        var newName = currentScriptName.Replace(".lua", "_node");
        
        node.name = newName;
        
        node.GetComponent<Node>().NodeConfig.LuaScript = currentScriptName;
        node.GetComponent<Node>().LoadLua();
        NodesMemory.Add(node.GetComponent<Node>());


        node.GetComponent<NodeConstructor>()
            .CreateNode(nodeName, inNodeSlotsNames, valNodeSlotsNames, outNodeSlotsNames);
    }
    
    [UsedImplicitly]
    public void SaveToJson()
    {
        FileUtilities.ClearFile("save.txt");
        foreach (var node in NodesMemory)
        {
            var json = JsonUtility.ToJson(node.NodeConfig);
            node.SavePosition();
            FileUtilities.WriteString(json, "save.txt");
        }
    }
    
    [UsedImplicitly]
    public void LoadFromJson()
    {
        var lines = FileUtilities.ReadString().ReadToEnd()
            .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

       foreach (var s in lines)
       {
           var newNode = JsonUtility.FromJson<NodeConfig>(s);
             currentScriptPosition = newNode.Position;
             CreateNode(newNode.LuaScript);
       }
    }
}