using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Stack : MonoBehaviour {
    [SerializeField] List<Card> stack = new List<Card>();

    [SerializeField] GameObject craftProgressBar;
    public bool isChanged;

    public void Place(Card card) {
        AddCard(card);
        isChanged = true;
        TryCraft();
    }

    // null input treated as placing stack on nothing
    public void PlaceAll(Stack otherStack) {
        if (otherStack == null) {
            TryCraft();
            return;
        }

        MoveCardRange(0, stack.Count, otherStack);
        otherStack.isChanged = true;
        otherStack.TryCraft();
    }
    
    public Transform Pickup(Card card) {
        isChanged = true;
        if (card == stack.First()) {
            return transform;
        } else {
            Transform newStack = SplitStack(card).transform;
            TryCraft();
            return newStack;
        }
    }

    // SplitStack returns a new stack (+object) containing all cards from input card to end of current stack
    // - Sets each cards mStack reference
    Transform SplitStack(Card card) {
        Stack newStack = CardFactory.CreateStack();
        int splitIndex = stack.IndexOf(card);
        newStack.transform.position = stack[splitIndex].transform.position;

        MoveCardRange(splitIndex, stack.Count - splitIndex, newStack);

        return newStack.transform;
    }

    // Tries crafting with current stack. This should always be only func called when crafting
    void TryCraft() {
        StartCoroutine(Craft());
    }
    
    // must be coroutine for pausing on DoCraftTime
    IEnumerator Craft() {
        yield return null;
        isChanged = false;
        
        List<string> cardNames = GetCardsNamesInStack();
        Recipe validRecipe = CardFactory.LookupRecipe(cardNames);
        if (validRecipe == null) { yield break; }
        
        int craftFinished = 0;
        // Delegate shortened syntax for returning a value from coroutine
        yield return StartCoroutine(DoCraftTime(validRecipe.craftTime, (res) => {
            craftFinished = res;
        }));

        if (craftFinished == 1 && !isChanged) {
            if (validRecipe.dropTable.Count > 0) {
                for (int i = 0; i < validRecipe.numDrops; i++) {
                    SO_Card cSO = CardFactory.RollDrop(validRecipe.dropTable);
                    if (cSO == null) {
                        continue;
                    }

                    Stack s = CardFactory.CreateStack(cSO);
                    s.transform.position = Utils.GenerateCircleVector(i, validRecipe.products.Length,
                        Constants.CardCreationRadius, transform.position);
                }
            } else {
                for (int i = 0; i < validRecipe.products.Length; i++) {
                    Stack s = CardFactory.CreateStack(validRecipe.products[i]);
                    s.transform.position = Utils.GenerateCircleVector(i, validRecipe.products.Length,
                        Constants.CardCreationRadius, transform.position);
                }
            }

            // TODO: convert to event
            // Destroys all children too
            Destroy(gameObject);
        }
    }

    IEnumerator DoCraftTime(float craftTime, Action<int> onCraftFinished) {
        // TODO: If this slow, make this pooled
        GameObject barObj = Instantiate(craftProgressBar, GameManager.WorldCanvas.gameObject.transform);
        barObj.transform.position =
            new Vector3(transform.position.x, 0, transform.position.z + 0.75f);

        Image craftProgressFill = barObj.transform.GetChild(0).GetComponent<Image>();
        // Fill bar over <craftTime> seconds. Interrupted by being pickedup and placed on
        while (craftProgressFill.fillAmount < 1 && !isChanged ) {
            craftProgressFill.fillAmount += (float) GameManager.TimeSpeed / craftTime * Time.deltaTime;
            yield return null;
        }

        Destroy(barObj);

        if (craftProgressFill.fillAmount >= 1) {
            onCraftFinished(1);
            yield break; // Above line runs delegate function, set craftFinished. Then return immediately
        }

        onCraftFinished(0);
    }

    void MoveCardRange(int startIndex, int count, Stack newStack) {
        List<Card> cards = stack.GetRange(startIndex, count);   // Save copy for adding to newStack, RemoveCard() will delete these cards
        foreach (Card c in cards) {
            RemoveCard(c);
        }
        foreach (Card c in cards) {
            newStack.AddCard(c);
        }
    }
    void AddCard(Card c) {
        stack.Add(c);
        c.mStack = this;
        c.transform.parent = transform;
    }
    void RemoveCard(Card c) {
        stack.Remove(c);
        c.mStack = null;
        c.transform.parent = null;
        if (stack.Count == 0) {
            Destroy(gameObject);
        }
    }
    
    // GetTopCardObj returns the highest (i.e. has cards under) card in the stack
    public GameObject GetTopCardObj() {
        return stack.Last().gameObject;
    }
    public List<T> GetComponentsInStack<T>() where T : Component {
        List<T> components = new List<T>();
        foreach (Card card in stack) {
            T component = card.GetComponent<T>();
            if (component != null) {
                components.Add(component);
            } else {
                Debug.LogErrorFormat("Component %s missing from card stack", typeof(T));
            }
        }
    
        return components;
    }
    public List<string> GetCardsNamesInStack() {
        List<string> cardNames = new List<string>();
        foreach (Card c in stack) {
            cardNames.Add(c.name);
        }

        return cardNames;
    }

    public List<Card> GetStack() {
        return new List<Card>(stack);
    }

    // CalculateStackPosition returns correct card position according to its stack index
    public Vector3 CalculateStackPosition(Card card) {
        int i = stack.IndexOf(card);
        return new Vector3(0, 0.01f * i, -0.2f * i);
    }
}
