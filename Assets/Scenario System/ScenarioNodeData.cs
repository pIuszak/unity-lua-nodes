using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum NodeType {Start, Normal, End, Condition, TimeDelay}

[Serializable]
public class ScenarioNodeData 
{
    public string title;
    public int id;

    public float posX;
    public float posY;

    public float value1;
    public float value2;
    public float value3;

    public string valueStr1;
    public string valueStr2;
    public string valueStr3;

    public NodeType nodeType;
}
