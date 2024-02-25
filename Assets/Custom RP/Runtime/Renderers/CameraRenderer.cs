using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    private ScriptableRenderContext context;
    private Camera camera;
    private CommandBuffer buffer;
    private CullingResults cullingResults;

    private static readonly string BUFFRE_NAME = "RenderCemera";
    private static readonly ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
    private static readonly ShaderTagId litShaderTagId = new ShaderTagId("CustomLit");
    private Lighting lighting = new Lighting();

    public void Render(ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing,
        ShadowSettings shadowSettings)
    {
        this.context = context;
        this.camera = camera;
        this.buffer = new CommandBuffer
        {
            name = BUFFRE_NAME
        };

        this.PrepareBuffer();
        this.PrepareForSceneWindow();
        if (!this.Cull(shadowSettings.maxDistance))
        {
            return;
        }
        buffer.BeginSample(SampleName);
        ExecuteBuffer();
        this.lighting.Setup(context, this.cullingResults, shadowSettings);
        buffer.EndSample(SampleName);
        this.Setup();
        this.DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
        this.DrawUnsupportedShaders();
        this.DrawGizmos();
        this.lighting.Cleanup();
        this.Submit();
    }

    private void Setup()
    {
        this.context.SetupCameraProperties(this.camera);
        this.buffer.BeginSample(this.SampleName);

        CameraClearFlags flags = this.camera.clearFlags;
        this.buffer.ClearRenderTarget(
            flags <= CameraClearFlags.Depth,
            flags == CameraClearFlags.Color,
            flags == CameraClearFlags.Color ?
                this.camera.backgroundColor.linear : Color.clear
        );

        this.ExecuteBuffer();
    }

    private void DrawVisibleGeometry(bool useDynamicBatching, bool useGPUInstancing)
    {
        var sortingSettings = new SortingSettings(this.camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };

        var drawingSettings = new DrawingSettings(
            unlitShaderTagId, sortingSettings
        )
        {
            enableDynamicBatching = useDynamicBatching,
            enableInstancing = useGPUInstancing,
            perObjectData = PerObjectData.Lightmaps | PerObjectData.LightProbe |
                            PerObjectData.LightProbeProxyVolume
        };
        drawingSettings.SetShaderPassName(1, litShaderTagId);

        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

        this.context.DrawRenderers(this.cullingResults, ref drawingSettings, ref filteringSettings);

        this.context.DrawSkybox(this.camera);

        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;

        this.context.DrawRenderers(
            this.cullingResults, ref drawingSettings, ref filteringSettings
        );
    }

    private void Submit()
    {
        this.buffer.EndSample(this.SampleName);
        this.ExecuteBuffer();
        this.context.Submit();
    }

    private bool Cull(float maxShadowDistance)
    {
        if (this.camera.TryGetCullingParameters(out ScriptableCullingParameters p))
        {
            p.shadowDistance = Mathf.Min(maxShadowDistance, camera.farClipPlane);;
            this.cullingResults = this.context.Cull(ref p);
            return true;
        }
        return false;
    }

    partial void PrepareBuffer();
    partial void PrepareForSceneWindow();
    partial void DrawGizmo();
    partial void DrawUnsupportedShaders();

    public void ExecuteBuffer()
    {
        this.context.ExecuteCommandBuffer(this.buffer);
        this.buffer.Clear();
    }
}