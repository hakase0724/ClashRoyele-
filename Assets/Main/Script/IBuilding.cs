using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建物がもつべきもの
/// </summary>
public interface IBuilding
{
    //自身を管理リストに登録させる
    void EnterTransform();
    void ReleaseTransform();
}
