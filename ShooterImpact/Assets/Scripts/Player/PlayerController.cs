using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using SimpleInputNamespace;
using UnityEngine.Networking;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // DROP WEAPON VARIABLES

    public bool droppedWeapon;
    public int ammo;
    public int clip;
    [SerializeField] private Sprite[] weaponPortraits;
    [SerializeField] private Image weaponPortrait;

    // EQUIP WEAPON VARIABLES

    private bool equipped;
    private bool isOnWeapon;
    private int tempID;
    public int tempAmmoLeft;
    public int tempAmmoLeftOnClip;
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
    [SerializeField] private int slowedMovementSpeed;
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

    private float movementSoundCooldown = 0.5f;
    private float lastMovementTime = 0f;
    private bool isAttacking;
    private bool isReloading;
    public string photonID;
    private int maxHealth = 100;
    public int currentHealth;
    private Animator playerAnimator;
    [SerializeField] private Animator muzzleFlashAnimator;
    [SerializeField] private Transform muzzleFlashPosition;

    private PhotonView photonView;
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
    private int avatarID;
    private Image backgroundImage;
    [SerializeField] private Image playerAvatar;
    [SerializeField] private Sprite[] portraits;

    //

    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject deathEffect;

    // Sound Effects

    [SerializeField] private AudioSource walkingSoundEffect;
    [SerializeField] private AudioSource rifleSoundEffect;
    [SerializeField] private AudioSource knifeSoundEffect;
    [SerializeField] private AudioSource deathSoundEffect;
    [SerializeField] private AudioSource reloadSoundEffect;
    [SerializeField] private AudioSource rifleEquipSoundEffect;
    [SerializeField] private AudioSource knifeEquipSoundEffect;
    [SerializeField] private AudioSource healSoundEffect;
    [SerializeField] private AudioSource ammoAddSoundEffect;

    //

    [SerializeField] private float stabingTime;
    public bool isStabing = false;
    public GameObject reloadingImage;
    public int ammoCapacity;
    private double startTime;
    [SerializeField] private TMP_Text timeText;
    public int maxKills;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private TMP_Text winnerText;
    private ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
    private bool timeStarted;
    private bool gameEnded;
    private bool gameEnded2;
    public double remaningTime;
    public GameOverController goc;

    //


    public PhotonView getPhotonView()
    {
        return photonView;
    }

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
        
    }

    private void Start()
    {
        timeStarted = false;
        gameEnded = false;
        gameEnded2 = false;

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            ht["StartTime"] = PhotonNetwork.Time;
            PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
        }

        isAttacking = false;
        playerAnimator.SetBool("reload", false);
        isReloading = false;
        
        if (photonView.IsMine) playerCanvas.enabled = true;

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
        
        knife = new Knife(knifeData, knifeData.maxAmmo, knifeData.maxAmmo);    // Creating default knife for player
        
        isHurt = false;
        currentHealth = maxHealth;

        equipWeapon(1, 45, 10); 
    }

    private void FixedUpdate()
    {
        Vector2 movementDirection = joystickMovement.Direction;
        movementDirection = movementDirection.normalized;
        horizontalDirection = movementDirection.x;
        verticalDirection = movementDirection.y;
        if (!isDead && photonView.IsMine) movementHandler();
        shootingDirection = joystickRotation.Direction;
        if (!isDead && joystickRotation.Direction != Vector2.zero && photonView.IsMine) rotationHandler();

        playerCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        playerCanvas.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void Update()
    {
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("StartTime") && !timeStarted)
        {
            startTime = (double) PhotonNetwork.CurrentRoom.CustomProperties["StartTime"];
            timeStarted = true;
        }

        Player win = null;
        if (timeStarted)
        {
            remaningTime = 600 - (PhotonNetwork.Time - startTime);
            int minutes;
            int seconds;
            if (remaningTime >= 0) minutes = (int)(remaningTime / 60);
            else minutes = 0;
            if (remaningTime >= 0) seconds = (int)(remaningTime - (minutes * 60));
            else seconds = 0;

            if (remaningTime <= 0 && !gameEnded2 && PhotonNetwork.IsMasterClient)
            {
                gameEnded2 = true;
                win = decideWinner();
            }

            timeText.text = minutes + ":" + seconds;
        }

        Player tempPlayer = null;
        if (!gameEnded && PhotonNetwork.IsMasterClient) tempPlayer = checkIfSomeoneWon();

        if (tempPlayer != null && !gameEnded)
        {      
            gameEnded = true;
            photonView.RPC("fuckMe", RpcTarget.All);      
            //PhotonNetwork.CloseConnection(PhotonNetwork.LocalPlayer);
            //SceneManager.LoadScene(0);
        }

        if (gameEnded)
        {
            winnerPanel.SetActive(true);
            winnerText.text = tempPlayer.NickName + " WON THE GAME!";
            StartCoroutine("endGame");
        }
        if (gameEnded2)
        {
            winnerPanel.SetActive(true);
            winnerText.text = win.NickName + " WON THE GAME!";
            StartCoroutine("endGame");
        }

        if (photonView.IsMine)
        {
            if (activeWeapon.id == -1)
            {
                ammoText.gameObject.SetActive(false);
            }
            else ammoText.gameObject.SetActive(true);

            ammoText.text = activeWeapon.ammoLeftOnClip + "/" + activeWeapon.ammoLeft;
        }
        
        if (activeWeapon.id != -1 && Time.time >= lastShotTime + 0.2f)
        {
            playerAnimator.SetBool("attack", false);
        }

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
        if((verticalDirection != 0 || horizontalDirection != 0) && Time.time >= lastMovementTime + movementSoundCooldown)
        {
            lastMovementTime = Time.time;
            if (photonView.IsMine) photonView.RPC("playMovementSoundEffect", RpcTarget.AllBuffered);
        }

        Vector3 targetVelocity;
        if(isHurt) targetVelocity = new Vector2(horizontalDirection * slowedMovementSpeed, verticalDirection * slowedMovementSpeed);
        else targetVelocity = new Vector2(horizontalDirection * activeWeapon.movementSpeed, verticalDirection * activeWeapon.movementSpeed);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }

    IEnumerator endGame()
    {
        yield return new WaitForSeconds(5f);
        goc.gameOver();
    }

    [PunRPC]
    public void playMovementSoundEffect()
    {
        walkingSoundEffect.PlayOneShot(walkingSoundEffect.clip);
    }

    [PunRPC]
    public void fuckMe()
    {
        gameEnded = true;
    }

    /*
        Handles inputs for player rotation. Gets input from mouse right now.
    */

    private void rotationHandler()
    {
        float angle = Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
         
        float dist = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(shootingDirection.x), 2) + Mathf.Pow(Mathf.Abs(shootingDirection.y), 2));
        if (dist >= joystickShootRange)
        {
            isAttacking = true;
            if (activeWeapon.id == -1) stabingHandler();
            else shootingHandler();
        }
        isAttacking = false;
    }

    /*
        Handles inputs for shooting action input from mouse right now.
    */
    public void shootingHandler()
    {
        if (!isDead && currentWeapon != null && activeWeapon.id != -1 && currentWeapon.ammoLeftOnClip > 0
           && Time.time >= lastShotTime + currentWeapon.fireRate && photonView.IsMine && !isReloading)
        {
            photonView.RPC("triggerMuzzleFlash", RpcTarget.AllBuffered);
            playerAnimator.SetBool("attack", true);
            float angle = Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (photonView.IsMine) photonView.RPC("playRifleSoundEffect", RpcTarget.AllBuffered);
            GameObject temp = PhotonNetwork.Instantiate(bulletPrefab.name, activeShootingPosition.position, rotation);
            temp.gameObject.GetComponent<PhotonView>().RPC("shoot", RpcTarget.AllBuffered, shootingDirection, currentWeapon.damageAmount, PhotonNetwork.LocalPlayer.UserId);
            lastShotTime = Time.time;
            currentWeapon.ammoLeftOnClip--;
        }
        else if (!isDead && currentWeapon != null && activeWeapon.id != -1 && currentWeapon.ammoLeftOnClip == 0 && photonView.IsMine && !isReloading) reloadHandler();
    }

    [PunRPC]
    public void playRifleSoundEffect()
    {
        rifleSoundEffect.Play();
        //AudioSource.PlayClipAtPoint(rifleSoundEffect.clip, transform.position);  
    }

    [PunRPC]
    public void playKnifeSoundEffect()
    {
        knifeSoundEffect.Play();
        //AudioSource.PlayClipAtPoint(knifeSoundEffect.clip, transform.position);
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
        if (Time.time >= lastShotTime + knife.fireRate && !isStabing)
        {
            isStabing = true;
            lastShotTime = Time.time;
            if (photonView.IsMine) photonView.RPC("playKnifeSoundEffect", RpcTarget.AllBuffered);
            playerAnimator.SetBool("attack", true);            
        }
    }

    public void endStab()
    {
        isStabing = false;
        playerAnimator.SetBool("attack", false);
    }

    /// /////////////////

    [PunRPC]
    private void KickPlayer()
    {
        PhotonNetwork.LeaveRoom(); // load lobby scene, returns to master server
    }

    public void SendKickPlayer()
    {
        //Player player = PhotonNetwork.Find(playerID);
        //PhotonNetwork.pla

        photonView.RPC("KickPlayer", PhotonNetwork.LocalPlayer);
    }

    public Player checkIfSomeoneWon()
    {
        int highestKill = 0;
        Player winner = null;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            int killCount = ((int[])(player.Value.CustomProperties["playerStats"]))[1];
            if (killCount >= maxKills)
            {
                winner = player.Value;
                break;
            }
        }
        return winner;
    }

    public Player decideWinner()
    {
        int highestKill = 0;
        Player winner = null;
        foreach (KeyValuePair <int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            int killCount = ((int[])(player.Value.CustomProperties["playerStats"]))[1];
            if (killCount >= highestKill)
            {
                highestKill = killCount;
                winner = player.Value;
            }
        }
        return winner;
    }

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
                int deathCount = ((int[])items[i].stats["playerStats"])[0];
                int killCount = ((int[])items[i].stats["playerStats"])[1] + 1;
                int portraitID = ((int[])items[i].stats["playerStats"])[2];
                int[] temp; temp = new int[3]; temp[0] = deathCount; temp[1] = killCount; temp[2] = portraitID;
                items[i].stats["playerStats"] = temp;
               
                foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
                {
                    if (player.Value.UserId == shooterUserID)
                    {
                        player.Value.SetCustomProperties(items[i].stats);
                    }
                }
                break;
            }
        }

        PhotonNetwork.Instantiate("HealthPack", gameObject.transform.position, Quaternion.identity);
        FindObjectOfType<StatManager>().someoneIsDead();
        if (photonView.IsMine) photonView.RPC("playDeathSoundEffect", RpcTarget.AllBuffered);
        photonView.RPC("die", RpcTarget.AllBuffered);
    }

    IEnumerator respawn()
    {
        rb.velocity = Vector2.zero;
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

        equipWeapon(1, 45, 10);
        isDead = false;
        transform.GetComponent<CircleCollider2D>().enabled = true;
        transform.GetComponent<SpriteRenderer>().enabled = true;
        if (photonView.IsMine) playerCanvas.enabled = true;
        playerAnimator.SetBool("hurt", false);
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
        this.shooterUserID = shooterUserID;
        lastHurtTime = Time.time;
        isHurt = true;
        if (photonView.IsMine) currentHealth -= damage;
        if (photonView.IsMine) healthBar.setHealth((int)currentHealth);
        playerAnimator.SetBool("hurt", true);        
    }

    [PunRPC]
    public void playDeathSoundEffect()
    {
        deathSoundEffect.Play();
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
                activeShootingPosition = knifeShootingPosition;
                activeWeapon = knife;              
            }
            playerAnimator.SetInteger("weaponID", activeWeapon.id);
        }
    }

    public void changeWeaponHandler()
    {
        if (!isDead && photonView.IsMine && !isAttacking) 
        {
            playerAnimator.SetBool("attack", false);
            GetComponent<PhotonView>().RPC("changeWeapon", RpcTarget.AllBuffered);

            if (activeWeapon.id == -1) knifeEquipSoundEffect.Play();
            else rifleEquipSoundEffect.Play();

            weaponPortrait.sprite = weaponPortraits[activeWeapon.id + 1];
            playerAnimator.SetBool("reload", false);
            isReloading = false;
            reloadingImage.SetActive(false);
        }
    }

    // Drop Weapon

    [PunRPC]
    public void dropWeapon(int id, int ammoLeft, int ammoLeftOnClip)
    {
        ammo = ammoLeft;
        clip = ammoLeftOnClip;

        if (photonView.IsMine && id == 0)
        {
            droppedWeapon = true;
            GameObject temp = PhotonNetwork.Instantiate(machineGunPrefab.name, gameObject.transform.position, Quaternion.identity);           
            photonView.RPC("changeWeapon", RpcTarget.AllBuffered);         
        }
        else if (photonView.IsMine && id == 1)
        {
            GameObject temp = PhotonNetwork.Instantiate(pistolPrefab.name, gameObject.transform.position, Quaternion.identity);
            droppedWeapon = true;
            photonView.RPC("changeWeapon", RpcTarget.AllBuffered);

        }

    }

    public void dropWeaponHandler()
    {
        if (!isDead && activeWeapon.id != -1 && !isAttacking)
        {
            playerAnimator.SetBool("attack", false);
            photonView.RPC("dropWeapon", RpcTarget.AllBuffered, currentWeapon.id, currentWeapon.ammoLeft, currentWeapon.ammoLeftOnClip);
            knifeEquipSoundEffect.Play();
            currentWeapon = null;
            weaponPortrait.sprite = weaponPortraits[activeWeapon.id + 1];
            playerAnimator.SetBool("reload", false);
            isReloading = false;
            reloadingImage.SetActive(false);
        }
    }

    // Equip Weapon


    public void equipWeapon(int id, int ammoLeft, int ammoLeftOnClip)
    {
        if (!isDead)
        {
            if (id == 0)
            {
                currentWeapon = new MachineGun(machineGunData, ammoLeft, ammoLeftOnClip);
                activeShootingPosition = machineGunShootingPosition;
                muzzleFlashPosition.position = machineGunShootingPosition.position;
            }
            else if (id == 1)
            {
                currentWeapon = new Pistol(pistolData, ammoLeft, ammoLeftOnClip);
                activeShootingPosition = pistolShootingPosition;
                muzzleFlashPosition.position = pistolShootingPosition.position;
            }

            playerAnimator.SetInteger("weaponID", currentWeapon.id);
            activeWeapon = currentWeapon;
        }

    }

    public void equipWeaponHandler()
    {
        if (isOnWeapon && !isDead && !isAttacking)
        {
            
            if (currentWeapon != null)
            {
                photonView.RPC("dropWeapon", RpcTarget.AllBuffered, currentWeapon.id, currentWeapon.ammoLeft, currentWeapon.ammoLeftOnClip);
            }

            playerAnimator.SetBool("attack", false);
            rifleEquipSoundEffect.Play();
            setEquipped(true);
            equipWeapon(tempID, tempAmmoLeft, tempAmmoLeftOnClip);
            weaponPortrait.sprite = weaponPortraits[activeWeapon.id + 1];
            isReloading = false;
            playerAnimator.SetBool("reload", false);
            reloadingImage.SetActive(false);
        }
    }

    public void equipWeaponHolder(int tempID, int tempAmmoLeft, int tempAmmoLeftOnClip)
    {
        this.tempID = tempID;
        this.tempAmmoLeft = tempAmmoLeft;
        this.tempAmmoLeftOnClip = tempAmmoLeftOnClip;
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

    // Reload Weapon

    public void reloadHandler()
    {
        if(activeWeapon.id != -1 && !isReloading)
        {
            if (activeWeapon.id == 0 && machineGunData.clipSize != activeWeapon.ammoLeftOnClip && activeWeapon.ammoLeft != 0)
            {
                isReloading = true;
                playerAnimator.SetBool("reload", true);
                reloadingImage.SetActive(true);
                StartCoroutine("reloadWeapon", machineGunData.reloadTime);
            }
            else if (activeWeapon.id == 1 && pistolData.clipSize != activeWeapon.ammoLeftOnClip && activeWeapon.ammoLeft != 0)
            {
                playerAnimator.SetBool("reload", true);
                reloadingImage.SetActive(true);
                isReloading = true;
                StartCoroutine("reloadWeapon", pistolData.reloadTime);
            }
        }
    }

    IEnumerator reloadWeapon(float reloadTime)
    {
        reloadSoundEffect.Play();
        yield return new WaitForSeconds(reloadTime);

        if (activeWeapon.id == 0 && isReloading)
        {
            int temp = machineGunData.clipSize - activeWeapon.ammoLeftOnClip;
            if (activeWeapon.ammoLeft >= temp)
            {
                activeWeapon.ammoLeft -= temp;
                activeWeapon.ammoLeftOnClip = machineGunData.clipSize;
            }
            else
            {
                activeWeapon.ammoLeftOnClip = activeWeapon.ammoLeft;
                activeWeapon.ammoLeft = 0;
            }
            playerAnimator.SetBool("reload", false);
            reloadingImage.SetActive(false);
            isReloading = false;
        }
        else if (activeWeapon.id == 1 && isReloading)
        {
            int temp = pistolData.clipSize - activeWeapon.ammoLeftOnClip;
            if (activeWeapon.ammoLeft >= temp)
            {
                activeWeapon.ammoLeft -= temp;
                activeWeapon.ammoLeftOnClip = pistolData.clipSize;               
            }
            else
            {
                activeWeapon.ammoLeftOnClip = activeWeapon.ammoLeft;
                activeWeapon.ammoLeft = 0;
            }
            playerAnimator.SetBool("reload", false);
            reloadingImage.SetActive(false);
            isReloading = false;
        }       
    }

    public void addAmmo(int ammoAmount)
    {
        ammoAddSoundEffect.Play();
        if (currentWeapon.ammoLeft + ammoAmount >= ammoCapacity) currentWeapon.ammoLeft = ammoCapacity;
        else currentWeapon.ammoLeft += ammoAmount; 
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
            healSoundEffect.Play();
            if (photonView.IsMine) healthBar.setHealth((int)currentHealth);
        }
        else
        {
            healSoundEffect.Play();
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
