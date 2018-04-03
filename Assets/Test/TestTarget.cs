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

    public float UnitEnergy
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

    public FloatReactiveProperty UnitHp
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
        throw new NotImplementedException();
    }

    public void Death()
    {
        throw new NotImplementedException();
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
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
