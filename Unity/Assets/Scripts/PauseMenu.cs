using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PauseMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject pauseMenu;
   
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    public void Pause(){
        pauseMenu.SetActive(true);
        Time.timeScale = 1f;
    }
  
    public void QuitToMenu()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        

        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);

        base.OnLeftRoom();
    }
}
