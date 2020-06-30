using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

/*
 * 1 get data from prevous node
 * 2 load lua script and compute or execute Unity API
 * 3 pass data to next node
 */

[Serializable]
public class NodeConfig
{
    public Vector3 Position = Vector3.zero;
    
    public List<Vector3> EnvoyPositions;
    public List<string> Values;

    public string LuaScript = "Test.lua";
}

[ExecuteInEditMode]
[Serializable]
public class Node : DragDrop
{
    [SerializeField] public Unit Brain;
    [SerializeField] protected NodeElement[] InNodeSlots;
    [SerializeField] protected NodeElement[] OutNodeSlots;
    [SerializeField] protected List<Node> OutputNodes = new List<Node>();
    [SerializeField] protected List<Node> InputNodes = new List<Node>();

    public NodeConfig NodeConfig;

    [SerializeField] public List<NodeEnvoy> NodeEnvoys = new List<NodeEnvoy>();

    [NaughtyAttributes.ResizableTextArea] [SerializeField]
    protected string LuaCode;

    public bool AlreadyExecuted;
    public bool AlreadyChecked;

    public void ConnectToOtherOutputNodes(Node node)
    {
        OutputNodes.Add(node);
    }

    public void ConnectToOtherInputNodes(Node node)
    {
        InputNodes.Add(node);
    }

    protected void GetData(NodeElement[] data)
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

    public void SetNewEnvoysPos(Vector3[] pos)
    {
        for (int i = 0; i < NodeEnvoys.Count; i++)
        {
            NodeEnvoys[i].transform.localPosition = pos[i];
        }
    }

    public void SavePosition()
    {
        NodeConfig.Position = transform.localPosition;
        foreach (var envoy in NodeEnvoys)
        {
            NodeConfig.EnvoyPositions.Add(envoy.transform.localPosition);
        }
    }
    
    public void Execute(params object[] args)
    {
        if (AlreadyExecuted) return;
        
        // check if every input node is fulfilled
        if (!AlreadyChecked)
        {
            foreach (Node node in InputNodes)
            {
                node.Execute();
                AlreadyChecked = true;
            }

            if (AlreadyChecked) return;
         
        }
        AlreadyExecuted = true;
 
        // import lua script
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "LUA");
        filePath = System.IO.Path.Combine(filePath, NodeConfig.LuaScript);
        LuaCode = System.IO.File.ReadAllText(filePath);
        var script = new Script();
        
        // Automatically register all MoonSharpUserData types
        UserData.RegisterAssembly();
        
        // TODO
        script.Globals["Brain"] = FindObjectOfType<Unit>();
        script.DoString(LuaCode);
        
        DynValue res = script.Call(script.Globals["main"], args);
        
        // execute next nodes 
        foreach (var outputNode in OutputNodes)
        {
            outputNode.Execute(res.Number);
        }
    }

}