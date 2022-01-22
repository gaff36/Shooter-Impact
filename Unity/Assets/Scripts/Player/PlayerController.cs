using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using SimpleInputNamespace;
using UnityEngine.Networking;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // DROP WEAPON VARIABLES

    public bool droppedWeapon;
    public int ammo;

    // EQUIP WEAPON VARIABLES

    private bool equipped;
    private bool isOnWeapon;
    private int tempID;
    public int tempAmmoLeft;
    private int tempPhotonView;

    // UI Elements

    [SerializeField] private FixedJoystick joystickMovement;
    [SerializeField] private FixedJoystick joystickRotation;


    [SerializeField] private float joystickShootRange;

    // Movement Variables
    private Rigidbody2D rb;
    private Vector3 m_Velocity = Vector3.zero;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private int movementSpeed;
    [SerializeField] private float rotationSpeed;

    private float horizontalDirection;
    private float verticalDirection;

    // Hurt Variables

    public bool isDead = false;
    private bool isHurt;
    private float lastHurtTime;
    private string shooterUserID;

    [SerializeField] private float hurtRecoveryCooldown;

    //

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject HealthPack;
    [SerializeField] private Transform machineGunShootingPosition;
    [SerializeField] private Transform pistolShootingPosition;
    [SerializeField] private Transform knifeShootingPosition;

    private Transform activeShootingPosition;
    private Projectile bulletProjectile;

    private Vector2 shootingDirection = Vector2.right;
    [SerializeField] private float stabRadius;
    [SerializeField] private LayerMask playerLayer;

    //

    private SpawnPoints sp;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private Canvas playerCanvas;

    //

    public string photonID;
    private int maxHealth = 100;
    public int currentHealth;
    private Animator playerAnimator;
    [SerializeField] private Animator muzzleFlashAnimator;
    [SerializeField] private Transform muzzleFlashPosition;

    private PhotonView photonView;
    private byte EQUIP_WEAPON_EVENT = 0;
    private byte DROP_WEAPON_EVENT = 1;
    public bool equipButtonPressed;

    private float lastShotTime;

    // Weapon Variables

    [SerializeField] private D_MachineGun machineGunData;
    [SerializeField] private D_Pistol pistolData;
    [SerializeField] private D_Knife knifeData;

    [SerializeField] private GameObject machineGunPrefab;
    [SerializeField] private GameObject pistolPrefab;

    private Knife knife;
    public Weapon currentWeapon;
    private Weapon activeWeapon;

    //

    [SerializeField] TMP_Text ammoText;
    [SerializeField] TMP_Text rifleName;
    private int avatarID;
    private Image backgroundImage;
    [SerializeField] private Image playerAvatar;
    [SerializeField] private Sprite[] portraits;

    //

    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject deathEffect;

    //

    [SerializeField] private AudioSource walkingSoundEffect;
    [SerializeField] private AudioSource rifleSoundEffect;
    [SerializeField] private AudioSource knifeSoundEffect;
    [SerializeField] private AudioSource hurtSoundEffect;

    //

    public PhotonView getPhotonView()
    {
        return photonView;
    }

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine) playerCanvas.enabled = true;

        //backgroundImage = GetComponent<Image>();
        if (photonView.IsMine)
         {
             Player player = PhotonNetwork.LocalPlayer;
             int[] temp = ((int[])player.CustomProperties["playerStats"]);
             avatarID = temp[2];
             playerAvatar.sprite = portraits[avatarID];
         }
        
        if (photonView.IsMine) healthBar.setMaxHealth((int)maxHealth);

        photonID = PhotonNetwork.LocalPlayer.UserId;
        equipButtonPressed = false;
        sp = GameObject.FindObjectOfType<SpawnPoints>();
        
        knife = new Knife(knifeData, knifeData.maxAmmo);    // Creating default knife for player
        activeShootingPosition = knifeShootingPosition;     // Setting default fire position for knife
        activeWeapon = knife;
        
        isHurt = false;
        currentHealth = maxHealth;
        playerAnimator.SetBool("attack", false);

        equipWeapon(1, 45);

    }

    private void FixedUpdate()
    {
        Vector2 movementDirection = joystickMovement.Direction;
        movementDirection = movementDirection.normalized;
        horizontalDirection = movementDirection.x;
        verticalDirection = movementDirection.y;
        if (!isDead && photonView.IsMine) movementHandler();
        shootingDirection = joystickRotation.Direction;
        shootingDirection = shootingDirection.normalized;
        if (!isDead && joystickRotation.Direction != Vector2.zero && photonView.IsMine) rotationHandler();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (activeWeapon.id == -1) rifleName.text = "KNIFE";
            else if (activeWeapon.id == 0) rifleName.text = "RIFLE";
            else if (activeWeapon.id == 1) rifleName.text = "PISTOL";

            if (activeWeapon.id == -1)
            {
                ammoText.gameObject.SetActive(false);
            }
            else ammoText.gameObject.SetActive(true);

            ammoText.text = "AMMO: " + activeWeapon.ammoLeft;
        }
        
        if (currentWeapon != null && Time.time >= lastShotTime + 0.2f)
        {
            playerAnimator.SetBool("attack", false);
        }
        //if (!isDead) stabingHandler();

        photonView.RPC("endHurt", RpcTarget.AllBuffered);
        if (currentHealth <= 0 && !isDead && photonView.IsMine)
        {
            isDead = true;
            dieHandler();
        }

       playerCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
       playerCanvas.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    /*
        Handles inputs for player movement. Gets input from keyboard right now
    */
    private void movementHandler()
    {
        Vector3 targetVelocity = new Vector2(horizontalDirection * activeWeapon.movementSpeed, verticalDirection * activeWeapon.movementSpeed);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        //gameObject.transform.position += x;

        //GetComponent<PhotonView>().RPC("movementAnimations", RpcTarget.AllBuffered, horizontalDirection, verticalDirection);
    }

    /*
        Handles inputs for player rotation. Gets input from mouse right now.
    */

    private void rotationHandler()
    {
        //Vector2 direction = playerCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition) - transform.position;

        float angle = Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);

        if (Mathf.Abs(shootingDirection.x) >= joystickShootRange || Mathf.Abs(shootingDirection.y) >= joystickShootRange)
        {
            if (activeWeapon.id != -1) shootingHandler();
            else stabingHandler();
        }
        //rb.rotation = angle * Time.fixedDeltaTime * rotationSpeed;

    }

    /*
        Handles inputs for shooting action input from mouse right now.
    */
    public void shootingHandler()
    {
        if (!isDead && currentWeapon != null && activeWeapon == currentWeapon && currentWeapon.ammoLeft > 0
           && Time.time >= lastShotTime + currentWeapon.fireRate && photonView.IsMine)
        {
            photonView.RPC("triggerMuzzleFlash", RpcTarget.AllBuffered);
            playerAnimator.SetBool("attack", true);
            //Vector2 direction = playerCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition) - transform.position;

            float angle = Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);



            //rifleSoundEffect.Play();
            if(photonView.IsMine) photonView.RPC("playRifleSoundEffect", RpcTarget.AllBuffered);
            GameObject temp = PhotonNetwork.Instantiate(bulletPrefab.name, activeShootingPosition.position, rotation);
            temp.gameObject.GetComponent<PhotonView>().RPC("shoot", RpcTarget.AllBuffered, shootingDirection, currentWeapon.damageAmount, PhotonNetwork.LocalPlayer.UserId);
            lastShotTime = Time.time;
            currentWeapon.ammoLeft--;
        }
    }

    [PunRPC]
    public void playRifleSoundEffect()
    {
        
            AudioSource.PlayClipAtPoint(rifleSoundEffect.clip, transform.position);
        
    }

    public void shoot()
    {
        playerAnimator.SetBool("attack", true);
    }

    public void endShoot()
    {
        playerAnimator.SetBool("attack", false);
    }

    /*
       Handles inputs for stabing action with knife input from mouse right now.
   */

    public void stabingHandler()
    {
        if (Time.time >= lastShotTime + knife.fireRate)
        {
            GetComponent<PhotonView>().RPC("stab", RpcTarget.AllBuffered);
        }
    }

    public void stopStabHandler()
    {
        GetComponent<PhotonView>().RPC("endStab", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void stab()
    {
        playerAnimator.SetBool("attack", true);
    }

    [PunRPC]
    public void endStab()
    {
        playerAnimator.SetBool("attack", false);
    }

    public void checkPlayer()
    {
        RaycastHit2D[] hit;
        hit = Physics2D.CircleCastAll(activeShootingPosition.position, stabRadius, new Vector2(0f, 0f), playerLayer);
        foreach (RaycastHit2D x in hit)
        {
            if (x.collider != null)
            {
                if (!x.collider.GetComponent<PlayerController>().GetComponent<PhotonView>().IsMine)
                {
                    x.collider.GetComponent<PlayerController>().GetComponent<PhotonView>().RPC("hurt", RpcTarget.AllBuffered, activeWeapon.damageAmount, PhotonNetwork.LocalPlayer.UserId);
                }
            }
        }

    }

    /// /////////////////

    [PunRPC]
    public void die()
    {
        GameObject.Instantiate(deathEffect, transform);
        StartCoroutine(respawn());
    }

    public void dieHandler()
    {

        StatManager tempStatManager = FindObjectOfType<StatManager>();
        List<StatItem> items = tempStatManager.statItemList;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == PhotonNetwork.LocalPlayer.UserId)
            {
                int deathCount = ((int[])items[i].stats["playerStats"])[0] + 1;
                int killCount = ((int[])items[i].stats["playerStats"])[1];
                int portraitID = ((int[])items[i].stats["playerStats"])[2];
                int[] temp; temp = new int[3]; temp[0] = deathCount; temp[1] = killCount; temp[2] = portraitID;
                items[i].stats["playerStats"] = temp;
                PhotonNetwork.LocalPlayer.SetCustomProperties(items[i].stats);
                break;
            }
        }

        for (int i = 0; i < items.Count; i++)
        {

            if (items[i].id == shooterUserID)
            {
                Debug.Log("32");
                int deathCount = ((int[])items[i].stats["playerStats"])[0];
                int killCount = ((int[])items[i].stats["playerStats"])[1] + 1;
                int portraitID = ((int[])items[i].stats["playerStats"])[2];
                int[] temp; temp = new int[3]; temp[0] = deathCount; temp[1] = killCount; temp[2] = portraitID;
                items[i].stats["playerStats"] = temp;

                foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
                {
                    if (player.Value.UserId == shooterUserID)
                    {
                        Debug.Log("2");
                        player.Value.SetCustomProperties(items[i].stats);
                    }
                }
                break;
            }

        }

        PhotonNetwork.Instantiate("HealthPack", gameObject.transform.position, Quaternion.identity);
        FindObjectOfType<StatManager>().someoneIsDead();
        photonView.RPC("die", RpcTarget.AllBuffered);
    }

    IEnumerator respawn()
    {
        float respawnTime = 5f;
        transform.GetComponent<CircleCollider2D>().enabled = false;
        transform.GetComponent<SpriteRenderer>().enabled = false;
        if (photonView.IsMine) playerCanvas.enabled = false;

        while (respawnTime >= 0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1f;
        }

        while (true)
        {
            int spNum = Random.Range(0, sp.spawnPointNum);
            if (sp.spawnPointValueArr[spNum] == true)
            {
                transform.position = sp.spawnPointArr[spNum].position;
                break;
            }
        }

        isDead = false;

        transform.GetComponent<CircleCollider2D>().enabled = true;
        transform.GetComponent<SpriteRenderer>().enabled = true;
        if (photonView.IsMine) playerCanvas.enabled = true;
        playerAnimator.SetBool("hurt", false);
        //GetComponent<PhotonView>().RPC("restoreHealth", RpcTarget.AllBuffered);
        if (photonView.IsMine) currentHealth = maxHealth;
        healthBar.setHealth((int)currentHealth);
    }

    public void restoreHealth()
    {
        currentHealth = maxHealth;
    }

    [PunRPC]
    public void hurt(int damage, string shooterUserID)
    {
        hurtSoundEffect.Play();
        this.shooterUserID = shooterUserID;
        lastHurtTime = Time.time;
        isHurt = true;
        if (photonView.IsMine) currentHealth -= damage;
        if (photonView.IsMine) healthBar.setHealth((int)currentHealth);
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

    // Change Weapon

    [PunRPC]
    public void changeWeapon()
    {
        if (currentWeapon != null)
        {
            if (activeWeapon.id == -1)
            {
                activeWeapon = currentWeapon;
                if (currentWeapon.id == 0)
                {
                    activeShootingPosition = machineGunShootingPosition;
                    muzzleFlashPosition.position = machineGunShootingPosition.position;
                }
                else if (currentWeapon.id == 1)
                {
                    activeShootingPosition = pistolShootingPosition;
                    muzzleFlashPosition.position = pistolShootingPosition.position;
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

    public void changeWeaponHandler()
    {
        if (!isDead && photonView.IsMine) 
        {
            GetComponent<PhotonView>().RPC("changeWeapon", RpcTarget.AllBuffered); 
        }
    }

    // Drop Weapon

    [PunRPC]
    public void dropWeapon(int id, int ammoLeft)
    {
        Debug.Log("DROPPED AMMO_LEFT: " + ammoLeft + "DROPPED ID: " + id);
        ammo = ammoLeft;

        if (photonView.IsMine && id == 0)
        {
            droppedWeapon = true;
            GameObject temp = PhotonNetwork.Instantiate(machineGunPrefab.name, gameObject.transform.position, Quaternion.identity);
            //temp.GetComponent<WeaponWorld>().weapon = new MachineGun(machineGunData, ammoLeft);            
            photonView.RPC("changeWeapon", RpcTarget.AllBuffered);         
        }
        else if (photonView.IsMine && id == 1)
        {
            GameObject temp = PhotonNetwork.Instantiate(pistolPrefab.name, gameObject.transform.position, Quaternion.identity);
            droppedWeapon = true;
            //temp.GetComponent<WeaponWorld>().weapon = new Pistol(pistolData, ammoLeft);
            photonView.RPC("changeWeapon", RpcTarget.AllBuffered);

        }

    }

    public void dropWeaponHandler()
    {
        if (!isDead && activeWeapon.id != -1)
        {
            photonView.RPC("dropWeapon", RpcTarget.AllBuffered, currentWeapon.id, currentWeapon.ammoLeft);
            currentWeapon = null;
        }
    }

    // Equip Weapon


    public void equipWeapon(int id, int ammoLeft)
    {
        if (!isDead)
        {
            if (id == 0)
            {
                Debug.Log("0");
                currentWeapon = new MachineGun(machineGunData, ammoLeft);
                activeShootingPosition = machineGunShootingPosition;
                muzzleFlashPosition.position = machineGunShootingPosition.position;
            }
            else if (id == 1)
            {
                Debug.Log("1");
                currentWeapon = new Pistol(pistolData, ammoLeft);
                activeShootingPosition = pistolShootingPosition;
                muzzleFlashPosition.position = pistolShootingPosition.position;
            }

            Debug.Log("EQUIPPED AMMO_LEFT: " + currentWeapon.ammoLeft);
            playerAnimator.SetInteger("weaponID", currentWeapon.id);
            activeWeapon = currentWeapon;
        }

    }

    public void equipWeaponHandler()
    {
        if (isOnWeapon && !isDead)
        {
            setEquipped(true);
            //photonView.RPC("equipWeapon", RpcTarget.AllBuffered, tempID, tempAmmoLeft);
            equipWeapon(tempID, tempAmmoLeft);
        }
    }

    public void equipWeaponHolder(int tempID, int tempAmmoLeft)
    {
        this.tempID = tempID;
        this.tempAmmoLeft = tempAmmoLeft;
    }

    public bool getEquipped()
    {
        return equipped;
    }

    public void setEquipped(bool equipped)
    {
        this.equipped = equipped;
    }

    public void setIsOnWeapon(bool isOnWeapon)
    {
        this.isOnWeapon = isOnWeapon;
    }

    

    [PunRPC]
    public void destroyWeapon(int id)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView targetPhotonView = PhotonNetwork.GetPhotonView(id);
            PhotonNetwork.Destroy(targetPhotonView);
        }
    }

    public bool addHealth(int hp) {
        if(currentHealth == maxHealth) 
            return false;

        if (hp + currentHealth >= maxHealth)
        {
            restoreHealth();
            if (photonView.IsMine) healthBar.setHealth((int)currentHealth);
        }
        else
        {
            currentHealth += hp;
            if (photonView.IsMine) healthBar.setHealth((int)currentHealth);
        }
            
        Debug.Log(hp + "Added | " + "hp: " + currentHealth);
        return true;
    }
    
    [PunRPC]
    public void triggerMuzzleFlash()
    {
        muzzleFlashAnimator.SetTrigger("shoot");
    }

}
