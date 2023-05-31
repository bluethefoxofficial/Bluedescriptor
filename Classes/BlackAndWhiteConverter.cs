using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bluedescriptor_Rewritten.Classes
{
    public class BlackAndWhiteConverter
    {
        public Texture2D TextureToTexture2D(Texture texture)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(texture, renderTexture);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTexture;

            Texture2D texture2D = new Texture2D(texture.width, texture.height);
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTexture);

            return texture2D;
        }

        public Texture2D ConvertToBlackAndWhite(Texture2D sourceTexture)
        {
            // Create a new texture with the same dimensions as the source texture
            Texture2D convertedTexture = new Texture2D(sourceTexture.width, sourceTexture.height);

            // Loop through all pixels in the source texture
            for (int x = 0; x < sourceTexture.width; x++)
            {
                for (int y = 0; y < sourceTexture.height; y++)
                {
                    // Get the color of the pixel
                    Color sourceColor = sourceTexture.GetPixel(x, y);

                    // Convert the color to HSB (Hue, Saturation, Brightness)
                    Color.RGBToHSV(sourceColor, out _, out _, out float brightness);

                    // Convert the brightness value to grayscale
                    Color grayscaleColor = new Color(brightness, brightness, brightness, sourceColor.a);

                    // Set the pixel in the converted texture to the grayscale color
                    convertedTexture.SetPixel(x, y, grayscaleColor);
                }
            }

            // Apply the modifications to the converted texture
            convertedTexture.Apply();

            return convertedTexture;
        }

    }
}
