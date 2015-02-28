using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.IO;

/// Key:
/// VAO     Vertex Array Object
/// VBO     Vertex Buffer Object
/// MVP     Model View Position Matrix

namespace OpenTKNeHeTut3
{
    internal class Game : GameWindow
    {
        private float[] triangleVerts =
        {
             0.0f,  1.0f, 0.0f,  // Top
            -1.0f, -1.0f, 0.0f,  // Bottom Left
             1.0f, -1.0f, 0.0f   // Bottom Right
        };

        private float[] squareVerts =
        {
            -1.0f, -1.0f, 0.0f, // Bottom left
            -1.0f,  1.0f, 0.0f, // Top left            
             1.0f, -1.0f, 0.0f, // Bottom right
             1.0f,  1.0f, 0.0f  // Top right             
        };

        private bool isFullscreen = false;
        private int triangleVao, triangleVbo, squareVao, squareVbo;
        private int program;
        private int MVPLocation;
        private Matrix4 projectionMatrix4, modelViewMatrix4, MVP;

        public Game()
            : base(640, 480, GraphicsMode.Default, "OpenTK NeHe Tutorial 3")
        {
            VSync = VSyncMode.On;
        }

        private void GenerateBuffers()
        {
            // Generate Vertex array object
            GL.GenVertexArrays(1, out triangleVao);
            GL.BindVertexArray(triangleVao);

            // Vertices
            GL.GenBuffers(1, out triangleVbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, triangleVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(triangleVerts.Length * sizeof(float)), triangleVerts, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            // Enable the position attribute
            GL.EnableVertexAttribArray(0);

            // Generate Vertex array object
            GL.GenVertexArrays(1, out squareVao);
            GL.BindVertexArray(squareVao);

            // Vertices
            GL.GenBuffers(1, out squareVbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, squareVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(squareVerts.Length * sizeof(float)), squareVerts, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            // Enable the position attribute
            GL.EnableVertexAttribArray(0);            
        }

        private void InitScene()
        {
            // Set up uniform locations
            GL.UseProgram(program);
            MVPLocation = GL.GetUniformLocation(program, "MVP");

            // Initialise MVP Matrix
            float ar = (float)ClientSize.Width / (float)ClientSize.Height;
            projectionMatrix4 = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, ar, 0.1f, 1000.0f);
            modelViewMatrix4 = Matrix4.LookAt(0, 0.0f, 0.1f, 0, 0, 0, 0, 1.0f, 0);
            MVP = modelViewMatrix4 * projectionMatrix4;
            GL.UniformMatrix4(MVPLocation, false, ref MVP);
            GL.UseProgram(0);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            // Update projection matrix for new window size
            float ar = (float)ClientSize.Width / (float)ClientSize.Height;
            projectionMatrix4 = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, ar, 0.1f, 1000.0f);
        }

        private void LoadShaders()
        {
            int status;

            // Read and compile vertex shader
            string shaderData = File.ReadAllText("./Shaders/vertex.glsl");
            int vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, shaderData);
            GL.CompileShader(vs);
            GL.GetShader(vs, ShaderParameter.CompileStatus, out status);
            if (status != 1)
            {
                Trace.TraceInformation(GL.GetShaderInfoLog(vs));
                Exit();
            }
            else
            {
                Trace.TraceInformation("Vertex Shader compiled OK...\n\n");
            }

            // Read and compile fragment shader
            shaderData = File.ReadAllText("./Shaders/fragment.glsl");
            int fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, shaderData);
            GL.CompileShader(fs);
            GL.GetShader(fs, ShaderParameter.CompileStatus, out status);
            if (status != 1)
            {
                Trace.TraceInformation(GL.GetShaderInfoLog(fs));
                Exit();
            }
            else
            {
                Trace.TraceInformation("Fragment Shader compiled OK...\n\n");
            }

            // Create and link shader program
            program = GL.CreateProgram();
            GL.AttachShader(program, vs);
            GL.AttachShader(program, fs);
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out status);
            if (status != 1)
            {
                Trace.TraceInformation(GL.GetProgramInfoLog(program));
                Exit();
            }
            else
            {
                Trace.TraceInformation("Program linked OK...\n\n");
            }

            // No need for the shaders now so just detach them
            GL.DetachShader(program, vs);
            GL.DetachShader(program, fs);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Keyboard.KeyDown += Keyboard_KeyDown;

            GenerateBuffers();
            LoadShaders();
            InitScene();
        }

        protected void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Exit();
            }

            // Press F1 to toggle full screen
            if (e.Key == Key.F1)
            {
                if (isFullscreen == false)
                {
                    WindowState = WindowState.Fullscreen;
                    isFullscreen = true;
                }
                else
                {
                    WindowState = WindowState.Normal;
                    isFullscreen = false;
                }
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(program);

            // Move triangle left 1.5 units from origin
            MVP = Matrix4.CreateTranslation(-1.5f, 0.0f, -6.0f) * projectionMatrix4;
            GL.UniformMatrix4(MVPLocation, false, ref MVP);

            GL.BindVertexArray(triangleVao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            // Move square right 1.5 units from origin
            MVP = Matrix4.CreateTranslation(1.5f, 0.0f, -6.0f) * projectionMatrix4;
            GL.UniformMatrix4(MVPLocation, false, ref MVP);

            // We use a trianglestrip as quads are deprecated
            GL.BindVertexArray(squareVao);            
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            GL.UseProgram(0);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            // Do amazing stuff here!
        }

        [STAThread]
        private static void Main()
        {
            TextWriterTraceListener listener = new TextWriterTraceListener("debugLog.log") { TraceOutputOptions = TraceOptions.DateTime };
            Trace.Listeners.Add(listener);

            using (Game game = new Game())
            {
                game.Run(60.0);
            }

            Trace.Flush();
        }
    }
}