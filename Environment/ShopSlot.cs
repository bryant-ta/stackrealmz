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
    
    public override bool PlaceAndMove(Stack stack) {
        return false;
    }

    public override Transform PickUp() {
        if (GameManager.Instance.Money < price) {
            return null;
        }
        GameManager.Instance.ModifyMoney(-price);
        
        Transform ret = base.PickUp();
        
        Restock();

        return ret;
    }

    void Restock() {
        if (infiniteQuantity || remainingQuantity > 0) {
            Stack s = CardFactory.CreateStack(stockCardData);
            s.transform.position = CalculateCardPosition();
            base.PlaceAndMove(s);

            if (remainingQuantity > 0) remainingQuantity--;
        }
    }
}
