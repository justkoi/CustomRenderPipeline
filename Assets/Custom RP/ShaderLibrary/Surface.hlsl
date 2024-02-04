#ifndef CUSTOM_SURFACE_INCLUDED
#define CUSTOM_SURFACE_INCLUDED

struct Surface
{
    float3 normal;
    float3 viewDirection;
	float depth;
    float3 color;
	float3 position;
    float alpha;
    float metallic_A;
    float smoothness_A;
    float metallic_B;
    float smoothness_B;
    float materialMixingRatio;
    float materialMixingCutOff;
	float dither;
};

#endif