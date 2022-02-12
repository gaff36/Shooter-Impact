using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerCamera;
    private PhotonView pv;
    [SerializeField] private TextMeshProUGUI playerName;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        if (pv.IsMine)
        {
            GetComponent<PlayerController>().enabled = true;
            playerCamera.GetComponent<Camera>().enabled = true;
        }
        else
        {
           GetComponent<PlayerController>().enabled = false;
           playerCamera.GetComponent<Camera>().enabled = false;
        }

        SetPlayerUI();
    }

    private void SetPlayerUI()
    {
        if(playerName != null)
        playerName.text = pv.Owner.NickName;
    }
}
