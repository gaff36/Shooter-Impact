using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawnPoints : MonoBehaviour
{
    [SerializeField] private LayerMask weaponMask;
    public Transform[] spawnPointArr;
    public int spawnPointNum;
    public float radius;

    private void Awake() {
        spawnPointNum = spawnPointArr.Length;

    }

    public float lookRadius = 10f;
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
