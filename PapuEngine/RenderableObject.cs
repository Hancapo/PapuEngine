using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PapuEngine;

public class RenderableObject(List<VertexData> vertices, Texture texture, PrimitiveType pt = PrimitiveType.Triangles)
{
    private int _vbo;
    private int _vao;
    public List<VertexData> Vertices = vertices;
    private float[] _vertices => Vertices.SelectMany(v => v.Flatten()).ToArray();
    private bool _isInitilized;
    private bool _rendered;
    private Texture _texture = texture;
    private PrimitiveType _primitiveType = pt;
    private Vector2 _center => new Vector2(Vertices.Average(p => p.Position.X), Vertices.Average(p => p.Position.Y));
    public Vector2 Center => _center;

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

    public void Draw()
    {
        if (!_isInitilized)
            throw new Exception("Object not initialized");
        _texture.Bind();
        GL.BindVertexArray(_vao);
        GL.DrawArrays(_primitiveType, 0, Vertices.Count);
        _rendered = true;
    }
}