﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class ARExperienceManager : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnInitialized = null;

    [SerializeField]
    private UnityEvent OnRestarted = null;

    private ARPlaneManager arPlaneManager = null;

    private bool Initialized { get; set; }

    void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arPlaneManager.planesChanged += PlanesChanged;

#if UNITY_EDITOR
        OnInitialized?.Invoke();
        Initialized = true;
        arPlaneManager.enabled = false;
#endif
    }

    void PlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (!Initialized)
        {
            Activate();
        }
    }

    private void Activate()
    {
        OnInitialized?.Invoke();
        Initialized = true;
        arPlaneManager.enabled = true;
    }

    public void Restart()
    {
        OnRestarted?.Invoke();
        Initialized = false;
        arPlaneManager.enabled = true;
    }
}
