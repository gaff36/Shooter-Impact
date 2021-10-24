using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    private int range;
    [SerializeField] private int damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    [PunRPC]
    public void shoot(Vector2 direction)
    {
        rb.velocity = speed * direction.normalized;
    }

    public int getRange()
    {
        return range;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PhotonView>().RPC("hurt", RpcTarget.AllBuffered, damage);
            PhotonNetwork.Destroy(gameObject.GetComponent<PhotonView>());
        }
    }

}
