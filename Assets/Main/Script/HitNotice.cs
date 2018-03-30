using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitNotice : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        transform.root.gameObject.GetComponent<PlayerUnit>().TriggerOn(other.gameObject);
    }
}
