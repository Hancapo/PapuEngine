namespace PapuEngine;

public abstract class ShaderManager
{
    private static Dictionary<string, Shader> _shaders = [];
    
    public static void Load(string name, string vertexPath, string fragmentPath)
    {
        _shaders[name] = new Shader(File.ReadAllText(vertexPath), File.ReadAllText(fragmentPath));
    }
    public static Shader Get(string name) => _shaders[name];

}