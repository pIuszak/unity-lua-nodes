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
    [Range(0, 100)] [SerializeField] private float health = 100;

    public float Health
    {
        get => health;
        protected set => health = value;
    }

    [Range(0, 100)] [SerializeField] private float stamina = 90;

    public float Stamina
    {
        get => stamina;
        protected set => stamina = value;
    }

    [Range(0, 100)] [SerializeField] private float hunger = 80;

    public float Hunger
    {
        get => hunger;
        protected set => hunger = value;
    }

    [SerializeField] private NavMeshAgent Agent;
    [SerializeField] private TextMeshPro TextMesh;
    [SerializeField] private Detector Sight;
    [SerializeField] private Detector Melee;
    public GameObject Target;
    [SerializeField] private Animator animator;

    private void Debug(string var)
    {
        UnityEngine.Debug.Log(var);
        TextMesh.text = var;
    }

    public float[] Detect(string xd)
    {
        //return new float[2] {99.0f, 55.0f};
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
        return Target == null ? -1 : Vector3.Distance(Target.gameObject.transform.position, transform.position);
    }

    [UsedImplicitly]
    public void TryEat()
    {
        UnityEngine.Debug.Log("TryEat");
        Target = Melee.DetectGameObject("Egg");
        if (Target != null)
        {
            Debug("Eat " + Target.name);
            Melee.CurrentlyDetected.Remove(Target.gameObject);
            Sight.CurrentlyDetected.Remove(Target.gameObject);
            Destroy(Target.gameObject);
            Target = null;
        }
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
        animator.CrossFade("Sleep", 0.3f);
        while (seconds > 0)
        {
            Debug("zzzZZZzzz " + --seconds);
            yield return new WaitForSeconds(1);
        }

        // UnityEngine.Debug.Log("Sleep C END" + seconds);
        animator.CrossFade("Idle", 0.3f);
        Debug(" ");
    }
}