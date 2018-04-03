using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticUse;

public class TestView : MonoBehaviour
{
    private GameObject target;
    private float angle = 30f;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (target == null) return;
		if(Vector3.Angle(target.transform.position - this.transform.position,transform.forward) <= angle)
        {
            Debug.Log("見えた！" + gameObject.name);
            //transform.LookAt(target.transform);
            //GetComponent<Rigidbody>().velocity = transform.forward;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent(typeof(IUnit)) as IUnit == null) return;
        target = other.gameObject;
    }
}
