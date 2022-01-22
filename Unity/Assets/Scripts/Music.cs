using UnityEngine;
using UnityEngine.SceneManagement;

public class Music: MonoBehaviour
{
    public static AudioSource _audioSource;
    public static bool isON = true;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        _audioSource = GetComponent<AudioSource>();

        
    }

    public void musicControl()
    {
        if(isON)
        {
            StopMusic();
        }
        else
        {
            PlayMusic();
        }
    }

    public void PlayMusic()
    {
        //if (_audioSource.isPlaying) return;
        _audioSource.mute = false;
        isON = !isON;
    }

    public static void StopMusic()
    {
        _audioSource.mute = true;
        isON = !isON;
    }
}