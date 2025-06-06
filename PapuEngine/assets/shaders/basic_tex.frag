﻿#version 460 core
in vec2 fragUV;
out vec4 frag_colour;

uniform sampler2D tex;

void main() {
    frag_colour = texture(tex, fragUV);
}