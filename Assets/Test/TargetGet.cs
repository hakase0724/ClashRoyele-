using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TargetGet : MonoBehaviour
{
    [SerializeField]
    private GameObject[] mineArray = new GameObject[3];
    [SerializeField]
    private GameObject[] enemyArray = new GameObject[3];
    public Vector3[] mArray {get{ return minePos; }}
    public Vector3[] eArray { get { return enemyPos; }}

    private Vector3[] minePos = new Vector3[3];
    private Vector3[] enemyPos = new Vector3[3];

    private void Awake()
    {
        minePos = PickPos(mineArray);
        enemyPos = PickPos(enemyArray);
    }

    private Vector3[] PickPos(GameObject[] game)
    {
        Vector3[] posArray = new Vector3[game.Length];
        for (int i = 0; i < game.Length; i++)
        {
            posArray[i] = game[i].transform.position;
        }
        return posArray;
    }
  
}
