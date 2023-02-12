using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public string playername { get; private set; }
    public int team { get; private set; }

    public PlayerData(string name, int teamid)
    {
        playername = name;
        team = teamid;
    }

  
}
