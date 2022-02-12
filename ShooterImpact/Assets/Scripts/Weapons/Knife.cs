using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Weapon
{
    public Knife(D_Knife data, int ammoLeft, int ammoLeftOnClip) : base(ammoLeft, ammoLeftOnClip)
    {
        fireRate = data.fireRate;
        movementSpeed = data.movementSpeed;
        damageAmount = data.damageAmount;
        maxAmmo = data.maxAmmo;
        ammoLeft = this.ammoLeft;
        ammoLeftOnClip = this.ammoLeftOnClip;
        id = -1;
    }
}
