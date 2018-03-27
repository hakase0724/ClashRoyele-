using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのデータ
/// </summary>
public class PlayerData 
{
    public string playerName { get; set; }
    public int playerId { get; set; }

    public PlayerData()
    {
        playerName = "";
        playerId = 0;
    }
}
