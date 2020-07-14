using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class NodeElement : MonoBehaviour, IDropHandler
{
    public string Name;
    public string Value;
    public int Index;
    [SerializeField] private Text DisplayText;
    [SerializeField] private InputField Input;

    private Node myNode;

    public Node MyNode
    {
        get => myNode;
        set => myNode = value;
    }
    
    public NodeElement(int index, string name, string value)
    {
        Index = index;
        Name = name;
        Value = value;
    }

    public void SetValue(string value)
    {
        Input.text = value;
        Value = value;
    }
    
    public void SetNodeSlot(NodeElement ns)
    {
        Name = ns.Name;
        Value = ns.Value;
    }

    public void InitVal(string n, Node node)
    {
        DisplayText.text = Name = n;
        myNode = node;
        node.AddValueNodeElement(this);
    }

    public void InitOut(int index, string n, Node node)
    {
        DisplayText.text = Name = n;
        Index = index;
        myNode = node;
        node.AddOutNodeElement(this);
    }

    public void InitIn(string n, Node node)
    {
        DisplayText.text = Name = n;
        myNode = node;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NodeEnvoy>())
        {
            ForceDrop(other.GetComponent<NodeEnvoy>());
        }
    }

    public void ForceDrop(NodeEnvoy nodeEnvoy)
    {
        nodeEnvoy.SetDest(gameObject);
        SetNodeSlot(nodeEnvoy.GetData());
        nodeEnvoy.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        // old ui collision system  
        // if (eventData.pointerDrag != null)
        // {
        //     var envoy = eventData.pointerDrag.GetComponent<NodeEnvoy>();
        //     envoy.SetDest(gameObject);
        //     SetNodeSlot(envoy.GetData());
        //     eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
        // }
    }
}