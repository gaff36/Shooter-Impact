using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : Weapon
{
    public MachineGun(D_MachineGun data)
    {
        fireRate = data.fireRate;
        movementSpeed = data.movementSpeed;
        animatorID = data.animatorID;
        damageAmount = data.damageAmount;
    }
}
