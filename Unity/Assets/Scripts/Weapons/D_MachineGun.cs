using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMachineGunData", menuName = "Data/Weapon Data/MachineGun")]
public class D_MachineGun : ScriptableObject
{
    public float fireRate;
    public float movementSpeed;
    public int animatorID;
    public int damageAmount;
}
