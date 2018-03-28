using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ユニットが持つべきもの
/// </summary>
public interface IUnit
{
    //ユニットの体力
    float UnitHp { get;set;}
    //自分の色を変える
    void MyColor(int id);
    //ユニットの動き
    void Move();
    //ユニットの攻撃
    void Attack(float attack);
    //ユニットがダメージを受ける
    void Damage(float damage);
}
