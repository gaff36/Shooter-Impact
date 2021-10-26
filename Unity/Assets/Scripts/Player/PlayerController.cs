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

    // Hurt Variables

    private bool isHurt;
    private float lastHurtTime;
    [SerializeField] private float hurtRecoveryCooldown;

    //

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootingPosition;
    private Projectile bulletProjectile;

    //

    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject playerCanvas;

    //

    private int maxHealth = 100;
    public int currentHealth;
    private Animator playerAnimator;


    // Weapon Variables

    private Weapon currentWeapon;
    [SerializeField] private D_MachineGun machineGunData;
    [SerializeField] private D_Pistol pistolData;
     
    //

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        isHurt = false;
        GetComponent<PhotonView>().RPC("onClickChangeWeapon", RpcTarget.AllBuffered);
        currentHealth = maxHealth;
        
    }

    private void Update()
    {
        shootingHandler();
        movementHandler();
        rotationHandler();

        if (Input.GetKeyDown(KeyCode.F)) GetComponent<PhotonView>().RPC("onClickChangeWeapon", RpcTarget.AllBuffered);

        GetComponent<PhotonView>().RPC("endHurt", RpcTarget.AllBuffered);
        
        playerCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        playerCanvas.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    /*
        Handles inputs for player movement. Gets input from keyboard right now
    */
    private void movementHandler()
    {
        float horizontalDirection = Input.GetAxis("Horizontal");
        float verticalDirection = Input.GetAxis("Vertical");

        Vector3 targetVelocity = new Vector2(horizontalDirection * movementSpeed, verticalDirection * currentWeapon.movementSpeed);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        //GetComponent<PhotonView>().RPC("movementAnimations", RpcTarget.AllBuffered, horizontalDirection, verticalDirection);
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

            GameObject temp = PhotonNetwork.Instantiate(bulletPrefab.name, shootingPosition.position, rotation);
            temp.gameObject.GetComponent<PhotonView>().RPC("shoot", RpcTarget.AllBuffered, direction);       
        }        
    }

    [PunRPC]
    public void hurt(int damage)
    {
        lastHurtTime = Time.time;
        isHurt = true;
        currentHealth -= damage;
        playerAnimator.SetBool("hurt", true);
    }

    [PunRPC]
    public void endHurt()
    {
        if (isHurt && Time.time >= lastHurtTime + hurtRecoveryCooldown)
        {
            isHurt = false;
            playerAnimator.SetBool("hurt", false);
        }
    }

    [PunRPC]
    public void movementAnimations(float horizontalDirection, float verticalDirection)
    {
        if (horizontalDirection != 0 || verticalDirection != 0)
        {
            playerAnimator.SetBool("move", true);
        }
        else
        {
            playerAnimator.SetBool("move", false);
        }
    }

    [PunRPC]
    public void changeWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;       
    }

 
    [PunRPC]
    public void onClickChangeWeapon()
    {
        if(currentWeapon == null)
        {
            changeWeapon(new Pistol(pistolData));
        }
        else if(currentWeapon.animatorID == 0)
        {
            changeWeapon(new Pistol(pistolData));
        }
        else if (currentWeapon.animatorID == 1)
        {
            changeWeapon(new MachineGun(machineGunData));
        }
        playerAnimator.SetInteger("weaponID", currentWeapon.animatorID);
    }
 
}
