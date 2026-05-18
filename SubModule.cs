using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace LowbornTweaks
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            var harmony = new Harmony("com.lowborntweaks.patch");
            harmony.PatchAll();
        }
    }
}