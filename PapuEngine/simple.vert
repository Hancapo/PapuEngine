#version 460 core
in vec3 vp;

uniform vec2 offset;
uniform float angle;
uniform vec2 center;
uniform float aspect;

void main() {
    float c = cos(angle);
    float s = sin(angle);
    
    vec2 p = vp.xy - center;
    
    vec2 rotated = vec2(
        p.x * c - p.y * s,
        p.x * s + p.y * c
    );
    
    vec2 final = rotated + center + offset;
    
    gl_Position = vec4(final.x / aspect, final.y, vp.z, 1.0);
}