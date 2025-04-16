using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace PapuEngine;

public class Entity
{
    public Vector2 Position;
    public Vector2 Velocity;
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

    public void Update(float deltaTime, float speed, Vector2 vel)
    {
        if (!isControllable) return;
            
        if (vel.LengthSquared > 0)
        {
            Velocity = vel.Normalized() * speed;
        }
        else
        {
            Velocity = Vector2.Zero;
        }
        Position += Velocity * deltaTime;
    }

    public void Render(float aspect)
    {
        if (!IsVisible) return;
        Shader.Use();
        Shader.SetUniform("dist", Scale);
        Shader.SetUniform("angle", Rotation);
        Shader.SetUniform("aspect", aspect);
        Shader.SetUniform("center", RenderObj.Center);
        Shader.SetUniform("offset", Position);
        RenderObj.Draw();
    }
    
}