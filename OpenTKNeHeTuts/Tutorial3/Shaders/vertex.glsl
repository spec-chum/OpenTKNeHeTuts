#version 400

layout(location = 0) in vec3 vertPos;

uniform mat4 MVP;
uniform vec3 translate;

void main()
{
	gl_Position = MVP * vec4(vertPos + translate, 1.0);
}
