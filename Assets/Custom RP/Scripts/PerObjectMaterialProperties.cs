using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
    private static int baseColorId = Shader.PropertyToID("_BaseColor");
    private static int cutoffId = Shader.PropertyToID("_Cutoff");
    private static int metallicAId = Shader.PropertyToID("_Metallic_A");
    private static int smoothnessAId = Shader.PropertyToID("_Smoothness_A");
    private static int metallicBId = Shader.PropertyToID("_Metallic_B");
    private static int smoothnessBId = Shader.PropertyToID("_Smoothness_B");
    private static int materialMixingRatioId = Shader.PropertyToID("_MaterialMixingRatio");
    private static int materialMixingCutOffId = Shader.PropertyToID("_MaterialMixingCutOff");

    private static MaterialPropertyBlock block;

    [SerializeField]
    private Color baseColor = Color.white;

    [SerializeField, Range(0f, 1f)]
    private float cutoff = 0.5f, metallicA = 0f, smoothnessA = 0.5f, metallicB = 0f, smoothnessB = 0.5f, materialMixingRatio = 1.0f, materialMixingCutOff = 1.0f;

    private void Awake()
    {
        this.OnValidate();
    }

    private void OnValidate()
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }
        block.SetColor(baseColorId, this.baseColor);
        block.SetFloat(cutoffId, this.cutoff);
        block.SetFloat(metallicAId, this.metallicA);
        block.SetFloat(smoothnessAId, this.smoothnessA);
        block.SetFloat(metallicBId, this.metallicB);
        block.SetFloat(smoothnessBId, this.smoothnessB);
        block.SetFloat(materialMixingRatioId, this.materialMixingRatio);
        block.SetFloat(materialMixingCutOffId, this.materialMixingCutOff);
        this.GetComponent<Renderer>().SetPropertyBlock(block);
    }
}