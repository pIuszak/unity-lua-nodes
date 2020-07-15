using System.Collections.Generic;
using UnityEngine;

public class NodeOrchestration : MonoBehaviour
{
    public static NodeOrchestration Instance;
    public List<Brain> NodeContainers = new List<Brain>();
    public void Awake()
    {
        Instance = this;
    }
}
