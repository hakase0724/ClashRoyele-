using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ModelColorChange : MonoBehaviour
{
    [SerializeField]
    private Color[] color = new Color[0];
    [SerializeField]
    private int colorNumber;

    private void Update()
    {
        ColorChange(color[colorNumber]);
    }

    public void ColorChange(Color color)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }
}
