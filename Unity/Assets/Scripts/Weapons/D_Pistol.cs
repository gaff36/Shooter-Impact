using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPistolData", menuName = "Data/Weapon Data/Pistol Data")]
public class D_Pistol : ScriptableObject
{
    public float fireRate;
    public float movementSpeed;
    public int damageAmount;
    public int maxAmmo;
}
