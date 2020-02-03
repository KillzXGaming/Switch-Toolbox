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

uniform int AlphaInterpolation;
uniform int numTevStages;

uniform sampler2D uvTestPattern;

#define gamma 2.2

vec3 ColorOP(int type, vec4 color)
{
    switch (type)
	{
	   case 0: return color.rgb;
	   case 1: return vec3(1.0) - color.rgb;
	   case 2: return color.aaa;
	   case 3: return  vec3(1.0) - color.aaa;
	   case 4: return color.rrr;
	   case 5: return  vec3(1.0) - color.rrr;
	   case 6: return color.ggg;
	   case 7: return  vec3(1.0) - color.ggg;
	   case 8: return color.bbb;
	   case 9: return  vec3(1.0) - color.bbb;
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

vec3 ColorCombiner(int type, vec4 j1, vec4 j2, vec4 j3)
{
    switch (type)
	{
	   case 0: return j1.rgb; //Replace
	   case 1: return j1.rgb * j2.rgb; //Modulate
	   case 2: return j1.rgb + j2.rgb; //Add
	   case 3: return j1.rgb + j2.rgb - vec3(0.5); //AddSigned
	   case 4: return j1.rgb * j3.rgb + j2.rgb * (vec3(1.0) - j3.rgb); //Interpolate
	   case 5: return j1.rgb - j2.rgb; //Subtract
	   case 6: return clamp(j1.rgb + j2.rgb, 0.0, 1.0) * j3.rgb; //AddMultiplicate
	   case 7: return (j1.rgb * j2.rgb) + j3.rgb; //MultiplcateAdd
	   case 8: return j1.rgb; //Overlay
	   case 9: return j1.rgb; //Indirect
	   case 10: return j1.rgb; //BlendIndirect
	   case 11: return j1.rgb; //EachIndirect
	   default: return j1.rgb;
	}
}

float AlphaCombiner(int type, vec4 j1, vec4 j2, vec4 j3)
{
    switch (type)
	{
	   case 0: return j1.a; //Replace
	   case 1: return j1.a * j2.a; //Modulate
	   case 2: return j1.a + j2.a; //Add
	   case 3: return j1.a + j2.a - 0.5; //AddSigned
	   case 4: return j3.a + j2.a * (1.0 - j3.a); //Interpolate
	   case 5: return j1.a - j2.a; //Subtract
	   case 6: return clamp(j1.a + j2.a, 0.0, 1.0) * j3.a; //AddMultiplicate
	   case 7: return (j1.a * j2.a) + j3.a; //MultiplcateAdd
	   case 8: return j1.a; //Overlay
	   case 9: return j1.a; //Indirect
	   case 10: return j1.a;  //BlendIndirect
	   case 11: return j1.a; //EachIndirect
	   default: return j1.a;
	}
}

void main()
{
	vec4 textureMap0 = vec4(1);
	vec4 textureMap1 = vec4(1);
	vec4 textureMap2 = vec4(1);

	if (numTextureMaps > 0)
	{
		if (hasTexture0 == 1)
             textureMap0 = texture2D(textures0, gl_TexCoord[0].st);
		if (hasTexture1 == 1)
             textureMap1 = texture2D(textures1, gl_TexCoord[0].st);
		if (hasTexture2 == 1)
             textureMap2 = texture2D(textures1, gl_TexCoord[0].st);
	}
	if (debugShading == 0)
	{
		// Convert to sRGB.
		vec3 whiteColorSRGB = pow(whiteColor.rgb, vec3(1.0 / gamma));

		vec3 whiteInterpolation = whiteColorSRGB.rgb * textureMap0.rgb;
		vec3 blackInterpolation = (vec3(1) - textureMap0.rgb) * blackColor.rgb;
  
  		//vec3 colorBlend = whiteInterpolation + blackInterpolation;
    	vec3 colorBlend = ColorCombiner(4, vec4(whiteColorSRGB.rgb, 1), blackColor, textureMap0);

		float alpha = textureMap0.a * whiteColor.a;

		//More that 1 texture uses texture combiners
		vec4 j1;
		vec4 j2;
		vec4 j3;
		vec4 fragOutput;
		vec4 previousStage = vec4(1);
		if (numTextureMaps > 1 && numTevStages > 4)
		{
		    for (int i = 0; i < numTevStages; i++)
			{
				j1 = textureMap0;
				j2 = textureMap1;
				j3 = vec4(1);
				if (numTextureMaps > 2)
					j3 = textureMap2;

				vec4 comb1 = vec4(ColorCombiner(tevStage0RGB, j1, j2, j3), AlphaCombiner(tevStage0A, j1,j2,j3));
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

		fragOutput *= gl_Color;
		gl_FragColor = fragOutput;
	}
	else if (debugShading == 5)
		gl_FragColor = vec4(textureMap0.rgb, 1);
	else if (debugShading == 1)
		gl_FragColor = gl_Color;
	else if (debugShading == 2)
		gl_FragColor = whiteColor;
	else if (debugShading == 3)
		gl_FragColor = blackColor;
	else if (debugShading == 4)
        gl_FragColor = texture2D(uvTestPattern, gl_TexCoord[0].st);
}