#include "Functions.hlsl"
#include "SDFFunctions.hlsl"
#ifndef SSCS_CUSTOM_INCLUDED
#define SSCS_CUSTOM_INCLUDED

#pragma multi_compile _MAIN_LIGHT_SHADOWS
#pragma multi_compile _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _SHADOWS_SOFT

#define EPSILON 0.0001
#define MAX_LIGHTS 16 // Define the maximum number of lights

float Pow2OneMinus(float a)
{
    a *= a;
    return (1.0 - a);
}

float smoothstep(float edge0, float edge1, float x)
{
    x = saturate((x - edge0) / (edge1 - edge0));
    return x * x * (3 - 2 * x);
}
float random (float2 uv)
{
    return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
}
float perlinNoise(float2 uv)
{
	float2 p = floor(uv);
	float2 f = frac(uv);
	f = f * f * (3.0 - 2.0 * f);
	float res = lerp(lerp(dot(random(p + float2(0.0, 0.0)), f - float2(0.0, 0.0)),
                        dot(random(p + float2(1.0, 0.0)), f - float2(1.0, 0.0)), f.x),
                    lerp(dot(random(p + float2(0.0, 1.0)), f - float2(0.0, 1.0)),
                        dot(random(p + float2(1.0, 1.0)), f - float2(1.0, 1.0)), f.x), f.y);
	return res;
}
#endif


float marbleTexture(float2 uv,float noiseStrength,float noiseFrequency,float numberOfLayers) {
    float noise = perlinNoise(uv * noiseFrequency/*noise frequency*/);
    float sinePattern = sin(uv.y * numberOfLayers/*number of layers*/ + noise *  noiseStrength);
    return sinePattern;
}
// Function to get lighting with multiple lights
void Marbel_float(float2 UV,float Scale, float NoiseStrength,float NoiseFrequency,float NumberOfLayers,out float3 result)
{

    float marble = marbleTexture(UV*Scale,NoiseStrength,NoiseFrequency,NumberOfLayers);

    
	result = float3(marble,marble,marble);

}
// Function to get lighting with multiple lights
void Noise_float(float2 UV,float Scale,out float3 result)
{
	
    float noise = perlinNoise(UV * Scale);
    
	result = float3(noise,noise,noise);

}

// Function to get lighting with multiple lights
void Wood_float(float2 UV,float Scale,out float3 result)
{
	
	float wood = woodTexture(UV * Scale - (Scale * 0.5f - float2(0.5f, 0.5f)));
  
    
	result = float3(wood,wood,wood);

}

void Array_float(float2 UV, UnitySamplerState stex,UnityTexture2D text,float2 num,out float3 result)
{
    int nx = ceil(num.x);
	int ny = ceil(num.y);
	result = SAMPLE_TEXTURE2D(text, stex, float2(UV.x * nx,UV.y * ny));

}

void Voronoi_float(float2 UV,float Scale,out float3 result)
{
    result =  float3(voronoi(UV*Scale),voronoi(UV*Scale),voronoi(UV*Scale));
}

