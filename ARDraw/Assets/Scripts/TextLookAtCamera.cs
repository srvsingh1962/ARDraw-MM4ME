using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLookAtCamera : MonoBehaviour
{
    // Make Line length text to look at camera always.
    void Update()
    {
        this.gameObject.transform.LookAt(Camera.main.transform);
    }
}
