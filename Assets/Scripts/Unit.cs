using System;
using System.Collections;
using MoonSharp.Interpreter;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[MoonSharpUserData]
public class Unit : MonoBehaviour
{
    [Range(0, 100)] public int Health;
    [Range(0, 100)] public int Hunger;
    [Range(0, 100)] public int Stamina;
    [SerializeField] private NavMeshAgent Agent;
    [SerializeField] private TextMeshPro TextMesh;
    [SerializeField] private Detector Detector;
    public Unit Target;
    private void DebugLog(string var)
    {
        TextMesh.text = var;
    }

    public int Mul(int a, int b)
    {
        return a * b;
    }

    public void Move(Vector3 destination)
    {
        DebugLog("Move " + destination);
        Agent.SetDestination(destination);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Unit>())
        {
            if(Target == null)
            {
                Target = other.GetComponent<Unit>();
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Unit>())
        {
            if(Target == other.GetComponent<Unit>())
            {
                Target = null;
            }
            
        }
    }

    [Button]
    //todo expand to dynamic "y"
    public int MoveTo(float x, float z)
    {
        var target = new Vector3(x, 1.56f, z);
        DebugLog("Move " + target);
        Agent.SetDestination(target);
        return 0;
    }



    public float GetPositionOfAxis(int axis)
    {
        switch (axis)
        {
            case 0: return transform.position.x;
            case 1: return transform.position.y;
            case 2: return transform.position.z;
        }

        Debug.LogWarning("GetPositionOfAxis takes only arguments 0-2");
        return -1;
    }

    public float GetDistanceFromTarget()
    {
        return Target == null ? -1 : Vector3.Distance(Target.gameObject.transform.position, transform.position);
    }
    
    public void Eat()
    {
        DebugLog("Eat " + Target.name);
        Target.gameObject.SetActive(false);
        Target = null;
    }

    public void Attack()
    {
        DebugLog("Attack " + Target.name);
        Target.gameObject.SetActive(false);
        Target = null;
        //Debug.Log("Attack");
    }

    public void Die()
    {
      //  DebugLog(this.gameObject.name + " Died");
        gameObject.SetActive(false);
    }

    private void Update()
    {
      //  Debug.Log("GetDistanceFromTarget() "+ GetDistanceFromTarget());
    }

    [Button]
    public void SleepSomeTime()
    {
        Sleep(Random.Range(0, 10));
    }

    public void Sleep(int seconds)
    {
        StartCoroutine(SleepC(seconds));
    }

    private IEnumerator SleepC(int seconds)
    {
        while (seconds > 0)
        {
            DebugLog("Sleep " + --seconds);
            yield return new WaitForSeconds(1);
        }
    }

    public void Wait(float time)
    {
    }
}