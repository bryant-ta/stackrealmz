using UnityEngine;

public class Card : MonoBehaviour {
    public SO_Card cardData;

    public new string name;
    public int value;
    public Sprite image;

    public Stack mStack;
    public Slot mSlot;

    public void Start() { Setup(cardData.name, cardData.value, cardData.image); }

    public void Setup(string name, int value, Sprite image) {
        this.name = name;
        this.value = value;
        this.image = image;

        gameObject.name = name;

        if (transform.parent != null && transform.parent.TryGetComponent(out Stack s)) {
            mStack = s;
        }
    }

    void OnEnable() { GameManager.Instance.cards.Add(this); }
    void OnDisable() { GameManager.Instance.cards.Remove(this); }
}