using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class ITweenMoveTest : MonoBehaviour
{
    private Vector3 target;
    [SerializeField]
    private GameObject[] targetList = new GameObject[0];
	// Use this for initialization
	void Start ()
    {
        //var pos = new List<Vector3>();
        //pos.Add(this.transform.position);
        //foreach(var t in targetList)
        //{
        //    pos.Add(t.transform.position);
        //}
        //var p = pos.ToArray();
        //iTween.MoveTo(gameObject, iTween.Hash("path",p,"time",10, "easetype", iTween.EaseType.easeOutSine,"islocal",true));
        int posPointa = 0;
        target = targetList[posPointa].transform.position;
        var move = this.UpdateAsObservable().Subscribe(_ => iTween.MoveUpdate(gameObject, iTween.Hash("position", target, "time", 10, "easetype", iTween.EaseType.easeOutSine)));
        this.UpdateAsObservable()
            .Where(_ => (transform.position - target).sqrMagnitude <= 1)
            .Subscribe(_ => 
            {
                posPointa++;
                target = targetList[posPointa].transform.position;
            });
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.P))
            .Subscribe(_ => iTween.Pause());
        this.UpdateAsObservable()
           .Where(_ => Input.GetKeyDown(KeyCode.R))
           .Subscribe(_ => iTween.Resume());
        
    }
}
