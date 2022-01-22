using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ExitGames.Client.Photon;
using Photon.Pun;

public class RoomItem : MonoBehaviour
{
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private TMP_Text roomPlayerNum;

     private LobbyManager lobbyManager;

    private void Start()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
    }

    public void SetRoomName(string roomName)
    {
        this.roomName.text = roomName;
    }

    public void SetRoomPlayerCount(string playerCount)
    {
        this.roomPlayerNum.text = playerCount;
    }

    public void OnClickItem()
    {
        lobbyManager.JoinRoom(roomName.text);
    }
}
