using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;


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
        else if (!master.isLoaded) SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 通信で受け取ったIDと自分のIDを比較し結果を返す
    /// </summary>
    /// <param name="receivedId">通信で受け取ったID</param>
    /// <param name="myId">自分のID</param>
    /// <returns></returns>
    public static bool IsSameId(int receivedId,int myId)
    {
        bool result = false;
        if (receivedId == myId) result = true;
        else result = false;
        return result;
    }
}
