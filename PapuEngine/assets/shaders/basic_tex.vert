#version 460 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoord;

uniform vec2 offset;
uniform float angle;
uniform vec2 center;
uniform float aspect;
uniform float dist;

out vec2 fragUV;

void main() {
    float c = cos(angle);
    float s = sin(angle);
    
    vec2 p = position.xy - center;
    
    vec2 rotated = vec2(
        p.x * c - p.y * s,
        p.x * s + p.y * c
    );
    
    vec2 final = rotated + center + offset;
    
    fragUV = texCoord;
    gl_Position = vec4(final.x / aspect, final.y, position.z, dist);
}