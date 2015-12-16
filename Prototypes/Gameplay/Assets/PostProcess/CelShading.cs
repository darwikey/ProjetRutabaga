using UnityEngine;
using System.Collections;

public class CelShading : MonoBehaviour {

    Material mat;

    // Use this for initialization
    void Start () {
        mat = new Material(Shader.Find("Post/CelShadingEffect"));
	}

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, mat);
    }
}
