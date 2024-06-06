using ModMenuAPI.ModMenuItems;
using UnityEngine;

namespace ModMenuAPI.Plugin.LC.CorePatches;

class LCActionPatches
{
    internal static QuickMenuManager QMM_Instance = null!;

    internal static void Init()
    {
        On.QuickMenuManager.Start += QuickMenuManager_Start;

        new ModMenu("Action")
            .RegisterItem(new TeleportSelfToEntranceAction())
            .RegisterItem(new ToggleTestRoomAction());
    }

    private static void QuickMenuManager_Start(
        On.QuickMenuManager.orig_Start orig,
        QuickMenuManager self
    )
    {
        QMM_Instance = self;
        orig(self);
    }
}

class TeleportSelfToEntranceAction() : MMButtonAction("Teleport Self To Entrance")
{
    protected override void OnClick()
    {
        var self = StartOfRound.Instance.localPlayerController;
        int id = 0; // Main entrance
        var entrances = GameObject.FindObjectsByType<EntranceTeleport>(FindObjectsSortMode.None);
        foreach (var entrance in entrances)
        {
            if (entrance.entranceId != id)
                continue;

            // IF inside, set outside, or vice-versa.
            if (self.isInsideFactory != entrance.isEntranceToBuilding)
            {
                entrance.TeleportPlayer(); // Teleport self
                return;
            }
        }
    }
}

class ToggleTestRoomAction() : MMButtonAction("Toggle Test Room")
{
    protected override void OnClick()
    {
        LCActionPatches.QMM_Instance.Debug_ToggleTestRoom();
    }
}
