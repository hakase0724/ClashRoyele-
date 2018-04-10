using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TargetGet : MonoBehaviour
{
    [SerializeField]
    private GameObject[] mineArray = new GameObject[3];
    [SerializeField]
    private GameObject[] enemyArray = new GameObject[3];
    public GameObject[] mArray { get { return mineArray; } }
    public GameObject[] eArray { get { return enemyArray; } }
    //public List<Vector3> mine { get; private set; } = new List<Vector3>();
    //public List<Vector3> enemys { get; private set; } = new List<Vector3>();

    //private void Start()
    //{
    //    //this.UpdateAsObservable()
    //    //    .Where(_ => Input.GetKeyDown(KeyCode.M))
    //    //    .Subscribe(_ =>
    //    //    {
    //    //        foreach (var m in mine)
    //    //        {
    //    //            Debug.Log(m);
    //    //        }
    //    //    });

    //    //this.UpdateAsObservable()
    //    //   .Where(_ => Input.GetKeyDown(KeyCode.E))
    //    //   .Subscribe(_ =>
    //    //   {
    //    //       foreach (var m in enemys)
    //    //       {
    //    //           Debug.Log(m);
    //    //       }
    //    //   });
    //}

    //public void Enter(GameObject obj,bool isMine)
    //{
    //    if (!isMine) mine.Add(obj.transform.position);
    //    else enemys.Add(obj.transform.position);
    //}
}
