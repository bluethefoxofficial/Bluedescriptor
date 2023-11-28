using MelonLoader;
using TMPro;
using UnityEngine;

public class FontLogger
{
    public void LogAllFonts()
    {
        var allFonts = Resources.FindObjectsOfTypeAll<Font>();

        if (allFonts.Length == 0)
        {
            MelonLogger.Msg("No fonts found.");
            return;
        }

        MelonLogger.Msg("Listing all loaded fonts:");

        foreach (Font font in allFonts)
            MelonLogger.Msg(font.name);
    }

    public void LogAllTMPFontsAndMaterials()
    {
        var allTMPFonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();

        if (allTMPFonts.Length == 0)
        {
            MelonLogger.Msg("No TextMesh Pro fonts found.");
            return;
        }

        MelonLogger.Msg("Listing all loaded TextMesh Pro fonts and their materials:");

        foreach (TMP_FontAsset font in allTMPFonts)
        {
            MelonLogger.Msg($"Font: {font.name}");

            if (font.material != null)
            {
                MelonLogger.Msg($"Material: {font.material.name}");
            }
            else
            {
                MelonLogger.Msg("No associated material found.");
            }
        }
    }
}