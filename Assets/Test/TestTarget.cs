using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TestTarget :Photon.MonoBehaviour,IBuilding,IUnit
{
    private Main main => GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();

    [SerializeField]
    private bool _IsMine;
    [SerializeField, Tooltip("死んだときに発生するポイント")]
    private int deathPoint;
    [SerializeField]
    private GameObject syncTarget;

    public BoolReactiveProperty isMine { get; set; } = new BoolReactiveProperty();

    public float maxUnitHp
    {
        get
        {
            return unitHp.Value;
        }
    }

    public float unitEnergy { get; set; }

    public FloatReactiveProperty unitHp { get; set; } = new FloatReactiveProperty(100f);

    public float unitSpeed { get; set; }

    public byte unitId
    {
        get
        {
            throw new NotImplementedException();
        }

        set
        {
            throw new NotImplementedException();
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
        if (!isMine.Value) main.EnemyCount(-deathPoint);
        photonView.RPC(("DeathSync"), PhotonTargets.Others);
        Destroy(gameObject);
    }

    public void EnterTransform()
    {
        
    }

    public void Move()
    {
        
    }

    public void MyColor(int id)
    {
        
    }

    public void ReleaseTransform()
    {
        
    }

    private void Awake()
    {
        isMine.Value = _IsMine;
    }

    // Use this for initialization
    void Start () {

        //Camera.main.GetComponent<TargetGet>().Enter(gameObject, isMine.Value);

        unitHp
            .Where(x => x <= 0)
            .Subscribe(x => Death())
            .AddTo(gameObject);

        unitHp
            .Subscribe(x => photonView.RPC(("Sync"), PhotonTargets.Others, x))
            .AddTo(gameObject);
    }

    [PunRPC]
    public void Sync(float nowHp)
    {
        if (syncTarget == null) return;
        var sync = syncTarget.GetComponent(typeof(IUnit)) as IUnit;
        if (sync.unitHp.Value < nowHp) return;
        sync.Damage(sync.unitHp.Value - nowHp);
    }

    [PunRPC]
    public void DeathSync()
    {
        if (syncTarget == null) return;
        (syncTarget.GetComponent(typeof(IUnit)) as IUnit).unitHp.Value = 0f;
    }
}
