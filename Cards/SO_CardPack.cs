using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/CardPack")]
public class SO_CardPack : SO_Card {
    public int numCards;
    public List<Drop> dropTable = new List<Drop>();
}