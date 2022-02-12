using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicController : MonoBehaviour
{
    Image SFXImage;
    [SerializeField] private Sprite MusicOnSprite;
    [SerializeField] private Sprite MusicOffSprite;
    [SerializeField] private AudioSource MenuMusic;
    public static bool MusicOn = true;

    private void Awake()
    {
        SFXImage = GetComponent<Image>();
        if (MusicOn) UnmuteMusic();
        else MuteMusic();
    }

    private void MuteMusic()
    {
        MusicOn = false;
        SFXImage.sprite = MusicOffSprite;
        MenuMusic.volume = 0f;
    }

    private void UnmuteMusic()
    {
        MusicOn = true;
        SFXImage.sprite = MusicOnSprite;
        MenuMusic.volume = 1f;
    }

    public void MusicControl()
    {
        if (MusicOn) MuteMusic();
        else UnmuteMusic();
    }
    
    public void OnEnable()
    {
        if (MusicOn) SFXImage.sprite = MusicOnSprite;
        else SFXImage.sprite = MusicOffSprite;
    }   

}
