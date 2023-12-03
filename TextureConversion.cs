using System;
using UnityEngine;

// Token: 0x0200000B RID: 11
public class TextureConversion : MonoBehaviour
{
	// Token: 0x0600003F RID: 63 RVA: 0x00003264 File Offset: 0x00001464
	public Texture2D ConvertTextureToTexture2D(Texture texture)
	{
		bool flag = texture == null;
		Texture2D result;
		if (flag)
		{
			Debug.LogError("Provided texture is null");
			result = null;
		}
		else
		{
			Texture2D texture2D = new Texture2D(texture.width, texture.height, 4, false);
			RenderTexture active = RenderTexture.active;
			RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 32);
			Graphics.Blit(texture, renderTexture);
			RenderTexture.active = renderTexture;
			texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
			texture2D.Apply();
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(renderTexture);
			result = texture2D;
		}
		return result;
	}
}
