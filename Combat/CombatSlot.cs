using UnityEngine;

public class CombatSlot : Slot
{
    public Animal Animal { get => animal; private set { animal = value; } }
    [SerializeField] protected Animal animal;

    public Terrain terrain;
    
    public override bool PlaceAndMove(Stack stack, bool isPlayerCalled = false) {
        // Conditions
        if (!stack.GetTopCard().TryGetComponent(out Animal a)) return false;
        if (isPlayerCalled && GameManager.Instance.Mana < a.manaCost.Value) return false;
        
        // Condition + Effect
        if (!base.PlaceAndMove(stack, isPlayerCalled)) return false;

        // Effects
        if (isPlayerCalled) {
            GameManager.Instance.ModifyMana(-a.manaCost.Value);
        }

        animal = a;
        
        if (!animal.isInCombat) {
            animal.StartCombatState();
            animal.Play();
        }

        return true;
    }
    
    public override Transform PickUp(bool isPlayerCalled = false, bool doEventInvoke = true) {
        Transform ret = base.PickUp(isPlayerCalled);
        if (ret == null) return null;
        
        animal.EndCombatState();
        animal = null;

        return ret;
    }

    void MoveCardToCombatSlot(CombatSlot slot) {
        Transform ret = base.PickUp(false, false);
        if (ret == null) return;

        slot.PlaceAndMove(animal.mStack);
        
        animal = null;
    }

    // SwapWithCombatSlot swaps two CombatSlots' Animals. If one slot does not have an Animal, this will
    // act as a move instead.
    public void SwapWithCombatSlot(CombatSlot slot) {
        Transform ret = base.PickUp(false, false);
        if (ret == null) return;

        Animal temp = animal;
        slot.MoveCardToCombatSlot(this);

        if (!animal) {
            slot.PlaceAndMove(temp.mStack);
        }
    }
}
