using System;
using UnityEngine;

namespace Bluedescriptor_Rewritten.Classes
{
 
    internal class MemoryCleaner
    {
        public void ClearRAM()
        {
            // Force a garbage collection
            GC.Collect();
            // Unload unused assets
            Resources.UnloadUnusedAssets();
        }
    }
}