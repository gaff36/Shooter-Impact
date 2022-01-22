using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class StatManager : MonoBehaviourPunCallbacks
{
    public List<StatItem> statItemList;
    public StatItem statItemPrefab;
    public Transform statItemParent;

    

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        statItemList = new List<StatItem>();
        UpdatePlayerList();
    }

    private void Update()
    {
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        foreach (StatItem item in statItemList)
        {
            Destroy(item.gameObject);
        }
        statItemList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            StatItem newPlayerItem = Instantiate(statItemPrefab, statItemParent);

            newPlayerItem.id = player.Value.UserId;
            newPlayerItem.SetPlayerInfo(player.Value);
            statItemList.Add(newPlayerItem);
        }

    }

    public void someoneIsDead()
    {
        UpdatePlayerList();
    }

}
