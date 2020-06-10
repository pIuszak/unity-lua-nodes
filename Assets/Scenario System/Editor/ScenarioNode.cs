using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

// Node class representing a single node
// of the scenario system in the EDITOR window
// for the basic data type see the ScenarioNodeData.cs
public class ScenarioNode {

    public static int NodeHeight = 200;

    public Rect rect;


    //Main entry representing the node data
    public ScenarioNodeData nodeData;


    public GUIStyle style;

    public GUIStyle bgStyle;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public bool isDragged;
    public bool isSelected;

    public GUIStyle defaultNodeStyle;
    public GUIStyle conditionalNodeStyle;

    public GUIStyle timeNodeStyle;

    public GUIStyle selectedNodeStyle;

    public Action<ScenarioNode> OnRemoveNode;

    private Texture2D myTexture;

    // This returns whether the node should be
    // repainted
    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }
 
                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;
 
            case EventType.MouseUp:
                isDragged = false;
                break;
 
            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
 
        return false;
    }

    public ScenarioNode(NodeType type, string name, Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle windowStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<ScenarioNode> OnClickRemoveNode)    {
        rect = new Rect(position.x, position.y, width, height);

        nodeData = new ScenarioNodeData();
        
        style = nodeStyle;
        bgStyle = windowStyle;

        nodeData.title = name;
        nodeData.nodeType = type;
        nodeData.posX = position.x;
        nodeData.posY = position.y;

        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);

        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;

        conditionalNodeStyle = new GUIStyle(bgStyle);
        conditionalNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png") as Texture2D;

        timeNodeStyle = new GUIStyle(bgStyle);
        timeNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;

        OnRemoveNode = OnClickRemoveNode;

        if (nodeData.nodeType == NodeType.Condition) nodeData.valueStr1 = "return true";
        if (nodeData.nodeType == NodeType.Normal)
        {
            nodeData.valueStr1 = @"    
		        function onStateEnter ()

		        end";
        }
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;

        nodeData.posX = rect.position.x;
        nodeData.posY = rect.position.y;
    }

    void DrawNormalNode() {
        GUI.Box(new Rect(rect.x, rect.y, rect.width, NodeHeight), "", bgStyle);

        GUI.Box(rect, nodeData.title +" [ID: "+nodeData.id.ToString()+"]", style);

        int topMargin = 50;
        int leftMargin = 10;

        Vector2 rectStart = new Vector2(rect.x+leftMargin, rect.y + topMargin);

        GUI.Label(new Rect(rectStart.x, rectStart.y, rect.width - leftMargin, 30), "Name: ");
        nodeData.title = GUI.TextField(new Rect(rectStart.x+90, rectStart.y, rect.width-90-leftMargin*2, 30), nodeData.title);

        nodeData.valueStr1 = GUI.TextArea(new Rect(rectStart.x + 5, rectStart.y + 40, rect.width - 10 - leftMargin * 2, 90), nodeData.valueStr1);

        if (EditorApplication.isPlaying)
        {
            var nodeInPlayer = ScenarioPlayer.Instance.nodeTree.FindNodeOfID(nodeData.id);
            if (nodeInPlayer != null)
            {
                nodeInPlayer.valueStr1 = nodeData.valueStr1;
            }
        }

        inPoint.Draw();
        outPoint.Draw();
    }


    void DrawEndNode() {
        if (myTexture == null) myTexture = EditorGUIUtility.Load("end_node.png") as Texture2D;

        GUI.Box(new Rect(rect.x, rect.y, rect.width, NodeHeight), "", bgStyle);
        GUI.DrawTexture(new Rect(rect.x + 25, rect.y + 25, rect.width - 50, NodeHeight - 50), myTexture);

        inPoint.Draw();
    }

    void DrawStartNode() {
        if (myTexture == null) myTexture = EditorGUIUtility.Load("start_node.png") as Texture2D;

        GUI.Box(new Rect(rect.x, rect.y, rect.width, NodeHeight), "", bgStyle);
        GUI.DrawTexture(new Rect(rect.x+25, rect.y+25, rect.width-50, NodeHeight-50), myTexture) ;
 
        outPoint.Draw();
    }

    bool _isLengthDecimal;

    void DrawTimeDelay()
    {
        GUI.Box(new Rect(rect.x, rect.y, rect.width, NodeHeight), "", timeNodeStyle);

        GUI.Box(rect, "Time Delay", style);

        int topMargin = 50;
        int leftMargin = 10;

        Vector2 rectStart = new Vector2(rect.x + leftMargin, rect.y + topMargin);
        GUI.Label(new Rect(rectStart.x, rectStart.y, rect.width - leftMargin, 30), "Time: ");

        string lengthText = GUI.TextField(new Rect(rectStart.x + 90, rectStart.y, rect.width - 90 - leftMargin * 2, 30), nodeData.value1.ToString() + (_isLengthDecimal ? "." : ""));
        _isLengthDecimal = lengthText.EndsWith(".");

        float newLength;
        if (float.TryParse(lengthText, out newLength))
        {
            nodeData.value1 = newLength;
        }

        outPoint.Draw();
        inPoint.Draw();
    }

    void DrawConditionalNode()
    {
        GUI.Box(new Rect(rect.x, rect.y, rect.width, NodeHeight), "", conditionalNodeStyle);

        GUI.Box(rect, "Conditional", style);

        int topMargin = 50;
        int leftMargin = 10;

        Vector2 rectStart = new Vector2(rect.x + leftMargin, rect.y + topMargin);
        nodeData.valueStr1 = GUI.TextArea(new Rect(rectStart.x+5, rectStart.y, rect.width - 10 - leftMargin * 2, 120), nodeData.valueStr1);

        if (EditorApplication.isPlaying)
        {
            var nodeInPlayer = ScenarioPlayer.Instance.nodeTree.FindNodeOfID(nodeData.id);
            if (nodeInPlayer != null)
            {
                nodeInPlayer.valueStr1 = nodeData.valueStr1;
            }
        }

        inPoint.Draw();
    }


    public void Draw()
    {
        switch (nodeData.nodeType)
        {
            case NodeType.Normal:
                DrawNormalNode();
                break;
            case NodeType.Start:
                DrawStartNode();
                break;
            case NodeType.End:
                DrawEndNode();
                break;
            case NodeType.Condition:
                DrawConditionalNode();
                break;
            case NodeType.TimeDelay:
                DrawTimeDelay();
                break;
        }


    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Rename node"), false, OnRenameNode);

        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnRenameNode()
    {
        Debug.Log("Rename");
    }
 
    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }
 

 
}
