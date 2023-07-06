using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelSheet : Sheet
{
    new void Start() {
        base.Start();

        // EventManager.Subscribe(fuelSlot.gameObject, EventID.SlotPlaced, PlaceSecondary);
        // EventManager.Subscribe(fuelSlot.gameObject, EventID.SlotPickedUp, PlaceSecondary);
    }

    public override void PlaceMain(Slot slot) {
        
    }

    public override void PlaceSecondary() {
        
    }

    public override void TryCraft() {
        if (fuelSlot.IsEmpty()) return;
        
        // Place ghost copy of building + fuel card on each main slot for crafting
        foreach (Slot mainSlot in mainSlots) {
            Card buildingCopy = new Card();
            Card fuelCopy = new Card();
            buildingCopy.name = this.name;
            fuelCopy.name = fuelSlot.Card.name;
            mainSlot.Stack.Place(buildingCopy);
            mainSlot.Stack.Place(fuelCopy);
            
            // Clean up ghost copies if they still exist
            // TODO: might need to wait 1 frame here or skip DoCraftTIme ocrooutine if this no work
            mainSlot.Stack.ExtractWithoutCraft(fuelCopy);
            mainSlot.Stack.ExtractWithoutCraft(buildingCopy);
        }
    }
}
