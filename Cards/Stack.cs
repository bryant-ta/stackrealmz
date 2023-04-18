using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline;
using UnityEngine;

public class Stack : MonoBehaviour {
    [SerializeField] List<Card> stack = new List<Card>();

    public void Place(Card card) {
        AddCard(card);
    }

    // PickUp handles pick up actions on stack cards
    // Returns the Transform of the stack, portion of stack, or single card being picked up
    // - Sets mStack reference
    public Transform Pickup(Card card) {
        if (card == stack.First()) {        // case: bottom of (2+ size) stack, return current stack
            return transform;
        } else if (card == stack.Last()) {  // case: top of stack, return single card
            // TODO: activate crafting for stack left behind
            Card singleCard = stack.Last();
            RemoveCard(singleCard);

            return singleCard.transform;
        } else {                            // case: middle of stack, return new stack starting at that card
            // TODO: activate crafting for stack left behind
            return SplitStack(card).transform;
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
        if (stack.Count == 1) {
            RemoveCard(stack.First());
            Destroy(gameObject);        // possible bug
        }
    }

    // SplitStack returns a new stack (+object) containing all cards from input card to end of current stack
    // - Sets each cards mStack reference
    Transform SplitStack(Card card) {
        // Create Stack GameObject
        int splitIndex = stack.FindIndex(c => c.Equals(card));
        GameObject newStackObj = new GameObject("Stack");
        newStackObj.transform.position = stack[splitIndex].transform.position;
        newStackObj.transform.rotation = stack[splitIndex].transform.rotation;
        Stack newStack = newStackObj.AddComponent<Stack>();

        while (stack.Count > splitIndex) {
            Card c = stack[splitIndex];
            RemoveCard(c);
            newStack.AddCard(c);
        }
        
        return newStack.transform;
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
    // public List<string> GetCardsNamesInStack() {
    //     List<string> cardNames = new List<string>();
    //     
    //     Transform t = GetTopCardObj().transform;
    //     while (t != null) {
    //         Card c = t.GetComponent<Card>();
    //         if (c != null) {
    //             cardNames.Add(c.name);
    //         } else {
    //             Debug.LogError("Card component missing from card stack");
    //         }
    //
    //         t = t.parent;
    //     }
    //
    //     return cardNames;
    // }

    public Vector3 CalculateNextStackPosition() {
        // -1 needed because this func is called by Moveable after a card has been Placed but not moved
        return new Vector3(0, -0.2f * (stack.Count-1), -0.01f * (stack.Count-1));
    }
}
