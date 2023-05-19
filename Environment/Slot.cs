using System;
using UnityEngine;

public class Slot : MonoBehaviour {
    public int x, y;
    public string terrain;  // TODO: prob make terrain an enum or object, placeholder

    public Card Card { get { return card; } private set { card = value; } }
    [SerializeField] Card card;
    
    [SerializeField] SlotGrid mSlotGrid;

    public Slot Forward {
        get {
            if (mSlotGrid) {
                return mSlotGrid.Forward(this);
            }
    
            return null;
        }
    }

    public void Place(Card c) {
        card = c;
        c.mSlot = this;
        if (c.TryGetComponent(out Animal animal)) {
            animal.StartAttack();
        }
    }

    public Transform PickUp() {
        Card c = card;
        card.mSlot = null;
        card = null;
        return c.transform;
    }

    public bool IsEmpty() {
        return card == null;
    }
}
