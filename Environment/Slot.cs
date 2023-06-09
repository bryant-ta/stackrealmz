using System.Collections;
using UnityEngine;

public class Slot : MonoBehaviour {
    public int x, y;

    public Card Card { get { return card; } private set { card = value; } }
    [SerializeField] protected Card card;
    
    public SlotGrid SlotGrid { get { return mSlotGrid; } private set { mSlotGrid = value; } }
    [SerializeField] protected SlotGrid mSlotGrid;

    // PlaceAndMove handles registering a stack with the slot and physically moving stack's location to this Slot
    // Slots only allow stacks of one card (for now?)
    public virtual bool PlaceAndMove(Stack stack) {
        if (!IsEmpty() || stack.GetStackSize() != 1) { return false; }

        card = stack.GetTopCard();

        // Reset card's previous slot, if any, such as when moving card directly between slots
        if (card.mSlot && card.mSlot.Card) {
            card.mSlot.Card = null;
        }
        card.mSlot = this;
        
        if (card.TryGetComponent(out Moveable m)) {
            m.isStackable = false;
        }

        StartCoroutine(card.GetComponent<Moveable>().MoveStackToPoint(stack, CalculateCardPosition()));

        return true;
    }

    public virtual Transform PickUp() {
        Card c = card;
        card.mSlot = null;
        card = null;
        
        if (c.TryGetComponent(out Moveable m)) {
            m.isStackable = true;
        }
        
        return c.transform;
    }

    public bool IsEmpty() {
        return card == null;
    }

    public Vector3 CalculateCardPosition() {
        return new Vector3(transform.position.x, 0.01f, transform.position.z);
    }
}
