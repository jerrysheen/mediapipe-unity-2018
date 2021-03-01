using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class GraphSelectorController : MonoBehaviour
{
    //[SerializeField] GameObject faceDetectionGraph = null;
    [SerializeField] GameObject faceMeshGraph = null;
    //[SerializeField] GameObject irisTrackingGraph = null;
    [SerializeField] GameObject handTrackingGraph = null;
    [SerializeField] GameObject poseTrackingGraph = null;
    //[SerializeField] GameObject hairSegmentationGraph = null;
    //[SerializeField] GameObject objectDetectionGraph = null;
    //[SerializeField] GameObject officialDemoGraph = null;

    private GameObject sceneDirector;
    private Dictionary<string, GameObject> graphs;

    void Start()
    {
        sceneDirector = GameObject.Find("SceneDirector");

        var graphSelector = GetComponent<Dropdown>();
        graphSelector.onValueChanged.AddListener(delegate { OnValueChanged(graphSelector); });

        InitializeOptions();
        //InitGraph(handTrackingGraph);
    }

    //public string getLandmarks() {
    //    return handTrackingGraph.GetComponent<HandTrackingAnnotationController>().getLandmarks();
    //}
    void InitializeOptions()
    {
        graphs = new Dictionary<string, GameObject>();

        AddGraph("Hand Tracking", handTrackingGraph);
        //AddGraph("Face Detection", faceDetectionGraph);
        AddGraph("Face Mesh", faceMeshGraph);
        //AddGraph("Iris Tracking", irisTrackingGraph);
        AddGraph("Pose Tracking", poseTrackingGraph);
        //AddGraph("Hair Segmentation", hairSegmentationGraph);
        //AddGraph("Object Detection", objectDetectionGraph);
        //AddGraph("Official Demo", officialDemoGraph);

        var graphSelector = GetComponent<Dropdown>();
        graphSelector.ClearOptions();
        graphSelector.AddOptions(graphs.Select(pair => pair.Key).ToList());

        OnValueChanged(graphSelector);

    }

    private void InitGraph(GameObject graph)
    {
        sceneDirector.GetComponent<SceneDirector>().ChangeGraph(Instantiate(graph));
    }
    void AddGraph(string label, GameObject graph)
    {
        if (graph != null)
        {
            graphs.Add(label, graph);
        }
    }
    void OnValueChanged(Dropdown dropdown)
    {
        Debug.Log("something happened");
        var option = dropdown.options[dropdown.value];
        var graph = graphs[option.text];

        Debug.Log($"graph changed: {option.text}");
        sceneDirector.GetComponent<SceneDirector>().ChangeGraph(graph);
    }
}


