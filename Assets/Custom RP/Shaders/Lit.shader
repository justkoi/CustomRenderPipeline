Shader "Custom RP/Lit"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (0.5, 0.5, 0.5,1)
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
		[Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		[Toggle(_CLIPPING)] _Clipping ("Alpha Clipping", Float) = 0
        _MetallicA ("MetallicA", Range(0, 1)) = 0
		_SmoothnessA ("SmoothnessA", Range(0, 1)) = 0.5
        _MetallicB ("MetallicB", Range(0, 1)) = 0
		_SmoothnessB ("SmoothnessB", Range(0, 1)) = 0.5
        _MaterialMixingRatio ("Material Mixing Ratio", Range(0.0, 1.0)) = 0.5
        _MaterialMixingCutOff ("Material Mixing CutOff", Range(0.0, 1.0)) = 0.5
        [Toggle(_PREMULTIPLY_ALPHA)] _PremulAlpha ("Premultiply Alpha", Float) = 0
    }
    SubShader
    {
        Pass
        {
            Tags {
				"LightMode" = "CustomLit"
			}

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]

            HLSLPROGRAM
            #pragma target 3.5
            #pragma shader_feature _CLIPPING
			#pragma shader_feature _PREMULTIPLY_ALPHA
            #pragma multi_compile_instancing
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #include "LitPass.hlsl"
            ENDHLSL
        }
    }

    CustomEditor "CustomShaderGUI"
}