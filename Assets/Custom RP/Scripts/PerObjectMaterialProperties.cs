using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
    private static int baseColorId = Shader.PropertyToID("_BaseColor");
    private static int cutoffId = Shader.PropertyToID("_Cutoff");

    private static MaterialPropertyBlock block;

    [SerializeField]
    private Color baseColor = Color.white;

    [SerializeField, Range(0f, 1f)]
    private float cutoff = 0.5f;

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
        this.GetComponent<Renderer>().SetPropertyBlock(block);
    }
}