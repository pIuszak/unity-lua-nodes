﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI; 


public class NodeConstructor : MonoBehaviour
{
    public GameObject InPrefab;
    public GameObject ValPrefab;
    public GameObject OutPrefab;

    public GameObject Envoy;
    public GameObject EnvoySrc;
    public GameObject Line;
    public Node MyNode;
    public Brain Brain;
    
    [SerializeField] private Transform InRoot;
    [SerializeField] private Transform ValRoot;
    [SerializeField] private Transform OutRoot;
    [SerializeField] private Text Name;

    private const int MaxNodeLength = 100;


    public void CreateNode(string nodeName, string[] inNodeSlotsNames,
        string[] valNodeSlotsNames,
        string[] outNodeSlotsNames, List<Vector3> envoyPos = null)
    {
        StartCoroutine(CreateNodeC(nodeName, inNodeSlotsNames, valNodeSlotsNames, outNodeSlotsNames,envoyPos));
    }

    private IEnumerator CreateNodeC(string nodeName, string[] inNodeSlotsNames, string[] valNodeSlotsNames,
        string[] outNodeSlotsNames, List<Vector3> envoyPos = null)
    {
        var env = new GameObject[MaxNodeLength];
        var envSrc = new GameObject[MaxNodeLength];
        
        //  ---------- In ----------
        Name.text = nodeName;
        for (var i = 0; i < inNodeSlotsNames.Length; i++)
        {
            var inElement = Instantiate(InPrefab, InRoot);
            inElement.transform.GetComponentInChildren<NodeElement>().InitIn(inNodeSlotsNames[i], MyNode);
        }

        //  ---------- Values ----------
        for (var i = 0; i < valNodeSlotsNames.Length; i++)
        {
            var valElement = Instantiate(ValPrefab, ValRoot);
            valElement.transform.GetComponentInChildren<NodeElement>().InitVal(valNodeSlotsNames[i], MyNode);
        }

        //  ---------- Out ----------
        for (var i = 0; i < outNodeSlotsNames.Length; i++)
        {
            var outElement = Instantiate(OutPrefab, OutRoot);
            outElement.transform.GetComponentInChildren<NodeElement>().InitOut(i, outNodeSlotsNames[i], MyNode);

            // ---------- Envoy & Line ----------
            env[i] = Instantiate(Envoy, transform);
            env[i].GetComponent<NodeEnvoy>().MyNodeElement = outElement.GetComponentInChildren<NodeElement>();
            GetComponent<Node>().NodeEnvoys.Add(env[i].GetComponent<NodeEnvoy>());

            var constraintSource = new ConstraintSource
            {
                sourceTransform = outElement.transform, weight = 1
            };
            envSrc[i] = Instantiate(EnvoySrc, transform);

            envSrc[i].GetComponent<PositionConstraint>().SetSource(0, constraintSource);


            var line = Instantiate(Line, transform);
            line.GetComponent<UINodeLineRenderer>().src = envSrc[i];
            line.GetComponent<UINodeLineRenderer>().dest = env[i];
        }

        // wait for unity ui to refresh
        yield return new WaitForSeconds(0.5f);
        for (var i = 0; i < outNodeSlotsNames.Length; i++)
        {
            env[i].transform.localPosition = envSrc[i].transform.localPosition;
        }
        yield return new WaitForSeconds(0.5f);
        if (envoyPos != null)
        {
            for (var i = 0; i < outNodeSlotsNames.Length; i++)
            {
                env[i].transform.localPosition = envoyPos[i];
            }
        }
       
    }
}