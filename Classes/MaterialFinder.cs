using UnityEngine;

namespace Bluedescriptor_Rewritten.Classes
{
    public static class MaterialFinder
    {
        public static Material FindMaterialByName(string materialName)
        {
            // Get all loaded materials
            Material[] materials = Resources.FindObjectsOfTypeAll<Material>();

            // Search for the material with the specified name
            foreach (Material mat in materials)
            {
                if (mat.name == materialName)
                {
                    return mat;
                }
            }

            // Return null if no material with the specified name is found
            return null;
        }
    }
}