using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
	
	private void Start ()
    {
        var rootComponent = transform.root.GetComponent(typeof(IUnit)) as IUnit;
        var MaxHp = rootComponent.maxUnitHp;

        rootComponent.unitHp
            .Do(x=>Debug.Log(rootComponent + ",最大体力" + MaxHp + ",現在体力" + x))
            .Subscribe(x => slider.value = x / MaxHp)
            .AddTo(gameObject);

        this.UpdateAsObservable()
            .Subscribe(_ => transform.rotation = Quaternion.Euler(0, 0, 0))
            .AddTo(gameObject);
    }
}
