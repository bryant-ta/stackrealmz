using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardFactory : MonoBehaviour {
    public static CardFactory Instance {
        get {
            if (_instance == null) {
                Instance = FindObjectOfType<CardFactory>().GetComponent<CardFactory>();
            }

            return _instance;
        }
        private set => _instance = value;
    }
    static CardFactory _instance;
    
    public GameObject baseCard;
    public static GameObject _baseCard;
    public GameObject baseStack;
    public static GameObject _baseStack;

    public Recipe[] recipes;
    public static Recipe[] _recipes;

    void Awake() {
        _baseCard = baseCard;
        _baseStack = baseStack;
        _recipes = recipes;
    }

    public static Stack CreateStack(SO_Card cSO = null) {
        Stack s = Instantiate(_baseStack).GetComponent<Stack>();
        if (cSO == null) {
            return s;
        }
        
        GameObject o = Instantiate(_baseCard);
        Card c = o.GetComponent<Card>();
        c.cardData = cSO;

        if (cSO is SO_Food fSO) {
            Destroy(c);
            Food f = o.AddComponent<Food>();
            f.foodData = fSO;
            o.GetComponent<Moveable>().mCard = f;
            s.Place(f);
        } else if (cSO is SO_Villager vSO) {
            Destroy(c);
            Villager v = o.AddComponent<Villager>();
            v.villagerData = vSO;
            o.GetComponent<Moveable>().mCard = v;
            s.Place(v);
        } else if (cSO is SO_CardPack cpSO) {
            Destroy(c);
            CardPack cp = o.AddComponent<CardPack>();
            cp.cardPackData = cpSO;
            o.GetComponent<Moveable>().mCard = cp;
            s.Place(cp);
        } else {
            o.GetComponent<Moveable>().mCard = c;
            s.Place(c);
        }

        return s;
    }

    public static Recipe LookupRecipe(List<string> materials) {
        string[] materialsArr = materials.OrderBy((x => x)).ToArray();
        foreach (Recipe r in _recipes) {
            if (materialsArr.SequenceEqual(r.materials.OrderBy(x => x))) {
                return r;
            }
        }

        return null;
    }
    
    public static SO_Card RollDrop(List<Drop> dropTable) {
        // Roll eligible drops
        int roll = Random.Range(1, 101);
        List<Drop> possibleDrops = new List<Drop>();
        foreach (Drop drop in dropTable) {
            if (roll <= drop.percentage) {
                possibleDrops.Add(drop);
            }
        }

        // Choose most rare drop
        if (possibleDrops.Count > 0) {
            Drop ret = possibleDrops[0];
            List<Drop> tiedDrops = new List<Drop>();
            for (int i = 1; i < possibleDrops.Count; i++) {
                if (possibleDrops[i].percentage == ret.percentage) {
                    tiedDrops.Add(possibleDrops[i]);
                }

                if (possibleDrops[i].percentage < ret.percentage) {
                    ret = possibleDrops[i];
                    tiedDrops.Clear();
                    tiedDrops.Add(ret);
                }
            }

            // Randomly choose drops with tied drop chance
            if (tiedDrops.Count > 1) {
                ret = tiedDrops[Random.Range(0, tiedDrops.Count)];
            }

            return ret.cSO;
        }

        // Did not roll any drops
        return null;
    }
}

// TODO: JSON recipe list import, possibly then able to better separate RandomizedRecipe
[Serializable]
public class Recipe {
    public string[] materials;
    public SO_Card[] products;
    public int craftTime;
    public List<Drop> dropTable;    // here until JSON recipe import
    public int numDrops;
}

// [Serializable]
// public class RandomizedRecipe : Recipe {
//     public List<Drop> dropTable;
// }