using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TestManeger:MonoBehaviour
{
    public List<GameObject> objects { get; private set; } = new List<GameObject>();

    private void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.C))
            .Subscribe(_ =>
            {
                foreach(var o in objects)
                {
                    Debug.Log(o);
                }
            });
    }
	
    public void Enter(GameObject enter)
    {
        objects.Add(enter);
    }
}
