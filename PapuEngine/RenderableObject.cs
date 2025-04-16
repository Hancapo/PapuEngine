using OpenTK.Graphics.OpenGL4;

namespace PapuEngine;

public class RenderableObject
{
    private int _vbo;
    private int _vao;
    public List<VertexData>? Vertices;
    private float[]? _vertices => Vertices?.SelectMany(v => v.Flatten()).ToArray();
    private bool _isInitilized;
    private bool _rendered;
    private Texture? _texture;

    public RenderableObject(List<VertexData>? vertices, Texture? texture)
    {
        Vertices = vertices;
        _texture = texture;
    }

    public int GetVbo()
    {
        return _vbo;
    }
    
    public int GetVao()
    {
        return _vao;
    }

    public void Initialize()
    {
        //Load VBO
        GL.GenBuffers(1, out _vbo);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices!.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
        
        //Load VAO
        GL.GenVertexArrays(1, out _vao);
        GL.BindVertexArray(_vao);
        
        //Read vertices position
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0); //Read first 3 floats as position.
        GL.EnableVertexAttribArray(0);
        
        //Read vertices UVs
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        
        
        _isInitilized = true;
    }

    public void Draw(int vertexCount, PrimitiveType primitiveType = PrimitiveType.Triangles)
    {
        if (!_isInitilized)
            throw new Exception("Object not initialized");
        _texture?.Bind();
        GL.BindVertexArray(_vao);
        GL.DrawArrays(primitiveType, 0, vertexCount);
        _rendered = true;
    }
}