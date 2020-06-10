using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ClickActionUnityEvent : UnityEvent<string>
{
    public string actionText;
}

public class ScenarioClickable : MonoBehaviour
{
    public static ClickActionUnityEvent OnObjectClick = new ClickActionUnityEvent();
    public static List<ScenarioClickable> clickables = new List<ScenarioClickable>();

    [Header("Settings")]
    [Space(10)]
    [SerializeField] private bool isHighlightable = true;

    private bool _isSelected;
    public bool isSelected
    {
        get { return _isSelected; }
        set
        {
            if (value != _isSelected)
            {
                _isSelected = value;
                UpdateSelection();
            }
        }
    }

    private Material originalMaterial;
    List<Renderer> renderers = new List<Renderer>();
    List<Canvas> worldCanvases = new List<Canvas>();

    void InitializeShader()
    {

        worldCanvases.AddRange(gameObject.GetComponentsInChildren<Canvas>(true));
        worldCanvases.AddRange(gameObject.GetComponents<Canvas>());

        renderers.AddRange(gameObject.GetComponentsInChildren<Renderer>(true));
        renderers.AddRange(gameObject.GetComponents<Renderer>());

        var targetShader = Shader.Find("Custom/SelectionShader");

        foreach (var renderer in renderers)
        {
      
                for (int i = 0; i < renderer.materials.Length; i++)
                {

                    renderer.materials[i] = new Material(renderer.materials[i]);
                    renderer.materials[i].shader = targetShader;
                }
            
        }
    }

    void UpdateSelection()
    {
        foreach (var renderer in renderers)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                renderer.materials[i].SetFloat("_isSelected", _isSelected ? 1f : 0f);
            }
        }

        foreach(var clickable in clickables)
        {
            if (isSelected && clickable != this) clickable.isSelected = false;
        }

        foreach (var canvas in worldCanvases)
        {
            canvas.gameObject.SetActive(_isSelected);
        }
    }

    private void Awake()
    {
        clickables.Add(this);
    }

    private void OnDisable()
    {
        isSelected = false;
    }

    private void OnDestroy()
    {
        if (clickables.Contains(this))
        {
            clickables.Remove(this);
        }
    }

    private void Start()
    {
        InitializeShader();
        UpdateSelection();
    }

    void OnMouseDown()
    {
        if (ScenarioPlayer.Instance == null) {
            Debug.LogError("No scenario player singleton");
            return;
        } 
        // RM
        // if (InteractivePresentation.Instance.networkObject.IsServer && !ScenarioPlayer.Instance.nodeTree.isScenarioStarted) {
        //     Debug.LogError("Scenario hasn't started");
        //     return;
        // }
        //
        // if (Multiplayer.Instance == null || Multiplayer.Instance.GetCurrentNetworker() == null) {
        //     Debug.LogError("No networker");
        //     return;
        // }

        if (ScenarioPlayer.Instance.nodeTree.lastGameStateNode == null)
        {
            Debug.LogError("No last game state found!");
            return;
        }

        ScenarioAction action = new ScenarioAction(SystemInfo.deviceUniqueIdentifier, 
                                                   ScenarioPlayer.Instance.nodeTree.lastGameStateNode.title,
                                                   ScenarioActionType.ClickGameObject,
                                                   gameObject.transform.name
                                                   );

        OnObjectClick.Invoke(action.ToSerialized());
        
        // RM
        // if (Multiplayer.Instance.GetCurrentNetworker().IsServer)
        // {
        // //    InteractivePresentation.Instance.SendHighlightObject(gameObject.transform.name);
        // }
        
    }
}
