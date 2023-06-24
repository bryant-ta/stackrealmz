using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Recipe")]
public class SO_Recipe : ScriptableObject {
    public int id;
    public List<SO_Card> products = new List<SO_Card>();
    public string[] materials;
    public string[] reusableMaterials;
    public int craftTime;
        
    public List<Drop> randomProducts = new List<Drop>();
    public int numRandomProducts;
}

[Serializable]
public class Recipe {
    public int id;
    public List<SO_Card> products;
    public string[] materials;
    public string[] reusableMaterials;
    public int craftTime;
        
    public List<Drop> randomProducts;
    public int numRandomProducts;
}

[Serializable]
public struct Drop {
    public List<SO_Card> cardDropsPool;
    public int percentage;
}