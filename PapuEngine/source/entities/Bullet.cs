using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Common.PhysicsLogic;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using PapuEngine.source.core.@base;
using PapuEngine.source.core.components;
using PapuEngine.source.core.utils;
using PapuEngine.source.graphics;
using Silk.NET.OpenGL;
using Texture = PapuEngine.source.graphics.Texture;

namespace PapuEngine.source.entities;

public sealed class Bullet : BaseEntity
{
    private const float BulletSpeed = 2f;
    public Bullet(GL glContext, World physicsWorld, float aspect, Player player)
    {
        PhysicsWorld = physicsWorld;
        Aspect = aspect;
        IsVisible = true;
        Scale = 1.0f;
        GLContext = glContext;
        Shader = ShaderManager.Get("basic_textured");
        Name = "Bullet";

        Ro = new RenderableObject(
            GLContext,
            GeomHelper.Create2DBox(0.015f),
            new Texture(GLContext,
                "assets/textures/bullet.png",
                repeat: false,
                transparent: true,
                min: TextureMinFilter.Nearest,
                mag: TextureMagFilter.Nearest),
            PrimitiveType.TriangleStrip
        );
        
        Ro.Initialize();
        PhysicsBody = PhysicsWorld.CreateRectangle(
            width: 0.015f, height: 0.015f, density: 0f,
            position: new Vector2(player.PhysicsBody.Position.X, player.PhysicsBody.Position.Y + 0.1f),
            rotation: 0, BodyType.Dynamic
        );
        PhysicsBody.OnCollision += OnBulletCollision;
    }

    private bool OnBulletCollision(Fixture sender, Fixture other, Contact contact)
    {
        Console.WriteLine("Bullet collision");
        return true;
    }

    public override void Update(float deltaTime)
    {
        PhysicsBody.LinearVelocity = new Vector2(0f, BulletSpeed);

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