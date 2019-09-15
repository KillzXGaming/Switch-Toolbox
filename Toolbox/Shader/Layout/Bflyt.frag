#version 330

uniform vec4 blackColor;
uniform vec4 whiteColor;
uniform int hasTexture0;
uniform int debugShading;
uniform int numTextureMaps;

uniform sampler2D textures0;
uniform sampler2D uvTestPattern;

in vec2 uv0;
in vec4 vertexColor0;

out vec4 fragColor;

void main()
{
	vec4 textureMap0 = vec4(1);
	vec4 textureMap1 = vec4(1);
	vec4 textureMap2 = vec4(1);

	if (numTextureMaps > 0)
	{
		if (hasTexture0 == 1)
             textureMap0 = texture2D(textures0, uv0);
	}

	if (debugShading == 0)
	{
		vec4 colorBlend = textureMap0 * whiteColor;
		fragColor = vertexColor0 * colorBlend;
	}
	else if (debugShading == 5)
		fragColor = vec4(textureMap0.rgb, 1);
	else if (debugShading == 1)
		fragColor = vertexColor0;
	else if (debugShading == 2)
		fragColor = whiteColor;
	else if (debugShading == 3)
		fragColor = blackColor;
	else if (debugShading == 4)
        fragColor = texture2D(uvTestPattern, uv0);
}