void Fbm_float(float2 UV,float Scale,out float3 result)
{
    result =  float3(fbm(UV*Scale),fbm(UV*Scale),fbm(UV*Scale));
}
void IqNoise_float(float2 UV,float Scale,float u, float v,out float3 result)
{

	float noiseVal =clamp(iqnoise(UV * Scale,u,v),0,1) ;
	result = float3(noiseVal, noiseVal, noiseVal);
}
void Vfbm_float(float2 UV,float Scale,out float3 result)
{
    float3 uv3 =rand2dTo3d(UV*Scale);
	float noiseval = noise(UV * Scale);
	result = float3(noiseval, noiseval, noiseval);
}
void Swirl_float(float2 UV, float Scale,float2 Center, float Angle, out float3 result)
{
	float value =swirl(UV * Scale - (Scale * 0.5f - float2(0.5f, 0.5f)),Center,Angle).x;
	result = float3(value,value,value);
}
void CurlNoise_float(float2 UV, float Scale, out float3 result)
{	
	float noise = curlNoise(UV * Scale).x;
	result = float3(noise, noise, noise);
}
void RadialGradient_float(float2 UV, float Scale, float2 Center, float Raduis,float Smooth, out float3 result)
{
	float gradient = radialGradient(UV, Center, abs(Raduis),-Smooth);
	result = float3(gradient, gradient, gradient);
}
void WorleyNoise_float(float2 UV, float Scale, out float3 result)
{
	float noise = worley(UV*Scale);
	result = float3(noise, noise, noise);
}
void TurbulanceNoise_float(float2 UV, float Scale, out float3 result)
{
	float noise = turbulence(UV * Scale);
	result = float3(noise, noise, noise);
}
void BrickTexture_float(float2 UV,float2 brickSize , float mortarThickness, float3 brickColor , float3 mortarColor, float colorVariation , out float3 result) // Amount of color variation in bricks)
{

	result = BrickTexture(UV, brickSize*0.01f, mortarThickness,  brickColor, mortarColor,  colorVariation);

}
void Rectangle_float(float2 UV, float2 Size,float4 Corners, float rounded,float invert, out float3 result)
{
	float rect = sdRoundedBox(UV+float2(-0.5f,-0.5f), Size,Corners);-rounded;
	rect = lerp(rect, rect < 0.0 ? 1.0 : 0.0, invert);
	
	result = float3(rect, rect, rect);
}
void Pentagon_float(float2 UV, float Size, float rounded,float invert, out float3 result)
{
	float value = sdPentagon(UV+float2(-0.5f,-0.5f), Size)-rounded;
	value = lerp(value, value < 0.0 ? 1.0 : 0.0, invert);
	
	result = float3(value,value,value);
}



void Triangle_float(float2 UV, float Size, float rounded, float invert, out float3 result)
{
	float tri = sdEquilateralTriangle(UV+float2(-0.5f,-0.5f),Size)-rounded;
	tri = lerp(tri, tri < 0.0 ? 1.0 : 0.0, invert);
	result = float3(tri,tri,tri);
}
void Rhombus_float(float2 UV, float2 Size, float rounded, float invert, out float3 result)
{
	float diamond = sdRhombus(UV+float2(-0.5f,-0.5f), Size)-rounded;
	diamond = lerp(diamond, diamond < 0.0 ? 1.0 : 0.0, invert);
	result = float3(diamond, diamond, diamond);
}

void ColoriesSDF_float(float input,float3 color1, float3 color2,float3 color3,float bands, float shadow,float lines,float outlineThickness,float outlineBlur, out float3 result)
{
	outlineThickness*=0.1f;
	float3 col = lerp(color1,color2,clamp(sign(input),0,1)) ;
	col = lerp(col,col*(1.0 - exp(-2.0*abs(input))),shadow);
	col = lerp(col,col*(0.8 + 0.2*cos(bands*10*input)),lines);
	col= float3(clamp (col.x,0,1),clamp (col.y,0,1),clamp (col.z,0,1));
	col = lerp( col, color3, 1-smoothstep((1-outlineBlur)*(outlineThickness)-0.001f,outlineThickness,clamp(abs(input),0,1)) );
	result = float3(col);
}
void Ring_float(float2 UV, float2 n,float radius,float th, float rounded, float invert, out float3 result)
{
	float value = sdRing(UV+float2(-0.5f,-0.5f), n,radius,th)-rounded;
	value = lerp(value, value < 0.0 ? 1.0 : 0.0, invert);
	result = float3(value, value, value);
}

void Heart_float(float2 UV, float rounded,float invert, out float3 result)
{
	float value = sdHeart(UV*2+float2(-1,-0.5f))-rounded;
	value = lerp(value, value < 0.0 ? 1.0 : 0.0, invert);
	result = float3(value, value, value);
}
void Circle_float(float2 UV, float Radius, float rounded,float invert, out float3 result)
{
	float value = sdCircle(UV+float2(-0.5f,-0.5f), Radius)-rounded;
	value = lerp(value, value < 0.0 ? 1.0 : 0.0, invert);
	
	result = float3(value,value,value);
}

void Star_float(float2 UV, float Radius,float RadiusFill, float rounded,float invert, out float3 result)
{
	float value = sdStar(UV+float2(-0.5f,-0.5f), Radius,RadiusFill)-rounded;
	value = lerp(value, value < 0.0 ? 1.0 : 0.0, invert);
	
	result = float3(value,value,value);
}



