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

    private List<Entity> _sceneEntities = [];

    private float _speed = 1f;
    private float _aspect;
    
    private List<(string, string, string)> _shaderList = [
        ("basic_textured", "shaders/basic_tex.vert", "shaders/basic_tex.frag")
    ];

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.8f, 0.3f, 0.2f, 1.0f);

        foreach (var shader in _shaderList)
        {
            ShaderManager.Load(shader.Item1, shader.Item2, shader.Item3);
        }
        
        var bgRender = new RenderableObject(
            BackgroundQuad,
            new Texture("textures/Cave_01.png",
                true,
                TextureMinFilter.Nearest,
                TextureMagFilter.Nearest),
            PrimitiveType.TriangleStrip);
        
        var playerRender = new RenderableObject(
            PlayerGeom,
            new Texture("textures/pearto.png"));

        var playerEnt = new Entity
        {
            IsStatic = false,
            Shader = ShaderManager.Get("basic_textured"),
            RenderObj = playerRender,
            Name = "Player1",
            isControllable = true,
            Scale = 1.0f
        };

        var backgroundEnt = new Entity
        {
            IsStatic = true,
            Shader = ShaderManager.Get("basic_textured"),
            RenderObj = bgRender,
            Name = "SceneBG",
            Scale = 1.0f
        };
        
        _sceneEntities.Add(backgroundEnt);
        _sceneEntities.Add(playerEnt);
        foreach (var entity in _sceneEntities)
        {
            entity.RenderObj.Initialize();
        }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
        _aspect = Size.X / (float)Size.Y;
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        var vel = Vector2.Zero;
        if (KeyboardState.IsKeyDown(Keys.W)) vel.Y += _speed;
        if (KeyboardState.IsKeyDown(Keys.S)) vel.Y -= _speed;
        if (KeyboardState.IsKeyDown(Keys.A)) vel.X -= _speed;
        if (KeyboardState.IsKeyDown(Keys.D)) vel.X += _speed;
        _sceneEntities.Where(e => e.isControllable).ToList().ForEach(x => x.Update((float)args.Time, _speed, vel));
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        foreach (var entity in _sceneEntities)
        {
            entity.Render(_aspect);
        }

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