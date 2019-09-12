uniform vec4 blackColor;
uniform vec4 whiteColor;
uniform int hasTexture0;
uniform int debugShading;
uniform int numTextureMaps;

uniform sampler2D textures0;
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
		vec4 colorBlend = textureMap0 * whiteColor;
	    vec3 blackBlend = (vec3(1) - textureMap0.rgb) + blackColor.rgb;
		gl_FragColor = gl_Color * colorBlend;
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