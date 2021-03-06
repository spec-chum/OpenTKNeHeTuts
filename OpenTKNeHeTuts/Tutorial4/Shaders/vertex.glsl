#version 400

layout(location = 0) in vec3 vertPos;
layout(location = 1) in vec3 vertColour;

uniform mat4 MVP;

out vec3 colour;

void main()
{
	colour = vertColour;
	gl_Position = MVP * vec4(vertPos, 1.0);
}
