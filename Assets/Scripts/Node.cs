using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;

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

[MoonSharpUserData]
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
    public Image BgImage;
    [SerializeField] public List<NodeEnvoy> NodeEnvoys = new List<NodeEnvoy>();

    [NaughtyAttributes.ResizableTextArea] [SerializeField]
    protected string LuaCode;

    public bool AlreadyExecuted;
    public bool AlreadyChecked;
    
    public float NodeWaitTime = 1f;

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

     public void Execute(params object[] args)
     {
         StartCoroutine(ExecuteC(args));
     }
    
    
    public IEnumerator ExecuteC(params object[] args)
    {
      
        if (AlreadyExecuted) yield break;
        
        // todo 
        // check if every input node is fulfilled
        if (!AlreadyChecked)
        {
            foreach (Node node in InputNodes)
            {
                AlreadyChecked = true;
                if (node != this) node.Execute();
            }
            if (AlreadyChecked) yield break;
        }
        // import lua scriptbra
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "LUA");
        filePath = System.IO.Path.Combine(filePath, NodeConfig.LuaScript);
        LuaCode = System.IO.File.ReadAllText(filePath);
        var script = new Script();
        
        // Automatically register all MoonSharpUserData types
        UserData.RegisterAssembly();
        
        // TODO
        script.Globals["Brain"] = FindObjectOfType<Brain>();
        //script.Globals["Node"] = this;
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
            newArgs.Add(nodeConfigValue.Value);
        }

        args =  newArgs.ToArray();

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
        
       yield return new WaitForSeconds(NodeWaitTime);
       
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
    
    public void NodeClearAction()
    {
        StartCoroutine(ClearActionC());
    }
    IEnumerator ClearActionC()
    {
        yield return new WaitForSeconds(1f);

        foreach (Node node in OutputNodes)
        {
            node.ClearAction();
        }
        
        Debug.Log("ClearAction");
        AlreadyExecuted = false;
        AlreadyChecked = false;
    }

}