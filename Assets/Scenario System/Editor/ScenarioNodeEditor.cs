using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;
using Json;

// Main editor window for the node editor of the scenario system
public class ScenarioNodeEditor : EditorWindow
{
    public static int CurrentNodeId = 10001;

    public static Texture2D logoTexture;


    // List of all nodes
    private List<ScenarioNode> nodes;

    // List of all node connections
    private List<Connection> connections;

    // Style of a single node
    private GUIStyle nodeStyle;

    private GUIStyle nodeBgStyle;
    private GUIStyle selectionStyle;

    private GUIStyle selectedNodeStyle;

    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;
 
    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    private Vector2 drag;

    private Vector2 offset;

    private bool isPlayMode;
    private ScenarioPlayer scenarioPlayer;

    private string editedText;

    [MenuItem("Window/Scenario Editor")]
    private static void OpenWindow()
    {
        ScenarioNodeEditor window = GetWindow<ScenarioNodeEditor>();
        window.titleContent = new GUIContent("Scenario Editor");

        CurrentNodeId = 10000;

     

       // logoTexture = EditorGUIUtility.Load("start_node.png") as Texture2D;
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            } 
        }
    }

    public ScenarioNode GetSelectedNode() {
        if (nodes == null) return null;

        for (int i=0; i<nodes.Count; i++) {
            if (nodes[i].isSelected) return nodes[i];
        }

        return null;
    }

    public int GetMaxID() {
        return nodes.Max(node => node.nodeData.id);
    }

    public ScenarioNode FindNodeOfID(int id) {

        if (nodes == null) return null;
        
        for (int i=0; i<nodes.Count; i++) {
            if (nodes[i].nodeData.id == id) return nodes[i];
        }

        return null;
    }

    public void LoadScenario(string name) {
        NewScenario();

        string path = Application.streamingAssetsPath + "/[Scenarios]/Data/" + name+".json";
        string pathConnections = Application.streamingAssetsPath + "/[Scenarios]/Data/" + name+"-connections.json";


        if (!File.Exists(path) || !File.Exists(pathConnections)) return;
        var nodesJson = File.ReadAllText(path);
        var connectionsJson = File.ReadAllText(pathConnections);

        ScenarioNodeData[] nodesToLoad = JsonHelper.FromJson<ScenarioNodeData>(nodesJson);
        ConnectionData[] connectionsToLoad = JsonHelper.FromJson<ConnectionData>(connectionsJson);
        if (nodes == null)
        {
            nodes = new List<ScenarioNode>();
        }

        if (connections == null)
        {
            connections = new List<Connection>();
        }

        for (int i=0; i<nodesToLoad.Length; i++) {
            var node = new ScenarioNode(nodesToLoad[i].nodeType, "Basic Node", new Vector2(nodesToLoad[i].posX, nodesToLoad[i].posY), 200, 50, nodeStyle, nodeBgStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
            node.nodeData = nodesToLoad[i];
            nodes.Add(node);            
        }

        CurrentNodeId = GetMaxID()+1;

        for (int i=0; i<connectionsToLoad.Length; i++) {
            
            ConnectionData data = connectionsToLoad[i];
            ScenarioNode inNode = FindNodeOfID(data.inPointID);
            ScenarioNode outNode = FindNodeOfID(data.outPointID);


            if (inNode != null && outNode != null) {


                ConnectionPoint inPoint = inNode.inPoint;
                ConnectionPoint outPoint = outNode.outPoint;

                connections.Add(new Connection(inPoint, outPoint, OnClickRemoveConnection));

            }
        }
    }

    public void SaveScenario(string name) {
        if (nodes.Count <= 0) return;
        
        var nodesToSave = new ScenarioNodeData[nodes.Count];
        var connectionsToSave = new ConnectionData[connections.Count];

        for (int i=0; i<nodes.Count; i++) {
            nodesToSave[i] = nodes[i].nodeData;
        } 
        for (int i=0; i<connections.Count; i++) {
            connectionsToSave[i] = connections[i].connectionData;
        } 

        string nodesJson = JsonHelper.ToJson(nodesToSave, true);
        string connectionJson = JsonHelper.ToJson(connectionsToSave, true);

        string path = Application.streamingAssetsPath + "/[Scenarios]/Data/" + name+".json";
        string pathConnections = Application.streamingAssetsPath + "/[Scenarios]/Data/" + name+"-connections.json";

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(nodesJson);
            }
        }

        using (FileStream fs = new FileStream(pathConnections, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(connectionJson);
            }
        }

        UnityEditor.AssetDatabase.Refresh();

    }

    private void OnEnable()
    {
        isPlayMode = EditorApplication.isPlaying;



        nodeBgStyle = new GUIStyle();
        nodeBgStyle.normal.background = EditorGUIUtility.Load("icons/avatarinspector/dotframe.png") as Texture2D;
        nodeBgStyle.border = new RectOffset(12, 12, 12, 12);
        nodeBgStyle.normal.textColor = Color.white;
        nodeBgStyle.padding = new RectOffset(20, 20, 15, 22);


        selectionStyle = new GUIStyle(nodeBgStyle);
        selectionStyle.normal.background = EditorGUIUtility.Load("icons/avatarinspector/dotselection.png") as Texture2D;

        nodeStyle = new GUIStyle();
    //    nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        nodeStyle.normal.textColor = Color.white;
        nodeStyle.padding = new RectOffset(20, 20, 15, 22);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("icons/avatarinspector/dotframe.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("icons/avatarinspector/dotframedotted.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);
 
        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("icons/avatarinspector/dotframe.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("icons/avatarinspector/dotframedotted.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

        selectedNodeStyle = new GUIStyle(nodeStyle);

      //  selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        if (nodes == null || nodes.Count <= 0)
        {
            if (scenarioPlayer == null) scenarioPlayer = ScenarioPlayer.Instance;
            if (scenarioPlayer == null)
            {
                Debug.LogError("[ Scenarios ] No ScenarioPlayer singleton in the scene.");
                return;
            }

            Debug.Log("[ Scenarios ] Attaching...");
            LoadScenario(scenarioPlayer.scenarioName);
        }

    }

    private void NewScenario() {
        if (nodes != null)
        nodes.Clear();
        if (connections != null)
        connections.Clear();
    }

    private void DrawSelectedNode()
    {
        if (!EditorApplication.isPlaying) return;
        if (scenarioPlayer == null) return;
        if (!scenarioPlayer.nodeTree.isScenarioStarted) return;

        var selectedNode = scenarioPlayer.nodeTree.currentNode;
        var nodeById = FindNodeOfID(selectedNode.id);

        if (nodeById == null) return;

        GUI.Box(new Rect(nodeById.rect.x-5, nodeById.rect.y-5, nodeById.rect.width+10, ScenarioNode.NodeHeight+10), "", selectionStyle);
    }

    private void DrawScriptEditor() {
        var nodeSelection = GetSelectedNode();
        if (nodeSelection == null) return;
        if (nodeSelection.nodeData.nodeType != NodeType.Normal && nodeSelection.nodeData.nodeType != NodeType.Condition) return;


        nodeSelection.nodeData.valueStr1 = GUI.TextArea(new Rect(position.width - 400, 0, 400, position.height), nodeSelection.nodeData.valueStr1);
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();
 
        DrawConnectionLine(Event.current);

        isPlayMode = EditorApplication.isPlaying;
       
        if (isPlayMode)
        {
            if (scenarioPlayer == null) scenarioPlayer = ScenarioPlayer.Instance;

            DrawSelectedNode();
            GUI.Button(new Rect(10, 10, 150, 100), "Play mode...");
            DrawScriptEditor();

            Repaint();

        } else
        {
            if (GUI.Button(new Rect(10, 10, 150, 100), "New Scenario"))
            {
                NewScenario();
            }

            if (GUI.Button(new Rect(10, 120, 150, 100), "Save Scenario"))
            {
                SaveScenario("basic");
            }

            if (GUI.Button(new Rect(10, 230, 150, 100), "Load Scenario"))
            {
                LoadScenario("basic");
            }
            DrawScriptEditor();
            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);
            if (GUI.changed) Repaint();

        }
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);
 
        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
 
        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);
 
        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }
 
        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }
 
        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );
 
            GUI.changed = true;
        }
 
        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );
 
            GUI.changed = true;
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);
 
                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void DrawNodes()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    // Displays the add node menu
    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add game state node"), false, () => OnClickAddNode(mousePosition, NodeType.Normal));
        genericMenu.AddItem(new GUIContent("Add conditional"), false, () => OnClickAddNode(mousePosition, NodeType.Condition));
        genericMenu.AddItem(new GUIContent("Add time delay"), false, () => OnClickAddNode(mousePosition, NodeType.TimeDelay));

        genericMenu.AddItem(new GUIContent("Add start node"), false, () => OnClickAddNode(mousePosition, NodeType.Start)); 
        genericMenu.AddItem(new GUIContent("Add end node"), false, () => OnClickAddNode(mousePosition, NodeType.End)); 

        genericMenu.ShowAsContext();
    }

    private void OnClickAddNode(Vector2 mousePosition, NodeType type)
    {
        if (nodes == null)
        {
            nodes = new List<ScenarioNode>();
        }

        CurrentNodeId++;
        var node = new ScenarioNode(type, "Game State", mousePosition, 200, 50, nodeStyle, nodeBgStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
        node.nodeData.id = CurrentNodeId;
        nodes.Add(node);
    }    

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;
 
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    ClearConnectionSelection();
                }
 
                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
            break;
 
            case EventType.MouseDrag:
                if (e.button == 2)
                {
                    OnDrag(e.delta);
                } 
            break;
        }
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;
 
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }
 
        GUI.changed = true;
    }

    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;
 
        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection(); 
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }
 
    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;
 
        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }
 
    private void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
    }
 
    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }
 
        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }
 
    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void OnClickRemoveNode(ScenarioNode node)
    {
        if (connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();
 
            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }
 
            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connections.Remove(connectionsToRemove[i]);
            }
 
            connectionsToRemove = null;
        }
 
        nodes.Remove(node);
    }
}
