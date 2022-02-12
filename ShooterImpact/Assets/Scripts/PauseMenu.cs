using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PauseMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject pauseMenu;
   
    public void QuitToMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();        
    }
}
