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

uniform int ThresholdingAlphaInterpolation;

uniform sampler2D uvTestPattern;

#define gamma 2.2

void main()
{
	vec4 textureMap0 = vec4(1);
	vec4 textureMap1 = vec4(1);
	vec4 textureMap2 = vec4(1);

	if (numTextureMaps > 0)
	{
		if (hasTexture0 == 1)
             textureMap0 = texture2D(textures0, gl_TexCoord[0].st);
	}

	if (debugShading == 0)
	{
		// Convert to sRGB.
		vec3 whiteColorSRGB = pow(whiteColor.rgb, vec3(1.0 / gamma));

		vec3 whiteInterpolation = whiteColorSRGB.rgb * textureMap0.rgb;
		vec3 blackInterpolation = (vec3(1) - textureMap0.rgb) * blackColor.rgb;

		vec3 colorBlend = whiteInterpolation + blackInterpolation;
		float alpha = textureMap0.a * whiteColor.a;
		if (ThresholdingAlphaInterpolation != 0)
		{
		    //Todo these need to interpolate and be smoother
		     if (textureMap0.a >= whiteColor.a) alpha = 1.0;
		     if (textureMap0.a <= blackColor.a) alpha = 0.0;
			// if (blackColor.a < alpha && alpha < whiteColor.a)
			//	 alpha = mix(0.0, 1.0, textureMap0.a);
		}

		gl_FragColor = gl_Color * vec4(colorBlend,alpha);
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