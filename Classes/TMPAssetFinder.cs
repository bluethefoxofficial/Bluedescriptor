using UnityEngine;
using TMPro;

namespace Bluedescriptor_Rewritten.Classes
{
    public static class TMPAssetFinder
    {
        public static TMP_FontAsset FindFontAssetByName(string fontAssetName)
        {
            // Get all loaded TMP_FontAssets
            TMP_FontAsset[] fontAssets = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();

            // Search for the font asset with the specified name
            foreach (TMP_FontAsset fontAsset in fontAssets)
            {
                if (fontAsset.name == fontAssetName)
                {
                    return fontAsset;
                }
            }

            // Return null if no font asset with the specified name is found
            return null;
        }
    }
}
