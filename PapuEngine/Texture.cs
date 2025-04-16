using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using PixelFormatSystem = System.Drawing.Imaging.PixelFormat;

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
        Id = LoadImage(_path, _repeat, _min, _mag);
    }
    
    private int LoadImage(string? path, bool repeat, 
        TextureMinFilter textureMinFilter = 
            TextureMinFilter.Linear, 
        TextureMagFilter textureMagFilter = TextureMagFilter.Linear)
    {
        var textureId = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, textureId);

        using (var image = new Bitmap(path))
        {
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            var data = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                PixelFormatSystem.Format32bppArgb);
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 
                0, PixelFormat.Bgra,PixelType.UnsignedByte, data.Scan0);
            
            image.UnlockBits(data);
        }
        
        TextureWrapMode twm = repeat ? TextureWrapMode.Repeat : TextureWrapMode.ClampToEdge;
        TextureMinFilter tminf = TextureMinFilter.Linear == textureMinFilter ? TextureMinFilter.Linear : TextureMinFilter.Nearest;
        TextureMagFilter tmf = TextureMagFilter.Linear == textureMagFilter ? TextureMagFilter.Linear : TextureMagFilter.Nearest;
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)tminf);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)tmf);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)twm);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)twm);

        return textureId;

    }

    public void Bind(TextureUnit unit = TextureUnit.Texture0)
    {
        if (Id == 0)
            Load();
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Id);
    }
}