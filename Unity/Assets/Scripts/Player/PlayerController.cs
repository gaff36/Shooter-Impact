using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    // Movement Variables

    private Rigidbody2D rb;
    private Vector3 m_Velocity = Vector3.zero;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private int movementSpeed;
    [SerializeField] private float rotationSpeed;

    //

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootingPosition;
    private Projectile bulletProjectile;

    //

    [SerializeField] private GameObject playerCamera;

    //

    private int maxHealth = 100;
    public int currentHealth;

    //
    
    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        shootingHandler();
        movementHandler();
        rotationHandler();
        playerCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    /*
        Handles inputs for player movement. Gets input from keyboard right now
    */
    private void movementHandler()
    {
        float horizontalDirection = Input.GetAxis("Horizontal");
        float verticalDirection = Input.GetAxis("Vertical");

        Vector3 targetVelocity = new Vector2(horizontalDirection * movementSpeed, verticalDirection * movementSpeed);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }

    /*
        Handles inputs for player rotation. Gets input from mouse right now.
    */
    private void rotationHandler()
    {
        Vector2 direction = playerCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    /*
        Handles inputs for shooting action input from mouse right now.
    */
    private void shootingHandler()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            Vector2 direction = playerCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            
            //GameObject temp = GameObject.Instantiate(bulletPrefab, shootingPosition.position, rotation);
            //bulletProjectile = temp.GetComponent<Projectile>();
            //bulletProjectile.shoot(direction);

            GameObject temp = PhotonNetwork.Instantiate(bulletPrefab.name, shootingPosition.position, rotation);
            temp.gameObject.GetComponent<PhotonView>().RPC("shoot", RpcTarget.AllBuffered, direction);

            /*
            bulletProjectile = temp.GetComponent<Projectile>();
            bulletProjectile.shoot(direction);
            */
            
        }        
    }

    [PunRPC]
    public void hurt(int damage)
    {
        currentHealth -= damage;
    }

   

}
