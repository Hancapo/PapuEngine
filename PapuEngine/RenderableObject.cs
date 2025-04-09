using OpenTK.Graphics.OpenGL4;

namespace PapuEngine;

public class RenderableObject
{
    private int _vbo;
    private int _vao;
    private int _textureId;
    public List<VertexData> Vertices;
    private float[] _vertices => Vertices.SelectMany(v => v.Flatten()).ToArray();
    private bool _isInitilized;
    private bool _rendered;
    public string TexturePath;
    public PrimitiveType PrimitiveType;
    public PixelFormat PxFormat;
    public TextureMinFilter MinFt;
    public TextureMagFilter MagFt;
    public bool RepeatTex;

    public void Initialize()
    {
        _textureId = ImageHelper.LoadImage(TexturePath, RepeatTex, PxFormat, MinFt, MagFt);
        
        //Load VBO
        GL.GenBuffers(1, out _vbo);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
        
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
        _rendered = true;
    }
}