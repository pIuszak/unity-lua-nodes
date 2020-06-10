using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;
using Json;

[System.Serializable]
public class InteractivePresPlayer
{
    public string playerName;

    public bool isHost;

    public string uniqueDeviceId;
    public string playerNetworkId;

    public bool isActive;

    public InteractivePresPlayer(string playerName, string uniqueDeviceId, string playerNetworkId)
    {
        this.playerName = playerName;
        this.uniqueDeviceId = uniqueDeviceId;
        this.playerNetworkId = playerNetworkId;
    }
}

[System.Serializable]
public enum ScenarioActionType { ClickGameObject }

[System.Serializable]
public class ScenarioAction {
    public ScenarioActionType actionType;

    public string playerName;

    public string gameStateName;

    public string param1;

    public bool isFromHost;

    public ScenarioAction(string player, string gameStateName, ScenarioActionType actionType, string param) {
        this.playerName = player;
        this.actionType = actionType;
        this.gameStateName = gameStateName;
        this.param1 = param;

        this.isFromHost = playerName == SystemInfo.deviceUniqueIdentifier;
    }

    public string ToSerialized() {
        return JsonUtility.ToJson(this);
    }
}

public class ScenarioPlayer : MonoBehaviour
{
    public static ScenarioPlayer Instance;

    [Header("Settings")]
    [Space(10)]
    [SerializeField] private string _scenarioName;

    public int test = 0;

    public string scenarioName
    {
        get { return _scenarioName; }
    }

    private ScenarioNodeTree _nodeTree;
    public ScenarioNodeTree nodeTree
    {
        get
        {
            if (_nodeTree == null)
            {
                _nodeTree = gameObject.GetComponent<ScenarioNodeTree>();
            }
            return _nodeTree;
        }
    }

    public Dictionary<string, List<ScenarioAction>> actions = new Dictionary<string, List<ScenarioAction>>();
    public Dictionary<string, InteractivePresPlayer> players = new Dictionary<string, InteractivePresPlayer>();

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public List<ScenarioAction> GetAllActionsForGameState(string gameStateName)
    {
        List<ScenarioAction> listOfActionsForGameState;

        if (!ScenarioPlayer.Instance.actions.TryGetValue(gameStateName, out listOfActionsForGameState))
        {
            Debug.Log("No scenario action points for gameState: " + gameStateName);
            return null;
        }
        else
        {
            return listOfActionsForGameState;
        }
    }

    public void SetPlayerActivity(string networkID, bool isActive)
    {
        foreach(KeyValuePair<string, InteractivePresPlayer> player in players)
        {
            if (player.Value.playerNetworkId == networkID) player.Value.isActive = isActive;
        }
    }

    public int GetNumOfActivePlayers(bool includeHost = false)
    {
        int playerNum = 0;

        foreach(var player in players)
        {
            if (player.Value.isActive)
            {
                if (!player.Value.isHost)
                {
                    playerNum++;
                } else
                {
                    if (includeHost) playerNum++;
                }
            }
        }

        return playerNum;
    }

    public void RegisterPlayer(string playerName, string networkID, string uniqueDeviceId)
    {
        InteractivePresPlayer player;
        if (players.TryGetValue(uniqueDeviceId, out player))
        {
            player.playerNetworkId = networkID;
            player.uniqueDeviceId = uniqueDeviceId;
            player.isActive = true;
            player.isHost = playerName == SystemInfo.deviceUniqueIdentifier;

            Debug.Log("Player reconnected: "+uniqueDeviceId);
        } else
        {
            player = new InteractivePresPlayer(playerName, uniqueDeviceId, networkID);
            player.isActive = true;
            player.isHost = playerName == SystemInfo.deviceUniqueIdentifier;

            players.Add(uniqueDeviceId, player);
            Debug.Log("New player connected: " + uniqueDeviceId);
        } 
        // RM
        // InteractivePresentation.Instance.SendPlayerListToAllPlayers();
    }

    public bool ParsePlayerAction(string actionString) {
        ScenarioAction action = JsonUtility.FromJson<ScenarioAction>(actionString);

        List<ScenarioAction> gameStateListOfActions;
        if (actions.TryGetValue(action.gameStateName, out gameStateListOfActions)) {
            
            gameStateListOfActions.Add(action);

            Debug.Log("Num of actions for this game state updated to: "+gameStateListOfActions.Count);
            return true;
        }

        Debug.Log("Action not recorded: "+actionString);
        // RM
        //  InteractivePresentation.Instance.SendPlayerListToAllPlayers();
        return false;
    }

    private void LoadScenario()
    {
        actions.Clear();

        Debug.Log("Attempt load...");

        string nodesJson = "";
        string connectionsJson = "";

        #if !UNITY_EDITOR
                string path = "jar:file://" + Application.dataPath + "!/assets/[Scenarios]/Data/" + _scenarioName + ".json";
                string pathConnections = "jar:file://" + Application.dataPath + "!/assets/[Scenarios]/Data/" + _scenarioName + "-connections.json";

                WWW www = new WWW(path);
                while (!www.isDone) { }
                nodesJson = www.text;

                WWW connectionsWWW = new WWW(pathConnections);
                while (!connectionsWWW.isDone) { }
                connectionsJson = connectionsWWW.text;
        #else
                string path = Application.streamingAssetsPath+"/[Scenarios]/Data/" + _scenarioName + ".json";
                string pathConnections = Application.streamingAssetsPath + "/[Scenarios]/Data/" + _scenarioName + "-connections.json";

                if (!File.Exists(path) || !File.Exists(pathConnections))
                {
                    Debug.LogError("[ Scenarios ] Missing scenario file at: " + path);
                    return;
                }

                nodesJson = File.ReadAllText(path);
                connectionsJson = File.ReadAllText(pathConnections);
        #endif

        ScenarioNodeData[] nodesToLoad = JsonHelper.FromJson<ScenarioNodeData>(nodesJson);
        ConnectionData[] connectionsToLoad = JsonHelper.FromJson<ConnectionData>(connectionsJson);

        nodeTree.SetTree(nodesToLoad, connectionsToLoad);
        Debug.Log("[ Scenarios ] Loaded: " + nodesToLoad.Length+" nodes.");

        var gameStateNodes = nodeTree.GetNodesOfType(NodeType.Normal);
        foreach(var node in gameStateNodes) {
            List<ScenarioAction> emptyActionList = new List<ScenarioAction>();
            actions.Add(node.title, emptyActionList);
        }

        Debug.Log("[ Actions ] List of actions for: "+actions.Count+" steps.");
    }


    private void OnEnable()
    {
        
    }

    private void Start()
    {
        Application.runInBackground = true;
        LoadScenario();
    }

    public void StartScenario()
    {
        nodeTree.StartTree();
    }
}
