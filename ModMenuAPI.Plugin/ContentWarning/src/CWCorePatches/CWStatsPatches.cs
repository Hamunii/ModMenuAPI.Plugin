using ModMenuAPI.ModMenuItems;

namespace ModMenuAPI.Plugin.CW.CorePatches;

class CWStatsPatches
{
    const string menuTitle = "Stats";
    internal static void Init()
    {
        ModMenu.RegisterItem(new SetMoneyAction(), menuTitle);
        ModMenu.RegisterItem(new ResetMoneyAction(), menuTitle);

        ModMenu.RegisterItem(new SetMetaCoinsAction(), menuTitle);
        ModMenu.RegisterItem(new ResetMetaCoinsAction(), menuTitle);

        ModMenu.RegisterItem(new NextDayAction(), menuTitle);
        ModMenu.RegisterItem(new FulfillQuotaAction(), menuTitle);
    }
}

class SetMoneyAction() : MMButtonAction("Set Money")
{
    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.Money = 100000000;
        SurfaceNetworkHandler.RoomStats.OnStatsUpdated();
    }
}

class ResetMoneyAction() : MMButtonAction("Reset Money")
{
    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.Money = 0;
        SurfaceNetworkHandler.RoomStats.OnStatsUpdated();
    }
}

class SetMetaCoinsAction() : MMButtonAction("Set Meta Coins")
{
    public override void OnClick()
    {
        MetaProgressionHandler.SetMetaCoins(100000000);
    }
}

class ResetMetaCoinsAction() : MMButtonAction("Reset Meta Coins")
{
    public override void OnClick()
    {
        MetaProgressionHandler.SetMetaCoins(0);
    }
}

class NextDayAction() : MMButtonAction("Next Day")
{
    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.NextDay();
    }
}

class FulfillQuotaAction() : MMButtonAction("Fulfill Quota")
{
    public override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.CurrentQuota = SurfaceNetworkHandler.RoomStats.QuotaToReach;
    }
}