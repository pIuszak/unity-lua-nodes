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
public class Neuron : DragDrop
{
    public bool IsNeuronBlocked;
    [SerializeField] public Creature Creature;
    protected List<NeuronPart> NeuronPartsOut = new List<NeuronPart>();
    protected List<NeuronPart> NeuronValues = new List<NeuronPart>();
    protected List<Neuron> NeuronsOut = new List<Neuron>();
    protected List<Neuron> NeuronsIn = new List<Neuron>();
    public NeuronConfig NeuronConfig;
    public Image BgImage;
    [SerializeField] public List<Synapse> Synapses = new List<Synapse>();

    [NaughtyAttributes.ResizableTextArea] [SerializeField]
    protected string LuaCode;

    public bool AlreadyExecuted;
    public bool AlreadyChecked;
    public float WaitTime = 1f;
    public int LocalIndex;

    public List<DynValue> CurrentArgs = new List<DynValue>();

    private void Awake()
    {
        Creature = GetComponentInParent<Creature>();
    }

    public void SaveValues()
    {
        foreach (var valuesNode in NeuronValues)
        {
            NeuronConfig.Values.Add(valuesNode.Value);
        }
    }

    public void AddNeuronValue(NeuronPart neuronPart)
    {
        NeuronValues.Add(neuronPart);
    }

    // new 
    public void AddNeuronPartOut(NeuronPart neuronPart)
    {
        NeuronPartsOut.Add(neuronPart);
        NeuronPartsOut = NeuronPartsOut.OrderBy(o => o.Index).ToList();
    }

    // Add and sort to match Synapse 
    public void ConnectToOtherOutputNodes(int index, Neuron neuron)
    {
        neuron.LocalIndex = index;
        NeuronsOut.Add(neuron);
        NeuronsOut = NeuronsOut.OrderBy(o => o.LocalIndex).ToList();
    }


    public void ConnectToOtherInputNodes(Neuron neuron)
    {
        NeuronsIn.Add(neuron);
    }

    public void SetNewValues(string[] val)
    {
        for (int i = 0; i < NeuronValues.Count; i++)
        {
            NeuronValues[i].SetValue(val[i]);
        }
    }

    public void SetNewEnvoysPos(Vector3[] pos)
    {
        for (int i = 0; i < Synapses.Count; i++)
        {
            Synapses[i].transform.localPosition = pos[i];
        }
    }

    public void SavePosition()
    {
        NeuronConfig.Position = transform.localPosition;
        foreach (var envoy in Synapses)
        {
            NeuronConfig.SynapsesPositions.Add(envoy.transform.localPosition);
        }
    }

    public void Execute(Table args)
    {
        if (AlreadyExecuted && NeuronsIn.Count != 1) return;
        // check if every input node is fulfilled
        if (!AlreadyChecked && NeuronsIn.Count != 1)
        {
            foreach (Neuron node in NeuronsIn)
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
        filePath = System.IO.Path.Combine(filePath, NeuronConfig.Behaviour);
        LuaCode = System.IO.File.ReadAllText(filePath);
        var script = new Script();
        // Automatically register all MoonSharpUserData types
        // UserData.RegisterAssembly();

        // add lua scripts access to Voxelland API 
        script.Globals["Brain"] = Creature;
        script.Globals["Neuron"] = this;
        script.DoString(LuaCode);

        // add arguments from value nodes 
        foreach (var nodeConfigValue in NeuronValues)
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
        yield return new WaitForSeconds(WaitTime);
        AlreadyExecuted = true;

        if (NeuronsOut.Count <= decimal.Zero)
        {
            Creature.Repeat();
        }

        // move to next node/nodes
        foreach (var outputNode in NeuronsOut.Where(outputNode => !outputNode.IsNeuronBlocked))
        {
            outputNode.Execute(res.Table);
        }

        CurrentArgs.Clear();
    }

    public void BlockNode(int val)
    {
        if (NeuronsOut[val]) NeuronsOut[val].IsNeuronBlocked = true;
    }

    public int NodeSetWaitTime(float val)
    {
        WaitTime = val;
        return 0;
    }

    public void ClearAction()
    {
        foreach (var node in NeuronsOut.Where(node => node))
        {
            node.IsNeuronBlocked = false;
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
        foreach (var node in NeuronsOut)
        {
            node.ClearAction();
        }

        AlreadyExecuted = false;
        AlreadyChecked = false;
    }
}