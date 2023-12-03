using System;
using System.IO;
using System.Reflection;
using MelonLoader;

// Token: 0x02000006 RID: 6
public class EmbedExtract
{
	// Token: 0x06000012 RID: 18 RVA: 0x000022D8 File Offset: 0x000004D8
	public static bool ExtractResource(string resourceName, string outputPath)
	{
		bool flag = File.Exists(outputPath);
		bool result;
		if (flag)
		{
			MelonLogger.Msg("File " + outputPath + " already exists. Extraction aborted.");
			result = false;
		}
		else
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(resourceName))
			{
				bool flag2 = manifestResourceStream == null;
				if (flag2)
				{
					MelonLogger.Msg("Resource " + resourceName + " not found.");
					return false;
				}
				string directoryName = Path.GetDirectoryName(outputPath);
				bool flag3 = !Directory.Exists(directoryName);
				if (flag3)
				{
					Directory.CreateDirectory(directoryName);
				}
				using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
				{
					manifestResourceStream.CopyTo(fileStream);
				}
			}
			MelonLogger.Msg(string.Concat(new string[]
			{
				"Resource ",
				resourceName,
				" extracted to ",
				outputPath,
				"."
			}));
			result = true;
		}
		return result;
	}
}
