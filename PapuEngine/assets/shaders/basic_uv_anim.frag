#version 460 core

in vec2 fragUV;
out vec4 frag_colour;

uniform sampler2D tex;
uniform float iTime;
uniform float iSpeed;

void main() {
    vec2 test = fragUV;
    test.y = fract(fragUV.y + iTime * iSpeed);
    frag_colour = texture(tex, test);
}
