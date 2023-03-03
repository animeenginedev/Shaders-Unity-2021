using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class PostProcessShader : MonoBehaviour
{
    public Material material;

    [Header("Generic Parameters")]
    public float intensity;
    public float invertStrength = 0.0f;


    //Depth Outline Parameters
    [Header("Outline Parameters")]
    public Color outlineColour;
    public float outlineSize;
    public float outlineThreshold = 0.01f;
    public bool enable_outline_input = false;

    private void Awake()
    {
        var cam = GetComponent<Camera>();
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.DepthNormals;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (intensity == 0 || material == null)
        {
            Graphics.Blit(source, destination);
            return;
        }
        material.SetFloat("_Intensity", intensity);
        material.SetFloat("_InvertStrength", invertStrength);
        if (enable_outline_input)
        {
            material.SetColor("_OutlineColour", outlineColour);
            material.SetFloat("_OutlineSize", outlineSize);
            material.SetFloat("_OutlineThreshold", outlineThreshold);
        }

        Graphics.Blit(source, destination, material);
    }
}