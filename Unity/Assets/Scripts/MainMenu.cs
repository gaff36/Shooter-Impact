using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MainMenu : MonoBehaviour
{
   public void PlayGame(){
        SceneManager.LoadScene(1);
   }
    public void QuitGame()
    {
        Debug.Log("oyundan çýktý");
        Application.Quit();
    }
    public void BackMainMenu() 
    {
        SceneManager.LoadScene(0);
    }

    public void BackMainMenuWithDisconnect()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }

    public void BackNickSelection()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(1);
    }

}

