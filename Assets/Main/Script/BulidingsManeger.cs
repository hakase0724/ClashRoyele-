using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using static StaticUse;

/// <summary>
/// 建物管理
/// </summary>
public class BulidingsManeger : MonoBehaviour
{
    public List<GameObject> wayPoints { get; private set; } = new List<GameObject>();

    public void Enter(GameObject enter)
    {
        wayPoints.Add(enter);
    }
}
