using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Events;
using MoonSharp.Interpreter;


public class ScenarioNodeTree : MonoBehaviour
{
    public static UnityEvent onCurrentNodeChange = new UnityEvent();
    public static UnityEvent onScenarioFinished = new UnityEvent();

    [Header("Settings")]
    [Space(10)]
    [SerializeField] private float checkConditionsEvery = 0.1f;

    [Header("References")]
    [Space(10)]
   // [SerializeField] private InteractivePresentation presentation;

    private List<ScenarioNodeData> nodes;
    private List<ConnectionData> connections;

    private ScenarioNodeData _currentNode;
    public ScenarioNodeData currentNode
    {
        get { return _currentNode; }
        private set {
            if (value != _currentNode)
            {
                MoveToNode(value);
            }
        }
    }

    private ScenarioNodeData _lastGameStateNode;
    public ScenarioNodeData lastGameStateNode {
        get { return _lastGameStateNode; }
        private set {
            _lastGameStateNode = value;
        }
    }

    private bool _isScenarioFinished;
    public bool isScenarioFinished
    {
        get { return _isScenarioFinished; }
        private set {
            if (value != _isScenarioFinished)
            {
                _isScenarioFinished = value;
                if (_isScenarioFinished) onScenarioFinished.Invoke();
            }
        }
    }

    private bool _isScenarioStarted;
    public bool isScenarioStarted
    {
        get { return _isScenarioStarted; }
        private set
        {
            _isScenarioStarted = value;
        }
    }

    private IEnumerator waitTimeRoutine;
    private bool isWaiting;
    private bool isCheckingConditions;
    private float lastConditionCheck;
    private bool _wasNextBtnPressed;
    public bool wasNextBtnPressed
    {
        get { return _wasNextBtnPressed; }
    }


    public void NextStepBtnCallback()
    {
        _wasNextBtnPressed = true;
    }

    public ScenarioNodeData GetNodeOfName(string name)
    {
        return nodes.Find(t => t.title == name);
    }

    public List<ScenarioNodeData> GetNodesOfType(NodeType type) {
        return nodes.Where(t => t.nodeType == type).ToList();
    }

    public ScenarioNodeData GetStartingNode()
    {
        return nodes.Find(t => t.nodeType == NodeType.Start);
    }

