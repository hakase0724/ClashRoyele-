using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestZone : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        Debug.Log(transform.root.gameObject);
        
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        var parentUnit = transform.root.gameObject.GetComponent<PlayerUnit>();
        parentUnit.TriggerOn();
    }
}
