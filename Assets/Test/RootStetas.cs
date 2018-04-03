using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootStetas
{
    public enum LRStetas
    {
        Left,
        Right
    }

    public LRStetas stetas;
    public int number;
    public GameObject rootObject;

    public RootStetas(LRStetas stetas,int number,GameObject gameObject)
    {
        this.stetas = stetas;
        this.number = number;
        this.rootObject = gameObject;
    }
}
