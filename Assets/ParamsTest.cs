using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ParamsTest : MonoBehaviour
{
 
    [Button]
    void Test()
    {
        X(1,2,3,4,5);
    }

    // Update is called once per frame
    void X(params object[] args)
    {
        D(args);
    }

    void D(params object[] args)
    {
        foreach (object o in args)
        {
            Debug.Log(o.ToString());
        }
    }
    
}
