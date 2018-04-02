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
    protected Main main => GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();
    private BulidingsManeger maneger => GameObject.FindGameObjectWithTag("Main").GetComponent<BulidingsManeger>();
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
        //maneger.EnterList(this.gameObject, this, isMine.Value);
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
        renderer.material.color = color[colorNumber];
    }

    public virtual void ReleaseTransform()
    {
        //maneger.ReleaseList(this.gameObject);
        if (!isMine.Value) main.EnemyCount(-1);
    }

    public void NextSet(GameObject next)
    {
        throw new NotImplementedException();
    }
}
