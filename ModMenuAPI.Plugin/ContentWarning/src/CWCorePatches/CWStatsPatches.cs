using ModMenuAPI.ModMenuItems;

namespace PluginCW.CorePatches;

class CWStatsPatches
{
    const string menuTitle = "Stats";
    internal static void Init()
    {
        ModMenu.RegisterItem(new SetMoneyPatch(), menuTitle);
        ModMenu.RegisterItem(new ResetMoneyPatch(), menuTitle);

        ModMenu.RegisterItem(new SetMetaCoinsPatch(), menuTitle);
        ModMenu.RegisterItem(new ResetMetaCoinsPatch(), menuTitle);

        ModMenu.RegisterItem(new NextDayPatch(), menuTitle);
        ModMenu.RegisterItem(new FulfillQuotaPatch(), menuTitle);
    }
}

class SetMoneyPatch() : ModMenuButtonActionBase("Set Money")
{
    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.Money = 100000000;
        SurfaceNetworkHandler.RoomStats.OnStatsUpdated();
    }
}

class ResetMoneyPatch() : ModMenuButtonActionBase("Reset Money")
{
    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.Money = 0;
        SurfaceNetworkHandler.RoomStats.OnStatsUpdated();
    }
}

class SetMetaCoinsPatch() : ModMenuButtonActionBase("Set Meta Coins")
{
    public override void OnClick()
    {
        MetaProgressionHandler.SetMetaCoins(100000000);
    }
}

class ResetMetaCoinsPatch() : ModMenuButtonActionBase("Reset Meta Coins")
{
    public override void OnClick()
    {
        MetaProgressionHandler.SetMetaCoins(0);
    }
}

class NextDayPatch() : ModMenuButtonActionBase("Next Day")
{
    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.NextDay();
    }
}

class FulfillQuotaPatch() : ModMenuButtonActionBase("Fulfill Quota")
{
    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.CurrentQuota = SurfaceNetworkHandler.RoomStats.QuotaToReach;
    }
}