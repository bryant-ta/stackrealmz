using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
    public SO_Card cardData;

    public new string name;
    public int value;
    public Sprite image;

    public void Start() { Setup(cardData.name, cardData.value, cardData.image); }

    public void Setup(string name, int value, Sprite image) {
        this.name = name;
        this.value = value;
        this.image = image;
    }
    
    void OnEnable() { GameManager.Instance.cards.Add(this); }
    void OnDisable() { GameManager.Instance.cards.Remove(this); }
    
    // GetTopCardObj returns the highest (i.e. has cards under) card in the stack
    public GameObject GetTopCardObj() {
        Transform t = transform;
        while (t.childCount > 0 && t.GetComponentInChildren<Moveable>()) {
            t = t.GetChild(0);
        }

        return t.gameObject;
    }
    public List<T> GetComponentsInStack<T>() where T : Component {
        List<T> components = new List<T>();
        
        Transform t = GetTopCardObj().transform;
        while (t != null) {
            T c = t.GetComponent<T>();
            if (c != null) {
                components.Add(c);
            } else {
                Debug.LogErrorFormat("Component %s missing from card stack ", typeof(T));
            }

            t = t.parent;
        }

        return components;
    }
    public List<string> GetCardsNamesInStack() {
        List<string> cardNames = new List<string>();
        
        Transform t = GetTopCardObj().transform;
        while (t != null) {
            Card c = t.GetComponent<Card>();
            if (c != null) {
                cardNames.Add(c.name);
            } else {
                Debug.LogError("Card component missing from card stack");
            }

            t = t.parent;
        }

        return cardNames;
    }
}