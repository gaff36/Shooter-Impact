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

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    private void Start()
    {
        backgroundImage = GetComponent<Image>();
        layoutGroup = FindObjectOfType<HorizontalLayoutGroup>();
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
        if((int) playerProperties["playerAvatar"] == 0)
        {
            playerProperties["playerAvatar"] = portraits.Length - 1;
        }
        else
        {
            playerProperties["playerAvatar"] = (int)playerProperties["playerAvatar"] - 1;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void OnClickRightArrow()
    {
        if ((int)playerProperties["playerAvatar"] == portraits.Length - 1)
        {
            playerProperties["playerAvatar"] = 0;
        }
        else
        {
            playerProperties["playerAvatar"] = (int)playerProperties["playerAvatar"] + 1;
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
        if(player.CustomProperties.ContainsKey("playerAvatar"))
        {
            playerAvatar.sprite = portraits[(int)player.CustomProperties["playerAvatar"]];
            playerProperties["playerAvatar"] = (int)player.CustomProperties["playerAvatar"];
        }
        else
        {
            playerProperties["playerAvatar"] = 0;
        }
    }
}
