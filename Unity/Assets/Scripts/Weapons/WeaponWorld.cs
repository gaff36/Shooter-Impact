using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class WeaponWorld : MonoBehaviour
{
    private Weapon weapon;
    [SerializeField] private int id;
    [SerializeField] private D_MachineGun machineGunData;
    [SerializeField] private D_Pistol pistolData;

    public int ammoLeft;



    // CONSTRUCTORS

    public WeaponWorld(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public WeaponWorld(D_MachineGun data, int ammoLeft)
    {
        weapon = new MachineGun(data, ammoLeft);
    }

    public WeaponWorld(D_Pistol data, int ammoLeft)
    {
        weapon = new Pistol(data, ammoLeft);
    }



    // COLLISION

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && (Input.GetKey(KeyCode.E)))
        {          
            collision.gameObject.GetComponent<PhotonView>().RPC("equipWeapon", RpcTarget.AllBuffered, id, ammoLeft);
        }
    }
}
