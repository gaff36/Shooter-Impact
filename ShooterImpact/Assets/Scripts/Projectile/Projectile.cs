using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject bulletImage;
    private Rigidbody2D rb;
    private string userID;
    public int damage;
    private Animator bulletAnimator;
    private BoxCollider2D bc;


    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        bulletAnimator = GetComponent<Animator>();
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
            bc.enabled = false;
            rb.velocity = Vector2.zero;
            bulletImage.SetActive(false);
            bulletAnimator.SetTrigger("boom");
            StartCoroutine("boom");
        }
        else if (collision.gameObject.CompareTag("Barrier"))
        {
            bc.enabled = false;
            rb.velocity = Vector2.zero;
            bulletImage.SetActive(false);
            bulletAnimator.SetTrigger("boom");
            StartCoroutine("boom");
        }
    }

    IEnumerator boom ()
    {
        yield return new WaitForSeconds(0.3f);
        GetComponent<PhotonView>().RPC("destroyProjectile", RpcTarget.AllBuffered);
    }

}
