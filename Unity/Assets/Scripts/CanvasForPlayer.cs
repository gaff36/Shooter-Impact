using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using SimpleInputNamespace;

public class CanvasForPlayer : MonoBehaviour
{
    public string photonID;
    void Start()
    {
        photonID = PhotonNetwork.LocalPlayer.UserId;
        
    }


}
