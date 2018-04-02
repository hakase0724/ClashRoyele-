using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestView : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    private float angle = 90f;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Vector3.Angle(target.transform.position - this.transform.position,transform.forward) <= angle)
        {
            Debug.Log("見えた！" + gameObject.name);
            transform.LookAt(target.transform);
            GetComponent<Rigidbody>().velocity = transform.forward;
        }
	}
}
