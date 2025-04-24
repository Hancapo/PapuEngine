using nkast.Aether.Physics2D.Dynamics;
using PapuEngine.source.core.components;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Shader = PapuEngine.source.graphics.Shader;

namespace PapuEngine.source.core.@base;

public abstract class BaseEntity
{
    public virtual RenderableObject Ro { get; set; }
    public virtual Shader Shader { get;set; }
    public virtual string Name { get; set; }
    public virtual bool IsVisible { get; set; }
    public virtual Body physicsBody { get; set; }

    public virtual World PhysicsWorld { get; set; }
    public virtual float Scale { get; set; }
    public virtual float Aspect { get; set; }
    public virtual Vector2D<float> UvOffset { get; set; } = Vector2D<float>.One;
    public virtual GL GLContext { get; set; }
    
    public bool IsActive { get; set; } = true;

    public abstract void Update(float deltaTime);

    public abstract void Render(float deltaTime, float aspect);
}