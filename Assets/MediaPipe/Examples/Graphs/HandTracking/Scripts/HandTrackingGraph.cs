using Mediapipe;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


public enum handState
{
    idle,
    move,
    outScreen
}

public enum handShape
{
    none,
    fist,
    finger_spread_out,
}
public enum moveDirection
{
    direction_idle,
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
    private List<NormalizedLandmarkList> handLandmarks;
    private const float freeze_time = 3;
    private const float noise_radio_ratio = 0.1f;
    private const float hand_spread_out_angle = 15;
    private const float fist_angle = 70;
    private float originX;
    private float originY;
    private float originTime;
    private handState currState = handState.idle;
    private handShape currShape = handShape.none;
    private moveDirection currDirection = moveDirection.direction_idle;
    private string handShapeDirectionMessage = "mttp://HandMessage/Status/";
    // decide how to send landmark
    private string handsLandmarkMessage = "mttp://HandMesaage/HandLandMarkMessage/Status/";

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
       // this.handLandmarks = handTrackingValue.HandLandmarkLists
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
            if (width >= noise_radio_ratio && height >= noise_radio_ratio)
            {
                // send landmark here:
                if (handTrackingValue.HandLandmarkLists != null && handTrackingValue.HandLandmarkLists.Count > 0)
                {
                    NormalizedLandmarkList landmarks = handTrackingValue.HandLandmarkLists[0];
                    for (int i = 0; i < landmarks.Landmark.Count; i++)
                    {
                        float tempX, tempY, tempZ;
                        tempX = landmarks.Landmark[i].X;
                        tempY = landmarks.Landmark[i].Y;
                        tempZ = landmarks.Landmark[i].Z;
                        string temp = handsLandmarkMessage + "X=&" + tempX + "Y=&" + tempY + "Z=&" + tempZ;
                        MessageMgr.GetIns().Dispatch("HandMessage", temp);
                    }
                    //Debug.Log(landmarks.Landmark[0].X + "  ,  " + landmarks.Landmark[0].Y);
                }

                if (currState == handState.outScreen)
                {
                    /*
                     *   outScreen state.
                     *   once we detect the hand show in screen, There will be two situation,
                     *   1. first time came into the screen.
                     *   2. the hand left screen because of the move command..
                     *   For the first situation, We set a 3s frozon time,( just change the handState to move. and set time);
                     *   For the second situation. We should not change the original time, 
                     *   because it's an instant state, we don't need to update location data, the location will be modified inside move state.
                     *   
                     *   by combining this two situations, we need set a relative small time, (say frozen_time/2) to fit the both cases.
                     */

                    currState = handState.move;
                    originTime = Time.time - freeze_time / 2;

                }
                else if (currState == handState.move)
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
                    if ((Time.time - originTime) > freeze_time)
                    {
                        // Frozon time end, change state to idle, ready to detect new action, Sync the time to current time.
                        currState = handState.idle;
                        originTime = Time.time;
                        currDirection = moveDirection.direction_idle;
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

                    if (handTrackingValue != null && handTrackingValue.HandLandmarkLists != null && handTrackingValue.HandLandmarkLists.Count > 0)
                    {



                        NormalizedLandmarkList landmarks = handTrackingValue.HandLandmarkLists[0];
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

                        // the angle between some knuckles.

                        float angle_index = Vector2.Angle(vector68, vector56);
                        float angle_middle = Vector2.Angle(vector910, vector1012);
                        float angle_ring = Vector2.Angle(vector1314, vector1416);
                        float angle_pinky = Vector2.Angle(vector1718, vector1820);

                        if (angle_index <= hand_spread_out_angle && angle_middle <= hand_spread_out_angle
                            && angle_ring <= hand_spread_out_angle && angle_pinky <= hand_spread_out_angle)
                        {
                            //Debug.Log("finger_spread_out");
                            currShape = handShape.finger_spread_out;

                        }
                        else if (angle_index >= fist_angle && angle_middle >= fist_angle
                            && angle_ring >= fist_angle && angle_pinky >= fist_angle)
                        {
                            //Debug.Log("fist");
                            currShape = handShape.fist;
                        }
                        else
                        {
                            // none shape 
                            currShape = handShape.none;
                        }

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
                    string temp = handShapeDirectionMessage + "Gesture=&" + currShape + "Direction=&" + currDirection;
                    MessageMgr.GetIns().Dispatch("HandMessage", temp);
                    Debug.Log(currShape + "  ,  " + currDirection);
                }



            }
        }
        else
        {
            currState = handState.outScreen;
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
