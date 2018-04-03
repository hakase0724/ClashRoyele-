using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPoint : MonoBehaviour
{
    [SerializeField]
    private GameObject next;
    [SerializeField]
    private GameObject prev;
	// Use this for initialization
	void Start ()
    {
        GameObject.FindGameObjectWithTag("Main").GetComponent<BulidingsManeger>().Enter(this.gameObject);
        if (Camera.main.GetComponent<CameraRotation>().IsRotated) transform.position = new Vector3(transform.position.x, transform.position.y, -transform.position.z);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("巡回点エリア：" + gameObject.name + "," + transform.position + "到着");
        var unit = other.gameObject.GetComponent(typeof(IUnit)) as IUnit;
        //if (unit.isMine.Value) unit.NextSet(next);
        //else unit.NextSet(prev);
    }
}
