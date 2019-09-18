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

uniform sampler2D uvTestPattern;

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
		vec3 whiteInterpolation = whiteColor.rgb * textureMap0.rgb;
		vec3 blackInterpolation = (vec3(1) - textureMap0.rgb) * blackColor.rgb;

		vec3 colorBlend = whiteInterpolation + blackInterpolation;
		float alpha = textureMap0.a * whiteColor.a;
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