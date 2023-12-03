using MelonLoader;
using TMPro;
using UnityEngine;

public class FontLogger
{
    public void LogAllFonts()
    {
        Font[] objectsOfTypeAll = Resources.FindObjectsOfTypeAll<Font>();

        if (objectsOfTypeAll == null || objectsOfTypeAll.Length == 0)
        {
            MelonLogger.Msg("No fonts found.");
            return;
        }

        MelonLogger.Msg("Listing all loaded fonts:");

        foreach (Font font in objectsOfTypeAll)
            MelonLogger.Msg(font.name);
    }

    public void LogAllTMPFontsAndMaterials()
    {
        TMP_FontAsset[] objectsOfTypeAll = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();

        if (objectsOfTypeAll == null || objectsOfTypeAll.Length == 0)
        {
            MelonLogger.Msg("No TextMesh Pro fonts found.");
            return;
        }

        MelonLogger.Msg("Listing all loaded TextMesh Pro fonts and their materials:");

        foreach (TMP_FontAsset tmpFontAsset in objectsOfTypeAll)
        {
            MelonLogger.Msg("Font: " + tmpFontAsset.name);

            if (tmpFontAsset.material != null)
                MelonLogger.Msg("Material: " + tmpFontAsset.material.name);
            else
                MelonLogger.Msg("No associated material found.");
        }
    }
}
