using ABI_RC.Core.UI;
using BTKUILib;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class ThemeEngine
    {
        public ThemeEngine()
        {
            var path = Path.Combine(new utils().GetAssemblyDirectory(), "bluedescriptor", "themes");

            if (Directory.Exists(path))
                return;

            Directory.CreateDirectory(path);
        }

        public string[] GetInstalledThemes()
        {
            var path = Path.Combine(new utils().GetAssemblyDirectory(), "bluedescriptor", "themes");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return Directory.GetDirectories(path).Select<string, string>(new Func<string, string>(Path.GetFileName)).ToArray<string>();
        }

        public void ApplyThemeQM(string themeName)
        {
            var str1 = Path.Combine(new utils().GetAssemblyDirectory(), "bluedescriptor", "themes", themeName);

            if (!Directory.Exists(str1))
            {
                CohtmlHud.Instance.ViewDropText("OH?", "This theme appears to not be installed");
            }
            else
            {
                var path = Path.Combine(str1, "QM.css");

                if (!File.Exists(path))
                {
                    CohtmlHud.Instance.ViewDropText("OH?", "This theme appears to not be installed. Missing theme CSS file");
                }
                else
                {
                    var str2 = Regex.Replace(File.ReadAllText(path), "url\\(([^)]+)\\)", "url(\"" + str1 + "/$1\")");
                    File.WriteAllText(Path.Combine(str1, "QM-DEBUG.CSS"), str2);
                    QuickMenuAPI.InjectCSSStyle("");
                    QuickMenuAPI.InjectCSSStyle(str2);
                    CohtmlHud.Instance.ViewDropText("Pretty", themeName + " is now applied.");
                }
            }
        }
    }
}