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


    [UsedImplicitly]
    public void CreateNode(string FileName)
    {
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


    public void CreateNew(string nodeName, string[] inNodeSlotsNames, string[] valNodeSlotsNames,
        string[] outNodeSlotsNames)
    {
        var node = Instantiate(NodePrefab, dragdrop);
        node.GetComponent<NodeConstructor>()
            .CreateNode(nodeName, inNodeSlotsNames, valNodeSlotsNames, outNodeSlotsNames);
    }
}