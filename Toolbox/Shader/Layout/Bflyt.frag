#version 330

uniform vec4 blackColor;
uniform vec4 whiteColor;

uniform int debugShading;
uniform int numTextureMaps;

uniform sampler2D textures0;
uniform sampler2D textures1;
uniform sampler2D textures2;

uniform int hasTexture0;
uniform int hasTexture1;
uniform int hasTexture2;

uniform int tevStage0RGB;
uniform int tevStage1RGB;
uniform int tevStage2RGB;
uniform int tevStage3RGB;
uniform int tevStage4RGB;
uniform int tevStage5RGB;

uniform int tevStage0A;
uniform int tevStage1A;
uniform int tevStage2A;
uniform int tevStage3A;
uniform int tevStage4A;
uniform int tevStage5A;

uniform int tevTexMode;

uniform int AlphaInterpolation;
uniform int numTevStages;

uniform vec4 IndirectMat0;
uniform vec4 IndirectMat1;

uniform sampler2D uvTestPattern;

in vec4 VertexColor;
in vec2 TexCoord0;
in vec2 TexCoord1;
in vec2 TexCoord2;

// Channel Toggles
uniform int RedChannel;
uniform int GreenChannel;
uniform int BlueChannel;
uniform int AlphaChannel;

out vec4 fragColor;

const int CombineModeMask = (1 << 5) - 1;

#define gamma 2.2

vec3 ColorOP(int type, vec4 color)
{
    switch (type)
	{
	   case 0: return color.rgb;
	   case 1: return vec3(1) - color.rgb;
	   case 2: return color.aaa;
	   case 3: return  vec3(1) - color.aaa;
	   case 4: return color.rrr;
	   case 5: return  vec3(1) - color.rrr;
	   case 6: return color.ggg;
	   case 7: return  vec3(1) - color.ggg;
	   case 8: return color.bbb;
	   case 9: return  vec3(1) - color.bbb;
	   default: return color.rgb;
	}
}

float AlphaOP(int type, vec4 color)
{
    switch (type)
	{
	   case 0: return color.a;
	   case 1: return 1.0 - color.a;
	   case 2: return color.r;
	   case 3: return 1.0 - color.r;
	   case 4: return color.g;
	   case 5: return 1.0 - color.g;
	   case 6: return color.b;
	   case 7: return 1.0 - color.b;
	   default: return color.a;
	}
}

float GetComponent(int Type, vec4 Texture)
{
	 switch (Type)
	 {
	     case 0: return Texture.r; 
	     case 1: return Texture.g; 
	     case 2: return Texture.b; 
	     case 3: return Texture.a; 
	     case 4: return 1; 
	     case 5: return 0; 
		 default: return 1;
	 }
}

vec3 Interpolate(vec4 j1, vec4 j2,  vec4 j3)
{
	return j1.rgb * j3.rgb + j2.rgb * (vec3(1.0) - j3.rgb);
}

vec4 IndTexSingle(vec4 color, sampler2D tex, vec2 texCoord)
{
    float a = color.a;
	vec2 indirect;
	indirect.x = dot(vec4(color.xyz,1.0), IndirectMat0);
	indirect.y = dot(vec4(color.xyz,1.0), IndirectMat1);

	vec4 outColor = texture2D(tex, texCoord + indirect);
	outColor.a = min(outColor.a, color.a);
	return outColor;
}

vec3 ColorCombiner(int type, vec4 j1, vec4 j2,  bool maxA)
{
    switch (type)
	{
	   case 0: return j1.rgb * j1.a + j2.rgb * (1.0 - j1.a); //Replace
	   case 1: return j1.rgb * j2.rgb; //Modulate
	   case 2: return j1.rgb + j2.rgb * j1.a; //Add
	   case 3: return j1.rgb + j2.rgb - vec3(0.5); //AddSigned
	   case 4: return j1.rgb * j2.rgb; //Interpolate
	   case 5: return j1.rgb - j2.rgb * j1.a; //Subtract
	   case 6: return clamp(j2.rgb / (1.00001 - j1.rgb * j1.a), 0.0, 1.0); //AddMultiplicate
	   case 7: return clamp(vec3(1.0) - (vec3(1.00001 - j2.rgb) / j1.rgb), 0.0, 1.0); //MultiplcateAdd
	   case 8: return j1.rgb; //Overlay
	   case 9: return j1.rgb; //Indirect
	   case 10: return j1.rgb; //BlendIndirect
	   case 11: return j1.rgb; //EachIndirect
	   default: return j1.rgb;
	}
}

void main()
{
    fragColor = vec4(1);

	return;

	vec4 textureMap0 = vec4(1);
	vec4 textureMap1 = vec4(1);
	vec4 textureMap2 = vec4(1);

	if (numTextureMaps > 0)
	{
		if (hasTexture0 == 1)
             textureMap0 = texture2D(textures0, TexCoord0.st);
		if (hasTexture1 == 1)
             textureMap1 = texture2D(textures1, TexCoord0.st);
		if (hasTexture2 == 1)
             textureMap2 = texture2D(textures1, TexCoord0.st);
	}
	if (debugShading == 0)
	{
		// Convert to sRGB.
		vec3 whiteColorSRGB = pow(whiteColor.rgb, vec3(1.0 / gamma));

		vec3 whiteInterpolation = whiteColorSRGB.rgb * textureMap0.rgb;
		vec3 blackInterpolation = (vec3(1) - textureMap0.rgb) * blackColor.rgb;
  
  		//vec3 colorBlend = whiteInterpolation + blackInterpolation;
    	vec3 colorBlend = Interpolate(vec4(whiteColorSRGB.rgb, 1), blackColor, textureMap0);

		float alpha = textureMap0.a * whiteColor.a;

		//More that 1 texture uses texture combiners
		vec4 j1;
		vec4 j2;
		vec4 j3;
		vec4 fragOutput;
		vec4 previousStage = vec4(1);
		if (numTextureMaps > 1 && numTevStages > 1)
		{
		    for (int i = 0; i < numTevStages; i++)
			{
				//Todo what determines the tev sources???
				if (i == 0)
				{
					j1 = textureMap0;
					j2 = textureMap1;
					j3 = vec4(1);
				}
				if (i == 1)
				{

				}
				if (numTextureMaps > 2 && tevStage0RGB == 9)
				{
					j3 = textureMap2;
				}
				vec4 comb1 = vec4(ColorCombiner(tevStage0RGB, j1, j2, false), 1);
				previousStage = comb1;
			}

			fragOutput = previousStage;
		}
		else
		{
			if (AlphaInterpolation != 0)
			{
				//Todo these need to interpolate and be smoother
				 if (textureMap0.a >= whiteColor.a) alpha = 1.0;
				 if (textureMap0.a <= blackColor.a) alpha = 0.0;
				// if (blackColor.a < alpha && alpha < whiteColor.a)
				//	 alpha = mix(0.0, 1.0, textureMap0.a);
			}

			fragOutput = vec4(colorBlend,alpha);;
		}

		fragOutput *= VertexColor;
		fragColor = fragOutput;
	}
	else if (debugShading == 5)
		fragColor = vec4(textureMap0.rgb, 1);
	else if (debugShading == 1)
		fragColor = VertexColor;
	else if (debugShading == 2)
		fragColor = whiteColor;
	else if (debugShading == 3)
		fragColor = blackColor;
	else if (debugShading == 4)
        fragColor = texture2D(uvTestPattern, TexCoord0.st);
}