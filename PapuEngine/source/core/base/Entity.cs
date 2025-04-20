using nkast.Aether.Physics2D.Dynamics;
using PapuEngine.source.core.components;
using PapuEngine.source.graphics;
using Silk.NET.Maths;
using PhyVector2 = nkast.Aether.Physics2D.Common.Vector2;


namespace PapuEngine.source.core.@base;

public class Entity
{
    public RenderableObject RenderObj;
    public Shader Shader;
    public float Scale = 1f;
    public string Name = "Entity";
    public bool IsActive = true;
    public bool isControllable = false;
    public bool IsVisible = true;
    public Body physicsBody;

    public void Update(float speed, Vector2D<float> vel)
    {
        if (!isControllable || physicsBody == null) return;
        
        var v = new PhyVector2(vel.X, vel.Y);
        
        if (vel.LengthSquared > 0)
        {
            v.Normalize();
            physicsBody.LinearVelocity = v * speed;
        }
        else
        {
            physicsBody.LinearVelocity = PhyVector2.Zero;
        }
        
    }

    public void Render(float aspect)
    {
        if (!IsVisible) return;
        Shader.Use();
        Shader.SetUniform("dist", Scale);
        Shader.SetUniform("angle", physicsBody.Rotation);
        Shader.SetUniform("aspect", aspect);
        Shader.SetUniform("center", RenderObj.Center);
        Shader.SetUniform("offset", physicsBody.Position);
        RenderObj.Draw();
    }
    
}