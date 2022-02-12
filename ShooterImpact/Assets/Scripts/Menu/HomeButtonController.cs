using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;



public class HomeButtonController : MonoBehaviourPunCallbacks
{
    public ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    public void OnClickHomeButton()
    {
        int[] temp; temp = new int[3]; temp[0] = 0; temp[1] = 0; temp[2] = 0;
        playerProperties["playerStats"] = temp;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
        base.OnLeftRoom();
    }
}
