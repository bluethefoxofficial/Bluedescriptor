using ABI_RC.Core.UI;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class ThemeEngine
    {
        public ThemeEngine()
        {
            var themesDirectory = Path.Combine(new utils().GetAssemblyDirectory(), "bluedescriptor", "themes");

            if (!Directory.Exists(themesDirectory))
            {
                Directory.CreateDirectory(themesDirectory);
            }
        }

        public string[] GetInstalledThemes()
        {
            var themesDirectory = Path.Combine(new utils().GetAssemblyDirectory(), "bluedescriptor", "themes");
            if(!Directory.Exists(themesDirectory))
            {
                Directory.CreateDirectory(themesDirectory);
            }
            // Get all subdirectories in the themes directory
            var themeDirectories = Directory.GetDirectories(themesDirectory);

            // Extract the theme names from the directory paths
            var themeNames = themeDirectories.Select(Path.GetFileName).ToArray();

            return themeNames;
        }

        public void ApplyThemeQM(string themeName)
        {
            var themeDirectory = Path.Combine(new utils().GetAssemblyDirectory(), "bluedescriptor", "themes", themeName);

            if (!Directory.Exists(themeDirectory))
            {
                CohtmlHud.Instance.ViewDropText("OH?", "This theme appears to not be installed");
                return;
            }

            var cssFilePath = Path.Combine(themeDirectory, "QM.css");

            if (!File.Exists(cssFilePath))
            {
                CohtmlHud.Instance.ViewDropText("OH?", "This theme appears to not be installed. Missing theme CSS file");
                return;
            }

            // Get theme CSS as string
            var themeCss = File.ReadAllText(cssFilePath);

            // Update URL paths in CSS
            themeCss = Regex.Replace(themeCss, @"url\(([^)]+)\)", $"url(\"{themeDirectory}/$1\")");
            File.WriteAllText(Path.Combine(themeDirectory,"QM-DEBUG.CSS"), themeCss);
            // Inject the CSS into the UI
            BTKUILib.QuickMenuAPI.InjectCSSStyle("");
            BTKUILib.QuickMenuAPI.InjectCSSStyle(themeCss);
            CohtmlHud.Instance.ViewDropText("Pretty", $"{themeName} is now applied.");
        }
    }
}