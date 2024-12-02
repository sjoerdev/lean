#version 330 core

layout (location = 0) in vec2 aposition;
layout (location = 1) in vec2 atexcoord;

out vec2 texcoord;

uniform vec2 position;

void main()
{
    gl_Position = vec4(aposition + position, 0.0, 1.0);
    texcoord = atexcoord;
}