using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{

    private bool isON = true;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void SFXControl()
    {
        if (isON)
        {
            StopSFX();
        }
        else
        {
            PlaySFX();
        }
    }

    public void PlaySFX()
    {
        //if (_audioSource.isPlaying) return;
        AudioListener.volume = 1;
        isON = !isON;
    }

    public void StopSFX()
    {
        AudioListener.volume = 0;
        isON = !isON;
    }
}
