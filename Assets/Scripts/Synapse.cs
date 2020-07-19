using UnityEngine;
using UnityEngine.EventSystems;

public class Synapse : DragDrop, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public NeuronPart MyNeuronPart;
    
    public NeuronPart GetData()
    {
        return MyNeuronPart;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

      if (!other.GetComponent<NeuronPart>()) return;
        MyNeuronPart.MyNeuron.ConnectToOtherOutputNodes(MyNeuronPart.Index, other.GetComponent<NeuronPart>().MyNeuron); 
        other.GetComponent<NeuronPart>().MyNeuron.ConnectToOtherInputNodes(MyNeuronPart.MyNeuron);
    }
    
    public new void OnBeginDrag(PointerEventData eventData) {
        base.OnBeginDrag(eventData);
    }

    public new void OnDrag(PointerEventData eventData) {
        base.OnDrag(eventData);
    }

    public new void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);
    }

    public new void OnPointerDown(PointerEventData eventData) {
        base.OnPointerDown(eventData);
    }
}
