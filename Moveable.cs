using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Possibly separate crafting from Moveable
[RequireComponent(typeof(Card))]
public class Moveable : MonoBehaviour {
    public bool isStackable = true;
    public float dropSpeed = 1;
    public List<GameObject> nearestCards = new List<GameObject>();

    [SerializeField] GameObject craftProgressBar;
    bool isPickedUp;
    public Card mCard; // public for CardFactory to reset correctly after destroying Card

    void Awake() { mCard = GetComponent<Card>(); }

    public void Pickup() {
        // trigger crafting for stack left behind
        if (transform.parent != null) {
            GameObject parent = transform.parent.gameObject;
            transform.parent = null; // Need to remove parent for Craft() and SetStackIsPickedUp() to be accurate

            if (parent.TryGetComponent(out Moveable moveable)) {
                StartCoroutine(moveable.Craft());
            }
        }

        SetStackIsPickedUp(true);
    }

    public void Drop() {
        SetStackIsPickedUp(false);

        // Snap to nearest Card
        float distance = int.MaxValue;
        Transform snapTrans = null;
        GameObject topCard = mCard.GetTopCardObj();

        if (isStackable) {
            foreach (GameObject card in nearestCards) {
                float d = Vector3.Distance(transform.position, card.transform.position);
                // Exclude every card not at the top of a stack, and exclude our stack's top card
                // This subset gives all valid cards to stack on
                if (card != topCard && (card.transform.childCount == 0 && card.GetComponentInChildren<Moveable>()) && d < distance) {
                    
                    distance = d;
                    snapTrans = card.transform;
                }
            }
        }

        if (snapTrans) {
            transform.SetParent(snapTrans);
            StartCoroutine(FallTo(new Vector3(0, -0.2f, -0.01f)));  // y = stack offset, z = height
        } else {
            StartCoroutine(FallTo(new Vector3(transform.position.x, 0, transform.position.z)));
        }

        // Do crafting
        if (isStackable) {
            Moveable topCardOfStack = mCard.GetTopCardObj().GetComponent<Moveable>();
            StartCoroutine(topCardOfStack.Craft());
        }
    }

    IEnumerator FallTo(Vector3 endPoint) {
        Vector3 startPos = transform.localPosition;
        float t = 0f;
        while (t < 1 && !isPickedUp) {
            t += dropSpeed * Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPos, endPoint, t);
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
            foreach (Card c in mCard.GetComponentsInStack<Card>()) {
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
        if (col.gameObject.layer == gameObject.layer && !nearestCards.Contains(col.gameObject)) {
            nearestCards.Add(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.gameObject.layer == gameObject.layer && nearestCards.Contains(col.gameObject)) {
            nearestCards.Remove(col.gameObject);
        }
    }

    void SetStackIsPickedUp(bool status) {
        foreach (var moveable in mCard.GetComponentsInStack<Moveable>()) {
            moveable.isPickedUp = status;
        }
    }
}