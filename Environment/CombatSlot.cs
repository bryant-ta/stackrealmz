using UnityEngine;

public class CombatSlot : Slot
{
    public Terrain terrain;

    Animal claimMovementCard;
    
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

    // RegisterMovementClaim tries to claim a slot to move to based on priority conditions
    public bool RegisterMovementClaim(Animal a) {
        if (!IsEmpty() || a.mStack.GetStackSize() != 1) { return false; }
        
        // Priority conditions: greater atkspd --> player animal
        if (claimMovementCard == null || a.atkSpd.Value > claimMovementCard.atkSpd.Value ||
            (a.atkSpd.Value == claimMovementCard.atkSpd.Value && !a.isEnemy)) {
            claimMovementCard = a;
            return true;
        }
        return false;
    }
    
    public bool CheckMovementClaim(Animal a) {
        if (claimMovementCard == a) {
            claimMovementCard = null;
            return true;
        }

        return false;
    }
}
