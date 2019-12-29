#version 330

float Luminance(vec3 rgb)
{
    const vec3 W = vec3(0.2125, 0.7154, 0.0721);
    return dot(rgb, W);
}

// The hardware conversion doesn't work on all drivers.
// http://entropymine.com/imageworsener/srgbformula/
float SrgbToLinear(float x)
{
    if (x < 0.03928)
        return x * 0.0773993808; // 1.0 / 12.92
    else
        return pow((x + 0.055) / 1.055, 2.4);
}

vec3 SrgbToLinear(vec3 color) {
    return vec3(SrgbToLinear(color.r), SrgbToLinear(color.g), SrgbToLinear(color.b));
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
