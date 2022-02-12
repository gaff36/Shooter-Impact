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

    public WeaponWorld(D_Pistol data, int ammoLeft, int ammoLeftOnClip)
    {
        weapon = new Pistol(data, ammoLeft, ammoLeftOnClip);
    }

    public WeaponWorld(D_MachineGun data, int ammoLeft, int ammoLeftOnClip)
    {
        weapon = new MachineGun(data, ammoLeft, ammoLeftOnClip);
    }

    public WeaponWorld(int ammoLeft, int ammoLeftOnClip)
    {
        if(id == 0)
        {
            weapon = new MachineGun(machineGunData, ammoLeft, ammoLeftOnClip);
        }
        else if (id == 1)
        {
            weapon = new Pistol(pistolData, ammoLeft, ammoLeftOnClip);
        }
    }

    public void Awake()
    {
        if (id == 0)
        {
            weapon = new MachineGun(machineGunData, machineGunData.maxAmmo, machineGunData.clipSize);
        }
        else if (id == 1)
        {
            weapon = new Pistol(pistolData, pistolData.maxAmmo, pistolData.clipSize);
        }
    }

    // COLLISION
    
    public void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            if(collision.gameObject.GetComponent<PlayerController>().droppedWeapon)
            {
                GetComponent<PhotonView>().RPC("changeAmmoWeapon", RpcTarget.AllBuffered, collision.gameObject.GetComponent<PlayerController>().ammo, collision.gameObject.GetComponent<PlayerController>().clip);
                collision.gameObject.GetComponent<PlayerController>().droppedWeapon = false;
            }

            if (collision.gameObject.GetComponent<PlayerController>().currentWeapon != null && weapon.id == collision.gameObject.GetComponent<PlayerController>().currentWeapon.id
               && collision.gameObject.GetComponent<PlayerController>().currentWeapon.ammoLeft != collision.gameObject.GetComponent<PlayerController>().ammoCapacity)
            {
                collision.gameObject.GetComponent<PlayerController>().addAmmo(weapon.ammoLeft);
                GetComponent<PhotonView>().RPC("destroyWeapon", RpcTarget.AllBuffered);
            }
            else
            {
                collision.gameObject.GetComponent<PlayerController>().setIsOnWeapon(true);
                collision.gameObject.GetComponent<PlayerController>().equipWeaponHolder(weapon.id, weapon.ammoLeft, weapon.ammoLeftOnClip);
            }
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
        }
    }

        public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().setIsOnWeapon(false);
        }
    }

    [PunRPC]
    public void destroyWeapon()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    public void changeAmmoWeapon(int ammo, int clip)
    {
        weapon.ammoLeft = ammo;
        weapon.ammoLeftOnClip = clip;
    }

}
