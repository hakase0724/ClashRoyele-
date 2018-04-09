using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootStetas
{
    public enum LRStates
    {
        Left,
        Right
    }

    public LRStates stetas;
    public int number;
    public GameObject rootObject;

    public RootStetas(LRStates stetas,int number,GameObject gameObject)
    {
        this.stetas = stetas;
        this.number = number;
        this.rootObject = gameObject;
    }
}
