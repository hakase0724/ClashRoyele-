using UnityEngine;
using System.Collections;

public class OnClickInstantiate : MonoBehaviour
{
    public GameObject Prefab;
    public int InstantiateType;
    private string[] InstantiateTypeNames = {"Mine", "Scene"};

    public bool showGui;

    void OnClick()
    {
        if (!PhotonNetwork.inRoom)
        {
            // only use PhotonNetwork.Instantiate while in a room.
            return;
        }

        switch (InstantiateType)
        {
            //プレイヤーが抜けたら消える
            case 0:
                PhotonNetwork.Instantiate(Prefab.name, InputToEvent.inputHitPos, Quaternion.identity, 0);
                break;
            //プレイヤーが抜けても消えない
            case 1:
                PhotonNetwork.InstantiateSceneObject(Prefab.name, InputToEvent.inputHitPos, Quaternion.identity, 0, null);
                break;
        }
    }

    void OnGUI()
    {
        if (showGui)
        {
            GUILayout.BeginArea(new Rect(Screen.width - 180, 0, 180, 50));
            InstantiateType = GUILayout.Toolbar(InstantiateType, InstantiateTypeNames);
            GUILayout.EndArea();
        }
    }


}
