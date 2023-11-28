using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class utils
    {
        public string GetAssemblyDirectory()
        {
            // Get the entry assembly
            Assembly assembly = Assembly.GetEntryAssembly();

            // If the entry assembly is null, get the calling assembly
            if (assembly == null)
                assembly = Assembly.GetCallingAssembly();

            // Get the location of the assembly file
            string assemblyPath = assembly.Location;

            // Get the directory of the assembly file
            string assemblyDirectory = Path.GetDirectoryName(assemblyPath);

            return assemblyDirectory;
        }
    }
}
