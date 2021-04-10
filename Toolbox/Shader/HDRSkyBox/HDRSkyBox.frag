#version 330 core
out vec4 FragColor;

in vec3 TexCoords;
  
uniform samplerCube environmentMap;
uniform int hdrEncoded;
uniform float gamma;

void main()
{
    vec4 envTexture = textureLod(environmentMap, TexCoords, 0.0).rgba;
    vec3 envColor = envTexture.rgb;

    if (hdrEncoded == 1)
    {
        envColor = envTexture.rgb * pow(envTexture.a, 4) * 1024;
        envColor = envColor / (envColor + vec3(1.0));
        envColor = pow(envColor, vec3(1.0/gamma)); 
    }

    FragColor = vec4(envColor, 1.0);
}