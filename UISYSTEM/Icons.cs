using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bluedescriptor_Rewritten.UISYSTEM
{
    internal class Icons
    {
        bool preparedicons = true;
        public void iconsinit() {
          
                BTKUILib.QuickMenuAPI.PrepareIcon("Bluedescriptor", "bd_settings", Assembly.GetExecutingAssembly().GetManifestResourceStream("Bluedescriptor_Rewritten.res.bluedescriptorsettings.png"));
                BTKUILib.QuickMenuAPI.PrepareIcon("Bluedescriptor", "bd_explorer", Assembly.GetExecutingAssembly().GetManifestResourceStream("Bluedescriptor_Rewritten.res.bluedescriptorscenexplorer.png"));
                BTKUILib.QuickMenuAPI.PrepareIcon("Bluedescriptor", "bd_logo", Assembly.GetExecutingAssembly().GetManifestResourceStream("Bluedescriptor_Rewritten.res.BLUEDESCRIPTOR.png"));
                BTKUILib.QuickMenuAPI.PrepareIcon("Bluedescriptor", "bd_scenehistory", Assembly.GetExecutingAssembly().GetManifestResourceStream("Bluedescriptor_Rewritten.res.BD_SCENEHISTORY.png"));
                BTKUILib.QuickMenuAPI.PrepareIcon("Bluedescriptor", "bd_warn", Assembly.GetExecutingAssembly().GetManifestResourceStream("Bluedescriptor_Rewritten.res.bd_warn.png"));
                BTKUILib.QuickMenuAPI.PrepareIcon("Bluedescriptor", "bd_reconnect", Assembly.GetExecutingAssembly().GetManifestResourceStream("Bluedescriptor_Rewritten.res.reconnect.png"));
                BTKUILib.QuickMenuAPI.PrepareIcon("Bluedescriptor", "bd_rewards", Assembly.GetExecutingAssembly().GetManifestResourceStream("Bluedescriptor_Rewritten.res.Rewards.png"));
                BTKUILib.QuickMenuAPI.PrepareIcon("Bluedescriptor", "bd_themes", Assembly.GetExecutingAssembly().GetManifestResourceStream("Bluedescriptor_Rewritten.res.themes.png"));
                BTKUILib.QuickMenuAPI.PrepareIcon("Bluedescriptor", "bd_download", Assembly.GetExecutingAssembly().GetManifestResourceStream("Bluedescriptor_Rewritten.res.download.png"));
                BTKUILib.QuickMenuAPI.PrepareIcon("Bluedescriptor", "bd_trash", Assembly.GetExecutingAssembly().GetManifestResourceStream("Bluedescriptor_Rewritten.res.trash.png"));
                preparedicons = false;
            
        }
    }
}
