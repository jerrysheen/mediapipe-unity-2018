using Mediapipe;
using System.Collections.Generic;
using UnityEngine;

public class HandTrackingAnnotationController : AnnotationController {
    [SerializeField] GameObject handLandmarkListsPrefab = null;
    [SerializeField] GameObject palmRectsPrefab = null;
    [SerializeField] GameObject palmDetectionsPrefab = null;

    private GameObject handLandmarkListsAnnotation;
    private GameObject palmRectsAnnotation;
    private GameObject palmDetectionsAnnotation;

    private List<NormalizedLandmarkList> handLandmarkLists;
    void Awake() {
        handLandmarkListsAnnotation = Instantiate(handLandmarkListsPrefab);
        palmRectsAnnotation = Instantiate(palmRectsPrefab);
        palmDetectionsAnnotation = Instantiate(palmDetectionsPrefab);
    }

    void OnDestroy() {
        Destroy(handLandmarkListsAnnotation);
        Destroy(palmDetectionsAnnotation);
        Destroy(palmRectsAnnotation);
    }

    public override void Clear() {
        handLandmarkListsAnnotation.GetComponent<MultiHandLandmarkListAnnotationController>().Clear();
        palmDetectionsAnnotation.GetComponent<DetectionListAnnotationController>().Clear();
        palmRectsAnnotation.GetComponent<RectAnnotationController>().Clear();
    }
    // handtracking will draw hand and plam.

    //public string getLandmarks()
    //{
    //    if (handLandmarkLists == null || handLandmarkLists.Count == 0) {
    //        return null;
    //    }
    //    Debug.Log(handLandmarkLists);
    //    return handLandmarkLists[0].ToString();
    //}

  public void Draw(Transform screenTransform, List<NormalizedLandmarkList> handLandmarkLists, List<ClassificationList> handednesses,
      List<Detection> palmDetections, List<NormalizedRect> handRects, bool isFlipped = false)
  {
        
        //if (handLandmarkLists.Count >= 1)
        //{
        //    Debug.Log(handLandmarkLists[0].ToString());
        //}
    handLandmarkListsAnnotation.GetComponent<MultiHandLandmarkListAnnotationController>().Draw(screenTransform, handLandmarkLists, isFlipped);
    palmDetectionsAnnotation.GetComponent<DetectionListAnnotationController>().Draw(screenTransform, palmDetections, isFlipped);
    palmRectsAnnotation.GetComponent<RectListAnnotationController>().Draw(screenTransform, handRects, isFlipped);
  }
}
