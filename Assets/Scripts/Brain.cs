using System.Collections;
using JetBrains.Annotations;
using MoonSharp.Interpreter;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[MoonSharpUserData]
public class Brain : MonoBehaviour
{
    [Range(0, 100)] public int Health;
    [Range(0, 100)] public int Hunger;
    [Range(0, 100)] public int Stamina;
    
    [SerializeField] private NavMeshAgent Agent;
    [SerializeField] private TextMeshPro TextMesh;
    [SerializeField] private Detector Detector;
    public Brain Target;
    
    private void Debug(string var)
    {
        TextMesh.text = var;
    }

    [UsedImplicitly]
    public void Move(Vector3 destination)
    {
        Debug("Move " + destination);
        Agent.SetDestination(destination);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Brain>())
        {
            if(Target == null)
            {
                Target = other.GetComponent<Brain>();
            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Brain>())
        {
            if(Target == other.GetComponent<Brain>())
            {
                Target = null;
            }
            
        }
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
        return Target == null ? -1 : Vector3.Distance(Target.gameObject.transform.position, transform.position);
    }
    
    [UsedImplicitly]
    public void Eat()
    {
        Debug("Eat " + Target.name);
        Target.gameObject.SetActive(false);
        Target = null;
    }
    
    [UsedImplicitly]
    public void Attack()
    {
        Debug("Attack " + Target.name);
        Target.gameObject.SetActive(false);
        Target = null;
        //Debug.Log("Attack");
    }
    
    [UsedImplicitly]
    public void Die()
    {
      //  DebugLog(this.gameObject.name + " Died");
        gameObject.SetActive(false);
    }
    

    [Button]
    public void SleepSomeTime()
    {
        Sleep(Random.Range(0, 10));
    }
    
    [UsedImplicitly]
    public void Sleep(int seconds)
    {
        StartCoroutine(SleepC(seconds));
    }
    
    [UsedImplicitly]
    public void Wait(float time)
    {
        
    }

    private IEnumerator SleepC(int seconds)
    {
        while (seconds > 0)
        {
            Debug("zzzZZZzzz " + --seconds);
            yield return new WaitForSeconds(1);
        }
    }
    
 
}