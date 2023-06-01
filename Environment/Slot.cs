using UnityEngine;

public class Slot : MonoBehaviour {
    public int x, y;
    public Terrain terrain;

    public Card Card { get { return card; } private set { card = value; } }
    [SerializeField] Card card;
    
    public SlotGrid SlotGrid { get { return mSlotGrid; } private set { mSlotGrid = value; } }
    [SerializeField] SlotGrid mSlotGrid;

    public void Place(Card c) {
        card = c;
        c.mSlot = this;
        if (c.TryGetComponent(out Animal animal)) {
            animal.StartCombatState();
        }
    }

    public Transform PickUp() {
        Card c = card;
        card.mSlot = null;
        card = null;
        
        if (c.TryGetComponent(out Animal animal)) {
            animal.EndCombatState();
        }
        
        return c.transform;
    }

    public bool IsEmpty() {
        return card == null;
    }
}
