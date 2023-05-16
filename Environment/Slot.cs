using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {
    public string terrain;  // TODO: prob make terrain an enum or object, placeholder

    [SerializeField] Card heldCard;
    [SerializeField] SlotGrid mSlotGrid;

    public void Place(Card card) {
        heldCard = card;
    }

    public Transform PickUp() {
        Card c = heldCard;
        heldCard = null;
        return c.transform;
    }
}
