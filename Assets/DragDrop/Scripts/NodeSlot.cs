using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeSlot : MonoBehaviour, IDropHandler {

    public string Name;
    public int Value;
    public void OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag != null) {

            eventData.pointerDrag.GetComponent<DragDrop>().SetDest(gameObject);
            eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
        }
    }

}
