
using TMPro;
using UnityEngine;

namespace Bluedescriptor_Rewritten.Classes
{
    public static class TMPAssetFinder
    {
        public static TMP_FontAsset FindFontAssetByName(string fontAssetName)
        {
            foreach (TMP_FontAsset fontAssetByName in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
            {
                if ((fontAssetByName).name == fontAssetName)
                    return fontAssetByName;
            }

            return null;
        }
    }
}