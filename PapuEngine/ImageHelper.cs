using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using PixelFormatSystem = System.Drawing.Imaging.PixelFormat;

namespace PapuEngine;

public abstract class ImageHelper
{
    public static int LoadImage(string? path, bool repeat, 
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
    
}