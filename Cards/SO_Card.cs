using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/SO_Card")]
public class SO_Card : ScriptableObject {
    public new string name;
    public int value;
    public Sprite image;
    public Rarity rarity;
    public Realm realm;
}

public enum Rarity {
    Common = 0,
    Rare = 1,
    Epic = 2,
    Legendary = 3,
    None = 4,
}

public enum Realm {
    Neutral = 0,
    Fire = 1,
    Earth = 2,
    Water = 3,
    Air = 4,
}