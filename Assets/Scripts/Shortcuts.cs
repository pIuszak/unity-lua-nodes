using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shortcuts : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


   [SerializeField] private Canvas canvas;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown("h"))
        {
            canvas.enabled = !canvas.isActiveAndEnabled;
        }
    }
}
