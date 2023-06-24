public class SellSlot : Slot
{
    public override bool PlaceAndMove(Stack stack, bool isPlayerCalled = false) {
        int sellValue = stack.TotalValue();
        if (sellValue <= 0) {
            return false;
        }
        
        GameManager.Instance.ModifyMoney(sellValue);
        
        Destroy(stack.gameObject);
        return true;
    }
}
