using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// 自分がいろいろな所で使うメソッドをまとめる
/// </summary>
public static class StaticUse
{
    /// <summary>
    /// 指定シーンが読み込まれていないときに指定シーンをロードする
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

    /// <summary>
    /// 2点間の距離を計算する
    /// </summary>
    /// <param name="myPos"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public static float CalcDistance(Vector3 myPos, Vector3 targetPos)
    {
        return (myPos - targetPos).sqrMagnitude;
    }
    /// <summary>
    /// 視界に入っているか確認する
    /// </summary>
    /// <param name="otherPos">相手の座標</param>
    /// <param name="myPos">自分の座標</param>
    /// <param name="viewVector">視界の方向</param>
    /// <param name="viewAngle">視界の角度</param>
    /// <returns></returns>
    public static bool IsLooked(Vector3 otherPos,Vector3 myPos ,Vector3 viewVector,float viewAngle)
    {
        if (Vector3.Angle(otherPos - myPos, viewVector) <= viewAngle) return true;
        else return false;
    }
}
