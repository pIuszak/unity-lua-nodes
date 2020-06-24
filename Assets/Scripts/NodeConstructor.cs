using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;


public class NodeConstructor : MonoBehaviour
{
    public GameObject InPrefab;
    public GameObject OutPrefab;

    public GameObject Envoy;
    public GameObject EnvoySrc;
    public GameObject Line;


    [SerializeField] private Transform InRoot;
    [SerializeField] private Transform OutRoot;
    [SerializeField] private Text Name;

    private const int MaxNodeLength = 100;


    public void CreateNode(string nodeName, string[] inNodeSlotsNames, float[] inNodeSlotsValues,
        string[] outNodeSlotsNames, float[] outNodeSlotsValues)
    {
        StartCoroutine(CreateNodeC(nodeName, inNodeSlotsNames, inNodeSlotsValues,
            outNodeSlotsNames, outNodeSlotsValues));
    }

    public IEnumerator CreateNodeC(string nodeName, string[] inNodeSlotsNames, float[] inNodeSlotsValues,
        string[] outNodeSlotsNames, float[] outNodeSlotsValues)
    {
        var env = new GameObject[MaxNodeLength];
        var envSrc = new GameObject[MaxNodeLength];

        Name.text = nodeName;
        for (int i = 0; i < inNodeSlotsNames.Length; i++)
        {
            var go = Instantiate(InPrefab, InRoot);
            go.transform.GetComponentInChildren<Text>().text = inNodeSlotsNames[i];
        }

        for (int i = 0; i < outNodeSlotsNames.Length; i++)
        {
            var outSlot = Instantiate(OutPrefab, OutRoot);
            outSlot.transform.GetComponentInChildren<Text>().text = outNodeSlotsNames[i];

            env[i] = Instantiate(Envoy, transform);

            env[i].GetComponent<NodeEnvoy>().MyNodeSlot = outSlot.GetComponentInChildren<NodeSlot>();

            ConstraintSource constraintSource = new ConstraintSource();
            constraintSource.sourceTransform = outSlot.transform;
            constraintSource.weight = 1;
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
    }
}