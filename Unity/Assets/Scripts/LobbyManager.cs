using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private TMP_InputField roomInputField;
    [SerializeField] private RoomItem roomItemPrefab;
    [SerializeField] private Transform contentObject;
    [SerializeField] private GameObject startGameButton;

    private RoomOptions roomOptions;
    private List<RoomItem> roomItemList;
    private List<PlayerItem> playerItemList;
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    //

   private float timeBetweenUpdates = 1.5f;
   private float nextUpdateTime;

    //

    private void Awake()
    {
 
        PhotonNetwork.AutomaticallySyncScene = true;

    }

    private void Start()
    {
        roomOptions = new RoomOptions();
        roomOptions.PublishUserId = true;
        roomOptions.MaxPlayers = 5;
        roomOptions.BroadcastPropsChangeToAll = true;
        roomItemList = new List<RoomItem>();
        playerItemList = new List<PlayerItem>();
        PhotonNetwork.JoinLobby();
    }

    public void onClickCreate()
    {
        if (roomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomInputField.text, roomOptions);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);

        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List <RoomInfo> roomList)
    {
        if(Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }       
    }

    private void UpdateRoomList(List<RoomInfo> list)
    {
        foreach(RoomItem item in roomItemList)
        {
            Destroy(item.gameObject);
        }
        roomItemList.Clear();

        foreach (RoomInfo room in list)
        {
            if (room.PlayerCount < room.MaxPlayers)
            {              
                RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
                newRoom.SetRoomName(room.Name);
                newRoom.SetRoomPlayerCount(room.PlayerCount + "/5");

                roomItemList.Add(newRoom);
            }
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom(null, 5);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    private void UpdatePlayerList()
    {
        foreach(PlayerItem item in playerItemList)
        {
            Destroy(item.gameObject);
        }
        playerItemList.Clear();

        if(PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            
            if(player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyLocalChanges();
            }

            playerItemList.Add(newPlayerItem);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else startGameButton.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }
    
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public void OnClickStartGame()
    {
        SceneManager.LoadScene(3);
    }

}
