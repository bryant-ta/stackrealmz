using UnityEngine;

public class ShopSlot : Slot
{
    public SO_Card stockCardData;
    public int price;
    public int remainingQuantity;
    public bool infiniteQuantity;

    void Start() {
        Restock();
    }

    public override Transform PickUpHeld(bool isPlayerCalled = false, bool endCombatState = false, bool doEventInvoke = true) {
        if (GameManager.Instance.Money < price) return null;
        GameManager.Instance.ModifyMoney(-price);
        
        Transform ret = base.PickUpHeld(isPlayerCalled);
        if (ret == null) return null;
        
        Restock();

        return ret;
    }

    void Restock() {
        if (infiniteQuantity || remainingQuantity > 0) {
            SpawnCard(stockCardData);

            if (remainingQuantity > 0) remainingQuantity--;
        } else {
            canPlace = false;
            canPickUp = false;
        }
    }
}
