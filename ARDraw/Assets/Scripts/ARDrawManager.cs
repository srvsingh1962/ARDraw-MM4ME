using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARAnchorManager))]
public class ARDrawManager : Singleton<ARDrawManager>
{
    [SerializeField]
    private LineSettings lineSettings = null;

    [SerializeField]
    private ARPlaneManager m_PlaneManager;

    [SerializeField]
    TMP_Dropdown _linewidthdropdown;

    [SerializeField]
    GameObject _canvas;

    [SerializeField]
    private UnityEvent OnDraw = null;

    [SerializeField]
    private ARAnchorManager anchorManager = null;

    [SerializeField]
    private Camera arCamera = null;

    public static event Action Ontouched;

    private ARRaycastManager _arRaycastManager;

    private List<ARAnchor> anchors = new List<ARAnchor>();

    private Dictionary<int, ARLine> Lines = new Dictionary<int, ARLine>();

    private bool CanDraw { get; set; }

    float[] linewidth = { 0.01f, 0.015f, 0.02f, 0.025f, 0.03f, 0.035f};

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    bool isplane;

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        m_PlaneManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        _canvas.SetActive(CanDraw);
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && CanDraw)
        {
            return;
        }
        DrawOnTouch();
    }

    public void AllowDraw(bool isAllow)
    {
        CanDraw = isAllow;
    }

    public void ChangeLineWidth()
    {
        lineSettings.startWidth = linewidth[_linewidthdropdown.value];
        lineSettings.endWidth = linewidth[_linewidthdropdown.value];
    }

    bool HandleRaycast(ARRaycastHit hit)
    {
        if ((hit.hitType & TrackableType.Planes) != 0)
        {
            var plane = m_PlaneManager.GetPlane(hit.trackableId);
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                return true;
            }
        }
        return false;
    }

    void DrawOnTouch()
    {
        if (!CanDraw) return;

        int tapCount = Input.touchCount > 1 && lineSettings.allowMultiTouch ? Input.touchCount : 1;

        for (int i = 0; i < tapCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            
            // Drawing in 3D plane
            //Vector3 touchPosition = arCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, lineSettings.distanceFromCamera));

            // Drawing on recognized plane (Horizontal and Vertical Plane)
            Vector3 touchPosition = Vector3.zero;
            if (_arRaycastManager.Raycast(Input.GetTouch(i).position, hits, TrackableType.PlaneWithinPolygon))
            {
                if (HandleRaycast(hits[0]))
                {

                }
                var hitPose = hits[0].pose;
                touchPosition = hitPose.position;
            }

            if (touch.phase == TouchPhase.Began)
            {
                OnDraw?.Invoke();
                Ontouched?.Invoke();

#pragma warning disable CS0618 // Type or member is obsolete
                ARAnchor anchor = anchorManager.AddAnchor(new Pose(touchPosition, Quaternion.identity));
#pragma warning restore CS0618 // Type or member is obsolete

                if (anchor == null)
                    Debug.LogError("Error creating reference point");
                else
                {
                    anchors.Add(anchor);
                }

                ARLine line = new ARLine(lineSettings);
                Lines.Add(touch.fingerId, line);
                isplane = HandleRaycast(hits[0]);
                line.AddNewLineRenderer(transform, anchor, touchPosition);
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if(isplane != HandleRaycast(hits[0]))
                {
                    Lines.Remove(touch.fingerId);
#pragma warning disable CS0618 // Type or member is obsolete
                    ARAnchor anchor = anchorManager.AddAnchor(new Pose(touchPosition, Quaternion.identity));
#pragma warning restore CS0618 // Type or member is obsolete
                    if (anchor == null)
                        Debug.LogError("Error creating reference point");
                    else
                    {
                        anchors.Add(anchor);
                    }
                    ARLine line = new ARLine(lineSettings);
                    Lines.Add(touch.fingerId, line);
                    isplane = HandleRaycast(hits[0]);
                    line.AddNewLineRenderer(transform, anchor, touchPosition);
                }
                Lines[touch.fingerId].AddPoint(touchPosition);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Lines.Remove(touch.fingerId);
            }
        }
    }

    GameObject[] GetAllLinesInScene()
    {
        return GameObject.FindGameObjectsWithTag("Line");
    }

    public void ClearLines()
    {
        GameObject[] lines = GetAllLinesInScene();
        foreach (GameObject currentLine in lines)
        {
            LineRenderer line = currentLine.GetComponent<LineRenderer>();
            Destroy(currentLine);
        }
    }
}