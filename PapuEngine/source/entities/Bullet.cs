using nkast.Aether.Physics2D.Common;
using PapuEngine.source.core.@base;
using PapuEngine.source.core.components;
using PapuEngine.source.graphics;

namespace PapuEngine.source.entities;

public class Bullet : BaseEntity
{
    public override Shader Shader => ShaderManager.Get("basic_textured");

    public const float BulletSpeed = 2f;

    public override void Update(float deltaTime)
    {
        physicsBody.ApplyForce(new Vector2(0,BulletSpeed));
    }

    public override void Render(float deltaTime, float aspect)
    {
        
    }
}