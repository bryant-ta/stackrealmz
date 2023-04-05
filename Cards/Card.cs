using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {
    public SO_Card cardData;

    public new string name;
    public int value;
    public Sprite image;

    public List<GameObject> nearestCards = new List<GameObject>();

    [SerializeField] GameObject craftProgressBar;

    bool isPickedUp;

    public void Start() { Setup(cardData.name, cardData.value, cardData.image); }

    public void Setup(string name, int value, Sprite image) {
        this.name = name;
        this.value = value;
        this.image = image;
    }

    public void Pickup() {
        // trigger crafting for stack left behind
        if (transform.parent != null) {
            Card c = transform.parent.GetComponent<Card>();
            
            transform.parent = null;    // Need to remove parent for Craft() and SetStackIsPickedUp() to be accurate
            
            if (c != null) {
                StartCoroutine(c.Craft());
            }
        }

        SetStackIsPickedUp(true);
    }

    public void Drop() {
        SetStackIsPickedUp(false);
        
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
        StartCoroutine(Craft());
    }

    IEnumerator Craft() {
        List<string> cardNames = GetCardsNamesInStack();
        SO_Card cSO = CardFactory.LookupRecipe(cardNames);
        if (cSO == null) { yield break; }

        int craftFinished = 0;
        // Delegate shortened syntax for returning a value from coroutine
        yield return StartCoroutine(DoCraftTime(cSO.recipe.time, (res) => {
            craftFinished = res;
        }));

        if (craftFinished == 1 && !isPickedUp) {
            GameObject cardObj = CardFactory.CreateCard(cSO);
            if (cardObj != null) {
                cardObj.transform.position = transform.position + Vector3.right;
            }

            foreach (Card c in GetCardsInStack()) {
                Destroy(c.gameObject);
            }
        }
    }

    IEnumerator DoCraftTime(float craftTime, System.Action<int> onCraftFinished) {
        // TODO: If this slow, make this pooled
        GameObject barObj = Instantiate(craftProgressBar, GameManager.WorldCanvas.gameObject.transform);
        barObj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.75f);

        Image craftProgressFill = barObj.transform.GetChild(0).GetComponent<Image>();
        // Fill bar over <craftTime> seconds. Interrupted by being pickedup and placed on
        while (craftProgressFill.fillAmount < 1 && !isPickedUp && transform.childCount == 0)
        {
            craftProgressFill.fillAmount += 1.0f / craftTime * Time.deltaTime;
            yield return null;
        }
        
        Destroy(barObj);
        
        if (craftProgressFill.fillAmount >= 1) {
            onCraftFinished(1);
            yield break;            // Above line runs delegate function, setting craftFinished. Then return immediately
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

    void OnEnable() { GameManager.Instance.cards.Add(this); }
    void OnDisable() { GameManager.Instance.cards.Remove(this); }

    // GetTopCardObj returns the highest (i.e. has cards under) card in the stack
    GameObject GetTopCardObj() {
        Transform t = transform;
        while (t.childCount > 0) {
            t = t.GetChild(0);
        }

        return t.gameObject;
    }
    List<Card> GetCardsInStack() {
        List<Card> cards = new List<Card>();
        
        Transform t = GetTopCardObj().transform;
        while (t != null) {
            Card c = t.GetComponent<Card>();
            if (c != null) {
                cards.Add(c);
            } else {
                Debug.LogError("Non-card obj in card stack");
            }

            t = t.parent;
        }

        return cards;
    }
    List<string> GetCardsNamesInStack() {
        List<string> cardNames = new List<string>();
        
        Transform t = GetTopCardObj().transform;
        while (t != null) {
            Card c = t.GetComponent<Card>();
            if (c != null) {
                cardNames.Add(c.name);
            } else {
                Debug.LogError("Non-card obj in card stack");
            }

            t = t.parent;
        }

        return cardNames;
    }

    void SetStackIsPickedUp(bool status) {
        foreach (var c in GetCardsInStack()) {
            c.isPickedUp = status;
        }
    }
}