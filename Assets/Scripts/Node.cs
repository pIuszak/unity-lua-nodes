using System;
using System.Collections.Generic;
using System.Linq;
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
    public string LuaScript = "Test.lua";
}

[ExecuteInEditMode]
[Serializable]
public class Node : DragDrop
{
    [SerializeField] public Brain Brain;
    [SerializeField] protected NodeElement[] InNodeSlots;
    [SerializeField] protected NodeElement[] OutNodeSlots;
    [SerializeField] protected List<NodeElement> ValuesNodes = new List<NodeElement>();
    [SerializeField] protected List<Node> OutputNodes = new List<Node>();
    [SerializeField] protected List<Node> InputNodes = new List<Node>();

    public NodeConfig NodeConfig;

    [SerializeField] public List<NodeEnvoy> NodeEnvoys = new List<NodeEnvoy>();

    [NaughtyAttributes.ResizableTextArea] [SerializeField]
    protected string LuaCode;

    public bool AlreadyExecuted;
    public bool AlreadyChecked;

    public void AddValueNodeElement(NodeElement nodeElement)
    {
        ValuesNodes.Add(nodeElement);
    }
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
    
    public void 
    
    public void Execute(params object[] args)
    {
      
        if (AlreadyExecuted) return;
        
        // todo 
        // check if every input node is fulfilled
        if (!AlreadyChecked)
        {
            foreach (Node node in InputNodes)
            {
                AlreadyChecked = true;
                node.Execute();
                
            }
            if (AlreadyChecked) return;
        }
        // import lua script
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "LUA");
        filePath = System.IO.Path.Combine(filePath, NodeConfig.LuaScript);
        LuaCode = System.IO.File.ReadAllText(filePath);
        var script = new Script();
        
        // Automatically register all MoonSharpUserData types
        UserData.RegisterAssembly();
        
        // TODO
        script.Globals["Brain"] = FindObjectOfType<Brain>();
        script.DoString(LuaCode);
     
        if (ValuesNodes.Count > 0)
        {
            
            foreach (var nodeConfigValue in ValuesNodes)
            {
                // newArgs.Add(nodeConfigValue);
               
            }
        }
        var newArgs = args.ToList();
        foreach (var nodeConfigValue in ValuesNodes)
        {
            // newArgs.Add(nodeConfigValue);
            Debug.Log(nodeConfigValue + " YYY ");
            newArgs.Add(nodeConfigValue.Value);
        }

        args =  newArgs.ToArray();
        foreach (var x in args)
        {
            // newArgs.Add(nodeConfigValue);
            Debug.Log(x + " ZZZ ");
        }
        
        // foreach (var o in newArgs)
        // {
        //     Debug.Log(NodeConfig.LuaScript + " YYY "+o.ToString());
        // }
        //
        // foreach (var o in args)
        // {
        //     Debug.Log(NodeConfig.LuaScript + " TTT "+o.ToString());
        // }
        
        DynValue res = script.Call(script.Globals["main"], args);
       // Debug.Log(NodeConfig.LuaScript + "  >>> "+res.Number);
        
        AlreadyExecuted = true;
        
        // execute next nodes 
        foreach (var outputNode in OutputNodes)
        {
            Debug.Log(NodeConfig.LuaScript + ">>> runs >>> " +outputNode.NodeConfig.LuaScript + " >>> val >>>"+ res.Number);
            outputNode.Execute(res.Number);
        }
    }
    
    
    
    public void ClearAction()
    {
        AlreadyExecuted = false;
        AlreadyChecked = false;
    }

}