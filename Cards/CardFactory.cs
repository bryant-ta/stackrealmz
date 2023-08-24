using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
    public GameObject spellBase;
    public static GameObject _spellBase;
    public GameObject cardPackBase;
    public static GameObject _cardPackBase;
    
    public GameObject sheetBase;
    public static GameObject _sheetBase;
    
    public GameObject stackBase;
    public static GameObject _stackBase;

    public List<Recipe> recipes;
    public static List<Recipe> _recipes = new List<Recipe>();

    void Awake() {
        _cardBase = cardBase;
        _foodBase = foodBase;
        _animalBase = animalBase;
        _cardPackBase = cardPackBase;

        _sheetBase = sheetBase;
        
        _stackBase = stackBase;
        
        LoadRecipes();
        recipes = _recipes;
    }

    /*
     * CreateStack creates an empty stack. If a SO_Card is provided, CreateStack will add SO_Card to the stack as a Card.
     * Typical Usage:
            Stack s = CardFactory.CreateStack(stockCardData);
            s.transform.position = CalculateCardPosition();
     */
    public static Stack CreateStack(Vector3 pos, SO_Card cSO = null) {
        // TODO: remove this and convert all to regular 2D (no rotation) when change is FOR SURE
        Quaternion alwaysRotate = Quaternion.Euler(90, 0, 0);
        
        Stack s = Instantiate(_stackBase, pos, Quaternion.identity).GetComponent<Stack>();
        if (cSO == null) {
            return s;
        }
        
        // possibly use interface to fix this strangeness
        if (cSO is SO_Food fSO) {
            Food f = Instantiate(_foodBase, pos, alwaysRotate).GetComponent<Food>();
            f.foodData = fSO;
            s.Place(f);
        } else if (cSO is SO_Animal aSO) {
            Animal a = Instantiate(_animalBase, pos, alwaysRotate).GetComponent<Animal>();
            a.animalData = aSO;
            s.Place(a);
        } else if (cSO is SO_Spell sSO) {
            Spell sp = Instantiate(_spellBase, pos, alwaysRotate).GetComponent<Spell>();
            sp.spellData = sSO;
            s.Place(sp);
        } else if (cSO is SO_CardPack cpSO) {
            CardPack cp = Instantiate(_cardPackBase, pos, alwaysRotate).GetComponent<CardPack>();
            cp.cardPackData = cpSO;
            s.Place(cp);
        } else {    // is just SO_Card
            Card c = Instantiate(_cardBase, pos, alwaysRotate).GetComponent<Card>();
            c.cardData = cSO;
            s.Place(c);
        }

        return s;
    }

    public static Spell CreateBaseSpell(Vector3 pos) {
        // TODO: remove this and convert all to regular 2D (no rotation) when change is FOR SURE
        Quaternion alwaysRotate = Quaternion.Euler(90, 0, 0);
        
        Stack s = Instantiate(_stackBase, pos, Quaternion.identity).GetComponent<Stack>();
        
        Spell sp = Instantiate(_spellBase, pos, alwaysRotate).GetComponent<Spell>();
        s.Place(sp);

        return sp;
    }

    public static Sheet CreateSheet(SO_Sheet sSO) {
        Sheet s = Instantiate(_sheetBase).GetComponent<Sheet>();
        s.sheetData = sSO;
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
    public static List<Recipe> LookupRecipesWithProduct(SO_Card cSO) {
        List<Recipe> matchingRecipes = new List<Recipe>();
        foreach (Recipe r in _recipes) {
            if (r.ContainsProduct(cSO)) {
                matchingRecipes.Add(r);
            }
        }
        return matchingRecipes;
    }
    public static List<Recipe> LookupRecipesWithMaterial(SO_Card cSO) {
        List<Recipe> matchingRecipes = new List<Recipe>();
        foreach (Recipe r in _recipes) {
            if (r.ContainsMaterial(cSO)) {
                matchingRecipes.Add(r);
            }
        }
        return matchingRecipes;
    }

    void LoadRecipes() {
        string[] assetPaths = AssetDatabase.FindAssets("t:SO_Recipe", new string[] { Constants.RecipeDataPath });

        _recipes.Clear();
        
        // Load recipes created in inspector
        foreach (Recipe recipe in recipes) {
            _recipes.Add(recipe);
        }
        
        // Load recipes from Assets
        for (int i = 0; i < assetPaths.Length; i++) {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetPaths[i]);
            SO_Recipe recipeData = AssetDatabase.LoadAssetAtPath<SO_Recipe>(assetPath);

            Recipe recipe = new Recipe {
                id = recipeData.id,
                products = recipeData.products,
                materials = recipeData.materials,
                reusableMaterials = recipeData.reusableMaterials,
                craftTime = recipeData.craftTime,
                randomProducts = recipeData.randomProducts,
                numRandomProducts = recipeData.numRandomProducts,
            };
            _recipes.Add(recipe);
        }
    }
    
    /*
     * Usage:
     * - Each Drop.cardDropPool is rolled at its percentage, favoring the lowest percentage successful roll
     * - Percentage between Drops in one drop table should never match, put same percentage cards in one cardDropPool
     * - Randomly selects card from cardDropPool
     */
    public static SO_Card RollDrop(List<Drop> dropTable) {
        // Roll eligible drops
        int roll = Random.Range(0, 101);
        List<Drop> possibleDrops = new List<Drop>();

        Drop retDrop = new Drop() { percentage = 100 };
        foreach (Drop drop in dropTable) {
            if (roll <= drop.percentage && roll <= retDrop.percentage) {
                retDrop = drop;
            }
        }
        
        // Randomly choose drop from Drop card pool
        return retDrop.cardDropsPool[Random.Range(0, retDrop.cardDropsPool.Count)];
    }
}