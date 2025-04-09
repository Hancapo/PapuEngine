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
    public Vector3[] points =
    [
        new(-0.25f, -0.25f, 0.0f),
        new(0f, -0.25f, 0.0f),
        new(0f, 0f, 0.0f),
    ];

    public Vector3[] points2 =
    [
        new(0f, -0.25f, 0.0f), 
        new(0.25f, -0.25f, 0.0f),
        new(0f, 0f, 0.0f),
    ];
    
    private Vector2 PointsCenter => new(points.Average(p => p.X), points.Average(p => p.Y));
    private Vector2 Points2Center => new(points2.Average(p => p.X), points2.Average(p => p.Y));

    private static float[] Flatten(Vector3[] points) => points.SelectMany(p => new [] { p.X, p.Y, p.Z }).ToArray();
    
    string _vertexShader = File.ReadAllText("simple.vert");
    string _fragmentShader = File.ReadAllText("simple.frag");
    int _vbo, _vbo2;
    
    int _vao, _vao2;
    int _shaderProgram;

    private float _time;
    private Vector2 offset;
    private float _angle;
    private float speed = 1f;
    private float aspect;
    private Vector2 input;
    
    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.2f, 0.3f, 0.8f, 1.0f);
        aspect = Size.X / (float)Size.Y;
        
        var _points = Flatten(points);
        var _points2 = Flatten(points2);
        //Es una zona de memoria (en la GPU) que guarda datos.
        //Ejemplo: Las coordenadas de los vértices que forman tu triángulo.
        
        //VBO 1
        GL.GenBuffers(1, out _vbo);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _points.Length * sizeof(float), _points, BufferUsageHint.StaticDraw);
        
        //Un VAO es como una configuración que guarda cómo se debe interpretar un buffer, una especie de receta que explica cómo leer los datos.
        //VAO 1
        GL.GenVertexArrays(1, out _vao); //Crear un VAO.
        GL.BindVertexArray(_vao); // Activarlo.
        
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); //Decir cómo leer el buffer.
        GL.EnableVertexAttribArray(0); // Habilitarlo.
        
        //VBO 2
        GL.GenBuffers(1, out _vbo2);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo2);
        GL.BufferData(BufferTarget.ArrayBuffer, _points2.Length * sizeof(float), _points2, BufferUsageHint.StaticDraw);
        
        //VAO 2
        GL.GenVertexArrays(1, out _vao2);
        GL.BindVertexArray(_vao2);
        
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        
        //Create Shader
        var vs = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vs, _vertexShader);
        GL.CompileShader(vs);
        
        var fs = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fs, _fragmentShader);
        GL.CompileShader(fs);
        
        _shaderProgram = GL.CreateProgram();

        GL.AttachShader(_shaderProgram, vs);
        GL.AttachShader(_shaderProgram, fs);
        GL.LinkProgram(_shaderProgram);
        
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
        _angle = _time;
        
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
        int colorLocation = GL.GetUniformLocation(_shaderProgram, "uColor");
        int offsetLocation = GL.GetUniformLocation(_shaderProgram, "offset");
        int angleLocation = GL.GetUniformLocation(_shaderProgram, "angle");
        int centerLocation = GL.GetUniformLocation(_shaderProgram, "center");
        int aspectLocation = GL.GetUniformLocation(_shaderProgram, "aspect");
        
        float r = (MathF.Sin(_time) + 1f) * 0.5f;
        float g = (MathF.Sin(_time + MathF.PI / 2f) + 1f) * 0.5f;
        float b = (MathF.Sin(_time + MathF.PI) + 1f) * 0.5f;
        
        GL.Uniform1(aspectLocation, aspect);
        GL.Uniform2(centerLocation, PointsCenter);
        GL.Uniform2(offsetLocation, new Vector2(offset.X, offset.Y));
        GL.Uniform4(colorLocation, new Vector4(r, g, b, 1.0f));
        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        
        GL.Uniform1(aspectLocation, aspect);
        GL.Uniform2(centerLocation, Points2Center);
        GL.Uniform2(offsetLocation, new Vector2(offset.X, offset.Y));
        GL.Uniform4(colorLocation, new Vector4(r, g, b, 1.0f));
        GL.BindVertexArray(_vao2);
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