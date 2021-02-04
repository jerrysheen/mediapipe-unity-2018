using Mediapipe;
using System.Collections.Generic;
using UnityEngine;

public enum handState
{
    idle,
    move,
    fist,
    finger_spread_out,
    outScreen
}

public enum moveDirection
{
    idle,
    left,
    right,
    up,
    down
}

public class HandTrackingGraph : DemoGraph {
  private const string handLandmarksStream = "hand_landmarks";
  private OutputStreamPoller<List<NormalizedLandmarkList>> handLandmarksStreamPoller;
  private NormalizedLandmarkListVectorPacket handLandmarksPacket;

  private const string handednessStream = "handedness";
  private OutputStreamPoller<List<ClassificationList>> handednessStreamPoller;
  private ClassificationListVectorPacket handednessPacket;

  private const string palmDetectionsStream = "palm_detections";
  private OutputStreamPoller<List<Detection>> palmDetectionsStreamPoller;
  private DetectionVectorPacket palmDetectionsPacket;

  private const string palmRectsStream = "hand_rects_from_palm_detections";
  private OutputStreamPoller<List<NormalizedRect>> palmRectsStreamPoller;
  private NormalizedRectVectorPacket palmRectsPacket;

  private const string handLandmarksPresenceStream = "hand_landmarks_presence";
  private OutputStreamPoller<bool> handLandmarksPresenceStreamPoller;
  private BoolPacket handLandmarksPresencePacket;

  private const string palmDetectionsPresenceStream = "palm_detections_presence";
  private OutputStreamPoller<bool> palmDetectionsPresenceStreamPoller;
  private BoolPacket palmDetectionsPresencePacket;

  private SidePacket sidePacket;
  public  HandTrackingValue handTrackingValue;
  private float originX;
  private float originY;
    private float originTime;
    private handState currState = handState.idle;
    private moveDirection currDirection = moveDirection.idle;


    public override Status StartRun() {
    handLandmarksStreamPoller = graph.AddOutputStreamPoller<List<NormalizedLandmarkList>>(handLandmarksStream).ConsumeValueOrDie();
    handLandmarksPacket = new NormalizedLandmarkListVectorPacket();

    handednessStreamPoller = graph.AddOutputStreamPoller<List<ClassificationList>>(handednessStream).ConsumeValueOrDie();
    handednessPacket = new ClassificationListVectorPacket();

    palmDetectionsStreamPoller = graph.AddOutputStreamPoller<List<Detection>>(palmDetectionsStream).ConsumeValueOrDie();
    palmDetectionsPacket = new DetectionVectorPacket();

    palmRectsStreamPoller = graph.AddOutputStreamPoller<List<NormalizedRect>>(palmRectsStream).ConsumeValueOrDie();
    palmRectsPacket = new NormalizedRectVectorPacket();

    handLandmarksPresenceStreamPoller = graph.AddOutputStreamPoller<bool>(handLandmarksPresenceStream).ConsumeValueOrDie();
    handLandmarksPresencePacket = new BoolPacket();

    palmDetectionsPresenceStreamPoller = graph.AddOutputStreamPoller<bool>(palmDetectionsPresenceStream).ConsumeValueOrDie();
    palmDetectionsPresencePacket = new BoolPacket();

    sidePacket = new SidePacket();
    sidePacket.Emplace("num_hands", new IntPacket(2));

    return graph.StartRun(sidePacket);
  }

  public override void RenderOutput(WebCamScreenController screenController, TextureFrame textureFrame) {
    // 这里是能收到一个value值的
    this.handTrackingValue =  FetchNextHandTrackingValue();
    RenderAnnotation(screenController, handTrackingValue);

    screenController.DrawScreen(textureFrame);
  }

