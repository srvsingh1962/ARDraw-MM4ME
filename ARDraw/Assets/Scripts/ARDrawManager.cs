using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARSubsystems;
using System.Collections;

[RequireComponent(typeof(ARAnchorManager))]
public class ARDrawManager : Singleton<ARDrawManager>
{
    [SerializeField]
    GameObject lengthTextGameobject;

    [SerializeField]
    private LineSettings lineSettings = null;

    [SerializeField]
    private ARPlaneManager _planeManager;

    [SerializeField]
    TMP_Dropdown _linewidthdropdown;

    [SerializeField]
    GameObject _Canvas;

    [SerializeField]
    private UnityEvent OnDraw = null;

    [SerializeField]
    private ARAnchorManager anchorManager = null;

    [SerializeField]
    private Camera _arCamera = null;

    [SerializeField]
    GameObject removeLinesinfo;

    public static event Action Ontouched;

    private ARRaycastManager _arRaycastManager;

    private List<ARAnchor> anchors = new List<ARAnchor>();

    private Dictionary<int, ARLine> Lines = new Dictionary<int, ARLine>();

    private bool CanDraw { get; set; }

    float[] linewidth = {0.01f, 0.015f, 0.02f, 0.025f, 0.03f, 0.035f};

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    bool isplane = false;

    bool infoShowned = false;

    private float _lineLength = 0;

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        _planeManager = GetComponent<ARPlaneManager>();
    }

    private void Start()
    {
        removeLinesinfo.SetActive(false);
        infoShowned = false;
    }

    IEnumerator ShowInfo()
    {
        infoShowned = true;
        removeLinesinfo.SetActive(true);
        yield return new WaitForSeconds(5);
        removeLinesinfo.SetActive(false);
    }

    void Update()
    {
        _Canvas.SetActive(CanDraw);
        if (!infoShowned && CanDraw)
        {
            StartCoroutine(ShowInfo());
        }
        
#if !UNITY_EDITOR
        DrawOnTouch();
#else
        DrawOnMouse();
#endif
    }

    void DrawOnMouse()
    {
        if (!CanDraw) return;

        Vector3 mousePosition = _arCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, lineSettings.distanceFromCamera));

        if (Input.GetMouseButton(0))
        {
            OnDraw?.Invoke();

            if (Lines.Keys.Count == 0)
            {
                ARLine line = new ARLine(lineSettings);
                Lines.Add(0, line);
                line.AddNewLineRenderer(transform, null, mousePosition);
            }
            else
            {
                Lines[0].AddPoint(mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector3[] PointsInLine = new Vector3[Lines[0].LineRenderer.positionCount];
            Lines[0].LineRenderer.GetPositions(PointsInLine);
            float max_x = PointsInLine[0].x;
            for (int j = 0; j < PointsInLine.Length - 1; j++)
            {
                _lineLength += Vector3.Distance(PointsInLine[j], PointsInLine[j + 1]);
                max_x = Mathf.Max(max_x, PointsInLine[j].x);
            }
            Vector3 pos = (PointsInLine[0] + PointsInLine[Lines[0].LineRenderer.positionCount - 1]) / 2;
            pos.x = max_x + 0.08f;
            GameObject lineLength = Instantiate(lengthTextGameobject, pos, Quaternion.identity, Lines[0].LineRenderer.gameObject.transform);

            TextMeshPro _linelengthtext = lineLength.GetComponent<TextMeshPro>();
            Math.Round(_lineLength, 3);
            _linelengthtext.text = _lineLength.ToString() + " m";
            _lineLength = 0;
            Lines.Remove(0);
        }
    }

    void DrawOnTouch()
    {
        if (!CanDraw) return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && CanDraw)
        {
            return;
        }

        int tapCount = Input.touchCount > 1 && lineSettings.allowMultiTouch ? Input.touchCount : 1;

        for (int i = 0; i < tapCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            // Drawing in 3D plane

            // Vector3 touchPosition = arCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, lineSettings.distanceFromCamera));

            // Drawing on recognized plane (Horizontal and Vertical Plane)

            Vector3 touchPosition = Vector3.zero;
            if (_arRaycastManager.Raycast(Input.GetTouch(i).position, hits, TrackableType.PlaneWithinPolygon))
            {
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
                if (isplane != HandleRaycast(hits[0]))
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
                Vector3[] PointsInLine = new Vector3[Lines[touch.fingerId].LineRenderer.positionCount];
                Lines[touch.fingerId].LineRenderer.GetPositions(PointsInLine);
                float max_x = PointsInLine[0].x;
                for (int j = 0; j < PointsInLine.Length - 1; j++)
                {
                    _lineLength += Vector3.Distance(PointsInLine[j], PointsInLine[j + 1]);
                    max_x = Mathf.Max(max_x, PointsInLine[j].x);
                }
                Vector3 pos = (PointsInLine[0] + PointsInLine[Lines[touch.fingerId].LineRenderer.positionCount - 1]) / 2;
                pos.x = max_x + 0.03f;
                GameObject lineLength = Instantiate(lengthTextGameobject, pos, Quaternion.identity, Lines[0].LineRenderer.gameObject.transform);
                Math.Round(_lineLength, 3);
                lineLength.GetComponent<TextMeshPro>().text = _lineLength.ToString() + " m";
                _lineLength = 0;
                Lines.Remove(touch.fingerId);
            }
        }
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
            var plane = _planeManager.GetPlane(hit.trackableId);
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                return true;
            }
        }
        return false;
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