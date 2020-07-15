using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class NeuronPart : MonoBehaviour, IDropHandler
{
    public string Name;
    public string Value;
    public int Index;
    [SerializeField] private Text DisplayText;
    [SerializeField] private InputField Input;

    private Neuron myNeuron;

    public Neuron MyNeuron
    {
        get => myNeuron;
        set => myNeuron = value;
    }
    
    public NeuronPart(int index, string name, string value)
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
    
    public void SetNodeSlot(NeuronPart ns)
    {
        Name = ns.Name;
        Value = ns.Value;
    }

    public void InitVal(string n, Neuron neuron)
    {
        DisplayText.text = Name = n;
        myNeuron = neuron;
        neuron.AddNeuronValue(this);
    }

    public void InitOut(int index, string n, Neuron neuron)
    {
        DisplayText.text = Name = n;
        Index = index;
        myNeuron = neuron;
        neuron.AddNeuronPartOut(this);
    }

    public void InitIn(string n, Neuron neuron)
    {
        DisplayText.text = Name = n;
        myNeuron = neuron;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Synapse>())
        {
            ForceDrop(other.GetComponent<Synapse>());
        }
    }

    public void ForceDrop(Synapse synapse)
    {
        synapse.SetDest(gameObject);
        SetNodeSlot(synapse.GetData());
        synapse.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
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