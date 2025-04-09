using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using PixelFormatSystem = System.Drawing.Imaging.PixelFormat;

namespace PapuEngine;

public abstract class ImageHelper
{
    public static int LoadImage(string path, bool repeat, 
        PixelFormat format = PixelFormat.Rgba, 
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
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        
        return textureId;
    }
    
}