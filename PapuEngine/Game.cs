﻿using System.Drawing;
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

public abstract class Game
{
    private static IWindow _window;
    private static IInputContext? _input;
    private static ImGuiController _imGui;
    private static IKeyboard? _kb;
    private static GL _gl;
    private static List<BaseEntity> _sceneEntities = [];

    private static float _aspect = 0f;

    private static World _physicsWorld = new();

    private static string _shadersPath = "assets/shaders/";
    private static readonly List<(string, string, string)> ShaderList =
    [
        ("basic_textured", $"{_shadersPath}basic_tex.vert", $"{_shadersPath}basic_tex.frag"),
        ("basic_anim_textured", $"{_shadersPath}basic_tex.vert", $"{_shadersPath}basic_uv_anim.frag")
    ];

    public static void Main()
    {
        var options = WindowOptions.Default;
        options.Title = "PapuEngine 2D";
        options.Size = new Vector2D<int>(1920, 1080);
        _window = Window.Create(options);
        
        _window.Load += OnLoad;
        _window.Update += OnUpdateFrame;
        _window.Render += OnRenderFrame;
        _window.Resize += OnResize;
        _window.Closing += OnClosing;

        _window.Run();
    }
    
    private static void CleanUnactiveEntities(List<BaseEntity> entities)
    {
        entities.RemoveAll(e => !e.IsActive);
    }


    private static void OnLoad()
    {
        _physicsWorld.Gravity = PVector2.Zero;
        _input = _window.CreateInput();
        _gl = _window.CreateOpenGL();
        _imGui = new ImGuiController(_gl, _window, _input);

        ImGui.StyleColorsClassic();

        _gl.ClearColor(Color.Aquamarine);
        _aspect = (uint)_window.Size.X / (float)(uint)_window.Size.Y;

        foreach (var shader in ShaderList)
        {
            ShaderManager.Load(shader.Item1, shader.Item2, shader.Item3, _gl);
        }

        Player player1 = new(_gl, _physicsWorld, _aspect, _input.Keyboards.FirstOrDefault(), ref _sceneEntities);
        AnimatedBackground bg = new(_gl, _physicsWorld, _aspect);
        _sceneEntities.Add(bg);
        _sceneEntities.Add(player1);
        _sceneEntities.ForEach(x => x.Ro.Initialize());
        
    }

    private static void OnResize(Vector2D<int> obj)
    {
        _gl.Viewport(0, 0, (uint)_window.Size.X, (uint)_window.Size.Y);
        _aspect = _window.Size.X / (float)_window.Size.Y;
    }


    private static void OnUpdateFrame(double deltaTime)
    {
        var fDeltaTime = (float)deltaTime;
        _imGui.Update(fDeltaTime);
        _physicsWorld.Step(fDeltaTime);
        CleanUnactiveEntities(_sceneEntities);
        Player? playerEnt = _sceneEntities.FirstOrDefault(e => e is Player) as Player;
        playerEnt?.Update(fDeltaTime);
        var allBullets = _sceneEntities.OfType<Bullet>().ToList();
        if (allBullets.Count > 0)
        {
            foreach (var bullet in allBullets)
            {
                bullet.Update((float)_window.Time);
                if (bullet.PhysicsBody.Position.Y > 2f)
                {
                    bullet.IsActive = false;
                }
            }
        }

    }

    private static void OnRenderFrame(double d)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        foreach (var ent in _sceneEntities)
        {
            ent.Render((float)_window.Time, _aspect);
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
                    Vector2 pos = new Vector2(ent.PhysicsBody.Position.X, ent.PhysicsBody.Position.Y);
                    float rot = ent.PhysicsBody.Rotation * (180f / MathF.PI);
                    if (ImGui.InputFloat2("Position", ref pos))
                    {
                        ent.PhysicsBody.Position = new PVector2(pos.X, pos.Y);
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
                        ent.PhysicsBody.Rotation = rot * (MathF.PI / 180f);
                    }

                    if (ent is Player player)
                    {
                        ImGui.InputFloat("Speed", ref player.Speed, 0.0f, 100f);
                        ImGui.InputFloat("Bullet Cooldown", ref player.BulletCooldown, 0.0f, 100f);

                    }
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
        ImGui.End();
        _imGui.Render();
    }

    private static void OnClosing()
    {
    }
}