using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
public class CustomShaderGUI : ShaderGUI
{
    private MaterialEditor editor;
    private Object[] materials;
    private MaterialProperty[] properties;

    private bool showPresets;
    
    enum ShadowMode {
        On, Clip, Dither, Off
    }

    ShadowMode Shadows {
        set {
            if (SetProperty("_Shadows", (float)value)) {
                SetKeyword("_SHADOWS_CLIP", value == ShadowMode.Clip);
                SetKeyword("_SHADOWS_DITHER", value == ShadowMode.Dither);
            }
        }
    }
    public override void OnGUI(
        MaterialEditor materialEditor, MaterialProperty[] properties
    )
    {
        EditorGUI.BeginChangeCheck();
        
        base.OnGUI(materialEditor, properties);
        this.editor = materialEditor;
        this.materials = materialEditor.targets;
        this.properties = properties;

        BakedEmission();
        
        EditorGUILayout.Space();
        this.showPresets = EditorGUILayout.Foldout(this.showPresets, "Presets", true);
        if (this.showPresets)
        {
            this.OpaquePreset();
            this.ClipPreset();
            this.FadePreset();
            if (this.HasPremultiplyAlpha)
            {
                this.TransparentPreset();
            }
        }
        
        if (EditorGUI.EndChangeCheck()) {
            SetShadowCasterPass();
        }
    }
    
    void BakedEmission () {
        EditorGUI.BeginChangeCheck();
        editor.LightmapEmissionProperty();
        if (EditorGUI.EndChangeCheck()) {
            foreach (Material m in editor.targets) {
                m.globalIlluminationFlags &=
                    ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
        }
    }
    
    void SetShadowCasterPass () {
        MaterialProperty shadows = FindProperty("_Shadows", properties, false);
        if (shadows == null || shadows.hasMixedValue) {
            return;
        }
        bool enabled = shadows.floatValue < (float)ShadowMode.Off;
        foreach (Material m in materials) {
            m.SetShaderPassEnabled("ShadowCaster", enabled);
        }
    }
    
    private void OpaquePreset()
    {
        if (this.PresetButton("Opaque"))
        {
            this.Clipping = false;
            this.PremultiplyAlpha = false;
            this.SrcBlend = BlendMode.One;
            this.DstBlend = BlendMode.Zero;
            this.ZWrite = true;
            this.RenderQueue = RenderQueue.Geometry;
        }
    }

    private void ClipPreset()
    {
        if (this.PresetButton("Clip"))
        {
            this.Clipping = true;
            this.PremultiplyAlpha = false;
            this.SrcBlend = BlendMode.One;
            this.DstBlend = BlendMode.Zero;
            this.ZWrite = true;
            this.RenderQueue = RenderQueue.AlphaTest;
        }
    }

    private void FadePreset()
    {
        if (this.PresetButton("Fade"))
        {
            this.Clipping = false;
            this.PremultiplyAlpha = false;
            this.SrcBlend = BlendMode.SrcAlpha;
            this.DstBlend = BlendMode.OneMinusSrcAlpha;
            this.ZWrite = false;
            this.RenderQueue = RenderQueue.Transparent;
        }
    }

    private void TransparentPreset()
    {
        if (this.PresetButton("Transparent"))
        {
            this.Clipping = false;
            this.PremultiplyAlpha = true;
            this.SrcBlend = BlendMode.One;
            this.DstBlend = BlendMode.OneMinusSrcAlpha;
            this.ZWrite = false;
            this.RenderQueue = RenderQueue.Transparent;
        }
    }

    private bool Clipping
    {
        set => this.SetProperty("_Clipping", "_CLIPPING", value);
    }

    private bool PremultiplyAlpha
    {
        set => this.SetProperty("_PremulAlpha", "_PREMULTIPLY_ALPHA", value);
    }

    private BlendMode SrcBlend
    {
        set => this.SetProperty("_SrcBlend", (float)value);
    }

    private BlendMode DstBlend
    {
        set => this.SetProperty("_DstBlend", (float)value);
    }

    private bool ZWrite
    {
        set => this.SetProperty("_ZWrite", value ? 1f : 0f);
    }

    private bool HasPremultiplyAlpha => this.HasProperty("_PremulAlpha");

    private RenderQueue RenderQueue
    {
        set
        {
            foreach (Material m in this.materials.Cast<Material>())
            {
                m.renderQueue = (int)value;
            }
        }
    }

    private bool PresetButton(string name)
    {
        if (GUILayout.Button(name))
        {
            this.editor.RegisterPropertyChangeUndo(name);
            return true;
        }
        return false;
    }

    private bool SetProperty(string name, float value)
    {
        var property = FindProperty(name, this.properties, propertyIsMandatory: false);
        if (property != null)
        {
            property.floatValue = value;
            return true;
        }
        return false;
    }

    private bool HasProperty(string name) =>
        FindProperty(name, this.properties, propertyIsMandatory: false) != null;

    private void SetKeyword(string keyword, bool enabled)
    {
        if (enabled)
        {
            foreach (Material m in this.materials.Cast<Material>())
            {
                m.EnableKeyword(keyword);
            }
        }
        else
        {
            foreach (Material m in this.materials.Cast<Material>())
            {
                m.DisableKeyword(keyword);
            }
        }
    }

    private void SetProperty(string name, string keyword, bool value)
    {
        if (this.SetProperty(name, value ? 1f : 0f))
        {
            this.SetKeyword(keyword, value);
        }
    }
#endif
}