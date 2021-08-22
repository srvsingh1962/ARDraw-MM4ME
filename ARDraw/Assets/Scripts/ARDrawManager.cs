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

    float[] linewidth = {0.015f, 0.02f, 0.025f, 0.03f, 0.035f, 0.04f};

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    ARPlane isplane = null;

    bool infoShowned = false;

    private double _lineLength = 0.0f;

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

    // Show remove line info tag for 5 seconds.
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

        DrawOnTouch();

#if !UNITY_EDITOR
        //DrawOnTouch();
#else
        //DrawOnMouse();
#endif
    }

    // Draw with mouse for editor purpose.
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
            pos.x = max_x + 0.2f;
            GameObject lineLength = Instantiate(lengthTextGameobject, pos, Quaternion.identity, Lines[0].LineRenderer.gameObject.transform);

            TextMeshPro _linelengthtext = lineLength.GetComponent<TextMeshPro>();
            _lineLength = Math.Round(_lineLength, 3);
            Debug.Log(_lineLength);
            _linelengthtext.text = _lineLength.ToString() + " m";
            _lineLength = 0.0f;
            Lines.Remove(0);
        }
    }

    // Draw with Touch for main screen purpose.
    void DrawOnTouch()
    {
        if (!CanDraw) return;

        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            return;
        }

        int tapCount = Input.touchCount > 1 && lineSettings.allowMultiTouch ? Input.touchCount : 1;

        for (int i = 0; i < tapCount && i < Input.touchCount; i++)
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

            if (touch.phase == TouchPhase.Began && hits.Count > 0)
            {
                touchBegin(touchPosition, touch);
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if(hits.Count > 0  && HandleRaycast(hits[0]) != isplane)
                {
                    touchEnded(touch);
                    touchBegin(touchPosition, touch);
                }

                if(hits.Count > 0 && HandleRaycast(hits[0]) == isplane && Lines.ContainsKey(touch.fingerId))
                {
                    Lines[touch.fingerId].AddPoint(touchPosition);
                }
            }
            else if (touch.phase == TouchPhase.Ended && Lines.ContainsKey(touch.fingerId))
            {
                touchEnded(touch);
            }
        }
    }

    private void touchBegin(Vector3 touchPosition,Touch touch)
    {
        if (!(hits.Count > 0))
        {
            return;
        }

        isplane = HandleRaycast(hits[0]);

        if (isplane == null)
        {
            return;
        }

        OnDraw?.Invoke();
        Ontouched?.Invoke();

        ARAnchor anchor = anchorManager.AddAnchor(new Pose(touchPosition, Quaternion.identity));

        if (anchor == null)
        {
            Debug.LogError("Error creating reference point");
        }
        else
        {
            anchors.Add(anchor);
        }

        ARLine line = new ARLine(lineSettings);
        Lines.Add(touch.fingerId, line);
        line.AddNewLineRenderer(transform, anchor, touchPosition);
    }

    void touchEnded(Touch touch)
    {
        Vector3[] PointsInLine = new Vector3[Lines[touch.fingerId].LineRenderer.positionCount];
        Lines[touch.fingerId].LineRenderer.GetPositions(PointsInLine);

        // Finding the Length of the line drawn by traversing all the points in Line Renderer.
        float max_x = PointsInLine[0].x; _lineLength = 0;
        for (int j = 0; j < PointsInLine.Length - 1; j++)
        {
            _lineLength += Vector3.Distance(PointsInLine[j], PointsInLine[j + 1]);
            max_x = Mathf.Max(max_x, PointsInLine[j].x);
        }

        // Position for placing the line le
        Vector3 pos = (PointsInLine[0] + PointsInLine[Lines[touch.fingerId].LineRenderer.positionCount - 1]) / 2;
        pos.x = max_x + 0.1f;

        // Instantiating the line length text at pos.
        GameObject lineLength = Instantiate(lengthTextGameobject, pos, Quaternion.identity, Lines[0].LineRenderer.gameObject.transform);
        _lineLength = Math.Round(_lineLength, 3);
        lineLength.GetComponent<TextMeshPro>().text = _lineLength.ToString() + " m";

        Lines.Remove(touch.fingerId);
    }

    // Allow to Draw the lines.
    public void AllowDraw(bool isAllow)
    {
        CanDraw = isAllow;
    }

    // To change the line width.
    public void ChangeLineWidth()
    {
        lineSettings.startWidth = linewidth[_linewidthdropdown.value];
        lineSettings.endWidth = linewidth[_linewidthdropdown.value];
    }

    // Handle the Raycast to the Plane.
    ARPlane HandleRaycast(ARRaycastHit hit)
    {
        if ((hit.hitType & TrackableType.Planes) != 0)
        {
            var plane = _planeManager.GetPlane(hit.trackableId);
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                return plane;
            }
        }
        return null;
    }

    GameObject[] GetAllLinesInScene()
    {
        return GameObject.FindGameObjectsWithTag("Line");
    }

    // Removing the lines.
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