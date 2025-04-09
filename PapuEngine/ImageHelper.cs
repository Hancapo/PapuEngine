using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using PixelFormatSystem = System.Drawing.Imaging.PixelFormat;

namespace PapuEngine;

public class ImageHelper
{
    public static int LoadImage(string path)
    {
        int textureID = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, textureID);

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
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        
        return textureID;
    }
}