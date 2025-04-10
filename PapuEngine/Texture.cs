using OpenTK.Graphics.OpenGL4;

namespace PapuEngine;

public class Texture(
    string path,
    bool repeat = false,
    TextureMinFilter min = TextureMinFilter.Linear,
    TextureMagFilter mag = TextureMagFilter.Linear)
{
    public int Id;
    private string? _path = path;
    private bool _repeat = repeat;
    private TextureMinFilter _min = min;
    private TextureMagFilter _mag = mag;

    public void Load()
    {
        Id = ImageHelper.LoadImage(_path, _repeat, _min, _mag);
    }

    public void Bind(TextureUnit unit = TextureUnit.Texture0)
    {
        if (Id == 0)
            Load();
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Id);
    }
}