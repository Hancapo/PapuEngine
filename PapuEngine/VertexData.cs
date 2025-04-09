using OpenTK.Mathematics;

namespace PapuEngine;

public struct VertexData
{
    public Vector3 Position;
    public Vector2 TexCoord;
    public float[] Flatten()
    {
        return [Position.X, Position.Y, Position.Z, TexCoord.X, TexCoord.Y];
    }
}