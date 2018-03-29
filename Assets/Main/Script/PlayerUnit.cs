using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using static StaticUse;

public class PlayerUnit : Photon.MonoBehaviour,IUnit
{
    public float UnitHp { get; set; } = 10;
    public float UnitEnergy { get; set; } = 1;
    private GameObject stageScript;
    private int identificationNumber = 0;
    private Rigidbody rb => GetComponent<Rigidbody>();
    private Animator anim => GetComponent<Animator>();
    private List<Transform> buildingTransforms = new List<Transform>();
    [SerializeField]
    private Color[] color = new Color[0];

    private void Start()
    {
        stageScript = GameObject.FindGameObjectWithTag("Main");
        Debug.Log(CalcDistance(transform.position, stageScript.GetComponent<BulidingsManeger>().bulidingsTransform) + "一番近い建物");
    }

    public void Move()
    {
        anim.enabled = true;
        transform.LookAt((CalcDistance(transform.position, stageScript.GetComponent<BulidingsManeger>().bulidingsTransform)));
        //Vector3 vector = new Vector3(0, 0, 1);
        //if (identificationNumber == 0)
        //{
        //    if (Camera.main.GetComponent<CameraRotation>().IsRotated)
        //    {
        //        Debug.Log("回転");
        //        transform.Rotate(new Vector3(0, 180, 0));
        //        vector *= -1;
        //    }
            
        //}
        //else
        //{
        //    if (!Camera.main.GetComponent<CameraRotation>().IsRotated)
        //    {
        //        Debug.Log("回転");
        //        transform.Rotate(new Vector3(0, 180, 0));
        //        vector *= -1;
        //    }
            
        //}
        this.UpdateAsObservable()
            .Subscribe(_ => rb.velocity = transform.forward);
    }

    public void MyColor(int id)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        int colorNumber;
        if (IsSameId(id,PhotonNetwork.player.ID)) colorNumber = 0;
        else colorNumber = 1;
        identificationNumber = colorNumber;
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color[colorNumber];
        }
        const int waitTime = 1;
        Observable.Timer(System.TimeSpan.FromSeconds(waitTime))
            .Subscribe(_ => Move());
    }

    public void Attack(float attack)
    {

    }

    public void Damage(float damage)
    {

    }
}
