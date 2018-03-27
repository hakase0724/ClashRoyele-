using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 自分がいろいろな所で使うメソッドをまとめる
/// </summary>
public static class StaticUse
{
    /// <summary>
    /// 指定シーンが読み込まれていないときに指定シーンを追加読み込みする
    /// </summary>
    /// <param name="sceneName">マスターシーンの名前</param>
    public static void SceneLoad(string sceneName)
    {
        var master = SceneManager.GetSceneByName(sceneName);
        if (master.isLoaded) return;
        else if (!master.isLoaded) SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
}
