using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;
using ModMenuAPI.ModMenuItems;

namespace ModMenuAPI.Plugin.LC.CorePatches;

class LCActionMenus
{
    const string menuAction = "Action";
    internal static EnemySpawnMenu enemySpawnInsideMenu = new("Spawn Inside Enemy >", 0);
    internal static EnemySpawnMenu enemySpawnOutsideMenu = new("Spawn Outside Enemy >", 1);
    internal static EnemySpawnMenu enemySpawnDaytimeMenu = new("Spawn Daytime Enemy >", 2);
    internal static MMButtonMenuInstantiable itemSpawnMenu = new("Spawn Item >");
    internal static void Init()
    {
        ModMenu.RegisterItem(enemySpawnInsideMenu, menuAction);
        ModMenu.RegisterItem(enemySpawnOutsideMenu, menuAction);
        ModMenu.RegisterItem(enemySpawnDaytimeMenu, menuAction);
        ModMenu.RegisterItem(new PrintEnemiesListAction(), menuAction);

        ModMenu.RegisterItem(itemSpawnMenu, menuAction);
        
        if(RoundManager.Instance is not null)
        {
            PopulateItems();
            return;
        }
        On.GameNetcodeStuff.PlayerControllerB.SpawnPlayerAnimation += PlayerControllerB_SpawnPlayerAnimation;
        
    }
    private static void PlayerControllerB_SpawnPlayerAnimation(On.GameNetcodeStuff.PlayerControllerB.orig_SpawnPlayerAnimation orig, GameNetcodeStuff.PlayerControllerB self)
    {
        orig(self);

        if (itemSpawnMenu.MenuItems.Count == 0)
        {
            PopulateItems();
        }
    }
    
    private static void PopulateItems()
    {
        foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
        {
            itemSpawnMenu.MenuItems.Add(new GiveSelfItemAction(item));
        }
    }
}

class EnemySpawnMenu(string menuName, int idx) : MMButtonMenu(menuName)
{
    public override void OnMenuOpened() => PopulateEnemies(idx);
    public override void OnMenuClosed() { }

    private void PopulateEnemies(int idx)
    {
        Plugin.Logger.LogInfo("filling list");
        this.MenuItems.Clear();

        List<SpawnableEnemyWithRarity> enemyList = null!; 
        var cl = StartOfRound.Instance.currentLevel;
        switch(idx)
        {
            case 0: enemyList = cl.Enemies; break;
            case 1: enemyList = cl.OutsideEnemies; break;
            case 2: enemyList = cl.DaytimeEnemies; break;
        }
        foreach(var enemy in enemyList)
        {
            this.MenuItems.Add(new SpawnEnemyAction(enemy));
            Plugin.Logger.LogInfo("Filled " + enemy.enemyType.enemyName);
        }
    }
}

class SpawnEnemyAction(SpawnableEnemyWithRarity enemyWithRarity) : MMButtonAction($"Spawn {enemyWithRarity.enemyType.enemyName}")
{
    private readonly SpawnableEnemyWithRarity _enemyWithRarity = enemyWithRarity;
    protected override void OnClick()
    {
        Vector3 spawnPosition = GameNetworkManager.Instance.localPlayerController.transform.position - Vector3.Scale(new Vector3(-5, 0, -5), GameNetworkManager.Instance.localPlayerController.transform.forward);
        RoundManager.Instance.SpawnEnemyGameObject(spawnPosition, 0f, -1, _enemyWithRarity.enemyType);
    }
}

class GiveSelfItemAction(Item item) : MMButtonAction($"Give {item.itemName}")
{
    private readonly Item _item = item;
    protected override void OnClick()
    {
        GameObject obj = UnityEngine.Object.Instantiate(_item.spawnPrefab, GameNetworkManager.Instance.localPlayerController.transform.position, Quaternion.identity, StartOfRound.Instance.propsContainer);
        GrabbableObject _obj = obj.GetComponent<GrabbableObject>();
        _obj.fallTime = 0f;
        _obj.NetworkObject.Spawn();
        _obj.StartCoroutine(WaitAndGrabObject(obj, _obj));
    }

    private static IEnumerator WaitAndGrabObject(GameObject obj, GrabbableObject _obj){
        // Stuff happens on the object's Start() method, so well have to wait for it to run first.
        yield return new WaitForEndOfFrame();
        _obj.InteractItem();
        PlayerControllerB self = GameNetworkManager.Instance.localPlayerController;
        if(GameNetworkManager.Instance.localPlayerController.FirstEmptyItemSlot() == -1){
            Plugin.Logger.LogInfo("GiveItemToSelf: Could not grab item, inventory full!");
            yield break;
        }
        self.twoHanded = _obj.itemProperties.twoHanded;
        self.carryWeight += Mathf.Clamp(_obj.itemProperties.weight - 1f, 0f, 10f);
        self.grabbedObjectValidated = true;
        self.GrabObjectServerRpc(_obj.NetworkObject);
        _obj.GrabItemOnClient();
        _obj.parentObject = self.localItemHolder;
        self.isHoldingObject = true;
        _obj.hasBeenHeld = true;
        _obj.EnablePhysics(false);
    }
}

class PrintEnemiesListAction() : MMButtonAction("Print Enemies")
{
    protected override void OnClick()
    {
        ListAllEnemies(true);
    }
    internal static void ListAllEnemies(bool nameOnly){
        // #if DEBUG
        // nameOnly = false;
        // #endif
        Plugin.Logger.LogInfo("-- Inside Enemies ---");
        PrintListEnemy(RoundManager.Instance.currentLevel.Enemies, nameOnly);
        Plugin.Logger.LogInfo("-- Outside Enemies --");
        PrintListEnemy(RoundManager.Instance.currentLevel.OutsideEnemies, nameOnly);
        Plugin.Logger.LogInfo("-- Daytime Enemies --");
        PrintListEnemy(RoundManager.Instance.currentLevel.DaytimeEnemies, nameOnly);
    }
    private static void PrintListEnemy(List<SpawnableEnemyWithRarity> listOfEnemies, bool nameOnly){
        foreach (var enemy_ in listOfEnemies){
            var alteredName = enemy_.enemyType.enemyName.Replace(' ','_');
            alteredName = alteredName.Replace('-', '_');
            if(nameOnly)
                Plugin.Logger.LogInfo($"{enemy_.enemyType.enemyName} | Weight: {enemy_.rarity}");
            else
                Plugin.Logger.LogInfo($"public const string {alteredName} = \"{enemy_.enemyType.enemyName}\";");
        }
        Plugin.Logger.LogInfo("---------------------");
    }
}