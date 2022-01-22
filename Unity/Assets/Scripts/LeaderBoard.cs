using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] GameObject leaderBoard;

    public void off()
    {
        leaderBoard.SetActive(false);
        Time.timeScale = 1f;
    }
    public void on()
    {
        leaderBoard.SetActive(true);
        Time.timeScale = 1f;
    }
}
