using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class NodeSlot : MonoBehaviour, IDropHandler {

    public string Name = " ";
    public int Value = 0;


    public NodeSlot(string name, int value)
    {
        Name = name;
        Value = value;
    }
    
    public void SetNodeSlot(NodeSlot ns)
    {
        Name = ns.Name;
        Value = ns.Value;
    }

    public void OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag != null)
        { 
            var envoy = eventData.pointerDrag.GetComponent<NodeEnvoy>();
            envoy.SetDest(gameObject);
            SetNodeSlot(envoy.GetData());
            eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
        }
    }

}
