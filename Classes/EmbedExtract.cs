using MelonLoader;
using System.IO;
using System.Reflection;

public class EmbedExtract
{
    // Extracts an embedded resource from the assembly and saves it to a specified directory.
    public static bool ExtractResource(string resourceName, string outputPath)
    {
        // Check if the file already exists
        if (File.Exists(outputPath))
        {
            MelonLogger.Msg($"File {outputPath} already exists. Extraction aborted.");
            return false;
        }

        var assembly = Assembly.GetExecutingAssembly();

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                MelonLogger.Msg($"Resource {resourceName} not found.");
                return false;
            }

            // Ensure the directory exists
            var directoryPath = Path.GetDirectoryName(outputPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
                stream.CopyTo(fileStream);
        }

        MelonLogger.Msg($"Resource {resourceName} extracted to {outputPath}.");
        return true;
    }
}