using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PapuEngine.source.graphics;

public class Texture(
    GL gl,
    string path,
    bool repeat = false,
    bool transparent = false,
    TextureMinFilter min = TextureMinFilter.Linear,
    TextureMagFilter mag = TextureMagFilter.Linear)
{
    public uint Id;
    private bool _repeat = repeat;
    private TextureMinFilter _min = min;
    private TextureMagFilter _mag = mag;
    private GL _gl = gl;


    private void Load()
    {
        Id = LoadImage(path, _repeat, _min, _mag);
    }

    private uint LoadImage(string path, bool repeat = false,
        TextureMinFilter textureMinFilter =
            TextureMinFilter.Linear,
        TextureMagFilter textureMagFilter = TextureMagFilter.Linear)
    {
        var textureId = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, textureId);
        if (transparent)
            _gl.Enable(GLEnum.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        using var img = Image.Load<Rgba32>(path);
        img.Mutate(x => x.Flip(FlipMode.Vertical));
        var pixelData = new byte[img.Width * img.Height * 4];
        img.CopyPixelDataTo(pixelData);

        unsafe
        {
            fixed (byte* ptr = pixelData)
            {
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint)img.Width, (uint)img.Height,
                    0, GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
            }
        }

        TextureWrapMode twm = repeat ? TextureWrapMode.Repeat : TextureWrapMode.ClampToEdge;
        TextureMinFilter tminf = TextureMinFilter.Linear == textureMinFilter
            ? TextureMinFilter.Linear
            : TextureMinFilter.Nearest;
        TextureMagFilter tmf = TextureMagFilter.Linear == textureMagFilter
            ? TextureMagFilter.Linear
            : TextureMagFilter.Nearest;

        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)tminf);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)tmf);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)twm);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)twm);

        return textureId;
    }

    public void Bind(TextureUnit unit = TextureUnit.Texture0)
    {
        if (Id == 0)
            Load();
        _gl.ActiveTexture(unit);
        _gl.BindTexture(TextureTarget.Texture2D, Id);
    }
}