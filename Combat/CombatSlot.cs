using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CombatSlot : Slot
{
    public Animal Animal { get => animal; private set { animal = value; } }
    [SerializeField] protected Animal animal;

    public bool isAllySlot;
    public Terrain terrain;
    
    public override bool PlaceAndMove(Stack stack, bool isPlayerCalled = false) {
        if (!stack.GetTopCard().TryGetComponent(out Animal a)) return false;
        if (isPlayerCalled && GameManager.Instance.Mana < a.manaCost.Value) return false;

        if (!base.PlaceAndMove(stack, isPlayerCalled)) return false;

        if (isPlayerCalled) {
            GameManager.Instance.ModifyMana(-a.manaCost.Value);
        }

        animal = a;
        animal.StartCombatState();
        animal.Play();
        
        return true;
    }

    public override Transform PickUp(bool isPlayerCalled = false, bool doEventInvoke = true) {
        Transform ret = base.PickUp(isPlayerCalled);
        if (ret == null) return null;
        
        animal.EndCombatState();
        animal = null;

        return ret;
    }
}
