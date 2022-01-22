using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    private SpawnPoints sp;
    private int avatarID;
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private LayerMask weaponMask;
    private WeaponSpawnPoints wsp;

    void Start()
    {

        if(PhotonNetwork.IsConnected)
        {
            Music.StopMusic();
            wsp = GameObject.FindObjectOfType<WeaponSpawnPoints>();
            StartCoroutine(spawnWeapons());
            sp = GameObject.FindObjectOfType<SpawnPoints>();
            while (true)
            {
                int spNum = Random.Range(0, sp.spawnPointNum);
                if (sp.spawnPointValueArr[spNum] == true)
                {
                    PhotonNetwork.Instantiate(playerPrefab.name, sp.spawnPointArr[spNum].position, Quaternion.identity);
                    break;
                }
            }
                       
        }
              
    }

    private IEnumerator spawnWeapons()
    {
        yield return new WaitForSeconds(10);
        for (int i = 0; i < wsp.spawnPointNum; i++)
        {
            if (!Physics2D.CircleCast(wsp.spawnPointArr[i].transform.position, 1f, new Vector2(1f, 1f), 1f, weaponMask))
            {
                PhotonNetwork.Instantiate(weaponPrefab.name, wsp.spawnPointArr[i].position, Quaternion.identity);
            }
        }
        StartCoroutine(spawnWeapons());
    }


}
