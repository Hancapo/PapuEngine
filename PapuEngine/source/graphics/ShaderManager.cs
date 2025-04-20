using Silk.NET.OpenGL;

namespace PapuEngine.source.graphics;

public abstract class ShaderManager
{
    private static Dictionary<string, Shader> _shaders = [];
    
    public static void Load(string name, string vertexPath, string fragmentPath, GL gl)
    {
        _shaders[name] = new Shader(File.ReadAllText(vertexPath), File.ReadAllText(fragmentPath), gl);
    }
    public static Shader Get(string name) => _shaders[name];

}