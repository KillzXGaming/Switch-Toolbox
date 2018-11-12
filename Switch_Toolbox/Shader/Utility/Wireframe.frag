#version 330

float WireframeIntensity(vec3 distanceToEdges) {
    float minDistance = min(min(distanceToEdges.x, distanceToEdges.y), distanceToEdges.z);

    // Constant wireframe thickness relative to the screen size.
    float thickness = 0.5;
    float smoothAmount = 0.5;

    float delta = fwidth(minDistance);
    float edge0 = delta * thickness;
    float edge1 = edge0 + (delta * smoothAmount);
    float smoothedDistance = smoothstep(edge0, edge1, minDistance);

    return 1 - smoothedDistance;
}
