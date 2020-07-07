using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : Mother
{
    private new void OnEnable()
    {
        base.OnEnable();
        Debug.Log("B");
    }
}


public class Mother : MonoBehaviour
{
    public void OnEnable()
    {
        Debug.Log("A");
    }
}
