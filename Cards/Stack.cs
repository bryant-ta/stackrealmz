using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stack : MonoBehaviour {
    [SerializeField] List<Card> stack = new List<Card>();

    public void PlaceAll(Stack otherStack) {
        MoveCardRange(0, stack.Count, otherStack);
    }
    
    public Transform Pickup(Card card) {
        if (card == stack.First()) {
            return transform;
        } else {
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
        if (stack.Count == 0) {
            Destroy(gameObject);
        }
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

    // SplitStack returns a new stack (+object) containing all cards from input card to end of current stack
    // - Sets each cards mStack reference
    Transform SplitStack(Card card) {
        int splitIndex = stack.IndexOf(card);
        
        // Create Stack GameObject
        GameObject newStackObj = new GameObject("Stack");
        newStackObj.transform.position = stack[splitIndex].transform.position;
        Stack newStack = newStackObj.AddComponent<Stack>();
        
        MoveCardRange(splitIndex, stack.Count-splitIndex, newStack);
        
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

    public List<Card> GetStack() {
        return new List<Card>(stack);
    }

    // CalculateStackPosition returns correct card position according to its stack index
    public Vector3 CalculateStackPosition(Card card) {
        int i = stack.IndexOf(card);
        return new Vector3(0, 0.01f * i, -0.2f * i);
    }
}
