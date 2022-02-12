using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StabingController : MonoBehaviour
{
    [SerializeField] private PlayerController pc;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && pc.isStabing)
        {
            collision.gameObject.GetComponent<PhotonView>().RPC("hurt", RpcTarget.AllBuffered, 10000, PhotonNetwork.LocalPlayer.UserId);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && pc.isStabing)
        {
            collision.gameObject.GetComponent<PhotonView>().RPC("hurt", RpcTarget.AllBuffered, 10000, PhotonNetwork.LocalPlayer.UserId);
        }
    }
}
