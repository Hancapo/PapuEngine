using System.Drawing;
using System.Numerics;
using ImGuiNET;
using nkast.Aether.Physics2D.Dynamics;
using PapuEngine.source.core.@base;
using PapuEngine.source.entities;
using PapuEngine.source.graphics;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using PVector2 = nkast.Aether.Physics2D.Common.Vector2;
using Window = Silk.NET.Windowing.Window;

namespace PapuEngine;

public class Game
{
    private static IWindow window;
    private static IInputContext input;
    private static ImGuiController imGui;
    private static IKeyboard? kb;
    public static GL _gl;
    private static List<BaseEntity> _sceneEntities = [];

    private static float _aspect = 0f;

    private static World _physicsWorld = new();

    private static string shadersPath = "assets/shaders/";
    private static readonly List<(string, string, string)> ShaderList =
    [
        ("basic_textured", $"{shadersPath}basic_tex.vert", $"{shadersPath}basic_tex.frag"),
        ("basic_anim_textured", $"{shadersPath}basic_tex.vert", $"{shadersPath}basic_uv_anim.frag")
    ];

    public static void Main()
    {
        var options = WindowOptions.Default;
        options.Title = "PapuEngine 2D";
        options.Size = new Vector2D<int>(1920, 1080);
        window = Window.Create(options);
        
        window.Load += OnLoad;
        window.Update += OnUpdateFrame;
        window.Render += OnRenderFrame;
        window.Resize += OnResize;
        window.Closing += OnClosing;

        window.Run();
    }
    
    private static void CleanUnactiveEntities(List<BaseEntity> entities)
    {
        entities.RemoveAll(e => !e.IsActive);
    }


    private static void OnLoad()
    {
        _physicsWorld.Gravity = PVector2.Zero;
        input = window.CreateInput();
        _gl = window.CreateOpenGL();
        imGui = new ImGuiController(_gl, window, input);

        ImGui.StyleColorsClassic();

        _gl.ClearColor(Color.Aquamarine);
        _aspect = (uint)window.Size.X / (float)(uint)window.Size.Y;

        foreach (var shader in ShaderList)
        {
            ShaderManager.Load(shader.Item1, shader.Item2, shader.Item3, _gl);
        }

        Player player1 = new(_gl, _physicsWorld, _aspect, kb = input.Keyboards.FirstOrDefault());
        AnimatedBackground bg = new(_gl, _physicsWorld, _aspect);
        _sceneEntities.Add(bg);
        _sceneEntities.Add(player1);
        _sceneEntities.ForEach(x => x.Ro.Initialize());
        
    }

    private static void OnResize(Vector2D<int> obj)
    {
        _gl.Viewport(0, 0, (uint)window.Size.X, (uint)window.Size.Y);
        _aspect = window.Size.X / (float)window.Size.Y;
    }


    private static void OnUpdateFrame(double deltaTime)
    {
        var fDeltaTime = (float)deltaTime;
        imGui.Update(fDeltaTime);
        _physicsWorld.Step(fDeltaTime);
        Player? playerEnt = _sceneEntities.FirstOrDefault(e => e is Player) as Player;
        var kb = input.Keyboards.FirstOrDefault();
        playerEnt?.Update(fDeltaTime);

    }

    private static void OnRenderFrame(double d)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        foreach (var ent in _sceneEntities)
        {
            ent.Render((float)d, _aspect);
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
                    bool a = ent.IsActive;
                    ImGui.Checkbox("Active", ref a);
                    ImGui.EndDisabled();
                    bool v = ent.IsVisible;
                    if (ImGui.Checkbox("Visible", ref v))
                    {
                        ent.IsVisible = v;
                    }
                    if (ImGui.InputFloat("Rotation", ref rot))
                    {
                        ent.physicsBody.Rotation = rot * (MathF.PI / 180f);
                    }
                    
                    if(ent is Player player)
                        ImGui.InputFloat("Speed", ref player.Speed, 0.0f, 100f);
                    float s = ent.Scale;
                    if (ImGui.InputFloat("Scale", ref s))
                    {
                        ent.Scale = s;
                    }
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

    private static void OnClosing()
    {
    }
}