using System.Numerics;
using nkast.Aether.Physics2D.Dynamics;
using PapuEngine.source.core.@base;
using PapuEngine.source.core.components;
using PapuEngine.source.core.utils;
using PapuEngine.source.graphics;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using PhyVector2 = nkast.Aether.Physics2D.Common.Vector2;
using Shader = PapuEngine.source.graphics.Shader;
using Texture = PapuEngine.source.graphics.Texture;

namespace PapuEngine.source.entities;

public sealed class Player : BaseEntity
{
    
    private readonly Body _physicsBody;
    private readonly RenderableObject _ro;
    public float _aspect;
    public IKeyboard _kb;
    private bool _canPressSpace = true;
    private float _cooldownTimer = 0f;
    
    public float Speed = 1f;
    public override Shader Shader => ShaderManager.Get("basic_textured");
    public override string Name => "Player1";
    public override Body physicsBody => _physicsBody;

    public new bool IsVisible { get; set; } = true;
    public override RenderableObject Ro => _ro;

    public override float Aspect => _aspect;

    public Vector2D<float> Vel = Vector2D<float>.Zero;

    public override void Update(float deltaTime)
    {
        if (!_canPressSpace)
        {
            _cooldownTimer -= deltaTime;
            if (_cooldownTimer <= 0f)
                _canPressSpace = true;
        }
        
        
        var v = new PhyVector2(Vel.X, Vel.Y);
        
        if (Vel.LengthSquared > 0)
        {
            v.Normalize();
            physicsBody.LinearVelocity = v * Speed;
        }
        else
        {
            physicsBody.LinearVelocity = PhyVector2.Zero;
        }
        
        Vel = Vector2D<float>.Zero;
        
        if (_kb.IsKeyPressed(Key.W)) Vel.Y += Speed;
        if (_kb.IsKeyPressed(Key.S)) Vel.Y -= Speed;
        if (_kb.IsKeyPressed(Key.A)) Vel.X -= Speed;
        if (_kb.IsKeyPressed(Key.D)) Vel.X += Speed;
        
        if (_kb.IsKeyPressed(Key.Space) && _canPressSpace)
        {
            Console.WriteLine("Space was pressed");
            _canPressSpace   = false;
            _cooldownTimer   = 0.5f;
            // tu lógica de disparo aquí
        }
    }

    public Player(GL glContext, World physicsWorld, float aspect, IKeyboard kb)
    {
        PhysicsWorld = physicsWorld;
        _aspect = aspect;
        _kb = kb;
        GLContext = glContext;
        _physicsBody = PhysicsWorld.CreateRectangle(
            width: 0.1f, height: 0.1f, density: 1f,
            position: new PhyVector2(0, -0.75f),
            rotation: 0, BodyType.Dynamic
        );
        
        var tex = new Texture(glContext, "assets/textures/mainship_base.png",
            repeat: false, transparent: true,
            min: TextureMinFilter.Nearest,
            mag: TextureMagFilter.Nearest);
        _ro = new RenderableObject(glContext,
            GeomHelper.Create2DBox(0.1f),
            tex,
            PrimitiveType.TriangleStrip
        );
    }

    public override void Render(float deltaTime, float aspect)
    {
        if (!IsVisible) return;
        Shader.Use();
        Shader.SetUniform("dist", Scale);
        Shader.SetUniform("angle", physicsBody.Rotation);
        Shader.SetUniform("aspect", aspect);
        Shader.SetUniform("center", Ro.Center);
        Shader.SetUniform("offset", physicsBody.Position);
        Ro.Draw();
    }
}