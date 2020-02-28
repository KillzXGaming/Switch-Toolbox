#version 330 core
out vec4 FragColor;
  
in vec3 color;

in vec4 coefficent0;
in vec4 coefficent1;
in vec4 coefficent2;
in vec4 coefficent3;
in vec4 coefficent4;
in vec4 coefficent5;
in vec4 coefficent6;

vec3 CalculateIrradiance()
{
   vec3 nor = vec3(1,1,1);
   vec3 l00 = vec3(coefficent0.x, coefficent0.y, coefficent0.z);
   vec3 l1m1 = vec3(coefficent0.w, coefficent1.x, coefficent1.y);
   vec3 l10 = vec3(coefficent1.z, coefficent1.w, coefficent2.x);
   vec3 l11 = vec3(coefficent2.y, coefficent2.z, coefficent2.w);
   vec3 l2m2 = vec3(coefficent3.x, coefficent3.y, coefficent3.z);
   vec3 l2m1 = vec3(coefficent3.w, coefficent4.x, coefficent4.y);
   vec3 l20 = vec3(coefficent4.z, coefficent4.w, coefficent5.x);
   vec3 l21 = vec3(coefficent5.y, coefficent5.z, coefficent5.w);
   vec3 l22 = vec3(coefficent6.x, coefficent6.y, coefficent6.z);

    const float c1 = 0.429043;
    const float c2 = 0.511664;
    const float c3 = 0.743125;
    const float c4 = 0.886227;
    const float c5 = 0.247708;
    return (max(
        (c1 * l22 * (nor.x * nor.x - nor.y * nor.y) +
        c3 * l20 * nor.z * nor.z +
        c4 * l00 -
        c5 * l20 +
        2.0 * c1 * l2m2 * nor.x * nor.y +
        2.0 * c1 * l21  * nor.x * nor.z +
        2.0 * c1 * l2m1 * nor.y * nor.z +
        2.0 * c2 * l11  * nor.x +
        2.0 * c2 * l1m1 * nor.y +
        2.0 * c2 * l10  * nor.z), 0.0)
    );
}

void main()
{
    vec3 irr = CalculateIrradiance();
    FragColor = vec4(irr, 1.0);
}