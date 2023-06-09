using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Card))]
public class Moveable : MonoBehaviour {
    public bool isStackable = true;
    public List<Transform> nearestSnappableObjs = new List<Transform>();

    bool isPickedUp;
    Card mCard;

    void Awake() { mCard = GetComponent<Card>(); }

    // PickUp manipulates this card and any cards under it in a stack.
    // Returns transform of this card's stack after being picked up
    public Transform PickUp() {
        if (mCard.mStack.isLocked) return null;
        if (mCard.mSlot) {
            if (!mCard.mSlot.PickUp()) {
                return null;    // failed to pickup from slot, such as when card is locked or not enough money to buy
            }
        }
        
        Transform t = mCard.mStack.PickUp(mCard);
        SetStackIsPickedUp(true);

        return t;
    }

    public void Drop() {
        SetStackIsPickedUp(false);

        // Snap to nearest Card
        // - not pretty... but needed to treat Stack and Slot differently but still snap... hmm
        float minDistance = int.MaxValue;
        Card snapCard = null;
        Slot snapSlot = null;
        foreach (Transform near in nearestSnappableObjs.ToList()) {
            // For keepCard recipes, a nearest card could be destroyed, but ref to it remains in nearestSnappableObjs.
            // This cleans up those refs... Other solution could be moving object to be destroyed far away to trigger OnTriggerExit?
            if (near == null) {
                nearestSnappableObjs.Remove(near);
                continue;
            }
            
            float d = Vector3.Distance(transform.position, near.transform.position);
            if (near.TryGetComponent(out Card card)) {
                // Card is not part of my stack and is top card of a stack
                if (card.mStack != mCard.mStack && card.mStack.GetTopCard() == card && card.GetComponent<Moveable>().isStackable && d < minDistance) {
                    snapSlot = null;
                    minDistance = d;
                    snapCard = card.transform.GetComponent<Card>();
                }
            } else if (near.TryGetComponent(out Slot slot)) {
                if (slot.IsEmpty() && d < minDistance) {
                    snapCard = null;
                    minDistance = d;
                    snapSlot = slot;
                }
            }
        }

        if (snapCard) {
            List<Card> movedCards = mCard.mStack.GetStack(); // Save copy since PlaceAll will delete parent stack object
            mCard.mStack.PlaceAll(snapCard.mStack);

            foreach (Card c in movedCards) { // Then move each card individually
                StartCoroutine(MoveCardToPoint(c, snapCard.mStack.CalculateStackPosition(c)));
            }
        } else if (snapSlot && mCard.mStack.GetStackSize() == 1) {
            snapSlot.PlaceAndMove(mCard.mStack);   // Handles card movement too, for use in non-player movement
        } else {
            mCard.mStack.PlaceAll(null); // No stack manipulations, but need to trigger crafting
            StartCoroutine(MoveStackToPoint(mCard.mStack, new Vector3(mCard.mStack.transform.position.x, 0, mCard.mStack.transform.position.z)));
        }
    }
    
    // Actually have a lot of trouble combining with MoveCardToPoint nicely... leaving separate for now for ease of use
    // prob need to move this out of moveable???
    public IEnumerator MoveStackToPoint(Stack stack, Vector3 endPoint) {
        Vector3 startPos = stack.transform.localPosition;
        float t = 0f;

        stack.isLocked = true;
        
        while (t < 1) {
            t += Constants.CardMoveSpeed * Time.deltaTime;
            stack.transform.localPosition = Vector3.Lerp(startPos, endPoint, t);
            yield return null;
        }
        
        stack.isLocked = false;
    }
    
    public IEnumerator MoveCardToPoint(Card card, Vector3 endPoint) {
        Vector3 startPos = card.transform.localPosition;
        float t = 0f;

        card.mStack.isLocked = true;
        
        while (t < 1) {
            t += Constants.CardMoveSpeed * Time.deltaTime;
            card.transform.localPosition = Vector3.Lerp(startPos, endPoint, t);
            yield return null;
        }
        
        card.mStack.isLocked = false;
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == gameObject.layer && !nearestSnappableObjs.Contains(col.transform)) {
            nearestSnappableObjs.Add(col.transform);
        }
    }
    void OnTriggerExit(Collider col) {
        if (col.gameObject.layer == gameObject.layer && nearestSnappableObjs.Contains(col.transform)) {
            nearestSnappableObjs.Remove(col.transform);
        }
    }

    void SetStackIsPickedUp(bool status) {
        if (mCard.mStack != null) {
            foreach (var moveable in mCard.mStack.GetComponentsInStack<Moveable>()) {
                moveable.isPickedUp = status;
            }
        } else {
            isPickedUp = status;
        }
    }
}