
using MelonLoader;
using System;
using System.IO;
using System.IO.Compression;

public class ZipExtractor
{
    public static bool ExtractZip(string zipFilePath, string destinationPath)
    {
        try
        {
            if (!File.Exists(zipFilePath))
            {
                MelonLogger.Msg("ZIP file " + zipFilePath + " does not exist.");
                return false;
            }

            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            ZipFile.ExtractToDirectory(zipFilePath, destinationPath);
            MelonLogger.Msg("ZIP file " + zipFilePath + " extracted to " + destinationPath + ".");
            return true;
        }
        catch (Exception ex)
        {
            MelonLogger.Msg("Error extracting ZIP file: " + ex.Message);
            return false;
        }
    }
}