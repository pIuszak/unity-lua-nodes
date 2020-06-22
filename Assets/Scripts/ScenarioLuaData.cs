using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using System;
/// <summary>
/// Static class responsible for sending data to the LUA scripts
/// </summary>
public static class ScenarioLuaData
{
    private static void SendGlobalVariablesToLua(Script scriptToRun)
    {
        scriptToRun.Globals["Debug"] = (Action<string>)SendDebug;
        scriptToRun.Globals["GlobalSetVisibility"] = (Action<string, bool>)GlobalObjectSetVisiblity;
        scriptToRun.Globals["SetObjectHighlight"] = (Action<string, bool>)HighlightObjectGlobally;
        scriptToRun.Globals["HasEveryoneClicked"] = (Func<string, string, bool, bool>)(HaveEveryoneClickedObject);
        scriptToRun.Globals["WaitForNextButton"] = (Func<bool>)(WasNextButtonPressed);
        scriptToRun.Globals["SetNextButtonState"] = (Action<string, bool>)SetNextButtonState;
    }
    public static bool WasNextButtonPressed()
    {
        return ScenarioPlayer.Instance.nodeTree.wasNextBtnPressed;
    }
    public static void SetNextButtonState(string nextButtonText, bool isVisible)
    {
        // This is run only on server
        // if (!Multiplayer.Instance.GetCurrentNetworker().IsServer) return;
        // UIManager.Instance.SetNextBtnState(nextButtonText, isVisible);
    }
    private static void HighlightObjectGlobally(string objectName, bool isVisible)
    {
        // This is run only on server
        // if (!Multiplayer.Instance.GetCurrentNetworker().IsServer) return;
        // var clickable = ScenarioClickable.clickables.Find(t => t.name == objectName);
        // if (clickable != null)
        // {
        //     InteractivePresentation.Instance.SendHighlightObject(objectName);
        //     // clickable.isSelected = isVisible;
        // }
    }
    public static void RunGameStateNodeScript(ScenarioNodeData node, string scriptText)
    {
        Script script = new Script();
        SendGlobalVariablesToLua(script);
        script.DoString(scriptText);
        DynValue res = script.Call(script.Globals["onStateEnter"], 4);
    }
    public static bool RunConditionalScript(string scriptText)
    {
        if (String.IsNullOrEmpty(scriptText)) return true;
        Script script = new Script();
        SendGlobalVariablesToLua(script);
        DynValue res = script.DoString(scriptText);
        return res.Boolean;
    }
    public static bool HaveEveryoneClickedObject(string gameStateName, string gameObjectName, bool includeHost) {
        List<ScenarioAction> listOfActionsForGameState = ScenarioPlayer.Instance.GetAllActionsForGameState(gameStateName);
        if (listOfActionsForGameState == null)
        {
            Debug.Log("No actions");
            return false;
        }
        int howManyClicks = 0;
        int numOfOnlinePlayers = ScenarioPlayer.Instance.GetNumOfActivePlayers(includeHost);
        for (int i=0; i<listOfActionsForGameState.Count; i++) {
            if (listOfActionsForGameState[i].actionType == ScenarioActionType.ClickGameObject && listOfActionsForGameState[i].param1 == gameObjectName)
            {
                if (!listOfActionsForGameState[i].isFromHost)
                {
                    howManyClicks++;
                } else
                {
                    if (includeHost) howManyClicks++;
                }
            }
        }
        return numOfOnlinePlayers > 0 ? howManyClicks >= numOfOnlinePlayers : false;
    }
    private static void SendDebug(string text)
    {
        Debug.Log("[ Script ]: " + text);
    }
    private static void GlobalObjectSetVisiblity(string objectName, bool isVisible)
    {
        var possibleObjects = Resources.FindObjectsOfTypeAll<ScenarioSyncObject>();
        for (int i=0; i< possibleObjects.Length; i++)
        {
            if (possibleObjects[i].gameObject.transform.name == objectName)
            {
                possibleObjects[i].gameObject.SetActive(isVisible);
                return;
            }
        }
    }
}