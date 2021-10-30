using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    public Pistol(D_Pistol data, int ammoLeft) : base(ammoLeft)
    {
        fireRate = data.fireRate;
        movementSpeed = data.movementSpeed;
        damageAmount = data.damageAmount;
        maxAmmo = data.maxAmmo;
        ammoLeft = this.ammoLeft;
    }
}
