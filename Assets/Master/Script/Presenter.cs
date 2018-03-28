using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
/// <summary>
/// MV(R)PのPresenter名前入力を受けて接続命令を走らせる
/// </summary>
public class Presenter : MonoBehaviour
{
    [SerializeField]
    private InputField nameInput;
    [SerializeField]
    private Model model;
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
