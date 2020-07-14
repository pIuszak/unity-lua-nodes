using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public List<GameObject> CurrentlyDetected = new List<GameObject>();

    public float[] Detect(string val)
    {
        var o =  CurrentlyDetected.FirstOrDefault(x => x.CompareTag(val));
        if (o == null) return null;
        var localPosition = o.transform.localPosition;
        return new[] {localPosition.x, localPosition.z};
    }
    
    public GameObject DetectGameObject(string val)
    {
        var o =  CurrentlyDetected.FirstOrDefault(x => x.CompareTag(val));
        return o != null ? o : null;
    }
    
    private void AddToCurrentlyDetected(GameObject c)
    {
        if (!CurrentlyDetected.Contains(c))
        {
            CurrentlyDetected.Add(c);
        }
    }
    
    // checks if entered Detectable area
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Detectable>()) return;
        AddToCurrentlyDetected(other.gameObject);
    }
    
    // checks if leaved Detectable area
    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<Detectable>()) return;
        CurrentlyDetected.Remove(other.gameObject);
    }

}
