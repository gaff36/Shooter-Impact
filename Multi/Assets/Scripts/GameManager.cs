using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            int randomPoint = Random.Range(-5, 5);
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPoint, randomPoint, 0f), Quaternion.identity);
        }
              
    }

}
