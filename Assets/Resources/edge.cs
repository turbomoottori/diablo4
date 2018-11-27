using UnityEngine;
using System.Collections;

[ExecuteInEditMode] //jotta voi editorissa leikkiä
public class edge : MonoBehaviour
{
    public float threshold;
	public float edgeSize;
    public float dimmer;
    public Color edgeColor;
    public Material material;

    // Creates a private material used to the effect
    void Awake()
    {
        //material = new Material(Shader.Find("Hidden/EdgeDetectionShader"));
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (threshold == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        //lähetetään arvot shaderille ja lopuksi blitataan kuva ruudulle
        material.SetFloat("_Threshold", threshold);
        material.SetFloat("_Dimmer", dimmer);
        material.SetColor("_EdgeColor", edgeColor);
		material.SetFloat("_Edgesize", edgeSize);
        Graphics.Blit(source, destination, material);
    }
}