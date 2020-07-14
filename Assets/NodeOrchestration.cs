using System.Collections.Generic;
using UnityEngine;

public class NodeOrchestration : MonoBehaviour
{
    public static NodeOrchestration Instance;
    public List<NodeContainer> NodeContainers = new List<NodeContainer>();
    public void Awake()
    {
        Instance = this;
    }
}
