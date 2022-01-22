using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class WeaponWorld : MonoBehaviour
{
    public Weapon weapon;
    public int id;
    public D_MachineGun machineGunData;
    public D_Pistol pistolData;

    // CONSTRUCTORS

    public WeaponWorld(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public WeaponWorld(D_Pistol data, int ammoLeft)
    {
        weapon = new Pistol(data, ammoLeft);
    }

    public WeaponWorld(D_MachineGun data, int ammoLeft)
    {
        weapon = new MachineGun(data, ammoLeft);
    }

    public WeaponWorld(int ammoLeft)
    {
        if(id == 0)
        {
            weapon = new MachineGun(machineGunData, ammoLeft);
        }
        else if (id == 1)
        {
            weapon = new Pistol(pistolData, ammoLeft);
        }
    }

    public void Awake()
    {
        if (id == 0)
        {
            weapon = new MachineGun(machineGunData, machineGunData.maxAmmo);
        }
        else if (id == 1)
        {
            weapon = new Pistol(pistolData, pistolData.maxAmmo);
        }
    }

    // COLLISION
    
    public void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("CHECK1");
            if(collision.gameObject.GetComponent<PlayerController>().droppedWeapon)
            {
                Debug.Log("CHECK2");
                GetComponent<PhotonView>().RPC("changeAmmoWeapon", RpcTarget.AllBuffered, collision.gameObject.GetComponent<PlayerController>().ammo);
                collision.gameObject.GetComponent<PlayerController>().droppedWeapon = false;
            }

            Debug.Log("WEAPON ON THE GROUND AMMO_LEFT: " + weapon.ammoLeft);

            collision.gameObject.GetComponent<PlayerController>().setIsOnWeapon(true);
            collision.gameObject.GetComponent<PlayerController>().equipWeaponHolder(weapon.id, weapon.ammoLeft);
        }        
    }
 
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerController>().getEquipped())
            {
                
                collision.GetComponent<PlayerController>().setEquipped(false);
                collision.gameObject.GetComponent<PlayerController>().setIsOnWeapon(false);
                GetComponent<PhotonView>().RPC("destroyWeapon", RpcTarget.AllBuffered);
            }

            //collision.gameObject.GetComponent<PlayerController>().equipButtonPressed = false;
            //GetComponent<PhotonView>().RPC("disableWeapon", RpcTarget.AllBuffered);
            //PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            //PhotonNetwork.Destroy(GetComponent<PhotonView>());
            //if (GetComponent<PhotonView>().IsMine) PhotonNetwork.Destroy(gameObject.GetComponent<PhotonView>());
        }
    }

        public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().setIsOnWeapon(false);
            //GetComponent<PhotonView>().RPC("equipWeaponHolder", RpcTarget.AllBuffered, id, weapon.ammoLeft, gameObject.GetComponent<PhotonView>(), false);
            //collision.gameObject.GetComponent<PlayerController>().equipWeaponHolder(id, weapon.ammoLeft, gameObject.GetComponent<PhotonView>().ViewID, false);
        }
    }

    [PunRPC]
    public void destroyWeapon()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    public void changeAmmoWeapon(int ammo)
    {
        weapon.ammoLeft = ammo;
    }
    public void disableWeaponHandler()
    {
        GetComponent<PhotonView>().RPC("disableWeapon", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void disableWeapon()
    {
        gameObject.SetActive(false);
    }
}
