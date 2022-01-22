using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class EventHandler : MonoBehaviourPun, IOnEventCallback
{
    private byte EQUIP_WEAPON_EVENT = 0;

    public void OnEvent(EventData photonEvent)
    {
        //PhotonNetwork.IsMessageQueueRunning = false;
        byte eventCode = photonEvent.Code;
        object[] data = (object[])photonEvent.CustomData;

        if (eventCode == EQUIP_WEAPON_EVENT)
        {
            int targetPhotonViewID = (int)data[0];
            PhotonView targetPhotonView = PhotonNetwork.GetPhotonView(targetPhotonViewID);
            PhotonNetwork.Destroy(targetPhotonView);
        }
        //PhotonNetwork.IsMessageQueueRunning = true;

    }

    private void OnEnable()
    {
        Debug.Log("INABLE");
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        Debug.Log("DISABLE");
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
}
