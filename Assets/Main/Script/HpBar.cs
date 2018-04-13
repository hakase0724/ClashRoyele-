using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Image bar;
    [SerializeField]
    private Color[] colors = new Color[2];
	
	private void Start ()
    {
        //親のIUnitを取得
        var rootComponent = transform.root.GetComponent(typeof(IUnit)) as IUnit;
        if (rootComponent == null) Destroy(gameObject);
        var MaxHp = rootComponent.maxUnitHp;
        //IUnitのisMineの値によってバーの色を変える
        if (rootComponent.isMine.Value) bar.color = colors[0];
        else bar.color = colors[1];

        //体力が変わったとき現在の体力比率をバーで表示する
        rootComponent.unitHp
            .Subscribe(x => slider.value = x / MaxHp)
            .AddTo(gameObject);

        //体力が尽きたときバーを消す
        rootComponent.unitHp
            .Where(x => x <= 0)
            .Subscribe(_ => Destroy(gameObject))
            .AddTo(gameObject);

        //常に一定のrotation保つ
        this.UpdateAsObservable()
            .Subscribe(_ => transform.rotation = Quaternion.Euler(0, 0, 0))
            .AddTo(gameObject);
    }
}
