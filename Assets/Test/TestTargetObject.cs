using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTargetObject : MonoBehaviour
{
    [SerializeField]
    private RootStetas.LRStates state;
    [SerializeField]
    private int rootNumber;
    

	void Start ()
    {
        RootStetas rootStates = new RootStetas(state, rootNumber, gameObject);
        GameObject.FindGameObjectWithTag("Main").GetComponent<TestManeger>().InsertRoot(rootStates);
	}
}
