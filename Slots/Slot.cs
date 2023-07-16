using UnityEngine;

public class Slot : MonoBehaviour {
    public int x, y;
    public bool canPlace;
    public bool canPickUp;
    
    public Stack Stack { get { return stack; } private set { stack = value; } }
    [SerializeField] protected Stack stack;

    public Card Card { get { return card; } private set { card = value; } }
    [SerializeField] protected Card card;
    bool cardWasStackable;
    
    public SlotGrid SlotGrid { get { return mSlotGrid; } private set { mSlotGrid = value; } }
    [SerializeField] protected SlotGrid mSlotGrid;


    // PlaceAndMove handles registering a stack with the slot and physically moving stack's location to this Slot
    // Slots only allow stacks of one card (for now?)
    public virtual bool PlaceAndMove(Stack stack, bool isPlayerCalled = false) {
        if (!IsEmpty() || (isPlayerCalled && !canPlace) || stack.GetStackSize() != 1) { return false; }

        // Set slot fields
        this.stack = stack;
        card = stack.GetTopCard();

        // Set card fields
        // Reset card's previous slot, if any, such as when moving card directly between slots
        if (card.mSlot && card.mSlot.Card) {
            card.mSlot.Card = null;
        }
        card.mSlot = this;
        stack.transform.SetParent(transform);
        if (card.TryGetComponent(out MoveableCard m)) {
            cardWasStackable = m.IsStackable;
            m.IsStackable = false;
        }

        // Move card to slot (negative StackDepthOffset in Z bc child of slot when placed, slot is rotated)
        StartCoroutine(Utils.MoveStackToPoint(stack, new Vector3(0,0,-Constants.StackDepthOffset)));
        
        // Send event
        EventManager.Invoke(gameObject, EventID.SlotPlaced);

        return true;
    }

    public virtual Transform PickUp(bool isPlayerCalled = false, bool doEventInvoke = true) {
        if (IsEmpty() || (isPlayerCalled && !canPickUp)) return null;
        
        // Set card fields
        card.mSlot = null;
        card.mStack.transform.SetParent(null);
        if (card.TryGetComponent(out MoveableCard m)) {
            m.IsStackable = cardWasStackable;
            cardWasStackable = false;
        }
        Card c = card;
        
        // Set slot fields
        stack = null;
        card = null;

        // Send event
        if (doEventInvoke) EventManager.Invoke(gameObject, EventID.SlotPickedUp);
        
        return c.transform;
    }

    public bool IsEmpty() {
        return card == null;
    }
}
