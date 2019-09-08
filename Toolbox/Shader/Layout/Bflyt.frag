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
	if (numTextureMaps > 0)
	{
		if (hasTexture0 == 1)
             textureMap0 = texture2D(textures0, gl_TexCoord[0].st);;
	}
	if (debugShading == 0)
	{
		vec4 colorFrag = gl_Color * textureMap0;
		vec4 colorBlend = colorFrag * whiteColor;
		gl_FragColor = colorBlend;
	}
	if (debugShading == 1)
		gl_FragColor = vec4(textureMap0.rgb, 1);
	if (debugShading == 2)
		gl_FragColor = whiteColor;
	if (debugShading == 3)
		gl_FragColor = blackColor;
	if (debugShading == 4)
        gl_FragColor = texture2D(uvTestPattern, gl_TexCoord[0].st);
}