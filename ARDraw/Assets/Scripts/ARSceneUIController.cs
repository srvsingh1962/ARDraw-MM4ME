using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ARSceneUIController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instructional test for visual UI")]
    TMP_Text m_InstructionText;

    [SerializeField]
    VideoClip m_findPlaneClip;

    public VideoClip findAPlaneClip
    {
        get => m_findPlaneClip;
        set => m_findPlaneClip = value;
    }

    [SerializeField]
    VideoPlayer m_VideoPlayer;

    public VideoPlayer videoPlayer
    {
        get => m_VideoPlayer;
        set => m_VideoPlayer = value;
    }

    [SerializeField]
    RawImage m_RawImage;

    public RawImage rawImage
    {
        get => m_RawImage;
        set => m_RawImage = value;
    }

    [SerializeField]
    Texture m_Transparent;

    public Texture transparent
    {
        get => m_Transparent;
        set => m_Transparent = value;
    }

    RenderTexture m_RenderTexture;

    [SerializeField]
    [Tooltip("time the UI takes to fade on")]
    float m_FadeOnDuration = 1.0f;
    [SerializeField]
    [Tooltip("time the UI takes to fade off")]
    float m_FadeOffDuration = 0.5f;

    Color m_AlphaWhite = new Color(1, 1, 1, 0);
    Color m_White = new Color(1, 1, 1, 1);

    public static event Action onFadeOffComplete;

    Color m_TargetColor;
    Color m_StartColor;
    Color m_LerpingColor;
    bool m_FadeOn;
    bool m_FadeOff;
    bool m_Tweening;
    float m_TweenTime;
    float m_TweenDuration;

    const string k_MoveDeviceText = "Move Device Slowly";

    void Start()
    {
        m_StartColor = m_AlphaWhite;
        m_TargetColor = m_White;
    }

    void Update()
    {
        if (!videoPlayer.isPrepared)
            return;

        if (m_FadeOff || m_FadeOn)
        {
            if (m_FadeOn)
            {
                m_StartColor = m_AlphaWhite;
                m_TargetColor = m_White;
                m_TweenDuration = m_FadeOnDuration;
                m_FadeOff = false;
            }

            if (m_FadeOff)
            {
                m_StartColor = m_White;
                m_TargetColor = m_AlphaWhite;
                m_TweenDuration = m_FadeOffDuration;

                m_FadeOn = false;
            }

            if (m_TweenTime < 1)
            {
                m_TweenTime += Time.deltaTime / m_TweenDuration;
                m_LerpingColor = Color.Lerp(m_StartColor, m_TargetColor, m_TweenTime);
                m_RawImage.color = m_LerpingColor;
                m_InstructionText.color = m_LerpingColor;

                m_Tweening = true;
            }
            else
            {
                m_TweenTime = 0;
                m_FadeOff = false;
                m_FadeOn = false;
                m_Tweening = false;

                // was it a fade off?
                if (m_TargetColor == m_AlphaWhite)
                {
                    if (onFadeOffComplete != null)
                    {
                        onFadeOffComplete();
                    }

                    // fix issue with render texture showing a single frame of the previous video
                    m_RenderTexture = m_VideoPlayer.targetTexture;
                    m_RenderTexture.DiscardContents();
                    m_RenderTexture.Release();
                    Graphics.Blit(m_Transparent, m_RenderTexture);
                }
            }
        }
    }
}
