using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TestTarget : MonoBehaviour,IBuilding,IUnit
{
    [SerializeField]
    private bool _IsMine;
    public BoolReactiveProperty isAlive
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

    public BoolReactiveProperty isMine
    {
        get
        {
            return isMine;
        }

        set
        {
            isMine.Value = _IsMine;
        }
    }

    public float unitEnergy
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

    public FloatReactiveProperty unitHp { get; set; } = new FloatReactiveProperty(100f);

    public float unitSpeed
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
        throw new NotImplementedException();
    }

    public void Damage(float damage)
    {
        Debug.Log("ダメージ発生" + damage);
        unitHp.Value -= damage;
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    public void EnterTransform()
    {
        throw new NotImplementedException();
    }

    public void Move()
    {
        throw new NotImplementedException();
    }

    public void MyColor(int id)
    {
        throw new NotImplementedException();
    }

    public void ReleaseTransform()
    {
        throw new NotImplementedException();
    }

    // Use this for initialization
    void Start () {
        isMine = new BoolReactiveProperty(_IsMine);


        unitHp
            .Where(x => x <= 0)
            .Subscribe(x => Death());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
