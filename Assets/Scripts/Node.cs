using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;

[MoonSharpUserData]
[ExecuteInEditMode]
[Serializable]
public class Node : DragDrop
{
    [SerializeField] public Brain Brain;
    protected List<NodeElement>  OutNodeElement =  new List<NodeElement>();
     protected List<NodeElement> ValuesNodes = new List<NodeElement>();
     protected List<Node> OutputNodes = new List<Node>();
     protected List<Node> InputNodes = new List<Node>();
        
    public bool isBlocked;

    public NodeConfig NodeConfig;
    public Image BgImage;
    [SerializeField] public List<NodeEnvoy> NodeEnvoys = new List<NodeEnvoy>();

    [NaughtyAttributes.ResizableTextArea] [SerializeField]
    protected string LuaCode;

    public bool AlreadyExecuted;
    public bool AlreadyChecked;

    public float NodeWaitTime = 1f;

    public int LocalIndex;
    public List<DynValue> CurrentArgs = new List<DynValue>();

    private void Awake()
    {
        Brain = GetComponentInParent<Brain>();
    }

    public void SaveValues()
    {
        foreach (var valuesNode in ValuesNodes)
        {
            NodeConfig.Values.Add(valuesNode.Value);
        }
    }
    public void AddValueNodeElement(NodeElement nodeElement)
    {
        ValuesNodes.Add(nodeElement);
    }

    // new 
    public void AddOutNodeElement(NodeElement nodeElement)
    {
        OutNodeElement.Add(nodeElement);
        OutNodeElement = OutNodeElement.OrderBy(o => o.Index).ToList();
    }
    
    // Add and sort to match NodeElement 
    public void ConnectToOtherOutputNodes(int index, Node node)
    {
        node.LocalIndex = index;
        OutputNodes.Add(node);
        OutputNodes = OutputNodes.OrderBy(o => o.LocalIndex).ToList();
    }
    
    
    public void ConnectToOtherInputNodes(Node node)
    {
        InputNodes.Add(node);
    }
    
    public void SetNewValues(string[] val)
    {
        for (int i = 0; i < ValuesNodes.Count; i++)
        {
            ValuesNodes[i].SetValue(val[i]);
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

    public void Execute(Table args)
    {
        if (AlreadyExecuted && InputNodes.Count != 1) return;
        // check if every input node is fulfilled
        if (!AlreadyChecked && InputNodes.Count != 1)
        {
            foreach (Node node in InputNodes)
            {
                AlreadyChecked = true;
                if (node != this) node.Execute(args);
            }

            if (AlreadyChecked) return;
        }

        StartCoroutine(ExecuteC(args));
    }


    public IEnumerator ExecuteC(Table args)
    {
        yield return new WaitForSeconds(1f);

        // import lua script bra
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "LUA");
        filePath = System.IO.Path.Combine(filePath, NodeConfig.LuaScript);
        LuaCode = System.IO.File.ReadAllText(filePath);
        var script = new Script();
        // Automatically register all MoonSharpUserData types
        UserData.RegisterAssembly();
        
        // add lua scripts access to Voxelland API 
        script.Globals["Brain"] = Brain;
        script.Globals["Node"] = this;
        script.DoString(LuaCode);
        
        // add arguments from value nodes 
        foreach (var nodeConfigValue in ValuesNodes)
        {
            CurrentArgs.Add(DynValue.NewString(nodeConfigValue.Value));
        }
        
        // add arguments from previous nodes 
        if (args?.Values != null)
        {
            var newArgs = args.Values.ToList();
            foreach (var newArg in newArgs)
            {
                CurrentArgs.Add(newArg);
            }
        }
        
        // call lua script
        var res = script.Call(script.Globals["main"], CurrentArgs);
        
        // wait 
        yield return new WaitForSeconds(NodeWaitTime);
        AlreadyExecuted = true;

        if (OutputNodes.Count <= decimal.Zero)
        {
            Brain.Repeat();
        }
        
        // move to next node/nodes
        foreach (var outputNode in OutputNodes.Where(outputNode => !outputNode.isBlocked))
        {
            outputNode.Execute(res.Table);
        }
        
        CurrentArgs.Clear();
    }

    public void BlockNode(int val)
    {
        if (OutputNodes[val]) OutputNodes[val].isBlocked = true;
    }

    public int NodeSetWaitTime(float val)
    {
        NodeWaitTime = val;
        return 0;
    }
    
    public void ClearAction()
    {
        foreach (var node in OutputNodes.Where(node => node))
        {
            node.isBlocked = false;
        }
        AlreadyExecuted = false;
        AlreadyChecked = false;
    }

    public void NodeClearAction()
    {
        StartCoroutine(ClearActionC());
    }

    private IEnumerator ClearActionC()
    {
        yield return new WaitForSeconds(1f);
        foreach (var node in OutputNodes)
        {
            node.ClearAction();
        }
        AlreadyExecuted = false;
        AlreadyChecked = false;
    }
}