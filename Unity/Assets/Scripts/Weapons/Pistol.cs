using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    public Pistol(D_Pistol data)
    {
        fireRate = data.fireRate;
        movementSpeed = data.movementSpeed;
        animatorID = data.animatorID;
        damageAmount = data.damageAmount;
    }
}
