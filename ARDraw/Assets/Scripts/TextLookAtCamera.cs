using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLookAtCamera : MonoBehaviour
{
    void Update()
    {
        this.gameObject.transform.LookAt(Camera.main.transform);
    }
}
