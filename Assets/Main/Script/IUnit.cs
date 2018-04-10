using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// ユニットが持つべきもの
/// </summary>
public interface IUnit
{
    BoolReactiveProperty isMine { get; set; }
    //ユニットの体力
    FloatReactiveProperty unitHp { get;set;}
    //それぞれのユニットを識別する番号　byte型なのはPUNのRaiseEventで使いやすくするため
    byte unitId { get; set; }
    //ユニットの移動速度
    float unitSpeed { get; set; }
    //ユニット生成時のコスト
    float unitEnergy { get; set; }
    //ユニットの最大体力を保存
    float maxUnitHp { get; }
    /// <summary>
    /// 自機の色を変える
    /// </summary>
    /// <param name="id">識別用のID</param>
    void MyColor(int id);
    /// <summary>
    /// 自機の移動
    /// </summary>
    void Move();
    /// <summary>
    /// 自機の攻撃
    /// </summary>
    /// <param name="attack">攻撃力</param>
    void Attack(float attack,GameObject attackTarget);
    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage">受けるダメージ</param>
    void Damage(float damage);
    /// <summary>
    /// 体力が尽きたときの処理
    /// </summary>
    void Death();
}
