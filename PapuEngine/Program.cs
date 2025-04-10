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

    private RenderableObject bg_render, player_render;

    private float _time;
    private Vector2 offset;
    private float speed = 1f;
    private float aspect;
    private Vector2 input;

    private Shader basicShader1;

    protected override void OnLoad()
    {
        base.OnLoad();
        ShaderManager.Load("basic_textured", "shaders/simple.vert", "shaders/simple.frag");
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
        
        basicShader1 = ShaderManager.Get("basic_textured");
        basicShader1.SetUniform("tex", 0);


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
        
        basicShader1.Use();
        
        basicShader1.SetUniform("dist", 1.0f);
        basicShader1.SetUniform("offset", Vector2.Zero);
        basicShader1.SetUniform("angle", 0.0f);
        basicShader1.SetUniform("center", Vector2.Zero);
        bg_render.Draw(PrimitiveType.TriangleStrip);

        basicShader1.SetUniform("dist", 1.0f);
        basicShader1.SetUniform("aspect", aspect);
        basicShader1.SetUniform("center", PointsCenter);
        basicShader1.SetUniform("offset", new Vector2(offset.X, offset.Y));
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