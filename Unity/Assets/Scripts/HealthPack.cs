using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    private int value;
    private void Start() {
        value = 50;
    }

    [PunRPC]
    public void destroyHealthPack() 
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<PlayerController>().addHealth(value))
            GetComponent<PhotonView>().RPC("destroyHealthPack", RpcTarget.AllBuffered);
    }
}
