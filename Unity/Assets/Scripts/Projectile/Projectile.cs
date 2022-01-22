using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    private string userID;
    public int damage;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    [PunRPC]
    public void shoot(Vector2 direction, int damageAmount, string userID)
    {
        this.userID = userID;
        damage = damageAmount;
        rb.velocity = speed * direction.normalized;
    }

    [PunRPC]
    public void destroyProjectile()
    {
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        { 
            collision.gameObject.GetComponent<PhotonView>().RPC("hurt", RpcTarget.AllBuffered, damage, userID);
            GetComponent<PhotonView>().RPC("destroyProjectile", RpcTarget.AllBuffered);
        }
        else if(!collision.gameObject.CompareTag("Weapon") || !collision.gameObject.CompareTag("HealthPack"))
        {
            GetComponent<PhotonView>().RPC("destroyProjectile", RpcTarget.AllBuffered);
        }
    }

}
