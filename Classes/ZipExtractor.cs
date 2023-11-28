using MelonLoader;
using System;
using System.IO;
using System.IO.Compression;

public class ZipExtractor
{
    // Extracts a ZIP file to a specified directory.
    public static bool ExtractZip(string zipFilePath, string destinationPath)
    {
        try
        {
            // Check if the ZIP file exists
            if (!File.Exists(zipFilePath))
            {
                MelonLogger.Msg($"ZIP file {zipFilePath} does not exist.");
                return false;
            }

            // Ensure the destination directory exists
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            // Extract the ZIP file
            ZipFile.ExtractToDirectory(zipFilePath, destinationPath);

            MelonLogger.Msg($"ZIP file {zipFilePath} extracted to {destinationPath}.");
            return true;
        }
        catch (Exception ex)
        {
            MelonLogger.Msg($"Error extracting ZIP file: {ex.Message}");
            return false;
        }
    }
}
