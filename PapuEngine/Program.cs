using System.Drawing;
using System.Numerics;
using ImGuiNET;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Dynamics;
using PapuEngine.source.core.@base;
using PapuEngine.source.core.components;
using PapuEngine.source.graphics;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using PVector2 = nkast.Aether.Physics2D.Common.Vector2;
using Texture = PapuEngine.source.graphics.Texture;
using Window = Silk.NET.Windowing.Window;

namespace PapuEngine;

public class Program
{
    private static IWindow window;
    private static IInputContext input;
    private static ImGuiController imGui;
    private static GL _gl;

    private static List<Entity> _sceneEntities = [];
    public static float _speed = 1f;
    public static Vector2D<float> vel = Vector2D<float>.Zero;
    private static float _aspect;
    private static World _physicsWorld = new();
    
    private static string shadersPath = "assets/shaders/";
    private static string texturesPath = "assets/textures/";

    private static List<(string, string, string)> _shaderList =
    [
        ("basic_textured", $"{shadersPath}basic_tex.vert", $"{shadersPath}basic_tex.frag")
    ];

    public static void Main(params string[] args)
    {
        WindowOptions options = WindowOptions.Default;
        options.Title = "PapuEngine 2D";
        options.Size = new Vector2D<int>(1280, 720);
        window = Window.Create(options);

        window.Load += OnLoad;
        window.Update += OnUpdateFrame;
        window.Render += OnRenderFrame;
        window.Resize += OnResize;

        window.Run();
    }


    private static List<VertexData> _playerGeom =
    [
        new()
        {
            Position = new Vector3D<float>(-0.1f, -0.1f, 0.0f),
            TexCoord = new Vector2D<float>(0f, 0f),
        },
        new()
        {
            Position = new Vector3D<float>(0.1f, -0.1f, 0.0f),
            TexCoord = new Vector2D<float>(1f, 0f),
        },
        new()
        {
            Position = new Vector3D<float>(-0.1f, 0.1f, 0.0f),
            TexCoord = new Vector2D<float>(0f, 1f),
        },
        new()
        {
            Position = new Vector3D<float>(0.1f, 0.1f, 0.0f),
            TexCoord = new Vector2D<float>(1f, 1f),
        }
    ];

    private static List<VertexData> _backgroundQuad =
    [
        new()
        {
            Position = new Vector3D<float>(-40f, -1f, 0f),
            TexCoord = new Vector2D<float>(0f, 0f),
        },
        new()
        {
            Position = new Vector3D<float>(40f, -1f, 0f),
            TexCoord = new Vector2D<float>(40f, 0f),
        },
        new()
        {
            Position = new Vector3D<float>(-40f, 1f, 0f),
            TexCoord = new Vector2D<float>(0f, 1f),
        },
        new()
        {
            Position = new Vector3D<float>(40f, 1f, 0f),
            TexCoord = new Vector2D<float>(40f, 1f),
        }
    ];


    private static void OnLoad()
    {
        input = window.CreateInput();

        _gl = window.CreateOpenGL();
        imGui = new ImGuiController(_gl, window, input);
        
        ImGui.StyleColorsClassic();

        _gl.ClearColor(Color.Aquamarine);
        _aspect = (uint)window.Size.X / (float)(uint)window.Size.Y;

        _physicsWorld.Gravity = PVector2.Zero;

        foreach (var shader in _shaderList)
        {
            ShaderManager.Load(shader.Item1, shader.Item2, shader.Item3, _gl);
        }

        var bgRender = new RenderableObject(
            _backgroundQuad,
            new Texture($"{texturesPath}Cave_01.png", _gl,
                true,
                false,
                TextureMinFilter.Nearest,
                TextureMagFilter.Nearest),
            _gl,
            PrimitiveType.TriangleStrip);

        var playerRender = new RenderableObject(
            _playerGeom,
            new Texture($"{texturesPath}pearto.png", _gl, default, true),
            _gl,
            PrimitiveType.TriangleStrip);

        var playerEnt = new Entity
        {
            Shader = ShaderManager.Get("basic_textured"),
            RenderObj = playerRender,
            Name = "Player1",
            isControllable = true,
            Scale = 1.0f,
            physicsBody = _physicsWorld.CreateBody(new PVector2(-0f, -0.75f), default, BodyType.Dynamic),
        };

        var backgroundEnt = new Entity
        {
            Shader = ShaderManager.Get("basic_textured"),
            RenderObj = bgRender,
            Name = "SceneBG",
            Scale = 1.0f,
            physicsBody = _physicsWorld.CreateBody(),
        };

        playerEnt.physicsBody.CreateFixture(new CircleShape(0.1f, 1f));

        _sceneEntities.Add(backgroundEnt);
        _sceneEntities.Add(playerEnt);

        foreach (var entity in _sceneEntities)
        {
            entity.RenderObj.Initialize();
        }
    }

    private static void OnKeyDown(IKeyboard keyboard, Key key, int keyCode)
    {

    }

    private static void OnResize(Vector2D<int> obj)
    {
        _gl.Viewport(0, 0, (uint)window.Size.X, (uint)window.Size.Y);
        _aspect = (uint)window.Size.X / (float)(uint)window.Size.Y;
    }

    private static void CleanUnactiveEntities(List<Entity> entities)
    {
        entities.RemoveAll(e => !e.IsActive);
    }

    private static void OnUpdateFrame(double d)
    {
        imGui.Update((float)d);
        _physicsWorld.Step((float)d);

        vel = Vector2D<float>.Zero;
        var kb = input.Keyboards.FirstOrDefault();
        if (kb.IsKeyPressed(Key.W)) vel.Y += _speed;
        if (kb.IsKeyPressed(Key.S)) vel.Y -= _speed;
        if (kb.IsKeyPressed(Key.A)) vel.X -= _speed;
        if (kb.IsKeyPressed(Key.D)) vel.X += _speed;

        _sceneEntities.Where(e => e.isControllable).ToList().ForEach(x => x.Update(_speed, vel));
    }

    private static void OnRenderFrame(double d)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        foreach (var t in _sceneEntities)
        {
            t.Render(_aspect);
        }

        if (ImGui.CollapsingHeader("Entities"))
        {
            ImGui.Indent(20f);
            for (int i = 0; i < _sceneEntities.Count; i++)
            {
                ImGui.PushID(i);
                var ent = _sceneEntities[i];
                if (ImGui.CollapsingHeader(ent.Name))
                {
                    ImGui.Spacing();
                    Vector2 pos = new Vector2(ent.physicsBody.Position.X, ent.physicsBody.Position.Y);
                    float rot = ent.physicsBody.Rotation * (180f / MathF.PI);
                    if (ImGui.InputFloat2("Position", ref pos))
                    {
                        ent.physicsBody.Position = new PVector2(pos.X, pos.Y);
                    }
                    
                    ImGui.BeginDisabled();
                    ImGui.Checkbox("Active", ref ent.IsActive);
                    ImGui.EndDisabled();
                    ImGui.Checkbox("Visible", ref ent.IsVisible);
                    if (ImGui.InputFloat("Rotation", ref rot))
                    {
                        ent.physicsBody.Rotation = rot * (MathF.PI / 180f);
                    }
                    ImGui.InputFloat("Scale", ref ent.Scale);
                    if (ImGui.Button("Destroy")) _sceneEntities[i].IsActive = false;
                    ImGui.Spacing();
                }
                ImGui.PopID();
            }
        }
        CleanUnactiveEntities(_sceneEntities);
        ImGui.End();
        imGui.Render();
    }
}