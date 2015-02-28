using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Diagnostics;

// Keys
// VAO	Vertex Array Object
// MAO	Matrix Arrat Object
// VBO	Vertex Bind Object
// MVP	Model View Position

namespace OpenTKNeHeTut1
{
    internal class Game : GameWindow
    {
        private bool isFullscreen = false;

        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK NeHe Tutorial 1")
        {
            VSync = VSyncMode.On;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Keyboard.KeyDown += Keyboard_KeyDown;

            // Place initialisation code here
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

            // Do drawing stuff here

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
            TextWriterTraceListener debugLog = new TextWriterTraceListener(Console.Out);
            Debug.Listeners.Add(debugLog);

            using (Game game = new Game())
            {
                game.Run(60.0);
            }

            debugLog.Flush();
            debugLog.Close();
        }
    }
}