using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.IO;

namespace OpenTKNeHeTut2
{
    internal class Game : GameWindow
    {
        private float[] triangle =
        {
             0.0f, 1.0f, 0.0f,  // Top
            -1.0f,-1.0f, 0.0f,  // Bottom Left
             1.0f,-1.0f, 0.0f   // Bottom Right
        };

        private float[] square =
        {
             // We use 2 triangles as quads are deprecated
            -1.0f,  1.0f, 0.0f, // Top left
            -1.0f, -1.0f, 0.0f, // Bottom left
             1.0f,  1.0f, 0.0f, // Top right

             1.0f,  1.0f, 0.0f, // Top right
            -1.0f, -1.0f, 0.0f, // Bottom left
             1.0f, -1.0f, 0.0f  // Bottom right
        };

        private bool isFullscreen = false;
        private int triangleVao, triangleVbo, squareVao, squareVbo;
        private int program;
        private int MVPLocation, translateLocation;
        private Matrix4 projectionMatrix4, modelMatrix4;

        public Game()
            : base(640, 480, GraphicsMode.Default, "OpenTK NeHe Tutorial 2")
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
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(triangle.Length * sizeof(float)), triangle, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            // Enable the position attribute
            GL.EnableVertexAttribArray(0);
            
            // Generate Vertex array object
            GL.GenVertexArrays(1, out squareVao);
            GL.BindVertexArray(squareVao);

            // Vertices
            GL.GenBuffers(1, out squareVbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, squareVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(square.Length * sizeof(float)), square, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            // Enable the position attribute
            GL.EnableVertexAttribArray(0);            
        }

        private void InitScene()
        {
            // Set up uniform locations
            GL.UseProgram(program);
            MVPLocation = GL.GetUniformLocation(program, "MVP");
            translateLocation = GL.GetUniformLocation(program, "translate");

            // Initialise MVP Matrix
            float ar = (float)ClientSize.Width / (float)ClientSize.Height;
            projectionMatrix4 = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, ar, 0.1f, 1000.0f);
            modelMatrix4 = Matrix4.LookAt(0, 0.0f, 10.0f, 0, 0, 0, 0, 1.0f, 0);
            Matrix4 MVP = modelMatrix4 * projectionMatrix4;
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
            string shaderData = File.ReadAllText("./Shaders/vertex.glsl");
            int vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, shaderData);
            GL.CompileShader(vs);
            GL.GetShader(vs, ShaderParameter.CompileStatus, out status);
            if (status != 1)
            {
                Debug.WriteLine(GL.GetShaderInfoLog(vs));
                Exit();
            }

            shaderData = File.ReadAllText("./Shaders/fragment.glsl");
            int fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, shaderData);
            GL.CompileShader(fs);
            GL.GetShader(fs, ShaderParameter.CompileStatus, out status);
            if (status != 1)
            {
                Debug.WriteLine(GL.GetShaderInfoLog(fs));
                Exit();
            }

            program = GL.CreateProgram();
            GL.AttachShader(program, vs);
            GL.AttachShader(program, fs);
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out status);
            if (status != 1)
            {
                Debug.WriteLine(GL.GetProgramInfoLog(program));
                Exit();
            }

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

            // Should really update the modelView matrix for this but want to keep it simple
            Vector3 translate = new Vector3(-1.5f, 0.0f, -6.0f);
            GL.Uniform3(translateLocation, ref translate);

            GL.BindVertexArray(triangleVao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            translate = new Vector3(1.5f, 0.0f, -6.0f);
            GL.Uniform3(translateLocation, ref translate);

            GL.BindVertexArray(squareVao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

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
            //TextWriterTraceListener debugLog = new TextWriterTraceListener(Console.Out);
            //Debug.Listeners.Add(debugLog);

            using (Game game = new Game())
            {
                game.Run(60.0);
            }

            //debugLog.Flush();
            //debugLog.Close();
        }
    }
}