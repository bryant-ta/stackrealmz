using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Possibly separate crafting from Moveable
[RequireComponent(typeof(Card))]
public class Moveable : MonoBehaviour {
    public bool isStackable = true;
    public float dropSpeed = 1;
    public List<Transform> nearestSnappableObjs = new List<Transform>();

    bool isPickedUp;
    Card mCard;

    void Awake() { mCard = GetComponent<Card>(); }

    // PickUp manipulates this card and any cards under it in a stack.
    // Returns transform of this card's stack after being picked up
    public Transform PickUp() {
        Transform t = mCard.mStack.PickUp(mCard);
        if (mCard.mSlot) {
            mCard.mSlot.PickUp();
            isStackable = true;
        }

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
                if (card.mStack != mCard.mStack && card.mStack.GetTopCardObj() == card.gameObject && card.GetComponent<Moveable>().isStackable && d < minDistance) {
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
                StartCoroutine(FallTo(c.transform, snapCard.mStack.CalculateStackPosition(c)));
            }
        } else if (snapSlot && mCard.mStack.GetStackSize() == 1) {
            snapSlot.Place(mCard);
            isStackable = false;
            StartCoroutine(FallTo(mCard.mStack.transform,
                new Vector3(snapSlot.transform.position.x, 0.01f, snapSlot.transform.position.z)));
        } else {
            mCard.mStack.PlaceAll(null); // No stack manipulations, but need to trigger crafting
            StartCoroutine(FallTo(mCard.mStack.transform,
                new Vector3(mCard.mStack.transform.position.x, 0, mCard.mStack.transform.position.z)));
        }
    }

    IEnumerator FallTo(Transform obj, Vector3 endPoint) {
        Vector3 startPos = obj.localPosition;
        float t = 0f;
        while (t < 1 && !isPickedUp) {
            t += dropSpeed * Time.deltaTime;
            obj.localPosition = Vector3.Lerp(startPos, endPoint, t);
            yield return null;
        }
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