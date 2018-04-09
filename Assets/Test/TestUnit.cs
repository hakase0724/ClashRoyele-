using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TestUnit : MonoBehaviour,IUnit
{
    public BoolReactiveProperty isMine { get; set; }
    public float maxUnitHp { get; }
    public float unitEnergy { get; set; }
    public FloatReactiveProperty unitHp { get; set; }
    public float unitSpeed { get; set; }

    public void Attack(float attack, GameObject attackTarget)
    {

    }

    public void Damage(float damage)
    {
        unitHp.Value -= damage;
        Debug.Log("ダメージを受けた" + unitHp.Value);
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    public void Move()
    {
        
    }

    public void MyColor(int id)
    {
        
    }

    
    void Start ()
    {
		
	}
	
	
	void Update ()
    {
		
	}
}
