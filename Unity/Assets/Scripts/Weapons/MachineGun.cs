using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : Weapon
{
    public MachineGun(D_MachineGun data, int ammoLeft) : base(ammoLeft)
    {
        fireRate = data.fireRate;
        movementSpeed = data.movementSpeed;
        damageAmount = data.damageAmount;
        maxAmmo = data.maxAmmo;
        ammoLeft = this.ammoLeft;
        id = 0;
    }
}
