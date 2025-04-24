#version 460 core
in vec2 fragUV;
out vec4 frag_colour;

uniform sampler2D tex;
uniform float iTime;
uniform float iSpeed;

void main() {
    fragUV.y += iTime * iSpeed;
    fragUV = fract(fragUV);
    frag_colour = texture(tex, fragUV);
}