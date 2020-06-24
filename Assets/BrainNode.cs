using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainNode : Node
{
    [SerializeField] protected Node[] EndNodes;

    //todo 
    private void GetAllEndNodes()
    {
        EndNodes = FindObjectsOfType<Node>();
    }

    public void ComputeDecision()
    {
        
    }
}
    