using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using static StaticUse;

public class MoveTest : MonoBehaviour,IUnit
{
    public enum States
    {
        Normal,
        Clash,
        Attack
    }

    private States myStates = States.Normal;
    private RootStetas.LRStates myLRStates;
    public BoolReactiveProperty isMine { get; set; }
    public FloatReactiveProperty unitHp { get; set; }
    public float unitSpeed { get; set; }
    public float unitEnergy { get; set; }
    public float maxUnitHp { get; }

    private Rigidbody rb => GetComponent<Rigidbody>();
    private List<RootStetas> wayPoints = new List<RootStetas>();
    private TestManeger maneger => GameObject.FindGameObjectWithTag("Main").GetComponent<TestManeger>();
    private IntReactiveProperty targetPointa = new IntReactiveProperty(0);
    private byte myCode = 0;

    private void Awake()
    {
        if (transform.position.z <= 0) myLRStates = RootStetas.LRStates.Left;
        else myLRStates = RootStetas.LRStates.Right;
    }

    private  void Start ()
    {
        switch (myLRStates)
        {
            case RootStetas.LRStates.Left:
                wayPoints = maneger.LeftRoot;
                break;
            case RootStetas.LRStates.Right:
                wayPoints = maneger.RightRoot;
                break;
        }

        this.UpdateAsObservable()
            .Subscribe(_ => Move())
            .AddTo(gameObject);

        this.UpdateAsObservable()
            .Where(_ => CalcDistance(transform.position, wayPoints[targetPointa.Value].rootObject.transform.position) <= 0.5f)
            .Subscribe(_ => targetPointa.Value++)
            .AddTo(gameObject);

        Observable.Interval(System.TimeSpan.FromSeconds(3))
            .Subscribe(_ => PhotonNetwork.RaiseEvent(myCode, transform.position, true, null));
	}

    private void OnEvent(byte evCode, Vector3 content)
    {
        if (myCode == evCode) Debug.Log("受信：" + content);
    }

    public void MyColor(int id)
    {
        
    }

    public void Move()
    {
        switch (myStates)
        {
            case States.Normal:
                transform.LookAt(wayPoints[targetPointa.Value].rootObject.transform);
                rb.velocity = transform.forward;
                break;
            case States.Clash:
                rb.velocity = transform.right;
                break;
            case States.Attack:
                rb.velocity = Vector3.zero;
                break;
        }
           
    }

    public void Attack(float attack, GameObject attackTarget)
    {
        
    }

    public void Damage(float damage)
    {
        unitHp.Value -= damage;
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision col)
    {
        myStates = States.Clash;
    }

    private void OnCollisionExit(Collision col)
    {
        myStates = States.Normal;
    }
}
