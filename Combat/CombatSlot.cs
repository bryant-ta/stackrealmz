using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CombatSlot : Slot
{
    public Terrain terrain;
    
    public override bool PlaceAndMove(Stack stack, bool isPlayerCalled = false) {
        if (!stack.GetTopCard().TryGetComponent(out Animal animal)) return false;
        if (isPlayerCalled) {
            if (GameManager.Instance.Mana >= animal.manaCost.Value) {
                GameManager.Instance.ModifyMana(-animal.manaCost.Value);
            } else {
                return false;
            }
        } 
            
        if (!base.PlaceAndMove(stack, isPlayerCalled)) return false;
            
        animal.StartCombatState();
        return true;
    }

    public override Transform PickUp(bool isPlayerCalled = false, bool doEventInvoke = true) {
        Transform ret = base.PickUp(isPlayerCalled);
        if (ret == null) return null;
        
        if (ret.TryGetComponent(out Animal animal)) {
            animal.EndCombatState();
        }
        
        return ret;
    }
}
