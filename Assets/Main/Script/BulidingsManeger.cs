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
    public List<RootStetas> LeftRoot { get; private set; } = new List<RootStetas>();
    public List<RootStetas> RightRoot { get; private set; } = new List<RootStetas>();
    public List<RootStetas> LeftEnemyRoot { get; private set; } = new List<RootStetas>();
    public List<RootStetas> RightEnemyRoot { get; private set; } = new List<RootStetas>();

    private void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.L))
            .Subscribe(_ =>
            {
                //for(int i = 0;i < LeftRoot.Count; i++)
                //{
                //    Debug.Log("左ルート" + i + "番目は" + LeftRoot[i].number + "番目の" + LeftRoot[i].rootObject + "です");
                //}
                foreach (var o in LeftRoot)
                {
                    Debug.Log("左ルート" + o.number + "番目" + o.rootObject);
                }
            });
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ =>
            {
                //for (int i = 0; i < RightRoot.Count; i++)
                //{
                //    Debug.Log("右ルート" + i + "番目は" + RightRoot[i].number + "番目の" + RightRoot[i].rootObject + "です");
                //}
                foreach (var o in RightRoot)
                {
                    Debug.Log("右ルート" + o.number + "番目" + o.rootObject);
                }
            });
    }

    public void Enter(GameObject enter)
    {
        wayPoints.Add(enter);
    }

    public void InsertRoot(RootStetas rootStetas)
    {
        //Debug.Log("挿入場所" + rootStetas.number);
        switch (rootStetas.stetas)
        {
            case RootStetas.LRStates.Left:
                LeftRoot.Add(rootStetas);
                LeftRoot.Sort((x, y) => x.number - y.number);
                break;
            case RootStetas.LRStates.Right:
                RightRoot.Add(rootStetas);
                RightRoot.Sort((x, y) => x.number - y.number);
                break;
        }
        //LeftEnemyRoot = LeftRoot;
        //LeftEnemyRoot.Reverse();
        //RightEnemyRoot = RightRoot;
        //RightEnemyRoot.Reverse();
    }
}
