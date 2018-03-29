using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuilding
{
    //自分の場所を他のオブジェクトに公開
    Vector3 myPos { get; set; }
    void EnterTransform();
}
