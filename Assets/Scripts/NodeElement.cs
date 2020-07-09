using System;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class NodeElement : MonoBehaviour, IDropHandler
{
    public string Name = " ";
    public DynValue Value;
    [SerializeField] private Text displayText;
    [SerializeField] private InputField inputField;
    public Node MyNode;

    [Tooltip("Input Field is for Node Value's")]
    public Text InputField;

    public NodeElement(string name, DynValue value)
    {
        Name = name;
        Value = value;
    }

    public void SetValue(DynValue value)
    {
        inputField.text = value.String;
        Value = value;
    }


    public void SetNodeSlot(NodeElement ns)
    {
        Name = ns.Name;
        Value = ns.Value;
    }

    public void InitVal(string n, Node node)
    {
        displayText.text = Name = n;
        MyNode = node;
        node.AddValueNodeElement(this);
    }

    public void Init(string n, Node node)
    {
        displayText.text = Name = n;
        MyNode = node;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NodeEnvoy>())
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