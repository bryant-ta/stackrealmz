using UnityEngine;

public class CombatSlot : Slot
{
    public Terrain terrain;
    
    public override bool PlaceAndMove(Stack stack, bool overrideIsLocked = false) {
        if (!base.PlaceAndMove(stack, overrideIsLocked)) return false;

        if (Card.TryGetComponent(out Animal animal) && !animal.isInCombat) {
            animal.StartCombatState();
        }

        return true;
    }

    public override Transform PickUp(bool isPlayerCalled = false) {
        Transform ret = base.PickUp(isPlayerCalled);
        if (ret == null) return null;
        
        if (ret.TryGetComponent(out Animal animal)) {
            animal.EndCombatState();
        }
        
        return ret;
    }
}
