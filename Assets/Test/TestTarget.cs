using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TestTarget : MonoBehaviour,IBuilding,IUnit
{
    private Main main => GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();

    [SerializeField]
    private bool _IsMine;
    [SerializeField, Tooltip("死んだときに発生するポイント")]
    private int deathPoint;

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

    public void Attack(float attack, GameObject attackTarget)
    {
        
    }

    public void Damage(float damage)
    {
        Debug.Log("ダメージ発生" + damage);
        unitHp.Value -= damage;
    }

    public void Death()
    {
        if (!isMine.Value) main.EnemyCount(deathPoint);
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

        Camera.main.GetComponent<TargetGet>().Enter(gameObject, isMine.Value);

        unitHp
            .Where(x => x <= 0)
            .Subscribe(x => Death())
            .AddTo(gameObject);
	}

    [PunRPC]
    public void Sync(Vector3 pos, float nowHp)
    {
        if (PhotonNetwork.isMasterClient) return;
        unitHp.Value = nowHp;
    }
}
