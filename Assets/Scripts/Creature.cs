using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    [Range(0, 100)] public int Health;
    [Range(0, 100)] public int Hunger;
    [Range(0, 100)] public int Stamina;
    [SerializeField] private NavMeshAgent Agent;
    [SerializeField] private TextMeshPro TextMesh;

    private void DebugLog(string var)
    {
        TextMesh.text = var;
    }
    
    public void Move(Vector3 destination)
    {
        DebugLog("Move "+ destination);
        Agent.SetDestination(destination);
    }
    [Button]
    public void MoveToRandom()
    {
        float rndX = Random.Range(-20, 20);
        float rndZ = Random.Range(-20, 20);
        var rand = new Vector3(rndX, 1.56f, rndZ);
        DebugLog("Move Random "+ rand);
        Agent.SetDestination(rand);
    }

    public void Attack(Creature creature)
    {
        DebugLog("Attack "+ creature);
    }
    
    public void Eat(Creature creature)
    {
        DebugLog("Eat "+ creature);
    }
    [Button]
    public void SleepSomeTime()
    {
        Sleep(Random.Range(0,10));
    }
    
    public void Sleep(int seconds)
    {
        StartCoroutine(SleepC(seconds));
    }

    private IEnumerator SleepC(int seconds)
    {
        while(seconds > 0)
        {
            DebugLog("Sleep "+ --seconds);
            yield return new WaitForSeconds(1);
        }
    }
    
    public void Wait(float time)
    {
        
    }
}
