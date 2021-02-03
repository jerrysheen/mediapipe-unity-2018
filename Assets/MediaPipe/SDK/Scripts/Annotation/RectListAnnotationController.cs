using Mediapipe;
using System.Collections.Generic;
using UnityEngine;

namespace Mediapipe {
  public class RectListAnnotationController : ListAnnotationController<RectAnnotationController> {
    public void Draw(Transform screenTransform, List<NormalizedRect> rects, bool isFlipped = false) {
      var drawingCount = Mathf.Min(rects.Count, MaxSize);
            //if (drawingCount > 0) {
            //    Debug.Log(rects[0]);
            //}
            
      for (var i = 0; i < drawingCount; i++) {
        GetAnnotationControllerAt(i).Draw(screenTransform, rects[i], isFlipped);
      }

      ClearAll(drawingCount);
    }
  }
}