  public override string GetMark() {
        // if detect hands change state machine.
        Debug.Log(currDirection);
        //Debug.Log(Time.time);
        if (handTrackingValue != null && handTrackingValue.PalmDetections != null && handTrackingValue.PalmDetections.Count > 0)
        {
            // blue color one...

            Queue<HandTrackingValue> idleOrMove = new Queue<HandTrackingValue>();
            //

            float width = (float)handTrackingValue.PalmDetections[0].LocationData.RelativeBoundingBox.Width;
            float height = (float)handTrackingValue.PalmDetections[0].LocationData.RelativeBoundingBox.Height;

            float currX = width / 2 + (float)handTrackingValue.PalmDetections[0].LocationData.RelativeBoundingBox.Xmin;
            float currY = height / 2 + (float)handTrackingValue.PalmDetections[0].LocationData.RelativeBoundingBox.Ymin;

            // the filter to remove noise rect...
            if (width >= 0.15 && height >= 0.15)
            {
                if (currState == handState.move)
                {
                    if ((Time.time - originTime) > 5)
                    {
                        currState = handState.idle;
                        originTime = Time.time;
                        originX = currX;
                        originY = currY;
                        currDirection = moveDirection.idle;
                    }
                    else
                    {
                        //Debug.Log(Time.time - originTime);
                    }
                }
                else if (currState == handState.idle)
                {
                    //Debug.Log("idle");
                    float diffX = currX - originX;
                    float diffY = currY - originY;
                    if (Mathf.Abs(diffX) > 0.15 || Mathf.Abs(originY - currY) > 0.15)
                    {
                        currState = handState.move;
                        originX = currX;
                        originY = currY;
                        originTime = Time.time;

                        // define which derection it move...
                        // first we just compare diff scale regrad horizontal and vertical...
                        if (Mathf.Pow(diffX, 2) - Mathf.Pow(diffY, 2) > 0)
                        {
                            // x is larger.
                            if (diffX > 0)
                            {
                                currDirection = moveDirection.right;
                            }
                            else
                            {
                                currDirection = moveDirection.left;
                            }
                        }
                        else
                        {
                            if (diffY > 0)
                            {
                                currDirection = moveDirection.down;
                            }
                            else
                            {
                                currDirection = moveDirection.up;
                            }
                        }

                    }
                }

            }  
        }

        if (handTrackingValue != null && handTrackingValue.HandLandmarkLists != null && handTrackingValue.HandLandmarkLists.Count > 0) {
            return handTrackingValue.HandLandmarkLists[0].ToString();
        }
         
        return null;
  }

  private HandTrackingValue FetchNextHandTrackingValue() {
    var isPalmDetectionsPresent = FetchNextPalmDetectionsPresence();
    var isHandLandmarksPresent = FetchNextHandLandmarksPresence();

    var handLandmarks = isHandLandmarksPresent ? FetchNextHandLandmarks() : new List<NormalizedLandmarkList>();
    var handednesses = isHandLandmarksPresent ? FetchNextHandednesses() : new List<ClassificationList>();
    var palmDetections = isPalmDetectionsPresent ? FetchNextPalmDetections() : new List<Detection>();
    var palmRects = isPalmDetectionsPresent ? FetchNextPalmRects() : new List<NormalizedRect>();

    return new HandTrackingValue(handLandmarks, handednesses, palmDetections, palmRects);
  }

  private List<ClassificationList> FetchNextHandednesses() {
    return FetchNext(handednessStreamPoller, handednessPacket, handednessStream);
  }

  private List<NormalizedRect> FetchNextPalmRects() {
    return FetchNext(palmRectsStreamPoller, palmRectsPacket, palmRectsStream);
  }

  private List<NormalizedLandmarkList> FetchNextHandLandmarks() {
    return FetchNext(handLandmarksStreamPoller, handLandmarksPacket, handLandmarksStream);
  }

  private bool FetchNextHandLandmarksPresence() {
    return FetchNext(handLandmarksPresenceStreamPoller, handLandmarksPresencePacket, handLandmarksPresenceStream);
  }

  private bool FetchNextPalmDetectionsPresence() {
    return FetchNext(palmDetectionsPresenceStreamPoller, palmDetectionsPresencePacket, palmDetectionsPresenceStream);
  }

  private List<Detection> FetchNextPalmDetections() {
    return FetchNextVector(palmDetectionsStreamPoller, palmDetectionsPacket, palmDetectionsStream);
  }

  private void RenderAnnotation(WebCamScreenController screenController, HandTrackingValue value) {
    // NOTE: input image is flipped
    GetComponent<HandTrackingAnnotationController>().Draw(
      screenController.transform, value.HandLandmarkLists, value.Handednesses, value.PalmDetections, value.PalmRects, true);
  }
}
