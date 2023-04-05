using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    
    public GameObject a_baseCard;
    public static GameObject baseCard;

    public List<SO_Card> a_cardSOs = new List<SO_Card>();
    public static List<SO_Card> cardSOs = new List<SO_Card>();

    void Awake() {
        baseCard = a_baseCard;
        cardSOs = a_cardSOs;
    }

    public static GameObject CreateCardFromMaterials(List<string> materials) {
        SO_Card cSO = LookupRecipe(materials);
        if (cSO == null) { return null; }

        return CreateCard(cSO);
    }

    public static GameObject CreateCard(SO_Card cSO) {
        GameObject o = Instantiate(baseCard);
        Card c = o.GetComponent<Card>();
        c.cardData = cSO;

        if (cSO is SO_Food fSO) {
            Destroy(c);
            Food f = o.AddComponent<Food>();
            f.foodData = fSO;
        } else if (cSO is SO_Villager vSO) {
            Destroy(c);
            Villager f = o.AddComponent<Villager>();
            f.villagerData = vSO;
        }

        return o;
    }

    public static SO_Card LookupRecipe(List<string> materials) {
        string[] materialsArr = materials.OrderBy((x => x)).ToArray();
        foreach (var cSO in cardSOs) {
            if (cSO.recipe.materials.Length == 0) {
                continue;
            }
            
            if (materialsArr.SequenceEqual(cSO.recipe.materials.OrderBy(x => x))) {
                print("craft matched: " + cSO.name);
                return cSO;     // cannot return SO_Food...
            }
        }

        return null;
    }
}
