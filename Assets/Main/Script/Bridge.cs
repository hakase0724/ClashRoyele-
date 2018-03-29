using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using static StaticUse;

/// <summary>
/// 橋
/// </summary>
public class Bridge : MonoBehaviour, IBuilding,IUnit
{
    public FloatReactiveProperty UnitHp { get; set; } = new FloatReactiveProperty(10);
    public float UnitEnergy { get; set; } = 0;
    private Main main => GameObject.FindGameObjectWithTag("Main").GetComponent<Main>();
    private BulidingsManeger maneger => GameObject.FindGameObjectWithTag("Main").GetComponent<BulidingsManeger>();
    //識別番号 0=自分.1=自分以外
    private int identificationNumber = 0;
    [SerializeField]
    private Color[] color = new Color[0];

    private void Start()
    {
        EnterTransform();
    }

    public void EnterTransform()
    {
        maneger.EnterList(this.transform, this,0);
        maneger.EnterList(this.transform, this,1);
    }

    public void MyColor(int id)
    {
       
    }

    public void Move()
    {
        
    }

    public void Attack(float attack)
    {
        
    }

    public void Damage(float damage)
    {
        UnitHp.Value -= damage;
    }

    public void Death()
    {
        ReleaseTransform();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    public void ReleaseTransform()
    {
        if (identificationNumber != 0) main.EnemyCount(-1);
    }
}
