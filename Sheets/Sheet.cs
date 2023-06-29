using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sheet is a collection of slots which usually form one advanced building
// Sheet is *certainly* not a child of Card, but SO_Sheet is a child of SO_Card. This enables integrating into Recipes.
public abstract class Sheet : MonoBehaviour {
    public SO_Sheet sheetData;
    
    public new string name;
    public int value;
    public Sprite image;

    [SerializeField] protected List<Slot> mainSlots;
    [SerializeField] protected Slot fuelSlot;

    public void Start() {
        name = sheetData.name;
        value = sheetData.value;
        image = sheetData.image;

        gameObject.name = name;
    }

    public abstract void PlaceMain(Slot slot);
    public abstract void PlaceSecondary();
    public abstract void TryCraft();
}
