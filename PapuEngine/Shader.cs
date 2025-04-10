using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace PapuEngine;

public class Shader
{
    private int _programId;
    public int ProgramId => _programId;
    
    public readonly Dictionary<string, int> Uniforms = new();
    public Shader(string vertexPath, string fragmentPath)
    {
        var vs = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vs, vertexPath);
        GL.CompileShader(vs);
        
        var fs = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fs, fragmentPath);
        GL.CompileShader(fs);
        
        _programId = GL.CreateProgram();
        GL.AttachShader(ProgramId, vs);
        GL.AttachShader(ProgramId, fs);
        GL.LinkProgram(ProgramId);
        GL.DeleteShader(vs);
        GL.DeleteShader(fs);
        
    }
    
    public void Use()
    {
        GL.UseProgram(_programId);
    }
    
    public void SetUniform(string name, float value)
    {
        int location = GL.GetUniformLocation(_programId, name);
        GL.Uniform1(location, value);
        Uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, int value)
    {
        int location = GL.GetUniformLocation(_programId, name);
        GL.Uniform1(location, value);
        Uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, Vector2 value)
    {
        int location = GL.GetUniformLocation(_programId, name);
        GL.Uniform2(location, value);
        Uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, Vector3 value)
    {
        int location = GL.GetUniformLocation(_programId, name);
        GL.Uniform3(location, value);
        Uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, Vector4 value)
    {
        int location = GL.GetUniformLocation(_programId, name);
        GL.Uniform4(location, value);
        Uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, Matrix4 value)
    {
        int location = GL.GetUniformLocation(_programId, name);
        GL.UniformMatrix4(location, false, ref value);
        Uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, Matrix3 value)
    {
        int location = GL.GetUniformLocation(_programId, name);
        GL.UniformMatrix3(location, false, ref value);
        Uniforms.TryAdd(name, location);
    }

    public void SetUniform(string name, Matrix2 value)
    {
        int location = GL.GetUniformLocation(_programId, name);
        GL.UniformMatrix2(location, false, ref value);
        Uniforms.TryAdd(name, location);
    }
    
}