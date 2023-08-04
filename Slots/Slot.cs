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
        EventManager.Invoke(gameObject, EventID.SlotPlaced, this);
        // EventManager.Invoke(card.gameObject, EventID.CardPlaced);

        return true;
    }

    public bool PickUpCondition(bool isPlayerCalled = false) { return IsEmpty() || (isPlayerCalled && !canPickUp); }

    public virtual Transform PickUpHeld(bool isPlayerCalled = false, bool endCombatState = false, bool doEventInvoke = true) {
        if (PickUpCondition(isPlayerCalled)) return null;
        
        // Set card fields
        card.mSlot = null;
        card.mStack.transform.SetParent(null);
        if (card.TryGetComponent(out MoveableCard m)) {
            m.IsStackable = cardWasStackable;
            cardWasStackable = false;
        }

        // Send event
        if (doEventInvoke) {
            EventManager.Invoke(card.gameObject, EventID.CardPickedUp);
            EventManager.Invoke(gameObject, EventID.SlotPickedUp, this);
        }

        // Set slot fields
        Stack s = stack;
        stack = null;
        card = null;
        
        return s.transform;
    }

    public virtual Stack SpawnCard(SO_Card cardData) {
        if (!IsEmpty()) return null;
        
        Stack s = CardFactory.CreateStack(transform.position, cardData);
        PlaceAndMove(s);

        return s;
    }

    public bool IsEmpty() {
        return card == null;
    }
}
