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
    //ユニットの動き
    void Move();
    //ユニットの攻撃
    void Attack(float attack);
    //ユニットがダメージを受ける
    void Damage(float damage);
}
