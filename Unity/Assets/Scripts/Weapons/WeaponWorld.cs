using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class WeaponWorld : MonoBehaviour
{
    public Weapon weapon;
    [SerializeField] private int id;
    [SerializeField] private D_MachineGun machineGunData;
    [SerializeField] private D_Pistol pistolData;

    // CONSTRUCTORS

    public WeaponWorld(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public WeaponWorld(D_Pistol data, int ammoLeft)
    {
        weapon = new Pistol(data, ammoLeft);
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

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && (Input.GetKey(KeyCode.E)))
        {
            collision.gameObject.GetComponent<PhotonView>().RPC("equipWeapon", RpcTarget.AllBuffered, id, weapon.ammoLeft);
            PhotonNetwork.Destroy(gameObject.GetComponent<PhotonView>());
        }
    }
}
