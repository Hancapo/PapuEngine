using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PapuEngine;

public class Game() : GameWindow(GameWindowSettings.Default,
    new NativeWindowSettings
    {
        Title = "PapuEngine 2D",
        API = ContextAPI.OpenGL,
        APIVersion = new Version(4, 6),
        ClientSize = new Vector2i(1280, 720),
    })
    {
        public List<VertexData> PlayerGeom =
        [
            new()
            {
                Position = new Vector3(-0.25f, -0.25f, 0.0f),
                TexCoord = new Vector2(0f, 0f),
            },
            new()
            {
                Position = new Vector3(0f, -0.25f, 0.0f),
                TexCoord = new Vector2(1f, 0f),
            },
            new()
            {
                Position = new Vector3(-0.125f, -0.05f, 0.0f),
                TexCoord = new Vector2(1f, 1f),
            }
        ];

        public List<VertexData> BackgroundQuad =
        [
            new()
            {
                Position = new Vector3(-4f, -1f, 0f),
                TexCoord = new Vector2(0f, 0f),
            },
            new()   
            {
                Position = new Vector3(4f, -1f, 0f),
                TexCoord = new Vector2(4f, 0f),
            },
            new()
            {
                Position = new Vector3(-4f, 1f, 0f),
                TexCoord = new Vector2(0f, 1f),
            },
            new()
            {
                Position = new Vector3(4f, 1f, 0f),
                TexCoord = new Vector2(4f, 1f),
            }
        ];
        private Vector2 PointsCenter => new(PlayerGeom.Average(p => p.Position.X), PlayerGeom.Average(p => p.Position.Y));
        
        string _vertexShader = File.ReadAllText("simple.vert");
        string _fragmentShader = File.ReadAllText("simple.frag");
        int _playerVBO, _bgVBO;
        
        int _playerVAO, _bgVAO;
        int _shaderProgram;

        private float _time;
        private Vector2 offset;
        private float speed = 1f;
        private float aspect;
        private Vector2 input;

        private int _bgTex, _playerTex;
        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.2f, 0.3f, 0.8f, 1.0f);
            aspect = Size.X / (float)Size.Y;

            var _points = PlayerGeom.SelectMany(x => x.Flatten()).ToArray();
            var _bgPoints = BackgroundQuad.SelectMany(x => x.Flatten()).ToArray();
            //Es una zona de memoria (en la GPU) que guarda datos.
            //Ejemplo: Las coordenadas de los vértices que forman tu triángulo.
            
            //VBO 1
            GL.GenBuffers(1, out _playerVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _playerVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, _points.Length * sizeof(float), _points, BufferUsageHint.StaticDraw);
            
            //Un VAO es como una configuración que guarda cómo se debe interpretar un buffer, una especie de receta que explica cómo leer los datos.
            //VAO 1
            GL.GenVertexArrays(1, out _playerVAO); //Crear un VAO.
            GL.BindVertexArray(_playerVAO); // Activarlo.
            
            // Leer la posición de los vértices
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0); //Decir cómo leer el buffer.
            GL.EnableVertexAttribArray(0); // Habilitarlo.
            
            //Leer las coordenadas de las UVs
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float)); // UV
            GL.EnableVertexAttribArray(1);
            
            //Background VB0
            GL.GenBuffers(1, out _bgVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _bgVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, _bgPoints.Length * sizeof(float), _bgPoints, BufferUsageHint.StaticDraw);
            
            //Background VAO
            GL.GenVertexArrays(1, out _bgVAO);
            GL.BindVertexArray(_bgVAO);
            
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            
            
            //Create Shader
            var vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, _vertexShader);
            GL.CompileShader(vs);
            
            var fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, _fragmentShader);
            GL.CompileShader(fs);
            
            _shaderProgram = GL.CreateProgram();

            GL.AttachShader(_shaderProgram, fs);
            GL.AttachShader(_shaderProgram, vs);
            GL.LinkProgram(_shaderProgram);

            _bgTex = ImageHelper.LoadImage("textures/Cave_01.png", repeat: true);
            _playerTex = ImageHelper.LoadImage("textures/pearto.png", repeat: false);
            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _bgTex);
            
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _playerTex);
            
            int texLocation = GL.GetUniformLocation(_shaderProgram, "tex");
            GL.Uniform1(texLocation, 0);
            
            GL.DeleteShader(vs);
            GL.DeleteShader(fs);

            /* Siempre al llamar a VertexAttribPointer se debe tener:
             el VAO vinculado con GL.BindVertexArray
             y el VBO vinculado con GL.BindArray*/

        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            _time += (float)args.Time;
            var sin = MathF.Sin(_time);
            var cos = MathF.Cos(_time);
            
            input = Vector2.Zero;

            if (KeyboardState.IsKeyDown(Keys.W)) input.Y += 1;
            if (KeyboardState.IsKeyDown(Keys.S)) input.Y -= 1;
            if (KeyboardState.IsKeyDown(Keys.D)) input.X += 1;
            if (KeyboardState.IsKeyDown(Keys.A)) input.X -= 1;

            if (input.LengthSquared > 0)
            {
                input = input.Normalized();
            }
            
            offset += input * speed * (float)args.Time;

        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(_shaderProgram);
            int offsetLocation = GL.GetUniformLocation(_shaderProgram, "offset");
            int angleLocation = GL.GetUniformLocation(_shaderProgram, "angle");
            int centerLocation = GL.GetUniformLocation(_shaderProgram, "center");
            int aspectLocation = GL.GetUniformLocation(_shaderProgram, "aspect");
            int distLocation = GL.GetUniformLocation(_shaderProgram, "dist");
            
            GL.Uniform1(distLocation, 1.0f);
            GL.Uniform2(offsetLocation, Vector2.Zero);
            GL.Uniform1(angleLocation, 0f);
            GL.Uniform2(centerLocation, Vector2.Zero);
            GL.BindVertexArray(_bgVAO);
            GL.BindTexture(TextureTarget.Texture2D, _bgTex);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            
            GL.Uniform1(distLocation, 1.0f);
            GL.Uniform1(aspectLocation, aspect);
            GL.Uniform2(centerLocation, PointsCenter);
            GL.Uniform2(offsetLocation, new Vector2(offset.X, offset.Y));
            GL.BindVertexArray(_playerVAO);
            GL.BindTexture(TextureTarget.Texture2D, _playerTex);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            SwapBuffers();
        }
    }   
public abstract class Program
{
    public static void Main()
    {
        using var game = new Game();
        game.Run();
    }
}