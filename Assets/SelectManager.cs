using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    public List<Select> Selects = new List<Select>();

    public static SelectManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        DeselectAll();
    }

    public void DeselectAll()
    {
        foreach (var s in Selects)
        {
            s.outline.enabled = false;
            s.EnableCanvas(false);
        }
    }
}
