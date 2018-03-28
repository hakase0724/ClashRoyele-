using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UniRx;
using UniRx.Triggers;

public class PlayerUnit : Photon.MonoBehaviour,IUnit
{
    public float UnitHp { get; set; }
    private Rigidbody rb => GetComponent<Rigidbody>();
    [SerializeField]
    private Color[] color = new Color[0];

    public void Move()
    {
        this.UpdateAsObservable()
            .Subscribe(_ => rb.velocity = Vector3.forward);
    }

    public void MyColor(int id)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color[id - 1];
        }
        Move();
    }

    public void Attack(float attack)
    {

    }

    public void Damage(float damage)
    {

    }
}
