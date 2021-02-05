using Mediapipe;
using System;
using System.Collections.Generic;
using System.Drawing;
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

public class HandTrackingGraph : DemoGraph
{
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
    public HandTrackingValue handTrackingValue;
    private float originX;
    private float originY;
    private float originTime;
    private handState currState = handState.idle;
    private moveDirection currDirection = moveDirection.idle;


    public override Status StartRun()
    {
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

    public override void RenderOutput(WebCamScreenController screenController, TextureFrame textureFrame)
    {

        this.handTrackingValue = FetchNextHandTrackingValue();
        RenderAnnotation(screenController, handTrackingValue);
        screenController.DrawScreen(textureFrame);
    }

    public override string GetMark()
    {
        // if detect hands change state machine.
        //Debug.Log(currDirection);

        if (handTrackingValue != null && handTrackingValue.PalmDetections != null && handTrackingValue.PalmDetections.Count > 0)
        {
            // blue color one...

            float width = (float)handTrackingValue.PalmDetections[0].LocationData.RelativeBoundingBox.Width;
            float height = (float)handTrackingValue.PalmDetections[0].LocationData.RelativeBoundingBox.Height;
            // center of the X and Y
            float currX = width / 2 + (float)handTrackingValue.PalmDetections[0].LocationData.RelativeBoundingBox.Xmin;
            float currY = height / 2 + (float)handTrackingValue.PalmDetections[0].LocationData.RelativeBoundingBox.Ymin;

            // the filter to remove noise rect...
            if (width >= 0.1 && height >= 0.1)
            {
                if (currState == handState.move)
                {
                    /*
                     *  
                     *   move state
                     *   we set a frozon time, within it the program won't detect any move you've down.
                     *   but we will track your current position.
                     *   Time.time return current program running time.  1.0000s,  2.38491s......
                     */

                    originX = currX;
                    originY = currY;
                    if ((Time.time - originTime) > 3)
                    {
                        // Frozon time end, change state to idle, ready to detect new action, Sync the time to current time.
                        currState = handState.idle;
                        originTime = Time.time;
                        currDirection = moveDirection.idle;
                    }
                    else
                    {
                        // Frozon time, do nothing here..
                    }
                }
                else if (currState == handState.idle)
                {
                    /*
                     *  
                     *   idle state
                     *   In this state, we detect hand shape instantly, (hand spread out, fist)
                     *   And we are ready to detect hand move direction (left, right, up, down)
                     *   If we detect a long range move, we change state to idle, change hand position to move direction, and start frozon time.
                     *   
                     */


                    // detect hand shape by calculate vector.

                    NormalizedLandmarkList landmarks = handTrackingValue.HandLandmarkLists[0];
                    Vector2 w1 = new Vector2(landmarks.Landmark[8].X, landmarks.Landmark[8].Y);
                    Vector2 w2 = new Vector2(landmarks.Landmark[8].X, landmarks.Landmark[8].Y);
                    Vector2[] cordinates = new Vector2[landmarks.Landmark.Count];
                    for (int i = 0; i < cordinates.Length; i++)
                    {
                        cordinates[i] = new Vector2(landmarks.Landmark[i].X, landmarks.Landmark[i].Y);
                    }
                    Vector2 vector68 = cordinates[8] - cordinates[6];
                    Vector2 vector56 = cordinates[6] - cordinates[5];
                    Vector2 vector1012 = cordinates[12] - cordinates[10];
                    Vector2 vector910 = cordinates[10] - cordinates[9];
                    Vector2 vector1416 = cordinates[16] - cordinates[14];
                    Vector2 vector1314 = cordinates[14] - cordinates[13];
                    Vector2 vector1820 = cordinates[20] - cordinates[18];
                    Vector2 vector1718 = cordinates[18] - cordinates[17];

                    float angle_index = Vector2.Angle(vector68, vector56);
                    float angle_middle = Vector2.Angle(vector910, vector1012);
                    float angle_ring = Vector2.Angle(vector1314, vector1416);
                    float angle_pinky = Vector2.Angle(vector1718, vector1820);

                    Debug.Log(angle_index);
                    Debug.Log(angle_middle);
                    Debug.Log(angle_ring);
                    Debug.Log(angle_pinky);
                    if (angle_index <= 15 && angle_middle <= 15 && angle_ring <= 15 && angle_pinky <= 15)
                    {
                        Debug.Log("finger_spread_out");
                    }
                    else if (angle_index >= 90 && angle_middle >= 70 && angle_ring >= 70 && angle_ring >= 70 && angle_pinky >= 70)
                    {
                        Debug.Log("fist");
                    }



                    // calculate hand move direction.
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

        return null;
    }

    private HandTrackingValue FetchNextHandTrackingValue()
    {
        var isPalmDetectionsPresent = FetchNextPalmDetectionsPresence();
        var isHandLandmarksPresent = FetchNextHandLandmarksPresence();

        var handLandmarks = isHandLandmarksPresent ? FetchNextHandLandmarks() : new List<NormalizedLandmarkList>();
        var handednesses = isHandLandmarksPresent ? FetchNextHandednesses() : new List<ClassificationList>();
        var palmDetections = isPalmDetectionsPresent ? FetchNextPalmDetections() : new List<Detection>();
        var palmRects = isPalmDetectionsPresent ? FetchNextPalmRects() : new List<NormalizedRect>();

        return new HandTrackingValue(handLandmarks, handednesses, palmDetections, palmRects);
    }

    private List<ClassificationList> FetchNextHandednesses()
    {
        return FetchNext(handednessStreamPoller, handednessPacket, handednessStream);
    }

    private List<NormalizedRect> FetchNextPalmRects()
    {
        return FetchNext(palmRectsStreamPoller, palmRectsPacket, palmRectsStream);
    }

    private List<NormalizedLandmarkList> FetchNextHandLandmarks()
    {
        return FetchNext(handLandmarksStreamPoller, handLandmarksPacket, handLandmarksStream);
    }

    private bool FetchNextHandLandmarksPresence()
    {
        return FetchNext(handLandmarksPresenceStreamPoller, handLandmarksPresencePacket, handLandmarksPresenceStream);
    }

    private bool FetchNextPalmDetectionsPresence()
    {
        return FetchNext(palmDetectionsPresenceStreamPoller, palmDetectionsPresencePacket, palmDetectionsPresenceStream);
    }

    private List<Detection> FetchNextPalmDetections()
    {
        return FetchNextVector(palmDetectionsStreamPoller, palmDetectionsPacket, palmDetectionsStream);
    }

    private void RenderAnnotation(WebCamScreenController screenController, HandTrackingValue value)
    {
        // NOTE: input image is flipped
        GetComponent<HandTrackingAnnotationController>().Draw(
          screenController.transform, value.HandLandmarkLists, value.Handednesses, value.PalmDetections, value.PalmRects, true);
    }
}
