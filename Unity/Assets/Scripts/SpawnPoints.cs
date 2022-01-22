using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    public Transform [] spawnPointArr;
    public bool[] spawnPointValueArr;
    public int spawnPointNum;
    public float radius;

    private void Awake()
    {
        spawnPointNum = spawnPointArr.Length;
        spawnPointValueArr = new bool[spawnPointNum];
        for (int i = 0; i < spawnPointNum; i++) spawnPointValueArr[i] = true;
    }

    private void Update()
    {
        checkSpawnPoints();
    }

    private void checkSpawnPoints()
    {
        for(int i  = 0; i < spawnPointNum; i++)
        {
            if (Physics2D.CircleCast(spawnPointArr[i].transform.position, radius, new Vector2(1f, 1f), 3f, playerMask))
            {
                spawnPointValueArr[i] = false;
            }
            else
            { 
                spawnPointValueArr[i] = true;
            }
        }
    }

    public float lookRadius = 10f;
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
