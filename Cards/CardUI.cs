using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Load Order: after default, for base classes to setup data first
[RequireComponent(typeof(Card))]
public class CardUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI valueText;
    public Image cardArt;

    Card mCard;
    
    protected void Start() {
        mCard = GetComponent<Card>();

        nameText.text = mCard.name;
        if (mCard.image) {
            cardArt.sprite = mCard.image;
        }

        if (mCard.value > 0) {
            valueText.text = mCard.value.ToString();
        } else {
            valueText.enabled = false;
        }
    }
}
