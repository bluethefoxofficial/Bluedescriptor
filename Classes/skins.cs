using BTKUILib.UIObjects;
using MelonLoader;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class skins
    {
        public static List<SkinInfo> GetSkinInfo()
        {
            var url = "https://raw.githubusercontent.com/bluethefoxofficial/Bluedescriptor-themedatabase/main/db.json";
            string json;

            using (WebClient client = new WebClient())
                json = client.DownloadString(url);

            var skinInfos = JsonConvert.DeserializeObject<List<SkinInfo>>(json);

            return skinInfos;
        }

        public void SkinUI(Category general)
        {
            var Skinsettings = general.AddPage("Themes", "bd_themes", "Manage theme settings", "Bluedescriptor");
            var installed = Skinsettings.AddCategory("Installed");

            var list = new List<string>();

            // look in skins directory of Blue descriptor.
            // get directory of the mod

            var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            foreach (string file in Directory.GetFiles(dir + "/bluedescriptor/skins/"))
                list.Add(file);

            foreach (string file in list)
            {
                var skin = general.AddPage(Path.GetFileNameWithoutExtension(file), "bd_skin", "Manage theme settings", "Bluedescriptor");

                // get the name of the skin
                var skinname = Path.GetFileNameWithoutExtension(file);
                // buttons for the skin, apply, delete, etc.
                var skincat = skin.AddCategory(skinname);
                var apply = skincat.AddButton("Apply", "bd_apply", "Apply the skin");
                var delete = skincat.AddButton("Delete", "bd_delete", "Delete the skin");

                apply.OnPress += delegate
                {
                    // apply the skin

                    // get the directory of the skin
                    var skindir = dir + "/bluedescriptor/skins/" + skinname;
                    // get the directory of the mod
                    var moddir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    // get the directory of the skin.json
                    var skindirjson = skindir + "/skin.json";
                    // get the directory of the mod.json
                    var moddirjson = moddir + "/bluedescriptor/skins/skin.json";

                    // copy the skin.json to the mod.json
                    File.Copy(skindirjson, moddirjson, true);

                    // apply the skin
                    MelonLogger.Msg("Applied skin " + skinname);
                };

                apply.OnPress += delegate
                {
                    // delete the skin

                    // get the directory of the skin
                    var skindir = dir + "/bluedescriptor/skins/" + skinname;
                    // delete the skin
                    Directory.Delete(skindir, true);

                    // delete skin from list
                    list.Remove(file);
                };
            }
        }

        public void Installskin(string url, string destinationFolder)
        {
            using (var client = new WebClient())
                // Download the zip file
                client.DownloadFile(url, Path.Combine(destinationFolder, "temp.zip"));

            // Extract the contents of the zip file
            ZipFile.ExtractToDirectory(Path.Combine(destinationFolder, "temp.zip"), destinationFolder);

            // Delete the temporary zip file
            File.Delete(Path.Combine(destinationFolder, "temp.zip"));
        }
    }
}