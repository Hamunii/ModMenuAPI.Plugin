using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;
using ModMenuAPI.ModMenuItems;
using ModMenuAPI.ModMenuItems.BaseItems;

namespace ModMenuAPI.Plugin.LC.CorePatches;

class LCActionEnemy
{

    const string menuAction = "Action";
    internal static MMButtonContextMenuInstantiable enemySpawnItem = new("Spawn Enemy >");
    internal static MMButtonContextMenuInstantiable itemSpawnItem = new("Spawn Item >");
    internal static List<SpawnableEnemyWithRarity> allEnemiesList = new();
    internal static void Init()
    {
        ModMenu.RegisterItem(enemySpawnItem, menuAction);
        ModMenu.RegisterItem(itemSpawnItem, menuAction);
        if(RoundManager.Instance is not null)
        {
            PopulateEnemies();
            PopulateItems();
            return;
        }
        On.GameNetcodeStuff.PlayerControllerB.SpawnPlayerAnimation += PlayerControllerB_SpawnPlayerAnimation;
        
    }
    private static void PlayerControllerB_SpawnPlayerAnimation(On.GameNetcodeStuff.PlayerControllerB.orig_SpawnPlayerAnimation orig, GameNetcodeStuff.PlayerControllerB self)
    {
        orig(self);

        if (allEnemiesList.Count == 0)
        {
            PopulateEnemies();
            PopulateItems();
        }
    }
    private static void PopulateEnemies()
    {
        allEnemiesList.AddRange(RoundManager.Instance.currentLevel.Enemies);
        allEnemiesList.AddRange(RoundManager.Instance.currentLevel.OutsideEnemies);
        allEnemiesList.AddRange(RoundManager.Instance.currentLevel.DaytimeEnemies);

        foreach(var enemy in allEnemiesList)
        {
            enemySpawnItem.MenuItems.Add(new SpawnEnemyAction(enemy));
        }
    }
    private static void PopulateItems()
    {
        foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
        {
            itemSpawnItem.MenuItems.Add(new GiveSelfItemAction(item));
        }
    }
}

class SpawnEnemyAction : MMButtonAction
{
    private SpawnableEnemyWithRarity _enemyWithRarity;
    internal SpawnEnemyAction(SpawnableEnemyWithRarity enemyWithRarity) : base($"Spawn {enemyWithRarity.enemyType.enemyName}")
    {
        _enemyWithRarity = enemyWithRarity;
    }

    public override void OnClick()
    {
        Vector3 spawnPosition = GameNetworkManager.Instance.localPlayerController.transform.position - Vector3.Scale(new Vector3(-5, 0, -5), GameNetworkManager.Instance.localPlayerController.transform.forward);
        RoundManager.Instance.SpawnEnemyGameObject(spawnPosition, 0f, -1, _enemyWithRarity.enemyType);
    }
}

class GiveSelfItemAction : MMButtonAction
{
    private readonly Item _item;
    internal GiveSelfItemAction(Item item) : base($"Give {item.itemName}")
    {
        _item = item;
    }

    public override void OnClick()
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