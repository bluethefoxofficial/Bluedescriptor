
using UnityEngine;

namespace Bluedescriptor_Rewritten.Classes
{
    public static class MaterialFinder
    {
        public static Material FindMaterialByName(string materialName)
        {
            foreach (Material materialByName in Resources.FindObjectsOfTypeAll<Material>())
            {
                if ((materialByName).name == materialName)
                    return materialByName;
            }

            return null;
        }
    }
}