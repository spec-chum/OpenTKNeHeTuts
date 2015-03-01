#version 400

layout(location = 0) in vec3 vertPos;

uniform mat4 MVP;

void main()
{
	gl_Position = MVP * vec4(vertPos, 1.0);
}
