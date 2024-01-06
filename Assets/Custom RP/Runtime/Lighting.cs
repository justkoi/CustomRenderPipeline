using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting
{
    private const int maxDirLightCount = 4;

    private static readonly int
        dirLightCountId = Shader.PropertyToID("_DirectionalLightCount"),
        dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors"),
        dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");

    private static readonly Vector4[]
        dirLightColors = new Vector4[maxDirLightCount],
        dirLightDirections = new Vector4[maxDirLightCount];

    private const string bufferName = "Lighting";
    private CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };
    private CullingResults cullingResults;

    public void Setup(ScriptableRenderContext context, CullingResults cullingResults)
    {
        this.cullingResults = cullingResults;
        this.buffer.BeginSample(bufferName);
        this.SetupLights();
        this.buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(this.buffer);
        this.buffer.Clear();
    }

    private void SetupDirectionalLight(int index, ref VisibleLight visibleLight)
    {
        dirLightColors[index] = visibleLight.finalColor;
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
    }

    private void SetupLights()
    {
        NativeArray<VisibleLight> visibleLights = this.cullingResults.visibleLights;
        int dirLightCount = 0;
        for (int i = 0; i < visibleLights.Length; i++)
        {
            VisibleLight visibleLight = visibleLights[i];
            if (visibleLight.lightType == LightType.Directional)
            {
                this.SetupDirectionalLight(dirLightCount++, ref visibleLight);
                if (dirLightCount >= maxDirLightCount)
                {
                    break;
                }
            }
        }

        this.buffer.SetGlobalInt(dirLightCountId, visibleLights.Length);
        this.buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
        this.buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);
    }
}