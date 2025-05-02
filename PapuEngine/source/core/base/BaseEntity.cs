using nkast.Aether.Physics2D.Dynamics;
using PapuEngine.source.core.components;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Shader = PapuEngine.source.graphics.Shader;

namespace PapuEngine.source.core.@base;

public abstract class BaseEntity
{
    public RenderableObject Ro { get; set; }
    public Shader Shader { get;set; }
    public string Name { get; set; }
    public bool IsVisible { get; set; }
    public Body PhysicsBody { get; set; }

    public World PhysicsWorld { get; set; }
    public float Scale { get; set; }
    public float Aspect { get; set; }
    public GL GLContext { get; set; }
    
    public bool IsActive { get; set; } = true;

    public abstract void Update(float deltaTime);

    public abstract void Render(float deltaTime, float aspect);
}