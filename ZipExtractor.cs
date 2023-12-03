using System;
using System.IO;
using System.IO.Compression;
using MelonLoader;

// Token: 0x0200000D RID: 13
public class ZipExtractor
{
	// Token: 0x06000050 RID: 80 RVA: 0x00004080 File Offset: 0x00002280
	public static bool ExtractZip(string zipFilePath, string destinationPath)
	{
		bool result;
		try
		{
			bool flag = !File.Exists(zipFilePath);
			if (flag)
			{
				MelonLogger.Msg("ZIP file " + zipFilePath + " does not exist.");
				result = false;
			}
			else
			{
				bool flag2 = !Directory.Exists(destinationPath);
				if (flag2)
				{
					Directory.CreateDirectory(destinationPath);
				}
				ZipFile.ExtractToDirectory(zipFilePath, destinationPath);
				MelonLogger.Msg(string.Concat(new string[]
				{
					"ZIP file ",
					zipFilePath,
					" extracted to ",
					destinationPath,
					"."
				}));
				result = true;
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Msg("Error extracting ZIP file: " + ex.Message);
			result = false;
		}
		return result;
	}
}
