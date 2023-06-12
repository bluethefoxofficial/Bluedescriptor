using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bluedescriptor_Rewritten.Classes
{
    internal class skins
    {
        public static List<SkinInfo> GetSkinInfo()
        {
            string url = "https://raw.githubusercontent.com/bluethefoxofficial/Bluedescriptor-themedatabase/main/db.json";
            string json;

            using (WebClient client = new WebClient())
            {
                json = client.DownloadString(url);
            }

            List<SkinInfo> skinInfos = JsonConvert.DeserializeObject<List<SkinInfo>>(json);

            return skinInfos;
        }


        public void Installskin(string url, string destinationFolder)
        {
            using (var client = new WebClient())
            {
                // Download the zip file
                client.DownloadFile(url, Path.Combine(destinationFolder, "temp.zip"));
            }

            // Extract the contents of the zip file
            ZipFile.ExtractToDirectory(Path.Combine(destinationFolder, "temp.zip"), destinationFolder);

            // Delete the temporary zip file
            File.Delete(Path.Combine(destinationFolder, "temp.zip"));
        }
    }
}
