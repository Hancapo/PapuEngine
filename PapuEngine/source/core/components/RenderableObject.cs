using PapuEngine.source.graphics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Texture = PapuEngine.source.graphics.Texture;

namespace PapuEngine.source.core.components;

public class RenderableObject
{
    private uint _vbo;
    private uint _vao;
    public List<VertexData> Vertices;
    private ReadOnlySpan<float> _vertices => Vertices.SelectMany(v => v.Flatten()).ToArray();
    public bool _isInitilized { get; set; }
    private bool _rendered { get; set; }
    private Texture _texture;
    private PrimitiveType _primitiveType;
    private Vector2D<float> _center => new(Vertices.Average(p => p.Position.X), Vertices.Average(p => p.Position.Y));
    public Vector2D<float> Center => _center;
    private GL _gl;

    public RenderableObject(GL gl, List<VertexData> vertices, Texture texture,
        PrimitiveType pt = PrimitiveType.Triangles)
    {
        Vertices = vertices;
        _texture = texture;
        _primitiveType = pt;
        _gl = gl;
    }

    public void Initialize()
    {
        //Load VBO
        _vbo = _gl.GenBuffers(1);
        _gl.BindBuffer(GLEnum.ArrayBuffer, _vbo);
        _gl.BufferData(GLEnum.ArrayBuffer, _vertices, GLEnum.StaticDraw);

        //Load VAO
        _gl.GenVertexArrays(1, out _vao);
        _gl.BindVertexArray(_vao);

        //Read vertices position
        _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float),
            0); //Read the first 3 floats as position.
        _gl.EnableVertexAttribArray(0);

        //Read vertices UVs
        _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        _gl.EnableVertexAttribArray(1);

        _isInitilized = true;
    }

    public void Draw()
    {
        if (!_isInitilized) Console.WriteLine("Renderable object not initialized");
        _texture.Bind();
        _gl.BindVertexArray(_vao);
        _gl.DrawArrays(_primitiveType, 0, (uint)Vertices.Count);
        _rendered = true;
    }
}