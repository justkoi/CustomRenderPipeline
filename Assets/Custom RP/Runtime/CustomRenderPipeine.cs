using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Custom_RP.Runtime
{
    public sealed class CustomRenderPipeine : RenderPipeline
    {
        private CameraRenderer renderer = new CameraRenderer();

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
                this.renderer.Render(context, cameras[i]);
            }
        }
    }
}
