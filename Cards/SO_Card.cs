using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Card")]
public class SO_Card : ScriptableObject {
    public new string name;
    public int value;
    public Sprite image;
}