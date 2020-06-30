using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class NodeElement : MonoBehaviour, IDropHandler
{
    public string Name = " ";
    public int Value = 0;
    [SerializeField] private Text displayText;
    public Node MyNode;
    public NodeElement(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public void SetNodeSlot(NodeElement ns)
    {
        Name = ns.Name;
        Value = ns.Value;
    }

    public void Init(string n, Node node)
    {
        displayText.text = Name = n;
        MyNode = node;
    }
    
    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.GetComponent<NodeEnvoy>())
        {
          //  Debug.Log ("Triggered " + other.name);
            ForceDrop(other.GetComponent<NodeEnvoy>());
        }
    }

    public void ForceDrop(NodeEnvoy nodeEnvoy)
    {
        nodeEnvoy.SetDest(gameObject);
        SetNodeSlot(nodeEnvoy.GetData());
        nodeEnvoy.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
    }
    
    
    // old ui collision system  
    public void OnDrop(PointerEventData eventData)
    {
        // if (eventData.pointerDrag != null)
        // {
        //     var envoy = eventData.pointerDrag.GetComponent<NodeEnvoy>();
        //     envoy.SetDest(gameObject);
        //     SetNodeSlot(envoy.GetData());
        //     eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
        // }
    }
}