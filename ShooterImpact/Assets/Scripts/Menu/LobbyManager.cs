using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private TMP_InputField roomInputField;
    [SerializeField] private RoomItem roomItemPrefab;
    [SerializeField] private Transform contentObject;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private TMP_Text infoText;

    private RoomOptions roomOptions;
    private List<RoomItem> roomItemList;
    private List<PlayerItem> playerItemList;
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    //

   private float timeBetweenUpdates = 1.5f;
   private float nextUpdateTime;

    //

    public ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    private void Awake()
    {
        if (PhotonNetwork.IsConnected)
        {   
            infoText.text = "You are logged in as " + PhotonNetwork.LocalPlayer.NickName;
            lobbyPanel.SetActive(true);
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            loginPanel.SetActive(true);
        }

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("playerStats") == false)
        {
            int[] temp; temp = new int[3]; temp[0] = 0; temp[1] = 0; temp[2] = 0;
            playerProperties["playerStats"] = temp;
            PhotonNetwork.SetPlayerCustomProperties(playerProperties);
        }

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

        
    }

    public void onClickCreate()
    {
        if (roomInputField.text.Length >= 1)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("playerStats") == true)
            {
                int[] temp; temp = new int[3]; temp[0] = 0; temp[1] = 0; temp[2] = 0;
                playerProperties["playerStats"] = temp;
                PhotonNetwork.SetPlayerCustomProperties(playerProperties);
            }
            PhotonNetwork.CreateRoom(roomInputField.text, roomOptions);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        createRoomPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("playerStats") == true)
        {
            int[] temp; temp = new int[3]; temp[0] = 0; temp[1] = 0; temp[2] = 0;
            playerProperties["playerStats"] = temp;
            PhotonNetwork.SetPlayerCustomProperties(playerProperties);
        }
        UpdatePlayerList();
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
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
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

    public void OnClickStartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void onClickConnect()
    {
        if (userNameInput.text.Length >= 1)
        {
            PhotonNetwork.LocalPlayer.NickName = userNameInput.text;
            infoText.text = "You are logged in as " + PhotonNetwork.LocalPlayer.NickName;
            lobbyPanel.SetActive(true);
            loginPanel.SetActive(false);         
            PhotonNetwork.ConnectUsingSettings();

        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    public void DisconnetFromLobby()
    {
        PhotonNetwork.Disconnect();
        loginPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }

    // OVERRIDES

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
    }

}
