
using ABI.CCK.Components;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class utils
    {
        public string GetAssemblyDirectory()
        {
            var assembly = Assembly.GetEntryAssembly();

            if (assembly == null)
                assembly = Assembly.GetCallingAssembly();

            return Path.GetDirectoryName(assembly.Location);
        }

        public CVRVideoPlayer[] GetAllVideoPlayers() => GameObject.FindObjectsOfType<CVRVideoPlayer>(true);
    }
}