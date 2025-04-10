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
            TexCoord = new Vector2(0.5f, 0.75f),
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

    int _shaderProgram;

    private RenderableObject bg_render, player_render;

    private float _time;
    private Vector2 offset;
    private float speed = 1f;
    private float aspect;
    private Vector2 input;

    protected override void OnLoad()
    {
        base.OnLoad();
        
        bg_render = new RenderableObject(
            BackgroundQuad,
            new Texture("textures/Cave_01.png",
                true,
                TextureMinFilter.Nearest,
                TextureMagFilter.Nearest));
        player_render = new RenderableObject(
            PlayerGeom,
            new Texture("textures/pearto.png"));
    
        GL.ClearColor(0.2f, 0.3f, 0.8f, 1.0f);
        aspect = Size.X / (float)Size.Y;
        player_render.Initialize();
        bg_render.Initialize();
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
        bg_render.Draw(PrimitiveType.TriangleStrip);

        GL.Uniform1(distLocation, 1.0f);
        GL.Uniform1(aspectLocation, aspect);
        GL.Uniform2(centerLocation, PointsCenter);
        GL.Uniform2(offsetLocation, new Vector2(offset.X, offset.Y));
        player_render.Draw();

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