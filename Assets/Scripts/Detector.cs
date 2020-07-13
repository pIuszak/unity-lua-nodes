using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public List<GameObject> CurrentlyDetected = new List<GameObject>();

    public float[] Detect(string val)
    {
        
        var o =  CurrentlyDetected.FirstOrDefault(x => x.CompareTag(val));
        if (o != null)
        {
            var localPosition = o.transform.localPosition;
            Debug.Log("--- Detected "+ val + "  at " + localPosition.x +"  "+  localPosition.z);
            return new float[2] {localPosition.x, localPosition.z};
            return o != null ? new float[2] {localPosition.x, localPosition.z} : null;
        }
        else
        {
            Debug.Log("Nothing Detected");
            return null;
           //return new float[2] {49f, 51f};
        }
    }
    
    public GameObject DetectGameObject(string val)
    {
        var o =  CurrentlyDetected.FirstOrDefault(x => x.CompareTag(val));
        return o != null ? o : null;
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
