using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Card")]
public class SO_Card : ScriptableObject {
    public new string name;
    public int value;
    public Sprite image;
    public Recipe[] recipe;
}

[Serializable]
public class Recipe {
    public string[] materials;
    public int resultQuantity;
    public int craftTime;            // craft time
}