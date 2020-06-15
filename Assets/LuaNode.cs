using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

public class LuaNode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LoadLua();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void LoadLua() {
        string filePath = System.IO.Path.Combine( Application.streamingAssetsPath, "LUA" );
        filePath = System.IO.Path.Combine( filePath, "Test.lua" );
        string myLuaCode = System.IO.File.ReadAllText( filePath );

      //  Debug.Log("My LUA Code");
      //  Debug.Log(myLuaCode);
      
      DynValue res = Script.RunString(myLuaCode);
      Debug.Log(res.Number);

      // Instantiate the singleton
      //  new Actions( myLuaCode );

    }
    Script myLuaScript;

}
