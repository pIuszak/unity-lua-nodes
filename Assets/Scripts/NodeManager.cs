using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using MoonSharp.Interpreter;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

[MoonSharpUserData]
public class NodeManager : MonoBehaviour
{
    public GameObject NodePrefab;
    public Transform DragDropRoot;
    public Transform DockRoot;
    public GameObject DockButtonPrefab;

    [NaughtyAttributes.ResizableTextArea] [SerializeField]
    protected string LuaCode;

    public List<Node> NodesMemory = new List<Node>();
    private string currentScriptName;
    private Vector3 currentScriptPosition;
    public Node StartNode;
    public static NodeManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeDockButtons();
    }
    
    // this method invokes 
    public void Play()
    {
        // run start 
        foreach (Node node in NodesMemory)
        {
            node.ClearAction();
        }

        StartNode.Execute();
        // run ne
    }

    //
    [UsedImplicitly]
    public void RestartScene()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void InitializeDockButtons()
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
        dockButton.MyButton.onClick.AddListener(delegate { CreateNode(luaScriptName); });
        var img = Resources.Load<Sprite>("Icons/" + cleanName);
        var defImg = Resources.Load<Sprite>("Icons/Default");
        dockButton.MyIcon.sprite = img != null ? img : defImg;
        dockButton.MyIcon.SetNativeSize();
    }

    public void CreateNode(string FileName)
    {
        CreateNode(FileName, new List<string>());
    }

    // todo merge with CreateNodeFromConfig !!!!
    [UsedImplicitly]
    public void CreateNode(string FileName, List<string> values)
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
        var node = Instantiate(NodePrefab, DragDropRoot);
        node.transform.localPosition = currentScriptPosition;


        // node.transform.localPosition.
        var newName = currentScriptName.Replace(".lua", "_node");

        node.name = newName;
        //todo refactor plz 
        node.GetComponent<Node>().NodeConfig.LuaScript = currentScriptName;
        //   Debug.Log(nodeName+"------------------------------------");
        if (nodeName == "Start")
        {
            StartNode = node.GetComponent<Node>();
        }

        NodesMemory.Add(node.GetComponent<Node>());
        var bgName = currentScriptName.Replace(".lua", "");

        var img = Resources.Load<Sprite>("Icons/" + bgName);
        var defImg = Resources.Load<Sprite>("Icons/Default");
        node.GetComponent<Node>().BgImage.sprite = img != null ? img : defImg;


        node.GetComponent<NodeConstructor>()
            .CreateNode(nodeName, inNodeSlotsNames, valNodeSlotsNames, outNodeSlotsNames);
    }

    public void LoadValuesFromJson()
    {
    }

    [UsedImplicitly]
    public void SaveToJson()
    {
        FileUtilities.ClearFile("save.txt");
        foreach (var node in NodesMemory)
        {
            node.SavePosition();
            node.SaveValues();
            var json = JsonUtility.ToJson(node.NodeConfig);

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
            var newNode = JsonUtility.FromJson<NodeConfig>(s);
            currentScriptPosition = newNode.Position;
            CreateNode(newNode.LuaScript, newNode.Values);
            //  newNode
        }

        StartCoroutine(Delayed(lines));
        // // create nodes 
        // foreach (var s in lines)
        // {
        //
        //     currentScriptPosition = newNode.Position;
        //     CreateNode(newNode.LuaScript);
        //     //  newNode
        // }
    }

    [Button]
    public void SmartSort()
    {
        var startX = -610.7512f;
        var startY = 284.0462f;
        var memX = -610.7512f;
        var memY = 284.0462f;
        var mem = 240f;


        for (int i = 0; i < NodesMemory.Count; i++)
        {
            NodesMemory[i].transform.localPosition =
                new Vector3(memX, memY, 0);
            memY -= mem;
            if ((i % 3) == 2)
            {
                memX += mem;
                memY = startY;
            }
        }
    }

    IEnumerator Delayed(string[] lines)
    {
        yield return new WaitForSeconds(1f);
        //  Debug.Log("Deleyed");
        for (int i = 0; i < lines.Length; i++)
        {
            var newNode = JsonUtility.FromJson<NodeConfig>(lines[i]);
            NodesMemory[i].SetNewEnvoysPos(newNode.EnvoyPositions.ToArray());
            NodesMemory[i].SetNewValues(newNode.Values.ToArray());
        }
    }
}