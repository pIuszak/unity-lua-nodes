using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using MoonSharp.Interpreter;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

[MoonSharpUserData]
public class Brain : MonoBehaviour
{
    [Header("Brain")]
    public GameObject NeuronPrefab;
    public Transform DragDropRoot;
    public Transform DockRoot;
    public GameObject DockButtonPrefab;
    public List<Neuron> Neurons = new List<Neuron>();
    private string currentScriptName;
    private Vector3 currentScriptPosition;
    private Neuron startNeuron;
    
    private void Start()    
    {
        InitializeDockButtons();
    }

    // this method invokes 
    public void Play()
    {
        // run start 
        foreach (var node in Neurons)
        {
            node.ClearAction();
        }

        startNeuron.Execute(new Table(new Script()));
    }
    
    [UsedImplicitly]
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void InitializeDockButtons()
    {
        var info = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "LUA"));
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            CreateDockButton(file.Name);
        }
    }
    private void CreateDockButton(string luaScriptName)
    {
        if (luaScriptName.Contains(".meta")) return;

        var cleanName = luaScriptName.Replace(".lua", "");
        var go = Instantiate(DockButtonPrefab, DockRoot);
        var dockButton = go.GetComponent<DockButton>();
        dockButton.name = cleanName;
        dockButton.MyName.text = cleanName;
        dockButton.MyButton.onClick.AddListener(delegate { CreateNeuron(luaScriptName); });
        var img = Resources.Load<Sprite>("Icons/" + cleanName);
        var defImg = Resources.Load<Sprite>("Icons/Default");
        dockButton.MyIcon.sprite = img != null ? img : defImg;
        dockButton.MyIcon.SetNativeSize();
    }
    private void CreateNeuron(string fileName)
    {
        CreateNeuron(fileName, new List<string>());
    }
    
    [UsedImplicitly]
    public void CreateNeuron(string fileName, List<string> values)
    {
        currentScriptName = fileName;
        var filePath = Path.Combine(Application.streamingAssetsPath, "LUA");
        filePath = Path.Combine(filePath, fileName);
        Debug.Log(filePath);
        var luaCode = File.ReadAllText(filePath);
        var script = new Script();

        // Automatically register all MoonSharpUserData types
        UserData.RegisterAssembly();
        script.Globals["Brain"] = this;
        script.DoString(luaCode);
        script.Call(script.Globals["config"]);
    }

    [UsedImplicitly]
    // invoked in config() in every lua script+
    public void CreateNewNeuron(string nodeName, string[] inNodeSlotsNames, string[] valNodeSlotsNames,
        string[] outNodeSlotsNames)
    {
        var node = Instantiate(NeuronPrefab, DragDropRoot);
        node.transform.localPosition = currentScriptPosition;


        // node.transform.localPosition.
        var newName = currentScriptName.Replace(".lua", "_node");

        node.name = newName;
        node.GetComponent<Neuron>().NeuronConfig.Behaviour = currentScriptName;
        
        if (nodeName == "Start")
        {
            startNeuron = node.GetComponent<Neuron>();
        }

        Neurons.Add(node.GetComponent<Neuron>());
        var bgName = currentScriptName.Replace(".lua", "");

        var img = Resources.Load<Sprite>("Icons/" + bgName);
        var defImg = Resources.Load<Sprite>("Icons/Default");
        node.GetComponent<Neuron>().BgImage.sprite = img != null ? img : defImg;


        node.GetComponent<NeuronConstructor>()
            .CreateNode(nodeName, inNodeSlotsNames, valNodeSlotsNames, outNodeSlotsNames);
    }
    

    [UsedImplicitly]
    public void SaveToJson()
    {
        FileUtilities.ClearFile("save.txt");
        foreach (var node in Neurons)
        {
            node.SavePosition();
            node.SaveValues();
            var json = JsonUtility.ToJson(node.NeuronConfig);

            FileUtilities.WriteString(json, "save.txt");
        }
    }

    [UsedImplicitly]
    public void LoadFromJson()
    {
        var lines = FileUtilities.ReadString().ReadToEnd()
            .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        // create nodes 
        foreach (var s in lines)
        {
            var newNode = JsonUtility.FromJson<NeuronConfig>(s);
            currentScriptPosition = newNode.Position;
            CreateNeuron(newNode.Behaviour, newNode.Values);
        }
        
        // delayed apply envoys and values  
        StartCoroutine(ApplyNodeElements(lines));

    }

    [Button]
    [UsedImplicitly]
    // snap all nodes to grid
    public void SmartSort()
    {
        const float startY = 284.0462f;
        var memX = -610.7512f;
        var memY = 284.0462f;
        const float mem = 240f;
        
        for (var i = 0; i < Neurons.Count; i++)
        {
            Neurons[i].transform.localPosition =
                new Vector3(memX, memY, 0);
            memY -= mem;
            if ((i % 3) != 2) continue;
            memX += mem;
            memY = startY;
        }
    }

    private IEnumerator ApplyNodeElements(IReadOnlyList<string> lines)
    {
        yield return new WaitForSeconds(1f);
        for (var i = 0; i < lines.Count; i++)
        {
            var newNode = JsonUtility.FromJson<NeuronConfig>(lines[i]);
            Neurons[i].SetNewEnvoysPos(newNode.SynapsesPositions.ToArray());
            Neurons[i].SetNewValues(newNode.Values.ToArray());
        }
    }
}