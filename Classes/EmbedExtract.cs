
using MelonLoader;
using System.IO;
using System.Reflection;

public class EmbedExtract
{
    public static bool ExtractResource(string resourceName, string outputPath)
    {
        if (File.Exists(outputPath))
        {
            MelonLogger.Msg("File " + outputPath + " already exists. Extraction aborted.");
            return false;
        }

        using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
        {
            if (manifestResourceStream == null)
            {
                MelonLogger.Msg("Resource " + resourceName + " not found.");
                return false;
            }

            var directoryName = Path.GetDirectoryName(outputPath);

            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            using (FileStream destination = new FileStream(outputPath, FileMode.Create))
                manifestResourceStream.CopyTo(destination);
        }

        MelonLogger.Msg("Resource " + resourceName + " extracted to " + outputPath + ".");
        return true;
    }
}