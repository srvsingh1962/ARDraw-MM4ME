using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSVPicker;
using TMPro;

public class ColorPicking : MonoBehaviour
{
    public Material LineMaterial;
    public GameObject pickerObject;
    public TextMeshProUGUI ColorButtonText;
    public ColorPicker picker;

    public Color Color = Color.white;
    public bool SetColorOnStart = false;
    public bool isSelectingColor = false;

    void Start()
    {
        picker.onValueChanged.AddListener(color =>
        {
            LineMaterial.color = color;
            LineMaterial.SetColor("_EmissionColor", color);
            Color = color;
        });

        LineMaterial.color = picker.CurrentColor;
        if (SetColorOnStart)
        {
            picker.CurrentColor = Color;
        }
    }

    public void selectingColor()
    {
        if (!isSelectingColor)
        {
            pickerObject.SetActive(true);
            ColorButtonText.text = "Close";
            isSelectingColor = true;
        }
        else
        {
            pickerObject.SetActive(false);
            ColorButtonText.text = "Color";
            isSelectingColor = false;
        }
    }
}
