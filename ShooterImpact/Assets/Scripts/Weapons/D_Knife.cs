using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newKnifeData", menuName = "Data/Weapon Data/Knife Data")]
public class D_Knife : ScriptableObject
{
    public float fireRate;
    public float movementSpeed;
    public int damageAmount;
    public int maxAmmo;
}