using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeEnvoy : DragDrop, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public NodeElement MyNodeElement;
    
    public NodeElement GetData()
    {
        return MyNodeElement;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.GetComponent<NodeElement>())
        {
            MyNodeElement.MyNode.ConnectToOtherOutputNodes(other.GetComponent<NodeElement>().MyNode, this); 
            other.GetComponent<NodeElement>().MyNode.ConnectToOtherInputNodes(MyNodeElement.MyNode);
        }
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
