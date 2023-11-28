using ABI.CCK.Components;
using System.IO;
using System.Reflection;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class utils
    {
        public string GetAssemblyDirectory()
        {
            // Get the entry assembly
            var assembly = Assembly.GetEntryAssembly();

            // If the entry assembly is null, get the calling assembly
            if (assembly == null)
                assembly = Assembly.GetCallingAssembly();

            // Get the location of the assembly file
            var assemblyPath = assembly.Location;

            // Get the directory of the assembly file
            var assemblyDirectory = Path.GetDirectoryName(assemblyPath);

            return assemblyDirectory;
        }
        public CVRVideoPlayer[] GetAllVideoPlayers()
        {
            return UnityEngine.Object.FindObjectsOfType<CVRVideoPlayer>(true);
        }
    }
}
