using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stack : MonoBehaviour {
    List<Card> stack;

    public void Place(Card card) {
        stack.Add(card);
        card.gameObject.transform.parent = gameObject.transform;
    }

    public Card Pickup() {
        Card last = stack.Last();
        last.transform.parent = null;
        stack.Remove(last);
        return last;
    }
}
