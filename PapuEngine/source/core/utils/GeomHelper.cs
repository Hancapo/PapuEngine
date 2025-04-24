using PapuEngine.source.graphics;
using Silk.NET.Maths;

namespace PapuEngine.source.core.utils;

public static class GeomHelper
{
    public static List<VertexData> Create2DBox(float size)
    {
        var vdList = new List<VertexData>
        {
            new(position: new Vector3D<float>(-size, -size, 0.0f), texCoord: new Vector2D<float>(0f, 0f)),
            new(position: new Vector3D<float>(size, -size, 0.0f), texCoord: new Vector2D<float>(1f, 0f)),
            new(position: new Vector3D<float>(-size, size, 0.0f), texCoord: new Vector2D<float>(0f, 1f)),
            new(position: new Vector3D<float>(size, size, 0.0f), texCoord: new Vector2D<float>(1f, 1f))
        };
        return vdList;
    }

    public static List<VertexData> Create2DRectangle(float width, float height)
    {
        var vdList = new List<VertexData>
        {
            new(position:new Vector3D<float>(0.0f, 0.0f, 0.0f), texCoord: new Vector2D<float>(0f, 0f)),
            new(position:new Vector3D<float>(width, 0.0f, 0.0f), texCoord: new Vector2D<float>(1f, 0f)),
            new(position:new Vector3D<float>(0.0f, height, 0.0f), texCoord: new Vector2D<float>(0f, 1f)),
            new(position:new Vector3D<float>(width, height, 0.0f), texCoord: new Vector2D<float>(1f, 1f))
        };

        return vdList;
    }

}