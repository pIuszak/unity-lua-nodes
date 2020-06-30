using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using NaughtyAttributes;
using UnityEngine;

public class DynValPass : MonoBehaviour
{
    public string script1Name;
    public string script2Name;
    
   // static int[] array1 = new int[] { 1, 3, 5, 7, 9 };
  //  private DynValue v1 = DynValue.NewTable(array1);
    
    [Button]
    public void Xd()
    {
       Debug.Log("XXXXXXXXXXXXXX  "+Execute(script2Name, Execute(script1Name,  100, 300,400)));
       //Debug.Log(Execute(script1Name, 100, 200, 300));
       //Debug.Log(Execute(script2Name, 100, 200, 300));
       //(Execute(script2Name));
    }
    
    //
    // // extract value list from inNodeSlots
    // protected int[] WrapData()
    // {
    //     var length = InNodeSlots.Length;
    //     var ret = new int[length];
    //
    //     for (var i = 0; i < length; i++)
    //     {
    //         ret[i] = InNodeSlots[i].Value;
    //     }
    //     return ret;
    // }

    private static DynValue Execute(string scriptName, params object[] args)
    {
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "LUA");
        filePath = System.IO.Path.Combine(filePath, scriptName);
        var LuaCode = System.IO.File.ReadAllText(filePath);
        Script script = new Script();
        UserData.RegisterAssembly();
        script.DoString(LuaCode);
        
        DynValue res = script.Call(script.Globals["main"], args);
        return res;
    }
}
