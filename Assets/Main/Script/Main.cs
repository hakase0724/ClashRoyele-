using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticUse;

/// <summary>
/// メインシーンが行う処理
/// </summary>
public class Main : MonoBehaviour
{

	private void Start ()
    {
        const string masterName = "Master";
        SceneLoad(masterName);
	}
	
	
}
