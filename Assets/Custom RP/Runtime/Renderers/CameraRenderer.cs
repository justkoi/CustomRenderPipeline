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

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        this.context = context;
        this.camera = camera;
        this.buffer = new CommandBuffer
        {
            name = BUFFRE_NAME
        };

        this.PrepareBuffer();
        this.PrepareForSceneWindow();
        if (!this.Cull())
        {
            return;
        }

        this.Setup();

        this.DrawVisibleGeometry();
        this.DrawUnsupportedShaders();
        this.DrawGizmos();

        this.Submit();
    }

    private void Setup()
    {
        this.context.SetupCameraProperties(this.camera);
        CameraClearFlags flags = this.camera.clearFlags;
        this.buffer.BeginSample(this.SampleName);
        this.buffer.ClearRenderTarget(
            flags <= CameraClearFlags.Depth,
            flags == CameraClearFlags.Color,
            flags == CameraClearFlags.Color ?
                this.camera.backgroundColor.linear : Color.clear
        );
        this.ExecuteBuffer();
    }

    private void DrawVisibleGeometry()
    {
        var sortingSettings = new SortingSettings(this.camera)
        {
            criteria = SortingCriteria.CommonOpaque
        };

        var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);

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

    private bool Cull()
    {
        if (this.camera.TryGetCullingParameters(out ScriptableCullingParameters p))
        {
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