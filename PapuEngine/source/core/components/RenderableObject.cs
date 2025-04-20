using PapuEngine.source.graphics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Texture = PapuEngine.source.graphics.Texture;

namespace PapuEngine.source.core.components;

public class RenderableObject(List<VertexData> vertices, Texture texture, GL gl, PrimitiveType pt = PrimitiveType.Triangles)
{
    private uint _vbo;
    private uint _vao;
    public List<VertexData> Vertices = vertices;
    private ReadOnlySpan<float> _vertices => Vertices.SelectMany(v => v.Flatten()).ToArray();
    private bool _isInitilized;
    private bool _rendered;
    private Texture _texture = texture;
    private PrimitiveType _primitiveType = pt;
    private Vector2D<float> _center => new(Vertices.Average(p => p.Position.X), Vertices.Average(p => p.Position.Y));
    public Vector2D<float> Center => _center;
    private GL _gl = gl;

    public uint GetVbo()
    {
        return _vbo;
    }
    
    public uint GetVao()
    {
        return _vao;
    }

    public void Initialize()
    {
        //Load VBO
        _vbo = gl.GenBuffers(1);
        gl.BindBuffer(GLEnum.ArrayBuffer, _vbo);
        gl.BufferData(GLEnum.ArrayBuffer, _vertices, GLEnum.StaticDraw);
        
        //Load VAO
        gl.GenVertexArrays(1, out _vao);
        gl.BindVertexArray(_vao);
        
        //Read vertices position
        gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0); //Read first 3 floats as position.
        gl.EnableVertexAttribArray(0);
        
        //Read vertices UVs
        gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        gl.EnableVertexAttribArray(1);
        
        _isInitilized = true;
    }

    public void Draw()
    {
        if (!_isInitilized)
            throw new Exception("Object not initialized");
        _texture.Bind();
        gl.BindVertexArray(_vao);
        gl.DrawArrays(_primitiveType, 0, (uint)Vertices.Count);
        _rendered = true;
    }
}