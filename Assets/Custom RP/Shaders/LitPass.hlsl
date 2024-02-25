#ifndef CUSTOM_UNLIT_PASS_INCLUDED
#define CUSTOM_UNLIT_PASS_INCLUDED

#if defined(LIGHTMAP_ON)
    #define GI_ATTRIBUTE_DATA float2 lightMapUV : TEXCOORD1;
    #define GI_VARYINGS_DATA float2 lightMapUV : VAR_LIGHT_MAP_UV;
    #define TRANSFER_GI_DATA(input, output) \
    output.lightMapUV = input.lightMapUV * \
    unity_LightmapST.xy + unity_LightmapST.zw;
    #define GI_FRAGMENT_DATA(input) input.lightMapUV
#else
    #define GI_ATTRIBUTE_DATA
    #define GI_VARYINGS_DATA
    #define TRANSFER_GI_DATA(input, output)
    #define GI_FRAGMENT_DATA(input) 0.0
#endif

#include "../ShaderLibrary/Common.hlsl"
#include "../ShaderLibrary/Surface.hlsl"
#include "../ShaderLibrary/Shadows.hlsl"
#include "../ShaderLibrary/Light.hlsl"
#include "../ShaderLibrary/BRDF.hlsl"
#include "../ShaderLibrary/GI.hlsl"
#include "../ShaderLibrary/Lighting.hlsl"

struct Attributes
{
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 baseUV : TEXCOORD0;
	GI_ATTRIBUTE_DATA
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float3 positionWS : VAR_POSITION;
    float3 normalWS : VAR_NORMAL;
    float2 baseUV : VAR_BASE_UV;
	GI_ATTRIBUTE_DATA
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

Varyings LitPassVertex(Attributes input)
{
    Varyings output;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
	TRANSFER_GI_DATA(input, output);
    output.positionWS = TransformObjectToWorld(input.positionOS);
    output.positionCS = TransformWorldToHClip(output.positionWS);
    output.normalWS = TransformObjectToWorldNormal(input.normalOS);
	
    float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseMap_ST);
    output.baseUV = input.baseUV * baseST.xy + baseST.zw;
    return output;
}

float4 LitPassFragment(Varyings input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    
    float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.baseUV);
    float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
    float4 base = baseMap * baseColor;
#if defined(_CLIPPING)
	clip(base.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Cutoff));
#endif
    //base.rgb = normalize(input.normalWS);
    
    Surface surface;
	surface.position = input.positionWS;
    surface.normal = normalize(input.normalWS);
    surface.viewDirection = normalize(_WorldSpaceCameraPos - input.positionWS);
	surface.depth = -TransformWorldToView(input.positionWS).z;
    surface.color = base.rgb;
    surface.alpha = base.a;
    surface.metallic_A = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Metallic_A);
    surface.metallic_B = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Metallic_B);
    surface.smoothness_A = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Smoothness_A);
    surface.smoothness_B = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Smoothness_B);
    surface.dither = InterleavedGradientNoise(input.positionCS.xy, 0);
    surface.materialMixingRatio = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _MaterialMixingRatio);
    surface.materialMixingCutOff = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _MaterialMixingCutOff);
#if defined(_PREMULTIPLY_ALPHA)
		BRDF brdf = GetBRDF(surface, true);
#else
    BRDF brdf = GetBRDF(surface);
	#endif
    GI gi = GetGI(GI_FRAGMENT_DATA(input), surface);
    float3 color = GetLighting(surface, brdf, gi);
    return float4(color, surface.alpha);
}
#endif