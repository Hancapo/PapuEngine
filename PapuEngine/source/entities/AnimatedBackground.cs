using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using PapuEngine.source.core.@base;
using PapuEngine.source.core.components;
using PapuEngine.source.core.utils;
using PapuEngine.source.graphics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Shader = PapuEngine.source.graphics.Shader;
using Texture = PapuEngine.source.graphics.Texture;

namespace PapuEngine.source.entities;

public sealed class AnimatedBackground : BaseEntity
{
    private readonly Body _physicsBody;
    private readonly RenderableObject _ro;
    public float _aspect;

    public override Vector2D<float> UvOffset { get; set; }

    public override Shader Shader { get; set; }

    public override Body physicsBody => _physicsBody;
    
    public override float Aspect => _aspect;
    public override string Name { get; set; }

    public override RenderableObject Ro => _ro;
    
    public AnimatedBackground(GL glContext, World physicsWorld, float aspect)
    {
        PhysicsWorld = physicsWorld;
        _aspect = aspect;
        GLContext = glContext;
        Shader = ShaderManager.Get("basic_textured");
        Name = "Background";
        
        _ro = new RenderableObject(
            GLContext,
            GeomHelper.Create2DBox(2f),
            new Texture(
                gl:GLContext,
                path:"assets/textures/space_bg.png",
                repeat:false,
                transparent:false,
                min:TextureMinFilter.Nearest,
                mag:TextureMagFilter.Nearest), 
            PrimitiveType.TriangleStrip
        );

        _physicsBody = physicsWorld.CreateBody();
    }

    public override void Update(float deltaTime)
    {
        //UvOffset = new Vector2D<float>(UvOffset.X, UvOffset.Y * deltaTime);
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
        //Shader.SetUniform("iTime", deltaTime);
        //Shader.SetUniform("iSpeed", 1.0f);
        Ro.Draw();
    }
}