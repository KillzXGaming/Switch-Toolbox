#version 330

//Samplers
uniform sampler2D tex;
uniform sampler2D bakeShadowMap;
uniform sampler2D bakeLightMap;

//Toggles
uniform int hasLightMap;
uniform int hasShadowMap;
uniform int hasNormalMap;

uniform vec4 highlight_color;

uniform float Saturation;
uniform float Hue;
uniform float Brightness;

uniform float ao_density;
uniform float shadow_density;

uniform int debugShading;

uniform float bake_shadow_type;
uniform float bake_light_type;

uniform vec3 gsys_bake_light_scale;

// Diffuse Channel Toggles
uniform int RedChannel;
uniform int GreenChannel;
uniform int BlueChannel;
uniform int AlphaChannel;

in vec2 f_texcoord0;
in vec2 f_texcoord1;
in vec2 f_texcoord2;
in vec3 fragPosition;
in vec3 probePosition;

in vec4 vertexColor;
in vec3 normal;

out vec4 fragOutput;


vec3 ApplySaturation(vec3 rgb, float adjustment)
{
    const vec3 W = vec3(0.2125, 0.7154, 0.0721);
    vec3 intensity = vec3(dot(rgb, W));
    return mix(intensity, rgb, adjustment);
}

//https://gist.github.com/mairod/a75e7b44f68110e1576d77419d608786
vec3 hueShift( vec3 color, float hueAdjust ){

    const vec3  kRGBToYPrime = vec3 (0.299, 0.587, 0.114);
    const vec3  kRGBToI      = vec3 (0.596, -0.275, -0.321);
    const vec3  kRGBToQ      = vec3 (0.212, -0.523, 0.311);

    const vec3  kYIQToR     = vec3 (1.0, 0.956, 0.621);
    const vec3  kYIQToG     = vec3 (1.0, -0.272, -0.647);
    const vec3  kYIQToB     = vec3 (1.0, -1.107, 1.704);

    float   YPrime  = dot (color, kRGBToYPrime);
    float   I       = dot (color, kRGBToI);
    float   Q       = dot (color, kRGBToQ);
    float   hue     = atan (Q, I);
    float   chroma  = sqrt (I * I + Q * Q);

    hue += hueAdjust;

    Q = chroma * sin (hue);
    I = chroma * cos (hue);

    vec3    yIQ   = vec3 (YPrime, I, Q);

    return vec3( dot (yIQ, kYIQToR), dot (yIQ, kYIQToG), dot (yIQ, kYIQToB) );
}

float GetComponent(int Type, vec4 Texture)
{
	 switch (Type)
	 {
	     case 0: return Texture.r; 
	     case 1: return Texture.g; 
	     case 2: return Texture.b; 
	     case 3: return Texture.a; 
	     case 4: return 1.0; 
	     case 5: return 0.0; 
		 default: return 1.0;
	 }
}

void main(){
    vec3 displayNormal = (normal.xyz * 0.5) + 0.5;
    float hc_a   = highlight_color.w;

    vec4 diffuseMapColor = texture(tex,f_texcoord0);

	vec4 albedo = vec4(0);
	//Comp Selectors
	albedo.r = GetComponent(RedChannel, diffuseMapColor);
	albedo.g = GetComponent(GreenChannel, diffuseMapColor);
	albedo.b = GetComponent(BlueChannel, diffuseMapColor);
	albedo.a = GetComponent(AlphaChannel, diffuseMapColor);

    vec4 color = vertexColor * albedo;
    color.rgb = ApplySaturation(color.rgb, Saturation) * Brightness;
    if (Hue > 0.0f)
        color.rgb = hueShift(color.rgb, Hue);

     float ShadowPass = 1;
     float AoPass = 1;
     vec3 LightingDiffuse = vec3(0);

     if (hasShadowMap == 1)
     {
     	float aoIntensity = texture(bakeShadowMap, f_texcoord1).r;
	    float  shadowIntensity = texture(bakeShadowMap, f_texcoord1).g;

	    float aoBlend = 0;
		aoBlend += 1.0 - aoIntensity;
		float shadowBlend = 0;
		shadowBlend += 1.0 - shadowIntensity;

		 ShadowPass *= 1.0 - shadowBlend * shadow_density * 0.5;
		 AoPass *= 1.0 - aoBlend * ao_density * 0.6;
     }
    if (hasLightMap == 1)
	{
        vec4 bakeMap1 = texture(bakeLightMap, f_texcoord2);
	    vec3 LightIntensity = vec3(0.1);
		LightingDiffuse += (gsys_bake_light_scale * bakeMap1.rgb * bakeMap1.a) * LightIntensity;
	}

    color.rgb += LightingDiffuse;
    color.rgb *= ShadowPass;
    color.rgb *= AoPass;

    float halfLambert = max(displayNormal.y,0.5);
    vec4 colorComb = vec4(color.rgb * (1-hc_a) + highlight_color.rgb * hc_a, color.a);

    fragOutput = vec4(colorComb.rgb * halfLambert, colorComb.a);
    //gl_FragColor = vec4(color.a, color.a, color.a, 1);
}