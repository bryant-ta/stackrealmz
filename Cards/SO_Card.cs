using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Card")]
public class SO_Card : ScriptableObject {
    public new string name;
    public int value;
    public Sprite image;
    public Rarity rarity;
}

public enum Rarity {
    Common = 0,
    Rare = 1,
    Epic = 2,
    Legendary = 3,
}