using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using ModMenuAPI.ModMenuItems;
using ModMenuAPI.Plugin.LC.CorePatches;
using MonoMod.RuntimeDetour.HookGen;

namespace ModMenuAPI.Plugin.LC;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; } = null!;
    internal static new ManualLogSource Logger { get; private set; } = null!;

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

#if DEBUG
        ModMenuAPI.HotLoadPlugin.OnLoad();
#endif
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");

        LCMiscPatches.Init();
        LCPlayerPatches.Init();
        LCActionPatches.Init();
        LCActionMenus.Init();
    }

    private void OnDestroy()
    {
        HookEndpointManager.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
        ModMenu.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
#if DEBUG
        ModMenuAPI.HotLoadPlugin.Dispose();
#endif
    }
}
