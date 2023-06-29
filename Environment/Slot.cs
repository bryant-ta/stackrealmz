using UnityEngine;

public class Slot : MonoBehaviour {
    public int x, y;
    public bool isLocked;
    
    public Stack Stack { get { return stack; } private set { stack = value; } }
    [SerializeField] protected Stack stack;

    public Card Card { get { return card; } private set { card = value; } }
    [SerializeField] protected Card card;
    
    public SlotGrid SlotGrid { get { return mSlotGrid; } private set { mSlotGrid = value; } }
    [SerializeField] protected SlotGrid mSlotGrid;

    // PlaceAndMove handles registering a stack with the slot and physically moving stack's location to this Slot
    // Slots only allow stacks of one card (for now?)
    public virtual bool PlaceAndMove(Stack stack, bool isPlayerCalled = false) {
        if (!IsEmpty() || (isPlayerCalled && isLocked) || stack.GetStackSize() != 1) { return false; }

        // Set slot fields
        this.stack = stack;
        card = stack.GetTopCard();

        // Set card fields
        // Reset card's previous slot, if any, such as when moving card directly between slots
        if (card.mSlot && card.mSlot.Card) {
            card.mSlot.Card = null;
        }
        card.mSlot = this;
        if (card.TryGetComponent(out Moveable m)) {
            m.isStackable = false;
        }

        // Move card to slot
        StartCoroutine(Utils.MoveStackToPoint(stack, CalculateCardPosition()));
        
        // Send event
        EventManager.Invoke(gameObject, EventID.SlotPlaced);

        return true;
    }

    public virtual Transform PickUp(bool isPlayerCalled = false) {
        if (isPlayerCalled && isLocked) return null;
        
        // Set card fields
        card.mSlot = null;
        if (card.TryGetComponent(out Moveable m)) {
            m.isStackable = true;
        }
        Card c = card;
        
        // Set slot fields
        stack = null;
        card = null;
        
        // Send event
        EventManager.Invoke(gameObject, EventID.SlotPickedUp);
        
        return c.transform;
    }

    public bool IsEmpty() {
        return card == null;
    }

    protected Vector3 CalculateCardPosition() {
        return new Vector3(transform.position.x, 0.01f, transform.position.z);
    }
}
