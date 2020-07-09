﻿using System;
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
    public List<string> Values;
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
    public List<object> currentArgs = new List<object>();
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

    public void Execute(params object[] args)
    {
        if (AlreadyExecuted && InputNodes.Count != 1) return;
        // todo 
        // check if every input node is fulfilled
        if (!AlreadyChecked && InputNodes.Count != 1)
        {
            foreach (Node node in InputNodes)
            {
                AlreadyChecked = true;
                if (node != this) node.Execute();
            }

            if (AlreadyChecked) return;
        }

        StartCoroutine(ExecuteC(args));
    }
    

    public IEnumerator ExecuteC(params object[] args)
    {
        yield return new WaitForSeconds(1f);
       // Debug.Log("ExecuteC " + this.gameObject.name);

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
        // ------ add arguments from value nodes 
        foreach (var nodeConfigValue in ValuesNodes)
        {
            currentArgs.Add(nodeConfigValue.Value);
        }
        
        // ------ add arguments from previous nodes 
        var newArgs = args.ToList();
        foreach (var newArg in newArgs)
        {
            currentArgs.Add(newArg);
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
        //  Debug.Log(NodeConfig.LuaScript + "  >>> "+ args.Length);
        
        DynValue res = script.Call(script.Globals["main"], currentArgs.ToArray());
        Debug.Log(NodeConfig.LuaScript + " >>> args >>>" + currentArgs.Count);
        yield return new WaitForSeconds(NodeWaitTime);

        AlreadyExecuted = true;
        
        // execute next nodes 
        foreach (var outputNode in OutputNodes)
        {
            if (outputNode.isBlocked) continue;
            Debug.Log(NodeConfig.LuaScript + ">>> runs >>> " + outputNode.NodeConfig.LuaScript + " >>> val >>>" + currentArgs.Count);
            outputNode.Execute(res.Tuple);
        }
        currentArgs.Clear();
    }

    public void BlockNode(int val)
    { 
        Debug.Log("++++++++++++++++++++++++++++++++++++++++++++Block Node");
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