
using System;
using UnityEngine;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class MemoryCleaner
    {
        public void ClearRAM()
        {
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }
    }
}
