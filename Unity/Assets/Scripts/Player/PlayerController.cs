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
    [SerializeField] private Transform machineGunShootingPosition;
    [SerializeField] private Transform pistolShootingPosition;
    [SerializeField] private Transform knifeShootingPosition;

    private Transform activeShootingPosition;
    private Projectile bulletProjectile;

    //

    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject playerCanvas;

    //

    private int maxHealth = 100;
    public int currentHealth;
    private Animator playerAnimator;
    private float lastShotTime;


    // Weapon Variables
 
    [SerializeField] private D_MachineGun machineGunData;
    [SerializeField] private D_Pistol pistolData;
    [SerializeField] private D_Knife knifeData;

    [SerializeField] private GameObject machineGunPrefab;
    [SerializeField] private GameObject pistolPrefab;

    private Knife knife;
    private Weapon currentWeapon;
    private Weapon activeWeapon;

    //

    private void Start()
    {
        knife = new Knife(knifeData, knifeData.maxAmmo);    // Creating default knife for player
        activeShootingPosition = knifeShootingPosition;     // Setting default fire position for knife
        activeWeapon = knife;

        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        isHurt = false;
        currentHealth = maxHealth;      
    }

    private void Update()
    {
        shootingHandler();
        movementHandler();
        rotationHandler();
        GetComponent<PhotonView>().RPC("dropWeapon", RpcTarget.AllBuffered);

        if (Input.GetKeyDown(KeyCode.F)) GetComponent<PhotonView>().RPC("changeWeapon", RpcTarget.AllBuffered);

        GetComponent<PhotonView>().RPC("endHurt", RpcTarget.AllBuffered);
        if(currentHealth <= 0) GetComponent<PhotonView>().RPC("die", RpcTarget.AllBuffered);
        
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

        Vector3 targetVelocity = new Vector2(horizontalDirection * movementSpeed, verticalDirection * activeWeapon.movementSpeed);
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
        if(Input.GetKeyDown(KeyCode.Mouse1) && currentWeapon != null && activeWeapon == currentWeapon && currentWeapon.ammoLeft > 0 
           && Time.time >= lastShotTime + currentWeapon.fireRate)
        {
            Vector2 direction = playerCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            GameObject temp = PhotonNetwork.Instantiate(bulletPrefab.name, activeShootingPosition.position, rotation);
            temp.gameObject.GetComponent<PhotonView>().RPC("shoot", RpcTarget.AllBuffered, direction);
            lastShotTime = Time.time;
            currentWeapon.ammoLeft--;
        }        
    }

    [PunRPC]
    public void die()
    {
        PhotonNetwork.Destroy(gameObject.GetComponent<PhotonView>());
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
    public void changeWeapon()
    {
        if (currentWeapon != null)
        {
            if(activeWeapon.id == -1)
            {
                activeWeapon = currentWeapon;
                if(currentWeapon.id == 0)
                {
                    activeShootingPosition = machineGunShootingPosition;
                }
                else if(currentWeapon.id == 1)
                {
                    activeShootingPosition = pistolShootingPosition;
                }
            }
            else
            {
                activeWeapon = knife;
                activeShootingPosition = knifeShootingPosition;
            }
            playerAnimator.SetInteger("weaponID", activeWeapon.id);
        }         
    }

    [PunRPC]
    public void equipWeapon(int id, int ammoLeft)
    {
        if(id == 0)
        {
            currentWeapon = new MachineGun(machineGunData, ammoLeft);
            activeShootingPosition = machineGunShootingPosition;
        }
        else if (id == 1)
        {
            currentWeapon = new Pistol(pistolData, ammoLeft);
            activeShootingPosition = pistolShootingPosition;
        }

        Debug.Log("AMMO_LEFT: " + currentWeapon.ammoLeft);
        playerAnimator.SetInteger("weaponID", currentWeapon.id);
        activeWeapon = currentWeapon;
    }

    [PunRPC]
    public void dropWeapon()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {

            if (currentWeapon.id == 0)
            {
                GameObject temp = PhotonNetwork.Instantiate(machineGunPrefab.name, gameObject.transform.position, Quaternion.identity);
                temp.GetComponent<WeaponWorld>().weapon = new MachineGun(machineGunData, currentWeapon.ammoLeft);
            }
            else if (currentWeapon.id == 1)
            {
                GameObject temp = PhotonNetwork.Instantiate(pistolPrefab.name, gameObject.transform.position, Quaternion.identity);
                temp.GetComponent<WeaponWorld>().weapon = new Pistol(pistolData, currentWeapon.ammoLeft);
            }
        }

    }

}
