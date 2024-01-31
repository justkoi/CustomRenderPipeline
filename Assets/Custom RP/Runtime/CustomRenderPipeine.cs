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
        private ShadowSettings shadowSettings;
        public CustomRenderPipeine(
        bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher, ShadowSettings shadowSettings)
        {
            this.useDynamicBatching = useDynamicBatching;
            this.useGPUInstancing = useGPUInstancing;
            this.shadowSettings = shadowSettings;
            GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
            GraphicsSettings.lightsUseLinearIntensity = true;
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
                this.renderer.Render(context, cameras[i], this.useDynamicBatching, this.useGPUInstancing,
                    shadowSettings);
            }
        }
    }
}
