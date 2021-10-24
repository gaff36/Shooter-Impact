using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomItem : MonoBehaviour
{
    [SerializeField] private TMP_Text roomName;

     private LobbyManager lobbyManager;

    private void Start()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
    }

    public void SetRoomName(string roomName)
    {
        this.roomName.text = roomName;
    }

    public void OnClickItem()
    {
        lobbyManager.JoinRoom(roomName.text);
    }
}
