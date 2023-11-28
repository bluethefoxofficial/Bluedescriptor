using UnityEngine;

public class TextureConversion : MonoBehaviour
{
    public Texture2D ConvertTextureToTexture2D(Texture texture)
    {
        if (texture == null)
        {
            Debug.LogError("Provided texture is null");
            return null;
        }

        // Create a new Texture2D with the same dimensions as the original Texture
        var texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        // Set the active RenderTexture to a temporary RenderTexture with the same dimensions
        var currentRT = RenderTexture.active;
        var renderTexture = new RenderTexture(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);
        RenderTexture.active = renderTexture;

        // Read the pixels from the RenderTexture into the Texture2D
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        // Restore the active RenderTexture
        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture2D;
    }
}
