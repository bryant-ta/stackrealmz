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

    public override Transform PickUp(bool isPlayerCalled = false) {
        if (GameManager.Instance.Money < price) return null;
        GameManager.Instance.ModifyMoney(-price);
        
        Transform ret = base.PickUp();
        if (ret == null) return null;
        
        Restock();

        return ret;
    }

    void Restock() {
        if (infiniteQuantity || remainingQuantity > 0) {
            Stack s = CardFactory.CreateStack(transform.position, stockCardData);
            base.PlaceAndMove(s);

            if (remainingQuantity > 0) remainingQuantity--;
        } else {
            isLocked = true;
        }
    }
}
