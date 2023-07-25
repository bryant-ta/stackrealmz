using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/SO_Recipe")]
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

    public bool ContainsProduct(SO_Card cSO) {
        return products.Contains(cSO) || randomProducts.Exists(drop => drop.cardDropsPool.Contains(cSO));
    }

    public bool ContainsMaterial(SO_Card cSO) {
        return materials.Contains(cSO.name);
    }
}

[Serializable]
public struct Drop {
    public List<SO_Card> cardDropsPool;
    public int percentage;
}