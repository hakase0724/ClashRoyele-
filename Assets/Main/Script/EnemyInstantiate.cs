using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticUse;

public class EnemyInstantiate : InstantiateFiled
{
    [PunRPC]
    protected override IEnumerator MyInstantiateRPC(Vector3 pos, int id, float energy)
    {
        if (!IsSameId(id, PhotonNetwork.player.ID)) yield return null;
        base.MyInstantiateRPC(pos, id, energy);
    }
}
