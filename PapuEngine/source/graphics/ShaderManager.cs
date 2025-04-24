using Silk.NET.OpenGL;

namespace PapuEngine.source.graphics;

public static class ShaderManager
{
    private static Dictionary<string, Shader> _shaders = [];
    
    public static void Load(string name, string vertexPath, string fragmentPath, GL gl)
    {
        _shaders[name] = new Shader(gl, File.ReadAllText(vertexPath), File.ReadAllText(fragmentPath));
    }
    public static Shader Get(string name) => _shaders[name];

}