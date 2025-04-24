using System.Numerics;
using PapuEngine.source.core.utils;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using PhyVector2 = nkast.Aether.Physics2D.Common.Vector2;

namespace PapuEngine.source.graphics;

public class Shader
{
    private uint _programId;
    private GL _gl;
    private readonly Dictionary<string, int> _uniforms = new();
    
    public Shader(GL gl, string vertexPath, string fragmentPath)
    {
        _gl = gl;
        var vs = _gl.CreateShader(ShaderType.VertexShader);
        _gl.ShaderSource(vs, vertexPath);
        _gl.CompileShader(vs);

        var fs = _gl.CreateShader(ShaderType.FragmentShader);
        _gl.ShaderSource(fs, fragmentPath);
        _gl.CompileShader(fs);

        _programId = _gl.CreateProgram();
        _gl.AttachShader(_programId, vs);
        _gl.AttachShader(_programId, fs);
        _gl.LinkProgram(_programId);
        _gl.DeleteShader(vs);
        _gl.DeleteShader(fs);
    }

    public void Use()
    {
        _gl.UseProgram(_programId);
    }

    public void SetUniform(string name, float value)
    {
        int location = _gl.GetUniformLocation(_programId, name);
        _gl.Uniform1(location, value);
        _uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, int value)
    {
        int location = _gl.GetUniformLocation(_programId, name);
        _gl.Uniform1(location, value);
        _uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, Vector2D<float> value)
    {
        int location = _gl.GetUniformLocation(_programId, name);
        _gl.Uniform2(location, value.X, value.Y);
        _uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, PhyVector2 value)
    {
        int location = _gl.GetUniformLocation(_programId, name);
        _gl.Uniform2(location, value.X, value.Y);
        _uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, Vector3D<float> value)
    {
        int location = _gl.GetUniformLocation(_programId, name);
        _gl.Uniform3(location, value.X, value.Y, value.Z);
        _uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, Vector4 value)
    {
        int location = _gl.GetUniformLocation(_programId, name);
        _gl.Uniform4(location, value);
        _uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, Matrix2X2<float> value)
    {
        int location = _gl.GetUniformLocation(_programId, name);
        _gl.UniformMatrix2(location, false, MiscHelper.GetMatrixAsSpanUnsafe(value));
        _uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, Matrix3X3<float> value)
    {
        int location = _gl.GetUniformLocation(_programId, name);
        _gl.UniformMatrix3(location, false, MiscHelper.GetMatrixAsSpanUnsafe(value));
        _uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, Matrix4X4<float> value)
    {
        int location = _gl.GetUniformLocation(_programId, name);
        _gl.UniformMatrix4(location, false, MiscHelper.GetMatrixAsSpanUnsafe(value));
        _uniforms.TryAdd(name, location);
    }
}