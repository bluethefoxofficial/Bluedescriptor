// Decompiled with JetBrains decompiler

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


        var texture2D = new Texture2D(texture.width, texture.height, (TextureFormat)4, false);
        var active = RenderTexture.active;
        var renderTexture = new RenderTexture(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0.0f, 0.0f, (float)((Texture)renderTexture).width, (float)((Texture)renderTexture).height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = active;
        RenderTexture.ReleaseTemporary(renderTexture);
        return texture2D;
    }
}