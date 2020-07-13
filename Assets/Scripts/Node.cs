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
    public List<string> Values = new List<string>();
    public string LuaScript = "Test.lua";
}

[MoonSharpUserData]
[ExecuteInEditMode]
[Serializable]
public class Node : DragDrop
{
    [SerializeField] public Brain Brain;
    [SerializeField] protected List<NodeElement> InNodeSlots = new List<NodeElement>();
    [SerializeField] protected NodeElement[] OutNodeElement = new NodeElement[NodeManager.MaxOutNodes];
    [SerializeField] protected List<NodeElement> ValuesNodes = new List<NodeElement>(NodeManager.MaxOutNodes);
    protected Node[] OutputNodes = new Node[NodeManager.MaxOutNodes];
    [SerializeField] protected List<Node> InputNodes = new List<Node>();

    public bool isBlocked;

    public NodeConfig NodeConfig;
    public Image BgImage;
    [SerializeField] public List<NodeEnvoy> NodeEnvoys = new List<NodeEnvoy>();

    [NaughtyAttributes.ResizableTextArea] [SerializeField]
    protected string LuaCode;

    public bool AlreadyExecuted;
    public bool AlreadyChecked;

    public float NodeWaitTime = 1f;

    public int argCounter = 0;
    public List<DynValue> currentArgs = new List<DynValue>();

    public void SaveValues()
    {
        foreach (NodeElement valuesNode in ValuesNodes)
        {
            NodeConfig.Values.Add(valuesNode.Value);
        }
    }

    public void Repeat()
    {
        NodeManager.Instance.Play();
    }

    public void AddValueNodeElement(NodeElement nodeElement)
    {
        ValuesNodes.Add(nodeElement);
    }

    // new 
    public void AddOutNodeElement(NodeElement nodeElement)
    {
        var x = OutNodeElement.ToList();
        x.Add(nodeElement);
        OutNodeElement = x.ToArray();
    }

    public void ConnectToOtherOutputNodes(Node node, NodeEnvoy nodeEnvoy)
    {
        var x = OutNodeElement.ToList();
        var index = x.ToList().FindIndex(c => c == nodeEnvoy.MyNodeElement);

        OutputNodes[index] = node;
    }

    public void SortOutputNode()
    {
    }

    public void ConnectToOtherInputNodes(Node node)
    {
        InputNodes.Add(node);
    }

    // protected void GetData(NodeElement[] data)
    // {
    //     InNodeSlots = data;
    //     Execute(args);
    // }
    //
    // protected void PassData()
    // {
    //     foreach (var outputNode in OutputNodes)
    //     {
    //         GetData(OutNodeSlots);
    //     }
    // }

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
        // todo 
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
         //Debug.Log("ExecuteC " + this.gameObject.name);

        // import lua scriptbra
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "LUA");
        filePath = System.IO.Path.Combine(filePath, NodeConfig.LuaScript);
        LuaCode = System.IO.File.ReadAllText(filePath);
        var script = new Script();

        // Automatically register all MoonSharpUserData types
        UserData.RegisterAssembly();

        // TODO
        script.Globals["Brain"] = FindObjectOfType<Brain>();
        script.Globals["Node"] = this;
        script.DoString(LuaCode);

        if (ValuesNodes.Count > 0)
        {
            foreach (var nodeConfigValue in ValuesNodes)
            {
                // newArgs.Add(nodeConfigValue);
            }
        }

        //var newArgs = args.ToList();


        // // ------ add arguments from value nodes 
        foreach (var nodeConfigValue in ValuesNodes)
        {
            //     Debug.Log("value node take me home _>>>>>>>> " + nodeConfigValue.Value);
            // TODO BUG HERE 
            currentArgs.Add(DynValue.NewString(nodeConfigValue.Value));
        }

        // ------ add arguments from previous nodes 
        // var x1 =  args;
        // var x2 =  args.Values;
        // var x3 =  args.Values.ToList();
        if (args != null)
        {
            if (args.Values != null)
            {
                var newArgs = args.Values.ToList();
                foreach (var newArg in newArgs)
                {
                    currentArgs.Add(newArg);
                }
            }
        }


        //Debug.Log(NodeConfig.LuaScript + " >>> args >>>" + currentArgs.Count);
        var res = script.Call(script.Globals["main"], currentArgs);

        yield return new WaitForSeconds(NodeWaitTime);

        AlreadyExecuted = true;

        // foreach (var o in currentArgs)
        // {
        //     Debug.Log( " ++++++++ " + o.String);
        // }
        
    //    Debug.Log("@@@@@ "+res.String);
        // execute next nodes 
        foreach (var outputNode in OutputNodes)
        {
            if (!outputNode) continue;
        
            if (outputNode.isBlocked) continue;
                Debug.Log(NodeConfig.LuaScript + ">>> runs >>> " + outputNode.NodeConfig.LuaScript + " >>> val >>>" + currentArgs.Count);
                outputNode.Execute(res.Table);
        }

        currentArgs.Clear();
    }

    public void BlockNode(int val)
    {
        // Debug.Log("++++++++++++++++++++++++++++++++++++++++++++Block Node");
        OutputNodes[val].isBlocked = true;
    }

    public int NodeSetWaitTime(float val)
    {
        // Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! NodeSetWaitTime "+ val);
        NodeWaitTime = val;
        return 0;
    }


    public void ClearAction()
    {
        foreach (var node in OutputNodes)
        {
            if(node) node.isBlocked = false;
         
           
        }

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

        AlreadyExecuted = false;
        AlreadyChecked = false;
    }
}