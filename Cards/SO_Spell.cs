using UnityEngine;

[CreateAssetMenu(menuName = "Cards/SO_Spell")]
public class SO_Spell : SO_Card {
    public int manaCost;
    public SpellType spellType;
    
    public CardText cardText;
}