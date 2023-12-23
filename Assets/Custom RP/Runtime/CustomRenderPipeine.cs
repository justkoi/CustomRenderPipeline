using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Custom_RP.Runtime
{
    public sealed class CustomRenderPipeine : RenderPipeline
    {
        private CameraRenderer renderer = new CameraRenderer();
        private readonly bool useDynamicBatching;
        private readonly bool useGPUInstancing;

        public CustomRenderPipeine(
        bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher)
        {
            this.useDynamicBatching = useDynamicBatching;
            this.useGPUInstancing = useGPUInstancing;
            GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
        }

        protected override void Render(
            ScriptableRenderContext context, Camera[] cameras
        )
        { }

        protected override void Render(
            ScriptableRenderContext context, List<Camera> cameras
        )
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                this.renderer.Render(context, cameras[i], this.useDynamicBatching, this.useGPUInstancing);
            }
        }
    }
}
