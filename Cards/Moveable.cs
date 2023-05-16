using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Possibly separate crafting from Moveable
[RequireComponent(typeof(Card))]
public class Moveable : MonoBehaviour {
    public bool isStackable = true;
    public float dropSpeed = 1;
    public List<Card> nearestCards = new List<Card>();

    bool isPickedUp;
    Card mCard;

    void Awake() { mCard = GetComponent<Card>(); }

    // PickUp manipulates this card and any cards under it in a stack.
    // Returns transform of this card's stack after being picked up
    public Transform PickUp() {
         Transform t = mCard.mStack.PickUp(mCard);
         
        SetStackIsPickedUp(true);

        return t;
    }
    
    public void Drop() {
        SetStackIsPickedUp(false);

        // Snap to nearest Card
        float distance = int.MaxValue;
        Transform snapTrans = null;
        Card snapCard = null;
        if (isStackable) {
            foreach (Card card in nearestCards) {
                float d = Vector3.Distance(transform.position, card.transform.position);
                // Card not part of my stack and is top card of a stack
                if (card.mStack != mCard.mStack && card.mStack.GetTopCardObj() == card.gameObject && d < distance) {
                    distance = d;
                    snapTrans = card.transform;
                    snapCard = snapTrans.GetComponent<Card>();
                }
            }
        }

        if (snapTrans) {
            List<Card> movedCards = mCard.mStack.GetStack();    // Save copy since PlaceAll will delete parent stack object
            mCard.mStack.PlaceAll(snapCard.mStack);

            foreach (Card c in movedCards) {                    // Then move each card individually
                StartCoroutine(FallTo(c.transform, snapCard.mStack.CalculateStackPosition(c)));
            }
        } else {
            mCard.mStack.PlaceAll(null);                // No stack manipulations, but need to trigger crafting
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
        if (col.gameObject.layer == gameObject.layer && col.gameObject.TryGetComponent(out Card card) && !nearestCards.Contains(card)) {
            nearestCards.Add(card);
        }
    }
    void OnTriggerExit(Collider col) {
        if (col.gameObject.layer == gameObject.layer && col.gameObject.TryGetComponent(out Card card) && nearestCards.Contains(card)) {
            nearestCards.Remove(card);
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