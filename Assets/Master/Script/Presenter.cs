using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 名前入力を受けてネットワーク接続命令を走らせる
/// </summary>
public class Presenter : MonoBehaviour
{
    [SerializeField]
    private InputField nameInput;
    [SerializeField]
    private Master master;

    private void Start()
    {
        nameInput.ActivateInputField();
    }

    public void EndInput()
    {
        PlayerData data = new PlayerData();
        string name = nameInput.text;
        data.playerName = name;
        master.ConnectNetWork(data);
    }
}
