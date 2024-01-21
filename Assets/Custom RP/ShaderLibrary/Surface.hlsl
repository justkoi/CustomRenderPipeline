#ifndef CUSTOM_SURFACE_INCLUDED
#define CUSTOM_SURFACE_INCLUDED

struct Surface
{
    float3 normal;
    float3 viewDirection;
    float3 color;
    float alpha;
    float metallic_A;
    float smoothness_A;
    float metallic_B;
    float smoothness_B;
    float materialMixingRatio;
    float materialMixingCutOff;
};

#endif