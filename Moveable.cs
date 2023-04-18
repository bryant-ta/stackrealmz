using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Possibly separate crafting from Moveable
[RequireComponent(typeof(Card))]
public class Moveable : MonoBehaviour {
    public bool isStackable = true;
    public float dropSpeed = 1;
    public List<Card> nearestCards = new List<Card>();

    [SerializeField] GameObject craftProgressBar;
    bool isPickedUp;
    public Card mCard; // public for CardFactory to reset correctly after destroying Card

    void Awake() { mCard = GetComponent<Card>(); }

    // TODO: return stack or card as needed
    public Transform Pickup() {
        // Trigger Stack pickup if in Stack (assumes Stack is always only possible parent)
        if (mCard.mStack != null) {
            Transform t = transform.parent.GetComponent<Stack>().Pickup(mCard);
            return t;
        }

        SetStackIsPickedUp(true);

        return transform;
    }

    public void Drop() {
        SetStackIsPickedUp(false);

        // Snap to nearest Card
        float distance = int.MaxValue;
        Transform snapTrans = null;

        if (isStackable) {
            foreach (Card card in nearestCards) {
                float d = Vector3.Distance(transform.position, card.transform.position);
                // Card not part of a stack, or is top card of a stack
                if ((card.mStack == null || 
                     (card.mStack != mCard.mStack && card.mStack.GetTopCardObj() == card.gameObject))
                    && d < distance) {
                    distance = d;
                    snapTrans = card.transform;
                }
            }
        }

        if (snapTrans) {
            print(snapTrans.gameObject.name + "\n" + snapTrans.position);
            Card snapCard = snapTrans.GetComponent<Card>();
            // create new stack and snap to card
            if (snapCard.mStack == null) {
                // TODO: prob move to CardFactory
                GameObject newStackObj = new GameObject("Stack");
                newStackObj.transform.position = snapTrans.position;
                newStackObj.transform.rotation = snapTrans.rotation;
                Stack newStack = newStackObj.AddComponent<Stack>();
                newStack.Place(snapCard);
                newStack.Place(mCard);
                
            }
            // snap to top card of existing stack
            else {
                snapCard.mStack.Place(mCard);
            }
            StartCoroutine(FallTo(mCard.transform, snapCard.mStack.CalculateNextStackPosition()));  // y = stack offset, z = height
        } else {
            if (mCard.mStack != null) {
                StartCoroutine(FallTo(mCard.mStack.transform, 
                    new Vector3(mCard.mStack.transform.position.x, 0, mCard.mStack.transform.position.z)));
            } else {
                StartCoroutine(FallTo(transform, new Vector3(transform.position.x, 0, transform.position.z)));
            }
        }

        // Do crafting
        // if (isStackable) {
        //     Moveable topCardOfStack = mCard.GetTopCardObj().GetComponent<Moveable>();
        //     StartCoroutine(topCardOfStack.Craft());
        // }
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

    IEnumerator Craft() {
        List<string> cardNames = mCard.GetCardsNamesInStack();
        Recipe validRecipe = CardFactory.LookupRecipe(cardNames);
        if (validRecipe == null) { yield break; }

        int craftFinished = 0;
        // Delegate shortened syntax for returning a value from coroutine
        yield return StartCoroutine(DoCraftTime(validRecipe.craftTime, (res) => {
            craftFinished = res;
        }));

        if (craftFinished == 1 && !isPickedUp) {
            if (validRecipe.dropTable.Count > 0) {
                for (int i = 0; i < validRecipe.numDrops; i++) {
                    SO_Card cSO = CardFactory.RollDrop(validRecipe.dropTable);
                    if (cSO == null) {
                        continue;
                    }

                    GameObject cardObj = CardFactory.CreateCard(cSO);
                    cardObj.transform.position = Utils.GenerateCircleVector(i, validRecipe.products.Length,
                        Constants.CardCreationRadius, transform.position);
                }
            } else {
                for (int i = 0; i < validRecipe.products.Length; i++) {
                    GameObject cardObj = CardFactory.CreateCard(validRecipe.products[i]);
                    cardObj.transform.position = Utils.GenerateCircleVector(i, validRecipe.products.Length,
                        Constants.CardCreationRadius, transform.position);
                }
            }

            // TODO: convert to event
            foreach (Card c in mCard.mStack.GetComponentsInStack<Card>()) {
                Destroy(c.gameObject);
            }
        }
    }

    IEnumerator DoCraftTime(float craftTime, Action<int> onCraftFinished) {
        // TODO: If this slow, make this pooled
        GameObject barObj = Instantiate(craftProgressBar, GameManager.WorldCanvas.gameObject.transform);
        // TODO: Bug where crafting bar doesnt move to correct spot when crafting holding a stack (parent is in the currently held stack)
        barObj.transform.position =
            new Vector3(transform.parent.position.x, 0, transform.parent.position.z - 1f);

        Image craftProgressFill = barObj.transform.GetChild(0).GetComponent<Image>();
        // Fill bar over <craftTime> seconds. Interrupted by being pickedup and placed on
        while (craftProgressFill.fillAmount < 1 && !isPickedUp && (transform.childCount == 0 && GetComponentInChildren<Moveable>())) {
            craftProgressFill.fillAmount += (float) GameManager.TimeSpeed / craftTime * Time.deltaTime;
            yield return null;
        }

        Destroy(barObj);

        if (craftProgressFill.fillAmount >= 1) {
            onCraftFinished(1);
            yield break; // Above line runs delegate function, setting craftFinished. Then return immediately
        }

        onCraftFinished(0);
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == gameObject.layer && col.gameObject.TryGetComponent(out Card card) 
            && !nearestCards.Contains(card)) {
            nearestCards.Add(card);
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.gameObject.layer == gameObject.layer && col.gameObject.TryGetComponent(out Card card)
            && nearestCards.Contains(card)) {
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