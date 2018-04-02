using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterTower : Tower
{
    public override void ReleaseTransform()
    {
        if(!isMine.Value) main.EnemyCount(-3);
    }
}
