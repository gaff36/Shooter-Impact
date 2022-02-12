using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats
{
    private int deathCount;
    private int killCount;

    public Stats()
    {
        deathCount = 0;
        killCount = 0;
    }

    public Stats(int deathCount, int killCount)
    {
        this.deathCount = deathCount;
        this.killCount = killCount;
    }

    public void setDeathCount(int deathCount)
    {
        this.deathCount = deathCount;
    }

    public void setKillCount(int killCount)
    {
        this.killCount = killCount;
    }

    public int getDeathCount()
    {
        return deathCount;
    }

    public int getKillCount()
    {
        return killCount;
    }
}
