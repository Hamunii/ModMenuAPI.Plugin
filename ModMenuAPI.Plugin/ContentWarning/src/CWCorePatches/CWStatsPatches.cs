using ModMenuAPI.ModMenuItems;

namespace ModMenuAPI.Plugin.CW.CorePatches;

class CWStatsPatches
{
    internal static void Init()
    {
        new ModMenu("Stats")
            .RegisterItem(new SetMoneyAction())
            .RegisterItem(new ResetMoneyAction())

            .RegisterItem(new SetMetaCoinsAction())
            .RegisterItem(new ResetMetaCoinsAction())

            .RegisterItem(new NextDayAction())
            .RegisterItem(new FulfillQuotaAction());
    }
}

class SetMoneyAction() : MMButtonAction("Set Money")
{
    protected override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.Money = 100000000;
        SurfaceNetworkHandler.RoomStats.OnStatsUpdated();
    }
}

class ResetMoneyAction() : MMButtonAction("Reset Money")
{
    protected override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.Money = 0;
        SurfaceNetworkHandler.RoomStats.OnStatsUpdated();
    }
}

class SetMetaCoinsAction() : MMButtonAction("Set Meta Coins")
{
    protected override void OnClick()
    {
        MetaProgressionHandler.SetMetaCoins(100000000);
    }
}

class ResetMetaCoinsAction() : MMButtonAction("Reset Meta Coins")
{
    protected override void OnClick()
    {
        MetaProgressionHandler.SetMetaCoins(0);
    }
}

class NextDayAction() : MMButtonAction("Next Day")
{
    protected override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.NextDay();
    }
}

class FulfillQuotaAction() : MMButtonAction("Fulfill Quota")
{
    protected override void OnClick()
    {
        SurfaceNetworkHandler.RoomStats.CurrentQuota = SurfaceNetworkHandler.RoomStats.QuotaToReach;
    }
}