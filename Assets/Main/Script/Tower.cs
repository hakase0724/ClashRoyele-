using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using static StaticUse;

public class Tower : MonoBehaviour, IBuilding, IUnit
{
    public BoolReactiveProperty isMine { get; set; } = new BoolReactiveProperty();
    public BoolReactiveProperty isAlive { get; set; } = new BoolReactiveProperty(true);
    public float UnitEnergy { get; set; } = 0;
    public FloatReactiveProperty UnitHp { get; set; } = new FloatReactiveProperty(10);
    private Main main => GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();
    private BulidingsManeger maneger => GameObject.FindGameObjectWithTag("Main").GetComponent<BulidingsManeger>();
    //識別番号 0=自分.1=自分以外
    private int identificationNumber = 0;
    [SerializeField]
    private Color[] color = new Color[0];

    private void Start()
    {
        if (!isMine.Value) main.EnemyCount(1);
        EnterTransform();
        UnitHp
            .Where(x => x <= 0)
            .Subscribe(_ => Death())
            .AddTo(gameObject);
    }

    public void Attack(float attack,GameObject attackTarget)
    {
        
    }

    public void Damage(float damage)
    {
        UnitHp.Value -= damage;
    }

    public void Death()
    {
        ReleaseTransform();
        isAlive.Value = false;
        gameObject.SetActive(false);
    }

    public void EnterTransform()
    {
        maneger.EnterList(this.gameObject, this, identificationNumber);
    }

    public void Move()
    {
        
    }

    public void MyColor(int id)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        int colorNumber;
        //生成者が自分か相手か判別
        if (IsSameId(id, PhotonNetwork.player.ID))
        {
            colorNumber = 0;
            isMine.Value = true;
        }
        else
        {
            colorNumber = 1;
            isMine.Value = false;
        }
        //判別結果に応じて識別番号を更新
        identificationNumber = colorNumber;
        renderer.material.color = color[colorNumber];
    }

    public void ReleaseTransform()
    {
        maneger.ReleaseList(this.gameObject);
        if (!isMine.Value) main.EnemyCount(-1);
    }
}
