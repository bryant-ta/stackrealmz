using UnityEngine;

public class CombatSlot : Slot
{
    public Terrain terrain;
    
    public override bool PlaceAndMove(Stack stack) {
        if (!base.PlaceAndMove(stack)) return false;

        if (Card.TryGetComponent(out Animal animal) && !animal.isInCombat) {
            animal.StartCombatState();
        }

        return true;
    }

    public override Transform PickUp() {
        Transform ret = base.PickUp();
        
        if (ret.TryGetComponent(out Animal animal)) {
            animal.EndCombatState();
        }
        
        return ret;
    }
}
