using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// 橋
/// </summary>
public class Bridge : MonoBehaviour, IBuilding,IUnit
{
    public FloatReactiveProperty UnitHp { get; set; } = new FloatReactiveProperty(10);
    public float UnitEnergy { get; set; }
    private BulidingsManeger maneger => GameObject.FindGameObjectWithTag("Main").GetComponent<BulidingsManeger>();

    private void Start()
    {
        EnterTransform();
        UnitHp
            .Where(x => x <= 0)
            .Subscribe(_ => Death())
            .AddTo(gameObject);
    }

    public void EnterTransform()
    {
        maneger.EnterList(this.transform, this);
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
        maneger.ReleaseList(this.transform);
    }
}
