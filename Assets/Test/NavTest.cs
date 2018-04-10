using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using Photon;

public class NavTest : Photon.MonoBehaviour
{
    public float Speed = 1.0f;
    public GameObject target;
    //private ReactiveProperty<GameObject> target = new ReactiveProperty<GameObject>();
    //private List<GameObject> targets = new List<GameObject>();
    private NavMeshAgent nav => GetComponent<NavMeshAgent>();
    private byte myCode = 0;
    private void Awake()
    {
        PhotonNetwork.OnEventCall += this.OnEvent;
    }
    private void OnEnable()
    {
        if (!photonView.isMine) nav.Warp(-transform.position);
    }
	// Use this for initialization
	void Start ()
    {
        target = GameObject.FindGameObjectWithTag("Enemy");
        nav.SetDestination(target.transform.position);

        this.UpdateAsObservable()
           .Where(_ => Input.GetKeyDown(KeyCode.S))
           .Subscribe(_ => PhotonNetwork.RaiseEvent(myCode, transform.position, true, null));
    }

    private void OnEvent(byte evCode, Vector3 content)
    {
        if (myCode == evCode) Debug.Log("受信：" + content);
    }

    private void OnEvent(byte evCode, object content,int senderId)
    {
        var pos = (Vector3)content;
        if (myCode == evCode) Debug.Log("受信：" + pos);
        nav.Warp(-pos);
    }

    //private List<float> CalcDistance()
    //{
    //    List<float> distances = new List<float>();
    //    float distance = 0f;
    //    foreach(var t in targets)
    //    {
    //        distance = (this.transform.position - t.transform.position).sqrMagnitude;
    //        distances.Add(distance);
    //    }
    //    return distances;
    //}

    //private float CalcDistance(Vector3 pos)
    //{
    //    return (this.transform.position - pos).sqrMagnitude;
    //}

    //private GameObject SearchMinDistance(List<float> distances)
    //{
    //    float ignoreDistance = 2f;
    //    var minIdx = distances
    //        .Select((val, idx) => new { V = val, I = idx })
    //        .Where((min, working) => min.V > ignoreDistance * ignoreDistance)
    //        .Aggregate((min, working) => (min.V < working.V) ? min : working).I;
    //    return targets[minIdx];
    //}

    public void TriggerOn(GameObject other)
    {
        //if (targets.Select(x => x).Any(x => x == other))
        //{
        //    Debug.Log(other.name);
        //    target.Value = SearchMinDistance(CalcDistance());
        //}
    }

    public void Attack(GameObject other)
    {
        //if (other.tag == "Enemy") target.Value = other;
    }
}
