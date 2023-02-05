using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour {
    [SerializeField] new string name;
    [SerializeField] int value;
    [SerializeField] Sprite image;

    public List<GameObject> nearestCards;

    public void Setup(string name, int value, Sprite image) {
        this.name = name;
        this.value = value;
        this.image = image;
    }

    public void Pickup() {
        // trigger crafting for stack left behind
        if (transform.parent != null) {
            Card c = transform.parent.GetComponent<Card>();
            if (c != null) {
                c.Craft();
            }
        }

        transform.parent = null;
    }

    public void Fall() {
        if (nearestCards.Count == 0) {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            return;
        }

        // Snap to nearest Card
        float distance = int.MaxValue;
        Transform snapTrans = null;
        GameObject topCard = GetTopCardObj();

        foreach (GameObject card in nearestCards) {
            float d = Vector3.Distance(transform.position, card.transform.position);
            // Exclude every card not at the bottom of a stack, and exclude our stack's bottom card
            // This subset gives all valid cards to stack on
            if (card.transform.childCount == 0 && card != topCard && d < distance) {
                distance = d;
                snapTrans = card.transform;
            }
        }

        if (!snapTrans) {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            return;
        }

        // Do stacking
        transform.SetParent(snapTrans);
        transform.localPosition = new Vector3(0, 0.2f, 0.01f); // y = stack offset, z = height

        // Do crafting
        Craft();
    }

    void Craft() {
        List<string> cardNames = GetCardsNamesInStack();
        Recipe r = RecipeList.LookupRecipe(cardNames);

        
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == gameObject.layer && !nearestCards.Contains(col.gameObject)) {
            nearestCards.Add(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.gameObject.layer == gameObject.layer) {
            nearestCards.Remove(col.gameObject);
        }
    }

// GetTopCardObj returns the highest (i.e. has cards under) card in the stack
    GameObject GetTopCardObj() {
        Transform t = transform;
        while (t.childCount > 0) {
            t = t.GetChild(0);
        }

        return t.gameObject;
    }

// GetCardsInStack returns all cards under this card
    List<Card> GetCardsInStack() {
        List<Card> cards = new List<Card> {this};
        Transform t = transform;

        while (t.parent != null) {
            Card c = t.parent.GetComponent<Card>();
            if (c != null) {
                cards.Add(c);
            } else {
                Debug.LogError("Non-card obj in card stack");
            }

            t = t.parent;
        }

        return cards;
    }

// GetCardsNamesInStack returns all cards' names under this card
    List<string> GetCardsNamesInStack() {
        List<string> cardNames = new List<string> {this.name};
        Transform t = transform;

        while (t.parent != null) {
            Card c = t.parent.GetComponent<Card>();
            if (c != null) {
                cardNames.Add(c.name);
            } else {
                Debug.LogError("Non-card obj in card stack");
            }

            t = t.parent;
        }

        return cardNames;
    }
}