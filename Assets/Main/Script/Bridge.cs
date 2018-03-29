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
    private BulidingsManeger maneger => GameObject.FindGameObjectWithTag("Main").GetComponent<BulidingsManeger>();
    [SerializeField]
    private Color[] color = new Color[0];

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
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        int colorNumber;
        //生成者が自分か相手か判別
        if (IsSameId(id, PhotonNetwork.player.ID)) colorNumber = 0;
        else colorNumber = 1;
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color[colorNumber];
        }
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
