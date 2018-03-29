using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour, IBuilding
{
    public Vector3 myPos
    {
        get
        {
            return myPos;
        }

        set
        {
            myPos = this.transform.position;
        }
    }

    private void Start()
    {
        EnterTransform();
    }

    public void EnterTransform()
    {
        GameObject.FindGameObjectWithTag("Main").GetComponent<BulidingsManeger>().EnterList(this.transform, this);
    }
}
