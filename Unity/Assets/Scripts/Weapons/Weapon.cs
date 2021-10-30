using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Weapon
{
    public float fireRate;
    public float movementSpeed;
    public int id;
    public int damageAmount;
    public int ammoLeft;
    public int maxAmmo;

    public Weapon(float fireRate, float movementSpeed, int id, int damageAmount, int ammoLeft)
    {
        this.fireRate = fireRate;
        this.movementSpeed = movementSpeed;
        this.id = id;
        this.damageAmount = damageAmount;
        this.ammoLeft = ammoLeft;
    }

    public Weapon(float fireRate, float movementSpeed, int id, int damageAmount)
    {
        this.fireRate = fireRate;
        this.movementSpeed = movementSpeed;
        this.id = id;
        this.damageAmount = damageAmount;
        ammoLeft = maxAmmo;
    }

    public Weapon(int ammoLeft)
    {
        this.ammoLeft = ammoLeft;
    }

}