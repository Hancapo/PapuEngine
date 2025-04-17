using nkast.Aether.Physics2D.Dynamics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PhyVector2 = nkast.Aether.Physics2D.Common.Vector2;


namespace PapuEngine;

public class Entity
{
    public RenderableObject RenderObj;
    public Shader Shader;
    public float Rotation = 0f;
    public float Scale = 1f;
    public string Name = "Entity";
    public bool IsActive = true;
    public bool IsStatic = false;
    public bool isControllable = false;
    public bool isCollidable = false;
    public bool IsVisible = true;
    public Body physicsBody;

    public void Update(float deltaTime, float speed, Vector2 vel)
    {
        if (!isControllable || physicsBody == null) return;
            
        if (vel.LengthSquared > 0)
        {
            physicsBody.LinearVelocity = MiscHelper.ConvertVectorToAetherVector(vel.Normalized()) * speed;
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
        Shader.SetUniform("angle", Rotation);
        Shader.SetUniform("aspect", aspect);
        Shader.SetUniform("center", RenderObj.Center);
        Shader.SetUniform("offset", physicsBody.Position);
        RenderObj.Draw();
    }
    
}