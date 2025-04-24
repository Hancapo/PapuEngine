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
    private IKeyboard _kb;
    private bool _canPressSpace = true;
    private float _cooldownTimer = 0f;
    private List<BaseEntity> _entities;
    
    public float Speed = 1f;
    public override Shader Shader => ShaderManager.Get("basic_textured");
    public override string Name => "Player1";

    private Vector2D<float> _vel = Vector2D<float>.Zero;

    public override void Update(float deltaTime)
    {
        if (!_canPressSpace)
        {
            _cooldownTimer -= deltaTime;
            if (_cooldownTimer <= 0f)
                _canPressSpace = true;
        }
        
        
        var v = new PhyVector2(_vel.X, _vel.Y);
        
        if (_vel.LengthSquared > 0)
        {
            v.Normalize();
            PhysicsBody.LinearVelocity = v * Speed;
        }
        else
        {
            PhysicsBody.LinearVelocity = PhyVector2.Zero;
        }
        
        _vel = Vector2D<float>.Zero;
        
        if (_kb.IsKeyPressed(Key.W)) _vel.Y += Speed;
        if (_kb.IsKeyPressed(Key.S)) _vel.Y -= Speed;
        if (_kb.IsKeyPressed(Key.A)) _vel.X -= Speed;
        if (_kb.IsKeyPressed(Key.D)) _vel.X += Speed;
        
        if (_kb.IsKeyPressed(Key.Space) && _canPressSpace)
        {
            AudioPlayer ap = new("assets/sounds/sfx_weapon_singleshot1.wav");
            ap.Play();
            Bullet bullet = new Bullet(GLContext, PhysicsWorld, Aspect, this);
            _entities.Add(bullet);
            _canPressSpace   = false;
            _cooldownTimer   = 0.2f;
        }
    }

    public Player(GL glContext, World physicsWorld, float aspect, IKeyboard kb, ref List<BaseEntity> entities)
    {
        PhysicsWorld = physicsWorld;
        Aspect = aspect;
        _kb = kb;
        IsVisible = true;
        Scale = 1.0f;
        GLContext = glContext;
        _entities = entities;
        PhysicsBody = PhysicsWorld.CreateCircle(
            radius: 0.01f, density: 0.01f,
            position: new PhyVector2(0, -0.75f), BodyType.Dynamic
        );
        Ro = new RenderableObject(glContext,
            GeomHelper.Create2DBox(0.1f),
            new Texture(glContext, "assets/textures/mainship_base.png",
                repeat: false, transparent: true,
                min: TextureMinFilter.Nearest,
                mag: TextureMagFilter.Nearest),
            PrimitiveType.TriangleStrip
        );

        PhysicsBody.IgnoreGravity = true;
    }

    public override void Render(float deltaTime, float aspect)
    {
        if (!IsVisible) return;
        Aspect = aspect;
        Shader.Use();
        Shader.SetUniform("dist", Scale);
        Shader.SetUniform("angle", PhysicsBody.Rotation);
        Shader.SetUniform("aspect", aspect);
        Shader.SetUniform("center", Ro.Center);
        Shader.SetUniform("offset", PhysicsBody.Position);
        Ro.Draw();
    }
}