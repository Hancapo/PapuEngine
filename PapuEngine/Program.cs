using System.Drawing;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Dynamics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PVector2 = nkast.Aether.Physics2D.Common.Vector2;

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
            Position = new Vector3(-0.1f, -0.1f, 0.0f),
            TexCoord = new Vector2(0f, 0f),
        },
        new()
        {
            Position = new Vector3(0.1f, -0.1f, 0.0f),
            TexCoord = new Vector2(1f, 0f),
        },
        new()
        {
            Position = new Vector3(-0.1f, 0.1f, 0.0f),
            TexCoord = new Vector2(0f, 1f),
        },
        new()
        {
            Position = new Vector3(0.1f, 0.1f, 0.0f),
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

    private List<Entity> _sceneEntities = [];

    private float _speed = 1f;
    private float _aspect;
    
    private World _physicsWorld = new World();
    
    private List<(string, string, string)> _shaderList = [
        ("basic_textured", "shaders/basic_tex.vert", "shaders/basic_tex.frag")
    ];

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.8f, 0.3f, 0.2f, 1.0f);
        _physicsWorld.Gravity = new PVector2(0);
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
            new Texture("textures/pearto.png"),
            PrimitiveType.TriangleStrip);

        var playerEnt = new Entity
        {
            IsStatic = false,
            Shader = ShaderManager.Get("basic_textured"),
            RenderObj = playerRender,
            Name = "Player1",   
            isControllable = true,
            Scale = 1.0f,
            physicsBody = _physicsWorld.CreateBody(new PVector2(-0f, -0.75f), default, BodyType.Dynamic),
        };
        playerEnt.physicsBody.CreateFixture(new CircleShape(0.1f, 1f));

        var backgroundEnt = new Entity
        {
            IsStatic = true,
            Shader = ShaderManager.Get("basic_textured"),
            RenderObj = bgRender,
            Name = "SceneBG",
            Scale = 1.0f,
            physicsBody = _physicsWorld.CreateBody(),
        };
        
        _sceneEntities.Add(backgroundEnt);
        _sceneEntities.Add(playerEnt);
        foreach (var entity in _sceneEntities)
        {
            entity.RenderObj.Initialize();
        }
    }

    private static void CleanUnactiveEntities(List<Entity> entities)
    {
        entities.RemoveAll(e => !e.IsActive);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
        _aspect = Size.X / (float)Size.Y;
    }

    private double _fpsTimer = 0;
    private double _lastFps = 0;
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        
        _fpsTimer += args.Time;
        if (_fpsTimer >= 0.4f)
        {
            _lastFps = 1.0 / args.Time;
            Title = $"PapuEngine 2D - FPS: {double.Round(_lastFps)}";
            _fpsTimer = 0;
        }
        

        
        var vel = Vector2.Zero;
        if (KeyboardState.IsKeyDown(Keys.W)) vel.Y += _speed;
        if (KeyboardState.IsKeyDown(Keys.S)) vel.Y -= _speed;
        if (KeyboardState.IsKeyDown(Keys.A)) vel.X -= _speed;
        if (KeyboardState.IsKeyDown(Keys.D)) vel.X += _speed;
        _sceneEntities.Where(e => e.isControllable).ToList().ForEach(x => x.Update((float)args.Time, _speed, vel));
        _physicsWorld.Step((float)args.Time);
        float fps = 1.0f / (float)args.Time;
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        foreach (var entity in _sceneEntities)
        {
            entity.Render(_aspect);
        }
        CleanUnactiveEntities(_sceneEntities);

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