using System;
using MelonLoader;
using TMPro;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class FontLogger
{
	// Token: 0x06000014 RID: 20 RVA: 0x000023F4 File Offset: 0x000005F4
	public void LogAllFonts()
	{
		Font[] array = Resources.FindObjectsOfTypeAll<Font>();
		bool flag = array.Length == 0;
		if (flag)
		{
			MelonLogger.Msg("No fonts found.");
		}
		else
		{
			MelonLogger.Msg("Listing all loaded fonts:");
			foreach (Font font in array)
			{
				MelonLogger.Msg(font.name);
			}
		}
	}

	// Token: 0x06000015 RID: 21 RVA: 0x00002450 File Offset: 0x00000650
	public void LogAllTMPFontsAndMaterials()
	{
		TMP_FontAsset[] array = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
		bool flag = array.Length == 0;
		if (flag)
		{
			MelonLogger.Msg("No TextMesh Pro fonts found.");
		}
		else
		{
			MelonLogger.Msg("Listing all loaded TextMesh Pro fonts and their materials:");
			foreach (TMP_FontAsset tmp_FontAsset in array)
			{
				MelonLogger.Msg("Font: " + tmp_FontAsset.name);
				bool flag2 = tmp_FontAsset.material != null;
				if (flag2)
				{
					MelonLogger.Msg("Material: " + tmp_FontAsset.material.name);
				}
				else
				{
					MelonLogger.Msg("No associated material found.");
				}
			}
		}
	}
}
