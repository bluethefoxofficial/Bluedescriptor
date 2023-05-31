using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    }
}
