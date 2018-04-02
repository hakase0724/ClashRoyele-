﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// ユニットが持つべきもの
/// </summary>
public interface IUnit
{
    BoolReactiveProperty isMine { get; set; }
    BoolReactiveProperty isAlive { get; set; }
    //ユニットの体力
    FloatReactiveProperty UnitHp { get;set;}
    //ユニット生成時のコスト
    float UnitEnergy { get; set; }
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
    void Death();
    void NextSet(GameObject next);
}
