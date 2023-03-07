using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeList : MonoBehaviour {
    // public static Recipes Instance {
    //     get {
    //         if (_instance == null) {
    //             Instance = JsonReader();
    //         }
    //
    //         return _instance;
    //     }
    //     private set => _instance = value;
    // }
    // static Recipes _instance;
    //
    // public TextAsset jsonFileAsset;
    // static TextAsset jsonFile;
    //
    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    // static void Init() {
    //     _instance = null;
    // }
    //
    // void Awake() {
    //     jsonFile = jsonFileAsset;
    // }
    //
    // static Recipes JsonReader() {
    //     return JsonUtility.FromJson<Recipes>(jsonFile.text);
    // }
    //
    // public static Recipe LookupRecipe(List<string> materials) {
    //     string[] materialsArr = materials.OrderBy((x => x)).ToArray();
    //     foreach (Recipe r in Instance.recipes) {
    //         if (materialsArr.SequenceEqual(r.materials.OrderBy(x => x))) {
    //             print("crafted!! " + r.name);
    //             return r;
    //         }
    //     }
    //
    //     return null;
    // }
}



[Serializable]
public class Recipes {
    public Recipe[] recipes;
}