    public ScenarioNodeData FindNodeOfID(int id)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].id == id) return nodes[i];
        }

        return null;
    }

    public List<ScenarioNodeData> GetOutcomingNodes(ScenarioNodeData node, params NodeType[] nodeTypes)
    {
        List<ScenarioNodeData> outcomingNodes = new List<ScenarioNodeData>();

        for (int i = 0; i < connections.Count; i++)
        {
            if (connections[i].outPointID == node.id)
            {
                ScenarioNodeData outcomingNode = FindNodeOfID(connections[i].inPointID);
                if (outcomingNode != null)
                {
                    bool isCorrectType = false;

                    foreach(var type in nodeTypes)
                    {
                        if (outcomingNode.nodeType == type) isCorrectType = true;
                    }

                    if (isCorrectType)
                    outcomingNodes.Add(outcomingNode);
                }
            }
        }

        return outcomingNodes;
    }

    public void SetTree(ScenarioNodeData[] nodesToLoad, ConnectionData[] connectionsToLoad)
    {
        nodes = nodesToLoad.ToList();
        connections = connectionsToLoad.ToList();
    }

    private IEnumerator WaitTimeRoutine(float time)
    {
        isWaiting = true;

        yield return new WaitForSeconds(time);

        isWaiting = false;
        GoToNextNode();
    }

    private void StartWaitTime()
    {
        if (waitTimeRoutine != null) StopCoroutine(waitTimeRoutine);
        waitTimeRoutine = WaitTimeRoutine(currentNode.value1);
        StartCoroutine(waitTimeRoutine);
    }

    private void TryToParseCurrentNode()
    {
        if (currentNode.nodeType == NodeType.Start)
        {
            GoToNextNode();
        }

        if (currentNode.nodeType == NodeType.End)
        {
            isScenarioFinished = true;
        //    isScenarioStarted = false;
        }

        if (currentNode.nodeType == NodeType.TimeDelay)
        {
            
            //UIManager.Instance.infoPanel.SetState(false);
            StartWaitTime();
        }

        if (currentNode.nodeType == NodeType.Normal)
        {
            lastGameStateNode = currentNode;
            // RM
            //InvokeGameStateScripts();
            GoToNextNode();
        }
    }

    // private void InvokeGameStateScripts()
    // {
    //     if (presentation != null)
    //     {
    //         if (presentation.networkObject.IsServer)
    //         presentation.SendNewState(currentNode.title);
    //     } else
    //     {
    //         Debug.Log("No presentation singleton...");
    //         return;
    //     }
    //
    //     string scriptText = currentNode.valueStr1;
    //     if (String.IsNullOrEmpty(scriptText)) return;
    //
    //     ScenarioLuaData.RunGameStateNodeScript(currentNode, scriptText);
    // }

    private bool IsConditionNodeMet(ScenarioNodeData conditionNode)
    {
        string scriptText = conditionNode.valueStr1;
        if (String.IsNullOrEmpty(scriptText)) return true;
        // RM
        // return ScenarioLuaData.RunConditionalScript(scriptText);
        return false;
    }

    public void ClientForceGoToNodeOfName(string name)
    {
        _currentNode = GetNodeOfName(name);
        if (_currentNode == null)
        {
            Debug.LogError("No such node: " + name);
            return;
        }

        if (_currentNode.nodeType == NodeType.Normal)
        {
            lastGameStateNode = _currentNode;
            // RM
            // InvokeGameStateScripts();
        } else
        {
            Debug.LogError("This is not a game state node: " + name);
            return;
        }

        onCurrentNodeChange.Invoke();
    }

    private void GoToNextNode()
    {
        if (isWaiting) return;
        if (isScenarioFinished) return;

        var possibleStateOutcomeNodes = GetOutcomingNodes(currentNode, NodeType.Normal, NodeType.TimeDelay, NodeType.End);

        if (possibleStateOutcomeNodes.Count > 1)
        {
            Debug.LogError("[ Scenarios ] This node contains multiple exit points.");
            _wasNextBtnPressed = false;
            return;
        }

        isCheckingConditions = false;

        var conditionNodes = GetOutcomingNodes(currentNode, NodeType.Condition);
        bool conditionsMet = true;

        foreach(var condition in conditionNodes)
        {
            if (!IsConditionNodeMet(condition)) conditionsMet = false;
        }

        if (!conditionsMet)
        {
            isCheckingConditions = true;
            _wasNextBtnPressed = false;
            return;
        }
        
        currentNode = possibleStateOutcomeNodes[0];
        TryToParseCurrentNode();
    }

    /// <summary>
    /// Moves to the given node, regardless of conditions,
    /// so this should be used to force app to this state
    /// </summary>
    /// <param name="node"></param>
    private void MoveToNode(ScenarioNodeData node)
    {
        _currentNode = node;
        onCurrentNodeChange.Invoke();
        Debug.Log("[ Scenarios ] Current Node Id: " + node.id);
    }

    public void StartTree()
    {
        isScenarioFinished = false;
        isScenarioStarted = true;

        Debug.Log("[ Scenarios ] Start...");

        currentNode = GetStartingNode();
        if (currentNode == null)
        {
            Debug.LogError("[ Scenarios ] Missing starting node.");
            return;
        }

        TryToParseCurrentNode();
    }

    private void Update()
    {
        
        if (isCheckingConditions)
        {
            if (Time.time > lastConditionCheck + checkConditionsEvery)
            {
                GoToNextNode();
                lastConditionCheck = Time.time;
            }
        }

    }
}
