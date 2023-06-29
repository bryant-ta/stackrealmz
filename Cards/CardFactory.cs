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
    
    public GameObject cardBase;
    public static GameObject _cardBase;
    public GameObject foodBase;
    public static GameObject _foodBase;
    public GameObject animalBase;
    public static GameObject _animalBase;
    public GameObject cardPackBase;
    public static GameObject _cardPackBase;
    
    public GameObject sheetBase;
    public static GameObject _sheetBase;
    
    public GameObject stackBase;
    public static GameObject _stackBase;

    public List<Recipe> recipes;
    public static List<Recipe> _recipes;

    void Awake() {
        _cardBase = cardBase;
        _foodBase = foodBase;
        _animalBase = animalBase;
        _cardPackBase = cardPackBase;

        _sheetBase = sheetBase;
        
        _stackBase = stackBase;
        
        _recipes = recipes;
    }

    /*
     * CreateStack creates an empty stack. If a SO_Card is provided, CreateStack will add SO_Card to the stack as a Card.
     * Typical Usage:
            Stack s = CardFactory.CreateStack(stockCardData);
            s.transform.position = CalculateCardPosition();
     */
    public static Stack CreateStack(SO_Card cSO = null) {
        Stack s = Instantiate(_stackBase).GetComponent<Stack>();
        if (cSO == null) {
            return s;
        }
        
        // possibly use interface to fix this strangeness
        if (cSO is SO_Food fSO) {
            Food f = Instantiate(_foodBase).GetComponent<Food>();
            f.foodData = fSO;
            s.Place(f);
        } else if (cSO is SO_Animal aSO) {
            Animal a = Instantiate(_animalBase).GetComponent<Animal>();
            a.animalData = aSO;
            s.Place(a);
        } else if (cSO is SO_CardPack cpSO) {
            CardPack cp = Instantiate(_cardPackBase).GetComponent<CardPack>();
            cp.cardPackData = cpSO;
            s.Place(cp);
        } else {    // is just SO_Card
            Card c = Instantiate(_cardBase).GetComponent<Card>();
            c.cardData = cSO;
            s.Place(c);
        }

        return s;
    }

    public static Sheet CreateSheet(SO_Sheet sSO) {
        Sheet s = Instantiate(_sheetBase).GetComponent<Sheet>();
        s.sheetData = sSO;
        return s;
    }

    public static Stack CreateEnemy(SO_Animal aSO) {
        Stack s = Instantiate(_stackBase).GetComponent<Stack>();
        Animal a = Instantiate(_animalBase).GetComponent<Animal>();
        a.animalData = aSO;
        a.isEnemy = true;
        
        s.Place(a);

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
    
    /*
     * Usage:
     * - Each Drop.cardDropPool is rolled at its percentage, favoring the lowest percentage successful roll
     * - Percentage between Drops in one drop table should never match, put same percentage cards in one cardDropPool
     * - Randomly selects card from cardDropPool
     */
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
            Drop retDrop = possibleDrops[0];
            for (int i = 1; i < possibleDrops.Count; i++) {
                if (possibleDrops[i].percentage < retDrop.percentage) {
                    retDrop = possibleDrops[i];
                }
            }

            // Randomly choose drop from Drop card pool
            return retDrop.cardDropsPool[Random.Range(0, retDrop.cardDropsPool.Count)];
        }

        // Did not roll any drops
        return null;
    }
}