
using Silk.NET.Maths;

namespace PapuEngine.source.graphics;

public struct VertexData
{
    public Vector3D<float> Position;
    public Vector2D<float> TexCoord;
    public float[] Flatten()
    {
        return [Position.X, Position.Y, Position.Z, TexCoord.X, TexCoord.Y];
    }
}