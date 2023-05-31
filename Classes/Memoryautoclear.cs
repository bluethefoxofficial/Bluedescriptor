using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class Memoryautoclear : MelonMod
    {
        private const float CleanupInterval = 60.0f;
        private float timeSinceLastCleanup = 0.0f;

        public override void OnFixedUpdate()
        {
            switch (MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "memorycleanup"))
            {
                case true:
                    timeSinceLastCleanup += Time.deltaTime;
                    if (timeSinceLastCleanup >= CleanupInterval)
                    {
                        CleanupMemory();
                        timeSinceLastCleanup = 0.0f;
                    }
               break;
            }
        }
        public void CleanupMemory()
        {
            MelonLogger.Msg("Performing memory cleanup.");
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            MelonLogger.Msg("Memory cleanup completed.");
        }
    }
}