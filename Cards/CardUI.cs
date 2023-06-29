using TMPro;
using UnityEngine;

// Load Order: after default, for base classes to setup data first
[RequireComponent(typeof(Card))]
public class CardUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI valueText;

    Card mCard;
    
    protected void Start() {
        mCard = GetComponent<Card>();

        nameText.text = mCard.name;

        if (mCard.value > 0) {
            valueText.text = mCard.value.ToString();
        } else {
            valueText.enabled = false;
        }
    }
}
