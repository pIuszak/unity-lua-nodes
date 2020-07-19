using System.Collections;
using JetBrains.Annotations;
using MoonSharp.Interpreter;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[MoonSharpUserData]
public class Creature : Brain
{
    [Header("Creature")]
    [ProgressBar("Health", 300, EColor.Red)]
    public int Health = 250;

    [ProgressBar("Hunger", 100, EColor.Blue)]
    public int Hunger = 25;

    [ProgressBar("Stamina", 200, EColor.Green)]
    public int Stamina = 150;
    
    [Header("Navigation")]
    [SerializeField] private NavMeshAgent Agent;
    [SerializeField] private TextMeshPro TextMesh;
    
    [Header("Detection")]
    [SerializeField] private Detector Sight;
    [SerializeField] private Detector Melee;
    
    
    private GameObject target;
    [SerializeField] private Animator Animator;

    public Select Select;
    private void Debug(string var)
    {
       // UnityEngine.Debug.Log(var);
        TextMesh.text = var;
    }

    public float[] Detect(string xd)
    {
        return Sight.Detect(xd);
    }
    
    [UsedImplicitly]
    public void Move(Vector3 destination)
    {
        Debug("Move " + destination);
        Agent.SetDestination(destination);
    }
    
    [UsedImplicitly]
    public void MoveTo(float x, float z)
    {
        var target = new Vector3(x, 1.56f, z);
        Debug("Move " + target);
        Agent.SetDestination(target);
    }

    [UsedImplicitly]
    public float GetPositionOfAxis(int axis)
    {
        switch (axis)
        {
            case 0: return transform.position.x;
            case 1: return transform.position.y;
            case 2: return transform.position.z;
        }

        UnityEngine.Debug.LogWarning("GetPositionOfAxis takes only arguments 0-2");
        return -1;
    }

    [UsedImplicitly]
    public float GetDistanceFromTarget()
    {
        return target == null ? -1 : Vector3.Distance(target.gameObject.transform.position, transform.position);
    }

    [UsedImplicitly]
    public void TryEat()
    {
        UnityEngine.Debug.Log("TryEat");
        target = Melee.DetectGameObject("Egg");
        if (target != null)
        {
            Debug("Eat " + target.name);
            Melee.CurrentlyDetected.Remove(target.gameObject);
            Sight.CurrentlyDetected.Remove(target.gameObject);
            Destroy(target.gameObject);
            target = null;
        }
    }

    [UsedImplicitly]
    public void Attack()
    {
        Debug("Attack " + target.name);
        target.gameObject.SetActive(false);
        target = null;
        //Debug.Log("Attack");
    }

    [UsedImplicitly]
    public void Die()
    {
        //  DebugLog(this.gameObject.name + " Died");
        gameObject.SetActive(false);
    }

    [UsedImplicitly]
    public void Sleep(string seconds)
    {
        StartCoroutine(SleepC(seconds));
    }

    [UsedImplicitly]
    public void Wait(float time)
    {
    }

    private IEnumerator SleepC(string val)
    {
        var seconds = float.Parse(val);
        //   UnityEngine.Debug.Log("Sleep C" + seconds);
        Animator.CrossFade("Sleep", 0.3f);
        while (seconds > 0)
        {
            Debug("zzzZZZzzz " + --seconds);
            yield return new WaitForSeconds(1);
        }

        // UnityEngine.Debug.Log("Sleep C END" + seconds);
        Animator.CrossFade("Idle", 0.3f);
        Debug(" ");
    }

    public void Repeat()
    {
        Play();
    }
}