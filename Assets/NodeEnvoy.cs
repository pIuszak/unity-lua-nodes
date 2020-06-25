using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeEnvoy : DragDrop
{
    //todo remove this reference, use events instead?
    public NodeElement MyNodeElement;

    public NodeElement GetData()
    {
        return MyNodeElement;
    }
}
