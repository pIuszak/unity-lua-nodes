using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NeuronConfig
{
    public Vector3 Position = Vector3.zero;
    public List<Vector3> SynapsesPositions;
    public List<string> Values = new List<string>();
    public string Behaviour = "Test.lua";
}