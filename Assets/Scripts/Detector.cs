using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public List<GameObject> CurrentlyDetected = new List<GameObject>();

    public bool Detect(string val)
    {
        return CurrentlyDetected.Select(x => x.tag = val).Any();
    }
    
    [Button]
    public void DetectTest()
    {
        Debug.Log(Detect("Egg"));
    }
    
    private void AddToList(GameObject c)
    {
        if (!CurrentlyDetected.Contains(c))
        {
            CurrentlyDetected.Add(c);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Detectable>()) return;
        AddToList(other.gameObject);
    }
    

    // checks if leaved area
    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<Detectable>()) return;
        CurrentlyDetected.Remove(other.gameObject);;
    }

}
