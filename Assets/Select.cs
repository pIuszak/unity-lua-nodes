using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select : MonoBehaviour
{
    public Outline outline;
    [SerializeField] private List<Canvas> canvases;
    public void Start()
    {
        SelectManager.Instance.Selects.Add(this);
    }

    void OnMouseDown()
    {
        // Destroy the gameObject after clicking on it
        //Destroy(gameObject);
        SelectManager.Instance.DeselectAll();
        outline.enabled = true;
        Debug.Log(this.gameObject.name);
        EnableCanvas(true);

    }

    public void EnableCanvas(bool val)
    {
        foreach (var canvas in GetComponentsInChildren<Canvas>())
        {
            canvas.enabled = val;
        }
    }

    // private void Update()
    // {
    //     check3DObjectClicked();
    // }
    //
    // void check3DObjectClicked()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         RaycastHit hitInfo = new RaycastHit();
    //         if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
    //         {
    //             Debug.Log("Object Hit is " + hitInfo.collider.gameObject.name);
    //
    //             if (hitInfo.collider.gameObject.CompareTag("Chicken"))
    //             {
    //                 
    //             }
    //         }
    //     }
    // }
}