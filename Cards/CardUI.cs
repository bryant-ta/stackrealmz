using TMPro;
using UnityEngine;

// Load Order: after default, for base classes to setup data first
[RequireComponent(typeof(Card))]
public class CardUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    Card mCard;
    
    protected void Start() {
        mCard = GetComponent<Card>();

        nameText.text = mCard.name;
    }
}
