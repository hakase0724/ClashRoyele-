﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestZone : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        var parentUnit = transform.root.gameObject.GetComponent<NavTest>();
        parentUnit.TriggerOn(other.gameObject);
    }
}