using UnityEngine;

public class Card : MonoBehaviour {
    public SO_Card cardData;

    public new string name;
    public int value;
    public Sprite image;

    public Stack mStack;
    public Slot mSlot;

    public void Start() { Setup(cardData); }

    protected void Setup(SO_Card cardData) {
        this.cardData = cardData;
        name = cardData.name;
        value = cardData.value;
        image = cardData.image;

        gameObject.name = name;

        if (transform.parent != null && transform.parent.TryGetComponent(out Stack s)) {
            mStack = s;
        }
        
        EventManager.Subscribe(gameObject, EventID.TertiaryDown, ActivateRecipeViewer);
    }

    void ActivateRecipeViewer() {
        EventManager.Invoke(UIManager.Instance.gameObject, EventID.TertiaryDown, cardData);
    }

    void OnEnable() { GameManager.Instance.cards.Add(this); }
    void OnDisable() { GameManager.Instance.cards.Remove(this); }
}