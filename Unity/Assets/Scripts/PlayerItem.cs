using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private Sprite [] portraits;
    
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    private Image backgroundImage;
    [SerializeField] private Image playerAvatar;

    private HorizontalLayoutGroup layoutGroup;
    private Player player;

    public ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    private void Start()
    {
        backgroundImage = GetComponent<Image>();
        layoutGroup = FindObjectOfType<HorizontalLayoutGroup>();
        if(GetComponent<PhotonView>().IsMine) UpdatePlayerItem(PhotonNetwork.LocalPlayer);
    }

    //////////////

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
        UpdatePlayerItem(player);
    }

    public void ApplyLocalChanges()
    {
        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
    }

    public void OnClickLeftArrow()
    {
        if(((int[])playerProperties["playerStats"])[2] == 0)
        {
            int[] temp; temp = new int[3]; temp[0] = 0; temp[1] = 0; temp[2] = portraits.Length - 1;
            playerProperties["playerStats"] = temp;
        }
        else
        {
            int[] temp; temp = new int[3]; temp[0] = 0; temp[1] = 0; temp[2] = ((int[])playerProperties["playerStats"])[2] - 1;
            playerProperties["playerStats"] = temp;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void OnClickRightArrow()
    {
        if (((int[])playerProperties["playerStats"])[2] == portraits.Length - 1)
        {
            int[] temp; temp = new int[3]; temp[0] = 0; temp[1] = 0; temp[2] = 0;
            playerProperties["playerStats"] = temp;
        }
        else
        {
            int[] temp; temp = new int[3]; temp[0] = 0; temp[1] = 0; temp[2] = ((int[])playerProperties["playerStats"])[2] + 1;
            playerProperties["playerStats"] = temp;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }
    }

    private void UpdatePlayerItem(Player player)
    {
        if(player.CustomProperties.ContainsKey("playerStats"))
        {
            Debug.Log("1_0");
            playerAvatar.sprite = portraits[((int[])player.CustomProperties["playerStats"])[2]];
            int[] temp; temp = new int[3]; temp[0] = 0; temp[1] = 0; temp[2] = ((int[])player.CustomProperties["playerStats"])[2];
            playerProperties["playerStats"] = temp;
        }
        else
        {
            Debug.Log("2_0");
            int[] temp; temp = new int[3]; temp[0] = 0; temp[1] = 0; temp[2] = 0;
            playerProperties["playerStats"] = temp;
            if(player == PhotonNetwork.LocalPlayer)PhotonNetwork.SetPlayerCustomProperties(playerProperties);
        }
    }
}
