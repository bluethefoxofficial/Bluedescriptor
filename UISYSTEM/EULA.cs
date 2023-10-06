using ABI_RC.Core.IO;
using MelonLoader;
namespace Bluedescriptor_Rewritten.UISYSTEM
{
    internal class EULA
    {
        bool checkeulastatus = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "networkeula");
        private void eulaagree()
        {
            MelonPreferences.SetEntryValue<bool>("Bluedescriptor", "networkeula", true);
            MelonPreferences.Save();
        }
        private void euladisagree()
        {
            //unload this mod
           // MelonLoader.ResolvedMelons().Mods.Remove(MelonLoader.MelonHandler.Mods.Find(x => x.Info.Name == "Blue Descriptor"));
        }
        public void eulacheck()
        {
            if (checkeulastatus == false) {
                        // public static void ShowConfirm(string title, string content, Action onYes, Action onNo = null, string yesText = "Yes", string noText = "No")
                BTKUILib.QuickMenuAPI.ShowConfirm("EULA Agreement",eulamsg(), () =>
                {
                    eulaagree();
                },() =>
                {
                }, "Agree", "Disagree(unloads mod)");
            }
        }
        string eulamsg()
        {
            //this is just to cover me legally
return @"
By using the BlueDescriptor Mod, you agree to the following terms:
    We may collect and use your username for Mod functionality.
    We collect usage information to improve the Mod and ensure compliance.
    Collected data will be kept confidential and shared only as necessary.
    We may restrict or modify certain Mod features without notice.
    The Mod and its intellectual property rights belong to the creators.
    The Mod is provided as is, and we are not liable for any issues.
    You can terminate the agreement by discontinuing Mod use.
Note: This is a simplified summary and does not cover all details. Please refer to the full agreement for complete information.
";
        }
    }
}
