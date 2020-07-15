using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NeuronConfig
{
    public Vector3 Position = Vector3.zero;
    public List<Vector3> EnvoyPositions;
    public List<string> Values = new List<string>();
    public string LuaScript = "Test.lua";
}