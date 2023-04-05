using UnityEngine;

public class CardPack : MonoBehaviour {
    [SerializeField] LayerMask cardLayer;

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == cardLayer) {
            Card c = col.gameObject.GetComponent<Card>();
            if (c != null && c.name == "Coin") {
                
            }
        }
    }

    void OnTriggerExit(Collider col) {
        
    }
}
