using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject loginPanel;
    
    [SerializeField] private TMP_Text infoText;

    private void Awake()
    {
        if(PhotonNetwork.IsConnected)
        {
            infoText.text = "You are logged in as " + PhotonNetwork.LocalPlayer.NickName;
            lobbyPanel.SetActive(true); 
        }
        else
        {
            loginPanel.SetActive(true);
        }
    }

    public void onClickConnect()
    {
        if(userNameInput.text.Length >= 1)
        {
            lobbyPanel.SetActive(true);
            loginPanel.SetActive(false);
            PhotonNetwork.LocalPlayer.NickName = userNameInput.text;
            infoText.text = "You are logged in as " + PhotonNetwork.LocalPlayer.NickName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster() 
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        loginPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }

    public void DisconnetFromLobby()
    {
        PhotonNetwork.Disconnect();
    }

}
