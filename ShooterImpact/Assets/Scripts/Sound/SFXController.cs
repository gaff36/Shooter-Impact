using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SFXController : MonoBehaviour
{
    Image SFXImage;
    [SerializeField] private Sprite SFXOnSprite;
    [SerializeField] private Sprite SFXOffSprite;
    public static bool SFXOn = true;

    private void Awake()
    {
        SFXImage = GetComponent<Image>();
        if (SFXOn)  UnmuteSFX();
        else MuteSFX();
    }

    private void MuteSFX()
    {
        SFXOn = false;
        SFXImage.sprite = SFXOffSprite;
        AudioListener.volume = 0f;
    }

    private void UnmuteSFX()
    {
        SFXOn = true;
        SFXImage.sprite = SFXOnSprite;
        AudioListener.volume = 1f;
    }

    public void SFXControl()
    {
        if (SFXOn) MuteSFX();
        else UnmuteSFX();
    }

    public void OnEnable()
    {
        if (SFXOn) SFXImage.sprite = SFXOnSprite;
        else SFXImage.sprite = SFXOffSprite;
    }

}
